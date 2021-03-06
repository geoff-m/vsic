﻿using System;

namespace SICXE.Exceptions
{
    /// <summary>
    /// Represents an exception that can be thrown by a SICXE machine.
    /// </summary>
    public class SICXEException : Exception
        // We use this class to certify that an exception in Machine is due to errors in client code (or data), not internal errors.
    {
        public SICXEException() : base() { }
        public SICXEException(string message) : base(message) { }
        public SICXEException(string message, Exception innerException) : base(message, innerException) { }
    }
}
