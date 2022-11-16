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
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
           
            
        }

        string receivedText="";

        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            /* if(robot.receivedText != "")
              {
                  textBoxReception.Text += robot.receivedText;
                  robot.receivedText = "";                 
              }*/
            while (robot.byteListReceived.Count != 0)
            {
                var c = robot.byteListReceived.Dequeue();
                textBoxReception.Text += "0x" + c.ToString("X2") + "        ";
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
            textBoxReception.Text += "\nReçu via envoyer: " + textBoxEmission.Text;
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
        }
    

        private void textBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                textBoxReception.Text += "\nReçu via entrer: " + textBoxEmission.Text;
                textBoxEmission.Text = "";
            }
           
        }
        private void textBoxReception_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                textBoxReception.Text = "";
            }

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
           
            byte[] byteList = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                byteList[i] = (byte)(2 * i);               
            }
            serialPort1.Write(byteList, 0, 20);
        }
    }
}
