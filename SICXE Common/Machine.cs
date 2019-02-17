//#define DEBUG_PRINT_ADDRESS_TYPE
//#define ENABLE_FEATURE_SAVE_COMPRESSION // this doesn't work yet.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SICXE.Devices;
using SICXE.Exceptions;
using System.Numerics;
//using SICXE.Extensions;
//using System.IO.Compression; // Don't use; GZipStream sucks

namespace SICXE
{
    public sealed class Machine : ISerialize
    {
        #region Serialization
        private const uint SERIALIZATION_MACHINE_MAGIC_NUMBER = 0x3AC817E;
        private const uint SERIALIZATION_VERSION = 0x00010001;
        private const uint SERIALIZATION_MEMORY_MAGIC_NUMBER = 0x11223344;
        private const uint SERIALIZATION_REGISTERS_MAGIC_NUMBER = 0x13E6157E;
        private const uint SERIALIZATION_DEVICES_MAGIC_NUMBER = 0xDEFF1CE5;
        private const uint BUFFER_SIZE = 4096;
        public void Serialize(Stream stream)
        {
            var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true);
            writer.Write(SERIALIZATION_MACHINE_MAGIC_NUMBER);
            writer.Write(SERIALIZATION_VERSION);

            // Write memory header.
            writer.Write(SERIALIZATION_MEMORY_MAGIC_NUMBER);
            int memoryLength = (int)Memory.Length;
            Debug.WriteLine($"Writing memory length {memoryLength}");
            writer.Write(memoryLength);
            Debug.WriteLine($"Stream position is now {stream.Position}");
            // Write memory with compression.
            Memory.Seek(0, SeekOrigin.Begin);
#if ENABLE_FEATURE_SAVE_COMPRESSION
            BZip2.Compress(Memory, stream, false, 5);
#else
            Memory.CopyTo(stream);
#endif
            // Write registers.
            Debug.WriteLine($"About to write registers. Stream position is now {stream.Position}");
            writer.Write(SERIALIZATION_REGISTERS_MAGIC_NUMBER);
            writer.Write((int)Register.A);
            writer.Write(regA);
            writer.Write((int)Register.B);
            writer.Write(regB);
            writer.Write((int)Register.CC);
            writer.Write((int)cc);
            writer.Write((int)Register.L);
            writer.Write(regL);
            writer.Write((int)Register.PC);
            writer.Write(PC);
            writer.Write((int)Register.S);
            writer.Write(regS);
            writer.Write((int)Register.T);
            writer.Write(regT);
            writer.Write((int)Register.X);
            writer.Write(regX);
            Debug.WriteLine($"Wrote registers. Stream position is now {stream.Position}");

            writer.Write(SERIALIZATION_DEVICES_MAGIC_NUMBER);
            // serialize devices 
            foreach (var dev in Devices)
            {
                dev.Serialize(stream);
            }

            writer.Dispose();
            Debug.WriteLine($"Disposed binarywriter. Stream position is now {stream.Position}");
            stream.Dispose();
        }

        public void Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true);
            uint uintbuf = reader.ReadUInt32();
            if (uintbuf != SERIALIZATION_MACHINE_MAGIC_NUMBER)
                throw new InvalidDataException();
            uintbuf = reader.ReadUInt32();
            if (uintbuf != SERIALIZATION_VERSION)
                throw new InvalidDataException($"Expected version {SERIALIZATION_VERSION:X}, got version {uintbuf:X} instead.");

            // Deserialize memory
            uintbuf = reader.ReadUInt32();
            if (uintbuf != SERIALIZATION_MEMORY_MAGIC_NUMBER)
                throw new InvalidDataException();

            int memoryLength = reader.ReadInt32();
            Debug.WriteLine($"Read memory length {memoryLength}");
            Memory.Dispose();
            memory = new byte[memoryLength];
            Memory = new MemoryStream(memory, true);

            Debug.WriteLine($"Stream position is now {stream.Position}");
            Memory.Seek(0, SeekOrigin.Begin);
#if ENABLE_FEATURE_SAVE_COMPRESSION
            BZip2.Decompress(stream, Memory, false); // todo: change how this is done.
#else
            int memoryBytesRead = stream.Read(memory, 0, memoryLength);
            Debug.WriteLine($"{memoryBytesRead} bytes read into memory");
#endif
            Debug.WriteLine($"Stream position is now {stream.Position}");

            // Deserialize registers
            uintbuf = reader.ReadUInt32();
            if (uintbuf != SERIALIZATION_REGISTERS_MAGIC_NUMBER)
                throw new InvalidDataException();

            int intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.A)
                throw new InvalidDataException();
            regA = (Word)reader.ReadInt32();
            intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.B)
                throw new InvalidDataException();
            regB = (Word)reader.ReadInt32();
            intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.CC)
                throw new InvalidDataException();
            cc = (ConditionCode)reader.ReadInt32();
            intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.L)
                throw new InvalidDataException();
            regL = (Word)reader.ReadInt32();
            intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.PC)
                throw new InvalidDataException();
            PC = reader.ReadInt32();
            intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.S)
                throw new InvalidDataException();
            regS = (Word)reader.ReadInt32();
            intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.T)
                throw new InvalidDataException();
            regT = (Word)reader.ReadInt32();
            intbuf = reader.ReadInt32();
            if (intbuf != (int)Register.X)
                throw new InvalidDataException();
            regX = (Word)reader.ReadInt32();

            // Deserialize devices
            // Destroy all existing devices.
            for (byte id = 0; id < byte.MaxValue; ++id)
                RemoveDevice(id);

            uintbuf = reader.ReadUInt32();
            if (uintbuf != SERIALIZATION_DEVICES_MAGIC_NUMBER)
                throw new InvalidDataException();

            while (stream.Position < stream.Length)
            {
                IODevice newDev = null;
                try
                {
                    newDev = IODevice.Deserialize(stream);
                }
                catch (InvalidDataException ide)
                {
                    Logger.LogError($"Failed to create an I/O device: {GetInnermost(ide).Message}");
                }
                if (newDev != null)
                    AddDevice(newDev.ID, newDev);
            }

            reader.Dispose();
            stream.Dispose();
        }

        private static Exception GetInnermost(Exception e)
        {
            Exception inner = e.InnerException;
            while (inner != null)
            {
                e = inner;
                inner = e.InnerException;
            }
            return e;
        }

        // for debug.
        private static void dumpstream(Stream s)
        {
            int buflen = (int)(s.Length - s.Position);
            var buf = new byte[buflen];
            s.Read(buf, 0, buflen);
            var str = string.Join("", buf.Select(b => b.ToString("x2")));
            Debug.WriteLine(str);

            var text = System.Text.Encoding.UTF8.GetString(buf);
            // this isn't rendering in debug output because it contains \0 chars
        }
        #endregion

        /// <summary>
        /// Gets or sets whether this Machine should write to the log each time it executes an instruction. Default is false.
        /// </summary>
        public bool LogEachInstruction
        { get; set; }

        public ILogSink Logger
        { get; set; }

        /// <summary>
        /// The number of instructions this Machine has ever executed.
        /// </summary>
        public BigInteger InstructionsExecuted
        {
            get;
            private set;
        }

        #region Memory and registers
        public const int SIC_MEMORY_SIZE = 0x8000; // 32K
        public const int SICXE_MEMORY_SIZE = 0x100000; // 1M

        private const byte MEMORY_INITIAL_VALUE = 0xff;
        private readonly Word REG_INITIAL_VALUE = (Word)0xffffff;

        /// <summary>
        /// A method called when the MemoryChanged event fires, indicating the Machine's memory has changed.
        /// </summary>
        /// <param name="startAddress">The first address that was changed in memory.</param>
        /// <param name="count">Size of the region (measured in bytes) beginning at "startAddress" that contains all modified addresses.</param>
        /// <param name="written">Indicates whether the specified address was written or read.</param>
        /// <returns>A bool indicating whether the machine should break execution.</returns>
        public delegate bool MemoryEventHandler(Word startAddress, int count, bool written);
        public event MemoryEventHandler MemoryChanged;

        /// <summary>
        /// A method called when the RegisterChanged event fires, indicating the Machine's memory has changed.
        /// </summary>
        /// <param name="reg">The register that was changed.</param>
        /// <param name="written">Indicates whether the specified register was written or read.</param>
        public delegate void RegisterEventHandler(Register reg, bool written);
        public event RegisterEventHandler RegisterChanged;

        private int PC; // Implementing this as an int saves a lot of casts in this class.
        public Word ProgramCounter
        {
            get { return (Word)PC; }
            set
            {
                if (value >= memory.Length || value < 0)
                {
                    throw new ArgumentException("Address is out of range.", nameof(value));
                }
                PC = value;
                RegisterChanged?.Invoke(Register.PC, true);
            }
        }

        private ConditionCode cc;
        public ConditionCode ConditionCode
        {
            get { return cc; }
            set
            {
                cc = value;
                RegisterChanged?.Invoke(Register.CC, true);
            }
        }

        /// <summary>
        /// Gets the machine's memory size in bytes.
        /// </summary>
        public int MemorySize
        {
            get { return memory.Length; }
        }

        private byte[] memory;

        // Registers as private fields that don't have any side effects when accessed.
        private Word regA, regB, regL, regS, regT, regX;

        // Registers as public properties that raise event on WRITE ONLY.
        public Word RegisterA
        {
            get { return regA; }
            set
            {
                RegisterAWithEvents = value;
            }
        }

        public Word RegisterB
        {
            get { return regB; }
            set
            {
                RegisterBWithEvents = value;
            }
        }

        public Word RegisterL
        {
            get { return regL; }
            set
            {
                RegisterLWithEvents = value;
            }
        }

        public Word RegisterS
        {
            get { return regS; }
            set
            {
                RegisterSWithEvents = value;
            }
        }

        public Word RegisterT
        {
            get { return regT; }
            set
            {
                RegisterTWithEvents = value;
            }
        }

        public Word RegisterX
        {
            get { return regX; }
            set
            {
                RegisterXWithEvents = value;
            }
        }

        // Registers as private properties that raise events on READ AND WRITE.
        private Word RegisterAWithEvents
        {
            get
            {
                RegisterChanged?.Invoke(Register.A, false);
                return regA;
            }
            set
            {
                regA = value;
                RegisterChanged?.Invoke(Register.A, true);
            }
        }

        private Word RegisterBWithEvents
        {
            get
            {
                RegisterChanged?.Invoke(Register.B, false);
                return regB;
            }
            set
            {
                regB = value;
                RegisterChanged?.Invoke(Register.B, true);
            }
        }

        private Word RegisterLWithEvents
        {
            get
            {
                RegisterChanged?.Invoke(Register.L, false);
                return regL;
            }
            set
            {
                regL = value;
                RegisterChanged?.Invoke(Register.L, true);
            }
        }

        private Word RegisterSWithEvents
        {
            get
            {
                RegisterChanged?.Invoke(Register.S, false);
                return regS;
            }
            set
            {
                regS = value;
                RegisterChanged?.Invoke(Register.S, true);
            }
        }

        private Word RegisterTWithEvents
        {
            get
            {
                RegisterChanged?.Invoke(Register.T, false);
                return regT;
            }
            set
            {
                regT = value;
                RegisterChanged?.Invoke(Register.T, true);
            }
        }

        private Word RegisterXWithEvents
        {
            get
            {
                RegisterChanged?.Invoke(Register.X, false);
                return regX;
            }
            set
            {
                regX = value;
                RegisterChanged?.Invoke(Register.X, true);
            }
        }

        public Stream Memory
        {
            get;
            private set;
        }
        #endregion

        public Machine(int memorySize = SICXE_MEMORY_SIZE)
        {
            memory = new byte[memorySize];
            Memory = new MemoryStream(memory, true);

            for (int i = 0; i < memory.Length; ++i)
            {
                memory[i] = MEMORY_INITIAL_VALUE;
            }

            regA = REG_INITIAL_VALUE;
            regB = REG_INITIAL_VALUE;
            regL = REG_INITIAL_VALUE;
            regS = REG_INITIAL_VALUE;
            regT = REG_INITIAL_VALUE;
            regX = REG_INITIAL_VALUE;
            PC = 0;

            Logger = new NullLog();
            LogEachInstruction = false;
        }

        #region Devices
        private readonly IODevice[] devices = new IODevice[256];
        /// <summary>
        /// The devices this machine has.
        /// </summary>
        public IReadOnlyList<IODevice> Devices
        {
            get { return devices.Where(d => d != null).ToList(); }
        }

        /// <summary>
        /// Adds the given device to the machine, with the specified ID. If a device with the specified ID number already exists, an exception is thrown.
        /// </summary>
        /// <param name="id">The ID to be associated with the device to add.</param>
        /// <param name="device">The device to add.</param>
        public void AddDevice(byte id, IODevice device)
        {
            if (devices[id] != null)
                throw new ArgumentException("A device with that ID already exists!");
            devices[id] = device;
        }

        /// <summary>
        /// Removes the device with the specified ID from the machine.
        /// </summary>
        /// <param name="id">The ID of the device to remove.</param>
        /// <returns>True if the device was removed, false if no device had the specified ID.</returns>
        public bool RemoveDevice(byte id)
        {
            if (devices[id] == null)
                return false; // No such device.

            devices[id].Dispose();
            devices[id] = null;
            return true;
        }

        /// <summary>
        /// Gets the device that has the specified ID.
        /// </summary>
        /// <param name="id">The ID of the device.</param>
        /// <returns>The device that has the specified ID, or null if the machine has no such device.</returns>
        public IODevice GetDevice(byte id)
        {
            return devices[id];
        }

        /// <summary>
        /// Tells all this machine's I/O devices to flush their internal buffers.
        /// </summary>
        public void FlushDevices()
        {
            devices.AsParallel().ForAll(d =>
            {
                d?.Flush();
            });
        }

        /// <summary>
        /// Closes all I/O devices associated with this machine.
        /// </summary>
        public void CloseDevices()
        {
            devices.AsParallel().ForAll(d =>
            {
                d?.Dispose();
            });
        }

        #endregion

        #region DMA functions
        /// <summary>
        /// Copies the given data into this Machine's memory, beginning at the specified address.
        /// Safety: This method will either perform the entire copy, or no memory will be changed.
        /// </summary>
        /// <param name="data">The data to copy.</param>
        /// <param name="address">The destination address.</param>
        public void DMAWrite(byte[] data, int address)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (address < 0)
                throw new ArgumentException("Address must be nonnegative.", nameof(address));
            if (address + data.Length > memory.Length)
                throw new ArgumentException("Write would go past the end of memory.");

            Array.ConstrainedCopy(data, 0, memory, 0, data.Length);
            //Buffer.BlockCopy(data, 0, memory, 0, data.Length);
        }

        /// <summary>
        /// Copies the specified number of words from this Machine's memory into a buffer.
        /// </summary>
        /// <param name="buffer">The destination array for the memory.</param>
        /// <param name="length">The maximum number of words to read.</param>
        /// <param name="address">The memory address to begin copying.</param>
        /// <returns>The number of words that were read.</returns>
        public int DMARead(byte[] buffer, int length, int address)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (length > buffer.Length)
                throw new ArgumentException("Length argument must not exceed that of the given buffer.");
            if (length < 0)
                throw new ArgumentException("Length must be nonnegative.", nameof(length));
            if (address >= memory.Length)
                throw new ArgumentException("Read must begin before the end of memory.");

            int stop = address + length;
            if (stop > memory.Length)
                stop = memory.Length;
            for (int i = address; i < stop; ++i)
            {
                buffer[i - address] = memory[i];
            }
            return stop - address;
        }
        #endregion

        #region Execution

        private bool running;
        public bool IsRunning
        {
            get { return running; }
            private set
            {
                if (running != value)
                {
                    running = value;
                    RunStateChanged?.Invoke(this, null);
                }
            }
        }
        public event EventHandler RunStateChanged;

        public enum RunResult
        {
            /// <summary>
            /// Nominal result.
            /// </summary>
            None = 0,
            /// <summary>
            /// A breakpoint has been hit.
            /// </summary>
            HitBreakpoint = 1,
            /// <summary>
            /// The instruction at the program counter could not be decoded.
            /// </summary>
            IllegalInstruction = 2,
            /// <summary>
            /// A hardware fault has occurred. Nothing should cause this.
            /// </summary>
            HardwareFault = 3,
            /// <summary>
            /// No execution was done because the program counter is at the end of memory.
            /// </summary>
            EndOfMemory = 4,
            /// <summary>
            /// A user-supplied cancellation token was signalled, causing execution to break.
            /// </summary>
            CancellationSignalled = 5,
            /// <summary>
            /// An address referenced in an instruction is out of bounds.
            /// </summary>
            AddressOutOfBounds = 6,
            /// <summary>
            /// An instruction attempted to divide by zero.
            /// </summary>
            DivideByZero = 7
        }

        public RunResult Run(Word address)
        {
            ProgramCounter = address; // We use property here to catch out-of-range.
            return Run();
        }

        public RunResult Run()
        {
            IsRunning = true;
            RunResult ret;
            try
            {
                do
                {
                    ret = Step();
                } while (ret == RunResult.None);
                return ret;
            }
            finally
            {
                IsRunning = false;
            }
        }

        public RunResult Run(CancelToken token)
        {
            RunResult ret;
            bool cancelledYet;
            IsRunning = true;
            try
            {
                do
                {
                    ret = Step();
                    cancelledYet = token.Cancelled;
                } while (ret == RunResult.None && !cancelledYet);
                if (cancelledYet)
                {
                    token.Reset();
                    return LastRunResult = RunResult.CancellationSignalled;
                }
                return ret;
            }
            finally
            {
                IsRunning = false;
            }
        }

        /// <summary>
        /// Executes the instruction at the program counter.
        /// </summary>
        /// <returns></returns>
        public RunResult Step()
        {
            if (PC >= memory.Length)
            {
                LastRunResult = RunResult.EndOfMemory;
                return LastRunResult;
            }
            int originalPC = PC; // Will be used to restore PC later if execution fails.
            try
            {
                ThrowForRead((Word)PC, 1); // We're about to do a memory read that should honor breakpoints. 
                byte b1 = memory[PC];
                if (PC < memory.Length)
                    ++PC;

                byte sextet = (byte)(b1 & 0xfc); // no opcode ends with 1, 2, or 3.
                if (Enum.IsDefined(typeof(Mnemonic), sextet))
                {
                    var op = (Mnemonic)sextet;
                    byte b2;
                    int r1, r2;
                    Word reg1value, reg2value;
                    Word addr;
                    AddressingMode mode;
                    byte deviceID;
                    IODevice dev;

                    switch (op)
                    {
                        // Arithmetic ------------------------------------------------------
                        case Mnemonic.ADD:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterAWithEvents = regA + ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.ADDR: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            reg1value = GetRegister(r1);
                            reg2value = GetRegister(r2);
                            SetRegister(r2, reg1value + reg2value);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {r1},{r2}.");
                            break;
                        case Mnemonic.SUB:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterAWithEvents = regA - ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.SUBR: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            reg1value = GetRegister(r1);
                            reg2value = GetRegister(r2);
                            SetRegister(r2, reg1value + reg2value);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {r1},{r2}.");
                            break;
                        case Mnemonic.MUL:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterAWithEvents = (Word)(regA * ReadWord(addr, mode));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.MULR: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            reg1value = GetRegister(r1);
                            reg2value = GetRegister(r2);
                            SetRegister(r2, (Word)(reg1value * reg2value));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {r1},{r2}.");
                            break;
                        case Mnemonic.DIV:
                            addr = DecodeLongInstruction(b1, out mode);
                            Word divisor = ReadWord(addr, mode);
                            if (divisor == Word.Zero)
                                throw new Exceptions.DivideByZeroException();
                            RegisterAWithEvents = (Word)(regA / divisor);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.DIVR: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            reg1value = GetRegister(r1);
                            reg2value = GetRegister(r2);
                            SetRegister(r2, (Word)(reg1value / reg2value));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {r1},{r2}.");
                            break;
                        // Bitwise ---------------------------------------------------------
                        case Mnemonic.AND:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterAWithEvents = (Word)(regA & ReadWord(addr, mode));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.OR:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterAWithEvents = (Word)(regA | ReadWord(addr, mode));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.SHIFTL:
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            reg1value = GetRegister(r1);
                            SetRegister(r1, (Word)(reg1value << r2));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {r1},{r2}.");
                            break;
                        case Mnemonic.SHIFTR:
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            reg1value = GetRegister(r1);
                            SetRegister(r1, (Word)(reg1value >> r2));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {r1},{r2}.");
                            break;
                        // Registers -------------------------------------------------------
                        case Mnemonic.LDA:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterAWithEvents = ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.LDB:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterBWithEvents = ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.LDL:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterLWithEvents = ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.LDS:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterSWithEvents = ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.LDT:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterTWithEvents = ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.LDX:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterXWithEvents = ReadWord(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.STA:
                            addr = DecodeLongInstruction(b1, out mode);
                            WriteWord(RegisterAWithEvents, addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.STB:
                            addr = DecodeLongInstruction(b1, out mode);
                            WriteWord(RegisterBWithEvents, addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.STL:
                            addr = DecodeLongInstruction(b1, out mode);
                            WriteWord(RegisterLWithEvents, addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.STS:
                            addr = DecodeLongInstruction(b1, out mode);
                            WriteWord(RegisterSWithEvents, addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.STT:
                            addr = DecodeLongInstruction(b1, out mode);
                            WriteWord(RegisterTWithEvents, addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.STX:
                            addr = DecodeLongInstruction(b1, out mode);
                            WriteWord(RegisterXWithEvents, addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.COMPR: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            reg1value = GetRegister(r1);
                            reg2value = GetRegister(r2);
                            ConditionCode = CompareWords(reg1value, reg2value);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {r1},{r2}.");
                            break;
                        case Mnemonic.RMO: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            r2 = b2 & 0xf;
                            Word r1value;
                            try
                            {
                                r1value = GetRegister(r1);
                            }
                            catch (SICXEException ex)
                            {
                                throw new IllegalInstructionException((Word)PC, ex);
                            }
                            switch ((Register)r2)
                            {
                                case Register.A:
                                    RegisterAWithEvents = r1value;
                                    break;
                                case Register.B:
                                    RegisterBWithEvents = r1value;
                                    break;
                                case Register.L:
                                    RegisterLWithEvents = r1value;
                                    break;
                                case Register.S:
                                    RegisterSWithEvents = r1value;
                                    break;
                                case Register.T:
                                    RegisterTWithEvents = r1value;
                                    break;
                                case Register.X:
                                    RegisterXWithEvents = r1value;
                                    break;
                            }
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {Enum.GetName(typeof(Register), r1)},{Enum.GetName(typeof(Register), r2)}.");
                            break;
                        case Mnemonic.CLEAR: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            SetRegister(r1, Word.Zero);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {Enum.GetName(typeof(Register), r1)}.");
                            break;
                        case Mnemonic.TIXR: // format 2
                            ThrowForRead((Word)PC, 1);
                            b2 = memory[PC++];
                            r1 = (b2 & 0xf0) >> 4;
                            ++regX;
                            ConditionCode = CompareWords(RegisterXWithEvents, GetRegister(r1));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {Enum.GetName(typeof(Register), r1)}.");
                            break;
                        case Mnemonic.LDCH:
                            // This instruciton operates on a single byte, not a word.
                            addr = DecodeLongInstruction(b1, out mode);
                            //regA = (Word)(regA | ~0xff); // Zero out lowest byte.
                            addr = DecodeAddress(addr, mode);
                            RegisterAWithEvents = (Word)((regA & ~0xff) | memory[addr]); // Or in lowest byte from memory.
                            ThrowForRead(addr, 1);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.STCH:
                            // This instruction operates on a single byte, not a word.
                            addr = DecodeLongInstruction(b1, out mode);
                            memory[addr] = (byte)(RegisterAWithEvents & 0xff);
                            ThrowForWrite(addr, 1);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        // Flow control ---------------------------------------------------
                        case Mnemonic.J:
                            addr = DecodeLongInstruction(b1, out mode);
                            PC = DecodeAddress(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.JEQ:
                            addr = DecodeLongInstruction(b1, out mode);
                            if (ConditionCode == ConditionCode.EqualTo)
                                PC = DecodeAddress(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.JGT:
                            addr = DecodeLongInstruction(b1, out mode);
                            if (ConditionCode == ConditionCode.GreaterThan)
                                PC = DecodeAddress(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.JLT:
                            addr = DecodeLongInstruction(b1, out mode);
                            if (ConditionCode == ConditionCode.LessThan)
                                PC = DecodeAddress(addr, mode);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.JSUB:
                            addr = DecodeLongInstruction(b1, out mode);
                            RegisterL = (Word)PC;
                            PC = DecodeAddress(addr, mode);
                            LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.RSUB:
                            PC = RegisterLWithEvents;
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op}.");
                            break;
                        // I/O ----------------------------------------------------------
                        case Mnemonic.TD:
                            addr = DecodeLongInstruction(b1, out mode);
                            deviceID = memory[DecodeAddress(addr, mode)];
                            dev = devices[deviceID];
                            if (dev != null)
                            {
                                if (dev.Test())
                                {
                                    cc = ConditionCode.GreaterThan;
                                }
                                else
                                {
                                    cc = ConditionCode.EqualTo; // Equal means not ready.
                                    Debug.WriteLine($"Device {deviceID:X2} is not ready.");
                                }
                            }
                            else
                            {
                                cc = ConditionCode.EqualTo;
                                Debug.WriteLine($"Device {deviceID:X2} does not exist!");
                            }
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.WD:
                            addr = DecodeLongInstruction(b1, out mode);
                            deviceID = memory[DecodeAddress(addr, mode)];
                            dev = devices[deviceID];
                            dev.WriteByte((byte)(RegisterAWithEvents & 0xff));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        case Mnemonic.RD:
                            addr = DecodeLongInstruction(b1, out mode);
                            deviceID = memory[DecodeAddress(addr, mode)];
                            dev = devices[deviceID];
                            byte rb = dev.ReadByte();
                            Debug.WriteLine($"devices[{deviceID:X2}].ReadByte() returned {rb:X2}.");
                            RegisterAWithEvents = (Word)(regA & ~0xff | rb);
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                        // Other --------------------------------------------------------
                        case Mnemonic.COMP:
                            addr = DecodeLongInstruction(b1, out mode);
                            ConditionCode = CompareWords(RegisterAWithEvents, ReadWord(addr, mode));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}");
                            break;
                        case Mnemonic.TIX:
                            addr = DecodeLongInstruction(b1, out mode);
                            ++regX;
                            ConditionCode = CompareWords(RegisterXWithEvents, ReadWord(addr, mode));
                            if (LogEachInstruction)
                                LogInstruction(originalPC, $"{op} {addr}.");
                            break;
                    }
                }
                else
                {
                    PC = originalPC;
                    LastRunResult = RunResult.IllegalInstruction;
                    return RunResult.IllegalInstruction;
                }
            }
            catch (IllegalInstructionException) // May be thrown by DecodeLongAddress.
            {
                PC = originalPC;
                LastRunResult = RunResult.IllegalInstruction;
                return LastRunResult;
            }
            catch (IndexOutOfRangeException)
            {
                if (PC >= memory.Length - 4)
                {
                    LastRunResult = RunResult.EndOfMemory;
                }
                else
                {
                    LastRunResult = RunResult.AddressOutOfBounds;
                }
                PC = originalPC;
                return LastRunResult;
            }
            catch (Exceptions.DivideByZeroException)
            {
                PC = originalPC; // Do not advance past the offending instruction.
                LastRunResult = RunResult.DivideByZero;
                return LastRunResult;
            }
            catch (BreakpointHitException bhe)
            {
                LastBreakpointHitException = bhe; // Store it so clients can inspect it (i.e. get the address that caused it, which is not necessarily the PC).
                PC = originalPC;
                LastRunResult = RunResult.HitBreakpoint;
                return LastRunResult;
            }
            catch (SICXEException) // Generic/unknown exception
            {
                PC = originalPC;
                LastRunResult = RunResult.HardwareFault;
                return LastRunResult;
            }

            // No error occurred.
            ++InstructionsExecuted;
            LastRunResult = RunResult.None;
            return LastRunResult;
        }

        /// <summary>
        /// A helper method for logging that an instruction was executed.
        /// </summary>
        /// <param name="addr">The address of the instruction</param>
        /// <param name="str">The string representation of the instruction</param>
        private void LogInstruction(int addr, string str)
        {
            Logger.Log($"0x{addr:X6}: {str}");
        }

        /// <summary>
        /// Gets the result of the last time Run or Step was called.
        /// </summary>
        public RunResult LastRunResult
        {
            get; private set;
        }

        public BreakpointHitException LastBreakpointHitException
        { get; private set; }

        /// <summary>
        /// Decodes the flags and operand of a standard SIC or SIC/XE format 3 or 4 instruction, while the progrm counter is at the second byte of the instruction.
        /// Advances the program counter to the end of the current instruction and returns the target address (operand) it indicates.
        /// </summary>
        /// <param name="ni">The byte whose lowest 2 bits represent N and I, repsectively. All other bits are ignored.</param>
        /// <param name="indirection">Indicates what level of indirection should be used on the returned operand.</param>
        /// <returns>The operand found at the given address. The meaning of the operand is subject to the value of "indirection".</returns>
        private Word DecodeLongInstruction(byte ni, out AddressingMode indirection)
        {
            ni &= 0x3; // keep only bottom 2 bits.

            int oldPC = PC; // for error reporting.

            ThrowForRead((Word)PC, 1);
            byte b2 = memory[PC++]; // We don't test for OOB in this method--we let Step() catch it.
            byte flags = (byte)(ni << 4);
            /* 00100000 0x20    N 
             * 00010000 0x10    I
             * 00001000 8       X
             * 00000100 4       B
             * 00000010 2       P
             * 00000001 1       E
             */
            if (flags == 0) // SIC-compatible instruction.
            {
                ThrowForRead((Word)PC, 1);
                // Format of standard SIC instruction (24 bits total):
                //   8      1     15
                // opcode   x   address
                if ((b2 & 0x8) == 0) // If X flag is not set.
                {
                    indirection = AddressingMode.Simple;
                    return (Word)((b2 & ~0x8) << 7 | memory[PC++]);
                }
                indirection = AddressingMode.Simple;
                return (Word)(RegisterXWithEvents + (b2 & ~0x8) << 7 | memory[PC++]);
            }
            flags |= (byte)((b2 & 0xf0) >> 4);
            int disp;
            switch (flags)
            {
                case 0b110000:   // disp
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("disp");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Simple;
                    return (Word)((b2 & 0xf) << 8 | memory[PC++]);
                case 0b110001: // addr (format 4)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("addr");
#endif
                    ThrowForRead((Word)PC, 2);
                    indirection = AddressingMode.Simple;
                    // Note: C# guarantees left-to-right evaluation, so stuff like this is fine.
                    return (Word)((b2 & 0xf) << 16 | memory[PC++] << 8 | memory[PC++]);
                case 0b110010: // (PC) + disp
                               // "For PC-relative addressing, [the disp] is interpreted as a 12-bit signed integer." (p. 9)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("pc + disp");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Simple;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + disp);
                case 0b110100: // (B) + disp
                               // "For base relative addressing, the displacement field disp in a Format 3 instruction is interpreted as a 12-bit unsigned integer." (p. 9)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("b + disp");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Simple;
                    return (Word)(RegisterBWithEvents + ((b2 & 0xf) << 8 | memory[PC++]));
                case 0b111000: // disp + (X)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("disp + x");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Simple;
                    return (Word)(RegisterXWithEvents + (b2 & 0xf) << 8 | memory[PC++]);
                case 0b111001: // addr + (X) (format 4)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("addr + x");
#endif
                    ThrowForRead((Word)PC, 2);
                    indirection = AddressingMode.Simple;
                    int addr = b2 & 0xf | memory[PC++] << 8 | memory[PC++];
                    return (Word)(RegisterXWithEvents + addr);
                case 0b111010: // (PC) + disp + (X)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("pc + disp + x");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Simple;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + RegisterXWithEvents + disp);
                case 0b111100: // (B) + disp + (X)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("b + disp + x");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Simple;
                    return (Word)(RegisterBWithEvents + RegisterXWithEvents + (b2 & 0xf) << 8 | memory[PC++]);
                case 0b100000: // disp
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("disp (indirect)");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Indirect;
                    return (Word)((b2 & 0xf) << 8 | memory[PC++]);
                case 0b100001: // addr (format 4)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("addr (indirect)");
#endif
                    ThrowForRead((Word)PC, 2);
                    indirection = AddressingMode.Indirect;
                    return (Word)((b2 & 0xf) << 16 | memory[PC++] << 8 | memory[PC++]);
                case 0b100010: // (PC) + disp
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("pc + disp (indirect)");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Indirect;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + disp);
                case 0b100100: // (B) + disp
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("b + disp (indirect)");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Indirect;
                    return (Word)(RegisterBWithEvents + (b2 & 0xf) << 8 | memory[PC++]);
                case 0b010000: // disp
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("disp (immediate)");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Immediate;
                    return (Word)DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                case 0b010001: // addr (format 4)
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("addr (immediate)");
#endif
                    ThrowForRead((Word)PC, 2);
                    indirection = AddressingMode.Immediate;
                    return (Word)((b2 & 0xf) << 16 | memory[PC++] << 8 | memory[PC++]);
                case 0b010010: // (PC) + disp
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("pc + disp (immediate)");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Immediate;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + disp);
                case 0b010100: // (B) + disp
#if DEBUG_PRINT_ADDRESS_TYPE
                    Debug.WriteLine("b + disp (immediate)");
#endif
                    ThrowForRead((Word)PC, 1);
                    indirection = AddressingMode.Immediate;
                    return (Word)(RegisterBWithEvents + (b2 & 0xf) << 8 | memory[PC++]);
            }
            throw new IllegalInstructionException((Word)oldPC);
        }

        private ConditionCode CompareWords(Word x, Word y)
        {
            var xv = (int)x;
            var yv = (int)y;
            if (xv < yv)
                return ConditionCode.LessThan;
            if (xv > yv)
                return ConditionCode.GreaterThan;
            return ConditionCode.EqualTo;
        }

        // Bits higher than 'bitCount' will be cleared if input is deemed negative. I cannot imagine any such bits should be set in the first place.
        private int DecodeTwosComplement(int n, int bitCount)
        {
            int highBit = 1 << (bitCount - 1);

            int lowerMask = highBit | highBit - 1; // The bits we don't care about.

            if ((n & ~lowerMask) > 0)
                Debug.WriteLine("Warning: Higher bits are set than are supposed to be in this number!");

            if ((n & highBit) > 0) // If sign bit is set.
            {
                // Number is negative: invert and increment.
                return -((~n + 1) & lowerMask);
            }

            // Number was positive all along--no change was necessary.
            // Optimizations will include skipping this method call in such cases.
            return n;
        }

        /// <summary>
        /// Helper function to get the value stored in a register from its binary code.
        /// </summary>
        private Word GetRegister(int r)
        {
            var reg = (Register)r;
            switch (reg)
            {
                case Register.A:
                    RegisterChanged?.Invoke(reg, false);
                    return regA;
                case Register.B:
                    RegisterChanged?.Invoke(reg, false);
                    return regB;
                case Register.L:
                    RegisterChanged?.Invoke(reg, false);
                    return regL;
                case Register.S:
                    RegisterChanged?.Invoke(reg, false);
                    return regS;
                case Register.T:
                    RegisterChanged?.Invoke(reg, false);
                    return regT;
                case Register.X:
                    RegisterChanged?.Invoke(reg, false);
                    return regX;
            }
            throw new SICXEException($"Illegal register code: '{r:X2}'.");
        }

        /// <summary>
        /// Helper function to set the value of a register specified by its binary code.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="value">The value to store in the register.</param>
        private void SetRegister(int r, Word value)
        {
            var reg = (Register)r;
            switch ((Register)r)
            {
                case Register.A:
                    regA = value;
                    break;
                case Register.B:
                    regB = value;
                    break;
                case Register.L:
                    regL = value;
                    break;
                case Register.S:
                    regS = value;
                    break;
                case Register.T:
                    regT = value;
                    break;
                case Register.X:
                    regX = value;
                    break;
                default:
                    throw new ArgumentException(nameof(r));
            }
            RegisterChanged?.Invoke(reg, true);
        }

        /// <summary>
        /// Sets the value of a register.
        /// </summary>
        /// <param name="r">The register to be changed.</param>
        /// <param name="value">The new value for the register.</param>
        public void SetRegister(Register r, Word value)
        {
            SetRegister((int)r, value);
        }

        private Word ReadWord(Word address, AddressingMode mode)
        {
            if (mode == AddressingMode.Immediate)
                return address;
            address = DecodeAddress(address, mode);

            ThrowForRead(address, 3);

            return Word.FromArray(memory, address);
        }

        private void WriteWord(Word w, Word address, AddressingMode mode)
        {
            if (mode != AddressingMode.Immediate)
            {
                address = DecodeAddress(address, mode);
            }
            var addr = (int)address;
            ThrowForWrite(address, 3);
            memory[addr] = w.High;
            memory[addr + 1] = w.Middle;
            memory[addr + 2] = w.Low;
        }

        /// <summary>
        /// Creates and throws a BreakpointHitException if appropriate for memory read from the specified region.
        /// </summary>
        /// <param name="address">The address to be checked for breakpoints.</param>
        /// <param name="count">Size in bytes of the region to check for breakpoints.</param>
        private void ThrowForRead(Word address, int count)
        {
            if (MemoryChanged == null)
                return;
            // Throw if ANY listener returned true.
            if (MemoryChanged.GetInvocationList().Aggregate(false,
                (bool existing, Delegate handler) => existing |= (bool)handler.DynamicInvoke(address, count, false)))
            {
                throw new BreakpointHitException(address, false);
            }
        }

        /// <summary>
        /// Creates and throws a BreakpointHitException if appropriate for memory write to the specified region.
        /// </summary>
        /// <param name="address">The address to be checked for breakpoints.</param>
        /// <param name="count">Size in bytes of the region to check for breakpoints.</param>
        private void ThrowForWrite(Word address, int count)
        {
            if (MemoryChanged == null)
                return;
            // Throw if ANY listener returned true.
            if (MemoryChanged.GetInvocationList().Aggregate(false,
                (bool existing, Delegate handler) => existing |= (bool)handler.DynamicInvoke(address, count, true)))
            {
                throw new BreakpointHitException(address, true);
            }
        }

        // Helper function for ReadWord and WriteWord.
        private Word DecodeAddress(Word address, AddressingMode mode)
        {
            switch (mode)
            {
                case AddressingMode.Immediate:
                    throw new ArgumentException("Addressing mode is immediate and should not be decoded!");
                case AddressingMode.Simple:
                    return address;
                case AddressingMode.Indirect:
                    return ReadWord(address, AddressingMode.Simple);
                default:
                    throw new ArgumentException("Illegal or unsupported addressing mode.");
            }
        }

        private void WriteByte(byte b, Word address)
        {
            memory[address] = b;
            //Debug.WriteLine($"memory[{(int)address}] = {memory[(int)address]}");
        }

        #endregion Execution

        /// <summary>
        /// Writes a portion of this machine's memory to the console.
        /// </summary>
        /// <param name="startAddress">The address of the first word to print.</param>
        /// <param name="count">The number of words to print.</param>
        public void PrintWords(int startAddress, int count)
        {
            int stop = startAddress + count;
            for (int wordIdx = startAddress; wordIdx < stop; wordIdx += 4)
            {
                Console.WriteLine("0x{0:X}: \t{1,8} {2,8} {3,8} {4,8}", wordIdx,
                 (int)memory[wordIdx],
                 (int)memory[wordIdx + 1],
                 (int)memory[wordIdx + 2],
                 (int)memory[wordIdx + 3]);
            }
        }

        public void LoadObj(string path)
        {
            Word blockAddr = (Word)(-1); // Initialized only to silence compiler warning.
            StreamReader read = null;
            int lineCount = 1;
            try
            {
                // First count the number of blocks in total.
                int blockCount = File.ReadAllLines(path).Count(l => l.Trim() == "!");

                read = new StreamReader(path);
                string line = null;
                int block;
                for (block = 0; block < blockCount - 1; ++block)
                {
                    // Read in the first line as the base address of this block.
                    line = read.ReadLine().Trim().ToLower();
                    ++lineCount;
                    blockAddr = (Word)Convert.ToInt32(line, 16);
                    Debug.WriteLine($"Block {block}'s base address is {blockAddr}.");

                    // Discard first line of non-final block. (?)
                    line = read.ReadLine().Trim().ToLower();
                    Debug.Assert(line == "000000", "Unexpected obj file format");

                    line = read.ReadLine().Trim().ToLower();
                    ++lineCount;
                    while (line != "!")
                    {
                        Debug.WriteLine($"Text in block {block}: {line}");
                        // Pair will always succeeded (i.e. find an even number of digits) for files assembled by sicasm.
                        foreach (var b in Pair(line).Select(p => Convert.ToByte(p, 16)))
                        {
                            WriteByte(b, blockAddr++);
                        }

                        line = read.ReadLine().Trim().ToLower();
                        ++lineCount;
                    }
                }

                // Load final block.
                line = read.ReadLine().Trim().ToLower();
                blockAddr = (Word)Convert.ToInt32(line, 16);
                Debug.WriteLine($"Block {block}'s base address is {blockAddr}.");

                line = read.ReadLine().Trim().ToLower();
                ++lineCount;
                int entryPoint = Convert.ToInt32(line, 16);
                Debug.WriteLine($"Read entry point {entryPoint:X2}.");
                while (true)
                {
                    line = read.ReadLine().Trim().ToLower();
                    ++lineCount;
                    if (line == "!")
                        break;
                    Debug.WriteLine($"Code/data in block {block}: {line}");
                    // Pair will always succeeded (i.e. find an even number of digits) for files assembled by sicasm.
                    foreach (var b in Pair(line).Select(p => Convert.ToByte(p, 16)))
                    {
                        WriteByte(b, blockAddr++);
                    }
                }
                PC = entryPoint;

            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    Logger.LogError("Error loading OBJ file \"{0}\".\n The file was not found.", path);
                    return;
                }
#if !DEBUG
                if (ex is FormatException || ex is IOException)
                {
                    Logger.LogError("Error loading OBJ file \"{0}\" at line {1}: {2}", path, lineCount, ex.Message);
                    Logger.LogError("The machine's state may be corrupt after an unsuccessful load.");
                    return;
                }
                else
                {
                    throw;
                }
#endif
            }
            finally
            {
                read?.Dispose();
            }
            Logger.Log("Loaded \"{0}\" successfully.", path);
        }

        public void MemoryRainbowTest()
        {
            for (Word i = Word.Zero; (int)i < MemorySize; ++i)
            {
                WriteByte((byte)i, i);
            }

            MemoryChanged?.Invoke(Word.Zero, MemorySize, true);
        }

        private static IEnumerable<string> Pair(string str)
        {
            for (int i = 0; i < str.Length; i += 2)
                yield return str.Substring(i, 2);
        }
    }
}
