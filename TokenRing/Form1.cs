using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using System.Windows.Forms;

namespace TokenRing
{
    public partial class Form1 : Form
    {
        private byte destinationAddress;
        public void ReceivedMessage(bool isMonitor, byte sourceAddress, TextBox textBox)
        {
            Thread.Sleep(1000);
            Console.Write("Package: ");
            foreach (byte b in package)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
            if (isMonitor)
            {
                if (package[0] == 1)
                {
                    if (package[1] == sourceAddress)
                    {
                        this.Invoke((MethodInvoker)(delegate
                        {
                            package[3] = 1;
                            textBox.Text += Encoding.ASCII.GetString(new byte[] { package[5] });
                        }));
                        return;
                    }
                    else
                    {
                        if (package[4] == 0)
                        {
                            this.Invoke((MethodInvoker)(delegate
                            {
                                package[4] = 1;
                            }));
                            return;
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)(delegate
                            {
                                for (int i = 0; i < 6; i++)
                                    package[i] = 0;
                                textBox5.Text += "Fix ring\r\n";
                                queue.RemoveAt(0);
                                position = 0;
                            }));
                            Thread.Sleep(1000);
                            return;
                        }
                    }
                }
                else return;
            }
            else
            {
                if (package[0] == 1)
                {
                    if (package[1] == sourceAddress)
                    {
                        this.Invoke((MethodInvoker)(delegate
                        {
                            package[3] = 1;
                            textBox.Text += Encoding.ASCII.GetString(new byte[] { package[5] });
                        }));
                    }
                    else return;

                }
                else return;
            }
            return;
        }

        public void SendMessage(byte bt, byte destinationAddress, byte sourceAddress)
        {
            if (bt == 0)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    string separator = "\r\n";
                    switch (this.destinationAddress)
                    {
                        case 1: textBox3.Text += separator; break;
                        case 10: textBox10.Text += separator; break;
                        case 100: textBox4.Text += separator; break;
                        default:break;
                    }
                    package[0] = 0;
                    package[1] = 0;
                    package[2] = 0;
                    package[3] = 0;
                    package[4] = 0;
                    package[5] = 0;
                }));
            }
            else
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    package[0] = 1;
                    package[1] = destinationAddress;
                    package[2] = sourceAddress;
                    package[3] = 0;
                    package[4] = 0;
                    package[5] = bt;
                    this.destinationAddress = destinationAddress;
                }));

            }
        }

        private Thread thread1;
        private Thread thread2;
        private Thread thread3;

        private volatile byte[] package;
        private byte activeStation;



        private List<Wait> queue = new List<Wait>();


        public void StationWork1()
        {
            while (true)
            {
                if (package != null && activeStation == 3)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        activeStation = 0;
                        textBox2.Text = "*\r\n";

                    }));
                    ReceivedMessage(false, 1, textBox3);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox2.Text = "";
                        if (queue.Count > 0 && queue[0].SourceAddress == 1)
                        {
                            if (position == 0)
                            {
                                StationWrite(1);
                            }
                            else
                            {
                                StationWrite(package[3]);
                            }
                        }
                    }));
                    activeStation = 1;
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
                        textBox11.Text = "*\r\n";
                    }));
                    ReceivedMessage(false, 10, textBox10);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox11.Text = "";
                        if (queue.Count > 0 && queue[0].SourceAddress == 10)
                        {
                            if (position == 0)
                            {
                                StationWrite(1);
                            }
                            else
                            {
                                StationWrite(package[3]);
                            }
                        }
                    }));
                    activeStation = 2;

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
                    foreach (Wait wait in queue)
                    {
                        Console.WriteLine("Station: " + wait.SourceAddress + " ; Message: " + wait.Message);
                    }
                    this.Invoke((MethodInvoker)(delegate
                    {
                        activeStation = 0;
                        textBox5.Text = "*\r\n";
                    }));
                    ReceivedMessage(true, 100, textBox4);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox5.Text = "";
                        if (queue.Count > 0 && queue[0].SourceAddress == 100)
                        {
                            if (position == 0)
                            {
                                StationWrite(1);
                            }
                            else
                            {
                                StationWrite(package[3]);
                            }
                        }
                    }));
                    activeStation = 3;
                }
                Thread.Sleep(50);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private int position = 0;
        private void StationWrite(byte status)
        {
            if (status == 1)
            {
                if (position < queue[0].Message.Length)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        SendMessage(queue[0].Message[position],
                        queue[0].DestinationAddress, queue[0].SourceAddress);
                        position++;
                        Console.WriteLine("Position: " + position);
                    }));

                }
                else
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        SendMessage(0, 0, 0);
                        queue.RemoveAt(0);
                        position = 0;
                    }));
                }
            }
        }


        public void Station1WriteEvent(object sender, MouseEventArgs e)
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
            }));

        }

        public void Station2WriteEvent(object sender, MouseEventArgs e)
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
            }));
        }

        public void Station3WriteEvent(object sender, MouseEventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (textBox9.Text.Length == 0 || textBox6.Text.Length == 0)
                {
                    textBox5.Text += "Empty \"Input\" or \"Destination address\" field";
                    return;
                }
                queue.Add(new Wait(100, Convert.ToByte(textBox9.Text),
                    Encoding.ASCII.GetBytes(textBox6.Text)));
            }));
        }

        public void CreateToken(object sender, MouseEventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate
            {
                package = new byte[6];
                for (int i = 0; i < 6; i++)
                    package[i] = 0;
                activeStation = 2;
            }));
            button4.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.MouseClick += Station1WriteEvent;
            button2.MouseClick += Station2WriteEvent;
            button3.MouseClick += Station3WriteEvent;
            button4.MouseClick += CreateToken;
            thread1 = new Thread(StationWork1);
            thread1.Start();
            thread2 = new Thread(StationWork2);
            thread2.Start();
            thread3 = new Thread(StationWork3);
            thread3.Start();


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread1.Abort();
            thread2.Abort();
            thread3.Abort();
        }
    }
}
