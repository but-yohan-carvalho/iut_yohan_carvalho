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
void UartDecodeMessage(unsigned char c);
void UartProcessDecodedMessage(unsigned char msgFunction, unsigned char msgPayloadLength, unsigned char msgPayload[]);
void SetRobotState(unsigned char payload);
void SetRobotAutoControlState(unsigned char state);

#define Waiting 0
#define FunctionMSB 1
#define FunctionLSB 2
#define PayloadLengthMSB 3
#define PayloadLengthLSB 4
#define Payload 5
#define CheckSum 6


#define SET_ROBOT_STATE 0x0051
#define SET_ROBOT_MANUAL_CONTROL 0x0052




#endif	/* UART_PROTOCOL_H */

