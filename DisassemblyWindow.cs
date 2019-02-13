using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using sicdisasm;
using SICXEAssembler;
using Word = SICXE.Word;
using System.Diagnostics;

namespace Visual_SICXE
{
    public partial class DisassemblyWindow : Form
    {
        public DisassemblyWindow()
        {
            InitializeComponent();
            disasm = new Disassembler();
            flowLines = new List<FlowLine>();
        }

        Disassembler disasm;

        /// <summary>
        /// Represents the contents of a single line in the disassembly display.
        /// Either a single instruction, or a word of data. Choosing to display a word of data (as opposed to some other quantity of bytes) is somewhat arbitrary.
        /// </summary>
        struct DisassembledUnit
        {
            public readonly Instruction Code;
            public readonly byte[] Data;
            public readonly bool IsCode;
            public DisassembledUnit(Instruction instr)
            {
                Code = instr;
                Data = null;
                IsCode = true;
            }

            public DisassembledUnit(byte[] data)
            {
                Debug.Assert(data.Length <= MAX_DATA_CHUNK_SIZE);
                Data = data;
                Code = null;
                IsCode = false;
            }

            public DisassembledUnit(byte[] data, int offset, int length)
            {
                Debug.Assert(length <= MAX_DATA_CHUNK_SIZE);
                Data = new byte[length];
                Array.Copy(data, offset, Data, 0, length);
                Code = null;
                IsCode = false;
            }

            public override string ToString()
            {
                if (IsCode)
                {
                    return Code.ToString();
                }
                else
                {
                    return $"{string.Join(" ", Data.Select(b => b.ToString("X2")))}";
                }
            }
        }

        const int MAX_DATA_CHUNK_SIZE = Word.Size;

        // For more efficient memory use, this could be replaced with a BST thing.
        DisassembledUnit[] disassembly;

        public void UpdateDisassembly(int startAddress, int stopAddress)
        {
            var m = ((MainForm)Owner)?.Machine;
            if (m == null)
                return;

            if (disassembly == null || disassembly.Length != m.MemorySize)
                disassembly = new DisassembledUnit[m.MemorySize];

            m.Memory.Seek(startAddress, System.IO.SeekOrigin.Begin);
            var res = disasm.DisassembleWithContinue(m.Memory, startAddress, stopAddress - startAddress);

            int instrIdx = 0;
            for (int addr = startAddress; addr < stopAddress;)
            {
                var instr = res.Instructions[instrIdx];
                if (instr.Address != addr)
                {
                    // The data at this address was not disassembled into a valid instruction.
                    // We want to display it anyway as data.

                    // How much data do we have until the next instruction?
                    int bytesOfData = res.Instructions[instrIdx + 1].Address.Value - addr;

                    Debug.WriteLine($"Found {bytesOfData} bytes of data at address {addr}.");
                    // Iterate through all these bytes now, adding them as chunks of data.
                    var dataRegion = new byte[bytesOfData];
                    int readRet = m.Memory.Read(dataRegion, 0, bytesOfData);
                    Debug.Assert(readRet == bytesOfData);
                    for (int dataAddr = addr; dataAddr < addr + bytesOfData; dataAddr += MAX_DATA_CHUNK_SIZE)
                    {
                        if (dataAddr + MAX_DATA_CHUNK_SIZE < addr + bytesOfData)
                        {
                            // take next MAX_DATA_CHUNK_SIZE bytes as new DisassembledUnit
                            disassembly[dataAddr] = new DisassembledUnit(dataRegion, dataAddr - addr, MAX_DATA_CHUNK_SIZE);
                        }
                        else
                        {
                            // take only as many bytes as remain as new DisassembledResult.
                            disassembly[dataAddr] = new DisassembledUnit(dataRegion, dataAddr - addr, addr + bytesOfData - dataAddr);
                        }
                    }
                }
                else
                {
                    // An instruction was disassembled at this address.
                    //addr = 
                    disassembly[addr] = new DisassembledUnit(instr);
                    Debug.WriteLine($"Found instruction {instr} at address {addr}.");
                    switch (instr.Operation)
                    {
                        case Instruction.Mnemonic.J:
                            flowLines.Add(new FlowLine(addr, instr.Operands[0].Value.Value, Color.Red));
                            break;
                        case Instruction.Mnemonic.JEQ:
                        case Instruction.Mnemonic.JLT:
                        case Instruction.Mnemonic.JGT:
                            flowLines.Add(new FlowLine(addr, instr.Operands[0].Value.Value, Color.Purple));
                            break;
                    }
                }
                rtb.AppendText($"{instr.Address} {instr}\n");

                ++instrIdx;
            }
        }

        List<FlowLine> flowLines;

        struct FlowLine
        {
            public readonly int OriginLine;
            public readonly int DestinationLine;
            public readonly Color Color;
            public FlowLine(int origin, int destination, Color color)
            {
                OriginLine = origin;
                DestinationLine = destination;
                Color = color;
            }
        }

        // todo: draw flowlines
    }
}
