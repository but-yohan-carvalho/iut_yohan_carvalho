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

        public Robot()
        { 
        
        }
            
        public Queue<byte> byteListReceived = new Queue<byte>();
    }
}
