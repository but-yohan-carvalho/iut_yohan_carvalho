#include <xc.h>
#include "UART_Protocol.h"
#include "CB_TX1.h"
#include "CB_RX1.h"

int rcvState = Waiting;
int msgDecodedFunction = 0;
int msgDecodedPayloadLength = 0;
int msgDecodedPayloadIndex = 0;
unsigned char msgDecodedPayload[128];
unsigned char receiveChecksum;
unsigned char calculChecksum;



unsigned char UartCalculateChecksum(int msgFunction, int msgPayloadLength, unsigned char* msgPayload) {
    //Fonction prenant entree la trame et sa longueur pour calculer le checksum
    unsigned char Checksum = 0;
    Checksum ^= 0xFE;
    Checksum ^= (unsigned char) (msgFunction >> 8);
    Checksum ^= (unsigned char) (msgFunction >> 0);
    Checksum ^= (unsigned char) (msgPayloadLength >> 8);
    Checksum ^= (unsigned char) (msgPayloadLength >> 0);

    int lg;
    for (lg = 0; lg < msgPayloadLength; lg++) {
        Checksum ^= msgPayload[lg];
    }
    return Checksum;
}



void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, unsigned char* msgPayload) {
    //Fonction d?encodage et d?envoi d?un message
    unsigned char mess [msgPayloadLength + 6];
    int position = 0;
    mess[position++] = 0xFE;
    mess[position++] = (unsigned char) (msgFunction >> 8);
    mess[position++] = (unsigned char) (msgFunction >> 0);
    mess[position++] = (unsigned char) (msgPayloadLength >> 8);
    mess[position++] = (unsigned char) (msgPayloadLength >> 0);
    int i;
    for (i = 0; i < msgPayloadLength; i++) {
        mess[position++] = msgPayload[i];
    }
    mess[position++] = UartCalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
    SendMessage(mess, position);
}

/*
void UartDecodeMessage(unsigned char c) {
    //Fonction prenant en entree un octet et servant a reconstituer les trames
    switch (rcvState) {
        case Waiting:
            if (c == 0xFE) {
                rcvState = FunctionMSB;
            }
            break;
        case FunctionMSB:
            msgDecodedFunction = (c << 8);
            rcvState = FunctionLSB;
            break;
        case FunctionLSB:
            msgDecodedFunction += (c << 0);
            rcvState = PayloadLengthMSB;
            break;
        case PayloadLengthMSB:
            msgDecodedPayloadLength = (c << 8);
            rcvState = PayloadLengthLSB;
            break;
        case PayloadLengthLSB:
            msgDecodedPayloadLength += (c << 0);
            if (msgDecodedPayloadLength == 0)
            {
                rcvState = CheckSum;
            }
            else
            {
                rcvState = Payload;
            }
            break;
        case Payload:
            
            msgDecodedPayload[msgDecodedPayloadIndex] = c;
            msgDecodedPayloadIndex++;
            if (msgDecodedPayloadIndex >= msgDecodedPayloadLength) {
                rcvState = CheckSum;
            }
            break;
        case CheckSum:
            
            receiveChecksum = c;
            calculChecksum = UartCalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
            if (calculChecksum == receiveChecksum)
            {
                UartEncodeAndSendMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
            }
            rcvState = Waiting;
            break;
            
        default:
            rcvState = Waiting;
    }
}

void UartProcessDecodedMessage(int function, int payloadLength, unsigned char* payload) {
    //Fonction appelee apres le decodage pour executer l?action
    //correspondant au message recu
    
}
*/

//*************************************************************************/
//Fonctions correspondant aux messages
//*************************************************************************/
