﻿using System;
using System.Collections.Generic;

namespace vsic
{
    class Breakpoint
    {
        public int Address
        { get; private set; }
        public bool Enabled
        { get; set; }
        public bool BreakOnRead
        { get; set; }
        public bool BreakOnWrite
        { get; set; }

        public Breakpoint(int address)
        {
            Address = address;
            Enabled = true;
        }

        public Breakpoint(int address, bool enabled)
        {
            Address = address;
            Enabled = enabled;
        }

        public class Comparer : IComparer<Breakpoint>
        {
            public int Compare(Breakpoint x, Breakpoint y)
            {
                return y.Address - x.Address;
            }
        }
    }
}
