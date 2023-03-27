#ifndef ROBOT_H
#define ROBOT_H

#include "Asservissement.h"


typedef struct robotStateBITS {

    union {

        struct {
            unsigned char taskEnCours;
            float vitesseGaucheConsigne;
            float vitesseGaucheCommandeCourante;
            float vitesseDroiteConsigne;
            float vitesseDroiteCommandeCourante;
            float distanceTelemetreExDroit;
            float distanceTelemetreExGauche;
            float distanceTelemetreDroit;
            float distanceTelemetreGauche;
            float distanceTelemetreCentre;
            float result;
            
            double vitesseDroitFromOdometry;
            double vitesseGaucheFromOdometry;
            double vitesseLineaireFromOdometry;
            double vitesseAngulaireFromOdometry;
            double xPosFromOdometry;
            double xPosFromOdometry_1;
            double yPosFromOdometry;
            double yPosFromOdometry_1;
            double angleRadianFromOdometry_1;
            double angleRadianFromOdometry;
            double xCorrectionVitessePourcent;
            double thetaCorrectionVitessePourcent;
            
            PidCorrector PidX;
            PidCorrector PidTheta;
        }
        ;
    }
    ;
} 
ROBOT_STATE_BITS;

extern volatile ROBOT_STATE_BITS robotState;
#endif /* ROBOT_H */

