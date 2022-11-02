namespace SICXE
{
    /// <summary>
    /// An implementation of ILogSink that does nothing.
    /// </summary>
    public class NullLog : ILogSink
    {
        public void Log(string str, params object[] args)
        {
            // Do nothing.
        }

        public void LogError(string str, params object[] args)
        {
            // Do nothing.
        }

        public void SetStatusMessage(string str, params object[] args)
        {
            // Do nothing.
        }
    }
}
