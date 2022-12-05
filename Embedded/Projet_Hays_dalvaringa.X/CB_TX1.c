#include <xc.h>
#include <stdio.h>
#include <stdlib.h>
#include "CB_TX1.h"
#include "Toolbox.h"
#define CBTX1_BUFER_SIZE 128

int dataSize;
int cbTx1Head;
int cbTx1Tail;
unsigned char cbTx1Buffer [CBTX1_BUFER_SIZE];
unsigned char isTransmitting = 0;

void SendMessage(unsigned char*message, int length) {
    unsigned char i = 0;
    if (CB_TX1_GetRemainingSize() > length) {
        for (i = 0; i < length; i++)
            CB_TX1_Add(message [i]);
        if (!CB_TX1_IsTranmitting())
            SendOne();
    }
}

void CB_TX1_Add(unsigned char value) {
    cbTx1Buffer [cbTx1Head] = value;
    cbTx1Head++;
    if (cbTx1Head >= CBTX1_BUFER_SIZE) {
        cbTx1Head = 0;
    }
}

unsigned char CB_TX1_Get(void) {
    unsigned char val;
    val = cbTx1Buffer[cbTx1Tail];
    cbTx1Tail++;
    if (cbTx1Tail >= CBTX1_BUFER_SIZE) {
        cbTx1Tail = 0;
    }
    return val;
}

void __attribute__((interrupt, no_auto_psv))_U1TXInterrupt(void) {
    IFS0bits.U1TXIF = 0; // c l e a r TX i n t e r r u p t f l a g
    if (cbTx1Tail != cbTx1Head) {
        SendOne();
    } else
        isTransmitting = 0;
}

void SendOne() {
    isTransmitting = 1;
    unsigned char value = CB_TX1_Get();
    U1TXREG = value; // Transmit one c h a r a c t e r
}

unsigned char CB_TX1_IsTranmitting(void) {
    return isTransmitting;
}

int CB_TX1_GetDataSize(void) {
    // r e t u r n s i z e o f data s t o r e d i n c i r c u l a r b u f f e r
    if (cbTx1Tail <= cbTx1Head) {
        dataSize = abs(cbTx1Head - cbTx1Tail);
    } 
    else {
        dataSize = CBTX1_BUFER_SIZE - abs(cbTx1Tail - cbTx1Head);
    }
    return dataSize;
}

int CB_TX1_GetRemainingSize(void) {
    // return size of remaining size in circular buffer
    int remainingSize;
    remainingSize = CBTX1_BUFER_SIZE - dataSize;
    return remainingSize;
}