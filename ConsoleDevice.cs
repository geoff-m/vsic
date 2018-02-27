using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace vsic
{
    public class ConsoleDevice : IODevice
    {
        public ConsoleDevice(byte id) : base(id)
        {
            sb = new StringBuilder();
        }

        StringBuilder sb;
        public override void Flush()
        {
            // Nothing to do.
        }

        public override byte ReadByte()
        {
            if (sb.Length == 0)
            {
                return EOF;
            }
            byte ret = (byte)sb[0];
            sb.Remove(0, 1);
            return ret;
        }

        public override void WriteByte(byte b)
        {
            sb.Append(b);
        }

        public override bool Test()
        {
            return true;
        }

        public override string ToString()
        {
            return $"{ID.ToString("X").PadLeft(2, '0')}: Console";
        }


        public override void Dispose()
        {
            sb.Clear();
        }

    }
}
