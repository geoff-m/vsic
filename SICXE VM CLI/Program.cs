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
                {"d", HandleDump },
                { "dump", HandleDump },
                {"run", HandleRun },
                {"step", HandleStep },
                {"stop", HandleStop },
                {"load", HandleLoad },
                {"version", HandleVersion },
                {"help", HandleHelp },
                {"exit", HandleExit },
                {"new", HandleNew },
                {"ls", HandleLS },
                {"dir", HandleLS },
                {"cd", HandleCD }
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
                        HandleHelp($"help {firstToken}");
                }
                else
                {
                    PrintTopLevelHelp();
                }
            }
            sess.ControlC(null, null);
            sess.WaitForStop();
        }

        static bool waitingForReadLine = false;
        public static void RestorePrompt()
        {
            if (waitingForReadLine)
                Console.Write("vsic> ");
        }

        static bool HandleLS(string line)
        {
            int firstSpaceIdx = line.IndexOf(' ');
            if (firstSpaceIdx >= 0)
                line = line.Substring(firstSpaceIdx + 1);
            else
                line = "";
            ConsoleHelper.ExecuteDirectoryListing(line);
            return true;
        }

        static bool HandleCD(string line)
        {
            int firstSpaceIdx = line.IndexOf(' ');
            if (firstSpaceIdx >= 0)
                line = line.Substring(firstSpaceIdx + 1);
            else
                line = "";
            ConsoleHelper.ChangeDirectory(line);
            return true;
        }

        static bool HandleNew(string line)
        {
            sess.New();

            return true;
        }

        static bool HandleStop(string line)
        {
            if (sess.IsRunning)
            {
                sess.ControlC(null, null);
                sess.WaitForStop();
                Console.WriteLine("Stopped.");
            }
            else
            {
                Console.WriteLine("Machine is not running.");
            }
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

        static bool HandleStep(string line)
        {
            if (sess.IsRunning)
            {
                Console.WriteLine("Machine is already running! Use 'stop' or Ctrl+C to stop.");
                return true;
            }

            sess.Step();
        
            return true;
        }

        static bool HandleDump(string line)
        {
            var tokens = line.Split(' ').ToArray();
            if (tokens.Length == 1)
            {
                return false;
            }

            if (tokens.Length == 3)
            {
                if (int.TryParse(tokens[1], System.Globalization.NumberStyles.HexNumber, null, out int startAddress))
                {
                    if (int.TryParse(tokens[2], System.Globalization.NumberStyles.Integer, null, out int length))
                    {
                        int stop = Math.Min(startAddress + length, sess.MemorySize);
                        sess.PrintMemory(startAddress, stop);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Error: Could not parse as length \"{tokens[2]}\"");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: Could not parse as address \"{tokens[1]}\"");
                    return false;
                }
            }

            string[] REGISTER_STRINGS = new string[] { "r", "reg", "register", "registers" };
            if (REGISTER_STRINGS.Any(s => s.Equals(tokens[1], StringComparison.OrdinalIgnoreCase)))
            {
                sess.PrintRegisters();
                return true;
            }

            return false;
        }

        static bool HandleLoad(string line)
        {
            int firstSpaceIdx = line.IndexOf(' ');
            if (firstSpaceIdx >= 0)
                line = line.Substring(firstSpaceIdx + 1);
            else
                return false;
            line = line.Trim('"');
            sess.LoadOBJ(line);
            return true;
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
                    Console.WriteLine("Loads an OBJ into the machine. Expects 1 argument: path to OBJ file");
                    return true;
                case "dump":
                    Console.WriteLine("Displays memory contents. Expects one or two arguments. Alias 'd'.");
                    Console.WriteLine("\tdump [start address] [length]");
                    Console.WriteLine("This will display the first [length] bytes that occur beginning at [start address].");
                    Console.WriteLine("Start address must be a valid address.");
                    Console.WriteLine("If the range goes past the end of memory, output will be silently truncated.");
                    Console.WriteLine("\n\tdump registers|reg|r");
                    Console.WriteLine("This will display the contents of all the SIC/XE registers.");
                    Console.WriteLine("\n\tdump [register name]");
                    Console.WriteLine("This will display the content of the specified register.");
                    Console.WriteLine("[Register name] may be any of A, B, S, T, X, L, PC.");
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
                    PrintTopLevelHelp();
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
            Console.WriteLine("ls\t\tList the contents of the current directory.");
            Console.WriteLine("cd\t\tChange the current directory.");
            Console.WriteLine("help\t\tDisplay this information. Also, any command can be added after it to show additional information.");
            Console.WriteLine("version\t\tShow the version of this program.");
            Console.WriteLine($"exit\t\tExit {_PROGRAM_NAME}.\n");
        }

        static readonly string _PROGRAM_NAME = "VSIC CLI";
        static readonly Version _VERSION;
        static readonly DateTime _BUILD_DATE;
        static Program()
        {
            _VERSION = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            _BUILD_DATE = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * _VERSION.Build + 2 * TimeSpan.TicksPerSecond * _VERSION.Revision));
        }
    }
}
