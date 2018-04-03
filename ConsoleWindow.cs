using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace vsic
{
    public partial class ConsoleWindow : Form
    {
        public ConsoleWindow()
        {
            InitializeComponent();
        }

        public void DisplayConsole(ConsoleDevice con)
        {
            Show();
            string tabName = con.ID.ToString("X").PadLeft(2, '0');
            if (tabControl.TabPages.ContainsKey(tabName))
            {
                // Display existing tab.
                tabControl.SelectTab(tabName);
            }
            else
            {
                // Add a new tab.
                var tab = new TabPage(tabName)
                {
                    Tag = con,
                    Name = tabName
                };
                var tb = new TextBox
                {
                    Multiline = true,
                    Dock = DockStyle.Fill,
                    Font = new Font("Courier New", 10f),
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both
                };
                tab.Controls.Add(tb);
                tabControl.TabPages.Add(tab);
                tabControl.SelectTab(tab);
            }
        }

        private void OnInputTBKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                var tab = tabControl.SelectedTab;
                var con = (ConsoleDevice)tab.Tag;
                var text = inputTB.Text;
                inputTB.Clear();
                foreach (char c in text)
                {
                    con.WriteByte((byte)c);
                }
                var tb = (TextBox)tab.Controls[0];
                tb.AppendText(text);
                tb.AppendText("\r\n"); // Just \n here was not enough to maintain line breaks after form resizing.
            }
        }



        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            // Hide-on-close behavior.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

    }
}
