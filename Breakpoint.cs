﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace vsic
{
    class Breakpoint : ByteMarker
    {
        private static readonly Color BREAKPOINT_COLOR = Color.FromArgb(192, Color.Red);

        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                UpdateMarker();
            }
        }
        public bool BreakOnRead
        { get; set; }
        public bool BreakOnWrite
        { get; set; }

        public Breakpoint(int address) : base(address, BREAKPOINT_COLOR, null)
        {
            Enabled = true;
            ForceDrawAsDisabled = false;
            Pen = new Pen(BREAKPOINT_COLOR, 0.2f);
        }

        public Breakpoint(int address, bool enabled) : base (address, BREAKPOINT_COLOR, null)
        {
            Enabled = enabled;
            ForceDrawAsDisabled = false;
            UpdateMarker();
            Pen = new Pen(BREAKPOINT_COLOR, 0.2f);
        }

        public class Comparer : IComparer<Breakpoint>
        {
            public int Compare(Breakpoint x, Breakpoint y)
            {
                return y.Address - x.Address;
            }
        }

        bool forceDrawAsDisabled = false;
        /// <summary>
        /// Marks the Breakpoint as disabled for graphical purposes without actually changing whether it is enabled.
        /// </summary>
        public bool ForceDrawAsDisabled
        {
            get { return forceDrawAsDisabled; }
            set
            {
                forceDrawAsDisabled = value;
                UpdateMarker();
            }
        }

        private void UpdateMarker()
        {
            if (forceDrawAsDisabled || !Enabled)
            {
                Hollow = true;
            }
            else
            {
                Hollow = false;
            }
        }
    }
}
