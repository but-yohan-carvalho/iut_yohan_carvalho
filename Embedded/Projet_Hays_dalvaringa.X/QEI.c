#include <xc.h> // library xc.h inclut tous les uC
#include <math.h>
#include "IO.h" 
#include "Robot.h"
#include "QEI.h"
#include "Toolbox.h"
#include "timer.h"
#include "Utilities.h"
#include "UART_Protocol.h"

#define POSITION_DATA 0x0061    
#define DISTROUES 281.2
#define FREQ_ECH_QEI 250

void InitQEI1() {
    QEI1IOCbits.SWPAB = 1; //QEAx and QEBx are swapped
    QEI1GECL = 0xFFFF;
    QEI1GECH = 0xFFFF;
    QEI1CONbits.QEIEN = 1; // Enable QEI Module
}

void InitQEI2() {
    QEI2IOCbits.SWPAB = 1; //QEAx and QEBx are not swapped
    QEI2GECL = 0xFFFF;
    QEI2GECH = 0xFFFF;
    QEI2CONbits.QEIEN = 1; // Enable QEI Module
}

double QeiDroitPosition_T_1;
double QeiGauchePosition_T_1;
double QeiDroitPosition;
double QeiGauchePosition;
double delta_d;
double delta_g;
double delta_theta;
double theta;

void QEIUpdateData() {

    //On sauvegarde les anciennes valeurs
    QeiDroitPosition_T_1 = QeiDroitPosition;
    QeiGauchePosition_T_1 = QeiGauchePosition;


    //On �ractualise les valeurs des positions
    long QEI1RawValue = POS1CNTL;
    QEI1RawValue += ((long) POS1HLD << 16);
    long QEI2RawValue = POS2CNTL;
    QEI2RawValue += ((long) POS2HLD << 16);


    //Conversion en mm (r\?egl\?e pour la taille des roues codeuses)
    QeiDroitPosition = 0.01620 * QEI1RawValue;
    QeiGauchePosition = -0.01620 * QEI2RawValue;


    //Calcul des deltas de position
    delta_d = QeiDroitPosition - QeiDroitPosition_T_1;
    delta_g = QeiGauchePosition - QeiGauchePosition_T_1;

    theta = (robotState.vitesseDroitFromOdometry - robotState.vitesseGaucheFromOdometry) / DISTROUES;
    //delta_theta = atan((delta_d - delta_g) / DISTROUES);
    delta_theta = (delta_d - delta_g) / DISTROUES;
    //dx = (delta_d + delta_g) / 2;



    //Calcul des vitesses
    //attention �remultiplier par la �frquence d�?chantillonnage
    robotState.vitesseDroitFromOdometry = delta_d*FREQ_ECH_QEI;
    robotState.vitesseGaucheFromOdometry = delta_g*FREQ_ECH_QEI;
    robotState.vitesseLineaireFromOdometry = (robotState.vitesseDroitFromOdometry + robotState.vitesseGaucheFromOdometry) / 2;
    robotState.vitesseAngulaireFromOdometry = delta_theta*FREQ_ECH_QEI;


    //Mise �jour du positionnement terrain �t-1
    robotState.xPosFromOdometry_1 = robotState.xPosFromOdometry;
    robotState.yPosFromOdometry_1 = robotState.yPosFromOdometry;
    robotState.angleRadianFromOdometry_1 = robotState.angleRadianFromOdometry;


    //Calcul des positions dans le referentiel du terrain
    robotState.xPosFromOdometry = robotState.xPosFromOdometry_1 + robotState.vitesseLineaireFromOdometry / FREQ_ECH_QEI * cos(robotState.angleRadianFromOdometry);
    robotState.yPosFromOdometry = robotState.yPosFromOdometry_1 + robotState.vitesseLineaireFromOdometry / FREQ_ECH_QEI * sin(robotState.angleRadianFromOdometry);
    robotState.angleRadianFromOdometry += delta_theta;
    if (robotState.angleRadianFromOdometry > PI)
        robotState.angleRadianFromOdometry -= 2 * PI;
    if (robotState.angleRadianFromOdometry < -PI)
        robotState.angleRadianFromOdometry += 2 * PI;
}



void SendPositionData() {
    unsigned char positionPayload[24];
    getBytesFromInt32(positionPayload, 0, timestamp);
    getBytesFromFloat(positionPayload, 4, (float) (robotState.xPosFromOdometry));
    getBytesFromFloat(positionPayload, 8, (float) (robotState.yPosFromOdometry));
    getBytesFromFloat(positionPayload, 12, (float) (robotState.angleRadianFromOdometry));
    getBytesFromFloat(positionPayload, 16, (float) (robotState.vitesseLineaireFromOdometry));
    getBytesFromFloat(positionPayload, 20, (float) (robotState.vitesseAngulaireFromOdometry));
    UartEncodeAndSendMessage(POSITION_DATA, 24, positionPayload);
}