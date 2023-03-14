#include <xc.h>
#include "UART_Protocol.h"
#include "CB_TX1.h"
#include "CB_RX1.h"
#include "main.h"
#include "Asservissement.h"
#include "Utilities.h"
#include "Robot.h"


int rcvState = Waiting;
int msgDecodedFunction = 0;
int msgDecodedPayloadLength = 0;
int msgDecodedPayloadIndex = 0;
unsigned char msgDecodedPayload[128];
unsigned char receiveChecksum;
unsigned char calculChecksum;

    double KpX;
    double KiX;
    double KdX;
    double proportionelleMaxX;
    double integralMaxX;
    double deriveeMaxX;
    double consigneX;

extern unsigned char stateRobot;

unsigned char autoControlActivated = 1;

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
    SendMessage(mess, position + 1);
}

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
            if (msgDecodedPayloadLength == 0) {
                rcvState = CheckSum;
            } else {
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
            if (calculChecksum == receiveChecksum) {
                UartEncodeAndSendMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
            }
            rcvState = Waiting;
            break;

        default:
            rcvState = Waiting;
    }
}

unsigned char tkp[4], tki[4], tkd[4], tkprop[4], tkintegrale[4], tkderivee[4], tkconsigne[4];

void UartProcessDecodedMessage(unsigned char function,
        unsigned char payloadLength, unsigned char payload[]) {
    //Fonction éappele èaprs le édcodage pour éexcuter l?action
    //correspondant au message çreu
    switch (function) {
        case SET_ROBOT_STATE:
            SetRobotState(payload[0]);
            break;
        case SET_ROBOT_MANUAL_CONTROL:
            SetRobotAutoControlState(payload[0]);
            break;
        case SET_CORRECTOR:
            
            tkp[0] = payload[1];
            tkp[1] = payload[2];
            tkp[2] = payload[3];
            tkp[3] = payload[4];
            
            tki[0] = payload[5];
            tki[1] = payload[6];
            tki[2] = payload[7];
            tki[3] = payload[8];
            
            tkd[0] = payload[9];
            tkd[1] = payload[10];
            tkd[2] = payload[11];
            tkd[3] = payload[12];
            
            tkprop[0] = payload[13];
            tkprop[1] = payload[14];
            tkprop[2] = payload[15];
            tkprop[3] = payload[16];
            
            tkintegrale[0] = payload[17];
            tkintegrale[1] = payload[18];
            tkintegrale[2] = payload[19];
            tkintegrale[3] = payload[20];
            
            tkderivee[0] = payload[21];
            tkderivee[1] = payload[22];
            tkderivee[2] = payload[23];
            tkderivee[3] = payload[24];
            
            tkconsigne[0] = payload[25];
            tkconsigne[1] = payload[26];
            tkconsigne[2] = payload[27];
            tkconsigne[3] = payload[28];
                          
            KpX = getFloat(tkp, 0);
            KiX = getFloat(tki, 0);
            KdX= getFloat(tkd, 0);
            proportionelleMaxX = getFloat(tkprop, 0);
            integralMaxX = getFloat(tkintegrale, 0);
            deriveeMaxX = getFloat(tkderivee, 0);
            consigneX = getFloat(tkconsigne, 0);
            
            if(payload[0] == 0){
                SetupPidAsservissement(&robotState.PidX, KpX, KiX, KdX, proportionelleMaxX, deriveeMaxX, consigneX);
            }
            else{
                SetupPidAsservissement(&robotState.PidTheta, KpX, KiX, KdX, proportionelleMaxX, deriveeMaxX, consigneX);
            }
            
            break;
            
        default:
            break;
    }
}



void SetRobotAutoControlState(unsigned char state){
    if((state!=autoControlActivated) && ((state==0)||(state==1))){
        autoControlActivated=state;
        stateRobot = STATE_ARRET;
    }
}

void SetRobotState(unsigned char state){
        stateRobot = state;
}
//*************************************************************************/
//Fonctions correspondant aux messages
//*************************************************************************/