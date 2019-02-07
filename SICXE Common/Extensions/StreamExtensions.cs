using System;
using System.IO;

namespace SICXE_Common.Extensions
{
    public static class StreamExtensions
    {
        static readonly int BUFFER_SIZE = Environment.SystemPageSize;

        /// <summary>
        /// Blocks until the specified number of bytes have been read.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="amount">The number of bytes to read.</param>
        /// <returns>An array of the bytes read, of the specified length.</returns>
        public static byte[] Read(this Stream s, int amount)
        {
            var ret = new byte[s.Length];
            int bufferSize = amount < BUFFER_SIZE ? amount : BUFFER_SIZE;
            int readSoFar = 0;
            int remaining = amount - readSoFar;
            while (remaining > 0)
            {
                int justRead = s.Read(ret, readSoFar, Math.Min(remaining, bufferSize));
                readSoFar += justRead;
                remaining -= justRead;
            }
            return ret;
        }
    }
}
