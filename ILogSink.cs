using System;
using System.Collections.Generic;
using System.Text;

namespace sicsim
{
    public interface ILogSink
    {
        void Log(string str, params object[] args);
        void LogError(string str, params object[] args);
        void SetStatusMessage(string str, params object[] args);
    }
}
