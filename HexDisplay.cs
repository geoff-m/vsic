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
            Click += OnClick;
            Enter += OnFocus;
            Leave += OnBlur;
        }

        HashSet<ByteMarker> boxes = new HashSet<ByteMarker>();
        public HashSet<ByteMarker> Boxes
        { get { return boxes; } }

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
                // redraw
            }
        }

        int cursorAddress = 0;
        public int CursorAddress
        {
            get
            {
                return cursorAddress;
            }
            set
            {
                if ((Data != null && cursorAddress >= Data.Length) || cursorAddress < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                // todo: if address is not contained in display, update displayed window to contain it.
                cursorAddress = value;
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
            if (newaddr > Data.Length)
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
            if (newaddr > Data.Length)
            {
                // Do nothing if moving right would put us after the end.
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
            // addrH * lineCount + lineSpacing * (lineCount - 1) = BOUNDS.Height - textYoffset
            // lineCount * (addrH + lineSpacing - 1) = BOUNDS.Height - textYoffset
            lineCount = (int)((BOUNDS.Height - 1 - textYoffset) / (addrH + textLineSpacing - 1));
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

            doRecalc = false;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
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

            // Draw boxes.
            int boxesDrawn = 0; // for debug.
            foreach (var box in boxes)
            {
                if (DrawBox(g, box))
                    ++boxesDrawn;
            }

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
                        if (lineWordCount < 0)
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

        private void OnResize(object sender, EventArgs e)
        {
            doRecalc = true;
            Invalidate();
        }

        private void OnClick(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me == null || me.Button != MouseButtons.Left)
                return;

            int line = (int)((me.Y - textYoffset) / (lineHeight + textLineSpacing));
            int wordIdx = (int)((me.X - dataXoffset) / (wordWidth + wordXgap));
            float charGap = wordWidth / 3;
            int wordByte = (int)((me.X - dataXoffset - wordIdx * (wordWidth + wordXgap)) / charGap);

            CursorAddress = line * bytesPerLine + wordIdx * WordDigits / 2 + wordByte;

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

        #endregion

    }
}
