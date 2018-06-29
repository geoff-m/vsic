namespace SICXE.Exceptions
{
    /// <summary>
    /// The exception thrown by a machine when it hits a breakpoint.
    /// </summary>
    public class BreakpointHitException : SICXEException
    {
        /// <summary>
        /// The address that triggered the breakpoint.
        /// </summary>
        public Word Address
        { get; }

        /// <summary>
        /// A Boolean value indicating whether the event causing the breakpoint was an attempted write. If false, it was an attempted read.
        /// </summary>
        public bool Write
        { get; }

        /// <summary>
        /// The exception thrown by a machine when it hits a breakpoint.
        /// </summary>
        /// <param name="address">The address that triggered the breakpoint.</param>
        /// <param name="write">Indicates whether a write was attempted. If false, a read was attempted.</param>
        public BreakpointHitException(Word address, bool write)
        {
            Address = address;
            Write = write;
        }
    }
}
