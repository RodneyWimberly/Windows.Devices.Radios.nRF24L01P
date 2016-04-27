using Common.Logging;
using System;
using Windows.Devices.Radios.nRF24L01P.Network.Enums;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class NetworkAddressing : INetworkAddressing
    {
        private byte _multiCastLevel;

        private ILog _logger;

        public ushort NodeAddress { get; set; }

        public ushort NodeMask { get; set; }

        public ushort ParentNode { get; set; }

        public byte ParentPipe { get; set; }

        public NetworkAddressing(ILoggerFactoryAdapter loggerFactoryAdapter)
        {
            _logger = loggerFactoryAdapter.GetLogger(GetType());
        }

        public ushort AddressOfPipe(ushort node, byte pipeNo)
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
            return (ushort)(node | (pipeNo << i));
        }

        public ulong PipeAddress(ushort node, byte pipeNo)
        {
           
            byte[] address_translation = { 0xc3, 0x3c, 0x33, 0xce, 0x3e, 0xe3, 0xec };
            ulong result = 0xCCCCCCCCCC;
            byte[] outData = BitConverter.GetBytes(result);

            // Translate the address to use our optimally chosen radio address bytes
            byte count = 1;
            ushort dec = node;

            while (dec > 0)
            {
	            if(pipeNo != 0 || node == 0)
                    outData[count] = address_translation[(dec%8)];
                // Convert our decimal values to octal, translate them to address bytes, and set our address
                dec /= 8;
                count++;
            }
    
	        if(pipeNo != 0 || node == 0)
                outData[0] = address_translation[pipeNo];
	        else
                outData[1] = address_translation[count-1];


            // Todo: Fix Tracing Here
            //IF_SERIAL_DEBUG(uint32_t*top = reinterpret_cast<uint32_t*>(out +1);
            //_logger.TraceFormat("{0}: NET Pipe {1} on node {2} has address {3}{4|\n\r"), millis(), pipeNo, node, *top, outData));

            return result;
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
            return (ushort)(node & childMask);

        }

        public bool IsDescendant(ushort node)
        {
            return (node & NodeMask) == NodeAddress;
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
                    _logger.DebugFormat("*** WARNING *** Invalid address {0}\n\r", node);
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
            ushort preConversionSendNode = ParentNode;

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

        public ushort Parent()
        {
            if (NodeAddress != 0)
                return ParentNode;
            else
                return unchecked((ushort)-1);
        }

        public void SetupAddress()
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
            ParentNode = (ushort)(NodeAddress & parentMask);

            // parent pipe is the part OUT of the mask
            ushort i = NodeAddress;
            ushort m = parentMask;
            while (m > 0)
            {
                i >>= 3;
                m >>= 3;
            }
            ParentPipe = (byte)i;

            _logger.TraceFormat("setup_address node={0} mask={1} parent={2} pipe={3}\n\r", NodeAddress, NodeMask, ParentNode, ParentPipe);
        }
    }
}
