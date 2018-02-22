using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Path = System.IO.Path;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace vsic
{
    public partial class MainForm : Form, ILogSink
    {
        public MainForm()
        {
            InitializeComponent();
            ccCB.Items.Add("Less than");
            ccCB.Items.Add("Equal to");
            ccCB.Items.Add("Greater than");

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (hexDisplayFocused)
            {
                switch (keyData)
                {
                    case Keys.Up:
                        hexDisplay.MoveCursorUp();
                        break;
                    case Keys.Down:
                        hexDisplay.MoveCursorDown();
                        break;
                    case Keys.Left:
                    case Keys.Back: // Backspace.
                        hexDisplay.MoveCursorLeft();
                        break;
                    case Keys.Right:
                        hexDisplay.MoveCursorRight();
                        break;
                    case Keys.Home:
                        hexDisplay.MoveCursorHome();
                        break;
                    case Keys.End:
                        hexDisplay.MoveCursorEnd();
                        break;
                    default:
                        return base.ProcessCmdKey(ref msg, keyData);
                }

                hexDisplay.Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        Session sess;
        private void Form1_Load(object sender, EventArgs e)
        {
            CreateNewSession();
        }

        private void CreateNewSession()
        {
            sess = new Session();
            sess.Logger = this;

            Log("Created new SIC/XE machine.");
            InitializeMachineDisplay();
            UpdateMachineDisplay();
            SetStatusMessage("Ready");
        }

        private void UnloadSession()
        {
            sess.Machine.MemoryChanged -= OnMemoryChanged;
            sess.Machine.RegisterChanged -= OnRegisterChanged;
        }

        #region ILogSink implementation
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
            string s = string.Format(str, args);
            Debug.WriteLine(s);
            logBox.AppendText(s);
            logBox.AppendText("\n");

            // Scroll to bottom.
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }

        public void LogError(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(str, args)));
                return;
            }
            logBox.SelectionColor = COLOR_ERROR;
            string s = string.Format(str, args);
            Debug.WriteLine(s);
            logBox.AppendText(s);
            logBox.AppendText("\n");

            // Scroll to bottom.
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
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
                int loadAddress = hexDisplay.CursorAddress;
                try
                {
                    bytesRead = sess.LoadMemory(openMemoryDialog.FileName, (Word)loadAddress);
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
                    Log($"Loaded {bytesRead} bytes from {Path.GetFileName(openMemoryDialog.FileName)} at address {loadAddress.ToString("X")}.");
                UpdateMachineDisplay();
            }
        }

        private const int PC_MARKER_ID = -1;
        private readonly Color PC_MARKER_COLOR = Color.Yellow;
        private void InitializeMachineDisplay()
        {
            if (sess != null && sess.Machine != null)
            {
                hexDisplay.Data = sess.Machine.Memory;
                sess.Machine.MemoryChanged += OnMemoryChanged;
                sess.Machine.RegisterChanged += OnRegisterChanged;
            }

            hexDisplay.Boxes.Clear();

            pcMarker = new ByteMarker((int)sess.Machine.ProgramCounter,
                (int)sess.Machine.InstructionsExecuted,
                PC_MARKER_COLOR,
                false,
                PC_MARKER_ID);

            ResetTextboxColors();
        }

        private void OnMemoryChanged(Word addr, int count, bool written)
        {
            int start = (int)addr;
            for (int i = start; i < start + count; ++i)
            {
                var newBox = new ByteMarker(i,
                    (int)sess.Machine.InstructionsExecuted,
                    written ? Color.LightGreen : Color.Pink);

                hexDisplay.Boxes.Add(newBox);
            }
        }

        readonly Color REGISTER_WRITTEN_COLOR = Color.LightGreen;
        readonly Color REGISTER_READ_COLOR = Color.Pink;
        private void OnRegisterChanged(Register r, bool written)
        {
            switch (r)
            {
                case Register.A:
                    regATB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.X:
                    regXTB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.L:
                    regLTB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.PC:
                    pcTB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.CC:
                    ccCB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.B:
                    regBTB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.S:
                    regSTB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.T:
                    regTTB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
                case Register.F:
                    regFTB.BackColor = written ? REGISTER_WRITTEN_COLOR : REGISTER_READ_COLOR;
                    break;
            }
        }

        private void ResetTextboxColors()
        {
            regATB.BackColor = SystemColors.Window;
            regXTB.BackColor = SystemColors.Window;
            regLTB.BackColor = SystemColors.Window;
            pcTB.BackColor = SystemColors.Window;
            ccCB.BackColor = SystemColors.Window;
            regBTB.BackColor = SystemColors.Window;
            regSTB.BackColor = SystemColors.Window;
            regTTB.BackColor = SystemColors.Window;
        }

        ByteMarker pcMarker;
        private void UpdateMachineDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateMachineDisplay()));
                return;
            }

            // Update labels.
            var m = sess.Machine;
            regATB.Text = m.RegisterA.ToString("X6");
            regBTB.Text = m.RegisterB.ToString("X6");
            regSTB.Text = m.RegisterS.ToString("X6");
            regTTB.Text = m.RegisterT.ToString("X6");
            regXTB.Text = m.RegisterX.ToString("X6");
            regLTB.Text = m.RegisterL.ToString("X6");
            pcTB.Text = m.ProgramCounter.ToString("X6");

            switch (m.ConditionCode)
            {
                case ConditionCode.EqualTo:
                    ccCB.SelectedIndex = 1;
                    break;
                case ConditionCode.LessThan:
                    ccCB.SelectedIndex = 0;
                    break;
                case ConditionCode.GreaterThan:
                    ccCB.SelectedIndex = 2;
                    break;
            }

            // Cull old markers.
            int instr = (int)m.InstructionsExecuted;
            int removed = hexDisplay.Boxes.RemoveWhere(bm => (instr - bm.Timestamp) > 1);
            //Debug.WriteLine($"Removed {removed} markers.");

            // Remove old program counter.
            hexDisplay.Boxes.RemoveWhere(bm => bm.GetHashCode() == PC_MARKER_ID);

            // Add new program counter marker.
            pcMarker = new ByteMarker(m.ProgramCounter,
                instr - 1,
                PC_MARKER_COLOR,
                false,
                PC_MARKER_ID);
            hexDisplay.Boxes.Add(pcMarker);

            // Update memory display.
            hexDisplay.Invalidate();
        }

        private void OnResize(object sender, EventArgs e)
        {
            memGrpBox.Width = regGrpBox.Location.X - memGrpBox.Location.X - 10;
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
            catch (FormatException)
            {
                gotoTB.BackColor = Color.LightPink;
                return;
            }
            gotoTB.BackColor = SystemColors.Window;
        }

        private void changedEncodingSelection(object sender, EventArgs e)
        {
            do // Not a loop.
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

        private void OnCursorMove(object sender, EventArgs e)
        {
            string addr = hexDisplay.CursorAddress.ToString("X");
            loadMemoryToolStripMenuItem.Text = $"Load Memory at {addr}...";
            cursorPositionLabel.Text = $"0x{addr}";
        }

        private void loadOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = openOBJdialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                sess.Machine.LoadObj(openOBJdialog.FileName);

                // Remove all temporary markers.
                hexDisplay.Boxes.Clear();

                ResetTextboxColors();

                UpdateMachineDisplay();
            }
        }

        private void stepButton_Click(object sender, EventArgs e)
        {
            ResetTextboxColors();
            var res = sess.Machine.Step();
            switch (res)
            {
                case Machine.RunResult.IllegalInstruction:
                    LogError($"Illegal instruction at address 0x{((int)sess.Machine.ProgramCounter).ToString("X")}!");
                    break;
            }
            UpdateMachineDisplay();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            sess.Machine.Run();
            UpdateMachineDisplay();
        }

        WatchForm watchForm = new WatchForm();
        private void watchesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            watchForm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newMachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnloadSession();
            CreateNewSession();
        }

        private void loadLstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = openLSTdialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                sess.Machine.LoadLst(openLSTdialog.FileName);

                // Remove all temporary markers.
                hexDisplay.Boxes.Clear();

                UpdateMachineDisplay();
            }
        }

        bool hexDisplayFocused;
        private void onHexDisplayFocus(object sender, EventArgs e)
        {
            hexDisplayFocused = true;
        }

        private void onHexDisplayBlur(object sender, EventArgs e)
        {
            hexDisplayFocused = false;
        }

        // todo: use this function to handle key press for all registers
        private void onRegisterTBKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null)
            {
                Debug.WriteLine($"Register textbox key press handler called from unexpected with unexpected sender: {sender.ToString()}");
                return;
            }

            char c = e.KeyChar;
            int cursorIndex = tb.SelectionStart;
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    if (cursorIndex < tb.MaxLength) // Do nothing if caret is after last character.
                    {
                        // Overwrite the character following the caret.
                        tb.SelectionLength = 1;
                        tb.SelectedText = char.ToUpper(c).ToString();
                    }
                    break;

                case (char)Keys.Enter:
                    // Commit changes to machine.
                    Word newValue = (Word)int.Parse(tb.Text, System.Globalization.NumberStyles.HexNumber);

                    if (ReferenceEquals(tb, pcTB))
                    {
                        sess.Machine.ProgramCounter = newValue;
                        UpdateMachineDisplay();
                        break;
                    }
                    if (ReferenceEquals(tb, regATB))
                    {
                        sess.Machine.RegisterA = newValue;
                        break;
                    }
                    if (ReferenceEquals(tb, regXTB))
                    {
                        sess.Machine.RegisterX = newValue;
                        break;
                    }
                    if (ReferenceEquals(tb, regTTB))
                    {
                        sess.Machine.RegisterT = newValue;
                        break;
                    }
                    if (ReferenceEquals(tb, regBTB))
                    {
                        sess.Machine.RegisterB = newValue;
                        break;
                    }
                    if (ReferenceEquals(tb, regSTB))
                    {
                        sess.Machine.RegisterS = newValue;
                        break;
                    }
                    if (ReferenceEquals(tb, regLTB))
                    {
                        sess.Machine.RegisterL = newValue;
                        break;
                    }
                    
                    // This should be unreachable.
                    Debug.WriteLine($"Register textbox key press handler called from unexpected with unexpected sender: {sender.ToString()}");
                    return;

                case (char)Keys.Back:
                    tb.SelectionStart = cursorIndex - 1;
                    break;
            }

            e.Handled = true;
        }


    }
}
