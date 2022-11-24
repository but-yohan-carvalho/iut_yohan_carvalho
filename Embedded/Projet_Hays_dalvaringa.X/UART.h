/* 
 * File:   UART.h
 * Author: TP-EO-1
 *
 * Created on 16 novembre 2022, 17:06
 */

#ifndef UART_H
#define	UART_H

    void InitUART(void);
    void __attribute__(( interrupt , no_auto_psv ))_U1RXInterrupt(void);
    void SendMessageDirect( unsigned char * message , int length);


#endif	/* UART_H */

