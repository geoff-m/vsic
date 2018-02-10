using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace sicsim
{
    public partial class MainForm : Form, ILogSink
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region unused
        //Label[] registerLabels;
        //TextBox[] registerTboxes;
        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    // Set up labels for registers.
        //    registerLabels = new Label[6];
        //    var regLabel = new Label();
        //    regLabel.Text = "A";
        //    registerLabels[0] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "B";
        //    registerLabels[1] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "S";
        //    registerLabels[2] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "T";
        //    registerLabels[3] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "X";
        //    registerLabels[4] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "L";
        //    registerLabels[5] = regLabel;

        //    // Set up text boxes for registers.
        //    registerTboxes = new TextBox[6];
        //    registerTboxes[0] = new TextBox();
        //    regGrpBox.Controls.AddRange(registerLabels);
        //    var offset = regGrpBox.ClientRectangle.Location;
        //    for (int i = 0; i < registerLabels.Length; ++i)
        //    {
        //        var labelLoc = new Point(offset.X + 2, offset.Y + 14 + (i == 0 ? 0 : i * registerLabels[i - 1].Height));
        //        registerLabels[i].Location = labelLoc;
        //    }

        //}
        #endregion

        Session sess;
        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Creating new machine...");

            new Task(() =>
            {
                sess = new Session();
                sess.Logger = this;

                Log("Done.");
                UpdateMachineDisplay();
                SetStatusMessage("Ready");
            }).Start();
        }

        #region Logging
        readonly Color COLOR_DEFAULT = Color.Black;
        readonly Color COLOR_ERROR = Color.DarkRed;
        public void Log(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(str, args)));
                return;
            }
            logBox.SelectionColor = COLOR_DEFAULT;
            logBox.AppendText(string.Format(str, args));
            logBox.AppendText("\n");
        }

        public void LogError(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(str, args)));
                return;
            }
            logBox.SelectionColor = COLOR_ERROR;
            logBox.AppendText(string.Format(str, args));
            logBox.AppendText("\n");
        }

        public void SetStatusMessage(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetStatusMessage(str, args)));
                return;
            }
            toolStripStatusLabel1.Text = string.Format(str, args);
        }
        #endregion Logging

        // Load the binary file into memory at the cursor.
        private void loadMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = openMemoryDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                bool success = true;
                long bytesRead = 0;
                int cursorAddress = 0; // todo: implement me.
                try
                {
                    bytesRead = sess.LoadMemory(openMemoryDialog.FileName, (Word)cursorAddress);
                }
                catch (ArgumentException argex)
                {
                    success = false;
                    LogError("Error loading file: {0}", argex.Message);
#if DEBUG
                    throw;
#else
                    return;
#endif
                }
                if (success)
                    Log($"Loaded {bytesRead} bytes from {Path.GetFileName(openMemoryDialog.FileName)} at address {cursorAddress}.");
                UpdateMachineDisplay();
            }
        }

        private void InitializeMachineDisplay()
        {
            if (sess != null && sess.Machine != null)
                hexDisplay.Data = sess.Machine.Memory;

            for (int i = 0; i < 60; ++i)
            {
                hexDisplay.Boxes.Add(new HexDisplay.BoxedByte(i, Pens.Green));
                //hexDisplay.Boxes.Add(new HexDisplay.BoxedByte(i, new Pen(new SolidBrush(Color.FromArgb(64, Color.Green)))));
            }

        }

        bool everUpdated = false;
        private void UpdateMachineDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateMachineDisplay()));
                return;
            }
            if (!everUpdated)
            {
                InitializeMachineDisplay();
            }

            // update labels.
            var m = sess.Machine;
            regATB.Text = m.RegisterA.ToString("X6");
            regBTB.Text = m.RegisterB.ToString("X6");
            regSTB.Text = m.RegisterS.ToString("X6");
            regTTB.Text = m.RegisterT.ToString("X6");
            regXTB.Text = m.RegisterX.ToString("X6");
            regLTB.Text = m.RegisterL.ToString("X6");
            pcTB.Text = m.ProgramCounter.ToString("X6");


            // update memory display
            hexDisplay.Invalidate();

            everUpdated = true;
        }

        private void OnResize(object sender, EventArgs e)
        {
            memGrpBox.Width = regGrpBox.Location.X - memGrpBox.Location.X;
            memGrpBox.Height = logBox.Location.Y - memGrpBox.Location.Y;
        }

        private void gotoTB_TextChanged(object sender, EventArgs e)
        {
            // If currently entered address is on screen, move cursor to it.
            // Else, wait for user to indicate they're done entering the address (e.g. on blur, press Enter)

            string text = gotoTB.Text.Trim();
            if (text.Length == 0)
            {
                gotoTB.BackColor = SystemColors.Window;
                return;
            }

            try
            {
                int addr = (int)Convert.ToUInt32(text, 16);
            }
            catch (FormatException ex)
            {
                gotoTB.BackColor = Color.LightPink;
                return;
            }
            gotoTB.BackColor = SystemColors.Window;

        }

        private void changedEncodingSelection(object sender, EventArgs e)
        {
            do
            {
                if (rawRB.Checked)
                {
                    hexDisplay.WordEncoding = HexDisplay.Encoding.Raw;
                    break;
                }
                if (utf8RB.Checked)
                {
                    hexDisplay.WordEncoding = HexDisplay.Encoding.UTF8;
                    break;
                }
            } while (false);
            hexDisplay.Invalidate();
        }
    }
}
