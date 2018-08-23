using System;
using System.Collections.Generic;
using System.Linq;

namespace SICXE_VM_CLI
{
    public static class ConsoleHelper
    {
        static Stack<CursorPosition> _cursorPositions = new Stack<CursorPosition>();
        public static void PushCursorPosition()
        {
            _cursorPositions.Push(new CursorPosition(Console.CursorTop,
                Console.CursorLeft));
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
    }
}
