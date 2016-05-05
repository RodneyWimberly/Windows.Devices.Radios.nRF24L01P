using System;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;
using Common.Logging;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class NetworkSync : INetworkSync
    {
        private readonly INetwork _network;
        private readonly ILog _logger;
        private byte[] _appData;
        private byte[] _internalData;
        private int _length;
        private ushort _toNode;

        public NetworkSync(ILoggerFactoryAdapter loggerFactoryAdapter, INetwork network)
        {
            _network = network;
            _logger = loggerFactoryAdapter.GetLogger(GetType());
            _appData = null;
            _internalData = null;
            _length = 0;
            _toNode = 0;
        }

        public void Begin(ushort toNode)
        {
            _toNode = toNode;
        }

        public void RegisterMe<T>(T data)
        {
            //_appData = BitConverter.GetBytes(data);
            _length = _appData.Length;
            _internalData = new byte[_length];
            Reset();
        }

        public void Reset()
        {
            Array.Copy(_appData, _internalData, _length);
        }

        public void Update()
        {
            // Pump the network
            _network.Update();

            // Look for changes to the data
            byte[] message = new byte[32];
            byte messageIndex = 0;
            byte index = 0;
            while (index < _length)
            {
                if (_appData != null && _internalData != null && _appData[index] != _internalData[index])
                {
                    // Compose a message with the deltas
                    message[messageIndex++] = (byte)(index + 1);
                    message[messageIndex++] = _appData[index];

                    // Update our internal view
                    _internalData[index] = _appData[index];
                }
                ++index;
            }
            // Zero out the remainder
            while (index++ < message.Length)
                message[messageIndex++] = 0;

            // If changes, send a message
            if (message[0] > 0)
            {
                // TODO: handle the case where this has to be broken into multiple messages
                INetworkHeader header = new NetworkHeader(_toNode, (byte)'S');
                _network.Write(header, message, (ushort)message.Length);
            }

            // Look for messages from the network. Is there anything ready for us?
            if (_network.Available())
            {
                // If so, take a look at it
                INetworkHeader header = null;
                _network.Peek(ref header);

                if(header.Type == 'S')
                {
                    _logger.Trace("SYNC: Received sync message");

                    _network.Read(ref header, ref message, (ushort)message.Length);
                    // Parse the message and update the vars
                    messageIndex = message[0];
                    index = 0;
                    while (messageIndex < message[0] + message.Length)
                    {
                        // A '0' in the first position means we are done
                        if (message[messageIndex] == 0)
                            break;
                        byte pos = message[messageIndex++ - 1];
                        byte val = message[messageIndex++];

                        _logger.TraceFormat("SYNC: Updated position {0} to value {1}", pos, val);

                        _appData[pos] = val;
                        _internalData[pos] = val;
                    }
                };
            }
        }
    }
}
