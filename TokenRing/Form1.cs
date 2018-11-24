using System;
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

        public void StationWork1()
        {
            while (true)
            {
                if (package != null && stationNumber == 3)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        stationNumber = 0;
                        byte[] pack = station1.ReceivedMessage(package);
                        if (stationName == "1")
                        {
                            if (station1.IsFrameReturn)
                            {
                                while (!isByteReady) ;
                                isByteReady = false;
                                package = station1.SendMessage(pack, dataByte);
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
                        byte[] pack = station2.ReceivedMessage(package);
                        if (stationName == "2")
                        {
                            if (station1.IsFrameReturn)
                            {
                                while (!isByteReady) ;
                                isByteReady = false;
                                package = station2.SendMessage(pack, dataByte);
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
                        byte[] pack = station2.ReceivedMessage(package);
                        if (stationName == "3")
                        {
                            if (station1.IsFrameReturn)
                            {
                                while (!isByteReady) ;
                                isByteReady = false;
                                package = station3.SendMessage(pack, dataByte);
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
        public void Station1Write(object sender, MouseEventArgs e)
        {
            Thread dataSendThread = new Thread(Station1Write);
            dataSendThread.Start();
        }

        public void Station2Write(object sender, MouseEventArgs e)
        {
            Thread dataSendThread = new Thread(Station2Write);
            dataSendThread.Start();
        }

        public void Station3Write(object sender, MouseEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.MouseClick += Station1Write;
            button2.MouseClick += Station2Write;
            button3.MouseClick += Station2Write;
        }
    }
}
