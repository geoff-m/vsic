namespace Visual_SICXE
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.memGB = new System.Windows.Forms.GroupBox();
            this.hexDisplay = new Visual_SICXE.HexDisplay();
            this.regGB = new System.Windows.Forms.GroupBox();
            this.ccCB = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.regSTB = new System.Windows.Forms.TextBox();
            this.regLTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.regATB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.regTTB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.regXTB = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.regFTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.regBTB = new System.Windows.Forms.TextBox();
            this.pcGB = new System.Windows.Forms.GroupBox();
            this.PCfromCursorLabel = new System.Windows.Forms.Label();
            this.bpDisableOverrideCB = new System.Windows.Forms.CheckBox();
            this.runButton = new System.Windows.Forms.Button();
            this.stepButton = new System.Windows.Forms.Button();
            this.pcTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cursorPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.selectedBytesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.bpEnabledCB = new System.Windows.Forms.CheckBox();
            this.bpWriteCB = new System.Windows.Forms.CheckBox();
            this.bpReadCB = new System.Windows.Forms.CheckBox();
            this.bpButton = new System.Windows.Forms.Button();
            this.bpGB = new System.Windows.Forms.GroupBox();
            this.openMemoryDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMachineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLstToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.watchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disassemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoTB = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.utf8RB = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.rawRB = new System.Windows.Forms.RadioButton();
            this.openOBJdialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.devLB = new System.Windows.Forms.ListBox();
            this.openLSTdialog = new System.Windows.Forms.OpenFileDialog();
            this.openSessionDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveSessionDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveMemoryDialog = new System.Windows.Forms.SaveFileDialog();
            this.memGB.SuspendLayout();
            this.regGB.SuspendLayout();
            this.pcGB.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.bpGB.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // memGB
            // 
            this.memGB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.memGB.Controls.Add(this.hexDisplay);
            this.memGB.Location = new System.Drawing.Point(186, 7);
            this.memGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.memGB.Name = "memGB";
            this.memGB.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.memGB.Size = new System.Drawing.Size(826, 665);
            this.memGB.TabIndex = 0;
            this.memGB.TabStop = false;
            this.memGB.Text = "Memory";
            // 
            // hexDisplay
            // 
            this.hexDisplay.AddressDigits = 6;
            this.hexDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexDisplay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.hexDisplay.CursorAddress = 0;
            this.hexDisplay.Data = null;
            this.hexDisplay.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hexDisplay.Location = new System.Drawing.Point(4, 18);
            this.hexDisplay.Margin = new System.Windows.Forms.Padding(5);
            this.hexDisplay.Name = "hexDisplay";
            this.hexDisplay.Size = new System.Drawing.Size(815, 643);
            this.hexDisplay.StartAddress = 0;
            this.hexDisplay.TabIndex = 1;
            this.hexDisplay.WordDigits = 6;
            this.hexDisplay.WordEncoding = Visual_SICXE.HexDisplay.Encoding.Raw;
            this.hexDisplay.CursorAddressChanged += new System.EventHandler(this.OnHexDisplayCursorMove);
            this.hexDisplay.Enter += new System.EventHandler(this.onHexDisplayFocus);
            this.hexDisplay.Leave += new System.EventHandler(this.onHexDisplayBlur);
            // 
            // regGB
            // 
            this.regGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.regGB.Controls.Add(this.ccCB);
            this.regGB.Controls.Add(this.label10);
            this.regGB.Controls.Add(this.label8);
            this.regGB.Controls.Add(this.regSTB);
            this.regGB.Controls.Add(this.regLTB);
            this.regGB.Controls.Add(this.label1);
            this.regGB.Controls.Add(this.label7);
            this.regGB.Controls.Add(this.regATB);
            this.regGB.Controls.Add(this.label6);
            this.regGB.Controls.Add(this.regTTB);
            this.regGB.Controls.Add(this.label5);
            this.regGB.Controls.Add(this.regXTB);
            this.regGB.Controls.Add(this.label4);
            this.regGB.Controls.Add(this.regFTB);
            this.regGB.Controls.Add(this.label3);
            this.regGB.Controls.Add(this.regBTB);
            this.regGB.Location = new System.Drawing.Point(1020, 7);
            this.regGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regGB.Name = "regGB";
            this.regGB.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regGB.Size = new System.Drawing.Size(176, 275);
            this.regGB.TabIndex = 1;
            this.regGB.TabStop = false;
            this.regGB.Text = "Registers";
            // 
            // ccCB
            // 
            this.ccCB.FormattingEnabled = true;
            this.ccCB.Location = new System.Drawing.Point(37, 232);
            this.ccCB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ccCB.Name = "ccCB";
            this.ccCB.Size = new System.Drawing.Size(104, 23);
            this.ccCB.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 235);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(23, 15);
            this.label10.TabIndex = 17;
            this.label10.Text = "CC";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 118);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "B";
            // 
            // regSTB
            // 
            this.regSTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regSTB.Location = new System.Drawing.Point(37, 142);
            this.regSTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regSTB.MaxLength = 6;
            this.regSTB.Name = "regSTB";
            this.regSTB.Size = new System.Drawing.Size(82, 24);
            this.regSTB.TabIndex = 9;
            this.regSTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // regLTB
            // 
            this.regLTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regLTB.Location = new System.Drawing.Point(37, 172);
            this.regLTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regLTB.MaxLength = 6;
            this.regLTB.Name = "regLTB";
            this.regLTB.Size = new System.Drawing.Size(82, 24);
            this.regLTB.TabIndex = 15;
            this.regLTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "A";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 178);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "L";
            // 
            // regATB
            // 
            this.regATB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regATB.Location = new System.Drawing.Point(37, 22);
            this.regATB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regATB.MaxLength = 6;
            this.regATB.Name = "regATB";
            this.regATB.Size = new System.Drawing.Size(82, 24);
            this.regATB.TabIndex = 4;
            this.regATB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 148);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "S";
            // 
            // regTTB
            // 
            this.regTTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regTTB.Location = new System.Drawing.Point(37, 82);
            this.regTTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regTTB.MaxLength = 6;
            this.regTTB.Name = "regTTB";
            this.regTTB.Size = new System.Drawing.Size(82, 24);
            this.regTTB.TabIndex = 5;
            this.regTTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(12, 208);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "F";
            // 
            // regXTB
            // 
            this.regXTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regXTB.Location = new System.Drawing.Point(37, 52);
            this.regXTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regXTB.MaxLength = 6;
            this.regXTB.Name = "regXTB";
            this.regXTB.Size = new System.Drawing.Size(82, 24);
            this.regXTB.TabIndex = 6;
            this.regXTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 59);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "X";
            // 
            // regFTB
            // 
            this.regFTB.Enabled = false;
            this.regFTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regFTB.Location = new System.Drawing.Point(37, 202);
            this.regFTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regFTB.Name = "regFTB";
            this.regFTB.Size = new System.Drawing.Size(106, 24);
            this.regFTB.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 90);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "T";
            // 
            // regBTB
            // 
            this.regBTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regBTB.Location = new System.Drawing.Point(37, 112);
            this.regBTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.regBTB.MaxLength = 6;
            this.regBTB.Name = "regBTB";
            this.regBTB.Size = new System.Drawing.Size(82, 24);
            this.regBTB.TabIndex = 8;
            this.regBTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // pcGB
            // 
            this.pcGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pcGB.Controls.Add(this.PCfromCursorLabel);
            this.pcGB.Controls.Add(this.bpDisableOverrideCB);
            this.pcGB.Controls.Add(this.runButton);
            this.pcGB.Controls.Add(this.stepButton);
            this.pcGB.Controls.Add(this.pcTB);
            this.pcGB.Controls.Add(this.label2);
            this.pcGB.Location = new System.Drawing.Point(1020, 295);
            this.pcGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pcGB.Name = "pcGB";
            this.pcGB.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pcGB.Size = new System.Drawing.Size(176, 166);
            this.pcGB.TabIndex = 3;
            this.pcGB.TabStop = false;
            this.pcGB.Text = "Program Counter";
            // 
            // PCfromCursorLabel
            // 
            this.PCfromCursorLabel.AutoSize = true;
            this.PCfromCursorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.PCfromCursorLabel.Location = new System.Drawing.Point(119, 24);
            this.PCfromCursorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PCfromCursorLabel.Name = "PCfromCursorLabel";
            this.PCfromCursorLabel.Size = new System.Drawing.Size(43, 13);
            this.PCfromCursorLabel.TabIndex = 19;
            this.PCfromCursorLabel.Text = "000000";
            this.PCfromCursorLabel.Click += new System.EventHandler(this.PCfromCursorLabel_Click);
            // 
            // bpDisableOverrideCB
            // 
            this.bpDisableOverrideCB.AutoSize = true;
            this.bpDisableOverrideCB.Location = new System.Drawing.Point(7, 65);
            this.bpDisableOverrideCB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bpDisableOverrideCB.Name = "bpDisableOverrideCB";
            this.bpDisableOverrideCB.Size = new System.Drawing.Size(146, 19);
            this.bpDisableOverrideCB.TabIndex = 6;
            this.bpDisableOverrideCB.Text = "Disable All Breakpoints";
            this.bpDisableOverrideCB.UseVisualStyleBackColor = true;
            this.bpDisableOverrideCB.CheckedChanged += new System.EventHandler(this.bpDisableOverrideCB_CheckedChanged);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(7, 91);
            this.runButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(159, 27);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run (F5)";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.OnClickRunButton);
            // 
            // stepButton
            // 
            this.stepButton.Location = new System.Drawing.Point(7, 127);
            this.stepButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stepButton.Name = "stepButton";
            this.stepButton.Size = new System.Drawing.Size(159, 27);
            this.stepButton.TabIndex = 10;
            this.stepButton.Text = "Step (F10)";
            this.stepButton.UseVisualStyleBackColor = true;
            this.stepButton.Click += new System.EventHandler(this.OnClickStepButton);
            // 
            // pcTB
            // 
            this.pcTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.pcTB.Location = new System.Drawing.Point(35, 18);
            this.pcTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pcTB.MaxLength = 6;
            this.pcTB.Name = "pcTB";
            this.pcTB.Size = new System.Drawing.Size(82, 24);
            this.pcTB.TabIndex = 17;
            this.pcTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "PC";
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(1210, 151);
            this.logBox.TabIndex = 4;
            this.logBox.Text = "";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.cursorPositionLabel,
            this.selectedBytesLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 856);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(1210, 22);
            this.statusStrip.TabIndex = 5;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(1078, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.Text = "Loading...";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cursorPositionLabel
            // 
            this.cursorPositionLabel.Name = "cursorPositionLabel";
            this.cursorPositionLabel.Size = new System.Drawing.Size(25, 17);
            this.cursorPositionLabel.Text = "0x0";
            this.cursorPositionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // selectedBytesLabel
            // 
            this.selectedBytesLabel.Name = "selectedBytesLabel";
            this.selectedBytesLabel.Size = new System.Drawing.Size(90, 17);
            this.selectedBytesLabel.Text = "0 bytes selected";
            this.selectedBytesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bpEnabledCB
            // 
            this.bpEnabledCB.AutoSize = true;
            this.bpEnabledCB.Location = new System.Drawing.Point(8, 67);
            this.bpEnabledCB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bpEnabledCB.Name = "bpEnabledCB";
            this.bpEnabledCB.Size = new System.Drawing.Size(68, 19);
            this.bpEnabledCB.TabIndex = 5;
            this.bpEnabledCB.Text = "Enabled";
            this.bpEnabledCB.UseVisualStyleBackColor = true;
            this.bpEnabledCB.Visible = false;
            this.bpEnabledCB.CheckedChanged += new System.EventHandler(this.bpEnabledCB_CheckedChanged);
            // 
            // bpWriteCB
            // 
            this.bpWriteCB.AutoSize = true;
            this.bpWriteCB.Location = new System.Drawing.Point(8, 120);
            this.bpWriteCB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bpWriteCB.Name = "bpWriteCB";
            this.bpWriteCB.Size = new System.Drawing.Size(103, 19);
            this.bpWriteCB.TabIndex = 4;
            this.bpWriteCB.Text = "Break on Write";
            this.bpWriteCB.UseVisualStyleBackColor = true;
            this.bpWriteCB.Visible = false;
            this.bpWriteCB.CheckedChanged += new System.EventHandler(this.bpWriteCB_CheckedChanged);
            // 
            // bpReadCB
            // 
            this.bpReadCB.AutoSize = true;
            this.bpReadCB.Location = new System.Drawing.Point(8, 93);
            this.bpReadCB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bpReadCB.Name = "bpReadCB";
            this.bpReadCB.Size = new System.Drawing.Size(101, 19);
            this.bpReadCB.TabIndex = 3;
            this.bpReadCB.Text = "Break on Read";
            this.bpReadCB.UseVisualStyleBackColor = true;
            this.bpReadCB.Visible = false;
            this.bpReadCB.CheckedChanged += new System.EventHandler(this.bpReadCB_CheckedChanged);
            // 
            // bpButton
            // 
            this.bpButton.Location = new System.Drawing.Point(7, 22);
            this.bpButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bpButton.Name = "bpButton";
            this.bpButton.Size = new System.Drawing.Size(155, 27);
            this.bpButton.TabIndex = 9;
            this.bpButton.Text = "Set (F9)";
            this.bpButton.UseVisualStyleBackColor = true;
            this.bpButton.Click += new System.EventHandler(this.OnClickSetBreakpointButton);
            // 
            // bpGB
            // 
            this.bpGB.Controls.Add(this.bpEnabledCB);
            this.bpGB.Controls.Add(this.bpWriteCB);
            this.bpGB.Controls.Add(this.bpReadCB);
            this.bpGB.Controls.Add(this.bpButton);
            this.bpGB.Location = new System.Drawing.Point(12, 148);
            this.bpGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bpGB.Name = "bpGB";
            this.bpGB.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bpGB.Size = new System.Drawing.Size(169, 160);
            this.bpGB.TabIndex = 0;
            this.bpGB.TabStop = false;
            this.bpGB.Text = "Breakpoint";
            // 
            // openMemoryDialog
            // 
            this.openMemoryDialog.Filter = "Binary image (*.bin)|*.bin|All files|*.*";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.machineToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1210, 24);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMachineToolStripMenuItem,
            this.loadSessionToolStripMenuItem,
            this.revertToolStripMenuItem,
            this.saveSessionToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.fileToolStripMenuItem.Text = "Session";
            // 
            // newMachineToolStripMenuItem
            // 
            this.newMachineToolStripMenuItem.Name = "newMachineToolStripMenuItem";
            this.newMachineToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.newMachineToolStripMenuItem.Text = "New";
            this.newMachineToolStripMenuItem.Click += new System.EventHandler(this.newMachineToolStripMenuItem_Click);
            // 
            // loadSessionToolStripMenuItem
            // 
            this.loadSessionToolStripMenuItem.Name = "loadSessionToolStripMenuItem";
            this.loadSessionToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.loadSessionToolStripMenuItem.Text = "Load...";
            this.loadSessionToolStripMenuItem.Click += new System.EventHandler(this.loadSessionToolStripMenuItem_Click);
            // 
            // revertToolStripMenuItem
            // 
            this.revertToolStripMenuItem.Enabled = false;
            this.revertToolStripMenuItem.Name = "revertToolStripMenuItem";
            this.revertToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.revertToolStripMenuItem.Text = "Revert";
            // 
            // saveSessionToolStripMenuItem
            // 
            this.saveSessionToolStripMenuItem.Name = "saveSessionToolStripMenuItem";
            this.saveSessionToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.saveSessionToolStripMenuItem.Text = "Save";
            this.saveSessionToolStripMenuItem.Click += new System.EventHandler(this.saveSessionToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // machineToolStripMenuItem
            // 
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadOBJToolStripMenuItem,
            this.loadLstToolStripMenuItem,
            this.loadMemoryToolStripMenuItem,
            this.saveMemoryToolStripMenuItem,
            this.manageDevicesToolStripMenuItem});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            this.machineToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.machineToolStripMenuItem.Text = "Machine";
            // 
            // loadOBJToolStripMenuItem
            // 
            this.loadOBJToolStripMenuItem.Name = "loadOBJToolStripMenuItem";
            this.loadOBJToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.loadOBJToolStripMenuItem.Text = "Load Obj...";
            this.loadOBJToolStripMenuItem.Click += new System.EventHandler(this.loadOBJToolStripMenuItem_Click);
            // 
            // loadLstToolStripMenuItem
            // 
            this.loadLstToolStripMenuItem.Enabled = false;
            this.loadLstToolStripMenuItem.Name = "loadLstToolStripMenuItem";
            this.loadLstToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.loadLstToolStripMenuItem.Text = "Load Lst...";
            // 
            // loadMemoryToolStripMenuItem
            // 
            this.loadMemoryToolStripMenuItem.Name = "loadMemoryToolStripMenuItem";
            this.loadMemoryToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.loadMemoryToolStripMenuItem.Text = "Load Memory at 0...";
            this.loadMemoryToolStripMenuItem.Click += new System.EventHandler(this.OnClickLoadMemoryToolStripMenuItem);
            // 
            // saveMemoryToolStripMenuItem
            // 
            this.saveMemoryToolStripMenuItem.Name = "saveMemoryToolStripMenuItem";
            this.saveMemoryToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveMemoryToolStripMenuItem.Text = "Save All Memory...";
            this.saveMemoryToolStripMenuItem.Click += new System.EventHandler(this.saveMemoryToolStripMenuItem_Click);
            // 
            // manageDevicesToolStripMenuItem
            // 
            this.manageDevicesToolStripMenuItem.Name = "manageDevicesToolStripMenuItem";
            this.manageDevicesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.manageDevicesToolStripMenuItem.Text = "Manage Devices...";
            this.manageDevicesToolStripMenuItem.Click += new System.EventHandler(this.manageDevicesToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.watchesToolStripMenuItem,
            this.breakpointsToolStripMenuItem,
            this.disassemblyToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // watchesToolStripMenuItem
            // 
            this.watchesToolStripMenuItem.Enabled = false;
            this.watchesToolStripMenuItem.Name = "watchesToolStripMenuItem";
            this.watchesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.watchesToolStripMenuItem.Text = "Watches";
            this.watchesToolStripMenuItem.Click += new System.EventHandler(this.watchesToolStripMenuItem_Click);
            // 
            // breakpointsToolStripMenuItem
            // 
            this.breakpointsToolStripMenuItem.Enabled = false;
            this.breakpointsToolStripMenuItem.Name = "breakpointsToolStripMenuItem";
            this.breakpointsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.breakpointsToolStripMenuItem.Text = "Breakpoints";
            // 
            // disassemblyToolStripMenuItem
            // 
            this.disassemblyToolStripMenuItem.Enabled = false;
            this.disassemblyToolStripMenuItem.Name = "disassemblyToolStripMenuItem";
            this.disassemblyToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.disassemblyToolStripMenuItem.Text = "Disassembly";
            // 
            // gotoTB
            // 
            this.gotoTB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gotoTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gotoTB.Location = new System.Drawing.Point(1117, 635);
            this.gotoTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gotoTB.MaxLength = 6;
            this.gotoTB.Name = "gotoTB";
            this.gotoTB.Size = new System.Drawing.Size(79, 24);
            this.gotoTB.TabIndex = 1;
            this.gotoTB.Text = "000000";
            this.gotoTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1024, 641);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "Go to address";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton3);
            this.groupBox4.Controls.Add(this.utf8RB);
            this.groupBox4.Controls.Add(this.radioButton4);
            this.groupBox4.Controls.Add(this.rawRB);
            this.groupBox4.Location = new System.Drawing.Point(12, 7);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox4.Size = new System.Drawing.Size(169, 134);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Decoding";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Enabled = false;
            this.radioButton3.Location = new System.Drawing.Point(10, 76);
            this.radioButton3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(121, 19);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.Text = "Decimal Unsigned";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.OnChangedEncodingSelection);
            // 
            // utf8RB
            // 
            this.utf8RB.AutoSize = true;
            this.utf8RB.Location = new System.Drawing.Point(10, 104);
            this.utf8RB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.utf8RB.Name = "utf8RB";
            this.utf8RB.Size = new System.Drawing.Size(88, 19);
            this.utf8RB.TabIndex = 1;
            this.utf8RB.Text = "Text (UTF-8)";
            this.utf8RB.UseVisualStyleBackColor = true;
            this.utf8RB.CheckedChanged += new System.EventHandler(this.OnChangedEncodingSelection);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Enabled = false;
            this.radioButton4.Location = new System.Drawing.Point(10, 50);
            this.radioButton4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(107, 19);
            this.radioButton4.TabIndex = 3;
            this.radioButton4.Text = "Decimal Signed";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.OnChangedEncodingSelection);
            // 
            // rawRB
            // 
            this.rawRB.AutoSize = true;
            this.rawRB.Checked = true;
            this.rawRB.Location = new System.Drawing.Point(10, 23);
            this.rawRB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rawRB.Name = "rawRB";
            this.rawRB.Size = new System.Drawing.Size(71, 19);
            this.rawRB.TabIndex = 0;
            this.rawRB.TabStop = true;
            this.rawRB.Text = "Raw Hex";
            this.rawRB.UseVisualStyleBackColor = true;
            this.rawRB.CheckedChanged += new System.EventHandler(this.OnChangedEncodingSelection);
            // 
            // openOBJdialog
            // 
            this.openOBJdialog.Filter = "SIC/XE OBJ files (*.obj)|*.obj";
            this.openOBJdialog.SupportMultiDottedExtensions = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gotoTB);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.label9);
            this.splitContainer1.Panel1.Controls.Add(this.regGB);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.pcGB);
            this.splitContainer1.Panel1.Controls.Add(this.bpGB);
            this.splitContainer1.Panel1.Controls.Add(this.memGB);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.logBox);
            this.splitContainer1.Size = new System.Drawing.Size(1210, 832);
            this.splitContainer1.SplitterDistance = 674;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.devLB);
            this.groupBox2.Location = new System.Drawing.Point(12, 320);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(169, 170);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "I/O Devices";
            // 
            // devLB
            // 
            this.devLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.devLB.FormattingEnabled = true;
            this.devLB.ItemHeight = 15;
            this.devLB.Location = new System.Drawing.Point(4, 19);
            this.devLB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.devLB.Name = "devLB";
            this.devLB.Size = new System.Drawing.Size(161, 148);
            this.devLB.TabIndex = 0;
            this.devLB.DoubleClick += new System.EventHandler(this.OnDevLBPick);
            // 
            // openLSTdialog
            // 
            this.openLSTdialog.Filter = "sicasm LST files (*.lst)|*.lst";
            // 
            // openSessionDialog
            // 
            this.openSessionDialog.Filter = "Saved sessions|*.sav";
            this.openSessionDialog.Title = "Load saved session";
            // 
            // saveSessionDialog
            // 
            this.saveSessionDialog.DefaultExt = "sav";
            this.saveSessionDialog.Filter = "Saved sessions|*.sav|All files|*.*";
            // 
            // saveMemoryDialog
            // 
            this.saveMemoryDialog.SupportMultiDottedExtensions = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1210, 878);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(563, 834);
            this.Name = "MainForm";
            this.Text = "Visual SICXE";
            this.Deactivate += new System.EventHandler(this.OnDeactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.OnResize);
            this.memGB.ResumeLayout(false);
            this.regGB.ResumeLayout(false);
            this.regGB.PerformLayout();
            this.pcGB.ResumeLayout(false);
            this.pcGB.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.bpGB.ResumeLayout(false);
            this.bpGB.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox memGB;
        private System.Windows.Forms.GroupBox regGB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox regSTB;
        private System.Windows.Forms.TextBox regLTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox regATB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox regTTB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox regXTB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox regFTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox regBTB;
        private System.Windows.Forms.GroupBox pcGB;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button stepButton;
        private System.Windows.Forms.TextBox pcTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.CheckBox bpDisableOverrideCB;
        private System.Windows.Forms.CheckBox bpEnabledCB;
        private System.Windows.Forms.CheckBox bpWriteCB;
        private System.Windows.Forms.CheckBox bpReadCB;
        private System.Windows.Forms.Button bpButton;
        private System.Windows.Forms.GroupBox bpGB;
        private System.Windows.Forms.OpenFileDialog openMemoryDialog;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newMachineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem machineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageDevicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadOBJToolStripMenuItem;
        private HexDisplay hexDisplay;
        private System.Windows.Forms.ToolStripStatusLabel cursorPositionLabel;
        private System.Windows.Forms.ToolStripStatusLabel selectedBytesLabel;
        private System.Windows.Forms.TextBox gotoTB;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton utf8RB;
        private System.Windows.Forms.RadioButton rawRB;
        private System.Windows.Forms.OpenFileDialog openOBJdialog;
        private System.Windows.Forms.ComboBox ccCB;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem breakpointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disassemblyToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem loadLstToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openLSTdialog;
        private System.Windows.Forms.Label PCfromCursorLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox devLB;
        private System.Windows.Forms.OpenFileDialog openSessionDialog;
        private System.Windows.Forms.SaveFileDialog saveSessionDialog;
        private System.Windows.Forms.SaveFileDialog saveMemoryDialog;
    }
}

