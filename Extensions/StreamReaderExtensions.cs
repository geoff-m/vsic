using System.IO;

namespace Visual_SICXE.Extensions
{
    internal static class StreamReaderExtensions
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
