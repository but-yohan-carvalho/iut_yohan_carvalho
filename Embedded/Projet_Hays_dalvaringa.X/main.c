#include <stdio.h>
#include <stdlib.h>
#include <libpic30.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h" 
#include "timer.h"
#include "robot.h"
#include "ToolBox.h"
#include "PWM.h"
#include "ADC.h"
#include "Robot.h"
#include "main.h"
#include "UART.h"
#include "CB_TX1.h"
#include "CB_RX1.h"
#include "UART.h"
#include "UART_Protocol.h"
#include "QEI.h"
#include "Asservissement.h"

unsigned int ADCValue0;
unsigned int ADCValue1;
unsigned int ADCValue2;
unsigned int ADCValue3;
unsigned int ADCValue4;
unsigned char nextStateRobot;
unsigned char stateRobot;

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l?oscillateur
    /****************************************************************************************************/
    InitOscillator();
    /****************************************************************************************************/
    // Configuration des entr�es sorties
    /****************************************************************************************************/
    InitIO();
    InitTimer23();
    InitTimer1();
    InitTimer4();
    InitPWM();
    InitADC1();
    InitUART();
    InitQEI1();
    InitQEI2();


    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;

    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        //unsigned char payload[] = {'B', 'o', 'n', 'j', 'o', 'u', 'r'};
        //UartEncodeAndSendMessage(0x0080, 7, payload);
                int i;
                for(i=0; i<CB_RX1_GetDataSize(); i++){
                    unsigned char c = CB_RX1_Get();
                    UartDecodeMessage(c);
                }
                
//        unsigned char payload[] = {};
//        UartEncodeAndSendMessage(0x0020, 2, payload);
        // SendMessage((unsigned char*) "Au revoir", 9);

        //__delay32(1000);
        //__delay32(40000000);
        
        if (ADCIsConversionFinished()) {
            ADCClearConversionFinishedFlag();
            unsigned int * result = ADCGetResult();
            ADCValue0 = result[0];
            ADCValue1 = result[1];
            ADCValue2 = result[2];
            ADCValue3 = result[3];
            ADCValue4 = result[4];


            float volts = ((float) result [1])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit = 34 / volts - 5;
            volts = ((float) result [0])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreExDroit = 34 / volts - 5;
            volts = ((float) result [2])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreCentre = 34 / volts - 5;
            volts = ((float) result [4])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche = 34 / volts - 5;
            volts = ((float) result [3])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreExGauche = 34 / volts - 5;

            if (robotState.distanceTelemetreCentre < 10) {
                robotState.distanceTelemetreCentre = 10;
            }
            if (robotState.distanceTelemetreGauche < 10) {
                robotState.distanceTelemetreGauche = 10;
            }
            if (robotState.distanceTelemetreDroit < 10) {
                robotState.distanceTelemetreDroit = 10;
            }
            if (robotState.distanceTelemetreExGauche < 10) {
                robotState.distanceTelemetreExGauche = 10;
            }
            if (robotState.distanceTelemetreExDroit < 10) {
                robotState.distanceTelemetreExDroit = 10;
            }

            if (robotState.distanceTelemetreDroit < 30) {
                LED_ORANGE = 1;
            } else {
                LED_ORANGE = 0;
            }

            //if (robotState.distanceTelemetreCentre < 30) {
            //    LED_BLEUE = 1;
            //} else {
            //    LED_BLEUE = 0;
            //}

            if (robotState.distanceTelemetreGauche < 30) {
                LED_BLANCHE = 1;
            } else {
                LED_BLANCHE = 0;
            }
            
            unsigned char payload[] = {robotState.distanceTelemetreGauche, robotState.distanceTelemetreCentre, robotState.distanceTelemetreDroit};
            UartEncodeAndSendMessage(0x0030, 3, payload);  
        }
        if ((robotState.vitesseGaucheCommandeCourante == 25) && (robotState.vitesseDroiteCommandeCourante == 25)) {
            LED_BLEUE = 1;
        } else {
            LED_BLEUE = 0;
        }
    }
}// fin main

void OperatingSystemLoop(void) {
    switch (stateRobot) {
    //    unsigned char payload[] = {MOTEUR_GAUCHE, MOTEUR_DROIT};
    //    UartEncodeAndSendMessage(0x0040, 2, payload);
        case STATE_ATTENTE:
            timestamp = 0;
            PWMSetSpeedConsigne(0, MOTEUR_DROIT);
            PWMSetSpeedConsigne(0, MOTEUR_GAUCHE);
            stateRobot = STATE_ATTENTE_EN_COURS;

        case STATE_ATTENTE_EN_COURS:
            if (timestamp > 1000)
                stateRobot = STATE_AVANCE;
            break;

        case STATE_AVANCE:
            PWMSetSpeedConsigne(20, MOTEUR_DROIT);
            PWMSetSpeedConsigne(20, MOTEUR_GAUCHE);
            stateRobot = STATE_AVANCE_EN_COURS;
            break;
        case STATE_AVANCE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_GAUCHE:
            PWMSetSpeedConsigne(15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(0, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_GAUCHE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_DROITE:
            PWMSetSpeedConsigne(0, MOTEUR_DROIT);
            PWMSetSpeedConsigne(15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_DROITE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_GAUCHE:
            PWMSetSpeedConsigne(15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(-15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_DROITE:
            PWMSetSpeedConsigne(-15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        default:
            stateRobot = STATE_ATTENTE;
            break;
    }
//    unsigned char payload[] = {robotState.vitesseGaucheConsigne, -robotState.vitesseDroiteConsigne};
//    UartEncodeAndSendMessage(0x0040, 2, payload);   
}

unsigned char nextStateRobot = 0;

void SendStateSupervision()
{       
    unsigned long timeStampCourant = timestamp;
    unsigned char payload[] = {stateRobot ,
    (unsigned char) (timeStampCourant>>24),
    (unsigned char) (timeStampCourant>>16),
    (unsigned char) (timeStampCourant>>8),
    (unsigned char) (timeStampCourant>>0)};
    UartEncodeAndSendMessage(0x0050, 5, payload);
}

/*
void SetNextRobotStateInAutomaticMode() {
    unsigned char positionObstacle = PAS_D_OBSTACLE;

    //D�termination de la position des obstacles en fonction des t�l�m�tres
    if (((stateRobot != (STATE_AVANCE)) || (stateRobot != (STATE_AVANCE_EN_COURS))) &&
            ((robotState.distanceTelemetreDroit < 45 &&
            robotState.distanceTelemetreCentre > 30 &&
            robotState.distanceTelemetreGauche > 30) ||
            (robotState.distanceTelemetreExDroit <= 12 &&
            robotState.distanceTelemetreGauche > 35) ||
            (robotState.distanceTelemetreExDroit <= 12)))//Obstacle � droite pas en vitesse de pointe
        positionObstacle = OBSTACLE_A_DROITE;

    if (((stateRobot == (STATE_AVANCE)) || (stateRobot == (STATE_AVANCE_EN_COURS))) &&
            ((robotState.distanceTelemetreDroit < 57 &&
            robotState.distanceTelemetreCentre > 44 &&
            robotState.distanceTelemetreGauche > 44) ||
            (robotState.distanceTelemetreExDroit < 32 &&
            robotState.distanceTelemetreGauche > 44) ||
            (robotState.distanceTelemetreExDroit <= 22)))//Obstacle � droite pas en vitesse de pointe
        positionObstacle = OBSTACLE_A_DROITEVP;

    else if (((stateRobot == (STATE_AVANCE)) || (stateRobot == (STATE_AVANCE_EN_COURS))) &&
            ((robotState.distanceTelemetreDroit > 44 &&
            robotState.distanceTelemetreCentre > 44 &&
            robotState.distanceTelemetreGauche < 57) ||
            (robotState.distanceTelemetreDroit > 44 && ((
            robotState.distanceTelemetreExGauche < 32) ||
            (robotState.distanceTelemetreExGauche <= 22))))) //Obstacle � gauche
        positionObstacle = OBSTACLE_A_GAUCHEVP;

    else if (((stateRobot != (STATE_AVANCE)) || (stateRobot != (STATE_AVANCE_EN_COURS))) &&
            ((robotState.distanceTelemetreDroit > 30 &&
            robotState.distanceTelemetreCentre > 30 &&
            robotState.distanceTelemetreGauche < 45) ||
            (robotState.distanceTelemetreDroit > 35 &&
            robotState.distanceTelemetreExGauche <= 12) ||
            (robotState.distanceTelemetreExGauche <= 12))) //Obstacle � gauche pas en vitesse de pointe
        positionObstacle = OBSTACLE_A_GAUCHE;

    else if ((robotState.distanceTelemetreCentre < 37) &&
            ((stateRobot != (STATE_AVANCE)) || (stateRobot != (STATE_AVANCE_EN_COURS)))) //Obstacle en face
        positionObstacle = OBSTACLE_EN_FACE;

    else if ((robotState.distanceTelemetreCentre < 47)&&
            ((stateRobot == (STATE_AVANCE)) || (stateRobot == (STATE_AVANCE_EN_COURS)))) //Obstacle en face en vitesse de pointe
        positionObstacle = OBSTACLE_EN_FACEVP;

    else if (robotState.distanceTelemetreDroit > 45 &&
            robotState.distanceTelemetreCentre > 55 &&
            robotState.distanceTelemetreGauche > 45) //pas d?obstacle
        positionObstacle = PAS_D_OBSTACLE;

    //D�termination de l?�tat � venir du robot
    if (positionObstacle == PAS_D_OBSTACLE)
        nextStateRobot = STATE_AVANCE;
    else if ((positionObstacle == OBSTACLE_A_DROITE) || (positionObstacle == OBSTACLE_A_DROITEVP))
        nextStateRobot = STATE_TOURNE_GAUCHE;
    else if ((positionObstacle == OBSTACLE_A_GAUCHE) || (positionObstacle == OBSTACLE_A_GAUCHEVP))
        nextStateRobot = STATE_TOURNE_DROITE;
    else if ((positionObstacle == OBSTACLE_EN_FACE) || (positionObstacle == OBSTACLE_EN_FACEVP)) {
        if (robotState.distanceTelemetreExDroit >= robotState.distanceTelemetreExGauche) {
            nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
        } else {
            nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
        }
    }
    //Si l'on n?est pas dans la transition de l?�tape en cours
    if (nextStateRobot != stateRobot - 1) {
        stateRobot = nextStateRobot;
        SendStateSupervision();
    }
}
*/

//UartProcessDecodedMessage(0x0063, 29, tabasx);

void SetNextRobotStateInAutomaticMode(void) {
    unsigned char positionObstacle = PAS_D_OBSTACLE;

    //D�termination de la position des obstacles en fonction des t�l�m�tres
    if (robotState.distanceTelemetreDroit > 30 &&
            robotState.distanceTelemetreExDroit > 20 &&
            robotState.distanceTelemetreCentre > 30 &&
            robotState.distanceTelemetreExGauche > 20 &&
            robotState.distanceTelemetreGauche > 30) //pas d?obstacle
        positionObstacle = PAS_D_OBSTACLE;
    else if (robotState.distanceTelemetreCentre <= 30)
        positionObstacle = OBSTACLE_EN_FACE;
    else if (robotState.distanceTelemetreDroit < 30 ||
            robotState.distanceTelemetreExDroit < 20)
        positionObstacle = OBSTACLE_A_DROITE;
    else if (robotState.distanceTelemetreExGauche < 30 ||
            robotState.distanceTelemetreGauche < 20)
        positionObstacle = OBSTACLE_A_GAUCHE;


    //D�termination de l?�tat � venir du robot
    if (positionObstacle == PAS_D_OBSTACLE)
        nextStateRobot = STATE_AVANCE;
    else if (positionObstacle == OBSTACLE_A_DROITE)
        nextStateRobot = STATE_TOURNE_GAUCHE;
    else if (positionObstacle == OBSTACLE_A_GAUCHE)
        nextStateRobot = STATE_TOURNE_DROITE;
    else if (positionObstacle == OBSTACLE_EN_FACE)
        nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;

    //Si l?on n?est pas dans la transition de l?�tape en cours
    if (nextStateRobot != stateRobot - 1) {
        stateRobot = nextStateRobot;
        //SendStateSupervision();
    }
}