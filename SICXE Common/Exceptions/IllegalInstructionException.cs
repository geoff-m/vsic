using System;

namespace SICXE.Exceptions
{
    /// <summary>
    /// The exception thrown by a machine when it attempts to execute an illegal instruction.
    /// </summary>
    internal class IllegalInstructionException : SICXEException
    {
        private static string GetMessage(Word w)
        {
            return $"An illegal instruction was encountered at address {w}.";
        }

        /// <summary>
        /// The address where the instruction began.
        /// </summary>
        public Word Address
        { get; private set; }

        public override string Message => GetMessage(Address);

        /// <param name="address">The address of the first byte of the instruction.</param>
        public IllegalInstructionException(Word address)
        {
            Address = address;
        }

        public IllegalInstructionException(Word address, Exception innerException) : base(GetMessage(address), innerException)
        {
            Address = address;
        }
    }
}
