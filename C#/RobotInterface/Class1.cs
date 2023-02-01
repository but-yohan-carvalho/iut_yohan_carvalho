using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotInterfaceNet
{
    public class Robot
    {
        public string receivedText = "";
        public float distanceTelemetreDroit;
        public float distanceTelemetreGauche;
        public float distanceTelemetreCentre;
        public bool led1;
        public bool led2;
        public bool led3;
        public int consigneGauche;
        public int consigneDroite;
        public int timestamp;
        public float positionX;
        public float positionY;
        public float angleRad;
        public float vitesseAng;
        public float vitesseLin;
        public Queue<byte> byteListReceived = new Queue<byte>();

        public Robot()
        { 
        
        }
            
        
    }
}
