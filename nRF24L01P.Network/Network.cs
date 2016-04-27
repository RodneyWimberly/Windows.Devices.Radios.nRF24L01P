using Common.Logging;
using System;
using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Common.Extensions;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Network.Enums;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class Network : INetwork
    {
        private ILog _logger;

        private ILoggerFactoryAdapter _loggerFactoryAdapter;

        private uint transmitTime;

        private Radio _radio;

        private byte _multiCastLevel;

        private byte _frameSize;

        private static int _maxFramePayloadSize = Configuration.MaxFrameSize - Configuration.FrameHeaderSize;

        private byte[] _frameQueue;

        private byte _nextFrame;

        private INetworkFrame _fragmentationQueue;

        private byte [] _fragmentationQueueMessageBuffer;

        private static uint _numFails;

        private static uint _numOk;

        private bool Write(ushort toNode, TransmitTypes directTo)
        {
            return false;
        }

        private bool WriteToPipe(ushort node, byte pipe, bool multiCast)
        {
            bool ok = false;
            //ulong outPipe = NetworkAddressing.PipeAddress(node, pipe);
            //_radio.TransmitPipe.Address = BitConverter.GetBytes(outPipe).ReverseBytes();
            //_radio.Configuration
            //radio1.writeFast(frame_buffer, frame_size);
            //ok = radio1.txStandBy(txTimeout, multicast);

            //IF_SERIAL_DEBUG(printf_P(PSTR("%lu: MAC Sent on %lx %S\n\r"), millis(), (uint32_t) outPipe,
            //    ok ? PSTR("ok") : PSTR("failed")));

            return ok;
        }

        private byte Enqueue(INetworkHeader networkHeader)
        {
            return 0;
        }

        private bool _Write(INetworkHeader networkHeader, byte[] message, uint length, TransmitTypes writeDirect)
        {
            return false;
        }

        public INetworkAddressing NetworkAddressing { get; set; }

        public Queue<INetworkFrame> ExternalQueue { get; set; }

        public byte[] FrameBuffer { get; set; }

        public bool MultiCaseRelay { get; set; }

        public NetworkFlags NetworkFlags { get; set; }

        public bool ReturnSysMessages { get; set; }

        public uint RouteTimeout { get; set; }

        public uint TransmitTimeout { get; set; }

        public Network(ILoggerFactoryAdapter loggerFactoryAdapter)
        {
            _loggerFactoryAdapter = loggerFactoryAdapter;
            _logger = _loggerFactoryAdapter.GetLogger(GetType());
            NetworkAddressing = new NetworkAddressing(_loggerFactoryAdapter);
        }

        public bool Available()
        {
            return _nextFrame > _frameQueue.Length;
        }

        public void Begin(ushort nodeAddress)
        {
            Begin(Configuration.UseCurrentChannel, nodeAddress);
        }

        public void Begin(byte channel, ushort nodeAddress)
        {
            if (!NetworkAddressing.IsValidAddress(nodeAddress))
                return;
            NetworkAddressing.NodeAddress = nodeAddress;
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

            NetworkAddressing.SetupAddress();

            byte i = 6;
            while (i-- > 0)
            {
                IReceivePipe receivePipe = _radio.ReceivePipes[i];
                receivePipe.Address = BitConverter.GetBytes(NetworkAddressing.PipeAddress(nodeAddress, i)).ReverseBytes();
                receivePipe.Enabled = true;
            }

            _radio.OperatingMode = OperatingModes.ReceiveMode;
        }

        public void Falures(ref uint fails, ref uint ok)
        {
            fails = _numFails;
            ok = _numOk;
        }

        public bool MultiCast(INetworkHeader networkHeader, byte[] message, ushort length, byte level)
        {
            networkHeader.ToNode = 0100;
            networkHeader.FromNode = NetworkAddressing.NodeAddress;
            return Write(networkHeader, message, length, NetworkAddressing.LevelToAddress(level));
        }

        public void MultiCastLevel(byte level)
        {
            _multiCastLevel = level;
            //_radio.OperatingMode = OperatingModes.StandBy; 
            IReceivePipe receivePipe = _radio.ReceivePipes[0];
            receivePipe.Address = BitConverter.GetBytes( NetworkAddressing.PipeAddress(NetworkAddressing.LevelToAddress(level), 0)).ReverseBytes();
            receivePipe.Enabled = true;
            // _radio.OperatingMode = OperatingModes.ReceiveMode;
        }

        public ushort Peek(INetworkHeader networkHeader)
        {
            throw new NotImplementedException();
        }

        public ushort Read(INetworkHeader networkHeader, byte message, ushort maxLength)
        {
            throw new NotImplementedException();
        }

        public byte Update()
        {
            throw new NotImplementedException();
        }

        public bool Write(INetworkHeader networkHeader, byte[] message, ushort length)
        {
            return Write(networkHeader, message, length, 070);
        }

        public bool Write(INetworkHeader networkHeader, byte[] message, ushort length, ushort writeDirect)
        {
            throw new NotImplementedException();
        }
    }
}
