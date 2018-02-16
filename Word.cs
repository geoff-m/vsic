using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsic
{
    public struct Word
    {
        /// <summary>
        /// Extracts a three-byte Word from the given location in the given array.
        /// </summary>
        public static Word FromArray(byte[] array, int startIndex)
        {
            return new Word(array[startIndex + 2],
                array[startIndex + 1],
                array[startIndex]);
        }

        /// <summary>
        /// Converts a byte array to a Word array.
        /// </summary>
        /// <param name="array">The array whose bytes will be converted to Words. If the length is not a multiple of three, the result will be padded with high bits.</param>
        /// <param name="offset">The index in the array to start from.</param>
        /// <param name="length">The number of elements in the array to consider.</param>
        public static Word[] FromArray(byte[] array, int offset, int length)
        {
            int fullWords = length / 3;
            int bytesInFullWords = 3 * fullWords;
            int extraBytes = length % 3;
            int rlen = extraBytes == 0 ? fullWords : fullWords + 1;
            var ret = new Word[rlen];
            int wi = 0;
            int i;
            for (i = 0; i < bytesInFullWords; i += 3)
            {
                //ret[wi++] = new Word(array[i], array[i + 1], array[i + 2]); // big-endian
                ret[wi++] = new Word(array[i + 2], array[i + 1], array[i]); // little-endian
            }
            switch (extraBytes)
            {
                case 1:
                    //ret[rlen - 1] = new Word(array[i], 0xff, 0xff);
                    ret[rlen - 1] = new Word(0xff, 0xff, array[i]);
                    break;
                case 2:
                    //ret[length - 1] = new Word(array[i], array[i + 1], 0xff);
                    ret[length - 1] = new Word(0xff, array[i + 1], array[i]);
                    break;
            }
            return ret;
        }

        public static explicit operator Word(int n)
        {
            return new Word((byte)n,
                            (byte)((n & 0xff00) >> 8),
                            (byte)((n & 0xff0000) >> 16));
        }

        public static implicit operator int(Word w)
        {
            return w.Low | w.Middle << 8 | w.High << 16;
        }

        public static Word operator +(Word x, Word y)
        {
            return (Word)((int)x + (int)y);
        }

        public static Word operator -(Word x, Word y)
        {
            return (Word)((int)x - (int)y);
        }

        public static Word operator ++(Word w)
        {
            return (Word)((int)w + 1);
        }

        public byte Low, Middle, High;
        public Word(byte low, byte middle, byte high)
        {
            Low = low;
            Middle = middle;
            High = high;
        }

        public override string ToString()
        {
            return "0x" + ((int)this).ToString("X");
        }

        public string ToString(string format)
        {
            return ((int)this).ToString(format);
        }

        public static readonly Word Zero = (Word)0;
    }
}
