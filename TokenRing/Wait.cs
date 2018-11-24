using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenRing
{
    class Wait
    {
        private byte sourceAddress;
        private byte[] message;
        private byte destinationAddress;
        public Wait(byte stationAddress, byte destinationAddress, byte[] message)
        {
            this.sourceAddress = stationAddress;
            this.destinationAddress = destinationAddress;
            this.message = message;
        }

        public byte SourceAddress { get => sourceAddress; set => sourceAddress = value; }
        public byte[] Message { get => message; set => message = value; }
        public byte DestinationAddress { get => destinationAddress; set => destinationAddress = value; }
    }
}
