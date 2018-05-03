using System;
using System.Drawing;
using System.Windows.Forms;
using Visual_SICXE.Devices;
using Visual_SICXE.Extensions;

namespace Visual_SICXE
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
                    Name = tabName,
                    Text = $"{con.ID:X2}\\{con.Name}"
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
                tb.DoubleBuffer(true);
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

        private void UpdateDisplayWithLine(ConsoleDevice sender, string s)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateDisplayWithLine(sender, s)));
                return;
            }
            var selectedTab = tabControl.SelectedTab;
            if (selectedTab.Tag == sender)
            {
                selectedTab.Controls["consoleTB"].Text += s;
            }
        }

        private void OnInputTBKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TabPage tab = tabControl.SelectedTab;
                var con = (ConsoleDevice)tab.Tag;
                string text = inputTB.Text + "\r\n";
                inputTB.Clear();
                con.WriteInputLine(text);
                UpdateDisplayWithLine(con, text);
                e.Handled = true;
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
