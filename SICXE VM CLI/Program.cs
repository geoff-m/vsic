using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SICXE;

namespace SICXE_VM_CLI
{
    class Program
    {
        static Session sess;

        private delegate bool CommandLineHandler(string line);

        static bool quit = false;
        static void Main(string[] args)
        {
            sess = new Session();
            sess.New();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(sess.ControlC);

            Console.WriteLine($"{_PROGRAM_NAME} {_VERSION}");
            Console.WriteLine($"Type \"help\" for list of commands");
            var handlers = new Dictionary<string, CommandLineHandler>()
            {
                {"dump", HandleDump },
                {"run", HandleRun },
                {"load", HandleLoad },
                {"version", HandleVersion },
                {"help", HandleHelp },
                {"exit", HandleExit },
                {"new", HandleNew }
            };

            string line;
            while (!quit)
            {
                waitingForReadLine = true;
                RestorePrompt();
                line = Console.ReadLine();
                waitingForReadLine = false;
                if (line == null)
                {
                    if (sess.IsRunning)
                    {
                        sess.WaitForStop();
                        continue;
                    }
                    else
                        break;
                }

                line = line.Trim();
                if (line.Length == 0)
                    continue;

                int firstSpace = line.IndexOf(' ');
                string firstToken;
                if (firstSpace >= 0)
                {
                    firstToken = line.Substring(0, firstSpace);
                }
                else
                {
                    firstToken = line;
                }
                if (handlers.TryGetValue(firstToken, out CommandLineHandler handler))
                {
                    if (!handler(line))
                        PrintTopLevelHelp();
                }
                else
                {
                    PrintTopLevelHelp();
                }
            }

        }

        static bool waitingForReadLine = false;
        public static void RestorePrompt()
        {
            if (waitingForReadLine)
                Console.Write("> ");
        }

        static bool HandleNew(string line)
        {
            sess.New();

            return true;
        }

        static bool HandleRun(string line)
        {
            if (sess.IsRunning)
            {
                Console.WriteLine("Machine is already running! Use 'stop' or Ctrl+C to stop.");
                return true;
            }

            var tokens = line.Split(' ').ToArray();
            if (tokens.Length == 2)
            {
                if (ulong.TryParse(tokens[1], out ulong steps))
                {
                    var ct = new CancelToken();
                    sess.BeginRun(steps, ct);
                }
                else
                {
                    Console.WriteLine($"Expected an integer between 0 and {ulong.MaxValue}.");
                }
            }
            else if (tokens.Length == 1)
            {
                var ct = new CancelToken();
                sess.BeginRun(ct);
            }

            return true;
        }

        static bool HandleDump(string line)
        {
            var tokens = line.Split(' ').ToArray();
            if (tokens.Length == 1)
            {
                // todo: print dump help
                Console.WriteLine("Bad dump syntax");
                return true;
            }

            string[] REGISTER_STRINGS = new string[] { "r", "reg", "register", "registers" };
            if (REGISTER_STRINGS.Any(s => s.Equals(tokens[1], StringComparison.OrdinalIgnoreCase)))
            {
                sess.PrintRegisters();
                return true;
            }

            // todo: print dump help
            Console.WriteLine("Bad dump syntax");
            return true;
        }

        static bool HandleLoad(string line)
        {
            var tokens = line.Split(' ').ToList();
            if (tokens.Count > 1)
            {
                sess.LoadOBJ(tokens[1]);
                return true;
            }

            return false;
        }

        static bool HandleVersion(string line)
        {
            Console.WriteLine($"Version: {_VERSION}");
            Console.WriteLine($"Build date: {_BUILD_DATE}");
            return true;
        }

        static bool HandleHelp(string line)
        {
            var tokens = line.Split(' ').ToList();
            if (tokens.Count == 1)
            {
                PrintTopLevelHelp();
                return true;
            }
            switch (tokens[1].ToLower())
            {
                case "new":
                    // todo: print detailed help for this command
                    return true;
                case "load":
                    // todo: print detailed help for this command
                    return true;
                case "dump":
                    // todo: print detailed help for this command
                    return true;
                case "device":
                    // todo: print detailed help for this command
                    return true;
                case "save":
                    // todo: print detailed help for this command
                    return true;
                case "bp":
                    // todo: print detailed help for this command
                    return true;
                case "run":
                    // todo: print detailed help for this command
                    return true;
                case "help":
                    // todo: print detailed help for this command
                    return true;
                case "version":
                    // todo: print detailed help for this command
                    return true;
                case "exit":
                    // todo: print detailed help for this command
                    return true;
            }
            return false;
        }

        static bool HandleExit(string line)
        {
            quit = true;
            return true;
        }

        static void PrintTopLevelHelp()
        {
            Console.WriteLine("Command\t\tUse");
            Console.WriteLine("------------------------------");
            Console.WriteLine("new\t\tReset the session, clearing the machine's state.");
            Console.WriteLine("load\t\tLoad an OBJ file containing assembled code.");
            Console.WriteLine("dump\t\tDisplay machine memory.");
            Console.WriteLine("device\t\tAdd, remove, or modify a virtual I/O device.");
            Console.WriteLine("save");
            Console.WriteLine("bp\t\tSet or remove breakpoints.");
            Console.WriteLine("run\t\tAdvance the machine state.");
            Console.WriteLine("stop\t\tStop running.");
            Console.WriteLine("help\t\tDisplay this information. Also, any command can be added after it to show additional information.");
            Console.WriteLine("version\t\tShow the version of this program.");
            Console.WriteLine($"exit\t\tExit {_PROGRAM_NAME}.\n");
        }

        static readonly string _PROGRAM_NAME = "vsic cli";
        static readonly Version _VERSION;
        static readonly DateTime _BUILD_DATE;
        static Program()
        {
            _VERSION = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            _BUILD_DATE = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * _VERSION.Build + 2 * TimeSpan.TicksPerSecond * _VERSION.Revision));
        }
    }
}
