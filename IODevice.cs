using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsic
{
    /// <summary>
    /// Represents a device that can be read from and written to by a SIC machine.
    /// </summary>
    public abstract class IODevice : IDisposable, ISerialize
    {
        public const byte EOF = 0xFF;

        public byte ID
        { get; protected set; }
        public IODevice(byte id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets the name of this type of device.
        /// </summary>
        public abstract string Type
        { get; }

        protected bool nameSet = false;
        protected string name;
        public virtual string Name
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

        public override string ToString()
        {
            return $"{ID.ToString("X2")}: {Name}";
        }

        public void Serialize(Stream stream)
        {
            // todo: write a magic number depending on the type i am, then call my (real class) serialize
            throw new NotImplementedException();
        }

        public void Deserialize(Stream stream)
        {
            // check magic number to determine which type's deserialize method should be used.
            // that is, make this class have a static list of known subclasses, each paired with their serialization magic number.
            throw new NotImplementedException();
        }
    }
}
