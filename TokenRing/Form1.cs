using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using System.Windows.Forms;

namespace TokenRing
{
    public partial class Form1 : Form
    {
        //public byte[] ReceivedMessage(byte[] package, bool isMonitor)
        //{
        //    if (isMonitor) // Если станция является монитором
        //    {
        //        if (package[0] == 1) // Если полученый пакет - это кадр
        //        {
        //            if (package[4] == 0)// Если байт "монитор" равен нулю, то меняем значение на 1
        //            {
        //                package[4] = 1;
        //            }
        //            else // Иначе пакет прошел всё кольцо, не найдя адрес станции получения, то чтобы избежать зацикливания, мы инициируем исправление кольца
        //            {
        //                for (int i = 0; i < 6; i++)
        //                    package[0] = 0;
        //                textBox5.Text += "Fix frame\r\n";
        //                //return package;
        //            }
        //        }
        //    }
        //    if (package[0] == 1) // Если получен кадр
        //    {
        //        if (package[3] == 1) // если байт статус равен 1, то станция приемник считала байт данных
        //        {
        //            if (package[2] == sourceAddress) //  если адрес источника в пакете равен адресу станции
        //            {
        //                isFrameReturn = true;

        //            }

        //        }
        //        if (package[1] == sourceAddress) //Если пакет предназначен для данной станции
        //        {

        //            if (senderAddress != package[2]) // Если хранящийся адрес отправителя не равен адресу, указанному в пакете, то это значит,
        //            {// что сообщение получено уже от другой станции и нужно добавить символ enter
        //                isFinishReceive = true;
        //                receivedMessage += "\r\n";
        //            }
        //            senderAddress = package[2]; // записываем новый адрес отправителя
        //            receivedMessage += Encoding.ASCII.GetString(new byte[] { package[5] });// пишем сообщение в буфер
        //            package[3] = 1; // устанавливаем флаг статус, что адрес совпадает и сообщение прочитано
        //        }

        //    }

        //    return package;
        //}

        //public byte[] SendMessage(byte[] package, byte bt)
        //{
        //    if (bt == 0)
        //    {
        //        package[0] = 0;
        //        package[1] = 0;
        //        package[2] = 0;
        //        package[3] = 0;
        //        package[4] = 0;
        //        package[5] = 0;
        //    }
        //    else
        //    {
        //        package[0] = 1;
        //        package[1] = destinationAddress;
        //        package[2] = sourceAddress;
        //        package[3] = 0;
        //        package[4] = 0;
        //        package[5] = bt;
        //    }
        //    Console.WriteLine("SendMessage - package: ");
        //    foreach (byte b in package)
        //    {
        //        Console.Write(b + " ");
        //    }
        //    return package;
        //}
        private Thread thread1;
        private Thread thread2;
        private Thread thread3;

        private volatile byte[] package;
        private byte activeStation;

        private Station station1;

        private Station station2;

        private Station station3;
        private List<Wait> queue = new List<Wait>();

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
                            Station1Write();
                        activeStation = 1;
                    }));

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
                            Station2Write();
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
                            Station3Write();
                        activeStation = 3;
                    }));
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
            this.Invoke((MethodInvoker)(delegate
            {
                if (station1.IsFrameReturn)
                {
                    if (position < queue[0].Message.Length)
                    {
                        station1.SourceAddress = queue[0].SourceAddress;
                        station1.DestinationAddress = queue[0].DestinationAddress;
                        package = station1.SendMessage(package, queue[0].Message[position]);
                        position++;
                        station1.IsFrameReturn = false;
                        
                    }
                    else
                    {
                        position = 0;
                    }
                }
            }));


        }

        private void Station2Write()
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (station2.IsFrameReturn)
                {
                    if (position < queue[0].Message.Length)
                    {
                        station2.SourceAddress = queue[0].SourceAddress;
                        station2.DestinationAddress = queue[0].DestinationAddress;
                        package = station2.SendMessage(package, queue[0].Message[position]);
                        position++;
                        station2.IsFrameReturn = false;
                    }
                    else
                    {
                        position = 0;
                    }

                }
            }));

        }

        private void Station3Write()
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (station3.IsFrameReturn)
                {
                    if (position < queue[0].Message.Length)
                    {
                        station3.SourceAddress = queue[0].SourceAddress;
                        station3.DestinationAddress = queue[0].DestinationAddress;
                        package = station3.SendMessage(package, queue[0].Message[position]);
                        position++;
                        station3.IsFrameReturn = false;
                    }
                    else
                    {
                        position = 0;
                    }
                }
            }));
        }
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
            }));

        }

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
            }));
        }

        public void Station3Write(object sender, MouseEventArgs e)
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
            package = new byte[6];
            for (int i = 0; i < 6; i++)
                package[i] = 0;
            activeStation = 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread1.Abort();
            thread2.Abort();
            thread3.Abort();
        }
    }
}
