using Common.Logging;
using System;
using Windows.Devices.Radios.nRF24L01P.Network.Enums;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class NetworkAddressing : INetworkAddressing
    {
        private byte _multiCastLevel;

        private readonly ILog _logger;

        private ushort _nodeAddress;
        public ushort NodeAddress
        {
            get { return _nodeAddress; }
            set
            {
                if (!IsValidAddress(value))
                    throw new InvalidOperationException("Invalid Node Address!");
                _nodeAddress = value;
                SetupAddress();
            }
        }

        public ushort NodeMask { get; set; }

        private ushort _parentNodeAddress;
        public ushort ParentNodeAddress
        {
            get
            {
                if (NodeAddress != 0)
                    return _parentNodeAddress;
                else
                    return unchecked((ushort)-1);
            }
            set { _parentNodeAddress = value; }
        }

        public byte ParentPipe { get; set; }

        public NetworkAddressing(ILoggerFactoryAdapter loggerFactoryAdapter)
        {
            _logger = loggerFactoryAdapter.GetLogger(GetType());
        }

        public ushort LogicalPipeAddress(ushort node, byte pipeNo)
        {
            //Say this node is 013 (1011), mask is 077 or (00111111)
            //Say we want to use pipe 3 (11)
            //6 bits in node mask, so shift pipeNo 6 times left and | into address		
            ushort m = (ushort)(NodeMask >> 3);
            byte i = 0;

            while (m > 0)
            {      //While there are bits left in the node mask
                m >>= 1;     //Shift to the right
                i++;       //Count the # of increments
            }
            ushort pipeAddress = (ushort) (node | (pipeNo << i));
            _logger.DebugFormat("ROUTING: Pipe {0} on node {1} has a logical address of {3}.", pipeNo, node, pipeAddress);
            return pipeAddress;
        }

        public byte[] PhysicalPipeAddress(ushort node, byte pipeNo)
        {
           
            byte[] addressTranslation = { 0xc3, 0x3c, 0x33, 0xce, 0x3e, 0xe3, 0xec };
            const ulong mask = 0xCCCCCCCCCC;
            byte[] outData = BitConverter.GetBytes(mask);

            // Translate the address to use our optimally chosen radio address bytes
            byte count = 1;
            ushort dec = node;

            while (dec > 0)
            {
                // Convert our decimal values to octal, translate them to address bytes, and set our address
                if (pipeNo != 0 || node == 0)
                    outData[count] = addressTranslation[(dec%8)];
                dec /= 8;
                count++;
            }
    
	        if(pipeNo != 0 || node == 0)
                outData[0] = addressTranslation[pipeNo];
	        else
                outData[1] = addressTranslation[count-1];


            _logger.DebugFormat("ROUTING: Pipe {0} on node {1} has a physical address of {2}{3}", pipeNo, node, outData[1], outData[0]);

            return outData;
        }

        public ushort LevelToAddress(byte level)
        {
            ushort levelAddress = 1;
            if (level > 0)
                levelAddress = (ushort)(levelAddress << ((level - 1) * 3));
            else
                levelAddress = 0;
            return levelAddress;
        }

        public ushort DirectChildRouteTo(ushort node)
        {
            // Presumes that this is in fact a child!!
            ushort childMask = (ushort)((NodeMask << 3) | 7);
            ushort directRoute = (ushort)(node & childMask);
            _logger.TraceFormat("ROUTING: Node {0} has a direct child route via node {1}", node, directRoute);
            return directRoute;
        }

        public bool IsDescendant(ushort node)
        {
            bool isDescendant  = (node & NodeMask) == NodeAddress;
            _logger.TraceFormat("ROUTING: Node {0} is {1}a descendant of this node", node, isDescendant ? string.Empty : "not ");
            return isDescendant;
        }

        public bool IsDirectChild(ushort node)
        {
            bool result = false;

            // A direct child of ours has the same low numbers as us, and only
            // one higher number.
            //
            // e.g. node 0234 is a direct child of 034, and node 01234 is a
            // descendant but not a direct child

            // First, is it even a descendant?
            if (IsDescendant(node))
            {
                // Does it only have ONE more level than us?
                ushort childNodeMask = (ushort)((~NodeMask) << 3);
                result = (node & childNodeMask) == 0;
            }

            _logger.TraceFormat("ROUTING: Node {0} is {1}a direct child of this node", node, result ? string.Empty : "not ");

            return result;
        }

        public bool IsValidAddress(ushort node)
        {
            bool result = true;

            while (node > 0)
            {
                byte digit = (byte)(node & 7);
                if (digit < 0 || digit > 5)	//Allow our out of range multicast address
                {
                    result = false;
                    _logger.WarnFormat("ROUTING: {0} is an invalid node address.", node);
                    break;
                }
                node >>= 3;
            }

            return result;
        }

        public INodeAddressInfo ConvertLogicalToPhysicalAddress(INodeAddressInfo conversionInfo)
        {
            //We take in the to_node(logical) now, at the end of the function, output the send_node(physical) address, etc.
            //back to the original memory address that held the logical information.
            ushort toNode = conversionInfo.SendNode;
            byte directTo = conversionInfo.SendPipe;
            bool multiCast = conversionInfo.MultiCast;

            // Where do we send this?  By default, to our parent
            ushort preConversionSendNode = ParentNodeAddress;

            // On which pipe
            byte preConversionSendPipe = (byte)(ParentPipe % 5);

            if (directTo > (byte)TransmitTypes.TransmitRouted)
            {
                preConversionSendNode = toNode;
                multiCast = true;
                //if(*directTo == USER_TX_MULTICAST || *directTo == USER_TX_TO_PHYSICAL_ADDRESS){
                preConversionSendPipe = 0;
                //}	
            }
            // If the node is a direct child,
            else if (IsDirectChild(toNode))
            {
                // Send directly
                preConversionSendNode = toNode;
                // To its listening pipe
                preConversionSendPipe = 5;
            }
            // If the node is a child of a child talk on our child's listening pipe, and let the direct child relay it.
            else if (IsDescendant(toNode))
            {
                preConversionSendNode = DirectChildRouteTo(toNode);
                preConversionSendPipe = 5;
            }

            toNode = preConversionSendNode;
            directTo = preConversionSendPipe;

            conversionInfo.SendNode = toNode;
            conversionInfo.SendPipe = directTo;
            conversionInfo.MultiCast = multiCast;

            return conversionInfo;
        }

        private void SetupAddress()
        {
            // First, establish the node_mask
            ushort nodeMaskCheck = 0xFFFF;
            byte count = 0;

            while ((NodeAddress & nodeMaskCheck) > 0)
            {
                nodeMaskCheck <<= 3;
                count++;
            }
            _multiCastLevel = count;

            NodeMask = (ushort)~nodeMaskCheck;

            // parent mask is the next level down
            ushort parentMask = (ushort)(NodeMask >> 3);

            // parent node is the part IN the mask
            ParentNodeAddress = (ushort)(NodeAddress & parentMask);

            // parent pipe is the part OUT of the mask
            ushort i = NodeAddress;
            ushort m = parentMask;
            while (m > 0)
            {
                i >>= 3;
                m >>= 3;
            }
            ParentPipe = (byte)i;

            _logger.InfoFormat("ROUTING: This node was initialized with the following configuration node={0} mask={1} parent={2} pipe={3}", NodeAddress, NodeMask, ParentNodeAddress, ParentPipe);
        }
    }
}
