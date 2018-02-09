using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace sicsim
{
    /// <summary>
    /// Represents the entire state of a session in vsic, including whatever machine is present.
    /// </summary>
    class Session
    {
        public Machine Machine
        { get; private set; }
        SortedSet<Breakpoint> breakpoints;

        /// <summary>
        /// Creates a new, empty session.
        /// </summary>
        public Session()
        {
            Machine = new Machine(100);
            breakpoints = new SortedSet<Breakpoint>(new Breakpoint.Comparer());
            Logger = new NullLog();
        }

        public ILogSink Logger
        { get; set; }

        /// <summary>
        /// Loads a binary file into the machine's memory at the specified address.
        /// </summary>
        /// <param name="path">The path to the binary file.</param>
        /// <param name="address">The destination address to copy the data.</param>
        public void LoadMemory(string path, Word address)
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
                    return;
                }

                // Read the file and copy it into the machine.
                // Doing this without a loop will not be possible when fileSize > int.MaxValue.
                // This is okay, since no SIC machine's memory is that big.
                byte[] buf = new byte[fileSize];
                read.Read(buf, 0, (int)fileSize);
                Machine.DMAWrite(buf, (int)address);
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
        }
    }
}
