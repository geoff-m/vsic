namespace vsic
{
    partial class ConsoleWindow
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
            this.conTB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // conTB
            // 
            this.conTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conTB.Location = new System.Drawing.Point(0, 0);
            this.conTB.Multiline = true;
            this.conTB.Name = "conTB";
            this.conTB.Size = new System.Drawing.Size(284, 261);
            this.conTB.TabIndex = 0;
            // 
            // ConsoleWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.conTB);
            this.Name = "ConsoleWindow";
            this.Text = "Console";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox conTB;
    }
}