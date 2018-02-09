using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sicsim
{
    interface ILogSink
    {
        void Log(string str, params object[] args);
        void LogError(string str, params object[] args);
        void SetStatusMessage(string str, params object[] args);
    }
}
