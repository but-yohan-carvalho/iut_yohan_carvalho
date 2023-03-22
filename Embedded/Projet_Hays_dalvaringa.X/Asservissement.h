/* 
 * File:   Asservissement.h
 * Author: TP-EO-5
 *
 * Created on 6 février 2023, 15:05
 */

#ifndef ASSERVISSEMENT_H
#define	ASSERVISSEMENT_H

#ifdef	__cplusplus
extern "C" {
#endif

typedef struct _PidCorrector
{
double Kp;
double Ki;
double Kd;

double erreurProportionelleMax;
double proportionelleMax;
double erreurIntegraleMax;
double integralMax;
double erreurDeriveeMax;
double deriveeMax;
double erreurIntegrale;
double epsilon_1;
double erreur;
double erreurProportionelle;
double erreurDerivee;
//For Debug only
double corrP;
double corrI;
double corrD;
}

PidCorrector;

void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax, double integralMax, double deriveeMax);
double Correcteur(volatile PidCorrector* PidCorr, double erreur);
void UpdateAsservissement();
void PWMSetSpeedConsignePolaire(double corVitX, double CorVitTheta);

#ifdef	__cplusplus
}
#endif

#endif	/* ASSERVISSEMENT_H */

