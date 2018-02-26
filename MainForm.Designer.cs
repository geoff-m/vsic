namespace vsic
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
            this.memGB = new System.Windows.Forms.GroupBox();
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
            this.button5 = new System.Windows.Forms.Button();
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
            this.saveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gotoTB = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.utf8RB = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.rawRB = new System.Windows.Forms.RadioButton();
            this.openOBJdialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.openLSTdialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.devLB = new System.Windows.Forms.ListBox();
            this.hexDisplay = new vsic.HexDisplay();
            this.memGB.SuspendLayout();
            this.regGB.SuspendLayout();
            this.pcGB.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.bpGB.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.memGB.Location = new System.Drawing.Point(159, 6);
            this.memGB.Name = "memGB";
            this.memGB.Size = new System.Drawing.Size(698, 572);
            this.memGB.TabIndex = 0;
            this.memGB.TabStop = false;
            this.memGB.Text = "Memory";
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
            this.regGB.Location = new System.Drawing.Point(863, 6);
            this.regGB.Name = "regGB";
            this.regGB.Size = new System.Drawing.Size(151, 238);
            this.regGB.TabIndex = 1;
            this.regGB.TabStop = false;
            this.regGB.Text = "Registers";
            // 
            // ccCB
            // 
            this.ccCB.FormattingEnabled = true;
            this.ccCB.Location = new System.Drawing.Point(32, 201);
            this.ccCB.Name = "ccCB";
            this.ccCB.Size = new System.Drawing.Size(90, 21);
            this.ccCB.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 204);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(21, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "CC";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 102);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "B";
            // 
            // regSTB
            // 
            this.regSTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regSTB.Location = new System.Drawing.Point(32, 123);
            this.regSTB.MaxLength = 6;
            this.regSTB.Name = "regSTB";
            this.regSTB.Size = new System.Drawing.Size(71, 24);
            this.regSTB.TabIndex = 9;
            this.regSTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // regLTB
            // 
            this.regLTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regLTB.Location = new System.Drawing.Point(32, 149);
            this.regLTB.MaxLength = 6;
            this.regLTB.Name = "regLTB";
            this.regLTB.Size = new System.Drawing.Size(71, 24);
            this.regLTB.TabIndex = 15;
            this.regLTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "A";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "L";
            // 
            // regATB
            // 
            this.regATB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regATB.Location = new System.Drawing.Point(32, 19);
            this.regATB.MaxLength = 6;
            this.regATB.Name = "regATB";
            this.regATB.Size = new System.Drawing.Size(71, 24);
            this.regATB.TabIndex = 4;
            this.regATB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "S";
            // 
            // regTTB
            // 
            this.regTTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regTTB.Location = new System.Drawing.Point(32, 71);
            this.regTTB.MaxLength = 6;
            this.regTTB.Name = "regTTB";
            this.regTTB.Size = new System.Drawing.Size(71, 24);
            this.regTTB.TabIndex = 5;
            this.regTTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "F";
            // 
            // regXTB
            // 
            this.regXTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regXTB.Location = new System.Drawing.Point(32, 45);
            this.regXTB.MaxLength = 6;
            this.regXTB.Name = "regXTB";
            this.regXTB.Size = new System.Drawing.Size(71, 24);
            this.regXTB.TabIndex = 6;
            this.regXTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "X";
            // 
            // regFTB
            // 
            this.regFTB.Enabled = false;
            this.regFTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regFTB.Location = new System.Drawing.Point(32, 175);
            this.regFTB.Name = "regFTB";
            this.regFTB.Size = new System.Drawing.Size(91, 24);
            this.regFTB.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "T";
            // 
            // regBTB
            // 
            this.regBTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regBTB.Location = new System.Drawing.Point(32, 97);
            this.regBTB.MaxLength = 6;
            this.regBTB.Name = "regBTB";
            this.regBTB.Size = new System.Drawing.Size(71, 24);
            this.regBTB.TabIndex = 8;
            this.regBTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // pcGB
            // 
            this.pcGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pcGB.Controls.Add(this.PCfromCursorLabel);
            this.pcGB.Controls.Add(this.bpDisableOverrideCB);
            this.pcGB.Controls.Add(this.button5);
            this.pcGB.Controls.Add(this.runButton);
            this.pcGB.Controls.Add(this.stepButton);
            this.pcGB.Controls.Add(this.pcTB);
            this.pcGB.Controls.Add(this.label2);
            this.pcGB.Location = new System.Drawing.Point(863, 256);
            this.pcGB.Name = "pcGB";
            this.pcGB.Size = new System.Drawing.Size(151, 166);
            this.pcGB.TabIndex = 3;
            this.pcGB.TabStop = false;
            this.pcGB.Text = "Program Counter";
            // 
            // PCfromCursorLabel
            // 
            this.PCfromCursorLabel.AutoSize = true;
            this.PCfromCursorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PCfromCursorLabel.Location = new System.Drawing.Point(102, 21);
            this.PCfromCursorLabel.Name = "PCfromCursorLabel";
            this.PCfromCursorLabel.Size = new System.Drawing.Size(43, 13);
            this.PCfromCursorLabel.TabIndex = 19;
            this.PCfromCursorLabel.Text = "000000";
            this.PCfromCursorLabel.Click += new System.EventHandler(this.PCfromCursorLabel_Click);
            // 
            // bpDisableOverrideCB
            // 
            this.bpDisableOverrideCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bpDisableOverrideCB.AutoSize = true;
            this.bpDisableOverrideCB.Location = new System.Drawing.Point(6, 56);
            this.bpDisableOverrideCB.Name = "bpDisableOverrideCB";
            this.bpDisableOverrideCB.Size = new System.Drawing.Size(134, 17);
            this.bpDisableOverrideCB.TabIndex = 6;
            this.bpDisableOverrideCB.Text = "Disable All Breakpoints";
            this.bpDisableOverrideCB.UseVisualStyleBackColor = true;
            this.bpDisableOverrideCB.CheckedChanged += new System.EventHandler(this.bpDisableOverrideCB_CheckedChanged);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(6, 108);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(136, 23);
            this.button5.TabIndex = 6;
            this.button5.Text = "Break (F6)";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.runButton.Location = new System.Drawing.Point(6, 79);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(136, 23);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run (F5)";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // stepButton
            // 
            this.stepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stepButton.Location = new System.Drawing.Point(6, 137);
            this.stepButton.Name = "stepButton";
            this.stepButton.Size = new System.Drawing.Size(136, 23);
            this.stepButton.TabIndex = 10;
            this.stepButton.Text = "Step (F10)";
            this.stepButton.UseVisualStyleBackColor = true;
            this.stepButton.Click += new System.EventHandler(this.stepButton_Click);
            // 
            // pcTB
            // 
            this.pcTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pcTB.Location = new System.Drawing.Point(30, 16);
            this.pcTB.MaxLength = 6;
            this.pcTB.Name = "pcTB";
            this.pcTB.Size = new System.Drawing.Size(71, 24);
            this.pcTB.TabIndex = 17;
            this.pcTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "PC";
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(1026, 129);
            this.logBox.TabIndex = 4;
            this.logBox.Text = "";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.cursorPositionLabel,
            this.selectedBytesLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 739);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1026, 22);
            this.statusStrip.TabIndex = 5;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(897, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.Text = "Loading...";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cursorPositionLabel
            // 
            this.cursorPositionLabel.Name = "cursorPositionLabel";
            this.cursorPositionLabel.Size = new System.Drawing.Size(24, 17);
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
            this.bpEnabledCB.Location = new System.Drawing.Point(7, 58);
            this.bpEnabledCB.Name = "bpEnabledCB";
            this.bpEnabledCB.Size = new System.Drawing.Size(65, 17);
            this.bpEnabledCB.TabIndex = 5;
            this.bpEnabledCB.Text = "Enabled";
            this.bpEnabledCB.UseVisualStyleBackColor = true;
            this.bpEnabledCB.Visible = false;
            this.bpEnabledCB.CheckedChanged += new System.EventHandler(this.bpEnabledCB_CheckedChanged);
            // 
            // bpWriteCB
            // 
            this.bpWriteCB.AutoSize = true;
            this.bpWriteCB.Location = new System.Drawing.Point(7, 104);
            this.bpWriteCB.Name = "bpWriteCB";
            this.bpWriteCB.Size = new System.Drawing.Size(97, 17);
            this.bpWriteCB.TabIndex = 4;
            this.bpWriteCB.Text = "Break on Write";
            this.bpWriteCB.UseVisualStyleBackColor = true;
            this.bpWriteCB.Visible = false;
            this.bpWriteCB.CheckedChanged += new System.EventHandler(this.bpWriteCB_CheckedChanged);
            // 
            // bpReadCB
            // 
            this.bpReadCB.AutoSize = true;
            this.bpReadCB.Location = new System.Drawing.Point(7, 81);
            this.bpReadCB.Name = "bpReadCB";
            this.bpReadCB.Size = new System.Drawing.Size(98, 17);
            this.bpReadCB.TabIndex = 3;
            this.bpReadCB.Text = "Break on Read";
            this.bpReadCB.UseVisualStyleBackColor = true;
            this.bpReadCB.Visible = false;
            this.bpReadCB.CheckedChanged += new System.EventHandler(this.bpReadCB_CheckedChanged);
            // 
            // bpButton
            // 
            this.bpButton.Location = new System.Drawing.Point(6, 19);
            this.bpButton.Name = "bpButton";
            this.bpButton.Size = new System.Drawing.Size(133, 23);
            this.bpButton.TabIndex = 9;
            this.bpButton.Text = "Set (F9)";
            this.bpButton.UseVisualStyleBackColor = true;
            this.bpButton.Click += new System.EventHandler(this.setBkptButton_Click);
            // 
            // bpGB
            // 
            this.bpGB.Controls.Add(this.bpEnabledCB);
            this.bpGB.Controls.Add(this.bpWriteCB);
            this.bpGB.Controls.Add(this.bpReadCB);
            this.bpGB.Controls.Add(this.bpButton);
            this.bpGB.Location = new System.Drawing.Point(8, 347);
            this.bpGB.Name = "bpGB";
            this.bpGB.Size = new System.Drawing.Size(145, 139);
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
            this.menuStrip.Size = new System.Drawing.Size(1026, 24);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMachineToolStripMenuItem,
            this.saveSessionToolStripMenuItem,
            this.revertToolStripMenuItem,
            this.saveToolStripMenuItem,
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
            // saveSessionToolStripMenuItem
            // 
            this.saveSessionToolStripMenuItem.Enabled = false;
            this.saveSessionToolStripMenuItem.Name = "saveSessionToolStripMenuItem";
            this.saveSessionToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.saveSessionToolStripMenuItem.Text = "Load...";
            // 
            // revertToolStripMenuItem
            // 
            this.revertToolStripMenuItem.Enabled = false;
            this.revertToolStripMenuItem.Name = "revertToolStripMenuItem";
            this.revertToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.revertToolStripMenuItem.Text = "Revert";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.saveToolStripMenuItem.Text = "Save";
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
            this.loadLstToolStripMenuItem.Click += new System.EventHandler(this.loadLstToolStripMenuItem_Click);
            // 
            // loadMemoryToolStripMenuItem
            // 
            this.loadMemoryToolStripMenuItem.Name = "loadMemoryToolStripMenuItem";
            this.loadMemoryToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.loadMemoryToolStripMenuItem.Text = "Load Memory at 0...";
            this.loadMemoryToolStripMenuItem.Click += new System.EventHandler(this.loadMemoryToolStripMenuItem_Click);
            // 
            // saveMemoryToolStripMenuItem
            // 
            this.saveMemoryToolStripMenuItem.Enabled = false;
            this.saveMemoryToolStripMenuItem.Name = "saveMemoryToolStripMenuItem";
            this.saveMemoryToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveMemoryToolStripMenuItem.Text = "Save Memory...";
            // 
            // manageDevicesToolStripMenuItem
            // 
            this.manageDevicesToolStripMenuItem.Enabled = false;
            this.manageDevicesToolStripMenuItem.Name = "manageDevicesToolStripMenuItem";
            this.manageDevicesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.manageDevicesToolStripMenuItem.Text = "Manage Devices...";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gotoTB);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Location = new System.Drawing.Point(14, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(139, 214);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Memory Map";
            // 
            // gotoTB
            // 
            this.gotoTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gotoTB.Location = new System.Drawing.Point(46, 152);
            this.gotoTB.MaxLength = 6;
            this.gotoTB.Name = "gotoTB";
            this.gotoTB.Size = new System.Drawing.Size(68, 24);
            this.gotoTB.TabIndex = 1;
            this.gotoTB.Text = "000000";
            this.gotoTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.onRegisterTBKeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 157);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Go to";
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(6, 180);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(127, 23);
            this.button7.TabIndex = 3;
            this.button7.Text = "Go to Disassembly";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton3);
            this.groupBox4.Controls.Add(this.utf8RB);
            this.groupBox4.Controls.Add(this.radioButton4);
            this.groupBox4.Controls.Add(this.rawRB);
            this.groupBox4.Location = new System.Drawing.Point(8, 225);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(145, 116);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Decoding";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Enabled = false;
            this.radioButton3.Location = new System.Drawing.Point(9, 66);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(111, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.Text = "Decimal Unsigned";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.changedEncodingSelection);
            // 
            // utf8RB
            // 
            this.utf8RB.AutoSize = true;
            this.utf8RB.Location = new System.Drawing.Point(9, 90);
            this.utf8RB.Name = "utf8RB";
            this.utf8RB.Size = new System.Drawing.Size(85, 17);
            this.utf8RB.TabIndex = 1;
            this.utf8RB.Text = "Text (UTF-8)";
            this.utf8RB.UseVisualStyleBackColor = true;
            this.utf8RB.CheckedChanged += new System.EventHandler(this.changedEncodingSelection);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Enabled = false;
            this.radioButton4.Location = new System.Drawing.Point(9, 43);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(99, 17);
            this.radioButton4.TabIndex = 3;
            this.radioButton4.Text = "Decimal Signed";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.changedEncodingSelection);
            // 
            // rawRB
            // 
            this.rawRB.AutoSize = true;
            this.rawRB.Checked = true;
            this.rawRB.Location = new System.Drawing.Point(9, 20);
            this.rawRB.Name = "rawRB";
            this.rawRB.Size = new System.Drawing.Size(69, 17);
            this.rawRB.TabIndex = 0;
            this.rawRB.TabStop = true;
            this.rawRB.Text = "Raw Hex";
            this.rawRB.UseVisualStyleBackColor = true;
            this.rawRB.CheckedChanged += new System.EventHandler(this.changedEncodingSelection);
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
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.regGB);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.memGB);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.pcGB);
            this.splitContainer1.Panel1.Controls.Add(this.bpGB);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.logBox);
            this.splitContainer1.Size = new System.Drawing.Size(1026, 715);
            this.splitContainer1.SplitterDistance = 580;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // openLSTdialog
            // 
            this.openLSTdialog.Filter = "sicasm LST files (*.lst)|*.lst";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.devLB);
            this.groupBox2.Location = new System.Drawing.Point(863, 428);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 147);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "I/O Devices";
            // 
            // devLB
            // 
            this.devLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devLB.FormattingEnabled = true;
            this.devLB.Location = new System.Drawing.Point(3, 16);
            this.devLB.Name = "devLB";
            this.devLB.Size = new System.Drawing.Size(145, 128);
            this.devLB.TabIndex = 0;
            // 
            // hexDisplay
            // 
            this.hexDisplay.AddressDigits = 6;
            this.hexDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexDisplay.CursorAddress = 0;
            this.hexDisplay.Data = null;
            this.hexDisplay.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexDisplay.Location = new System.Drawing.Point(3, 16);
            this.hexDisplay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.hexDisplay.Name = "hexDisplay";
            this.hexDisplay.Size = new System.Drawing.Size(688, 553);
            this.hexDisplay.StartAddress = 0;
            this.hexDisplay.TabIndex = 1;
            this.hexDisplay.WordDigits = 6;
            this.hexDisplay.WordEncoding = vsic.HexDisplay.Encoding.Raw;
            this.hexDisplay.CursorAddressChanged += new System.EventHandler(this.OnCursorMove);
            this.hexDisplay.Scroll += new System.Windows.Forms.ScrollEventHandler(this.onHexDisplayScroll);
            this.hexDisplay.Enter += new System.EventHandler(this.onHexDisplayFocus);
            this.hexDisplay.Leave += new System.EventHandler(this.onHexDisplayBlur);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 761);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(485, 728);
            this.Name = "MainForm";
            this.Text = "Virtual SICXE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Button button5;
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
        private System.Windows.Forms.ToolStripMenuItem saveSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem machineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageDevicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadOBJToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem loadLstToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openLSTdialog;
        private System.Windows.Forms.Label PCfromCursorLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox devLB;
    }
}

