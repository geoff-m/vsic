using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using IOPath = System.IO.Path;

namespace vsic
{
    public class FileDevice : IODevice
    {
        private FileStream fs;
        long recentlyWritten = 0;

        public string Path
        { get; private set; }

        /// <summary>
        /// Creates a new FileDevice that operates on the file with the given path. If the file does not exist, it will be created.
        /// </summary>
        /// <param name="id">The unique ID for this device.</param>
        /// <param name="path">The path of the file that backs this device.</param>
        public FileDevice(byte id, string path) : base(id)
        {
            Path = path;
            fs = new FileStream(path, FileMode.OpenOrCreate);
        }

        public override bool Test()
        {
            // We could replace this with randomness if we wanted to simulate slowness.
            return fs != null;
        }

        public override byte ReadByte()
        {
            if (fs.Position == fs.Length)
                return EOF;
            int b = fs.ReadByte();
            if (b < 0)
                return EOF;
            return (byte)b;
        }

        public override void WriteByte(byte b)
        {
            fs.WriteByte(b);
            ++recentlyWritten;
            if (recentlyWritten > 100)
                Flush();
        }

        // Todo: Low priority: Change this to a more robust solution. (Lower level API to force flush?)
        long position;
        public override void Flush()
        {
            if (recentlyWritten > 0)
            {
                Debug.WriteLine($"Device {ID.ToString("X")}: Flushing {recentlyWritten} bytes.");

                // All this is necessary because fs.Flush() does not actually guarantee disk update.
                position = fs.Position;
                fs.Dispose();
                // Race condition: hope nothing changes about the file between these two operations.
                fs = new FileStream(Path, FileMode.OpenOrCreate);
                fs.Position = position; // restore position to what it was before close and reopen.

                recentlyWritten = 0;
            }
            else
            {
                //Debug.WriteLine($"Device {ID.ToString("X")}: Flushing not necessary.");
            }
        }

        public override void Dispose()
        {
            fs.Dispose();
            fs = null;
        }

        public override string Name
        {
            get
            {
                if (!nameSet) // Provide a default name if none has ever been set.
                {
                    name = $"...{IOPath.DirectorySeparatorChar}{IOPath.GetFileName(Path)}";
                }
                return name;
            }
            set
            {
                nameSet = true;
                name = value;
            }
        }

        public override string Type
        {
            get { return "File"; }
        }
    }
}
