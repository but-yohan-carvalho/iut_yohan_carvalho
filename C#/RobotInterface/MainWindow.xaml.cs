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
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();
            
        }

        string receivedText="";

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
                textBoxReception.Text += "0x" + c.ToString("X2") + " ";
            }
           
        }
        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            ///robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            foreach (var c in e.Data){
                robot.byteListReceived.Enqueue(c);
            }
        }

        private void TextBoxReception_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        bool toggle = false;
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            
            textBoxReception.Text += "Reçu envoyer: " + textBoxEmission.Text + "\n";
            serialPort1.WriteLine(textBoxEmission.Text);
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
                textBoxEmission.Text = "";
            }
        }
        private void textBoxReception_KeyUp(object sender, KeyEventArgs e)
        {
            

        }


       /* public void functionRecu(int a)
        {
            textBoxReception.Text = receivedText;
            serialPort1.WriteLine(textBoxEmission.Text);
            if (a == 0)
            {
                textBoxReception.Text += "\nReçu via entrer: " + textBoxEmission.Text;
                textBoxEmission.Text = "";
            }
            else if (a == 1)
            {
                textBoxReception.Text += "\nReçu via envoyer: " + textBoxEmission.Text;
                textBoxEmission.Text = "";
            }
            
        }*/

        private void buttonclear_Click(object sender, RoutedEventArgs e)
        {
            textBoxReception.Text = "";
        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            string s = "Bonjour";

            byte[] byteList;// = new byte[20];
            byteList = Encoding.ASCII.GetBytes(s);
            /* for (int i = 0; i < 20; i++)
             {
                 //byteList[i] = (byte)(2 * i);  

             }*/
            UartEncodeAndSendMessage(0x0080, byteList.Length, byteList);

        }
        byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte [] msgPayload)
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
            byte[] mess = new byte [msgPayloadLength+6];
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
            serialPort1.Write(mess,0,position);
        }
    }
}
