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
using System.IO;

namespace Visual_SICXE
{
    public partial class DisassemblyWindow : Form
    {
        public DisassemblyWindow()
        {
            InitializeComponent();
            disasm = new Disassembler();
            flowLines = new List<FlowLine>();
            FormClosing += OnClosing;
            rtb.ReadOnly = true;
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        Disassembler disasm;

        /// <summary>
        /// Represents the contents of a single line in the disassembly display.
        /// Either a single instruction, or a word of data. Choosing to display a word of data (as opposed to some other quantity of bytes) is somewhat arbitrary.
        /// </summary>
        struct DisassembledUnit
        {
            public int LineNumber;
            public readonly Instruction Code;
            public readonly byte[] Data;
            public readonly bool IsCode;
            public DisassembledUnit(int lineNumber, Instruction instr)
            {
                LineNumber = lineNumber;
                Code = instr;
                Data = null;
                IsCode = true;
            }

            public DisassembledUnit(int lineNumber, byte[] data)
            {
                LineNumber = lineNumber;
                Debug.Assert(data.Length <= MAX_DATA_CHUNK_SIZE);
                Data = data;
                Code = null;
                IsCode = false;
            }

            public DisassembledUnit(int lineNumber, byte[] data, int offset, int length)
            {
                LineNumber = lineNumber;
                Debug.Assert(length <= MAX_DATA_CHUNK_SIZE);
                Data = new byte[length];
                Array.Copy(data, offset, Data, 0, length);
                Code = null;
                IsCode = false;
            }

            public static readonly DisassembledUnit Empty = new DisassembledUnit();

            public override bool Equals(object obj)
            {
                if (obj is DisassembledUnit other)
                {
                    if (IsCode)
                        return Equals(Code, other.Code);
                    if (Data == null)
                        return other.Data == null;
                    if (other.Data == null)
                        return false;
                    return Data.SequenceEqual(other.Data);
                }
                return false;
            }

            public static bool operator ==(DisassembledUnit x, DisassembledUnit y)
            {
                return x.Equals(y);
            }

            public static bool operator !=(DisassembledUnit x, DisassembledUnit y)
            {
                return !x.Equals(y);
            }

            public override string ToString()
            {
                if (IsCode)
                {
                    return Code.ToString();
                }
                else
                {
                    if (Data == null)
                        return "<empty>";
                    return $"{string.Join(" ", Data.Select(b => b.ToString("X2")))}";
                }
            }
        }

        const int MAX_DATA_CHUNK_SIZE = Word.Size;

        // For more efficient memory use, this could be replaced with a BST thing.
        DisassembledUnit[] disassembly;

        public Color AddressColor = Color.Black;
        public Color DataColor = Color.DarkRed;
        public Color InstructionColor = Color.Blue;
        public Color OperandColor = Color.DarkOrange;

        public Font RegularFont => rtb.Font;
        public Font BoldFont => new Font(RegularFont, FontStyle.Bold);

        public void UpdateDisassembly(int startAddress, int stopAddress)
        {
            var m = ((MainForm)Owner)?.Machine;
            if (m == null)
                return;

            rtb.Clear();

            if (disassembly == null || disassembly.Length != m.MemorySize)
                disassembly = new DisassembledUnit[m.MemorySize];

            Debug.WriteLine($"Disassembling from {startAddress:X} to {stopAddress:X}.");
            var res = disasm.DisassembleWithContinue(m.Memory, startAddress, stopAddress - startAddress);

            int lineNumber = 0;
            int memAddr = startAddress;
            for (int disasmInstrIdx = 0; disasmInstrIdx < res.Instructions.Count; ++disasmInstrIdx)
            {
                var instr = res.Instructions[disasmInstrIdx];
                var instrAddr = instr.Address.Value;
                if (instrAddr > memAddr)
                {
                    // There was some data that failed to disassemble between this instruction and the previous one.
                    // Add it to the display as data.
                    Debug.WriteLine($"Found data at address {memAddr:X}.");
                    m.Memory.Seek(memAddr, SeekOrigin.Begin);
                    var dataChunks = ChunkStream(m.Memory, instrAddr - memAddr, Word.Size);
                    foreach (var dc in dataChunks)
                    {
                        disassembly[memAddr] = new DisassembledUnit(lineNumber++, dc);
                        var hexBytes = dc.Select(b => b.ToString("X2"));
                        rtb.SelectionColor = AddressColor;
                        rtb.AppendText($"0x{memAddr:X} ");
                        rtb.SelectionColor = DataColor;
                        rtb.AppendText($"{string.Join(" ", hexBytes)}\n");
                        memAddr += dc.Length;
                    }
                }

                disassembly[instrAddr] = new DisassembledUnit(lineNumber++, instr);
                Debug.WriteLine($"Found instruction {instr} at address {instrAddr:X}.");
                switch (instr.Operation)
                {
                    case Instruction.Mnemonic.J:
                        flowLines.Add(new FlowLine(instrAddr, instr.Operands[0].Value.Value, Color.Red));
                        break;
                    case Instruction.Mnemonic.JEQ:
                    case Instruction.Mnemonic.JLT:
                    case Instruction.Mnemonic.JGT:
                        flowLines.Add(new FlowLine(instrAddr, instr.Operands[0].Value.Value, Color.Purple));
                        break;
                }
                memAddr = instrAddr + (int)instr.Format;

                rtb.SelectionColor = AddressColor;
                rtb.AppendText($"0x{instrAddr:X} ");
                rtb.SelectionColor = InstructionColor;
                rtb.SelectionFont = BoldFont;
                rtb.AppendText(instr.Operation.ToString());
                if (instr.Operands.Count > 0)
                {
                    rtb.SelectionColor = OperandColor;
                    rtb.AppendText(" " + string.Join(",", instr.Operands));
                }
                rtb.SelectionFont = RegularFont;
                rtb.AppendText("\n");
            }

        }

        public void ScrollToAddress(int address)
        {
            // Search for address in disassembly.
            // Walk back until you find the DU that isn't empty.
            DisassembledUnit du = DisassembledUnit.Empty;
            for (; address >= 0; --address)
            {
                du = disassembly[address];
                if (du != DisassembledUnit.Empty)
                    break;
            }

            // 'address' is now the index of the beginning of a disassembled unit.

            // Now we must map this to character index in rtb.

            //// Back up existing selection.
            //int selStart = rtb.SelectionStart;
            //int selLength = rtb.SelectionLength;

            int scrollLine = (int)(du.LineNumber - CountTextLinesInRTB() * 0.3);
            if (scrollLine < 0)
                scrollLine = 0;
            rtb.SelectionStart = rtb.GetFirstCharIndexFromLine(scrollLine);
            rtb.ScrollToCaret();


            int duStartChar = rtb.GetFirstCharIndexFromLine(du.LineNumber);
            //rtb.SelectionStart = duStartChar;
            //rtb.ScrollToCaret();

            rtb.SelectionStart = duStartChar + 3 + address.ToString("x").Length;

            //rtb.SelectionStart = selStart;
            //rtb.SelectionLength = selLength;
        }

        /// <summary>
        /// Computes the number of lines of text that can be displayed in the RTB on screen at the same time.
        /// </summary>
        private int CountTextLinesInRTB()
        {
            var lineHeight = TextRenderer.MeasureText("M", rtb.Font).Height;
            return Math.Max(1, rtb.ClientSize.Height / lineHeight);
        }

        /// <summary>
        /// Groups a Stream's bytes into arrays of a given size.
        /// </summary>
        /// <param name="s">The stream to be read.</param>
        /// <param name="length">The maximum number of bytes to read.</param>
        /// <param name="chunkSize">The size of each array that will be returned.</param>
        /// <returns>A list of byte arrays of length given by 'chunkSize'. The total number of bytes over all arrays will not exceed 'length'.</returns>
        private static IList<byte[]> ChunkStream(Stream s, int length, int chunkSize)
        {
            var ret = new List<byte[]>();
            Func<Stream, bool> streamNotDone = (stream) => stream.Position < stream.Length;
            for (int totalRead = 0; totalRead < length && streamNotDone(s); totalRead += chunkSize)
            {
                // Read a chunk.
                var chunk = new byte[chunkSize];
                int chunkRead = 0;
                do
                {
                    int justRead = s.Read(chunk, 0, chunkSize - chunkRead);
                    if (justRead <= 0)
                        break; // Reached end of stream.
                    chunkRead += justRead;
                } while (chunkRead < chunkSize && streamNotDone(s));
                ret.Add(chunk);
            }
            return ret;
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
