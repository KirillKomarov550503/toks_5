using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace TokenRing
{
    public partial class Form1 : Form
    {
        private Thread thread1;
        private Thread thread2;
        private Thread thread3;

        private byte[] package;
        private byte stationNumber;

        private Station station1;

        private Station station2;

        private Station station3;

        private string stationName;

        private byte dataByte;

        private bool isByteReady;

        private List<Wait> deque = new List<Wait>();

        private void SendDataMonitor()
        {
            while (true)
            {
                Thread.Sleep(10);
            }
        }
        public void StationWork1()
        {
            while (true)
            {
                if (package != null && stationNumber == 3)
                {

                    this.Invoke((MethodInvoker)(delegate
                    {
                        stationNumber = 0;
                        textBox3.Text = station1.ReceivedMessage1;
                        textBox2.Text = "*";
                        package = station1.ReceivedMessage(package);

                    }));
                    Thread.Sleep(1000);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox2.Text = "";
                        if (stationName == "1")
                        {
                            if (station1.IsFrameReturn)
                            {
                                while (!isByteReady) ;
                                isByteReady = false;
                                station1.SourceAddress = 1;
                                station1.DestinationAddress = Convert.ToByte(textBox7.Text);
                                package = station1.SendMessage(package, dataByte);
                                station1.IsFrameReturn = false;
                            }
                        }
                        stationNumber = 1;
                    }));
                }
                Thread.Sleep(50);
            }
        }

        public void StationWork2()
        {
            while (true)
            {
                if (package != null && stationNumber == 1)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        stationNumber = 0;
                        package = station2.ReceivedMessage(package);
                        textBox11.Text = "*";
                        textBox10.Text = station2.ReceivedMessage1;

                    }));
                    Thread.Sleep(1000);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox11.Text = "";
                        if (stationName == "2")
                        {
                            if (station1.IsFrameReturn)
                            {
                                while (!isByteReady) ;
                                isByteReady = false;
                                station1.SourceAddress = 10;
                                station1.DestinationAddress = Convert.ToByte(textBox8.Text);
                                package = station2.SendMessage(package, dataByte);
                                station1.IsFrameReturn = false;
                            }
                        }
                        stationNumber = 2;
                    }));
                }
                Thread.Sleep(50);
            }
        }

        public void StationWork3()
        {
            while (true)
            {
                if (package != null && stationNumber == 2)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        stationNumber = 0;
                        package = station3.ReceivedMessage(package);
                        textBox5.Text = "*";
                        textBox4.Text = station3.ReceivedMessage1;
                        if (station3.IsTerminate)
                        {
                            textBox5.Text += "\r\nFix frame";
                        }
                    }));
                    if (station3.IsTerminate)
                    {
                        station3.IsTerminate = true;
                        Thread.Sleep(2000);
                    }
                    Thread.Sleep(1000);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox5.Text = "";
                        if (stationName == "3")
                        {
                            if (station1.IsFrameReturn)
                            {
                                while (!isByteReady) ;
                                isByteReady = false;
                                station1.SourceAddress = 100;
                                station1.DestinationAddress = Convert.ToByte(textBox9.Text);
                                package = station3.SendMessage(package, dataByte);
                                station1.IsFrameReturn = false;
                            }
                        }

                        stationNumber = 3;

                    }));
                }
                Thread.Sleep(50);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private int count = 0;
        private void Station1Write()
        {
            while (true)
            {
                if (stationName == "1")
                {
                    if (station1.IsFrameReturn)
                    {
                        if (count < textBox1.Text.Length)
                            dataByte = (byte)textBox1.Text[count];
                        else
                            dataByte = 0;
                        isByteReady = true;
                    }
                }
            }
        }

        private void Station2Write()
        {
            while (true)
            {
                if (stationName == "2")
                {
                    if (station2.IsFrameReturn)
                    {
                        if (count < textBox12.Text.Length)
                            dataByte = (byte)textBox12.Text[count];
                        else
                            dataByte = 0;
                        isByteReady = true;
                    }
                }
            }
        }

        private void Station3Write()
        {
            while (true)
            {
                if (stationName == "3")
                {
                    if (station3.IsFrameReturn)
                    {
                        if (count < textBox6.Text.Length)
                            dataByte = (byte)textBox6.Text[count];
                        else
                            dataByte = 0;
                        isByteReady = true;
                    }
                }
            }
        }
        private Thread dataSendThread1;
        public void Station1Write(object sender, MouseEventArgs e)
        {
            dataSendThread1 = new Thread(Station1Write);
            dataSendThread1.Start();
        }

        private Thread dataSendThread2;

        public void Station2Write(object sender, MouseEventArgs e)
        {
            dataSendThread2 = new Thread(Station2Write);
            dataSendThread2.Start();
        }

        private Thread dataSendThread3;

        public void Station3Write(object sender, MouseEventArgs e)
        {
            dataSendThread3 = new Thread(Station3Write);
            dataSendThread3.Start();
        }

        public void CreateToken(object sender, MouseEventArgs e)
        {
            package = new byte[6];
            for (int i = 0; i < 6; i++)
                package[i] = 0;
            stationNumber = 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stationName = "";
            station1 = new Station(1, false);
            station2 = new Station(10, false);
            station3 = new Station(100, true);
            button1.MouseClick += Station1Write;
            button2.MouseClick += Station2Write;
            button3.MouseClick += Station3Write;
            button4.MouseClick += CreateToken;
            thread1 = new Thread(StationWork1);
            thread1.Start();
            thread2 = new Thread(StationWork2);
            thread2.Start();
            thread3 = new Thread(StationWork3);
            thread3.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread1.Abort();
            thread2.Abort();
            thread3.Abort();
            dataSendThread1.Abort();
            dataSendThread2.Abort();
            dataSendThread3.Abort();
        }
    }
}
