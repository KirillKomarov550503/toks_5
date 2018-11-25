using System;
using System.Text;
using System.Threading;

using System.Windows.Forms;

namespace TokenRing
{
    public partial class Form1 : Form
    {
        private byte destinationAddress;
        private Wait waitStation1;
        private Wait waitStation2;
        private Wait waitStation3;
        private bool isFix = false;
        public void ReceivedMessage(bool isMonitor, byte sourceAddress, TextBox outBox) //Метод анализирует полученный из кольца пакет данных
        {
            Thread.Sleep(1000); // Делаем задержку в 1 секунду
            if(package[0] == 0 && position > 0)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    position = 0;
                    isFix = true;
                }));
            }
            if (isMonitor) // Проверяем, является ли станция станцией-монитором
            {
                if (package[0] == 1) // Если получен frame
                {
                    if (package[1] == sourceAddress)// Проверяем, что адрес приемника в пакете равен адресу станции
                    {
                        this.Invoke((MethodInvoker)(delegate
                        {
                            package[3] = 1; // Устанавливаем байт Status в 1, то есть станция считала байт данных
                            outBox.Text += Encoding.ASCII.GetString(new byte[] { package[5] }); // считываем байт данных
                        }));
                        return; // завершаем работу метода
                    }
                    else
                    {
                        if (package[4] == 0) // Если байт Monitor равен нулю
                        {
                            this.Invoke((MethodInvoker)(delegate
                            {
                                package[4] = 1; // Устанавливаем 1 в байте Monitor
                            }));
                            return;
                        }
                        else // Иначе, значение байта Monitor равно 1,
                        {//а это означает, что прозишло зацикливание кадра и нужна починка кольца
                            this.Invoke((MethodInvoker)(delegate
                            {
                                for (int i = 0; i < 6; i++) // чиним кольцо, устанавливая все значения пакета в 0
                                    package[i] = 0;
                                textBox5.Text += "Fix ring\r\n";
                            }));
                            Thread.Sleep(1000); // делаем дополнительую задержку в 1 секунду, чтобы успеть отобразить в окне вывода сообщение и починке кольца
                            return;
                        }
                    }
                }
                else return;
            }
            else // Если станция не является станцией-монитором
            {
                if (package[0] == 1) // если получен frame
                {
                    if (package[1] == sourceAddress)// Проверяем, что адрес приемника в пакете равен адресу станции
                    {
                        this.Invoke((MethodInvoker)(delegate
                        {
                            package[3] = 1;// Устанавливаем байт Status в 1, то есть станция считала байт данных
                            outBox.Text += Encoding.ASCII.GetString(new byte[] { package[5] });// считываем байт данных
                        }));
                    }
                    else return;

                }
                else return;
            }
            return;
        }

        public void SendMessage(byte bt, byte destinationAddress, byte sourceAddress) // запись байта в кольцо
        {
            if (bt == 0) // Если байт равен нулю, то сообщение со станции было полностью отправлено
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    string separator = "\r\n";
                    switch (this.destinationAddress) // в зависимости от адреса станции приемника, в соответствующее окно вывода добавляем символ Enter
                    {
                        case 1: textBox3.Text += separator; break;
                        case 10: textBox10.Text += separator; break;
                        case 100: textBox4.Text += separator; break;
                        default: break;
                    }
                    for (int i = 0; i < 6; i++) // освобождаем frame, переведя его в token
                        package[i] = 0;
                }));
            }
            else// если байт не равен нулю, то переводим token в frame и устанавливаем соответствующие байты
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    package[0] = 1;
                    package[1] = destinationAddress;
                    package[2] = sourceAddress;
                    package[3] = 0;
                    package[4] = 0;
                    package[5] = bt;
                    this.destinationAddress = destinationAddress; //сохраняем адрес станции приемника
                }));

            }
        }

        private Thread thread1; //переменная для создания потока работы станции
        private Thread thread2;
        private Thread thread3;

        private volatile byte[] package; // пересылаем пакет байтов между станциями
        private byte activeStation; // номер станции, на которую пришел пакет
        private bool isNull;

        public void StationWork1() // поток работы станции с адресом равным 1
        {
            while (true)
            {
                if (package != null && activeStation == 3) // Проверка, что на станцию пришел пакет байтов
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        activeStation = 0;
                        textBox2.Text = "*\r\n"; // выводим в окно Debug символ *

                    }));
                    ReceivedMessage(false, 1, textBox3); // Анализируем полученный пакет
                    if(isFix)
                    {
                        isFix = false;
                        waitStation1 = null;
                    }
                    foreach(byte bt in package)
                    {
                        Console.Write(bt + " ");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Position: " + position);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox2.Text = "";
                        if ((package[0] == 1 && package[2] == 1 && package[3] == 1 && waitStation1 != null) || (package[0] == 0 && waitStation1 != null)) // проверяем, что в очереди на отправку сообщения первым стоит данная станция
                        {
                            if (position == 0)
                            {
                                StationWrite(1, waitStation1); // пишем в пакет данные для отправки
                            }
                            else
                            {
                                StationWrite(package[3], waitStation1);// пишем в пакет данные для отправки
                                if (isNull)
                                {
                                    waitStation1 = null;
                                    isNull = false;
                                }
                            }
                            
                        }
                    }));
                    activeStation = 1; // отправка пакета следующей станции
                }
            }
        }

        public void StationWork2()// поток работы станции с адресом равным 10
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
                    if (isFix)
                    {
                        isFix = false;
                        waitStation2 = null;
                    }
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox11.Text = "";
                        if ((package[0] == 1 && package[2] == 10 && package[3] == 1 && waitStation2 != null) || (package[0] == 0 && waitStation2 != null))
                        {
                            if (position == 0)
                            {
                                StationWrite(1, waitStation2);
                            }
                            else
                            {
                                StationWrite(package[3], waitStation2);
                                if(isNull)
                                {
                                    waitStation2 = null;
                                    isNull = false;
                                }
                            }
                        }
                    }));
                    activeStation = 2;

                }

            }
        }

        public void StationWork3()// поток работы станции с адресом равным 100
        {
            while (true)
            {
                if (package != null && activeStation == 2)
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        activeStation = 0;
                        textBox5.Text = "*\r\n";
                    }));
                    ReceivedMessage(true, 100, textBox4);
                    if (isFix)
                    {
                        isFix = false;
                        waitStation3 = null;
                    }
                    this.Invoke((MethodInvoker)(delegate
                    {
                        textBox5.Text = "";
                        if ((package[0] == 1 && package[2] == 100 && package[3] == 1 && waitStation3 != null) || (package[0] == 0 && waitStation3 != null))
                        {
                            
                            if (position == 0)
                            {
                                StationWrite(1, waitStation3);
                            }
                            else
                            {
                                StationWrite(package[3], waitStation3);
                                if(isNull)
                                {
                                    waitStation2 = null;
                                    isNull = false;
                                }
                            }
                        }
                    }));
                    activeStation = 3;
                }

            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private int position = 0;//позиция индекса в сообщении для отправки

        private void StationWrite(byte status, Wait waitStation) // начало записи байта в пакет
        {
            if (status == 1) // Проверка,что можно записывать данные
            {
                if (position < waitStation.Message.Length) // Проверяем, что позиция индекса в сообщении меньше длины сообщения
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        SendMessage(waitStation.Message[position], waitStation.DestinationAddress, waitStation.SourceAddress); // записываем байт в пакет
                        position++; //увеличиваем позицию индекса
                    }));

                }
                else // Иначе, сообщение было полностью успешно отправлено и нужно освободить frame, переведя его в token
                {
                    this.Invoke((MethodInvoker)(delegate
                    {
                        SendMessage(0, 0, 0);
                        isNull = true;
                        position = 0; // сбрасываем позицию индекса в 0
                    }));
                }
            }
        }


        public void Station1WriteEvent(object sender, MouseEventArgs e) //событие отправки сообщения станцией с адресом 1
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (textBox1.Text.Length == 0 || textBox7.Text.Length == 0) // проверка, что введен текст сообщения и адрес приемника
                {
                    textBox2.Text += "Empty \"Input\" or \"Destination address\" field";
                    return;
                }

                waitStation1 = new Wait(1, Convert.ToByte(textBox7.Text), //добавляем данную станцию в очередь ожидания
                     Encoding.ASCII.GetBytes(textBox1.Text));
            }));
        }

        public void Station2WriteEvent(object sender, MouseEventArgs e)//событие отправки сообщения станцией с адресом 10
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (textBox8.Text.Length == 0 || textBox12.Text.Length == 0)
                {
                    textBox11.Text += "Empty \"Input\" or \"Destination address\" field";
                    return;
                }
                waitStation2 = new Wait(10, Convert.ToByte(textBox8.Text),
                    Encoding.ASCII.GetBytes(textBox12.Text));
            }));
        }

        public void Station3WriteEvent(object sender, MouseEventArgs e)//событие отправки сообщения станцией с адресом 100
        {
            this.Invoke((MethodInvoker)(delegate
            {
                if (textBox9.Text.Length == 0 || textBox6.Text.Length == 0)
                {
                    textBox5.Text += "Empty \"Input\" or \"Destination address\" field";
                    return;
                }
                waitStation3 = new Wait(100, Convert.ToByte(textBox9.Text),
                    Encoding.ASCII.GetBytes(textBox6.Text));
            }));
        }

        public void CreateToken(object sender, MouseEventArgs e) // событие создания токена
        {
            this.Invoke((MethodInvoker)(delegate
            {
                package = new byte[6]; // создание токена
                for (int i = 0; i < 6; i++)
                    package[i] = 0;
                activeStation = 2;
            }));
            button4.Enabled = false; //блокировка создания нового токена
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
            isNull = false;
            textBox3.ScrollBars = ScrollBars.Both;
            textBox2.ScrollBars = ScrollBars.Both;
            textBox10.ScrollBars = ScrollBars.Both;
            textBox11.ScrollBars = ScrollBars.Both;
            textBox4.ScrollBars = ScrollBars.Both;
            textBox5.ScrollBars = ScrollBars.Both;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread1.Abort();
            thread2.Abort();
            thread3.Abort();
        }
    }
}
