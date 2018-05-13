using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace vsic
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
            DoubleBuffered = true;
            Enter += OnFocus;
            Leave += OnBlur;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
        }

        public HashSet<ByteMarker> Boxes = new HashSet<ByteMarker>();

        public Stream Data
        { get; set; }

        Encoding enc = Encoding.Raw;
        public Encoding WordEncoding
        {
            get
            {
                return enc;
            }
            set
            {
                enc = value;
                // I used to have Invalidate() here, but we'll leave that to the caller.
                // So for now (2-9-2018), 'enc' is pointless and there's no reason this isn't an auto property.
            }
        }

        #region Format properties
        private int WordBytes => WordDigits / 2;
        string wordFormatString = "X6";
        int wordDigits = 6;
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

        string addressFormatString = "X6";
        int addressDigits = 6;
        public int AddressDigits
        {
            get
            {
                return addressDigits;
            }
            set
            {
                addressDigits = value;
                addressFormatString = 'X' + addressDigits.ToString();
                doRecalc = true;
            }
        }

        int startAddress = 0;
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

        public int SelectionStartAddress
        { get; private set; }
        public int SelectionEndAddress
        { get; private set; }

        int cursorAddress = 0;
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


                if (CursorAddressChanged != null)
                    CursorAddressChanged.Invoke(this, null);
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

        float textYoffset = 0; // the Y offset for both the address and the data.
        float addressX = 4; // distance between left column and left of address.

        float addressDataGap = 5; // distance between right of address and left of data.

        float dataXendPadding = 4; // distance between right of data and right column.
        float wordXgap = 4; // distance between each successive word of data.

        float wordWidth;
        float byteWidth;
        float dataXoffset; // computed distance between left column and left of leftmost address.
        int wordsPerLine; // computed number of words to display per line.
        int bytesPerLine; // computed number of bytes that are on each line.
        int lineCount; // computed number of lines to display.
        float textLineSpacing; // computed distance between lines.
        float lineHeight; // computed height of a line of text.
        int screenByteCount; // computed number of bytes that fit on the screen.

        bool doRecalc = true;
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
            byteWidth = wordWidth / (WordDigits / 2); // todo: get correct value for byteWidth. i think it is being overestimated.

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

            screenByteCount = lineCount * wordsPerLine * AddressDigits;

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
                Enabled = false;
                return;
            }
            Enabled = true;

            if (doRecalc)
                UpdateMeasurements(g);

            if (wordsPerLine == 0)
                return;

            // Draw boxes.
            int boxesDrawn = 0; // for debug.
            foreach (var box in Boxes)
            {
                if (DrawBox(g, box))
                    ++boxesDrawn;
            }

            HighlightSelection(g);

            // Draw addresses and data.
            int bytesPerLine = wordsPerLine * 3;
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

                switch (enc)
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
        }

        private static Brush _highlightBrush = new SolidBrush(Color.FromArgb(127, Color.Blue));
        private void HighlightSelection(Graphics g)
        {
            if (SelectionStartAddress > startAddress + screenByteCount)
                return; // If selection begins after screen, abort.

            int highlightStart = SelectionStartAddress - startAddress;

            if (SelectionStartAddress < startAddress)
                highlightStart = startAddress; // The beginning of the selection will be cut off.

            int endAddress = startAddress + screenByteCount;
            int highlightEnd = SelectionEndAddress - startAddress;
            if (SelectionEndAddress > endAddress)
                highlightEnd = endAddress; // The end of the selection will be cut off.

            // Find the first line to be highlighted.
            int startLine = highlightStart / bytesPerLine;
            // Find the first byte in the first line to be highlighted.
            int firstByteInLine = highlightStart % bytesPerLine;
            // Find the word containing the first byte.
            int firstByteWordInLine = firstByteInLine / WordBytes;
            // Identify the byte inside the word.
            int firstByteInWord = highlightStart % WordBytes;

            // Find the last line to be highlighted.
            int endLine = highlightEnd / bytesPerLine;
            // Find the last byte in the last line to be highlighted.
            int lastByteInLine = highlightEnd % bytesPerLine;
            // Find the word containing the last byte.
            int lastByteWordInLine = lastByteInLine / WordBytes;
            // Identify the byte inside the word.
            int lastByteInWord = highlightEnd % WordBytes;

            if (startLine == endLine)
            {
                // Highlight the only line.
                g.FillRectangle(_highlightBrush,
                    dataXoffset + firstByteWordInLine * (wordWidth + wordXgap) + firstByteInWord * byteWidth,
                    textYoffset + startLine * (lineHeight + textLineSpacing),
                    byteWidth * (lastByteInLine - firstByteInLine),
                    lineHeight);
                return;
            }

            // Highlight the first line.            
            g.FillRectangle(_highlightBrush,
                 dataXoffset + firstByteWordInLine * (wordWidth + wordXgap) + firstByteInWord * byteWidth,
                 textYoffset + startLine * (lineHeight + textLineSpacing),
                 byteWidth * (bytesPerLine - firstByteInLine),
                 lineHeight);

            // Highlight the middle lines.
            for (int line = startLine + 1; line < endLine; ++line)
            {
                g.FillRectangle(_highlightBrush,
                     dataXoffset,
                     textYoffset + line * (lineHeight + textLineSpacing),
                     wordsPerLine * wordXgap + byteWidth * bytesPerLine,
                     lineHeight);
            }

            // Highlight the last line.
            g.FillRectangle(_highlightBrush,
                 dataXoffset + lastByteInWord * byteWidth,
                 textYoffset + endLine * (lineHeight + textLineSpacing),
                 wordXgap * (lastByteInLine / 3) + byteWidth * lastByteInLine,
                 lineHeight);
        }

        private bool DrawBox(Graphics g, ByteMarker box)
        {
            int addr = box.Address;
            int screenByte = addr - StartAddress;
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

        // Returns the address under the specified screen coordinates.
        // The result is not guaranteed to be a valid memory address.
        private int GetAddressFromPoint(int x, int y)
        {
            int line = (int)((y - textYoffset) / (lineHeight + textLineSpacing));
            int wordIdx = (int)((x - dataXoffset) / (wordWidth + wordXgap));
            float charGap = wordWidth / 3;
            int wordByte = (int)((x - dataXoffset - wordIdx * (wordWidth + wordXgap)) / charGap);
            int addr = StartAddress + line * bytesPerLine + wordIdx * WordDigits / 2 + wordByte;
            return addr;
        }

        #region Event Handlers

        private void OnResize(object sender, EventArgs e)
        {
            doRecalc = true;
            Invalidate();
        }

        private void UpdateCursorLocation(int x, int y)
        {
            int addr = GetAddressFromPoint(x, y);
            if (addr >= 0 && addr < Data.Length)
            {
                CursorAddress = addr;
                Invalidate();
            }
        }

        private bool dragging = false;
        private int dragStartAddress;
        private int dragEndAddress;
        private void OnMouseDown(object sender, MouseEventArgs me)
        {
            if (me.Button != MouseButtons.Left)
                return;

            UpdateCursorLocation(me.X, me.Y);
            dragging = true;
            dragStartAddress = GetAddressFromPoint(me.X, me.Y);
            dragEndAddress = dragStartAddress;
        }

        public void OnMouseMove(object sender, MouseEventArgs me)
        {
            if (!dragging)
                return;

            UpdateCursorLocation(me.X, me.Y);
            dragEndAddress = GetAddressFromPoint(me.X, me.Y);
            UpdateSelection();
            Invalidate();
        }

        public struct Selection
        {
            public int StartAddress
            { get; private set; }

            public int EndAddress
            { get; private set; }

            public Selection(int start, int end)
            {
                StartAddress = start;
                EndAddress = end;
            }
        }

        public event EventHandler<Selection> SelectionChanged;

        public const int SELECTION_BYTE_MARKER_ID = 0x5e1ec7;
        // This method publishes information about the selection.
        private void UpdateSelection()
        {
            if (dragStartAddress < dragEndAddress)
            {
                SelectionStartAddress = dragStartAddress;
                SelectionEndAddress = dragEndAddress;
            }
            else
            {
                SelectionStartAddress = dragEndAddress;
                SelectionEndAddress = dragStartAddress;
            }
            //Boxes.RemoveWhere(bm => bm.GetHashCode() == SELECTION_BYTE_MARKER_ID);
            //for (int i = SelectionStartAddress; i < SelectionEndAddress; ++i)
            //{
            //    Boxes.Add(new ByteMarker(i, Color.FromArgb(127, Color.Blue), null, false, SELECTION_BYTE_MARKER_ID));
            //}
            SelectionChanged?.Invoke(this, new Selection(SelectionStartAddress, SelectionEndAddress));
        }

        public void OnMouseUp(object sender, MouseEventArgs me)
        {
            if (!dragging || me.Button != MouseButtons.Left)
                return;

            dragEndAddress = GetAddressFromPoint(me.X, me.Y);
            // complete the drag selection and make results public.
            dragging = false;
            UpdateSelection();
            Invalidate();
        }

        private void OnFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnBlur(object sender, EventArgs e)
        {
            Invalidate();
        }

        private const float SCROLL_MULTIPLIER = -0.02f;
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int newSA = startAddress + bytesPerLine * (int)Math.Round(e.Delta * SCROLL_MULTIPLIER, 0);
            if (newSA >= 0 && newSA < Data.Length)
                StartAddress = newSA;
        }

        #endregion
    }
}
