using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace SICXE_VM_CLI
{
    public static class ConsoleHelper
    {
        static readonly Stack<CursorPosition> _cursorPositions = new Stack<CursorPosition>();
        public static void PushCursorPosition()
        {
            _cursorPositions.Push(new CursorPosition(Console.CursorTop, Console.CursorLeft));
        }

        public static void PopCursorPosition()
        {
            var p = _cursorPositions.Pop();
            Console.CursorLeft = p.Left;
            Console.CursorTop = p.Top;
        }

        struct CursorPosition
        {
            public int Top, Left;
            public CursorPosition(int top, int left)
            {
                Top = top;
                Left = left;
            }
        }

        public static void WriteRightAligned(string str)
        {
            int len = str.Length;
            Console.CursorLeft = Console.WindowWidth - len;
            Console.Write(str);
        }


        public static void WriteCenterAligned(string str)
        {
            int len = str.Length;
            Console.CursorLeft = (Console.WindowWidth - len) / 2;
            Console.Write(str);
        }

        private static void PrintStreamReader(System.IO.StreamReader s)
        {
            string line;
            while ((line = s.ReadLine()) != null)
                Console.WriteLine(line);
        }

        public static void ExecuteDirectoryListing(string args)
        {
            var proc = new Process();
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    proc.StartInfo = new ProcessStartInfo("cmd.exe", "/C dir " + args);
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    proc.StartInfo = new ProcessStartInfo("ls", args);
                    break;
            }

            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            try
            {
                proc.Start();
                PrintStreamReader(proc.StandardOutput);
                proc.WaitForExit();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is Win32Exception || ex is ObjectDisposedException || ex is PlatformNotSupportedException)
            {
                Console.WriteLine($"Error listing directory: {ex.Message}");
            }
            finally
            {
                proc.Dispose();
            }
        }

        public static void ChangeDirectory(string path)
        {
            if (path.Length > 0)
                Environment.CurrentDirectory = path;
        }
    }
}
