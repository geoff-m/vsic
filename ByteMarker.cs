using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

namespace vsic
{
    public class ByteMarker
    {
        public float PenWidth
        { get; set; }
        public int Address
        { get; private set; }
        public Color Color
        { get; private set; }
        public Pen Pen
        { get; private set; }
        public Brush Brush
        { get; private set; }
        public bool Hollow
        { get; private set; }
        public int Timestamp
        { get; private set; }

        /// <summary>
        /// Describes a colored marker to be drawn around a byte at a certain address.
        /// </summary>
        /// <param name="address">The address to be marked.</param>
        /// <param name="color">The color that will be used for drawing.</param>
        /// <param name="uniqueId">User markers (from breakpoints) keyed on their addresses. However, special markers must be considered unique beyond address. For example, program counter needs to be considered unique at all times.</param>
        public ByteMarker(int address, int timestamp, Color color, bool hollow = false, int uniqueId = 0, float penWidth = 0.2f)
        {
            Address = address;
            Timestamp = timestamp;
            Color = color;
            id = uniqueId;
            if (hollow)
            {
                Pen = new Pen(color, penWidth);
            }
            else
            {
                Brush = new SolidBrush(color);
            }
            Hollow = hollow;
        }

        int id;
        public override int GetHashCode()
        {
            if (id != 0)
                return id;
            //Debug.WriteLine($"hello from bytemarker at {Address} with timestamp {Timestamp}");
            return Address ^ Timestamp << 24;
        }

        public override string ToString()
        {
            return $"{Address} ({Color}) (time={Timestamp})";
        }
    }
}
