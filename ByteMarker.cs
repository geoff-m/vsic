using System.Drawing;

namespace Visual_SICXE
{
    /// <summary>
    /// Represents a visual marker to be drawn on a particular byte.
    /// </summary>
    public class ByteMarker
    {
        public float PenWidth
        { get; set; }

        public readonly int Address;

        public Color Color
        { get; protected set; }
        public Pen Pen
        { get; protected set; }
        public Brush Brush
        { get; protected set; }
        public bool Hollow
        { get; protected set; }

        public readonly long? ExpiresAfter;

        /// <summary>
        /// Describes a colored marker to be drawn around a byte at a certain address.
        /// </summary>
        /// <param name="address">The address to be marked.</param>
        /// <param name="color">The color that will be used for drawing.</param>
        /// <param name="uniqueId">User markers (from breakpoints) keyed on their addresses. However, special markers must be considered unique beyond address. For example, program counter needs to be considered unique at all times.</param>
        public ByteMarker(int address, Color color, long? expiration, bool hollow = false, int uniqueId = 0, float penWidth = 0.2f)
        {
            Address = address;
            ExpiresAfter = expiration;
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

        private readonly int id;
        public override int GetHashCode()
        {
            if (id != 0)
                return id;
            return Address ^ ExpiresAfter.GetHashCode() << 24;
        }

        public override bool Equals(object obj)
        {
            if (obj is ByteMarker other)
            {
                if (Hollow && other.Hollow)
                {
                    return Address == other.Address && ExpiresAfter == other.ExpiresAfter && PenWidth == other.PenWidth && Color == other.Color && Brush == other.Brush;
                }
                if (!Hollow && !other.Hollow)
                {
                    return Address == other.Address && ExpiresAfter == other.ExpiresAfter && PenWidth == other.PenWidth && Color == other.Color && Pen == other.Pen;
                }   
            }
            return false;
        }

        public override string ToString()
        {
            return $"{Address} ({Color}) (ExpiresAfter={ExpiresAfter.ToString()})";
        }
    }
}
