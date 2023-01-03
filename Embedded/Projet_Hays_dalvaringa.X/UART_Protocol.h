/* 
 * File:   UART_Protocol.h
 * Author: TP-EO-1
 *
 * Created on 12 décembre 2022, 18:24
 */

#ifndef UART_PROTOCOL_H
#define	UART_PROTOCOL_H

unsigned char UartCalculateChecksum(int msgFunction, int msgPayloadLength, unsigned char* msgPayload);
void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, unsigned char* msgPayload);
void UartProcessDecodedMessage(int function, int payloadLength, unsigned char* payload);

#define Waiting 0
#define FunctionMSB 1
#define FunctionLSB 2
#define PayloadLengthMSB 3
#define PayloadLengthLSB 4
#define Payload 5
#define CheckSum 6




#endif	/* UART_PROTOCOL_H */

