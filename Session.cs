using System.IO;
using SICXE;

namespace Visual_SICXE
{
    /// <summary>
    /// Represents the entire state of a session in vsic, including whatever machine is present.
    /// </summary>
    internal class Session
    {
        public Machine Machine
        { get; }

        /// <summary>
        /// Creates a new, empty session.
        /// </summary>
        public Session()
        {
            Machine = new Machine();
            Logger = new NullLog();
        }

        private ILogSink logger;
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
                read = new FileStream(path, FileMode.Open);
                // Sanity check.
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
                read?.Dispose();
            }
            return 0; // Reachable only on error.
        }

        /// <summary>
        /// Writes a segment of the machine's memory to a file.
        /// </summary>
        /// <param name="path">The path of the file to write. If the file exists, it will be overwritten.</param>
        /// <param name="startAddress">The address of the first byte to be written.</param>
        /// <param name="length">The number of bytes to write. This parameter, plus the start address, must not exceed the total number of bytes in the memory space.</param>
        public void SaveMemory(string path, Word startAddress, int length)
        {
            FileStream writer = null;
            try
            {
                writer = new FileStream(path, FileMode.OpenOrCreate);
                lock (Machine)
                {
                    Machine.Memory.Seek(startAddress + 1, SeekOrigin.Begin);
                    Machine.Memory.CopyTo(writer);
                }
                writer.SetLength(length);
            }
            catch (IOException ex)
            {
                Logger.LogError("Error writing file \"{0}\": {1}", path, ex.Message);
            }
            finally
            {
                writer?.Dispose();
            }
        }

        public void SaveToFile(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Create);
                Machine.Serialize(stream);
            }
            finally
            {
                stream?.Dispose();
            }
        }

        public void LoadFromFile(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open);
                Machine.Deserialize(stream);
            }
            finally
            {
                stream?.Dispose();
            }
        }

    }
}
