using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANBUSSniffer
{
    public class CanMessage
    {
        public byte Id { get; set; }
        public byte[] Data { get; set; } = new byte[8];

        public override string ToString()
        {
            return $"ID: {Id:X}: {string.Join(",", Data.Select(x=> x.ToString("X")))}";
        }
    }

}
