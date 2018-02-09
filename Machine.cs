using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace sicsim
{
    public class Machine
    {
        public const int SIC_MEMORY_MAXIMUM = 0x8000; // 32K
        public const int SICXE_MEMORY_MAXIMUM = 0x100000; // 1M

        const byte MEMORY_INITIAL_VALUE = 0xff;
        readonly Word REG_INITIAL_VALUE = (Word)0xffffff;

        public Word ProgramCounter
        {
            get;
            private set;
        }

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

        public RunResult Step()
        {
            // todo: Execute the instruction at PC.

            return RunResult.None; // if no error occurs.
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
    }
}
