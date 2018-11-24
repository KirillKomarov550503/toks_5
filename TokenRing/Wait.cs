using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenRing
{
    class Wait
    {
        private byte stationAddress;
        private string message;
        public Wait(byte stationAddress, string message)
        {
            this.stationAddress = stationAddress;
            this.message = message;
        }

        public byte StationAddress { get => stationAddress; set => stationAddress = value; }
        public string Message { get => message; set => message = value; }
    }
}
