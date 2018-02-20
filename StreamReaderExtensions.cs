using System;
using System.IO;

namespace vsic
{
    static class StreamReaderExtensions
    {
        public static bool TryReadLine(this StreamReader reader, out string line)
        {
            if (reader.EndOfStream)
            {
                line = null;
                return false;
            }
            line = reader.ReadLine();
            return true;
        }
    }
}
