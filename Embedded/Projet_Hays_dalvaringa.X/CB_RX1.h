/* 
 * File:   CB_RX1.h
 * Author: TP-EO-1
 *
 * Created on 5 décembre 2022, 13:45
 */

#ifndef CB_RX1_H
#define	CB_RX1_H

#ifdef	__cplusplus
extern "C" {
#endif

void SendMessage (unsigned char*message,int length);
void CB_RX1_Add(unsigned char value);
void SendOne();
unsigned char CB_RX1_IsTranmitting(void);
int CB_RX1_GetDataSize ( void );
int CB_RX1_GetRemainingSize(void);
unsigned char CB_RX1_Get(void);


#ifdef	__cplusplus
}
#endif

#endif	/* CB_RX1_H */

