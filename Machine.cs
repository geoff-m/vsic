using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace vsic
{
    public class Machine
    {
        /// <summary>
        /// The number of instructions this Machine has ever executed.
        /// </summary>
        public long InstructionsExecuted
        { get; private set; }

        /// <summary>
        /// The number of operations that have ever changed this Machine's memory.
        /// </summary>
        public long MemoryWrites
        { get; private set; }

        #region Memory and registers
        public const int SIC_MEMORY_MAXIMUM = 0x8000; // 32K
        public const int SICXE_MEMORY_MAXIMUM = 0x100000; // 1M

        const byte MEMORY_INITIAL_VALUE = 0xff;
        readonly Word REG_INITIAL_VALUE = (Word)0xffffff;

        /// <summary>
        /// A method called when the MemoryChanged event fires, indicating the Machine's memory has changed.
        /// </summary>
        /// <param name="startAddress">The first address that was changed in memory.</param>
        /// <param name="count">Size of the region (measured in bytes) beginning at "startAddress" that contains all modified addresses.</param>
        /// <param name="written">Indicates whether the specified address was written or read.</param>
        public delegate void MemoryEventHandler(Word startAddress, int count, bool written);
        public event MemoryEventHandler MemoryChanged;

        /// <summary>
        /// A method called when the RegisterChanged event fires, indicating the Machine's memory has changed.
        /// </summary>
        /// <param name="reg">The register that was changed.</param>
        /// <param name="written">Indicates whether the specified register was written or read.</param>
        public delegate void RegisterEventHandler(Register reg, bool written);
        public event RegisterEventHandler RegisterChanged;

        int PC; // Implementing this as an int saves a lot of casts in this class.
        public Word ProgramCounter
        {
            get { return (Word)PC; }
            set
            {
                int i = (int)value;
                if (i >= memory.Length || i < 0)
                {
                    throw new ArgumentException("Address is out of range.", nameof(value));
                }
                PC = i;
                RegisterChanged?.Invoke(Register.PC, true);
            }
        }

        ConditionCode CC;
        public ConditionCode ConditionCode
        {
            get { return CC; }
            set
            {
                CC = value;
                RegisterChanged?.Invoke(Register.CC, true);
            }
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
            set
            {
                regA = value;
                RegisterChanged?.Invoke(Register.A, true);
            }
        }

        public Word RegisterB
        {
            get { return regB; }
            set
            {
                regB = value;
                RegisterChanged?.Invoke(Register.B, true);
            }
        }

        public Word RegisterL
        {
            get { return regL; }
            set
            {
                regL = value;
                RegisterChanged?.Invoke(Register.L, true);
            }
        }

        public Word RegisterS
        {
            get { return regS; }
            set
            {
                regS = value;
                RegisterChanged?.Invoke(Register.S, true);
            }
        }

        public Word RegisterT
        {
            get { return regT; }
            set
            {
                regT = value;
                RegisterChanged?.Invoke(Register.T, true);
            }
        }

        public Word RegisterX
        {
            get { return regX; }
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
            PC = 0;

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
            ProgramCounter = address; // We use property here to catch out-of-range.
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
            int originalPC = PC; // Will be used to restore PC later if execution fails.
            byte b1 = memory[PC++];
            byte sextet = (byte)(b1 & 0xfc); // no opcode ends with 1, 2, or 3.
            if (Enum.IsDefined(typeof(Mnemonic), sextet))
            {
                var op = (Mnemonic)sextet;
                byte b2;
                int r1, r2;
                Word reg1value, reg2value;
                Word addr;
                AddressingMode mode;
                switch (op)
                {
                    // Arithmetic ------------------------------------------------------
                    case Mnemonic.ADD:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = regA + ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.ADDR: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        reg1value = GetRegister(r1);
                        reg2value = GetRegister(r2);
                        SetRegister(r2, reg1value + reg2value);
                        Logger.Log($"Executed {op.ToString()} {r1},{r2}.");
                        break;
                    case Mnemonic.SUB:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = regA - ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.SUBR: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        reg1value = GetRegister(r1);
                        reg2value = GetRegister(r2);
                        SetRegister(r2, reg1value + reg2value);
                        Logger.Log($"Executed {op.ToString()} {r1},{r2}.");
                        break;
                    case Mnemonic.MUL:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = (Word)(regA * ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.MULR: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        reg1value = GetRegister(r1);
                        reg2value = GetRegister(r2);
                        SetRegister(r2, (Word)(reg1value * reg2value));
                        Logger.Log($"Executed {op.ToString()} {r1},{r2}.");
                        break;
                    case Mnemonic.DIV:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = (Word)(regA / ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.DIVR: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        reg1value = GetRegister(r1);
                        reg2value = GetRegister(r2);
                        SetRegister(r2, (Word)(reg1value / reg2value));
                        Logger.Log($"Executed {op.ToString()} {r1},{r2}.");
                        break;
                    // Bitwise ---------------------------------------------------------
                    case Mnemonic.AND:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = (Word)(regA & ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.OR:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = (Word)(regA | ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.SHIFTL:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = (Word)(regA << ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.SHIFTR:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = (Word)(regA >> ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    // Registers -------------------------------------------------------
                    case Mnemonic.LDA:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterA = ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.LDB:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterB = ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.LDL:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterL = ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.LDS:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterS = ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.LDT:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterT = ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.LDX:
                        addr = DecodeLongInstruction(b1, out mode);
                        RegisterX = ReadWord(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.STA:
                        addr = DecodeLongInstruction(b1, out mode);
                        WriteWord(regA, addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.STB:
                        addr = DecodeLongInstruction(b1, out mode);
                        WriteWord(regB, addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.STL:
                        addr = DecodeLongInstruction(b1, out mode);
                        WriteWord(regL, addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.STS:
                        addr = DecodeLongInstruction(b1, out mode);
                        WriteWord(regS, addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.STT:
                        addr = DecodeLongInstruction(b1, out mode);
                        WriteWord(regT, addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.STX:
                        addr = DecodeLongInstruction(b1, out mode);
                        WriteWord(regX, addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.COMPR: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        reg1value = GetRegister(r1);
                        reg2value = GetRegister(r2);
                        ConditionCode = CompareWords(reg1value, reg2value);
                        Logger.Log($"Executed {op.ToString()} {r1},{r2}.");
                        break;
                    case Mnemonic.RMO: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        r2 = b2 & 0xf;
                        switch ((Register)r2)
                        {
                            case Register.A:
                                RegisterA = GetRegister(r1);
                                break;
                            case Register.B:
                                RegisterB = GetRegister(r1);
                                break;
                            case Register.L:
                                RegisterL = GetRegister(r1);
                                break;
                            case Register.S:
                                RegisterS = GetRegister(r1);
                                break;
                            case Register.T:
                                RegisterT = GetRegister(r1);
                                break;
                            case Register.X:
                                RegisterX = GetRegister(r1);
                                break;
                        }
                        Logger.Log($"Executed {op.ToString()} {Enum.GetName(typeof(Register), r1)},{Enum.GetName(typeof(Register), r2)}.");
                        break;
                    case Mnemonic.CLEAR: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        SetRegister(r1, Word.Zero);
                        Logger.Log($"Executed {op.ToString()} {Enum.GetName(typeof(Register), r1)}.");
                        break;
                    case Mnemonic.TIXR: // format 2
                        b2 = memory[PC++];
                        r1 = (b2 & 0xf0) >> 4;
                        // increment X, then compare it to the operand
                        ++regX;
                        ConditionCode = CompareWords(RegisterX, GetRegister(r1));
                        Logger.Log($"Executed {op.ToString()} {Enum.GetName(typeof(Register), r1)}.");
                        break;
                    // Flow control ---------------------------------------------------
                    case Mnemonic.J:
                        addr = DecodeLongInstruction(b1, out mode);
                        PC = DecodeAddress(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.JEQ:
                        addr = DecodeLongInstruction(b1, out mode);
                        if (ConditionCode == ConditionCode.EqualTo)
                            PC = DecodeAddress(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.JGT:
                        addr = DecodeLongInstruction(b1, out mode);
                        if (ConditionCode == ConditionCode.GreaterThan)
                            PC = DecodeAddress(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.JLT:
                        addr = DecodeLongInstruction(b1, out mode);
                        if (ConditionCode == ConditionCode.LessThan)
                            PC = DecodeAddress(addr, mode);
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    // Other --------------------------------------------------------
                    case Mnemonic.COMP:
                        addr = DecodeLongInstruction(b1, out mode);
                        ConditionCode = CompareWords(RegisterA, ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                    case Mnemonic.TIX:
                        // increment X, then compare it to the operand
                        addr = DecodeLongInstruction(b1, out mode);
                        ++regX;
                        ConditionCode = CompareWords(RegisterX, ReadWord(addr, mode));
                        Logger.Log($"Executed {op.ToString()} {addr.ToString()}.");
                        break;
                }
            }
            else
            {
                PC = originalPC;
                LastResult = RunResult.IllegalInstruction;
                return RunResult.IllegalInstruction;
            }

            ++InstructionsExecuted;
            LastResult = RunResult.None;
            return RunResult.None; // if no error occurs.
        }

        /// <summary>
        /// Gets the result of the last time Run or Step was called.
        /// </summary>
        public RunResult LastResult
        {
            get; private set;
        }

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

            int oldPC = PC - 1; // for error reporting.

            byte b2 = memory[PC++];
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
                // Format of standard SIC instruction (24 bits total):
                //   8      1     15
                // opcode   x   address
                if ((b2 & 0x8) == 0) // If X flag is not set.
                {
                    indirection = AddressingMode.Simple;
                    return (Word)((b2 & ~0x8) << 7 | memory[PC++]);
                }
                indirection = AddressingMode.Simple;
                return (Word)(regX + (b2 & ~0x8) << 7 | memory[PC++]);
            }
            flags |= (byte)((b2 & 0xf0) >> 4);
            int disp;
            switch (flags)
            {
                case 0b110000:   // disp
                    Debug.WriteLine("disp");
                    indirection = AddressingMode.Simple;
                    return (Word)((b2 & 0xf) << 8 | memory[PC++]);
                case 0b110001: // addr (format 4)
                    Debug.WriteLine("addr");
                    indirection = AddressingMode.Simple;
                    // Note: C# guarantees left-to-right evaluation, so stuff like this is fine.
                    return (Word)((b2 & 0xf) << 16 | memory[PC++] << 8 | memory[PC++]);
                case 0b110010: // (PC) + disp
                    // "For PC-relative addressing, [the disp] is interpreted as a 12-bit signed integer." (p. 9)
                    Debug.WriteLine("pc + disp");
                    indirection = AddressingMode.Simple;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + disp);
                case 0b110100: // (B) + disp
                    // "For base relative addressing, the displacement field disp in a Format 3 instruction is interpreted as a 12-bit unsigned integer." (p. 9)
                    Debug.WriteLine("b + disp");
                    indirection = AddressingMode.Simple;
                    return (Word)(regB + ((b2 & 0xf) << 8 | memory[PC++]));
                case 0b111000: // disp + (X)
                    Debug.WriteLine("disp + x");
                    indirection = AddressingMode.Simple;
                    return (Word)(regX + (b2 & 0xf) << 8 | memory[PC++]);
                case 0b111001: // addr + (X) (format 4)
                    Debug.WriteLine("addr + x");
                    indirection = AddressingMode.Simple;
                    return (Word)(regX + (b2 & 0xf | memory[PC++] << 8 | memory[PC++]));
                case 0b111010: // (PC) + disp + (X)
                    Debug.WriteLine("pc + disp + x");
                    indirection = AddressingMode.Simple;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + regX + disp);
                case 0b111100: // (B) + disp + (X)
                    Debug.WriteLine("b + disp + x");
                    indirection = AddressingMode.Simple;
                    return (Word)(regB + regX + (b2 & 0xf) << 8 | memory[PC++]);
                case 0b100000: // disp
                    Debug.WriteLine("disp (indirect)");
                    indirection = AddressingMode.Indirect;
                    return (Word)((b2 & 0xf) << 8 | memory[PC++]);
                case 0b100001: // addr (format 4)
                    Debug.WriteLine("addr (indirect)");
                    indirection = AddressingMode.Indirect;
                    return (Word)((b2 & 0xf) << 16 | memory[PC++] << 8 | memory[PC++]);
                case 0b100010: // (PC) + disp
                    Debug.WriteLine("pc + disp (indirect)");
                    indirection = AddressingMode.Indirect;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + disp);
                case 0b100100: // (B) + disp
                    Debug.WriteLine("b + disp (indirect)");
                    indirection = AddressingMode.Indirect;
                    return (Word)(regB + (b2 & 0xf) << 8 | memory[PC++]);
                case 0b010000: // disp
                    Debug.WriteLine("disp (immediate)");
                    indirection = AddressingMode.Immediate;
                    return (Word)DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                case 0b010001: // addr (format 4)
                    Debug.WriteLine("addr (immediate)");
                    indirection = AddressingMode.Immediate;
                    return (Word)((b2 & 0xf) << 16 | memory[PC++] << 8 | memory[PC++]);
                case 0b010010: // (PC) + disp
                    Debug.WriteLine("pc + disp (immediate)");
                    indirection = AddressingMode.Immediate;
                    disp = DecodeTwosComplement((b2 & 0xf) << 8 | memory[PC++], 12);
                    return (Word)(PC + disp);
                case 0b010100: // (B) + disp
                    Debug.WriteLine("b + disp (immediate)");
                    indirection = AddressingMode.Immediate;
                    return (Word)(regB + (b2 & 0xf) << 8 | memory[PC++]);
            }
            throw new IllegalInstructionException((Word)oldPC);
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

        // Bits higher than 'bitCount' will be cleared if input is deemed negative. I cannot imagine any such bits should be set in the first place.
        private int DecodeTwosComplement(int n, int bitCount)
        {
            int highBit = 1 << (bitCount - 1);

            int lowerMask = highBit | highBit - 1; // The bits we don't care about.

            if ((n & ~lowerMask) > 0)
                Debug.WriteLine("Warning: Higher bits are set than are supposed to be in this number!");

            if ((n & highBit) > 0) // If sign bit is set.
            {
                // Number is negative: invert and increment
                //Debug.WriteLine("It's a negative number!");
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
            switch ((Register)r)
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
            throw new ArgumentException(nameof(r));
        }

        /// <summary>
        /// Helper function to set the value of a register specified by its binart code.
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

        public enum RunResult
        {
            None = 0,
            HitBreakpoint = 1,
            IllegalInstruction = 2,
            HardwareFault = 3
        }

        private Word ReadWord(Word address, AddressingMode mode)
        {
            if (mode == AddressingMode.Immediate)
                return address;
            address = DecodeAddress(address, mode);
            MemoryChanged?.Invoke(address, 3, false);
            return Word.FromArray(memory, (int)address);
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

        private void WriteWord(Word w, Word address, AddressingMode mode)
        {
            if (mode != AddressingMode.Immediate)
            {
                address = DecodeAddress(address, mode);
            }
            int addr = (int)address;
            memory[addr] = w.High;
            memory[addr + 1] = w.Middle;
            memory[addr + 2] = w.Low;
            MemoryChanged?.Invoke(address, 3, true);
        }

        private void WriteByte(byte b, Word address)
        {
            memory[(int)address] = b;
            //Debug.WriteLine($"memory[{(int)address}] = {memory[(int)address]}");
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

        public void LoadObj(string path)
        {
            Word blockAddr = (Word)(-1); // Initialized only to silence compiler warning.
            StreamReader read = null;
            int lineCount = 1;
            int entryPoint;
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
                    Debug.Assert(line == "000000");

                    line = read.ReadLine().Trim().ToLower();
                    ++lineCount;
                    while (line != "!")
                    {
                        Debug.WriteLine($"Code/data in block {block}: {line}");
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
                PC = entryPoint;
            #if !DEBUG
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
#endif
            }
            finally
            {
                if (read != null)
                    read.Dispose();
            }
            Logger.Log("Loaded \"{0}\" successfully.", path);
        }

        /// <summary>
        /// Loads a listing file as generated by 'sicasm' present on Osprey.
        /// </summary>
        /// <param name="path"></param>
        public void LoadLst(string path)
        {
            StreamReader read = null;
            int lineCount = 0;
            var splitter = new Regex(@"\s+");
            var allDigits = new Regex(@"\d+");
            try
            {
                read = new StreamReader(path);
                string expectedLineStart;
                string line;

                while (!read.EndOfStream)
                {
                    expectedLineStart = $"{lineCount.ToString().PadLeft(3, '0')}- ";
                    line = read.ReadLine().Trim();
                    if (!line.StartsWith(expectedLineStart))
                    {
                        Logger.LogError($"Error loading \"{path}\": Expected line to {lineCount} to start with \"{expectedLineStart}\" but got \"{line}\".");
                        return; // Just abort if we don't see the line number we expect.
                    }

                    var tokens = splitter.Split(line, 6, expectedLineStart.Length - 1);

                    int addr;
                    if (!int.TryParse(tokens[1], out addr))
                    {
                        Logger.LogError($"Error loading \"{path}\": cannot parse address field on line {lineCount}: {line}");
                        return;
                    }

                    // let's assume labels cannot be entirely numbers.
                    if (allDigits.IsMatch(tokens[2]))
                    {
                        // We assume it's object code.

                    }
                    else
                    {
                        // We assume it's a label or an assembler directive.

                    }

                }

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
            finally
            {
                if (read != null)
                    read.Dispose();
            }

        }

        public void MemoryRainbowTest()
        {
            for (Word i = Word.Zero; (int)i < MemorySize; ++i)
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
