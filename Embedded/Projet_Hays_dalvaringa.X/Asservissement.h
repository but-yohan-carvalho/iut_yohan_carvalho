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
double erreurIntegraleMax;
double erreurDeriveeMax;
double erreurIntegrale;
double epsilon_1;
double erreur;
//For Debug only
double corrP;
double corrI;
double corrD;
}

PidCorrector;

void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double pro);

#ifdef	__cplusplus
}
#endif

#endif	/* ASSERVISSEMENT_H */

