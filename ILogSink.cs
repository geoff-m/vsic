namespace Visual_SICXE
{
    public interface ILogSink
    {
        void Log(string str, params object[] args);
        void LogError(string str, params object[] args);
        void SetStatusMessage(string str, params object[] args);
    }
}
