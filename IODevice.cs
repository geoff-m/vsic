using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsic
{
    /// <summary>
    /// Represents a device that can be read from and written to by a SIC machine.
    /// </summary>
    public abstract class IODevice : IDisposable
    {
        public const byte EOF = 0xFF;

        public byte ID
        { get; private set; }
        public IODevice(byte id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets the name of this type of device.
        /// </summary>
        public abstract string Type
        { get; }

        bool nameSet = false;
        string name;
        public string Name
        {
            get
            {
                if (!nameSet) // Provide a default name if none has ever been set.
                {
                    name = $"{Type}{ID.ToString("X")}";
                }
                return name;
            }
            set
            {
                nameSet = true;
                name = value;
            }
        }

        /// <summary>
        /// Checks whether or not the device is ready to read or write.
        /// </summary>
        /// <returns>A Boolean value indicating whether the device is ready.</returns>
        public abstract bool Test();

        /// <summary>
        /// Writes a single byte to the device.
        /// </summary>
        /// <param name="b">The byte to be written to the device.</param>
        public abstract void WriteByte(byte b);

        /// <summary>
        /// Reads a single byte from the device.
        /// </summary>
        /// <returns>The byte that was read from the device.</returns>
        public abstract byte ReadByte();


        public abstract void Flush();

        public abstract void Dispose();

    }
}
