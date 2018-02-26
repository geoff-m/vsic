using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace vsic
{
    /// <summary>
    /// Represents the entire state of a session in vsic, including whatever machine is present.
    /// </summary>
    class Session
    {
        public Machine Machine
        { get; private set; }

        /// <summary>
        /// Creates a new, empty session.
        /// </summary>
        public Session()
        {
            Machine = new Machine();
            Machine.Devices[0x05] = new FileDevice(0x05, "goodbye.txt");
            Machine.Devices[0xf1] = new FileDevice(0xf1, "hello.txt");
            //Machine.MemoryRainbowTest();
            Logger = new NullLog();
            
        }

        ILogSink logger;
        public ILogSink Logger
        {
            get { return logger; }
            set
            {
                logger = value;
                Machine.Logger = value; // Give the same logger to my Machine.
            }
        }

        /// <summary>
        /// Loads a binary file into the machine's memory at the specified address.
        /// </summary>
        /// <param name="path">The path to the binary file.</param>
        /// <param name="address">The destination address to copy the data.</param>
        public long LoadMemory(string path, Word address)
        {
            FileStream read = null;
            try
            {
                // Sanity check.
                read = new FileStream(path, FileMode.Open);
                long fileSize = read.Length;
                int destinationWindowSize = Machine.MemorySize - (int)address;
                if (destinationWindowSize < fileSize)
                {
                    Logger.LogError("Error: The {0}-byte file is too large to load at address {1} ({2} bytes too long). No memory was changed.",
                        fileSize,
                        address,
                        fileSize - destinationWindowSize);
                    return 0;
                }

                long ret = read.Length;
                // Read the file and copy it into the machine at the desired location.
                lock (Machine)
                {
                    Machine.Memory.Seek((int)address + 1, SeekOrigin.Begin); // idk why i need + 1 here, but it seems I do.
                    read.CopyTo(Machine.Memory);
                }
                return ret;
            }
            catch (IOException ex)
            {
                Logger.LogError("Error loading file \"{0}\": {1}", path, ex.Message);
            }
            finally
            {
                if (read != null)
                    read.Dispose();
            }
            return 0; // Reachable only on error.
        }
        
    }
}
