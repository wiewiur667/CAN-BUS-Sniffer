using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANBUSSniffer
{
    static class SerialPortExtensions
    {
        public static async Task<byte[]> ReadAsync(this SerialPort serialPort)
        {
            var buffer = new byte[serialPort.BytesToRead];
            await serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
            return buffer;
        }

        public static async Task<string> ReadLineAsync(this SerialPort serialPort)
        {
            var result = new StringBuilder();
            var buffer = await ReadAsync(serialPort);
            foreach (var item in buffer)
            {
                var c = (char)item;
                result.Append(c);
                if (result.ToString().IndexOf("\r\n") != -1) break;
            }
            return result.ToString();

        }
    }
}
