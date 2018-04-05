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
            string tabName = con.ID.ToString("X2");
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
                    Name = "consoleTB",
                    Multiline = true,
                    Dock = DockStyle.Fill,
                    Font = new Font("Courier New", 10f),
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both
                };
                con.OutputByteWritten += UpdateDisplay;
                tab.Controls.Add(tb);
                tabControl.TabPages.Add(tab);
                tabControl.SelectTab(tab);
            }
        }

        private void UpdateDisplay(ConsoleDevice sender, byte b)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateDisplay(sender, b)));
                return;
            }
            var selectedTab = tabControl.SelectedTab;
            if (selectedTab.Tag == sender)
            {
                if (b == '\n')
                {
                    selectedTab.Controls["consoleTB"].Text += "\r\n";
                }
                else
                {
                    selectedTab.Controls["consoleTB"].Text += ((char)b).ToString();
                }
            }
        }

        private void OnInputTBKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                var tab = tabControl.SelectedTab;
                var con = (ConsoleDevice)tab.Tag;
                var text = inputTB.Text + "\n";
                inputTB.Clear();
                foreach (char c in text)
                {
                    con.WriteInputByte((byte)c);
                }
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
