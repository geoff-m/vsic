using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SICXE;
using System.Diagnostics;

namespace Visual_SICXE
{
    public partial class HexDisplay : UserControl
    {
        public enum Encoding
        {
            Raw = 0,
            DecimalSigned = 1,
            DecimalUnsigned = 2,
            UTF8 = 32
        }

        public HexDisplay()
        {
            InitializeComponent();
            Font = new Font(FontFamily.GenericMonospace, 12);
            DoubleBuffered = true;
            Enter += OnFocus;
            Leave += OnBlur;
            EnabledChanged += (sender, args) => Invalidate();
        }

        public HashSet<ByteMarker> Boxes = new HashSet<ByteMarker>();

        public Stream Data
        { get; set; }

        public Encoding WordEncoding { get; set; } = Encoding.Raw;

        #region Format properties
        private int WordBytes => WordDigits / 2;

        private string wordFormatString = "X6";
        private int wordDigits = 6;
        public int WordDigits
        {
            get
            {
                return wordDigits;
            }
            set
            {
                wordDigits = value;
                wordFormatString = 'X' + wordDigits.ToString();
                doRecalc = true;
            }
        }

        private string addressFormatString = "X6";
        private int addressDigits = 6;
        public int AddressDigits
        {
            get { return addressDigits; }
            set
            {
                addressDigits = value;
                addressFormatString = 'X' + addressDigits.ToString();
                doRecalc = true;
            }
        }

        private int startAddress = 0;
        /// <summary>
        /// Represents the address of the lowest byte that will be displayed.
        /// </summary>
        public int StartAddress
        {
            get
            {
                return startAddress;
            }
            set
            {
                startAddress = value;
                doRecalc = true;
                //Debug.WriteLine($"StartAddress has been set to {startAddress.ToString("X")}.");
                // redraw
                Invalidate(); // todo: remove redundant calls to this. i think at least one caller to this setter is calling Invalidate after.
            }
        }

        private int cursorAddress = 0;
        public int CursorAddress
        {
            get
            {
                return cursorAddress;
            }
            set
            {
                if ((Data != null && value >= Data.Length) || value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                // If address is not contained in display, update displayed window to contain it.
                cursorAddress = value;

                if (bytesPerLine > 0)
                {

                    int screenBytes = bytesPerLine * lineCount;
                    int stopAddress = startAddress + screenBytes;
                    int cursorLine = cursorAddress / bytesPerLine;
                    //Debug.WriteLine($"Cursor is on line {cursorLine + 1} of {lineCount}.");
                    //Debug.WriteLine($"StopAddress: {stopAddress.ToString("X")}");

                    // If this address is after the end of the window, move the window so that the cursor appears on the last line.
                    if (cursorAddress > stopAddress)
                    {
                        StartAddress = Math.Max(0, bytesPerLine * (cursorLine - lineCount + 1));
                    }

                    // If this adderess is before the start of the window, move the window so that the cursor appears on the first line.
                    if (cursorAddress < startAddress)
                    {
                        StartAddress = Math.Max(0, bytesPerLine * (cursorLine));
                    }
                }

                CursorAddressChanged?.Invoke(this, null);
            }
        }

        public event EventHandler CursorAddressChanged;

        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                doRecalc = true;
                Invalidate();
            }
        }
        #endregion

        #region Cursor Movement Mutators
        /// <summary>
        /// Moves the cursor to the beginning of the line it's on.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorHome()
        {
            int diff = CursorAddress % bytesPerLine;
            if (diff == 0)
                return false; // We are already at the beginning of the line.

            CursorAddress -= diff;
            return true;
        }

        /// <summary>
        /// Moves the cursor to the end of the line it's on.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorEnd()
        {
            int diff = bytesPerLine - CursorAddress % bytesPerLine - 1;
            if (diff == 0)
                return false; // We are already at the end of the line.

            CursorAddress += diff;
            return true;
        }

        /// <summary>
        /// Moves the cursor up one line.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorUp()
        {
            int newaddr = CursorAddress - bytesPerLine;
            if (newaddr < 0)
            {
                // Do nothing if moving up would put us before the beginning.
                return false;
            }
            CursorAddress = newaddr;
            return true;
        }

        /// <summary>
        /// Moves the cursor down one line.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorDown()
        {
            int newaddr = CursorAddress + bytesPerLine;
            if (newaddr >= Data.Length)
            {
                // Do nothing if moving up would put us after the end.
                return false;
            }
            CursorAddress = newaddr;
            return true;
        }

        /// <summary>
        /// Moves the cursor left one byte.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorLeft()
        {
            int newaddr = CursorAddress - 1;
            if (newaddr < 0)
            {
                // Do nothing if moving left would put us before the beginning.
                return false;
            }
            CursorAddress = newaddr;
            return true;
        }

        /// <summary>
        /// Moves the cursor right one byte.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorRight()
        {
            int newaddr = CursorAddress + 1;
            if (newaddr >= Data.Length)
            {
                // Do nothing if moving right would put us after the end.
                return false;
            }
            CursorAddress = newaddr;
            return true;
        }

        /// <summary>
        /// Represents the proportion of visible lines that the cursor moves when MoveCursorUpFar and MoveCursorDownFar are called.
        /// </summary>
        private const float PAGE_SIZE_RATIO = 0.67f;

        /// <summary>
        /// Moves the cursor up two thirds of the screen in distance.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorUpFar()
        {
            int newaddr = CursorAddress - bytesPerLine * (int)Math.Floor(lineCount * PAGE_SIZE_RATIO);
            if (newaddr < 0)
            {
                // If paging up would put us before the beginning, just make sure we're scrolled to the top line.
                if (StartAddress > 0)
                {
                    StartAddress = 0;
                    return true;
                }
                return false;
            }
            CursorAddress = newaddr;
            return true;
        }

        /// <summary>
        /// Moves the cursor down two thirds of the screen in distance.
        /// </summary>
        /// <returns>A Boolean value indicating whether the cursor position was changed.</returns>
        public bool MoveCursorDownFar()
        {
            int newaddr = CursorAddress + bytesPerLine * (int)Math.Floor(lineCount * PAGE_SIZE_RATIO);
            if (newaddr >= Data.Length)
            {
                // If paging down would put us after the end, just make sure we're scrolled to the bottom line.
                int max = (int)(Data.Length - bytesPerLine * lineCount);
                if (StartAddress < max)
                {
                    StartAddress = max;
                    return true;
                }
                return false;
            }
            CursorAddress = newaddr;
            return true;
        }

        #endregion

        private const float textYoffset = 0; // the Y offset for both the address and the data.
        private const float addressX = 4; // distance between left column and left of address.

        private const float addressDataGap = 5; // distance between right of address and left of data.

        private const float dataXendPadding = 4; // distance between right of data and right column.
        private const float wordXgap = 4; // distance between each successive word of data.

        private float wordWidth;
        private float byteWidth;
        private float dataXoffset; // computed distance between left column and left of leftmost address.
        private int wordsPerLine; // computed number of words to display per line.
        private int bytesPerLine; // computed number of bytes that are on each line.
        private int lineCount; // computed number of lines to display.
        private float textLineSpacing; // computed distance between lines.
        private float lineHeight; // computed height of a line of text.

        private bool doRecalc = true;
        #region Painting
        /// <summary>
        /// Recalculates the parameters needed to draw this HexDisplay.
        /// </summary>
        /// <param name="g">The graphics object to be used for measurements.</param>
        private void UpdateMeasurements(Graphics g)
        {
            var BOUNDS = g.VisibleClipBounds;
            var addressSize = g.MeasureString(new string('F', addressDigits), Font);
            float addrH = addressSize.Height;
            float addrW = addressSize.Width;
            textLineSpacing = 0.05f * addrH;

            lineCount = (int)((BOUNDS.Height - textYoffset) / (addrH + textLineSpacing));
            lineHeight = addrH;

            dataXoffset = addressX + addrW + addressDataGap;

            wordWidth = g.MeasureString(new string('F', WordDigits), Font).Width;
            byteWidth = wordWidth / (WordDigits / 2f); // todo: get correct value for byteWidth. i think it is being overestimated.

            // BOUNDS.Width - (addressX + addrW + addressDataGap) = dataLine.Width + dataXendPadding
            // BOUNDS.Width - (addressX + addrW + addressDataGap) = wordsPerLine * wordW+ (wordsPerLine - 1) * wordXgap + dataXendPadding
            // BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding = wordsPerLine * wordW + wordsPerLine * wordXgap -  wordXgap
            // BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding = wordsPerLine * (wordW + wordXgap) - wordXgap
            // BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding + wordXgap = wordsPerLine * (wordW + wordXgap)
            // wordsPerLine = (BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding + wordXgap) / (wordW + wordXgap)
            wordsPerLine = (int)((BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding + wordXgap) / (wordWidth + wordXgap));
            if (wordsPerLine < 0)
                wordsPerLine = 0;
            bytesPerLine = wordsPerLine * WordDigits / 2; // div by 2 because each digit is half a byte.

            doRecalc = false;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            if (Data == null)
            {
                PaintCenteredText(g, "No Data");
                return;
            }
            else if (!Data.CanRead)
            {
                PaintCenteredText(g, "Cannot read data");
                return;
            }

            if (doRecalc)
                UpdateMeasurements(g);

            if (wordsPerLine == 0)
                return;

            if (selectionStartAddress != selectionStopAddress)
            {
                int highlightStart, highlightStop;
                if (selectionStartAddress < selectionStopAddress)
                {
                    highlightStart = selectionStartAddress;
                    highlightStop = selectionStopAddress;
                }
                else
                {
                    highlightStart = selectionStopAddress;
                    highlightStop = selectionStartAddress;
                }
                DrawHighlight(g, highlightStart, highlightStop, new SolidBrush(Color.FromArgb(80, 0, 100, 120)));
            }


            // Draw boxes.
            int boxesDrawn = 0; // for debug.
            foreach (var box in Boxes)
            {
                if (DrawBox(g, box))
                    ++boxesDrawn;
            }

            // Draw addresses and data.
            //int bytesPerLine = wordsPerLine * 3;
            int lineAddress = startAddress;
            var lineBytes = new byte[bytesPerLine];
            Data.Position = StartAddress; // Reset the stream to the beginning of the window.
            for (int line = 0; line < lineCount; ++line)
            {
                float y = textYoffset + line * (textLineSpacing + lineHeight); // caching the Y offset of this line.

                // Draw address.
                g.DrawString(lineAddress.ToString(addressFormatString), Font, Brushes.DarkSlateGray, addressX, y);

                // FOR DEBUG
                //g.DrawString($"{line}of{lineCount}", Font, Brushes.DarkSlateGray, addressX, y);
                //var strSz = g.MeasureString(lineAddress.ToString(addressFormatString), Font);
                //g.DrawRectangle(Pens.Red, addressX, y, strSz.Width, strSz.Height);

                // Draw data, depending on selected encoding.
                int bytesRead = Data.Read(lineBytes, 0, bytesPerLine);

                switch (WordEncoding)
                {
                    case Encoding.Raw:
                        var lineWords = Word.FromArray(lineBytes, 0, bytesRead);
                        int lineWordCount = lineWords.Length; // The actual number of words we have to display on this line.
                        if (lineWordCount == 0)
                            continue;

                        g.DrawString(lineWords[0].ToString(wordFormatString), Font, Brushes.Black, dataXoffset, y);
                        for (int wordIdx = 1; wordIdx < lineWordCount; ++wordIdx)
                        {
                            float x = dataXoffset + wordIdx * (wordWidth + wordXgap);
                            g.DrawString(lineWords[wordIdx].ToString(wordFormatString), Font, Brushes.Black, x, y);
                        }
                        break;

                    case Encoding.UTF8:
                        var str = DecodeAsUTF8(lineBytes, 0, 3);
                        float charGap = wordWidth / 3; // This works well enough in practice.
                        if (str.Length >= 1)
                            g.DrawString(str.Substring(0, 1), Font, Brushes.Black, dataXoffset, y);
                        if (str.Length >= 2)
                            g.DrawString(str.Substring(1, 1), Font, Brushes.Black, dataXoffset + charGap, y);
                        if (str.Length >= 3)
                            g.DrawString(str.Substring(2, 1), Font, Brushes.Black, dataXoffset + charGap + charGap, y);
                        for (int wordIdx = 1; wordIdx < Math.Ceiling(bytesRead / 3d); ++wordIdx)
                        {
                            str = DecodeAsUTF8(lineBytes, wordIdx * 3, 3);
                            if (str.Length >= 1)
                                g.DrawString(str.Substring(0, 1), Font, Brushes.Black, dataXoffset + wordIdx * (wordWidth + wordXgap), y);
                            if (str.Length >= 2)
                                g.DrawString(str.Substring(1, 1), Font, Brushes.Black, dataXoffset + wordIdx * (wordWidth + wordXgap) + charGap, y);
                            if (str.Length >= 3)
                                g.DrawString(str.Substring(2, 1), Font, Brushes.Black, dataXoffset + wordIdx * (wordWidth + wordXgap) + charGap + charGap, y);
                        }
                        break;
                }

                lineAddress += bytesPerLine;
            }
            // Draw cursor.
            DrawCursor(g);

            if (!Enabled)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(128, SystemColors.Control)), g.VisibleClipBounds);
                PaintCenteredText(g, "Running...", 18);
            }
        }

        private bool DrawHighlight(Graphics g, int startAddress, int stopAddress, Brush b)
        {
            if (stopAddress < startAddress)
                throw new ArgumentException("Start address must not exceed stop address.");

            int startScreenByte = startAddress - StartAddress;
            int startScreenLine, firstLineStartByte;
            if (startScreenByte < 0)
            {
                startScreenLine = 0;
                firstLineStartByte = 0;
            }
            else
            {
                startScreenLine = startScreenByte / bytesPerLine;
                firstLineStartByte = startScreenByte % bytesPerLine;
            }

            int stopScreenByte = stopAddress - StartAddress;
            int stopScreenLine = stopScreenByte / bytesPerLine;
            if (stopScreenLine > lineCount)
            {
                stopScreenByte = bytesPerLine;
                stopScreenLine = lineCount;
            }
            else if (stopScreenLine < 0)
            {
                return false;
            }
            int highlightLineCount = stopScreenLine - startScreenLine;
            //Parent.Text = $"start line: {startScreenLine}   stop line: {stopScreenLine}";
            // Draw first line.
            int firstLineEndByte;
            if (highlightLineCount > 0)
            {
                firstLineEndByte = bytesPerLine;
            }
            else
            {
                firstLineEndByte = stopScreenByte % bytesPerLine;
            }

            int bytesHighlightedOnFirstLine = firstLineEndByte - firstLineStartByte;
            float firstLineXStart = firstLineStartByte * byteWidth + firstLineStartByte / WordBytes * wordXgap;
            Debug.Assert(firstLineXStart >= 0);
            g.FillRectangle(b,
                            dataXoffset + firstLineXStart,
                            textYoffset + startScreenLine * (textLineSpacing + lineHeight),
                            bytesHighlightedOnFirstLine * byteWidth + bytesHighlightedOnFirstLine / WordBytes * wordXgap,
                            lineHeight
                            );

            // Draw middle lines as one block.
            if (highlightLineCount >= 2)
            {
                g.FillRectangle(b,
                                dataXoffset,
                                textYoffset + lineHeight * (startScreenLine + 1) + textLineSpacing * startScreenLine,
                                dataXoffset + wordWidth * wordsPerLine + wordXgap * (wordsPerLine - 1),
                                lineHeight * (stopScreenLine - startScreenLine - 1) + (stopScreenLine - startScreenLine - 2) * textLineSpacing
                                );
            }

            // Draw last line.
            if (highlightLineCount >= 1)
            {
                int lastLineStopByte = stopScreenByte % bytesPerLine;
                g.FillRectangle(b,
                                dataXoffset,
                                textYoffset + stopScreenLine * lineHeight + (stopScreenLine - 1) * textLineSpacing,
                                lastLineStopByte * byteWidth + lastLineStopByte / WordBytes * wordXgap,
                                lineHeight
                                );
            }
            return true;
        }

        private bool DrawBox(Graphics g, ByteMarker box)
        {
            int addr = box.Address;
            int screenByte = addr - StartAddress;
            int screenByteCount = lineCount * wordsPerLine * AddressDigits;
            if (screenByte < 0 || screenByte > screenByteCount)
                return false; // Ignore if address is not on screen.

            int line = screenByte / bytesPerLine; // Which line should the box appear on?
            int lineByte = screenByte % bytesPerLine;
            int lineWord = lineByte / WordBytes;
            int wordByte = addr % WordBytes;

            float boxX = dataXoffset + lineWord * (wordWidth + wordXgap) + wordByte * (byteWidth);

            if (box.Hollow)
            {
                g.DrawRectangle(box.Pen,
                                   boxX,
                                   textYoffset + line * (textLineSpacing + lineHeight),
                                   byteWidth - 1,
                                   lineHeight - 1);
            }
            else
            {
                g.FillRectangle(box.Brush,
                                   boxX,
                                   textYoffset + line * (textLineSpacing + lineHeight),
                                   byteWidth - 1,
                                   lineHeight - 1);
            }

            return true;
        }

        private bool DrawCursor(Graphics g)
        {
            int addr = cursorAddress;

            int screenByte = addr - StartAddress;
            int screenByteCount = lineCount * wordsPerLine * AddressDigits;
            if (screenByte < 0 || screenByte > screenByteCount)
                return false; // This shouldn't happen. Always push the cursor so that it stays on the screen.

            int line = screenByte / bytesPerLine; // Which line should the box appear on?
            //Debug.WriteLine($"cursor is at {screenByte}th byte on line");
            //Debug.WriteLine($"bytes per line  = {bytesPerLine}");
            //Debug.WriteLine($"cursor is on line {line}");
            int lineByte = screenByte % bytesPerLine;
            int lineWord = lineByte / WordBytes;
            int wordByte = addr % WordBytes;

            float boxX = dataXoffset + lineWord * (wordWidth + wordXgap) + wordByte * (byteWidth);

            if (Focused)
            {
                g.FillRectangle(Brushes.Black,
                                boxX,
                                textYoffset + line * (textLineSpacing + lineHeight),
                                3,
                                lineHeight);
            }
            else
            {
                g.DrawRectangle(Pens.Black,
                                boxX,
                                textYoffset + line * (textLineSpacing + lineHeight),
                                3,
                                lineHeight);
            }

            return true;
        }

        private void PaintCenteredText(Graphics g, string text)
        {
            var bounds = g.VisibleClipBounds;
            var textsz = g.MeasureString(text, Font);
            g.DrawString(text, Font, Brushes.Black, (bounds.Width - textsz.Width) / 2, (bounds.Height - textsz.Height) / 2);
        }

        private void PaintCenteredText(Graphics g, string text, float emSize)
        {
            var font = new Font(Font.FontFamily, emSize);
            var bounds = g.VisibleClipBounds;
            var textsz = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.Black, (bounds.Width - textsz.Width) / 2, (bounds.Height - textsz.Height) / 2);
        }

        #endregion

        private static string DecodeAsUTF8(byte[] array, int offset, int count)
        {
            var ret = System.Text.Encoding.UTF8.GetChars(array, offset, count);
            for (int i = 0; i < ret.Length; ++i)
            {
                char c = ret[i];
                if (char.IsWhiteSpace(c))
                {
                    ret[i] = ' ';
                    continue;
                }
                if (char.IsControl(c))
                {
                    ret[i] = '�';
                }
            }
            return new string(ret);
        }

        #region Event Handlers

        public struct Selection
        {
            public readonly int StartAddress, EndAddress, ByteCount;
            internal Selection(int startAddress, int endAddress)
            {
                if (startAddress < endAddress)
                {
                    StartAddress = startAddress;
                    EndAddress = endAddress;
                }
                else
                {
                    StartAddress = endAddress;
                    EndAddress = startAddress;
                }
                ByteCount = EndAddress - StartAddress;
            }

            //public static readonly Selection Empty = new Selection(0, 0);
        }

        public class SelectionChangedEventArgs : EventArgs
        {
            public SelectionChangedEventArgs(Selection data)
            {
                Data = data;
            }

            public readonly Selection Data;
        }

        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs args);
        public event SelectionChangedEventHandler SelectionChanged;

        public Selection GetSelection()
        {
            return new Selection(selectionStartAddress, selectionStopAddress);
        }

        public int SelectedByteCount
        {
            get { return Math.Abs(selectionStopAddress - selectionStartAddress); }
        }

        int selectionStartAddress = 0, selectionStopAddress = 0;
        bool dragging = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e); // calls Focus().

            if (e.Button != MouseButtons.Left)
                return;

            MoveCursorToPoint(e.Location);
            dragging = true;
            selectionStartAddress = CursorAddress;
            selectionStopAddress = CursorAddress;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            base.OnMouseUp(e);
            dragging = false;
        }

        protected override void OnResize(EventArgs e)
        {
            doRecalc = true;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (dragging)
            {
                MoveCursorToPoint(e.Location);
                selectionStopAddress = CursorAddress;
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(GetSelection()));
            }
        }

        private void OnFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnBlur(object sender, EventArgs e)
        {
            Invalidate();
        }

        #endregion

        private void MoveCursorToPoint(Point p)
        {
            int line = (int)((p.Y - textYoffset) / (lineHeight + textLineSpacing));
            int wordIdx = (int)((p.X - dataXoffset) / (wordWidth + wordXgap));
            float charGap = wordWidth / 3;
            int wordByte = (int)((p.X - dataXoffset - wordIdx * (wordWidth + wordXgap)) / charGap);

            int addr = StartAddress + line * bytesPerLine + wordIdx * WordDigits / 2 + wordByte;
            if (addr >= 0 && addr < Data.Length)
            {
                CursorAddress = addr;
                Invalidate();
            }
        }

        private const float SCROLL_MULTIPLIER = -0.02f;
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int newSA = startAddress + bytesPerLine * (int)Math.Round(e.Delta * SCROLL_MULTIPLIER, 0);
            if (newSA >= 0 && newSA < Data.Length)
                StartAddress = newSA;
        }
    }
}
