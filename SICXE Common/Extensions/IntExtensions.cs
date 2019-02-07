using System;

namespace SICXE_Common.Extensions
{
    public static class IntExtensions
    {
        public static int Decode12BitTwosComplement(this int n, out bool isPositive)
        {
            return DecodeTwosComplement(n, 12, out isPositive);
        }

        public static int Decode20BitTwosComplement(this int n, out bool isPositive)
        {
            return DecodeTwosComplement(n, 20, out isPositive);
        }

        public static int DecodeTwosComplement(this int n, int bitCount, out bool isPositive)
        {
            if ((n & (1 << (bitCount - 1))) != 0)
            {
                // Number is negative.
                isPositive = false;
                n = ~n + 1;
                return n & ((1 << bitCount) - 1);
            }
            isPositive = true;
            return n;
        }
    }
}
