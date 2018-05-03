using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Visual_SICXE.Devices
{
    /// <summary>
    /// Represents a device that can be read from and written to by a SIC machine.
    /// </summary>
    public abstract class IODevice : IDisposable, ISerialize
    {
        private const uint SERIALIZATION_FILE_DEVICE_MAGIC_NUMBER = 0xF11EF11E;
        private const uint SERIALIZATION_CONSOLE_DEVICE_MAGIC_NUMBER = 0xC07501ED;
        private static readonly IReadOnlyDictionary<uint, Type> _TYPES = new Dictionary<uint, Type>
        {
            {SERIALIZATION_FILE_DEVICE_MAGIC_NUMBER, typeof(FileDevice) },
            {SERIALIZATION_CONSOLE_DEVICE_MAGIC_NUMBER, typeof(ConsoleDevice) }
        };

        public const byte EOF = 0xFF;
        internal const string SUBCLASS_DESERIALIZE_METHOD_NAME = "Deserialize";

        public byte ID
        { get; }
        protected IODevice(byte id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets the name of this type of device.
        /// </summary>
        public abstract string Type
        { get; }

        /// <summary>
        /// Indicates whether the Name setter has ever been used.
        /// </summary>
        protected bool NameSet = false;
        protected string name;
        public virtual string Name
        {
            get
            {
                if (!NameSet) // Provide a default name if none has ever been set.
                {
                    name = $"{Type}{ID:X}";
                }
                return name;
            }
            set
            {
                NameSet = true;
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
            return $"{ID:X2}: {Name}";
        }

        // write a magic number depending on the (concrete) type i am, then let that type's implementation do the rest of the work.
        // that is, we expect that all subclass Serialize methods begin by calling base.Serialize(stream) (i.e. this method).
        public virtual void Serialize(Stream stream)
        {
            var mytype = GetType().Name;
            uint magicNumber = 0;
            switch (mytype)
            {
                case nameof(FileDevice):
                    magicNumber = SERIALIZATION_FILE_DEVICE_MAGIC_NUMBER;
                    break;
                case nameof(ConsoleDevice):
                    magicNumber = SERIALIZATION_CONSOLE_DEVICE_MAGIC_NUMBER;
                    break;
                default:
                    Debug.Fail("I don't know how to serialize that type of IODevice.");
                    break;
            }
            if (magicNumber != 0)
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.Write(magicNumber);
                    writer.Write(ID);
                    writer.Write(Name);
                }
            }

            // (Now control flows back to subclass Serialize method...)
        }

        public static IODevice Deserialize(Stream stream)
        {
            uint magicNumber;
            byte id;
            string name;
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                magicNumber = reader.ReadUInt32();
                id = reader.ReadByte();
                name = reader.ReadString();
            }
            if (_TYPES.TryGetValue(magicNumber, out Type subclass))
            {
                var ret = (IODevice)subclass.GetConstructor(new Type[] { typeof(byte) }).Invoke(new object[] { id });
                ret.Name = name;
                var subdes = subclass.GetMethod(SUBCLASS_DESERIALIZE_METHOD_NAME, BindingFlags.NonPublic);
                if (subdes != null)
                    subdes.Invoke(ret, new object[] { stream });
                else
                    Debug.WriteLine($"IODevice type \"{subclass.Name}\" did not contain a \"{SUBCLASS_DESERIALIZE_METHOD_NAME}\" method. Subclass-specific deserialization will not be performed.");
                return ret;
            }
            else
            {
                Debug.Fail("IODevice deserialization failed: Unknown magic number");
                throw new InvalidDataException();
            }
        }
    }
}
