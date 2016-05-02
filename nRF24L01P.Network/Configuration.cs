namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public static class Configuration
    {
        /*********************************************************************************/
        // Sizes
        /*********************************************************************************/
        /** Size of individual radio frames*/
        public const int MaxFrameSize = 32; 

        /** Size of RF24Network frames - data*/
        public const int FrameHeaderSize = 10; 

        /** Use current radio channel when setting up the network*/
        public const int UseCurrentChannel = 255;

        /** The size of the main buffer. This is the user-cache, where incoming data is stored.
         Data is stored using Frames: Header (8-bytes) + Frame_Size (2-bytes) + Data (?-bytes)
         @note The MAX_PAYLOAD_SIZE is (MAIN_BUFFER_SIZE - 10), and the result must be divisible by 24. */
        public const int MainBufferSize = 144 + 10;

        /** Maximum size of fragmented network frames and fragmentation cache. This MUST BE divisible by 24.
         @note: Must be a multiple of 24.
         @note: If used with RF24Ethernet, this value is used to set the buffer sizes.*/
        public const int MaxPayloadSize = MainBufferSize - 10;

        /**This is the number of 24-byte payloads the network layer will cache for the user. 
         If using fragmentation, this number * 24 must be larger than MAX_PAYLOAD_SIZE |*/
        public const int NumUserPayloads = 5;

        /*********************************************************************************/
        // Nodes
        /*********************************************************************************/
        public const ushort RequesterNode = 04444;

        public const ushort MulticastNode = 0100;

        public const ushort WriteDirectUnicast = 0100;

        public const int HeaderIndexType = 6;
        public const int HeaderTypeAcknowledgementStart = 64;
        public const int HeaderTypeAcknowledgementEnd = 192;

        /*********************************************************************************/
        // Enabling
        /*********************************************************************************/
        /** Uncomment this option to enable use of dual radios */
        public const bool DualHeadRadio = false;

        /** Uncomment this option to enable sleep mode for AVR devices. (ATTiny,Uno, etc)*/
        public const bool SleepModeEnabled = false;

        public const bool MulticastEnabled = true;

        /** Fragmentation is enabled by default, and uses an additional 144 bytes of memory. */
        public const bool FragmentationEnabled = true;

        /** This option will disable user-caching of payloads entirely. Use with RF24Ethernet to reduce memory usage. (TCP/IP is an external data type, and not cached) |*/
        public const bool UserPayloadsEnabled = true;
        
        /** Enable tracking of success and failures for all transmissions, routed and user initiated */
        public const bool NetworkStatsEnabled = true;
       
        /** Enable dynamic payloads - If using different types of NRF24L01 modules, some may be incompatible when using this feature **/
        public const bool DynamicPayloadsEnabled = true;
        
    }
}
