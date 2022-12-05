/* 
 * File:   PWM.h
 * Author: TP-EO-1
 *
 * Created on 9 septembre 2022, 18:48
 */

#ifndef PWM_H

#define	PWM_H
#define MOTEUR_DROIT 0
#define MOTEUR_GAUCHE 1

void InitPWM(void);
void PWMSetSpeed(float vitesseEnPourcents, int moteur);
void PWMUpdateSpeed();
void PWMSetSpeedConsigne(float vitesseEnPourcents, char moteur);

#endif	/* PWM_H */

