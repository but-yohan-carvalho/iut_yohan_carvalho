/* 
 * File:   QEI.h
 * Author: TP-EO-5
 *
 * Created on 23 janvier 2023, 08:27
 */

#ifndef QEI_H
#define	QEI_H

#ifdef	__cplusplus
extern "C" {
#endif

#define FREQ_ECH_QEI 250
#define POSITION_DATA 0x0061    
//#define DISTROUES 281.2
#define DISTROUES 218.5
void InitQEI1();
void InitQEI2();
void QEIUpdateData();
void SendPositionData();





#ifdef	__cplusplus
}
#endif

#endif	/* QEI_H */

