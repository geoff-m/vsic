using System;
using System.Collections.Generic;
using System.Linq;

namespace sicsim
{
    public class Machine
    {
        public const int SIC_MEMORY_MAXIMUM = 0x8000; // 32K
        public const int SICXE_MEMORY_MAXIMUM = 0x100000; // 1M

        public int ProgramCounter
        {
            get;
            private set;
        }

        private Word[] memory;
        Word regA, regB, regL, regS,regT, regX;

        public Machine(int memorySize = SICXE_MEMORY_MAXIMUM)
        {
            memory = new Word[memorySize];

            Word MEMORY_INITIAL_VALUE = (Word)0xffffff;
            for (int i = 0; i < memory.Length; ++i)
            {
                memory[i] = MEMORY_INITIAL_VALUE;
            }
        }

        /// <summary>
        /// Copies the given data into this Machine's memory, beginning at the specified address.
        /// Safety: This method will either perform the entire copy, or no memory will be changed.
        /// </summary>
        /// <param name="data">The data to copy.</param>
        /// <param name="address">The destination address.</param>
        public void DMAWrite(Word[] data, int address)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (address < 0)
                throw new ArgumentException("Address must be nonnegative.", nameof(address));
            if (address + data.Length > memory.Length)
                throw new ArgumentException("Write would go past end of memory.");

            Buffer.BlockCopy(data, 0, memory, address, data.Length);
        }

        /// <summary>
        /// Copies the specified number of words from this Machine's memory into a buffer.
        /// </summary>
        /// <param name="buffer">The destination array for the memory.</param>
        /// <param name="length">The maximum number of words to read.</param>
        /// <param name="address">The memory address to begin copying.</param>
        /// <returns>The number of words that were read.</returns>
        public int DMARead(Word[] buffer, int length, int address)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (length > buffer.Length)
                throw new ArgumentException("The given buffer is too small.");
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

        public RunResult Run(int address)
        {
            if (address < 0 || address >= memory.Length)
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
            // [Execute the instruction at PC.]

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
        private Word ReadWord(int address, AddressingMode mode)
        {
            if (mode == AddressingMode.Immediate)
                return (Word)address;
            //return Word.FromArray(memory, DecodeAddress(address, mode));
            return memory[DecodeAddress(address, mode)];
        }

        // Helper function for ReadWord and WriteWord.
        private int DecodeAddress(int address, AddressingMode mode)
        {
            switch (mode)
            {
                case AddressingMode.Immediate:
                    throw new ArgumentException("Addressing mode is immediate: address should not be decoded!");
                case AddressingMode.Simple: // todo: In Machine, replace this with Direct. "Simple" should be disallowed here.
                    return address;
                case AddressingMode.Indirect:
                    return address + (int)regX;
                case AddressingMode.RelativeToBase:
                    return address + (int)regB;
                case AddressingMode.RelativeToProgramCounter:
                    return address + ProgramCounter;
                default:
                    throw new ArgumentException("Illegal or unsupported addressing mode");
            }
        }

        private void WriteWord(Word w, int address, AddressingMode mode)
        {
            if (mode != AddressingMode.Immediate)
            {
                address = DecodeAddress(address, mode);
            }
            memory[address] = w;
            //address *= 3; // to convert from word index to byte index.
            //memory[address] = w.Low;
            //memory[address + 1] = w.Middle;
            //memory[address + 2] = w.High;
        }

        /// <summary>
        /// Writes a portion of this machine's memory to the console.
        /// </summary>
        /// <param name="startAddress">The address of the first word to print.</param>
        /// <param name="count">The number of words to print.</param>
        public void PrintWords(int startAddress, int count)
        {
            //startAddress *= 3;
            //count *= 3;
            int stop = startAddress + count;
            for (int wordIdx = startAddress; wordIdx < stop; wordIdx += 4) // += 12
            {
                //Console.WriteLine("0x{0:X}: \t{1,5} {2,5} {3,5} {4,5}", wordIdx,
                //    (int)Word.FromArray(memory, wordIdx),
                //    (int)Word.FromArray(memory, wordIdx + 3),
                //    (int)Word.FromArray(memory, wordIdx + 6),
                //    (int)Word.FromArray(memory, wordIdx + 9));
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
