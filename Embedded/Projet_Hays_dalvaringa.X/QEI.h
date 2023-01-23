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


void InitQEI1();
void InitQEI2();
void QEIUpdateData();
void SendPositionData();




#ifdef	__cplusplus
}
#endif

#endif	/* QEI_H */

