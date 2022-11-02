namespace Visual_SICXE
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
            this.inputTB = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // inputTB
            // 
            this.inputTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputTB.Location = new System.Drawing.Point(0, 267);
            this.inputTB.Name = "inputTB";
            this.inputTB.Size = new System.Drawing.Size(473, 24);
            this.inputTB.TabIndex = 0;
            this.inputTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnInputTBKeyPress);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(473, 261);
            this.tabControl.TabIndex = 2;
            // 
            // ConsoleWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 292);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.inputTB);
            this.Name = "ConsoleWindow";
            this.Text = "Console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox inputTB;
        private System.Windows.Forms.TabControl tabControl;
    }
}