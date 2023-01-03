/* 
 * File:   UART_Protocol.h
 * Author: TP-EO-1
 *
 * Created on 12 décembre 2022, 18:24
 */

#ifndef UART_PROTOCOL_H
#define	UART_PROTOCOL_H

unsigned char UartCalculateChecksum(int msgFunction,int msgPayloadLength, unsigned char* msgPayload);
void UartEncodeAndSendMessage(int msgFunction,int msgPayloadLength, unsigned char* msgPayload);
void UartProcessDecodedMessage(int function,int payloadLength, unsigned char* payload);






#endif	/* UART_PROTOCOL_H */

