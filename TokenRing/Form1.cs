using System;
using System.Collections.Generic;
using System.Text;
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
        private byte activeStation;

        private Station station1;

        private Station station2;

        private Station station3;

        private string stationName;

        private byte dataByte;

        private bool isByteReady;

        private List<Wait> queue = new List<Wait>();

        private void SendDataMonitor()
        {
            while (true)
            {
                Thread.Sleep(10);
            }
        }
        private bool IsNeedWrite;
        public void StationWork1()
        {
            while (true)
            {
                if (package != null && activeStation == 3)
                {

                    this.Invoke((MethodInvoker)(delegate
                    {
                        activeStation = 0;
                        textBox3.Text = station1.ReceivedMessage1;
                        textBox2.Text = "*\r\n";
                        package = station1.ReceivedMessage(package);
                        if (station1.IsFinishReceive && queue.Count > 0)
                        {
                            queue.RemoveAt(0);
                            station1.IsFinishReceive = false;
                        }
                    }));
                    Thread.Sleep(1000);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox2.Text = "";
                        if (queue.Count > 0 && queue[0].SourceAddress == 1)
                            IsNeedWrite = true;
                        while (IsNeedWrite) ;
                        activeStation = 1;
                    }));

                    //this.Invoke((MethodInvoker)(delegate
                    //{
                    //    textBox2.Text = "";
                    //    if (queue.Count > 0 && queue[0].StationAddress == 1)
                    //    {
                    //        if (station1.IsFrameReturn)
                    //        {
                    //            while (!isByteReady) ;
                    //            isByteReady = false;
                    //            station1.SourceAddress = 1;
                    //            station1.DestinationAddress = Convert.ToByte(textBox7.Text);
                    //            package = station1.SendMessage(package, dataByte);
                    //            station1.IsFrameReturn = false;
                    //            position++;
                    //        }
                    //    }
                    //    
                    //}));
                }
                Thread.Sleep(50);
            }
        }

        public void StationWork2()
        {
            while (true)
            {
                if (package != null && activeStation == 1)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        activeStation = 0;
                        package = station2.ReceivedMessage(package);
                        textBox11.Text = "*\r\n";
                        textBox10.Text = station2.ReceivedMessage1;
                        if (station2.IsFinishReceive && queue.Count > 0)
                        {
                            queue.RemoveAt(0);
                            station2.IsFinishReceive = false;
                        }
                    }));
                    Thread.Sleep(1000);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox11.Text = "";
                        if (queue.Count > 0 && queue[0].SourceAddress == 10)
                            IsNeedWrite = true;
                        while (IsNeedWrite) ;
                        activeStation = 2;
                    }));
                }
                Thread.Sleep(50);
            }
        }

        public void StationWork3()
        {
            while (true)
            {
                if (package != null && activeStation == 2)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        activeStation = 0;
                        package = station3.ReceivedMessage(package);
                        textBox5.Text = "*\r\n";
                        textBox4.Text = station3.ReceivedMessage1;
                        if (station3.IsTerminate)
                        {
                            textBox5.Text += "Fix frame\r\n";
                        }
                        if (station3.IsFinishReceive && queue.Count > 0)
                        {
                            queue.RemoveAt(0);
                            station3.IsFinishReceive = false;
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
                        if (queue.Count > 0 && queue[0].SourceAddress == 100)
                            IsNeedWrite = true;
                        while (IsNeedWrite) ;
                        activeStation = 3;
                    }));
                    //this.Invoke((MethodInvoker)(delegate
                    //{
                    //    textBox5.Text = "";
                    //    if (queue.Count > 0 && queue[0].StationAddress == 100)
                    //    {
                    //        if (station1.IsFrameReturn)
                    //        {
                    //            while (!isByteReady) ;
                    //            isByteReady = false;
                    //            station1.SourceAddress = 100;
                    //            station1.DestinationAddress = Convert.ToByte(textBox9.Text);
                    //            package = station3.SendMessage(package, dataByte);
                    //            station1.IsFrameReturn = false;
                    //            position++;
                    //        }
                    //    }
                    //}));
                }
                Thread.Sleep(50);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private int position = 0;
        private void Station1Write()
        {
            while (true)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (IsNeedWrite)
                    {
                        if (queue.Count > 0 && queue[0].SourceAddress == 1)
                        {

                            if (station1.IsFrameReturn)
                            {
                                if (position < queue[0].Message.Length)
                                {
                                    station1.SourceAddress = queue[0].SourceAddress;
                                    station1.DestinationAddress = queue[0].DestinationAddress;
                                    package = station1.SendMessage(package, queue[0].Message[position]);
                                    station1.IsFrameReturn = false;
                                    position++;
                                }
                                else
                                {
                                    dataByte = 0;
                                    position = 0;
                                }
                                isByteReady = true;
                                IsNeedWrite = false;
                            }
                        }
                    }
                }));
                Thread.Sleep(50);
            }
        }

        private void Station2Write()
        {

            while (true)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (IsNeedWrite)
                    {
                        if (queue.Count > 0 && queue[0].SourceAddress == 10)
                        {
                            if (station2.IsFrameReturn)
                            {
                                if (position < queue[0].Message.Length)
                                {
                                    station2.SourceAddress = queue[0].SourceAddress;
                                    station2.DestinationAddress = queue[0].DestinationAddress;
                                    package = station2.SendMessage(package, queue[0].Message[position]);
                                    station2.IsFrameReturn = false;
                                    position++;
                                }
                                else
                                {
                                    dataByte = 0;
                                    position = 0;
                                }
                                isByteReady = true;
                                IsNeedWrite = false;
                            }
                        }
                    }
                }));
                Thread.Sleep(50);
            }

        }

        private void Station3Write()
        {
            while (true)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (IsNeedWrite)
                    {
                        if (queue.Count > 0 && queue[0].SourceAddress == 100)
                        {

                            if (station3.IsFrameReturn)
                            {
                                if (position < queue[0].Message.Length)
                                {
                                    station3.SourceAddress = queue[0].SourceAddress;
                                    station3.DestinationAddress = queue[0].DestinationAddress;
                                    package = station3.SendMessage(package, queue[0].Message[position]);
                                    station3.IsFrameReturn = false;
                                    position++;
                                }
                                else
                                {
                                    dataByte = 0;
                                    position = 0;
                                }
                                isByteReady = true;
                                IsNeedWrite = false;
                            }
                        }
                    }
                }));
                Thread.Sleep(50);
            }

        }
        private Thread dataSendThread1;
        public void Station1Write(object sender, MouseEventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (textBox1.Text.Length == 0 || textBox7.Text.Length == 0)
                {
                    textBox2.Text += "Empty \"Input\" or \"Destination address\" field";
                    return;
                }

                queue.Add(new Wait(1, Convert.ToByte(textBox7.Text),
                    Encoding.ASCII.GetBytes(textBox1.Text)));
                //IsNeedWrite = true;
            }));

        }

        private Thread dataSendThread2;

        public void Station2Write(object sender, MouseEventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (textBox8.Text.Length == 0 || textBox12.Text.Length == 0)
                {
                    textBox11.Text += "Empty \"Input\" or \"Destination address\" field";
                    return;
                }
                queue.Add(new Wait(10, Convert.ToByte(textBox8.Text),
                    Encoding.ASCII.GetBytes(textBox12.Text)));
                //IsNeedWrite = true;

            }));
        }

        private Thread dataSendThread3;

        public void Station3Write(object sender, MouseEventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (textBox9.Text.Length == 0 || textBox6.Text.Length == 0)
                {
                    textBox5.Text += "Empty \"Input\" or \"Destination address\" field";
                    return;
                }
                //IsNeedWrite = true;
                queue.Add(new Wait(100, Convert.ToByte(textBox9.Text),
                    Encoding.ASCII.GetBytes(textBox6.Text)));
            }));
        }

        public void CreateToken(object sender, MouseEventArgs e)
        {
            package = new byte[6];
            for (int i = 0; i < 6; i++)
                package[i] = 0;
            activeStation = 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stationName = "";
            IsNeedWrite = false;
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
            dataSendThread1 = new Thread(Station1Write);
            dataSendThread1.Start();
            dataSendThread2 = new Thread(Station2Write);
            dataSendThread2.Start();
            dataSendThread3 = new Thread(Station3Write);
            dataSendThread3.Start();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread1.Abort();
            thread2.Abort();
            thread3.Abort();
            if (dataSendThread1 != null)
                dataSendThread1.Abort();
            if (dataSendThread2 != null)
                dataSendThread2.Abort();
            if (dataSendThread3 != null)
                dataSendThread3.Abort();
        }
    }
}
