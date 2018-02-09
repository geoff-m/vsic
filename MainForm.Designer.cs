namespace sicsim
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
            this.memGrpBox = new System.Windows.Forms.GroupBox();
            this.hexDisplay = new sicsim.HexDisplay();
            this.regGrpBox = new System.Windows.Forms.GroupBox();
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.pcTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.button7 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.button8 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.openMemoryDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cursorPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.selectedBytesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.label9 = new System.Windows.Forms.Label();
            this.gotoTB = new System.Windows.Forms.TextBox();
            this.memGrpBox.SuspendLayout();
            this.regGrpBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // memGrpBox
            // 
            this.memGrpBox.Controls.Add(this.hexDisplay);
            this.memGrpBox.Location = new System.Drawing.Point(163, 27);
            this.memGrpBox.Name = "memGrpBox";
            this.memGrpBox.Size = new System.Drawing.Size(716, 572);
            this.memGrpBox.TabIndex = 0;
            this.memGrpBox.TabStop = false;
            this.memGrpBox.Text = "Memory";
            // 
            // hexDisplay
            // 
            this.hexDisplay.AddressDigits = 6;
            this.hexDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexDisplay.CursorAddress = 0;
            this.hexDisplay.Data = null;
            this.hexDisplay.FontSize = 10F;
            this.hexDisplay.Location = new System.Drawing.Point(3, 16);
            this.hexDisplay.Name = "hexDisplay";
            this.hexDisplay.Size = new System.Drawing.Size(710, 553);
            this.hexDisplay.StartAddress = 0;
            this.hexDisplay.TabIndex = 0;
            this.hexDisplay.WordDigits = 6;
            // 
            // regGrpBox
            // 
            this.regGrpBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.regGrpBox.Controls.Add(this.label8);
            this.regGrpBox.Controls.Add(this.regSTB);
            this.regGrpBox.Controls.Add(this.regLTB);
            this.regGrpBox.Controls.Add(this.label1);
            this.regGrpBox.Controls.Add(this.label7);
            this.regGrpBox.Controls.Add(this.regATB);
            this.regGrpBox.Controls.Add(this.label6);
            this.regGrpBox.Controls.Add(this.regTTB);
            this.regGrpBox.Controls.Add(this.label5);
            this.regGrpBox.Controls.Add(this.regXTB);
            this.regGrpBox.Controls.Add(this.label4);
            this.regGrpBox.Controls.Add(this.regFTB);
            this.regGrpBox.Controls.Add(this.label3);
            this.regGrpBox.Controls.Add(this.regBTB);
            this.regGrpBox.Location = new System.Drawing.Point(888, 27);
            this.regGrpBox.Name = "regGrpBox";
            this.regGrpBox.Size = new System.Drawing.Size(151, 214);
            this.regGrpBox.TabIndex = 1;
            this.regGrpBox.TabStop = false;
            this.regGrpBox.Text = "Registers";
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
            this.regSTB.Name = "regSTB";
            this.regSTB.Size = new System.Drawing.Size(71, 24);
            this.regSTB.TabIndex = 9;
            // 
            // regLTB
            // 
            this.regLTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regLTB.Location = new System.Drawing.Point(32, 149);
            this.regLTB.Name = "regLTB";
            this.regLTB.Size = new System.Drawing.Size(71, 24);
            this.regLTB.TabIndex = 15;
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
            this.label7.Location = new System.Drawing.Point(13, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "L";
            // 
            // regATB
            // 
            this.regATB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regATB.Location = new System.Drawing.Point(32, 19);
            this.regATB.Name = "regATB";
            this.regATB.Size = new System.Drawing.Size(71, 24);
            this.regATB.TabIndex = 4;
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
            this.regTTB.Location = new System.Drawing.Point(32, 45);
            this.regTTB.Name = "regTTB";
            this.regTTB.Size = new System.Drawing.Size(71, 24);
            this.regTTB.TabIndex = 5;
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
            this.regXTB.Location = new System.Drawing.Point(32, 71);
            this.regXTB.Name = "regXTB";
            this.regXTB.Size = new System.Drawing.Size(71, 24);
            this.regXTB.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "X";
            // 
            // regFTB
            // 
            this.regFTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regFTB.Location = new System.Drawing.Point(32, 175);
            this.regFTB.Name = "regFTB";
            this.regFTB.Size = new System.Drawing.Size(91, 24);
            this.regFTB.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "T";
            // 
            // regBTB
            // 
            this.regBTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regBTB.Location = new System.Drawing.Point(32, 97);
            this.regBTB.Name = "regBTB";
            this.regBTB.Size = new System.Drawing.Size(71, 24);
            this.regBTB.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.checkBox4);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.pcTB);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(888, 247);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 166);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Program Counter";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(9, 139);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(134, 17);
            this.checkBox4.TabIndex = 6;
            this.checkBox4.Text = "Disable All Breakpoints";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(9, 81);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(136, 23);
            this.button5.TabIndex = 20;
            this.button5.Text = "Break (F6)";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(9, 52);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(136, 23);
            this.button4.TabIndex = 19;
            this.button4.Text = "Run (F5)";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(9, 110);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(136, 23);
            this.button3.TabIndex = 18;
            this.button3.Text = "Step (F10)";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // pcTB
            // 
            this.pcTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pcTB.Location = new System.Drawing.Point(30, 16);
            this.pcTB.Name = "pcTB";
            this.pcTB.Size = new System.Drawing.Size(71, 24);
            this.pcTB.TabIndex = 17;
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
            this.logBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logBox.Location = new System.Drawing.Point(0, 605);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(1051, 127);
            this.logBox.TabIndex = 4;
            this.logBox.Text = "";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.cursorPositionLabel,
            this.selectedBytesLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 735);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1051, 22);
            this.statusStrip.TabIndex = 5;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(904, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Loading...";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(12, 367);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(139, 23);
            this.button7.TabIndex = 3;
            this.button7.Text = "Go to Disassembly";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(7, 58);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(65, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Enabled";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(7, 104);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(97, 17);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "Break on Write";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(7, 81);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(98, 17);
            this.checkBox3.TabIndex = 3;
            this.checkBox3.Text = "Break on Read";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(6, 19);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(127, 23);
            this.button8.TabIndex = 0;
            this.button8.Text = "Set (F9)";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Controls.Add(this.checkBox2);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Controls.Add(this.button8);
            this.groupBox3.Location = new System.Drawing.Point(6, 460);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(145, 139);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Breakpoint";
            // 
            // openMemoryDialog
            // 
            this.openMemoryDialog.Filter = "Binary image|*.bin|All files|*.*";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.machineToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1051, 24);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSessionToolStripMenuItem,
            this.saveSessionToolStripMenuItem,
            this.revertToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.fileToolStripMenuItem.Text = "Session";
            // 
            // loadSessionToolStripMenuItem
            // 
            this.loadSessionToolStripMenuItem.Name = "loadSessionToolStripMenuItem";
            this.loadSessionToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.loadSessionToolStripMenuItem.Text = "New";
            // 
            // saveSessionToolStripMenuItem
            // 
            this.saveSessionToolStripMenuItem.Name = "saveSessionToolStripMenuItem";
            this.saveSessionToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.saveSessionToolStripMenuItem.Text = "Load...";
            // 
            // revertToolStripMenuItem
            // 
            this.revertToolStripMenuItem.Name = "revertToolStripMenuItem";
            this.revertToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.revertToolStripMenuItem.Text = "Revert";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // machineToolStripMenuItem
            // 
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadOBJToolStripMenuItem,
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
            this.loadOBJToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.loadOBJToolStripMenuItem.Text = "Load OBJ...";
            // 
            // loadMemoryToolStripMenuItem
            // 
            this.loadMemoryToolStripMenuItem.Name = "loadMemoryToolStripMenuItem";
            this.loadMemoryToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.loadMemoryToolStripMenuItem.Text = "Load Memory to [addr]...";
            this.loadMemoryToolStripMenuItem.Click += new System.EventHandler(this.loadMemoryToolStripMenuItem_Click);
            // 
            // saveMemoryToolStripMenuItem
            // 
            this.saveMemoryToolStripMenuItem.Name = "saveMemoryToolStripMenuItem";
            this.saveMemoryToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.saveMemoryToolStripMenuItem.Text = "Save Memory...";
            // 
            // manageDevicesToolStripMenuItem
            // 
            this.manageDevicesToolStripMenuItem.Name = "manageDevicesToolStripMenuItem";
            this.manageDevicesToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.manageDevicesToolStripMenuItem.Text = "Manage Devices...";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gotoTB);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(139, 270);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Memory Map";
            // 
            // cursorPositionLabel
            // 
            this.cursorPositionLabel.Name = "cursorPositionLabel";
            this.cursorPositionLabel.Size = new System.Drawing.Size(36, 17);
            this.cursorPositionLabel.Text = "0x100";
            this.cursorPositionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // selectedBytesLabel
            // 
            this.selectedBytesLabel.Name = "selectedBytesLabel";
            this.selectedBytesLabel.Size = new System.Drawing.Size(96, 17);
            this.selectedBytesLabel.Text = "56 bytes selected";
            this.selectedBytesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 236);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Go to";
            // 
            // gotoTB
            // 
            this.gotoTB.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gotoTB.Location = new System.Drawing.Point(45, 233);
            this.gotoTB.Name = "gotoTB";
            this.gotoTB.Size = new System.Drawing.Size(87, 21);
            this.gotoTB.TabIndex = 1;
            this.gotoTB.TextChanged += new System.EventHandler(this.gotoTB_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 757);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.regGrpBox);
            this.Controls.Add(this.memGrpBox);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Virtual SIC";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.OnResize);
            this.memGrpBox.ResumeLayout(false);
            this.regGrpBox.ResumeLayout(false);
            this.regGrpBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox memGrpBox;
        private System.Windows.Forms.GroupBox regGrpBox;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox pcTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.OpenFileDialog openMemoryDialog;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSessionToolStripMenuItem;
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
    }
}

