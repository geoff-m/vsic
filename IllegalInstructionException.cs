using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsic
{
    /// <summary>
    /// The exception thrown by a machine when it attempts to execute an illegal instruction.
    /// </summary>
    class IllegalInstructionException : Exception
    {
        /// <summary>
        /// The address where the instruction began.
        /// </summary>
        public Word Address
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">The address of the first byte of the instruction.</param>
        public IllegalInstructionException(Word address)
        {
            Address = address;
        }
    }
}
