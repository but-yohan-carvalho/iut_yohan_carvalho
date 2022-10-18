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


namespace RobotInterfaceNet
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReliableSerialPort serialPort1;
        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
            
        }

        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            textBoxReception.Text += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
        }

        private void TextBoxReception_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        bool toggle = false;
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            functionRecu(1);
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
                functionRecu(0);
                
            }
        }
        public void functionRecu(int a)
        {
            serialPort1.WriteLine(textBoxEmission.Text);
            if (a == 0)
            {
                textBoxReception.Text += "Reçu via entrer: " + textBoxEmission.Text;
                textBoxEmission.Text = "";
            }
            else if (a == 1)
            {
                textBoxReception.Text += "\nReçu via envoyer: " + textBoxEmission.Text;
                textBoxEmission.Text = "";
            }
        }
    }
}
