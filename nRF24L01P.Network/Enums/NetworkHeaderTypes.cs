namespace Windows.Devices.Radios.nRF24L01P.Network.Enums
{
    public enum NetworkHeaderTypes : byte
    {
        /*********************************************************************************/
        // ACK Response Types
        /*********************************************************************************/
        /**
        * **Reserved network message types**
        * The network will determine whether to automatically acknowledge payloads based on their general type <br>
        * **User types** (1-127) 1-64 will NOT be acknowledged <br>
        * **System types** (128-255) 192 through 255 will NOT be acknowledged<br>
        * System types can also contain message data.
        */

        /**
        * A NETWORK_ADDR_RESPONSE type is utilized to manually route custom messages containing a single RF24Network address
        * 
        * Used by RF24Mesh
        * 
        * If a node receives a message of this type that is directly addressed to it, it will read the included message, and forward the payload
        * on to the proper recipient. <br>
        * This allows nodes to forward multicast messages to the master node, receive a response, and forward it back to the requester.
        */
        MinUserDefinedHeaderType = 0,
        MaxUserDefinedHeaderType = 127,
        NetworkAddrResponse = 128,
        NetworkAddrConfirm = 129,
        /**
        * Messages of type NETWORK_PING will be dropped automatically by the recipient. A NETWORK_ACK or automatic radio-ack will indicate to the sender whether the 
        * payload was successful. The time it takes to successfully send a NETWORK_PING is the round-trip-time.
        */
        NetworkPing = 130,
        /**
         * External data types are used to define messages that will be passed to an external data system. This allows RF24Network to route and pass any type of data, such 
         * as TCP/IP frames, while still being able to utilize standard RF24Network messages etc.
         *
         * **Linux**
         * Linux devices (defined RF24_LINUX) will buffer all data types in the user cache. 
         *
         * **Arduino/AVR/Etc:** Data transmitted with the type set to EXTERNAL_DATA_TYPE will not be loaded into the user cache. <br>
         * External systems can extract external data using the following process, while internal data types are cached in the user buffer, and accessed using network.read() :
         * @code
         * uint8_t return_type = network.update(),
         * if(return_type == EXTERNAL_DATA_TYPE){
         *     uint16_t size = network.frag_ptr->message_size,	
         *     memcpy(&myDataBuffer,network.frag_ptr->message_buffer,network.frag_ptr->message_size),
         * }		
         * @endcode
         */
        ExternalDataType = 131,
        /**
         * Messages of this type designate the first of two or more message fragments, and will be re-assembled automatically.
         */
        NetworkFirstFragment = 148,
        /**
         * Messages of this type indicate a fragmented payload with two or more message fragments.
         */
        NetworkMoreFragments = 149,
        /**
         * Messages of this type indicate the last fragment in a sequence of message fragments.
         * Messages of this type do not receive a NETWORK_ACK
         */
        NetworkLastFragment = 150,

        
        /*********************************************************************************/
        // NO ACK Response Types
        /*********************************************************************************/
        NetworkAckRequest = 192,
        /**
         * Messages of this type are used internally, to signal the sender that a transmission has been completed.
         * RF24Network does not directly have a built-in transport layer protocol, so message delivery is not 100% guaranteed.<br>
         * Messages can be lost via corrupted dynamic payloads, or a NETWORK_ACK can fail, while the message was actually successful.
         * 
         * NETWORK_ACK messages can be utilized as a traffic/flow control mechanism, since transmitting nodes will be forced to wait until
         * the payload is transmitted across the network and acknowledged, before sending additional data. 
         *
         * In the event that the transmitting device will be waiting for a direct response, manually sent by the recipient, a NETWORK_ACK is not required. <br>
         * User messages utilizing a 'type' with a decimal value of 64 or less will not be acknowledged across the network via NETWORK_ACK messages.
         */
        NetworkAck = 193,
        /**
         * Used by RF24Mesh
         *
         * Messages of this type are used with multi-casting , to find active/available nodes.
         * Any node receiving a NETWORK_POLL sent to a multicast address will respond directly to the sender with a blank message, indicating the
         * address of the available node via the header.
         */
        NetworkPoll = 194,
        /**
         * Used by RF24Mesh
         *
         * Messages of this type are used to request information from the master node, generally via a unicast (direct) write.
         * Any (non-master) node receiving a message of this type will manually forward it to the master node using an normal network write.
         */
        NetworkReqAddress = 195,
        NetworkAddrLookup = 196,
        NetworkAddrRelease = 197,
        NetworkMoreFragmentsNack = 200
    }
}
