#include <xc.h>
#include <stdio.h>
#include <stdlib.h>
#include "CB_TX1.h"
#define CBTX1_BUFER_SIZE 128

int cbTx1Head ;
int cbTx1Tail ;
unsigned char cbTx1Buffer [CBTX1_BUFFER_SIZE];
unsigned char isTransmitting = 0 ;
void SendMessage (unsigned char*message,int length)
{
unsigned char i=0;
if (CB_TX1_RemainingSize() > length)
{
for (i=0; i<length; i++)
CB_TX1_Add(message [i]);
if (!CB_TX1_IsTranmitting())
SendOne() ;
}
}
void CB_TX1_Add(unsigned char value)
{
...
}
unsigned char CB_TX1_Get(void)
{
...
}
void __attribute__ ((interrupt , no_auto_psv))_U1TXInterrupt(void){
IFS0bits.U1TXIF = 0 ; // c l e a r TX i n t e r r u p t f l a g
if(cbTx1Tail!=cbTx1Head)
{
SendOne() ;
}
else
isTransmitting = 0 ;
}
void SendOne()
{
isTransmitting = 1 ;
unsigned char value = CB_TX1_Get() ;
U1TXREG = value ; // Transmit one c h a r a c t e r
}
unsigned char CB_TX1_IsTranmitting(void)
{
    ...
}
int CB_TX1_GetDataSize ( void )
{
// r e t u r n s i z e o f data s t o r e d i n c i r c u l a r b u f f e r
int dataSize;
...
return dataSize;
}
int CB_TX1_GetRemainingSize(void)
{
// return size of remaining size in circular buffer
int remainingSize;
...
return remainingSize;
}