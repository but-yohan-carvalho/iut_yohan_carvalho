using ExtendedSerialPort;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO.Ports;
using System.Windows.Threading;
using SciChart.Charting.Visuals;
using System.Diagnostics;
using MathNet.Spatial.Euclidean;
using SciChart.Charting3D.Model;
using SciChart.Charting3D.PointMarkers;
using System.Linq;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Charting3D.RenderableSeries;
using SciChart.Charting3D;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Model.DataSeries;
using System.Windows.Media.Media3D;
using MathNet.Spatial;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;



using Matrix3D = MathNet.Spatial.Euclidean.Matrix3D;
using SciChart.Charting3D.Axis;
using SciChart.Data.Model;
using SciChart.Charting.ChartModifiers;
using Vector3D = MathNet.Spatial.Euclidean.Vector3D;
using Point3D = MathNet.Spatial.Euclidean.Point3D;

namespace RobotInterfaceNet
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        Robot robot = new Robot();
        DispatcherTimer timerAffichage;
        ReliableSerialPort serialPort1;
        //var dataSeries3D<double> = new XyzDataSeries3D<double>();
        XyzDataSeries3D<double> xyzDataSeries3D = new XyzDataSeries3D<double>();
        ScatterRenderableSeries3D renderSerias = new ScatterRenderableSeries3D();   


        public MainWindow()
        {
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("osuraiMh+/Ur8XOBfDQ8DGxK4LgYsM/LqTbAxL+Zr/plYfLTO8DCQqcE5HEX0FNuCbD4UhyjOWV8n7OfWJPpsgOBUy+YzmjEgegfGB6FT5X/CSO2T/RmZMkumH+dPLGF+MluNzd9wPXBdefHmN5vz7vVIGM+XSqWrTSeQU8sp49g3HTgiFHs1I7zb+h2Dprp+56rEKr0e2FfGpn2n/CTkb/NxfkyYHsYp4+aGYhTE/VjaozHfzPm3/oP6qKO6wSWmzCikHOnk9XKYFPJwZtqKjCR6shU2HaINnKlroD8/1m8v4QWnLDvHk2rLjmekbEcfMD0dRkKKpCo4//gmwtWl9BHR3n75WBLNh4JyReLNUZvMsR/ObrRpcBJOKoi9ALDzW7Y/HE90g8Hqqo31Et/ZoqR68AZzDPXzc1J7VYxBrzgkJEr/YwXxO6pVZsczo3wAVKdDB4qwnv0o6R9URSJOc7wA8cBkiqrz4XxIQUDqqbIN1Z9En/PKpKiJS2a3Zq+n7snJ8vj"); 
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM10", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();
            //oscilloSpeed.AddOrUpdateLine(1, 200, "Ligne1");
            //oscilloSpeed.ChangeLineColor(1, Color.FromRgb(255,0,100));
            
            renderSerias.DataSeries = xyzDataSeries3D;
            renderSerias.PointMarker = new CubePointMarker3D();
            renderSerias.PointMarker.Size = 5;
            SciChart.RenderableSeries.Add(renderSerias);





            var camera = new Camera3D();
            camera.Position = new Vector3 (800, -300, 100);        
            camera.Target = new Vector3 (0,0,0);
            camera.OrbitalPitch = 30;
            camera.OrbitalYaw = -55;  
            SciChart.Camera = camera;





        }

        string receivedText = "";

        List<byte> currentByteList = new List<byte>();

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
                if(c!='\r' && c != '\n')
                {
                    receivedText += Encoding.ASCII.GetString(new byte[] { c });

                    currentByteList.Add(c);
                }
                else if(c == '\r')
                {
                    Debug.WriteLine(receivedText);
                    receivedText = "";

                    DecodeMessage(c);
                    byte[] LastCompleteByteList = new byte[currentByteList.Count];


                    currentByteList.CopyTo(LastCompleteByteList);


                    textBoxReception.Text = Encoding.ASCII.GetString(LastCompleteByteList) + "\n" + textBoxReception.Text;
                    if (textBoxReception.Text.Length > 200)
                        textBoxReception.Text = textBoxReception.Text.Substring(0, 200);
                    currentByteList.Clear();
                }

                    // Vider la liste currentByteList
                   
                

             

               // char msg = (char)c;

              //  string msgString = msg.ToString();

               // serialPort1.Write(msgString);




            }


            //oscilloSpeed.AddPointToLine(1, robot.positionX, robot.positionY);
            //asservSpeedDisplay2.UpdateIndependantOdometrySpeed(robot.positionX, robot.positionY);
            //asservSpeedDisplay2.UpdatePolarOdometrySpeed(robot.vitesseLin, robot.vitesseAng);
            //asservSpeedDisplay2.UpdatePolarSpeedConsigneValues(robot.consigneGauche, robot.consigneDroite);
           
        }

        List<byte> byteListReceived = new List<byte>();
        List<Point3D>trajectoire = new List<Point3D>();

        //private (Point3D, double) Prediction(List<Point3D> trajectoire)
        //{
        //    int nbPtsUsedForPrediction = 5;
        //    if (trajectoire.Count > nbPtsUsedForPrediction)
        //    {
        //        var trajRecente = trajectoire.Reverse<Point3D>().Take(nbPtsUsedForPrediction).Reverse<Point3D>();

        //        double[] ptX = trajectoire.Select(x => x.X).ToArray();
        //        double[] ptY = trajectoire.Select(y => y.Y).ToArray();
        //        double[] ptZ = trajectoire.Select(z => z.Z).ToArray();

        //        double[] ptT = new double[nbPtsUsedForPrediction];



        //    }

        //}


        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            ///robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            foreach (var c in e.Data) {
                if (c != '\r')
                {
                    if (c != '\n')
                        byteListReceived.Add(c);
                }
                else
                {
                    /// On a une trame complète
                    string s = Encoding.ASCII.GetString(byteListReceived.ToArray());
                    Console.WriteLine(s);
                    try
                    {
                        /// Des fois, ça merde, car on a des trames d'init de la caméra qui arrivent
                        var sp = s.Split(' ');
                        BallPerception bp = new BallPerception();


                        bp.xImage = int.Parse(sp[0]) - 1920 / 2;
                        bp.yImage = 540 - int.Parse(sp[1]);
                        bp.diameterImage = int.Parse(sp[2]);



                        Console.WriteLine("X : " + bp.xImage + " Y : " + bp.yImage + " Diameter : " + bp.diameterImage);

                        ///Calcul des angles alpha et theta :
                        ///alpha est la tan-1 de la distance de la balle au centre en pixels / distance au centre d'un pt à 45°
                        var distanceBalleCentrePx = Math.Sqrt(bp.xImage * bp.xImage + bp.yImage * bp.yImage);

                        bp.alphaAngle = Math.Atan2(distanceBalleCentrePx, 612);

                        Console.WriteLine("Alpha : " + bp.alphaAngle * 180 / Math.PI);

                        /// theta est la tan-1 de xball / yball
                        bp.thetaAngle = Math.Atan2(-bp.xImage, bp.yImage);
                        Console.WriteLine("Theta : " + bp.thetaAngle * 180 / Math.PI);

                        /// Reste à faire les rotations 3D de l'axe optique pour obtenir
                        /// 1 - l'axe dans le plan vertical de la paroi du cône qui est une rotation d'angle alpha de l'axe optique autour de l'axe Y

                        //Vector3D opticalAxis = new Vector3D(1, 0, 0); anncienne

                        //var matRotY = Matrix3D.RotationAroundYAxis(MathNet.Spatial.Units.Angle.FromRadians(bp.alphaAngle));
                        //Vector3D rotatedAxis = opticalAxis.TransformBy(matRotY);

                        //var matRottheta = Matrix3D.RotationAroundArbitraryVector(opticalAxis.Normalize(), MathNet.Spatial.Units.Angle.FromRadians(bp.thetaAngle));
                        //var axeObjet = rotatedAxis.TransformBy(matRottheta);



                        Vector3D opticalAxis = new Vector3D(1, 0, 0);

                        var matRotY = Matrix3D.RotationAroundYAxis(MathNet.Spatial.Units.Angle.FromRadians(-bp.thetaAngle));
                        Vector3D rotatedAxis = opticalAxis.TransformBy(matRotY);

                        var matRottheta = Matrix3D.RotationAroundArbitraryVector(opticalAxis.Normalize(), MathNet.Spatial.Units.Angle.FromRadians(bp.alphaAngle));
                        var axeObjet = rotatedAxis.TransformBy(matRottheta);


                        //// Récupérer les axes de la surface SciChart
                        SciChart.Dispatcher.Invoke(() =>
                        {
                            var xAxis = SciChart.XAxis as NumericAxis3D;
                            var yAxis = SciChart.YAxis as NumericAxis3D;
                            var zAxis = SciChart.ZAxis as NumericAxis3D;

                            //// Définir les nouvelles plages (échelles)
                            xAxis.VisibleRange = new DoubleRange(-1, 5);
                            yAxis.VisibleRange = new DoubleRange(-2, 2);
                            zAxis.VisibleRange = new DoubleRange(-2, 2);
                        });



                        if (bp.diameterImage != 0)
                        {
                            double distance = 100.0 / bp.diameterImage;

                            Vector3D ballpos = distance * axeObjet;
                            trajectoire.Add(new Point3D(ballpos.X, ballpos.Y, ballpos.Z));
                            Console.WriteLine($"ballposition: Xi ={ballpos.X},Yi={ballpos.Y},Zi={ballpos.Z} \n");
                        //Console.WriteLine("\n Xi =" + ballpos.X,"Yi=" + ballpos.Y, "Zi=" + ballpos.Z);

                        while (trajectoire.Count > 50)
                            {
                                trajectoire.RemoveAt(0);
                            }

                            xyzDataSeries3D.Clear();
                            xyzDataSeries3D.Append(
                                trajectoire.Select(o => o.X).ToList(),
                                trajectoire.Select(o => o.Y).ToList(),
                                trajectoire.Select(o => o.Z).ToList());
                            SciChart.InvalidateArrange();
                           

                        }



                        //var lineSeries3D = new PointLineRenderableSeries3D
                        //{
                        //    DataSeries = dataSeries3D,
                        //    Stroke = Colors.Yellow,
                        //    StrokeThickness = 2
                        //};

                        var marker3D = new PointMarker
                        {

                            //};
                            // Console.WriteLine($"Rotated Axis: Xi ={rotatedAxis.X},Yi={rotatedAxis.Y},Zi={rotatedAxis.Z}");

                        };
                    }
                    catch
                    {

                    }
                    byteListReceived.Clear();
                }
                //robot.byteListReceived.Enqueue(c);
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
            textBoxReception.Text = receivedText;

        }


         public void functionRecu(int a)
         {
            
             //textBoxReception.Text = receivedText;
             //serialPort1.WriteLine(textBoxEmission.Text);
             //if (a == 0)
             //{
             //    textBoxReception.Text += "Reçu envoyer: " + textBoxEmission.Text + "\n";
             //    textBoxEmission.Text = "";
             //}
             //else if (a == 1)
             //{
             //    textBoxReception.Text += "Reçu via envoyer: " + textBoxEmission.Text + "\n";
             //    textBoxEmission.Text = "";
             //}

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

            /* string s = "Bonjour";
             byte[] byteList;
             byteList = Encoding.ASCII.GetBytes(s);
             UartEncodeAndSendMessage(0x0080, 7, byteList);
             //textBoxReception.Text = s;*/

            float kp = 0;
            float ki = 1;
            float kd = 2;
            float propmax = 3;
            float intmax = 4;
            float dermax = 5;
            float consigne = 6;

            byte[] tabasx = new byte[30];
            Array.Copy(BitConverter.GetBytes(kp), 0, tabasx, 0, 4);
            Array.Copy(BitConverter.GetBytes(ki), 0, tabasx, 4, 4);
            Array.Copy(BitConverter.GetBytes(kd), 0, tabasx, 8, 4);
            Array.Copy(BitConverter.GetBytes(propmax), 0, tabasx, 12, 4);
            Array.Copy(BitConverter.GetBytes(intmax), 0, tabasx, 16, 4);
            Array.Copy(BitConverter.GetBytes(dermax), 0, tabasx, 20, 4);
            Array.Copy(BitConverter.GetBytes(consigne), 0, tabasx, 24, 4);


            UartEncodeAndSendMessage(0x0063, 30, tabasx);
            //asservSpeedDisplay2.UpdatePolarSpeedErrorValues(erreurX, erreurT);
            //asservSpeedDisplay2.UpdatePolarOdometrySpeed(kpX, kpT);
        }
        public enum msgFonction
        {
            transmissionText = 0x0080,
            ReglageLed = 0x0020,
            DistanceTelemeter = 0x0030,
            ConsigneVitesse = 0x0040,
            EtapeEnCours = 0x0050,
            position = 0x0061,
            Corrector = 0x0063,

        }
        float kpX = 0;
        float kiX = 0;
        float kdX = 0;
        float erreurX = 0;
        float consigneX = 0;
        float propmaxX = 0;
        float intmaxX = 0;
        float dermaxX = 0;
        float corPX = 0;
        float corIX = 0;
        float corDX = 0;

        float kpT = 0;
        float kiT = 0;
        float kdT = 0;
        float erreurT = 0;
        float consigneT = 0;
        float propmaxT = 0;
        float intmaxT = 0;
        float dermaxT = 0;
        float corPT = 0;
        float corIT = 0;
        float corDT = 0;

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

                   // tempsCourant.Content = "Temps : " + robot.timestamp;
                  //  posX.Content = "Position X : " + robot.positionX;
                  //  posY.Content = "Position Y : " + robot.positionY;
                  //  angRad.Content = "Angle en radian : " + robot.angleRad;
                 //   vitLin.Content = "Vitesse linéaire : " + robot.vitesseLin;
                 //   vitAng.Content = "Vitesse angulaire : " + robot.vitesseAng;

                    break;

                case (int)msgFonction.Corrector:
                    kpX = BitConverter.ToSingle(msgPayload, 4);
                    kiX = BitConverter.ToSingle(msgPayload, 8);
                    kdX = BitConverter.ToSingle(msgPayload, 12);
                    propmaxX = BitConverter.ToSingle(msgPayload, 16);
                    intmaxX = BitConverter.ToSingle(msgPayload, 20);
                    dermaxX = BitConverter.ToSingle(msgPayload, 24);
                    erreurX = BitConverter.ToSingle(msgPayload, 28);
                    consigneX = BitConverter.ToSingle(msgPayload, 32);
                    corPX = BitConverter.ToSingle(msgPayload, 36);
                    corIX = BitConverter.ToSingle(msgPayload, 40);
                    corDX = BitConverter.ToSingle(msgPayload, 44);

                    kpT = BitConverter.ToSingle(msgPayload, 48);
                    kiT = BitConverter.ToSingle(msgPayload, 52);
                    kdT = BitConverter.ToSingle(msgPayload, 56);
                    propmaxT = BitConverter.ToSingle(msgPayload, 60);
                    intmaxT = BitConverter.ToSingle(msgPayload, 64);
                    dermaxT = BitConverter.ToSingle(msgPayload, 68);
                    erreurT = BitConverter.ToSingle(msgPayload, 72);
                    consigneT = BitConverter.ToSingle(msgPayload, 76);
                    corPT = BitConverter.ToSingle(msgPayload, 80);
                    corIT = BitConverter.ToSingle(msgPayload, 84);
                    corDT = BitConverter.ToSingle(msgPayload, 88);

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

    public class BallPerception
    {
        public int xImage;
        public int yImage;
        public int diameterImage;

        public double diameter;
        public double alphaAngle;
        public double thetaAngle;

        public double xPosRefCamera;
        public double yPosRefCamera;
        public double zPosRefCamera;
    }


  
 //class Vector3D
 //   {
 //       public double Xi, Yi, Zi;
        
 //       public Vector3D(double x, double y, double z)
 //       {
 //           Xi = x;
 //           Yi = y;
 //           Zi = z;
 //       }
 //       public Vector3D RotateAroundY(double angleRadians)
 //       {
 //           double cosTheta = Math.Cos(angleRadians);
 //           double sinTheta = Math.Sin(angleRadians);

 //           double newX = cosTheta * Xi+sinTheta*Zi;
 //           double newY = Yi;
 //           double newZ = -sinTheta * Xi+cosTheta*Zi;

 //           return new Vector3D(newX, newY, newZ);



 //       }

 //   }     
   

}