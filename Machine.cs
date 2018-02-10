using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace sicsim
{
    public class Machine
    {
        #region Memory and registers
        public const int SIC_MEMORY_MAXIMUM = 0x8000; // 32K
        public const int SICXE_MEMORY_MAXIMUM = 0x100000; // 1M

        const byte MEMORY_INITIAL_VALUE = 0xff;
        readonly Word REG_INITIAL_VALUE = (Word)0xffffff;

        //public event EventHandler StateChanged; // unused

        public Word ProgramCounter
        { get; private set; }

        public ConditionCode ConditionCode
        { get; private set; }

        /// <summary>
        /// Gets the machine's memory size in bytes.
        /// </summary>
        public int MemorySize
        { get; } // a readonly property

        private byte[] memory;
        Word regA, regB, regL, regS, regT, regX;

        public Word RegisterA
        {
            get { return regA; }
            set { regA = value; }
        }

        public Word RegisterB
        {
            get { return regB; }
            set { regB = value; }
        }

        public Word RegisterL
        {
            get { return regL; }
            set { regL = value; }
        }

        public Word RegisterS
        {
            get { return regS; }
            set { regS = value; }
        }

        public Word RegisterT
        {
            get { return regT; }
            set { regT = value; }
        }

        public Word RegisterX
        {
            get { return regX; }
            set { regX = value; }
        }

        public Stream Memory
        {
            get;
            private set;
        }
        #endregion

        public Machine(int memorySize = SICXE_MEMORY_MAXIMUM)
        {
            memory = new byte[memorySize];
            Memory = new MemoryStream(memory, true);
            MemorySize = memory.Length;

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
            ProgramCounter = (Word)0;

            Logger = new NullLog();
        }

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
                throw new ArgumentException("Write would go past end of memory.");

            //Array.ConstrainedCopy(data, 0, memory, 0, data.Length);
            Buffer.BlockCopy(data, 0, memory, 0, data.Length);
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

        public RunResult Run(Word address)
        {
            int addr = (int)address;
            if (addr < 0 || addr >= memory.Length)
                throw new ArgumentException("Address is out of range.", nameof(address));

            ProgramCounter = address;
            return Run();
        }

        public RunResult Run()
        {
            RunResult ret;
            do
            {
                ret = Step();
            } while (ret == RunResult.None);
            return ret;
        }

        /// <summary>
        /// Executes the instruction at the program counter.
        /// </summary>
        /// <returns></returns>
        public RunResult Step()
        {
            /* instructions needed for 'addnums': 
            COMPR   2
            RMO     2
            LDA     3/4
            MUL     3/4
            LDX     3/4
            ADD     3/4
            STA     3/4
            STX     3/4
            JGT     3/4
            */


            int sextet = memory[(int)ProgramCounter++] & 0xfc; // no opcode ends with 1, 2, or 3.
            if (Enum.IsDefined(typeof(Mnemonic), sextet))
            {
                var op = (Mnemonic)sextet;
                byte b2;
                int r1, r2;
                switch (op)
                {
                    case Mnemonic.COMPR: // format 2
                        b2 = memory[(int)ProgramCounter++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        Word reg1value = GetRegister(r1);
                        Word reg2value = GetRegister(r2);
                        ConditionCode = CompareWords(reg1value, reg2value);
                        break;
                    case Mnemonic.RMO: // format 2
                        b2 = memory[(int)ProgramCounter++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        switch ((Register)r2)
                        {
                            case Register.A:
                                regA = GetRegister(r1);
                                break;
                            case Register.T:
                                regT = GetRegister(r1);
                                break;
                            case Register.X:
                                regX = GetRegister(r1);
                                break;
                        }
                        break;
                    case Mnemonic.LDA: // format 3/4

                        break;

                }
            }
            else
            {
                return RunResult.IllegalInstruction;
            }

            return RunResult.None; // if no error occurs.
        }

        private ConditionCode CompareWords(Word x, Word y)
        {
            int xv = (int)x;
            int yv = (int)y;
            if (xv < yv)
                return ConditionCode.LessThan;
            if (xv > yv)
                return ConditionCode.GreaterThan;
            return ConditionCode.EqualTo;
        }

        private Word GetRegister(int r)
        {
            switch ((Register)r)
            {
                case Register.A:
                    return regA;
                case Register.T:
                    return regT;
                case Register.X:
                    return regX;
            }
            throw new ArgumentException(nameof(r));
        }

        public enum RunResult
        {
            None = 0,
            HitBreakpoint = 1,
            IllegalInstruction = 2,
            HardwareFault = 3
        }

        #region Helper functions
        private Word ReadWord(Word address, AddressingMode mode)
        {
            if (mode == AddressingMode.Immediate)
                return address;
            //return memory[(int)DecodeAddress(address, mode)];
            address = DecodeAddress(address, mode);
            return Word.FromArray(memory, (int)address);
        }

        // Helper function for ReadWord and WriteWord.
        private Word DecodeAddress(Word address, AddressingMode mode)
        {
            switch (mode)
            {
                case AddressingMode.Immediate:
                    throw new ArgumentException("Addressing mode is immediate: address should not be decoded!");
                case AddressingMode.Simple: // todo: In Machine, replace this with Direct. "Simple" should be disallowed here.
                    return address;
                case AddressingMode.Indirect:
                    return address + regX;
                case AddressingMode.RelativeToBase:
                    return address + regB;
                case AddressingMode.RelativeToProgramCounter:
                    return address + ProgramCounter;
                default:
                    throw new ArgumentException("Illegal or unsupported addressing mode");
            }
        }

        private void WriteWord(Word w, Word address, AddressingMode mode)
        {
            if (mode != AddressingMode.Immediate)
            {
                address = DecodeAddress(address, mode);
            }
            //memory[(int)address] = w;
            int addr = (int)address;
            memory[addr] = w.High;
            memory[addr + 1] = w.Middle;
            memory[addr + 2] = w.Low;
        }

        private void WriteByte(byte b, Word address)
        {
            memory[(int)address] = b;
            Debug.WriteLine($"memory[{(int)address}] = {memory[(int)address]}");
        }

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
        #endregion

        public void LoadObj(string path)
        {
            Word blockAddr = (Word)(-1); // Initialized only to silence compiler warning.
            int lineCount = 1;
            int entryPoint;
            try
            {
                // First count the number of blocks in total.
                int blockCount = File.ReadAllLines(path).Count(l => l.Trim() == "!");

                var read = new StreamReader(path);
                string line = null;
                int block;
                for (block = 0; block < blockCount - 1; ++block)
                {
                    // Read in the first line as the base address of this block.
                    line = read.ReadLine().Trim().ToLower();
                    ++lineCount;
                    blockAddr = (Word)Convert.ToInt32(line, 16);
                    Debug.WriteLine($"Block {block}'s base address is {blockAddr}.");

                    line = read.ReadLine().Trim().ToLower();
                    ++lineCount;
                    do
                    {
                        Debug.WriteLine($"Code/data in block {block}: {line}");
                        // Pair will always succeeded (i.e. find an even number of digits) for files assembled by sicasm.
                        foreach (var b in Pair(line).Select(p => Convert.ToByte(p, 16)))
                        {
                            WriteByte(b, blockAddr++);
                        }

                        line = read.ReadLine().Trim().ToLower();
                        ++lineCount;
                    } while (line != "!");
                }

                // Load final block.
                line = read.ReadLine().Trim().ToLower();
                blockAddr = (Word)Convert.ToInt32(line, 16);
                Debug.WriteLine($"Block {block}'s base address is {blockAddr}.");

                line = read.ReadLine().Trim().ToLower();
                ++lineCount;
                entryPoint = Convert.ToInt32(line, 16);
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
                ProgramCounter = (Word)entryPoint;
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is IOException)
                {
                    Logger.LogError("Error loading \"{0}\" at line {1}: {2}", path, lineCount, ex.Message);
                    Logger.LogError("The machine's state may be corrupt after an unsuccessful load.");
                    return;
                }
                else
                {
                    throw;
                }
            }
            Logger.Log("Loaded \"{0}\" successfully.", path);
        }

        public void MemoryRainbowTest()
        {
            for (Word i = (Word)0; (int)i < MemorySize; ++i)
            {
                WriteByte((byte)i, i);
            }
        }

        private IEnumerable<string> Pair(string str)
        {
            for (int i = 0; i < str.Length; i += 2)
                yield return str.Substring(i, 2);
        }

        public ILogSink Logger
        { get; set; }
    }
}
