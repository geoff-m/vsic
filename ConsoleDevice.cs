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
            inStr = new StringBuilder();
            outStr = new StringBuilder();
        }

        public delegate void ByteWrittenEventHandler(ConsoleDevice sender, byte b);
        public event ByteWrittenEventHandler OutputByteWritten;

        public string OutData
        {
            get
            {
                return outStr.ToString();
            }
        }

        StringBuilder inStr, outStr;
        public override void Flush()
        {
            // Nothing to do.
        }

        /// <summary>
        /// Reads a byte from the input string.
        /// </summary>
        /// <returns></returns>
        public override byte ReadByte()
        {
            if (inStr.Length == 0)
            {
                return EOF;
            }
            byte ret = (byte)inStr[0];
            inStr.Remove(0, 1);
            return ret;
        }

        /// <summary>
        /// Writes a byte to the output string.
        /// </summary>
        /// <param name="b"></param>
        public override void WriteByte(byte b)
        {
            outStr.Append((char)b);
            OutputByteWritten?.Invoke(this,b);
        }

        public void WriteInputByte(byte b)
        {
            inStr.Append((char)b);
        }

        public override bool Test()
        {
            return true;
        }

        public override string ToString()
        {
            return $"{ID.ToString("X2")}: Console";
        }

        public override void Dispose()
        {
            inStr.Clear();
            outStr.Clear();
        }

        public override string Type
        {
            get { return "Console"; }
        }
    }
}
