using System.Text;
namespace TokenRing
{
    class Station
    {
        private bool isMonitor;
        private byte sourceAddress;
        private byte destinationAddress;
        private string sendMessage;
        private string receivedMessage;
        private byte senderAddress;
        private bool isTerminate;
        private bool isFrameReturn;
        private bool isFinishReceive;
        
        public Station(byte sourceAddress, bool isMonitor)
        {
            this.isMonitor = isMonitor;
            this.sourceAddress = sourceAddress;
            this.destinationAddress = 0;
            this.sendMessage = "";
            this.receivedMessage = "";
            this.senderAddress = 0;
            this.isTerminate = false;
            this.isFrameReturn = false;
        }

        public bool IsMonitor { get => isMonitor; set => isMonitor = value; }
        public byte SourceAddress { get => sourceAddress; set => sourceAddress = value; }
        public byte DestinationAddress { get => destinationAddress; set => destinationAddress = value; }
        public string SendMessage1 { get => sendMessage; set => sendMessage = value; }
        public string ReceivedMessage1 { get => receivedMessage; set => receivedMessage = value; }
        public byte SenderAddress { get => senderAddress; set => senderAddress = value; }
        public bool IsTerminate { get => isTerminate; set => isTerminate = value; }
        public bool IsFrameReturn { get => isFrameReturn; set => isFrameReturn = value; }
        public bool IsFinishReceive { get => isFinishReceive; set => isFinishReceive = value; }

        public byte[] ReceivedMessage(byte[] package)
        {
            if (isMonitor) // Если станция является монитором
            {
                if (package[0] == 0) // Если полученый пакет - это токен
                {
                    if (package[4] == 0)// Если байт "монитор" равен нулю, то меняем значение на 1
                    {
                        package[4] = 1;
                    }
                    else // Иначе пакет прошел всё кольцо, не найдя адрес станции получения, то чтобы избежать зацикливания, мы удаляем данный token
                    {
                        isTerminate = true;
                        return null;
                    }
                }
            }
            if (package[0] == 1) // Если получен кадр
            {
                if (package[3] == 1) // если байт статус равен 1, то станция приемник считала байт данных
                {
                    if (package[2] == sourceAddress) //  если адрес источника в пакете равен адресу станции
                    {
                        isFrameReturn = true;

                    }

                }
                if (package[1] == sourceAddress) //Если пакет предназначен для данной станции
                {
                    
                    if (senderAddress != package[2]) // Если хранящийся адрес отправителя не равен адресу, указанному в пакете, то это значит,
                    {// что сообщение получено уже от другой станции и нужно добавить символ enter
                        isFinishReceive = true;
                        receivedMessage += "\r\n";
                    }
                    senderAddress = package[2]; // записываем новый адрес отправителя
                    receivedMessage += Encoding.ASCII.GetString(new byte[] { package[5] });// пишем сообщение в буфер
                    package[3] = 1; // устанавливаем флаг статус, что адрес совпадает и сообщение прочитано
                }

            }

            return package;
        }

        public byte[] SendMessage(byte[] package, byte bt)
        {
            if (bt == 0)
            {
                package[0] = 0;
                package[1] = 0;
                package[2] = 0;
                package[3] = 0;
                package[5] = 0;
            }
            else
            {
                package[0] = 1;
                package[1] = destinationAddress;
                package[2] = sourceAddress;
                package[3] = 0;
                package[5] = bt;
            }
            
            return package;
        }
    }
}
