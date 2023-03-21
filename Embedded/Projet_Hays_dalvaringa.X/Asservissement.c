#include "Asservissement.h"
#include "main.h"
#include "Utilities.h"
#include "Toolbox.h"
#include "UART_Protocol.h"

void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax, double integralMax, double deriveeMax) {
    PidCorr->Kp = Kp;
    PidCorr->erreurProportionelleMax = proportionelleMax; //On limite la correction due au Kp
    PidCorr->Ki = Ki;
    PidCorr->erreurIntegraleMax = integralMax; //On limite la correction due au Ki
    PidCorr->Kd = Kd;
    PidCorr->erreurDeriveeMax = deriveeMax; //On limite la correction due au Kd

}

void EnvoieAsservissementConstant(volatile PidCorrector* PidCorr) {

    unsigned char AsservissementPayload[30];

    getBytesFromFloat(AsservissementPayload, 0, PidCorr->Kp);
    getBytesFromFloat(AsservissementPayload, 4, PidCorr->Ki);
    getBytesFromFloat(AsservissementPayload, 8, PidCorr->Kd);
    getBytesFromFloat(AsservissementPayload, 12, PidCorr->proportionelleMax);
    getBytesFromFloat(AsservissementPayload, 16, PidCorr->integralMax);
    getBytesFromFloat(AsservissementPayload, 20, PidCorr->deriveeMax);

    UartEncodeAndSendMessage(0x0063 , 20 , AsservissementPayload);
}

//double Correcteur(volatile PidCorrector* PidCorr, double erreur) {
//    PidCorr->erreur = erreur;
//    double erreurProportionnelle = LimitToInterval(erreur , 0 , 10);
//    PidCorr->corrP = ...;
//    PidCorr->erreurIntegrale += ...;
//    PidCorr->erreurIntegrale = LimitToInterval(...);
//    PidCorr->corrI = ...;
//    double erreurDerivee = (erreur - PidCorr->epsilon_1) * FREQ_ECH_QEI;
//    double deriveeBornee = LimitToInterval(erreurDerivee, -PidCorr->erreurDeriveeMax / PidCorr->Kd, PiPidCorr->epsilon_1 = erreur;
//            PidCorr->corrD = deriveeBornee * PidCorr->Kd;
//    return PidCorr->corrP + PidCorr->corrI + PidCorr->corrD;
//}

