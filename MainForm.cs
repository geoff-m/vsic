using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Path = System.IO.Path;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

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

            conWindow = new ConsoleWindow();
            breakpoints = new SortedSet<Breakpoint>(new Breakpoint.Comparer());

            devman = new DeviceManager();
            devman.Owner = this;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Global hotkeys.
            switch (keyData)
            {
                case Keys.F5:
                    runButton_Click(this, null);
                    return true;
                case Keys.F10:
                    stepButton_Click(this, null);
                    return true;
                case Keys.F9:
                    setBkptButton_Click(this, null);
                    return true;
            }

            // Hexdisplay navigation.
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
                    case Keys.PageUp:
                        hexDisplay.MoveCursorUpFar();
                        break;
                    case Keys.PageDown:
                        hexDisplay.MoveCursorDownFar();
                        break;
                    default:
                        return base.ProcessCmdKey(ref msg, keyData);
                }

                hexDisplay.Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        Thread machineThread;
        Session sess;
        private void Form1_Load(object sender, EventArgs e)
        {
            CreateNewSession();
        }

        private void CreateNewSession()
        {
            sess = new Session();
            sess.Logger = this;
            devman.Machine = sess.Machine;

            Log("Created new SIC/XE machine.");
            breakpoints.Clear();
            InitializeMachineDisplay();
            UpdateMachineDisplay();
            UpdateIODevices(null, null);
            SetStatusMessage("Ready");
        }

        private void UnloadSession()
        {
            if (sess != null)
            {
                var m = sess.Machine;
                if (m != null)
                {
                    m.MemoryChanged -= OnMemoryChanged;
                    m.RegisterChanged -= OnRegisterChanged;
                    Debug.WriteLine("Disposing I/O devices...");
                    m.CloseDevices();
                    Debug.WriteLine("Done disposing I/O devices.");
                }
            }
            
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
            //logBox.SelectionColor = COLOR_DEFAULT;
            string s = string.Format(str, args);
            Debug.WriteLine(s);
            logBox.AppendText(s);
            logBox.AppendText("\n");

            // Skip these calls during 'Run'.
            // SuspendDrawing() does not prevent these calls from being very slow.
            if (machineThread == null || !machineThread.IsAlive)
            {
                // Scroll to bottom.
                logBox.SelectionStart = logBox.Text.Length;
                logBox.ScrollToCaret();
            }
        }

        public void LogError(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LogError(str, args)));
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
            toolStripStatusLabel.Text = string.Format(str, args);
        }
        #endregion

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

            pcMarker = new ByteMarker(sess.Machine.ProgramCounter,
                PC_MARKER_COLOR,
                sess.Machine.InstructionsExecuted,
                false,
                PC_MARKER_ID);

            ResetTextboxColors();
        }

        readonly Color REGISTER_WRITTEN_COLOR = Color.LightGreen;
        readonly Color REGISTER_READ_COLOR = Color.Pink;
        private void OnRegisterChanged(Register r, bool written)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnRegisterChanged(r, written)));
                return;
            }
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

        private void SuspendMachineDisplayUpdates()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SuspendMachineDisplayUpdates()));
                return;
            }
            logBox.SuspendDrawing();
            regATB.SuspendDrawing();
            regXTB.SuspendDrawing();
            regLTB.SuspendDrawing();
            regBTB.SuspendDrawing();
            regTTB.SuspendDrawing();
            regFTB.SuspendDrawing();
            pcTB.SuspendDrawing();
            ccCB.SuspendDrawing();
        }

        private void ResumeMachineDisplayUpdates()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ResumeMachineDisplayUpdates()));
                return;
            }
            logBox.ResumeDrawing();
            regATB.ResumeDrawing();
            regXTB.ResumeDrawing();
            regLTB.ResumeDrawing();
            regBTB.ResumeDrawing();
            regTTB.ResumeDrawing();
            regFTB.ResumeDrawing();
            pcTB.ResumeDrawing();
            ccCB.ResumeDrawing();

            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
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
            int instr = (int)m.InstructionsExecuted - 1;
            int removed = hexDisplay.Boxes.RemoveWhere(bm =>
            {
                long? expiry = bm.ExpiresAfter;
                bool ret = expiry.HasValue && instr > expiry.Value;
                //Debug.WriteLineIf(ret, $"Culling byte marker {bm.ToString()}. Instr = {instr}.");
                return ret;
            }
            );
            //Debug.WriteLine($"Removed {removed} byte markers.");

            // Remove old program counter.
            hexDisplay.Boxes.RemoveWhere(bm => bm.GetHashCode() == PC_MARKER_ID);

            // Add new program counter marker.
            pcMarker = new ByteMarker(m.ProgramCounter,
                PC_MARKER_COLOR,
                null,
                false,
                PC_MARKER_ID);
            hexDisplay.Boxes.Add(pcMarker);

            // Update memory display.
            hexDisplay.Invalidate();
        }

        private void OnResize(object sender, EventArgs e)
        {
            memGB.Width = regGB.Location.X - memGB.Location.X - 10;
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

        private void OnHexDisplayCursorMove(object sender, EventArgs e)
        {
            int addr = hexDisplay.CursorAddress;
            string addrString = addr.ToString("X");
            loadMemoryToolStripMenuItem.Text = $"Load Memory at {addrString}...";
            cursorPositionLabel.Text = $"0x{addrString}";
            PCfromCursorLabel.Text = addrString.PadLeft(6, '0');

            UpdateBreakpointGB();
        }

        private void loadOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = openOBJdialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                sess.Machine.LoadObj(openOBJdialog.FileName);

                // Remove all temporary markers.
                hexDisplay.Boxes.RemoveWhere(bm => bm.ExpiresAfter.HasValue);

                ResetTextboxColors();

                UpdateMachineDisplay();
            }
        }

        private void stepButton_Click(object sender, EventArgs e)
        {
            ResetTextboxColors();
            sess.Machine.Step();
            sess.Machine.FlushDevices();
            HandleExecutionResult();
            UpdateMachineDisplay();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            StartMachineRun();
        }

        long instructionsAtRunStart;
        private void StartMachineRun()
        {
            Debug.Assert(machineThread == null || !machineThread.IsAlive, "Machine thread is already running?!");
            instructionsAtRunStart = sess.Machine.InstructionsExecuted;
            SuspendMachineDisplayUpdates();
            machineThread = new Thread(new ThreadStart(() => { sess.Machine.Run(); EndMachineRun(); }));
            machineThread.Start();
        }

        private void EndMachineRun()
        {
            var m = sess.Machine;
            m.FlushDevices();
            ResumeMachineDisplayUpdates();
            long endInstr = m.InstructionsExecuted;
            long diff = endInstr - instructionsAtRunStart;
            Log($"Run: {diff.ToString()} instructions executed.");
            HandleExecutionResult();
            UpdateMachineDisplay();
        }

        private void HandleExecutionResult()
        {
            switch (sess.Machine.LastResult)
            {
                case Machine.RunResult.None:
                    toolStripStatusLabel.Text = "";
                    break;
                case Machine.RunResult.HardwareFault:
                    LogError("A hardware fault has occurred!");
                    toolStripStatusLabel.Text = "Hardware fault";
                    break;
                case Machine.RunResult.HitBreakpoint:
                    toolStripStatusLabel.Text = "Hit breakpoint";
                    break;
                case Machine.RunResult.IllegalInstruction:
                    LogError($"Illegal instruction at address 0x{((int)sess.Machine.ProgramCounter).ToString("X")}!");
                    toolStripStatusLabel.Text = "Illegal instruction";
                    break;
                case Machine.RunResult.EndOfMemory:
                    LogError("The program counter has hit the end of memory.");
                    break;
            }
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

        // This function to handles key press for all registers.
        private void onRegisterTBKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null)
            {
                Debug.WriteLine($"Register textbox key press handler called from unexpected with unexpected sender: {(sender ?? "null").ToString()}");
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

                    if (ReferenceEquals(tb, gotoTB))
                    {
                        int maxmem = sess.Machine.MemorySize;
                        if (newValue < maxmem)
                        {
                            tb.BackColor = SystemColors.Window;
                            hexDisplay.CursorAddress = newValue;
                            UpdateMachineDisplay();
                        }
                        else
                        {
                            tb.BackColor = Color.LightPink;
                            LogError($"Address {newValue.ToString("X")} is out of bounds (maximum {maxmem.ToString("X")}).");
                        }
                        break;
                    }
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
                        sess.Machine.RegisterS = newValue;
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
                    if (cursorIndex > 0)
                        tb.SelectionStart = cursorIndex - 1;
                    break;
            }

            e.Handled = true;
        }

        private readonly Color MEMORY_WRITTEN_COLOR = Color.FromArgb(127, Color.Lime);
        private readonly Color MEMORY_READ_COLOR = Color.FromArgb(64, Color.Red);
        private bool OnMemoryChanged(Word addr, int count, bool written)
        {
            int stop = addr + count;
            if (written)
            {
                //Debug.WriteLine($"Memory written: {count} bytes at {addr}.");
                for (int i = addr; i < stop; ++i)
                {
                    var newBox = new ByteMarker(i,
                        MEMORY_WRITTEN_COLOR,
                        sess.Machine.InstructionsExecuted);

                    hexDisplay.Boxes.Add(newBox);
                }
            }
            else
            {
                //Debug.WriteLine($"Memory read: {count} bytes at {addr}.");
                for (int i = addr; i < stop; ++i)
                {
                    var newBox = new ByteMarker(i,
                        MEMORY_READ_COLOR,
                        sess.Machine.InstructionsExecuted);
                    bool added;
                    added = hexDisplay.Boxes.Add(newBox);
                    //Debug.WriteLine($"Box was added? {added}");
                }
            }

            if (breakpointsEnabled)
            {
                foreach (var b in breakpoints)
                {
                    int diff = b.Address - addr;
                    if (diff < 0 || diff > count)
                        continue; // This breakpoint is out of range.
                    if (b.Enabled)
                    {
                        if (b.BreakOnWrite == written || b.BreakOnRead == !written)
                        {
                            Log("Breakpoint hit at {0}.", addr);
                            return true; // Tell machine to break.
                        }
                    }
                }
            }
            return false; // Allow VM execution to continue.
        }

        #region Breakpoints
        // We use a sorted set instead of a dictionary because we want to search for breakpoints over intervals, not just at exact addresses.
        private SortedSet<Breakpoint> breakpoints;
        private bool breakpointsEnabled = true;

        private void setBkptButton_Click(object sender, EventArgs e)
        {
            Word addr = (Word)hexDisplay.CursorAddress;
            Breakpoint bp = breakpoints.FirstInRange(b => b.Address, addr, 1);
            if (bp != null)
            {
                // There is an existing breakpoint at the cursor. Clear it.
                var toRemove = breakpoints.Where(b => b.Address == addr).ToList();
                foreach (var bkpt in toRemove)
                {
                    breakpoints.Remove(bkpt);
                    hexDisplay.Boxes.Remove(bkpt);
                }
                Log("Removed breakpoint at {0}.", addr);
            }
            else
            {
                // No breakpoint exists at the cursor. Create one.
                var newbp = new Breakpoint(addr)
                {
                    Enabled = true,
                    BreakOnRead = true,
                    BreakOnWrite = true
                };
                if (!breakpointsEnabled)
                    newbp.ForceDrawAsDisabled = true;
                breakpoints.Add(newbp);
                bool success = hexDisplay.Boxes.Add(newbp);
                if (success)
                {
                    Log("Added breakpoint at {0}.", addr);
                }
                else
                {
                    Debug.WriteLine($"Adding new ByteMarker for breakpoint at {addr} returned false!");
                }
            }
            UpdateBreakpointGB();
            hexDisplay.Invalidate();
        }

        private void UpdateBreakpointGB()
        {
            Breakpoint bp = breakpoints.FirstInRange(b => b.Address, hexDisplay.CursorAddress, 1);
            if (bp != null)
            {
                bpButton.Text = "Clear (F9)";
                bpEnabledCB.Visible = true;
                bpEnabledCB.Checked = bp.Enabled;
                bpReadCB.Visible = true;
                bpReadCB.Checked = bp.BreakOnRead;
                bpWriteCB.Visible = true;
                bpWriteCB.Checked = bp.BreakOnWrite;
            }
            else
            {
                bpButton.Text = "Set (F9)";
                bpEnabledCB.Visible = false;
                bpReadCB.Visible = false;
                bpWriteCB.Visible = false;
            }
        }

        private void bpEnabledCB_CheckedChanged(object sender, EventArgs e)
        {
            var addr = (Word)hexDisplay.CursorAddress;
            Breakpoint bp = breakpoints.FirstInRange(b => b.Address, addr, 1);
            if (bp != null)
            {
                bp.Enabled = bpEnabledCB.Checked;
                hexDisplay.Invalidate();
            }
            else
            {
                Debug.WriteLine($"Enable checkbox checked but no breakpoint at {addr} was found!");
            }
        }

        private void bpReadCB_CheckedChanged(object sender, EventArgs e)
        {
            var addr = (Word)hexDisplay.CursorAddress;
            Breakpoint bp = breakpoints.FirstInRange(b => b.Address, addr, 1);
            if (bp != null)
            {
                bp.BreakOnRead = bpReadCB.Checked;
            }
            else
            {
                Debug.WriteLine($"Break on read checkbox checked but no breakpoint at {addr} was found!");
            }
        }

        private void bpWriteCB_CheckedChanged(object sender, EventArgs e)
        {
            var addr = (Word)hexDisplay.CursorAddress;
            Breakpoint bp = breakpoints.FirstInRange(b => b.Address, addr, 1);
            if (bp != null)
            {
                bp.BreakOnWrite = bpWriteCB.Checked;
            }
            else
            {
                Debug.WriteLine($"Break on write checkbox checked but no breakpoint at {addr} was found!");
            }
        }
        #endregion

        private void PCfromCursorLabel_Click(object sender, EventArgs e)
        {
            sess.Machine.ProgramCounter = (Word)hexDisplay.CursorAddress;
            UpdateMachineDisplay();
        }

        private void bpDisableOverrideCB_CheckedChanged(object sender, EventArgs e)
        {
            if (bpDisableOverrideCB.Checked)
            {
                breakpointsEnabled = false;
                foreach (var bp in breakpoints)
                {
                    bp.ForceDrawAsDisabled = true;
                }
            }
            else
            {
                breakpointsEnabled = true;
                foreach (var bp in breakpoints)
                {
                    bp.ForceDrawAsDisabled = false;
                }
            }

            hexDisplay.Invalidate();
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            UnloadSession();
        }

        private void UpdateIODevices(object sender, EventArgs e)
        {
            devLB.Items.Clear();
            devLB.Items.AddRange(sess.Machine.Devices.ToArray());
        }

        ConsoleWindow conWindow;
        private void OnDevLBPick(object sender, EventArgs e)
        {
            var dev = (IODevice)devLB.SelectedItem;
            if (dev is ConsoleDevice con)
            {
                conWindow.DisplayConsole(con);
            }
        }

        DeviceManager devman;
        private void manageDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            devman.Show();
            devman.Deactivate += UpdateIODevices;
        }

        private void OnDeactivate(object sender, EventArgs e)
        {
            hexDisplay.Invalidate();
        }

        private void loadSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sess.LoadFromFile("session.txt");
            InitializeMachineDisplay();
            UpdateMachineDisplay();
        }

        private void saveSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sess.SaveToFile("session.txt");
        }
    }
}
