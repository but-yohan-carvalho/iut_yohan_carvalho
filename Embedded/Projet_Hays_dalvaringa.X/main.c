#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h" 
#include "timer.h"
#include "PWM.h"
#include "ADC.h"
#include "Robot.h"
#include "main.h"
#include "UART.h"

unsigned int ADCValue0;
unsigned int ADCValue1;
unsigned int ADCValue2;
unsigned int ADCValue3;
unsigned int ADCValue4;
unsigned char nextStateRobot;
int main (void){
/***************************************************************************************************/
//Initialisation de l?oscillateur
/****************************************************************************************************/
InitOscillator();
/****************************************************************************************************/
// Configuration des entrées sorties
/****************************************************************************************************/
InitIO();
InitTimer23 ();
InitTimer1();
InitPWM();
InitADC1();
InitUART();


LED_BLANCHE = 1;
LED_BLEUE = 1;
LED_ORANGE = 1;

/****************************************************************************************************/
// Boucle Principale
/****************************************************************************************************/
while(1){
     if ( ADCIsConversionFinished ( ) == 1 )
{
ADCClearConversionFinishedFlag() ;
unsigned int * result = ADCGetResult();
float volts = ((float) result[2])*3.3/4096 * 3.2;
robotState.distanceTelemetreDroit = 34 / volts - 5;
volts = ((float)result[1]) * 3.3/4096 * 3.2 ;
robotState.distanceTelemetreCentre = 34 /volts - 5 ;
volts = ((float)result[0])*3.3 / 4096*3.2 ;
robotState.distanceTelemetreGauche = 34 / volts - 5;

/*SendMessageDirect((unsigned char*)"Bonjour",7);
__delay32(FCY);*/
     }
   /* if (ADCIsConversionFinished())
    {
        ADCClearConversionFinishedFlag();
        unsigned int * result = ADCGetResult();
        ADCValue0 = result[0];
        ADCValue1 = result[1];
        ADCValue2 = result[2];
        ADCValue3 = result[3];
        ADCValue4 = result[4];
        
    }*/
     
     if (robotState.distanceTelemetreGauche <= 30)
        {
            LED_ORANGE = 0;
            
        }
        else
        {
            LED_ORANGE = 1;
        }
        /*if (robotState.distanceTelemetreCentre <= 30)
        {
            LED_BLEUE = 0;
        }
        else
        {
            LED_BLEUE = 1;
        }*/
        if (robotState.distanceTelemetreDroit <= 30)
        {
            LED_BLANCHE = 0;
        }
        else
        {
            LED_BLANCHE = 1;
        }
     if (robotState.distanceTelemetreExDroit <= 30)
        {
            LED_BLANCHE = 0;
        }
        else
        {
            LED_BLANCHE = 1;
        }
     if (robotState.distanceTelemetreExGauche <= 30)
        {
            LED_BLANCHE = 0;
        }
        else
        {
            LED_BLANCHE = 1;
        }
    
    
} // fin main
}