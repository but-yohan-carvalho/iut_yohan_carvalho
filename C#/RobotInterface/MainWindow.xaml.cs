using ExtendedSerialPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using SciChart.Charting.Visuals;

namespace RobotInterfaceNet
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Robot robot = new Robot();
        DispatcherTimer timerAffichage;
        ReliableSerialPort serialPort1;
        public MainWindow()
        {
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("v1q9N0ZRgIzj1ebP4riiz/JWED4s1OkznHZBV5wW8Fv5kBlyo+UaxAlOttFNldGfu7yXJSsa7Jb6i9wafTefd2NQC4bCrpYo611tmhju/mJJnJFpbJ0NurEv42jlwADPuF8w2faKMUYO5hCc6bEbI0aj34uP0XjXYHNQ4ZgVCgCuzmSW9LKHcr5lVmwkRXf5xpQDm/6J62WcC2nWRhXwe9gPSe+ewRqPifnSK5FhgmKkf/v9cVIlLQYuAK3x840/+ygJKmBvyc+05j/qQSNxPPcGLbKV1Pdinm00KBMjVKCGkA6FIAj/QOAjCLtLk3+7oV0i3GLKjqELR167YeXJZE8z8XZRoXRXaEh4RL4C/DFbpg7mTMQ1hDDiNx4hA/zisuBtMzDgMkaQ9tSn3IghsHvCnuA90lgSZh/gpDp6ZPO6lNqqCrBas/5fTvhMvYgKCg27lxk6qVPN/6v3SS31sAeEyc0U8Erj+MgGBvpwFr7vHbaASaVKtlRQWNChiU6QqA==");
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM12", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();
            oscilloSpeed.AddOrUpdateLine(1, 200, "Ligne1");
            oscilloSpeed.ChangeLineColor(1, Color.FromRgb(255,0,100));
           


        }

        string receivedText = "";

        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            /* if(robot.receivedText != "")
              {
                  textBoxReception.Text += "Utilisation Serial Port : " +robot.receivedText+ "/n";
                  robot.receivedText = "";                 
              }*/
            while (robot.byteListReceived.Count > 0)
            {
                var c = robot.byteListReceived.Dequeue();
               //textBoxReception.Text += "0x" + c.ToString("X2") + " ";
                DecodeMessage(c);
            }
            oscilloSpeed.AddPointToLine(1, robot.positionX, robot.positionY);
            //asservSpeedDisplay2.UpdateIndependantOdometrySpeed(robot.positionX, robot.positionY);
            asservSpeedDisplay2.UpdatePolarOdometrySpeed(robot.vitesseLin, robot.vitesseAng);
            //asservSpeedDisplay2.UpdatePolarSpeedConsigneValues(robot.consigneGauche, robot.consigneDroite);
           
        }
        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            ///robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            foreach (var c in e.Data) {
                robot.byteListReceived.Enqueue(c);
            }
        }

        //private void TextBoxReception_TextChanged(object sender, TextChangedEventArgs e)
        //{

        //}

        bool toggle = false;
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {

            textBoxReception.Text += "Reçu envoyer: " + textBoxEmission.Text + "\n";
            serialPort1.WriteLine(textBoxEmission.Text);
            functionRecu(1);
            textBoxEmission.Text = "";
            

            if (!toggle)
            {
                buttonEnvoyer.Background = Brushes.Aqua;
            }
            else
            {
                buttonEnvoyer.Background = Brushes.Beige;
            }
            toggle = !toggle;


            /* private void messageSent(int send)
             {
                 if((textBoxEmission.Text !="") && (textBoxEmission.Text != "\r\n"))
                 {
                     serialPort1.WriteLine(textBoxEmission.Text);
                     if(send == 1)
                     {
                     }
                 }
                 textBoxEmission.Text = "";
             }*/

        }
        private void textBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var bList = Encoding.UTF8.GetBytes(textBoxEmission.Text);
                serialPort1.Write(bList, 0, bList.Length);
                functionRecu(1);
                textBoxEmission.Text = "";
            }
        }
        private void textBoxReception_KeyUp(object sender, KeyEventArgs e)
        {
            

        }


         public void functionRecu(int a)
         {
             textBoxReception.Text = receivedText;
             serialPort1.WriteLine(textBoxEmission.Text);
             if (a == 0)
             {
                 textBoxReception.Text += "Reçu envoyer: " + textBoxEmission.Text + "\n";
                 textBoxEmission.Text = "";
             }
             else if (a == 1)
             {
                 textBoxReception.Text += "Reçu via envoyer: " + textBoxEmission.Text + "\n";
                 textBoxEmission.Text = "";
             }

         }

        private void buttonclear_Click(object sender, RoutedEventArgs e)
        {
            textBoxReception.Text = "";
        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            //string s = "Bonjour";
            //byte[] byteList;// = new byte[20];
            //byteList = Encoding.ASCII.GetBytes(s);
            /* for (int i = 0; i < 20; i++)
             {
                 //byteList[i] = (byte)(2 * i);  

             }*/

            //processDecodeMessage(0x0080, byteList.Length, byteList);
            //byte[] telemetre = {0x34 ,0x3C, 0x80};
            //processDecodeMessage(0x0030, 3, telemetre);

            //byte[] moteur = { 0x34, 0x3C };
            //processDecodeMessage(0x0040, 2, moteur);

            //byte[] leds = { 0x00, 0x01 };
            //processDecodeMessage(0x0020, 2, leds);

            string s = "Bonjour";
            byte[] byteList;
            byteList = Encoding.ASCII.GetBytes(s);
            UartEncodeAndSendMessage(0x0080, 7, byteList);
            //textBoxReception.Text = s;


        }
        public enum msgFonction
        {
            transmissionText = 0x80,
            ReglageLed = 0x20,
            DistanceTelemeter = 0x30,
            ConsigneVitesse = 0x40,
            EtapeEnCours = 0x50,
            position = 0x61,
            Corrector = 0x63,

        }
        void processDecodeMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            switch (msgFunction)
            {
                case (int) msgFonction.transmissionText:
                    robot.receivedText += Encoding.UTF8.GetString(msgDecodedPayload, 0, msgDecodedPayloadLength);
                    break;

                case (int) msgFonction.ReglageLed:
                    switch (msgPayload[0])
                    {
                        case 0:
                            if (msgPayload[1] == 0x01)
                            {
                                Led0.IsChecked = true;
                            }
                            else
                            {
                                Led0.IsChecked = false;
                            }
                            break;

                        case 1:
                            if (msgPayload[1] == 0x01)
                            {
                                Led1.IsChecked = true;
                            }
                            else
                            {
                                Led1.IsChecked = false;
                            }
                            break;

                        case 2:
                            if (msgPayload[1] == 0x01)
                            {
                                Led2.IsChecked = true;
                            }
                            else
                            {
                                Led2.IsChecked = false;
                            }
                            break;
                    }
                    break;

                case (int)msgFonction.DistanceTelemeter:
                    robot.distanceTelemetreGauche = msgPayload[0];
                    robot.distanceTelemetreCentre = msgPayload[1];                 
                    robot.distanceTelemetreDroit = msgPayload[2];

                    string gauche = robot.distanceTelemetreGauche.ToString();
                    string centre = robot.distanceTelemetreCentre.ToString();
                    string droite = robot.distanceTelemetreDroit.ToString();

                    telemG.Content = "IR Gauche : " + gauche + " cm";
                    telemC.Content = "IR Centre : " + centre + " cm";
                    telemD.Content = "IR Droit : " + droite + " cm";                   
                    break;

                case (int)msgFonction.ConsigneVitesse:
                    robot.consigneGauche = msgPayload[0];
                    robot.consigneDroite = msgPayload[1];

                    VitesseG.Content = "Vitesse Gauche : " + robot.consigneGauche + "%";
                    VitesseD.Content = "Vitesse Droite : " + robot.consigneDroite + "%";
                       
                    break;

                case (int)msgFonction.EtapeEnCours:
                    int instant = (((int)msgPayload[1]) << 24) +
                    (((int)msgPayload[2]) << 16) +
                    (((int)msgPayload[3]) << 8) +
                    ((int)msgPayload[4]);
                    textBoxReception.Text += "\nRobot␣State␣:␣" +
                    ((StateRobot)(msgPayload[0])).ToString() + "␣-␣" +
                    instant.ToString() + "␣ms";
                    break;

                case (int)msgFonction.position:
                    robot.timestamp = (((int)msgPayload[0]) << 24) + (((int)msgPayload[1]) << 16)
                    + (((int)msgPayload[2]) << 8) + ((int)msgPayload[3] << 0);

                    robot.positionX = BitConverter.ToSingle(msgPayload, 4);
                    robot.positionY = BitConverter.ToSingle(msgPayload, 8);
                    robot.angleRad = BitConverter.ToSingle(msgPayload, 12);
                    robot.vitesseLin = BitConverter.ToSingle(msgPayload, 16);
                    robot.vitesseAng = BitConverter.ToSingle(msgPayload, 20);

                    tempsCourant.Content = "Temps : " + robot.timestamp;
                    posX.Content = "Position X : " + robot.positionX;
                    posY.Content = "Position Y : " + robot.positionY;
                    angRad.Content = "Angle en radian : " + robot.angleRad;
                    vitLin.Content = "Vitesse linéaire : " + robot.vitesseLin;
                    vitAng.Content = "Vitesse angulaire : " + robot.vitesseAng;

                    break;

                case (int)msgFonction.Corrector:
                    
                    break;
            }
            
        }
       
        byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte Checksum = 0;
            Checksum ^= 0xFE;
            Checksum ^= (byte)(msgFunction >> 8);
            Checksum ^= (byte)(msgFunction >> 0);
            Checksum ^= (byte)(msgPayloadLength >> 8);
            Checksum ^= (byte)(msgPayloadLength >> 0);

            int lg;
            for (lg = 0; lg < msgPayloadLength; lg++)
            {
                Checksum ^= msgPayload[lg];
            }
            return Checksum;
        }
        void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte[] mess = new byte[msgPayloadLength + 6];
            int position = 0;
            mess[position++] = 0xFE;
            mess[position++] = (byte)(msgFunction >> 8);
            mess[position++] = (byte)(msgFunction >> 0);
            mess[position++] = (byte)(msgPayloadLength >> 8);
            mess[position++] = (byte)(msgPayloadLength >> 0);
            int i;
            for (i = 0; i < msgPayloadLength; i++)
            {
                mess[position++] = msgPayload[i];
            }
            mess[position++] = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
            serialPort1.Write(mess, 0, position);
        }
        public enum StateReception
        {
            Waiting,
            FunctionMSB,
            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }


        StateReception rcvState = StateReception.Waiting;
        int msgDecodedFunction = 0;
        int msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        int msgDecodedPayloadIndex = 0;

        private void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:
                if(c == 0xFE)
                    {
                        rcvState = StateReception.FunctionMSB;
                    }
                break;
                case StateReception.FunctionMSB:
                    msgDecodedFunction = (c << 8);
                    rcvState = StateReception.FunctionLSB;
                break;
                case StateReception.FunctionLSB:
                    msgDecodedFunction += (c << 0);
                    rcvState = StateReception.PayloadLengthMSB;
                break;
                case StateReception.PayloadLengthMSB:
                    msgDecodedPayloadLength = (c << 8);
                    rcvState = StateReception.PayloadLengthLSB;
                break;
                case StateReception.PayloadLengthLSB:
                    msgDecodedPayloadLength += (c << 0);
                    msgDecodedPayload = new byte[msgDecodedPayloadLength];
                    msgDecodedPayloadIndex = 0;
                    rcvState = StateReception.Payload;
                break;
                case StateReception.Payload:
                    msgDecodedPayload[msgDecodedPayloadIndex] = c;
                    msgDecodedPayloadIndex++;
                    if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
                    {
                        rcvState = StateReception.CheckSum;
                    }
                        
                break;
                case StateReception.CheckSum:

                if (CalculateChecksum(msgDecodedFunction,msgDecodedPayloadLength, msgDecodedPayload) == c)
                    {
                        //Success, on a un message valide
                        //textBoxReception.Text+= " OK \n";
                        processDecodeMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    }
                    //else
                    //{
                    //    textBoxReception.Text += " Pas OK \n";
                    //}
                    rcvState = StateReception.Waiting;
                    break;
                    default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }
       
        public enum StateRobot
        {
            STATE_ATTENTE = 0,
            STATE_ATTENTE_EN_COURS = 1,
            STATE_AVANCE = 2,
            STATE_AVANCE_EN_COURS = 3,
            STATE_TOURNE_GAUCHE = 4,
            STATE_TOURNE_GAUCHE_EN_COURS = 5,
            STATE_TOURNE_DROITE = 6,
            STATE_TOURNE_DROITE_EN_COURS = 7,
            STATE_TOURNE_SUR_PLACE_GAUCHE = 8,
            STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS = 9,
            STATE_TOURNE_SUR_PLACE_DROITE = 10,
            STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS = 11,
            STATE_ARRET = 12,
            STATE_ARRET_EN_COURS = 13,
            STATE_RECULE = 14,
            STATE_RECULE_EN_COURS = 15
        }
    }
}
