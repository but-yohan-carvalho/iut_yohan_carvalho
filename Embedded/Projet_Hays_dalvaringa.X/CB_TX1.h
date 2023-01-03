/* 
 * File:   CB_TX1.h
 * Author: TP-EO-1
 *
 * Created on 28 novembre 2022, 15:33
 */

#ifndef CB_TX1_H
#define	CB_TX1_H

#ifdef	__cplusplus
extern "C" {
#endif

void SendMessage (unsigned char*message,int length);
void CB_TX1_Add(unsigned char value);
void SendOne();
unsigned char CB_TX1_IsTranmitting(void);
int CB_TX1_GetRemainingSize(void);
unsigned char CB_TX1_Get(void);

#ifdef	__cplusplus
}
#endif

#endif	/* CB_TX1_H */

