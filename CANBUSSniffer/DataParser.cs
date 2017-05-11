using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANBUSSniffer
{
    class DataParser
    {
        public static CanMessage ParseToCanMessage(string input)
        {
            var array = input
                .Trim()
                .Replace("<", "")
                .Replace(">", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Split(',')
                .Select(a => { if (String.IsNullOrEmpty(a)) return (byte)0; return byte.Parse(a); })
                .ToArray();

            var result = new CanMessage
            {
                Id = array[0],
                Data = array.Skip(2).ToArray()
        };

            return result;
        }

        public static bool VerifyData(string input)
        {
            input = input.Replace("\r", "").Replace("\n","");
            if (!String.IsNullOrEmpty(input))
            {
                if (input.Substring(0, 1) == "<" & input.Substring(input.Length - 1, 1) == ">")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
