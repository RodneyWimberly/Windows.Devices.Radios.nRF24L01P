/*
TMRh20 2014

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
version 2 as published by the Free Software Foundation.
*/

/*
 General Data Transfer Rate Test
This example demonstrates basic data transfer functionality with the
updated library. This example will display the transfer rates acheived using
the slower form of high-speed transfer using blocking-writes.
*/

#include <SPI.h>
#include "RF24.h"
#include "printf.h"

/*************  USER Configuration *****************************/
// Hardware configuration
RF24 radio(7, 8);                        // Set up nRF24L01 radio on SPI bus plus pins 7 & 8

/***************************************************************/

const uint64_t pipes[2] = { 0xABCDABCD71LL, 0x544d52687CLL };   // Radio pipe addresses for the 2 nodes to communicate.

byte data[8];                           //Data buffer for testing data transfer speeds

unsigned long counter, rxTimer;          //Counter and timer for keeping track transfer info
unsigned long startTime, stopTime;
bool TX = 1, RX = 0, role = 0;

void setup(void) {

	Serial.begin(9600);
	printf_begin();
	radio.begin();                           // Setup and configure rf radio
	radio.setChannel(1);
	radio.setPALevel(RF24_PA_MAX);
	radio.setDataRate(RF24_1MBPS);
	radio.setAutoAck(1);                     // Ensure autoACK is enabled
	radio.setRetries(2, 15);                   // Optionally, increase the delay between retries & # of retries
	radio.setCRCLength(RF24_CRC_8);
	radio.openWritingPipe(pipes[0]);
	radio.openReadingPipe(1, pipes[1]);
	radio.enableDynamicPayloads();
	radio.startListening();                 // Start listening
	radio.printDetails();                   // Dump the configuration of the rf unit for debugging

	printf("John SendString test\n\r");
	printf("*** PRESS 'T' to begin transmitting to the other node\n\r");



	radio.powerUp();                        //Power up the radio
}

void loop(void){


	if (role == TX){

		delay(2000);
		sendString("Hello world");
		Serial.println("String Sent");
	}



	if (role == RX){
		readLine();
	}
	if (Serial.available())
	{
		char c = toupper(Serial.read());
		if (c == 'T' && role == RX)
		{
			printf("*** CHANGING TO TRANSMIT ROLE -- PRESS 'R' TO SWITCH BACK\n\r");
			radio.openWritingPipe(pipes[0]);
			radio.openReadingPipe(1, pipes[1]);
			radio.stopListening();
			role = TX;                  // Become the primary transmitter (ping out)
		}
		else if (c == 'R' && role == TX)
		{
			radio.openWritingPipe(pipes[0]);
			radio.openReadingPipe(1, pipes[1]);
			radio.startListening();
			printf("*** CHANGING TO RECEIVE ROLE -- PRESS 'T' TO SWITCH BACK\n\r");
			role = RX;                // Become the primary receiver (pong back)
		}
	}
}

void sendString(String s)
{
	unsigned int length = s.length();
	char buffer[length];
	s.toCharArray(buffer, length);
	char sendBuffer[1];

	char endSignal[8] = "1234567";
	for (int i = 0; i<length; i++)
	{
		sendBuffer[0] = buffer[i];
		//radio.writeFast(&sendBuffer,1);
		//radio.txStandBy();//flush FIFO 
	}

	//for (int i=0;i<3;i++)
	//{
	radio.writeFast(&endSignal, 8);
	radio.txStandBy();//flush FIFO 
	//}

	//    radio.txStandBy();//flush FIFO 
}

void readLine()
{

	
	char buffer1[8] = "7654321";


	while (radio.available()){
		byte payloadSize = radio.getDynamicPayloadSize();
                char buffer[payloadSize+1];
                buffer[payloadSize]=0; //to support receiving data from Netduino, need to fix this at Netduino side
		radio.read(&buffer, payloadSize);
		Serial.println(buffer);
		//      Serial.print("|");
		//      Serial.print(sizeof(buffer));
		//      Serial.print("|");
		//      Serial.println(buffer1);
	}
	//Serial.println("data arrived");


}