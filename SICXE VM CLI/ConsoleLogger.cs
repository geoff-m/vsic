using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SICXE;

namespace SICXE_VM_CLI
{
    class ConsoleLogger : ILogSink
    {
        public void Log(string str, params object[] args)
        {
            Console.WriteLine(str, args);
        }

        public void LogError(string str, params object[] args)
        {
            Console.Error.WriteLine(str, args);
        }

        public void SetStatusMessage(string str, params object[] args)
        {
            Console.Error.WriteLine(str, args);
        }
    }
}
