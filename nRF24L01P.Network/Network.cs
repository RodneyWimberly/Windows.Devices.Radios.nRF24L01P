using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Radios.nRF24L01P.Common;
using Windows.Devices.Radios.nRF24L01P.Common.Extensions;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Network.Enums;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class Network : INetwork
    {
        private static readonly int _maxFramePayloadSize;

        private readonly ILog _logger;
        private readonly Stopwatch _transmitTime;
        private readonly IRadio _radio;
        private byte _multiCastLevel;

        private byte _frameSize;
        private readonly Queue<INetworkFrame> _frameQueue;
        private readonly IDictionary<ushort, INetworkFrame> _frameFragmentsCache;

        public static uint NumberOfFails { get; private set; }
        public static uint NumberOfSuccessful { get; private set; }

        public INetworkAddressing Addressing { get; set; }
        public Queue<INetworkFrame> ExternalQueue { get; set; }
        public byte[] FrameBuffer { get; set; }
        public bool MultiCaseRelay { get; set; }
        public NetworkFlags NetworkFlags { get; set; }
        public bool ReturnSysMessages { get; set; }
        public uint RouteTimeout { get; set; }
        public uint TransmitTimeout { get; set; }

        static Network()
        {
            _maxFramePayloadSize = Configuration.MaxFrameSize - Configuration.FrameHeaderSize;
        }

        public Network(ILoggerFactoryAdapter loggerFactoryAdapter, IRadio radio)
        {
            _radio = radio;
            _logger = loggerFactoryAdapter.GetLogger(GetType());
            _transmitTime = new Stopwatch();
            _frameQueue = new Queue<INetworkFrame>();
            _frameFragmentsCache= new Dictionary<ushort, INetworkFrame>();
            Addressing = new NetworkAddressing(loggerFactoryAdapter);
        }

        public bool Available()
        {
            return _frameQueue.Any();
        }

        public void Begin(ushort nodeAddress)
        {
            Begin(Configuration.UseCurrentChannel, nodeAddress);
        }

        public void Begin(byte channel, ushort nodeAddress)
        {
            Addressing.NodeAddress = nodeAddress;

            if (channel != Configuration.UseCurrentChannel)
                _radio.Configuration.Channel = channel;
            _radio.RegisterContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe0 = false;
            _radio.RegisterContainer.EnableAutoAcknowledgementRegister.Save();

            _radio.Configuration.DynamicPayloadLengthEnabled = true;

            AutoRetransmitDelays retryVar = (AutoRetransmitDelays)((((nodeAddress % 6) + 1) * 2) + 3);
            _radio.Configuration.AutoRetransmitDelay = retryVar;
            _radio.Configuration.AutoRetransmitCount = 5;

            TransmitTimeout = 25;
            RouteTimeout = TransmitTimeout*3;

            byte i = 6;
            while (i-- > 0)
            {
                IReceivePipe receivePipe = _radio.ReceivePipes[i];
                receivePipe.Address = Addressing.PhysicalPipeAddress(nodeAddress, i);
                receivePipe.Enabled = true;
            }

            _radio.OperatingMode = OperatingModes.ReceiveMode;
        }

        public bool MultiCast(INetworkHeader networkHeader, byte[] message, ushort length, byte level)
        {
            networkHeader.ToNode = 0100;
            networkHeader.FromNode = Addressing.NodeAddress;
            return Write(networkHeader, message, length, Addressing.LevelToAddress(level));
        }

        public void MultiCastLevel(byte level)
        {
            _multiCastLevel = level;
            //_radio.OperatingMode = OperatingModes.StandBy; 
            IReceivePipe receivePipe = _radio.ReceivePipes[0];
            receivePipe.Address = Addressing.PhysicalPipeAddress(Addressing.LevelToAddress(level), 0);
            receivePipe.Enabled = true;
            // _radio.OperatingMode = OperatingModes.ReceiveMode;
        }

        public ushort Peek(ref INetworkHeader networkHeader)
        {
            if (!Available()) return 0;
            INetworkFrame networkFrame = _frameQueue.Peek();
            networkHeader = networkFrame.Header;
            return networkFrame.MessageSize;
        }

        public ushort Read(ref INetworkHeader networkHeader, ref byte[] message, ushort maxLength)
        {
            ushort bufferSize = 0;
            if (Available())
            {
                INetworkFrame networkFrame = _frameQueue.Dequeue();
                networkHeader = networkFrame.Header;
                bufferSize = networkFrame.MessageSize;
                
                if (maxLength > 0)
                {
                    bufferSize = Math.Min(bufferSize, maxLength);
                    Array.Resize(ref message, bufferSize);
                    Array.Copy(networkFrame.MessageBuffer, message, bufferSize);
                    _logger.TraceFormat("NET message size {0}\n", bufferSize);
                    _logger.TraceFormat("NET r message {0}\n", message.GetHexString());
                    _logger.TraceFormat("NET Received {0}\n", networkHeader);
                }
            }

            return bufferSize;
        }

        public byte Update()
        {
            byte pipeNumber,
                returnValue = 0;

            // If bypass is enabled, continue although incoming user data may be dropped
            // Allows system payloads to be read while user cache is full
            // Incoming Hold prevents data from being read from the radio, preventing incoming payloads from being acked
            if (!NetworkFlags.HasFlag(NetworkFlags.BypassHolds))
            {
                // TODO: Figure out math
                if (NetworkFlags.HasFlag(NetworkFlags.HoldIncoming)) //|| Configuration.MainBufferSize)
                {
                    if (!Available())
                        NetworkFlags &= ~NetworkFlags.HoldIncoming;
                    else
                        return returnValue;
                }
            }

            IRegisterContainer registers = _radio.RegisterContainer;
            while(_radio.ReceivePipes.FifoStatus != FifoStatus.Empty)
            {
                pipeNumber = _radio.ReceivePipes.ReceivePipeNumber;
                if ((_frameSize = _radio.Configuration.DynamicPayloadSize) < Configuration.FrameHeaderSize)
                {
                    Delay.WaitMilliseconds(10);
                    continue;
                }

                // Dump the payloads until we've gotten everything
                // Fetch the payload, and see if this was the last one.
                FrameBuffer = _radio.ReceivePipes[pipeNumber].ReadBuffer(_frameSize);

                // Read the beginning of the frame as the header
                INetworkHeader header = new NetworkHeader(FrameBuffer);

                _logger.TraceFormat("MAC Received on {0} {1}\r\n", pipeNumber, header);
                _logger.TraceFormat("FRG Rcv frame size {0}\r\n", _frameSize);
                _logger.TraceFormat("FRG Rcv frame {0}\r\n", FrameBuffer.GetHexString());

                if (!Addressing.IsValidAddress(header.ToNode))
                    continue;

                returnValue = header.Type;
                NetworkHeaderTypes headerType = (NetworkHeaderTypes)header.Type;

                // Is this for us?
                if (header.ToNode == Addressing.NodeAddress)
                {
                    if (headerType == NetworkHeaderTypes.NetworkPing)
                        continue;
                    if (headerType == NetworkHeaderTypes.NetworkAddrResponse)
                    {
                        if (Addressing.NodeAddress != Configuration.RequesterNode)
                        {
                            header.ToNode = Configuration.RequesterNode;
                            Write(header.ToNode, TransmitTypes.UserTransmitToPhysicalAddress);
                            Delay.WaitMilliseconds(10);
                            Write(header.ToNode, TransmitTypes.UserTransmitToPhysicalAddress);
                            continue;
                        }
                    }
                    if (headerType == NetworkHeaderTypes.NetworkReqAddress && Addressing.NodeAddress > 0)
                    {
                        header.FromNode = Addressing.NodeAddress;
                        header.ToNode = 0;
                        Write(header.ToNode, TransmitTypes.TransmitNormal);
                        continue;
                    }
                    if ((ReturnSysMessages && (byte)headerType > 127) || headerType == NetworkHeaderTypes.NetworkAck)
                    {
                        _logger.TraceFormat("MAC: System payload rcvd {0}\r\n", returnValue);
                        if (headerType != NetworkHeaderTypes.NetworkFirstFragment &&
                            headerType != NetworkHeaderTypes.NetworkMoreFragments &&
                            headerType != NetworkHeaderTypes.NetworkMoreFragmentsNack &&
                            headerType != NetworkHeaderTypes.ExternalDataType &&
                            headerType != NetworkHeaderTypes.NetworkLastFragment)
                            return returnValue;
                    }
                    // External data received
                    if (Enqueue(header) == 2)
                    {
                        _logger.Trace("ret ext\r\n");
                        returnValue = (byte)NetworkHeaderTypes.ExternalDataType;
                        return returnValue;
                    }
                }
                else
                {
                    if (header.ToNode == Configuration.MulticastNode)
                    {
                        if (headerType == NetworkHeaderTypes.NetworkPoll &&
                            Addressing.NodeAddress != Configuration.RequesterNode)
                        {
                            if (!NetworkFlags.HasFlag(NetworkFlags.NoPoll))
                            {
                                header.ToNode = header.FromNode;
                                header.FromNode = Addressing.NodeAddress;
                                Delay.WaitMilliseconds(Addressing.ParentPipe);
                                Write(header.ToNode, TransmitTypes.UserTransmitToPhysicalAddress);
                            }
                            continue;
                        }
                        byte val = Enqueue(header);
                        if (MultiCaseRelay)
                        {
                            _logger.TraceFormat("MAC: FWD multicast frame from {0} to level {1}\r\n", header.FromNode, _multiCastLevel + 1);
                            Write((ushort)(Addressing.LevelToAddress(_multiCastLevel) << 3), TransmitTypes.UserTransmitMulticast);
                        }
                        //External data received
                        if (val == 2)
                        {
                            _logger.Trace("ret ext multicast\r\n");
                            returnValue = (byte)NetworkHeaderTypes.ExternalDataType;
                            return returnValue;
                        }
                    }
                    else
                        //Send it on, indicate it is a routed payload
                        Write(header.ToNode, TransmitTypes.TransmitRouted);
                }
            }

            return returnValue;
        }

        public bool Write(INetworkHeader networkHeader, byte[] message, ushort length)
        {
            return Write(networkHeader, message, length, Configuration.WriteDirectUnicast);
        }

        public bool Write(INetworkHeader networkHeader, byte[] message, ushort length, ushort writeDirect)
        {
            // Allows time for requests (RF24Mesh) to get through between failed writes on busy nodes
            while (_transmitTime.ElapsedMilliseconds < 25)
                if (Update() > (byte)NetworkHeaderTypes.MaxUserDefinedHeaderType) break;

            // Normal Write (Un-Fragmented)
            if (length <= _maxFramePayloadSize)
            {
                _frameSize = (byte)(length + Configuration.FrameHeaderSize);
                if (_Write(networkHeader, message, length, writeDirect))
                    return true;
                _transmitTime.Restart();
                return false;
            }

            // Check payload size
            if (length > Configuration.MaxPayloadSize)
            {
                _logger.DebugFormat("NET write message failed. Given 'len'{0} is bigger than the MAX Payload size {1}\n\r", length, Configuration.MaxPayloadSize);
                return false;
            }

            // Divide the message payload into chunks of _maxFramePayloadSize
            byte messageCount = 0,
                fragmentId = (byte)((length % _maxFramePayloadSize != 0 ? 1 : 0) + (length / _maxFramePayloadSize));

            _logger.TraceFormat("FRG Total message fragments {0)\r\n", fragmentId);
            if (networkHeader.FromNode != Configuration.MulticastNode)
            {
                NetworkFlags |= NetworkFlags.FastFrag;
                _radio.OperatingMode = OperatingModes.StandBy;
            }

            byte retriesPerFragment = 0,
                type = networkHeader.Type;
            bool ok = false;
            while (fragmentId > 0)
            {
                //Copy and fill out the header
                networkHeader.Reserved = fragmentId;
                if (fragmentId == 1)
                {
                    networkHeader.Type = (byte)NetworkHeaderTypes.NetworkLastFragment;
                    networkHeader.Reserved = type;
                }
                else
                {
                    if (messageCount == 0)
                        networkHeader.Type = (byte) NetworkHeaderTypes.NetworkFirstFragment;
                    else
                        networkHeader.Type = (byte)NetworkHeaderTypes.NetworkMoreFragments;
                }

                ushort offset = (ushort)(messageCount * _maxFramePayloadSize);
                ushort fragmentLength = (ushort)Math.Min(length - offset, _maxFramePayloadSize);

                // Try to send the payload chunk with the copied header
                _frameSize = (byte)(Configuration.FrameHeaderSize + fragmentLength);
                // TODO: ((char *)message)+offset
                ok = _Write(networkHeader, message, fragmentLength, writeDirect);

                if (!ok)
                {
                    Delay.WaitMilliseconds(2);
                    ++retriesPerFragment;
                }
                else
                {
                    retriesPerFragment = 0;
                    fragmentId--;
                    messageCount++;
                }

                if (!ok && retriesPerFragment >= 3)
                {
                    _logger.DebugFormat("FRG TX with fragmentID '{0}' failed after {1} fragments. Abort.\r\n", fragmentId, messageCount);
                    break;
                }
                _logger.TraceFormat("FRG message transmission with fragmentID '{0}' successful.\r\n", fragmentId);
            }
            networkHeader.Type = type;
            if (NetworkFlags.HasFlag(NetworkFlags.FastFrag))
            {
                _radio.OperatingMode = OperatingModes.StandBy;
                _radio.OperatingMode = OperatingModes.ReceiveMode;
                _radio.TransmitPipe.AutoAcknowledgementEnabled = false;
            } 
            NetworkFlags &= ~NetworkFlags.FastFrag;
            if (!ok)
                return false;
            _logger.TraceFormat("FRG total message fragments sent {0}.\r\n", messageCount);
            if (fragmentId > 0)
            {
                _transmitTime.Restart();
                return false;
            }
            return true;
        }

        private bool Write(ushort toNode, TransmitTypes directTo)
        {
            bool ok = false,
                isAckType = FrameBuffer[Configuration.HeaderIndexType] > Configuration.HeaderTypeAcknowledgementStart
                    && FrameBuffer[Configuration.HeaderIndexType] < Configuration.HeaderTypeAcknowledgementEnd;

            // Throw it away if it's not a valid address
            if (!Addressing.IsValidAddress(toNode))
                return ok;

            //Load info into our conversion structure, and get the converted address info
            INodeAddressInfo nodeAddressInfo = new NodeAddressInfo { SendNode = toNode, MultiCast = false, SendPipe = (byte)directTo};
            nodeAddressInfo = Addressing.ConvertLogicalToPhysicalAddress(nodeAddressInfo);

            _logger.TraceFormat("MAC Sending to {0} via {1} on pipe {2}\n\r", toNode, nodeAddressInfo.SendNode, nodeAddressInfo.SendPipe);

            ok = WriteToPipe(nodeAddressInfo);
            if(!ok)
                _logger.DebugFormat("MAC Send fail to {0} via {1} on pipe {2}\n\r", toNode, nodeAddressInfo.SendNode, nodeAddressInfo.SendPipe);

            if (ok && isAckType
                && nodeAddressInfo.SendNode == toNode
                && directTo == TransmitTypes.TransmitRouted)
            {
                INetworkHeader header = new NetworkHeader(FrameBuffer);
                header.Type = (byte)NetworkHeaderTypes.NetworkAck;
                header.ToNode = header.FromNode;

                nodeAddressInfo.SendNode = header.FromNode;
                nodeAddressInfo.SendPipe = (byte)TransmitTypes.TransmitRouted;
                nodeAddressInfo.MultiCast = false;
                nodeAddressInfo = Addressing.ConvertLogicalToPhysicalAddress(nodeAddressInfo);

                //Write the data using the resulting physical address
                _frameSize = Configuration.FrameHeaderSize;
                WriteToPipe(nodeAddressInfo);

                _logger.TraceFormat("MAC: Route OK to {0} ACK sent to {1}\r\n", toNode, header.FromNode);
            }

            if (ok && isAckType
                && nodeAddressInfo.SendNode != toNode
                && (directTo == TransmitTypes.TransmitNormal || directTo == TransmitTypes.UserTransmitToLogicalAddress))
            {
                if (NetworkFlags.HasFlag(NetworkFlags.FastFrag))
                {
                    _radio.OperatingMode = OperatingModes.StandBy;
                    NetworkFlags &= ~NetworkFlags.FastFrag;
                    _radio.TransmitPipe.AutoAcknowledgementEnabled = false;
                }
                _radio.OperatingMode = OperatingModes.ReceiveMode;
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (Update() != (byte) NetworkHeaderTypes.NetworkAck)
                {
                    if (stopwatch.ElapsedMilliseconds > RouteTimeout)
                    {
                        ok = false;
                        break;
                    }
                }
            }

            if(!NetworkFlags.HasFlag(NetworkFlags.FastFrag))
                _radio.OperatingMode = OperatingModes.ReceiveMode;

            if (ok)
                NumberOfSuccessful++;
            else
                NumberOfFails++;

            return ok;
        }

        private bool _Write(INetworkHeader networkHeader, byte[] message, uint length, ushort writeDirect)
        {
            // Fill out the header
            networkHeader.FromNode = Addressing.NodeAddress;

            // Build the full frame to send
            Array.Copy(networkHeader.ToBytes(), FrameBuffer, Configuration.FrameHeaderSize);
            _logger.TraceFormat("NET Sending {0}\r\n", networkHeader);

            if (length > 0)
            {
                Array.Copy(message, 0, FrameBuffer, Configuration.FrameHeaderSize, (int)length);
                _logger.TraceFormat("NET message {0}\r\n", message.GetHexString());
            }

            if (writeDirect != Configuration.WriteDirectUnicast)
            {
                TransmitTypes transmitType = TransmitTypes.UserTransmitToLogicalAddress;
                if (networkHeader.ToNode == Configuration.MulticastNode)
                    transmitType = TransmitTypes.UserTransmitMulticast;
                else if (networkHeader.ToNode == writeDirect)
                    transmitType = TransmitTypes.UserTransmitToPhysicalAddress;

                return Write(writeDirect, transmitType);
            }

            return Write(networkHeader.ToNode, TransmitTypes.TransmitNormal);
        }

        private bool WriteToPipe(INodeAddressInfo nodeAddressInfo)
        {
            bool ok = false;
            byte[] outPipe = Addressing.PhysicalPipeAddress(nodeAddressInfo.SendNode, nodeAddressInfo.SendPipe);
            if (!NetworkFlags.HasFlag(NetworkFlags.FastFrag))
                _radio.OperatingMode = OperatingModes.StandBy;
            _radio.TransmitPipe.AutoAcknowledgementEnabled = !nodeAddressInfo.MultiCast;
            _radio.TransmitPipe.Address = outPipe;
            _radio.OperatingMode = OperatingModes.TransmitMode;
            ok = _radio.TransmitPipe.Write(FrameBuffer, nodeAddressInfo.MultiCast);

            if (!NetworkFlags.HasFlag(NetworkFlags.FastFrag))
            {
                _radio.OperatingMode = OperatingModes.StandBy;
                _radio.TransmitPipe.AutoAcknowledgementEnabled = false;
            }

            _logger.TraceFormat("MAC Sent on {0} {1}\r\n", BitConverter.ToInt32(outPipe, 0), ok ? "ok" : "failed");

            return ok;
        }

        private byte Enqueue(INetworkHeader networkHeader)
        {
            byte result = 0;
            INetworkFrame networkFrame = new NetworkFrame
            {
                Header = networkHeader,
                MessageSize = Configuration.FrameHeaderSize
            };
            Array.Copy(FrameBuffer, Configuration.FrameHeaderSize, networkFrame.MessageBuffer, 0, _frameSize - Configuration.FrameHeaderSize);
            bool isFragment = networkHeader.Type == (byte)NetworkHeaderTypes.NetworkFirstFragment ||
                              networkHeader.Type == (byte)NetworkHeaderTypes.NetworkMoreFragments ||
                              networkHeader.Type == (byte)NetworkHeaderTypes.NetworkLastFragment ||
                              networkHeader.Type == (byte)NetworkHeaderTypes.NetworkMoreFragmentsNack;
            
            // This is sent to itself
            if (networkFrame.Header.FromNode == Addressing.NodeAddress)
            {
                if (isFragment)
                {
                    _logger.Warn("Cannot enqueue multi-payload frames to self");
                    result = 0;
                }
                else
                {
                    _frameQueue.Enqueue(networkFrame);
                    result = 1;
                }
            }
            else
            {
                if (isFragment)
                {
                    // The received frame contains the a fragmented payload
                    // Set the more fragments flag to indicate a fragmented frame
                    _logger.TraceFormat("FRG Payload type {0} of size {1} Bytes with fragmentID '{2}' received.",
                        networkFrame.Header.Type, networkFrame.MessageSize, networkFrame.Header.Reserved);

                    // Append payload
                    result = (byte) (AppendFragmentToFrame(networkFrame) ? 1 : 0);

                    // The Header.Reserved contains the actual Header.Type on the last fragment
                    if (result == 1 && networkFrame.Header.Type == (byte) NetworkHeaderTypes.NetworkLastFragment)
                    {
                        _logger.Trace("FRG Last fragment received.");
                        _logger.TraceFormat("NET Enqueue assembled frame @{0}", _frameQueue.Count);

                        INetworkFrame cacheFrame = _frameFragmentsCache[networkFrame.Header.FromNode];
                        result = (byte) ((cacheFrame.Header.Type == (byte) NetworkHeaderTypes.ExternalDataType) ? 2 : 1);

                        // Load external payloads into a separate queue on linux
                        if (result == 2)
                            ExternalQueue.Enqueue(_frameFragmentsCache[networkFrame.Header.FromNode]);
                        else
                            _frameQueue.Enqueue(_frameFragmentsCache[networkFrame.Header.FromNode]);
                        _frameFragmentsCache.Remove(networkFrame.Header.FromNode);
                    }

                }
                else
                {
                    // This is not a fragmented payload but a whole frame.
                    _logger.TraceFormat("NET Enqueue @{0}", _frameQueue.Count);
                    // Copy the current frame into the frame queue
                    result = (byte) ((networkFrame.Header.Type == (byte) NetworkHeaderTypes.ExternalDataType) ? 2 : 1);
                    //Load external payloads into a separate queue on linux
                    if (result == 2)
                        ExternalQueue.Enqueue(networkFrame);
                    else
                        _frameQueue.Enqueue(networkFrame);
                }
            }

            if (result == 1)
                _logger.Debug("Enqueue succeeded");
            else
                _logger.Warn("Enqueue failed");

            return result;
        }

        private bool AppendFragmentToFrame(INetworkFrame frame)
        {

            if (frame.Header.Type == (byte)NetworkHeaderTypes.NetworkFirstFragment)
            {
                if (_frameFragmentsCache.Any(f => f.Value.Header.FromNode == frame.Header.FromNode))
                {
                    //Already rcvd first fragment
                    if (_frameFragmentsCache[frame.Header.FromNode].Header.Id == frame.Header.Id)
                        return false;
                }
                if (frame.Header.Reserved > (Configuration.MaxPayloadSize / 24) + 1)
                {
                    _logger.WarnFormat("FRG Too many fragments in payload {0}, dropping...\r\n", frame.Header.Reserved); 
                    // If there are more fragments than we can possibly handle, return
                    return false;
                }
                _frameFragmentsCache[frame.Header.FromNode] = frame;
                return true;
            }
            else if (frame.Header.Type == (byte)NetworkHeaderTypes.NetworkMoreFragments || 
                    frame.Header.Type == (byte)NetworkHeaderTypes.NetworkMoreFragmentsNack)
            {

                if (!_frameFragmentsCache.Any(f => f.Value.Header.FromNode == frame.Header.FromNode))
                    return false;
                INetworkFrame cacheFrame = _frameFragmentsCache[frame.Header.FromNode];
                if (cacheFrame.Header.Reserved - 1 == frame.Header.Reserved && cacheFrame.Header.Id == frame.Header.Id)
                {
                    // Cache the fragment
                    Array.Copy(frame.MessageBuffer, 0, cacheFrame.MessageBuffer, cacheFrame.MessageSize, frame.MessageSize);
                    cacheFrame.MessageSize += frame.MessageSize;  //Increment message size
                    cacheFrame.Header = frame.Header; //Update header
                    return true;
                }
                else
                {
                    _logger.WarnFormat("FRG Dropping fragment for frame with header id:{0}, out of order fragment(s).\r\n", frame.Header.Id);
                    return false;
                }
            }
            else if (frame.Header.Type == (byte)NetworkHeaderTypes.NetworkLastFragment)
            {
                // We have received the last fragment
                if (!_frameFragmentsCache.Any(f1 => f1.Value.Header.FromNode == frame.Header.FromNode))
                    return false;

                // Create a reference to the cached frame
                INetworkFrame cacheFrame = _frameFragmentsCache[frame.Header.FromNode];
                if (cacheFrame.MessageSize + frame.MessageSize > Configuration.MaxPayloadSize)
                {
                    _logger.WarnFormat("FRG Frame of size {0} plus enqueued frame of size {1} exceeds max payload size \r\n", frame.MessageSize, cacheFrame.MessageSize);
                    return false;
                }

                // Error checking for missed fragments and payload size
                if (cacheFrame.Header.Reserved - 1 != 1 || cacheFrame.Header.Id != frame.Header.Id)
                {
                    _logger.WarnFormat("FRG Duplicate or out of sequence frame {0}, expected {1}. Cleared.\r\n", frame.Header.Reserved, cacheFrame.Header.Reserved);
                    return false;
                }

                // The user specified Header.Type is sent with the last fragment in the reserved field
                frame.Header.Type = frame.Header.Reserved;
                frame.Header.Reserved = 1;

                // Append the received fragment to the cached frame
                Array.Copy(frame.MessageBuffer, 0, cacheFrame.MessageBuffer, cacheFrame.MessageSize, frame.MessageSize);
                cacheFrame.MessageSize += frame.MessageSize; 
                cacheFrame.Header = frame.Header; 
                return true;
            }

            return false;
        }
    }
}
