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
        private readonly ILog _logger;
        private readonly IRadio _radio;
        private readonly Stopwatch _transmitTime;
        private readonly int _maxFramePayloadSize;
        private readonly Queue<INetworkFrame> _frameQueue;
        private readonly IDictionary<ushort, INetworkFrame> _frameFragmentsCache;
        private byte _frameSize;
        private byte[] _frameBuffer;

        public uint NumberOfFailures { get; private set; }
        public uint NumberOfSuccessful { get; private set; }

        public INetworkAddressing Addressing { get; set; }
        public Queue<INetworkFrame> ExternalQueue { get; set; }
        public NetworkFlags NetworkFlags { get; set; }
        public bool ReturnSysMessages { get; set; }
        public uint RouteTimeout { get; set; }
        public uint TransmitTimeout { get; set; }
        public bool MultiCaseRelay { get; set; }

        private byte _multiCastLevel;
        public byte MultiCastLevel
        {
            get { return _multiCastLevel; }
            set
            {
                _multiCastLevel = value;
                _radio.OperatingMode = OperatingModes.StandBy;
                IReceivePipe receivePipe = _radio.ReceivePipes[0];
                receivePipe.Address = Addressing.PhysicalPipeAddress(Addressing.LevelToAddress(value), 0);
                receivePipe.Enabled = true;
                _radio.OperatingMode = OperatingModes.ReceiveMode;
            }
        }

        public Network(ILoggerFactoryAdapter loggerFactoryAdapter, IRadio radio)
        {
            _radio = radio;
            _logger = loggerFactoryAdapter.GetLogger(GetType());
            _maxFramePayloadSize = Configuration.MaxFrameSize - Configuration.NetworkHeaderSize;
            _transmitTime = new Stopwatch();
            _frameQueue = new Queue<INetworkFrame>();
            _frameFragmentsCache= new Dictionary<ushort, INetworkFrame>();
            Addressing = new NetworkAddressing(loggerFactoryAdapter);
            TransmitTimeout = 25;
            RouteTimeout = TransmitTimeout * 3;
        }

        public void Begin(ushort nodeAddress)
        {
            Begin(Configuration.UseCurrentChannel, nodeAddress);
        }

        public void Begin(byte channel, ushort nodeAddress)
        {
            Addressing.NodeAddress = nodeAddress;

            // Set up the radio the way we want it to look
            if (channel != Configuration.UseCurrentChannel)
                _radio.Configuration.Channel = channel;
            _radio.RegisterContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe0 = false;
            _radio.RegisterContainer.EnableAutoAcknowledgementRegister.Save();
            _radio.Configuration.DynamicPayloadLengthEnabled = true;

            // Use different retry periods to reduce data collisions
            AutoRetransmitDelays retryVar = (AutoRetransmitDelays)((((nodeAddress % 6) + 1) * 2) + 3);
            _radio.Configuration.AutoRetransmitDelay = retryVar;
            _radio.Configuration.AutoRetransmitCount = 5;

            byte i = 6;
            while (i-- > 0)
            {
                IReceivePipe receivePipe = _radio.ReceivePipes[i];
                receivePipe.Address = Addressing.PhysicalPipeAddress(nodeAddress, i);
                receivePipe.Enabled = true;
            }

            _radio.OperatingMode = OperatingModes.ReceiveMode;
        }

        public byte Update()
        {
            byte pipeNumber,
                returnValue = 0;

            IRegisterContainer registers = _radio.RegisterContainer;
            while (_radio.ReceivePipes.FifoStatus != FifoStatus.Empty)
            {
                pipeNumber = _radio.ReceivePipes.ReceivePipeNumber;
                if ((_frameSize = _radio.Configuration.DynamicPayloadSize) < Configuration.NetworkHeaderSize)
                {
                    Delay.WaitMilliseconds(10);
                    continue;
                }

                // Dump the payloads until we've gotten everything
                // Fetch the payload, and see if this was the last one.
                _frameBuffer = _radio.ReceivePipes[pipeNumber].ReadBuffer(_frameSize);

                // Read the beginning of the frame as the header
                INetworkHeader header = new NetworkHeader(_frameBuffer);

                _logger.DebugFormat("ROUTING: Received on {0} {1}", pipeNumber, header);
                _logger.TraceFormat("FRAGMENTATION: Received frame size {0}", _frameSize);
                _logger.TraceFormat("FRAGMENTATION: Received frame {0}", _frameBuffer.GetHexString());

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
                            Send(header.ToNode, TransmitTypes.UserTransmitToPhysicalAddress);
                            Delay.WaitMilliseconds(10);
                            Send(header.ToNode, TransmitTypes.UserTransmitToPhysicalAddress);
                            continue;
                        }
                    }
                    if (headerType == NetworkHeaderTypes.NetworkReqAddress && Addressing.NodeAddress > 0)
                    {
                        header.FromNode = Addressing.NodeAddress;
                        header.ToNode = 0;
                        Send(header.ToNode, TransmitTypes.TransmitNormal);
                        continue;
                    }
                    if ((ReturnSysMessages && (byte)headerType > 127) || headerType == NetworkHeaderTypes.NetworkAck)
                    {
                        _logger.DebugFormat("ROUTING: System payload received {0}", returnValue);
                        if (headerType != NetworkHeaderTypes.NetworkFirstFragment &&
                            headerType != NetworkHeaderTypes.NetworkMoreFragments &&
                            headerType != NetworkHeaderTypes.NetworkMoreFragmentsNack &&
                            headerType != NetworkHeaderTypes.ExternalDataType &&
                            headerType != NetworkHeaderTypes.NetworkLastFragment)
                            return returnValue;
                    }
                    if (Enqueue(header) == 2)
                    {
                        _logger.Trace("ROUTING: Update returned external data unicast");
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
                                Send(header.ToNode, TransmitTypes.UserTransmitToPhysicalAddress);
                            }
                            continue;
                        }
                        byte val = Enqueue(header);
                        if (MultiCaseRelay)
                        {
                            _logger.DebugFormat("ROUTING: Forward multicast frame from {0} to level {1}", header.FromNode, _multiCastLevel + 1);
                            Send((ushort)(Addressing.LevelToAddress(_multiCastLevel) << 3), TransmitTypes.UserTransmitMulticast);
                        }
                        if (val == 2)
                        {
                            _logger.Trace("ROUTING: Update returned external data multicast");
                            returnValue = (byte)NetworkHeaderTypes.ExternalDataType;
                            return returnValue;
                        }
                    }
                    else
                        // Send it on, indicate it is a routed payload
                        Send(header.ToNode, TransmitTypes.TransmitRouted);
                }
            }

            return returnValue;
        }

        public bool Available()
        {
            return _frameQueue.Any();
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
                    _logger.TraceFormat("NETWORK: Read message size {0}", bufferSize);
                    _logger.TraceFormat("NETWORK: Read message {0}", message.GetHexString());
                    _logger.TraceFormat("NETWORK: Read Header {0}", networkHeader);
                }
            }

            return bufferSize;
        }

        public bool MultiCast(INetworkHeader networkHeader, byte[] message, ushort length, byte level)
        {
            networkHeader.ToNode = Configuration.MulticastNode;
            networkHeader.FromNode = Addressing.NodeAddress;
            return Write(networkHeader, message, length, Addressing.LevelToAddress(level));
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
                _frameSize = (byte)(length + Configuration.NetworkHeaderSize);
                if (WriteToFrameBufferAndSend(networkHeader, message, length, writeDirect))
                    return true;
                _transmitTime.Restart();
                return false;
            }

            if (length > Configuration.MaxPayloadSize)
            {
                _logger.WarnFormat("NETWORK: write message failed. Given 'length' {0} is bigger than the maximum payload size {1}", length, Configuration.MaxPayloadSize);
                return false;
            }

            // Divide the message payload into chunks of _maxFramePayloadSize
            byte messageCount = 0,
                fragmentId = (byte)((length % _maxFramePayloadSize != 0 ? 1 : 0) + length / _maxFramePayloadSize);

            _logger.TraceFormat("FRAGMENTATION: Total message fragments {0)", fragmentId);
            if (networkHeader.FromNode != Configuration.MulticastNode)
            {
                NetworkFlags |= NetworkFlags.FastFrag;
                _radio.OperatingMode = OperatingModes.StandBy;
            }

            byte retriesPerFragment = 0,
                type = networkHeader.Type;
            bool ok = false;

            // Loop through and send all fragments for this message
            while (fragmentId > 0)
            {
                if (fragmentId == 1)
                {
                    networkHeader.Type = (byte) NetworkHeaderTypes.NetworkLastFragment;
                    networkHeader.Reserved = type;
                }
                else
                {
                    networkHeader.Type =(byte)(messageCount == 0 ? NetworkHeaderTypes.NetworkFirstFragment : NetworkHeaderTypes.NetworkMoreFragments);
                    networkHeader.Reserved = fragmentId;
                }

                ushort offset = (ushort)(messageCount * _maxFramePayloadSize);
                ushort fragmentLength = (ushort)Math.Min(length - offset, _maxFramePayloadSize);
                byte[] messageFragrament = new byte[fragmentLength];
                Array.Copy(message, offset, messageFragrament, 0, fragmentLength);

                // Try to send the payload chunk with the copied header
                _frameSize = (byte)(Configuration.NetworkHeaderSize + fragmentLength);
                ok = WriteToFrameBufferAndSend(networkHeader, messageFragrament, fragmentLength, writeDirect);

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
                    _logger.WarnFormat("FRAGMENTATION: Transmit with fragmentId '{0}' failed after {1} fragments. Abort.", fragmentId, messageCount);
                    break;
                }
                _logger.DebugFormat("FRAGMENTATION: message transmission with fragmentId '{0}' successful.", fragmentId);
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
            _logger.DebugFormat("FRAGMENTATION: Total message fragments sent {0}.", messageCount);
            if (fragmentId > 0)
            {
                _transmitTime.Restart();
                return false;
            }
            return true;
        }

        private bool WriteToFrameBufferAndSend(INetworkHeader networkHeader, byte[] message, uint length, ushort writeDirect)
        {
            // Fill out the header
            networkHeader.FromNode = Addressing.NodeAddress;

            // Build the full frame to send
            Array.Copy(networkHeader.ToBytes(), _frameBuffer, Configuration.NetworkHeaderSize);
            _logger.TraceFormat("NETWORK: Sending {0}", networkHeader);

            if (length > 0)
            {
                Array.Copy(message, 0, _frameBuffer, Configuration.NetworkHeaderSize, (int)length);
                _logger.TraceFormat("NETWORK: message {0}", message.GetHexString());
            }

            if (writeDirect != Configuration.WriteDirectUnicast)
            {
                TransmitTypes transmitType = TransmitTypes.UserTransmitToLogicalAddress;
                if (networkHeader.ToNode == Configuration.MulticastNode)
                    transmitType = TransmitTypes.UserTransmitMulticast;
                else if (networkHeader.ToNode == writeDirect)
                    transmitType = TransmitTypes.UserTransmitToPhysicalAddress;

                return Send(writeDirect, transmitType);
            }

            return Send(networkHeader.ToNode, TransmitTypes.TransmitNormal);
        }

        private bool Send(ushort toNode, TransmitTypes directTo)
        {
            bool ok = false,
                isAckType = _frameBuffer[Configuration.HeaderIndexType] > Configuration.HeaderTypeAcknowledgementStart
                    && _frameBuffer[Configuration.HeaderIndexType] < Configuration.HeaderTypeAcknowledgementEnd;

            // Throw it away if it's not a valid address
            if (!Addressing.IsValidAddress(toNode))
                return ok;

            // Load info into our conversion structure, and get the converted address info
            INodeAddressInfo nodeAddressInfo = new NodeAddressInfo { SendNode = toNode, MultiCast = false, SendPipe = (byte)directTo};
            nodeAddressInfo = Addressing.ConvertLogicalToPhysicalAddress(nodeAddressInfo);

            _logger.DebugFormat("ROUTING: Sending to {0} via {1} on pipe {2}", toNode, nodeAddressInfo.SendNode, nodeAddressInfo.SendPipe);

            ok = WriteFrameBufferToPipe(nodeAddressInfo);
            if(!ok)
                _logger.WarnFormat("ROUTING: Send fail to {0} via {1} on pipe {2}", toNode, nodeAddressInfo.SendNode, nodeAddressInfo.SendPipe);

            if (ok && isAckType
                && nodeAddressInfo.SendNode == toNode
                && directTo == TransmitTypes.TransmitRouted)
            {
                INetworkHeader header = new NetworkHeader(_frameBuffer);
                header.Type = (byte)NetworkHeaderTypes.NetworkAck;
                header.ToNode = header.FromNode;

                nodeAddressInfo.SendNode = header.FromNode;
                nodeAddressInfo.SendPipe = (byte)TransmitTypes.TransmitRouted;
                nodeAddressInfo.MultiCast = false;
                nodeAddressInfo = Addressing.ConvertLogicalToPhysicalAddress(nodeAddressInfo);

                // Write the data using the resulting physical address
                _frameSize = Configuration.NetworkHeaderSize;
                WriteFrameBufferToPipe(nodeAddressInfo);

                _logger.DebugFormat("ROUTING: Route OK to {0} Acknowledgement sent to {1}", toNode, header.FromNode);
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
                NumberOfFailures++;

            return ok;
        }

        private bool WriteFrameBufferToPipe(INodeAddressInfo nodeAddressInfo)
        {
            bool ok = false;
            byte[] outPipe = Addressing.PhysicalPipeAddress(nodeAddressInfo.SendNode, nodeAddressInfo.SendPipe);
            if (!NetworkFlags.HasFlag(NetworkFlags.FastFrag))
                _radio.OperatingMode = OperatingModes.StandBy;
            _radio.TransmitPipe.AutoAcknowledgementEnabled = !nodeAddressInfo.MultiCast;
            _radio.TransmitPipe.Address = outPipe;
            _radio.OperatingMode = OperatingModes.TransmitMode;
            ok = _radio.TransmitPipe.Write(_frameBuffer, nodeAddressInfo.MultiCast);

            if (!NetworkFlags.HasFlag(NetworkFlags.FastFrag))
            {
                _radio.OperatingMode = OperatingModes.StandBy;
                _radio.TransmitPipe.AutoAcknowledgementEnabled = false;
            }

            _logger.DebugFormat("ROUTING: Sent on {0} {1}", BitConverter.ToInt32(outPipe, 0), ok ? "ok" : "failed");

            return ok;
        }

        private byte Enqueue(INetworkHeader networkHeader)
        {
            byte result = 0;
            INetworkFrame networkFrame = new NetworkFrame
            {
                Header = networkHeader,
                MessageSize = Configuration.NetworkHeaderSize
            };
            Array.Copy(_frameBuffer, Configuration.NetworkHeaderSize, networkFrame.MessageBuffer, 0, _frameSize - Configuration.NetworkHeaderSize);
            bool isFragment = networkHeader.Type == (byte)NetworkHeaderTypes.NetworkFirstFragment ||
                              networkHeader.Type == (byte)NetworkHeaderTypes.NetworkMoreFragments ||
                              networkHeader.Type == (byte)NetworkHeaderTypes.NetworkLastFragment ||
                              networkHeader.Type == (byte)NetworkHeaderTypes.NetworkMoreFragmentsNack;
            
            // This is sent to itself
            if (networkFrame.Header.FromNode == Addressing.NodeAddress)
            {
                if (isFragment)
                {
                    _logger.Warn("ROUTING: Cannot enqueue multi-payload frames to self");
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
                    _logger.TraceFormat("FRAGMENTATION: Payload type {0} of size {1} Bytes with fragmentId '{2}' received.",
                        networkFrame.Header.Type, networkFrame.MessageSize, networkFrame.Header.Reserved);

                    // Append payload
                    result = (byte) (AppendFragmentToFrame(networkFrame) ? 1 : 0);

                    // The Header.Reserved contains the actual Header.Type on the last fragment
                    if (result == 1 && networkFrame.Header.Type == (byte) NetworkHeaderTypes.NetworkLastFragment)
                    {
                        _logger.Trace("FRAGMENTATION: Last fragment received.");
                        _logger.TraceFormat("NETWORK: Enqueue assembled frame @{0}", _frameQueue.Count);

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
                    _logger.TraceFormat("NETWORK: Enqueue @{0}", _frameQueue.Count);
                    // Copy the current frame into the frame queue
                    result = (byte) ((networkFrame.Header.Type == (byte) NetworkHeaderTypes.ExternalDataType) ? 2 : 1);
                    // Load external payloads into a separate queue on linux
                    if (result == 2)
                        ExternalQueue.Enqueue(networkFrame);
                    else
                        _frameQueue.Enqueue(networkFrame);
                }
            }

            if (result == 1)
                _logger.Debug("NETWORK: Enqueue succeeded");
            else
                _logger.Warn("NETWORK: Enqueue failed");

            return result;
        }

        private bool AppendFragmentToFrame(INetworkFrame frame)
        {

            if (frame.Header.Type == (byte)NetworkHeaderTypes.NetworkFirstFragment)
            {
                if (_frameFragmentsCache.ContainsKey(frame.Header.FromNode))
                {
                    // Already rcvd first fragment
                    if (_frameFragmentsCache[frame.Header.FromNode].Header.Id == frame.Header.Id)
                        return false;
                }
                if (frame.Header.Reserved > (Configuration.MaxPayloadSize / 24) + 1)
                {
                    _logger.WarnFormat("FRAGMENTATION: Too many fragments in payload {0}, dropping...", frame.Header.Reserved); 
                    return false;
                }
                _frameFragmentsCache[frame.Header.FromNode] = frame;
                return true;
            }
            else if (frame.Header.Type == (byte)NetworkHeaderTypes.NetworkMoreFragments || 
                    frame.Header.Type == (byte)NetworkHeaderTypes.NetworkMoreFragmentsNack)
            {

                if (!_frameFragmentsCache.ContainsKey(frame.Header.FromNode))
                    return false;

                INetworkFrame cacheFrame = _frameFragmentsCache[frame.Header.FromNode];
                if (cacheFrame.Header.Reserved - 1 == frame.Header.Reserved && cacheFrame.Header.Id == frame.Header.Id)
                {
                    // Cache the fragment
                    Array.Copy(frame.MessageBuffer, 0, cacheFrame.MessageBuffer, cacheFrame.MessageSize, frame.MessageSize);
                    cacheFrame.MessageSize += frame.MessageSize;  
                    cacheFrame.Header = frame.Header;
                    return true;
                }
                else
                {
                    _logger.WarnFormat("FRAGMENTATION: Dropping fragment for frame with header id:{0}, out of order fragment(s).", frame.Header.Id);
                    return false;
                }
            }
            else if (frame.Header.Type == (byte)NetworkHeaderTypes.NetworkLastFragment)
            {
                // We have received the last fragment
                if (!_frameFragmentsCache.ContainsKey(frame.Header.FromNode))
                    return false;

                INetworkFrame cacheFrame = _frameFragmentsCache[frame.Header.FromNode];
                if (cacheFrame.MessageSize + frame.MessageSize > Configuration.MaxPayloadSize)
                {
                    _logger.WarnFormat("FRAGMENTATION: Frame of size {0} plus enqueued frame of size {1} exceeds max payload size ", frame.MessageSize, cacheFrame.MessageSize);
                    return false;
                }

                // Error checking for missed fragments and payload size
                if (cacheFrame.Header.Reserved - 1 != 1 || cacheFrame.Header.Id != frame.Header.Id)
                {
                    _logger.WarnFormat("FRAGMENTATION: Duplicate or out of sequence frame {0}, expected {1}. Cleared.", frame.Header.Reserved, cacheFrame.Header.Reserved);
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
