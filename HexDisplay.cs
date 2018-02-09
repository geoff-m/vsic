using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace sicsim
{
    public partial class HexDisplay : UserControl
    {
        public HexDisplay()
        {
            InitializeComponent();
        }

        public Stream Data
        { get; set; }

        #region Format properties
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
                // todo: address is not contained in display, update displayed window to contain it.
                cursorAddress = value;
            }
        }

        Font font = new Font(FontFamily.GenericMonospace, 10);
        public float FontSize
        {
            get
            {
                return font.Size;
            }
            set
            {
                font = new Font(FontFamily.GenericMonospace, value);
                doRecalc = true;
                // redraw
            }
        }
        #endregion

        float textYoffset = 0; // the Y offset for both the address and the data.
        float addressX = 4; // distance between left column and left of address.

        float addressDataGap = 5; // distance between right of address and left of data.

        float dataXendPadding = 4; // distance between right of data and right column.
        float wordXgap = 4; // distance between each successive word of data.

        float wordWidth;
        float dataXoffset; // computed distance between left column and left of leftmost address.
        int wordsPerLine; // computed number of words to display per line.
        int lineCount; // computed number of lines to display.
        float textLineSpacing; // computed distance between lines.
        float textHeight; // computed height of a line of text.

        /// <summary>
        /// Recalculates the parameters needed to draw this HexDisplay.
        /// </summary>
        /// <param name="g">The graphics object to be used for measurements.</param>
        private void UpdateMeasurements(Graphics g)
        {
            var BOUNDS = g.VisibleClipBounds;
            var addressSize = g.MeasureString(new string('F', addressDigits), font);
            float addrH = addressSize.Height;
            float addrW = addressSize.Width;
            textLineSpacing = 0.05f * addrH;
            // addrH * lines + lineSpacing * (lines - 1) = BOUNDS.Height - textYoffset
            // addrH * lines + 0.1f * addrH * (lines - 1) = BOUNDS.Height - textYoffset
            // addrH * lines + 0.1f * addrH * lines - 0.1f * addrH = BOUNDS.Height - textYoffset
            // lines * (addrH + 0.1f * addrH - 0.1f * addrH) = BOUNDS.Height - textYoffset
            lineCount = (int)((BOUNDS.Height - textYoffset) / (addrH + 0.1f * addrH - 0.1f * addrH));
            textHeight = addrH;

            dataXoffset = addressX + addrW + addressDataGap;

            wordWidth = g.MeasureString(new string('F', WordDigits), font).Width;

            // BOUNDS.Width - (addressX + addrW + addressDataGap) = dataLine.Width + dataXendPadding
            // BOUNDS.Width - (addressX + addrW + addressDataGap) = wordsPerLine * wordW+ (wordsPerLine - 1) * wordXgap + dataXendPadding
            // BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding = wordsPerLine * wordW + wordsPerLine * wordXgap -  wordXgap
            // BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding = wordsPerLine * (wordW + wordXgap) - wordXgap
            // BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding + wordXgap = wordsPerLine * (wordW + wordXgap)
            // wordsPerLine = (BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding + wordXgap) / (wordW + wordXgap)
            wordsPerLine = (int)((BOUNDS.Width - (addressX + addrW + addressDataGap) - dataXendPadding + wordXgap) / (wordWidth + wordXgap));

            doRecalc = false;
        }

        bool doRecalc = true;
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

            int bytesPerLine = wordsPerLine * 3;
            int lineAddress = startAddress;
            var lineBytes = new byte[bytesPerLine];
            for (int line = 0; line < lineCount; ++line)
            {
                float y = textYoffset + line * (textLineSpacing + textHeight); // caching the Y offset of this line.

                //g.DrawString(lineAddress.ToString(addressFormatString), font, Brushes.DarkSlateGray, addressX, y);
                g.DrawString($"{line} of {lineCount}", font, Brushes.DarkSlateGray, addressX, y);

                int bytesRead = Data.Read(lineBytes, 0, bytesPerLine);

                var lineWords = Word.FromArray(lineBytes, 0, bytesRead);
                g.DrawString(lineWords[0].ToString(wordFormatString), font, Brushes.Black, dataXoffset, y);
                for (int wordIdx = 1; wordIdx < lineWords.Length; ++wordIdx)
                {
                    g.DrawString(lineWords[wordIdx].ToString(wordFormatString), font, Brushes.Black, dataXoffset + wordIdx * (wordWidth + wordXgap), y);
                }

                lineAddress += bytesPerLine;
            }

        }

        private void PaintCenteredText(Graphics g, string text)
        {
            var bounds = g.VisibleClipBounds;
            var textsz = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.Black, (bounds.Width - textsz.Width) / 2, (bounds.Height - textsz.Height) / 2);
        }

        private void OnResize(object sender, EventArgs e)
        {
            doRecalc = true;
            Invalidate();
        }
    }
}
