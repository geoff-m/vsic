using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sicsim
{
    public struct Word
    {
        public static Word FromArray(byte[] array, int start)
        {
            return new Word(array[start],
                array[start + 1],
                array[start + 2]);
        }

        public static explicit operator Word(int n)
        {
            return new Word((byte)n,
                            (byte)((n & 0xff00) >> 8),
                            (byte)((n & 0xff0000) >> 16));
        }

        public static explicit operator int(Word w)
        {
            return w.Low | w.Middle << 8 | w.High << 16;
        }

        public static Word operator +(Word x, Word y)
        {
            return (Word)((int)x + (int)y);
        }

        public Word(byte low, byte middle, byte high)
        {
            Low = low;
            Middle = middle;
            High = high;
        }
        public byte Low, Middle, High;

        public override string ToString()
        {
            return ((int)this).ToString();
        }
    }
}
