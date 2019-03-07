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
using Visual_SICXE.Extensions;

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
            flPanel.DoubleBuffer(true);
            flPanel.Paint += DrawFlowLines;
            rtb.VScroll += OnVerticalScroll;
        }

        private void OnVerticalScroll(object sender, EventArgs e)
        {
            UpdateHorizontalOffsets();
            flPanel.Invalidate();
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
            const int MAX_DATA_CHUNK_SIZE = Word.Size;
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

        private DisassembledUnit FindDisassembledUnit(int addr)
        {
            DisassembledUnit guess = DisassembledUnit.Empty;
            for (; addr > 0 && guess == DisassembledUnit.Empty; --addr)
            {
                guess = disassembly[addr];
            }
            return guess;
        }

        // For more efficient memory use, this could be replaced with a BST thing.
        DisassembledUnit[] disassembly;
        int disassemblyStartAddress;
        int disassemblyEndAddress;

        public void UpdateDisassembly(int startAddress, int endAddress)
        {
            var m = ((MainForm)Owner)?.Machine;
            if (m == null)
                return;

            if (disassembly == null || disassembly.Length != m.MemorySize)
                disassembly = new DisassembledUnit[m.MemorySize];
            else
                Array.Clear(disassembly, 0, disassembly.Length);
            flowLines.Clear();
            disassemblyStartAddress = startAddress;
            disassemblyEndAddress = endAddress;

            Debug.WriteLine($"Disassembling from {startAddress:X} to {endAddress:X}.");
            var res = disasm.DisassembleWithContinue(m.Memory, startAddress, endAddress - startAddress);
            //var res = disasm.DisassembleReachable(m.Memory, startAddress, endAddress - startAddress);

            int lineNumber = 0;
            int memAddr = startAddress;
            DisassembledUnit du;
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
                        memAddr += dc.Length;
                    }
                }

                disassembly[instrAddr] = du = new DisassembledUnit(lineNumber++, instr);
                //Debug.WriteLine($"Found instruction {instr} at address {instrAddr:X}.");
                int jumpTarget;
                switch (instr.Operation)
                {
                    case Instruction.Mnemonic.J:
                    case Instruction.Mnemonic.JSUB:
                        // only add flow line if we can account for addressing mode.
                        if (Disassembler.TryDisassembleAbsoluteJumpTarget(instr, out jumpTarget))
                        {
                            flowLines.Add(new FlowLine(du.LineNumber, jumpTarget, Color.Red, du));
                        }
                        break;
                    case Instruction.Mnemonic.JEQ:
                    case Instruction.Mnemonic.JLT:
                    case Instruction.Mnemonic.JGT:
                        if (Disassembler.TryDisassembleAbsoluteJumpTarget(instr, out jumpTarget))
                        {
                            flowLines.Add(new FlowLine(du.LineNumber, jumpTarget, Color.Purple, du));
                        }
                        break;
                }
                memAddr = instrAddr + (int)instr.Format;
            }
            WriteDisassemblyText();


            Debug.WriteLine("Disassembly update success.");
        }

        public Color AddressColor = Color.Black;
        public Color DataColor = Color.DarkRed;
        public Color InstructionColor = Color.Blue;
        public Color OperandColor = Color.DarkOrange;

        private void WriteDisassemblyText()
        {
            rtb.SuspendDrawing();
            rtb.Clear();
            int addrPadLength = disassemblyEndAddress.ToString("x").Length;
            int opPadLength = 6;
            var regularFont = rtb.Font;
            var boldFont = new Font(regularFont, FontStyle.Bold);
            for (int duAddr = disassemblyStartAddress; duAddr < disassemblyEndAddress; ++duAddr)
            {
                var du = disassembly[duAddr];
                if (du == DisassembledUnit.Empty)
                    continue;
                var addr = duAddr;
                if (du.IsCode)
                {
                    var instr = du.Code;
                    Debug.Assert(instr.Address == addr);
                    rtb.SelectionColor = AddressColor;
                    rtb.AppendText($"0x{addr.ToString("X").PadRight(addrPadLength)} ");
                    rtb.SelectionColor = InstructionColor;
                    rtb.SelectionFont = boldFont;
                    bool operandPositive = true;
                    int fmt34operand = 0;
                    var opcodeStr = instr.Operation.ToString();
                    Instruction.Flag flags = 0;
                    if (instr.Flags.HasValue)
                    {
                        flags = instr.Flags.Value;
                        if (instr.Format == InstructionFormat.Format4)
                        {
                            opcodeStr = "+" + opcodeStr;
                            if (instr.Operands.Count == 1)
                                fmt34operand = Instruction.Decode20BitTwosComplement(instr.Operands[0].Value.Value, out operandPositive);
                        }
                        else if (instr.Format == InstructionFormat.Format3)
                        {
                            if (instr.Operands.Count == 1)
                                fmt34operand = Instruction.Decode12BitTwosComplement(instr.Operands[0].Value.Value, out operandPositive);
                        }
                    }

                    rtb.AppendText(opcodeStr.PadRight(opPadLength));
                    if ((instr.Format == InstructionFormat.Format3 || instr.Format == InstructionFormat.Format4)
                        && instr.Operands.Count == 1
                        && instr.Flags.HasValue)
                    {
                        rtb.SelectionColor = OperandColor;
                        var indirStr = flags.HasFlag(Instruction.Flag.N) && !flags.HasFlag(Instruction.Flag.I) ? "@" : "";
                        var immStr = !flags.HasFlag(Instruction.Flag.N) && flags.HasFlag(Instruction.Flag.I) ? "#" : "";
                        bool printed = false;
                        if (flags.HasFlag(Instruction.Flag.P))
                        {
                            if (operandPositive)
                                rtb.AppendText($" P+{indirStr}{immStr}{fmt34operand}");
                            else
                                rtb.AppendText($" P-{indirStr}{immStr}{fmt34operand}");
                            printed = true;
                        }
                        else if (flags.HasFlag(Instruction.Flag.B))
                        {
                            if (operandPositive)
                                rtb.AppendText($" B+{indirStr}{immStr}{fmt34operand}");
                            else
                                rtb.AppendText($" B-{indirStr}{immStr}{fmt34operand}");
                            printed = true;
                        }
                        if (!printed)
                        {
                            rtb.AppendText($" {indirStr}{immStr}{fmt34operand}");
                        }
                        if (flags.HasFlag(Instruction.Flag.X))
                            rtb.AppendText(",X");
                    }
                    else
                    {
                        if (instr.Operands.Count > 1)
                        {
                            rtb.SelectionColor = OperandColor;
                            if (instr.Operation == Instruction.Mnemonic.SHIFTL || instr.Operation == Instruction.Mnemonic.SHIFTR)
                            {
                                rtb.AppendText($" {instr.Operands[0]},{instr.Operands[1].Value.Value}");
                            }
                            else
                            {
                                rtb.AppendText(" " + string.Join(",", instr.Operands));
                            }
                        }
                    }
                    rtb.SelectionFont = regularFont;
                    rtb.AppendText("\n");
                }
                else
                {
                    var dc = du.Data;
                    var hexBytes = dc.Select(b => b.ToString("X2"));
                    rtb.SelectionColor = AddressColor;
                    rtb.AppendText($"0x{addr.ToString("X").PadRight(addrPadLength)} ");
                    rtb.SelectionColor = DataColor;
                    rtb.AppendText($"{string.Join(" ", hexBytes)}\n");
                }
            }
            rtb.ResumeDrawing();
        }

        // Updates each FlowLine with its destination line number.
        // This should not be called before all lines have been added to the RTB.
        private void UpdateHorizontalOffsets()
        {
            //Debug.WriteLine("\nUpdateHorizontalOffsets()");
            // Get top line in RTB.
            var topCharIdx = rtb.GetCharIndexFromPosition(new Point(0, 0));
            var topLine = rtb.GetLineFromCharIndex(topCharIdx);
            var bottomCharIdx = rtb.GetCharIndexFromPosition(new Point(0, rtb.Bottom));
            var bottomLine = rtb.GetLineFromCharIndex(bottomCharIdx);

            var intervalTracker = new IntervalTracker<FlowLine>(0, rtb.Lines.Length);
            // Get the FlowLines that begin or end on the screen.
            var beginOrEndOnScreen = new HashSet<FlowLine>();
            for (int i = 0; i < flowLines.Count; ++i)
            {
                var fl = flowLines[i];
                if (fl.DestinationLine == 0)
                    fl.DestinationLine = FindDisassembledUnit(fl.DestinationAddress).LineNumber; // Update line number.

                intervalTracker.Increment(fl.OriginLine, fl.DestinationLine, fl); // Register my appearance on the lines that I span.
                fl.BeginsOrEndsOnScreen = false;
                if ((fl.OriginLine >= topLine && fl.OriginLine <= bottomLine)
                    || (fl.DestinationLine >= topLine && fl.DestinationLine <= bottomLine))
                {
                    beginOrEndOnScreen.Add(fl);
                    fl.BeginsOrEndsOnScreen = true;
                }
            }

            int ho = 0;
            var onScreen = beginOrEndOnScreen.ToList();
            for (int i = 0; i < onScreen.Count; ++i)
            {
                var fl = onScreen[i];
                fl.HorizontalOffset = 5 + (ho++) * 5;
                //Debug.WriteLine($"FL {i}/{beginOrEndOnScreen.Count} on screen: ho = {fl.HorizontalOffset}");
            }
        }

        private void DrawFlowLines(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            // Proof of concept for drawing something next to a line in the RTB.
            //int addrWithFlow = 0x5;
            //var duWithAddr = FindDisassembledUnit(addrWithFlow);
            //int linewithAddr = duWithAddr.LineNumber;
            //var lineCharIndex = rtb.GetFirstCharIndexFromLine(linewithAddr);
            //var charPoint = rtb.GetPositionFromCharIndex(lineCharIndex);
            //charPoint.Offset(0, RTBLineHeight / 2);
            //var lineStartPoint = charPoint;
            //var lineEndPoint = lineStartPoint;
            //lineEndPoint.Offset(20, 0);
            //g.DrawLine(Pens.Red, charPoint, lineEndPoint);

            // Only push things away if they would actually overlap.


            for (int i = 0; i < flowLines.Count; ++i)
            {
                var fl = flowLines[i];
                if (!fl.BeginsOrEndsOnScreen)
                    continue;
                DrawFlowLine(g, fl);
            }
        }

        private int GetAddressY(int address)
        {
            var du = FindDisassembledUnit(address);
            if (du == DisassembledUnit.Empty)
                return 0;

            var lineCharIndex = rtb.GetFirstCharIndexFromLine(du.LineNumber);
            var charPoint = rtb.GetPositionFromCharIndex(lineCharIndex);
            return charPoint.Y + RTBLineHeight / 2;
        }

        private int GetLineY(int lineNumber)
        {
            var lineCharIndex = rtb.GetFirstCharIndexFromLine(lineNumber);
            var charPoint = rtb.GetPositionFromCharIndex(lineCharIndex);
            return charPoint.Y + RTBLineHeight / 2;
        }

        private void DrawFlowLine(Graphics g, FlowLine fl)
        {
            Debug.Assert(fl.HorizontalOffset != 0);
            DrawDestinationArrow(g, fl);
            DrawOrigin(g, fl);
            DrawFlowLineVertical(g, fl);
        }

        private void DrawDestinationArrow(Graphics g, FlowLine fl)
        {
            int lineY = GetAddressY(fl.DestinationAddress);
            var lineLeftPoint = new Point(flPanel.Right - fl.HorizontalOffset, lineY);
            var lineRightPoint = new Point(flPanel.Right, lineY);
            g.DrawLine(new Pen(fl.Color), lineLeftPoint, lineRightPoint);

            var tri = new Point[]
            {
                new Point(lineRightPoint.X - 3, lineRightPoint.Y - 3),
                new Point(lineRightPoint.X, lineRightPoint.Y),
                new Point(lineRightPoint.X - 3, lineRightPoint.Y + 3)
            };
            g.FillPolygon(new SolidBrush(fl.Color), tri);
        }

        private void DrawOrigin(Graphics g, FlowLine fl)
        {
            int lineY = GetLineY(fl.OriginLine);
            var lineLeftPoint = new Point(flPanel.Right - fl.HorizontalOffset, lineY);
            var lineRightPoint = new Point(flPanel.Right - 3, lineY);

            var pen = new Pen(fl.Color);
            g.DrawLine(pen, lineLeftPoint, lineRightPoint);
        }

        private void DrawFlowLineVertical(Graphics g, FlowLine fl)
        {
            int originY = GetLineY(fl.OriginLine);
            var originEnd = new Point(flPanel.Right - fl.HorizontalOffset, originY);
            int destY = GetAddressY(fl.DestinationAddress);
            var destEnd = new Point(flPanel.Right - fl.HorizontalOffset, destY);
            g.DrawLine(new Pen(fl.Color), originEnd, destEnd);
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
            return Math.Max(1, rtb.ClientSize.Height / RTBLineHeight);
        }

        private int RTBLineHeight => TextRenderer.MeasureText("M", rtb.Font).Height;

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

        class FlowLine
        {
            public readonly int OriginLine;
            public readonly int DestinationAddress;
            public int DestinationLine; // Set later by creator.
            public int HorizontalOffset; // Set later by creator.
            public readonly Color Color;
            public bool BeginsOrEndsOnScreen;
            public FlowLine(int originLine, int destinationAddress, Color color, DisassembledUnit? origin = null)
            {
                OriginLine = originLine;
                DestinationAddress = destinationAddress;
                DestinationLine = 0;
                Color = color;
                Origin = origin;
                HorizontalOffset = 0;
            }

            public DisassembledUnit? Origin; // For debug.

        }

        private class IntervalTracker<T>
        {
            //int[] data;
            List<T>[] data;
            public readonly int Minimum, Maximum;
            public IntervalTracker(int minimum, int maximum)
            {
                Minimum = minimum;
                Maximum = maximum;
                //data = new int[maximum - minimum];
                data = new List<T>[maximum - minimum];
            }

            public void Increment(int start, int end, T cause)
            {
                Sort(ref start, ref end);
                for (int i = start; i < end; ++i)
                {
                    //++data[i - Minimum];
                    var d = data[i - Minimum];
                    if (d == null)
                        d = data[i - Minimum] = new List<T>(4);
                    d.Add(cause);
                }
            }

            public void Decrement(int start, int end, T cause)
            {
                Sort(ref start, ref end);
                for (int i = start; i < end; ++i)
                {
                    //++data[i - Minimum];
                    var d = data[i - Minimum];
                    if (d == null)
                        d = data[i - Minimum] = new List<T>(4);
                    var removeOk = d.Remove(cause);
                    Debug.Assert(removeOk);
                }
            }


            public void Clear()
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    var d = data[i - Minimum];
                    if (d != null)
                        d.Clear();
                }
                Array.Clear(data, 0, data.Length);
            }

            public IList<T> this[int i]
            {
                get
                {
                    return data[i - Minimum];
                }
            }

            /// <summary>
            /// Gets a set containing all the items that occurr at least once in the specified range.
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <returns></returns>
            public ISet<T> GetInRange(int start, int end)
            {
                Sort(ref start, ref end);
                var ret = new HashSet<T>();
                for (int i = start; i < end; ++i)
                {
                    var t = data[i - Minimum];
                    if (t != null)
                        ret.UnionWith(t);
                }
                return ret;
            }

            /// <summary>
            /// Gets the maximum number of items that occur at any single index in the specified range.
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <returns></returns>
            public int GetMaximum(int start, int end)
            {
                Sort(ref start, ref end);
                int ret = 0;
                for (int i = start; i < end; ++i)
                {
                    var t = data[i - Minimum]?.Count ?? 0;
                    if (t > ret)
                        ret = t;
                }
                return ret;
            }
        }

        static void Sort(ref int x, ref int y)
        {
            if (x > y)
            {
                int t = x;
                x = y;
                y = t;
            }
        }
    }
}
