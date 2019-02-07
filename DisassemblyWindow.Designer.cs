namespace Visual_SICXE
{
    partial class DisassemblyWindow
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
            this.rtb = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtb
            // 
            this.rtb.BackColor = System.Drawing.SystemColors.Control;
            this.rtb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb.Location = new System.Drawing.Point(0, 0);
            this.rtb.Name = "rtb";
            this.rtb.ShortcutsEnabled = false;
            this.rtb.Size = new System.Drawing.Size(314, 582);
            this.rtb.TabIndex = 0;
            this.rtb.Text = "";
            // 
            // DisassemblyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 582);
            this.Controls.Add(this.rtb);
            this.Name = "DisassemblyWindow";
            this.Text = "Disassembly";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb;
    }
}