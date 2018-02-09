using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sicsim
{
    /// <summary>
    /// An implementation of ILogSink that does nothing.
    /// </summary>
    class NullLog : ILogSink
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
