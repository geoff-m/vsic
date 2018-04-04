using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

/*
 * "Name" of a device is an optional thing that's just used by VSIC for ease of use purposes--
 * has no effect on the device's behavior or identity as far as the VM is concerned.
 * 
 * Before user enters a name, we can automatically generate a name for the new device,
 * like "Console 1", "Console 2", etc.
 * 
 * Clicking the Create new device button may open a new dialog to get parameters for
 * that specific type of device. 
 *
 * For file device, we will need file path.
 * 
 * For console device, it may be trivial enough to avoid this dialog for now.
 * 
 * In general, other devices (like graphics) will most likely want to have dialogs to customize their creation.
 */

namespace vsic
{
    public partial class DeviceManager : Form
    {
        public DeviceManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Machine associated with this DeviceManager.
        /// </summary>
        public Machine Machine
        { get; set; }

        /// <summary>
        /// Returns the valid ID number the user has entered, or else null.
        /// </summary>
        private byte? GetID()
        {
            string idtext = idTB.Text;
            int len = idtext.Length;
            byte ret;
            if (len == 1 || len == 2)
            {
                if (byte.TryParse(idtext, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ret))
                    return ret;
                else
                    return null;
            }
            return null;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            byte? id = GetID();
            if (!id.HasValue)
            {
                MessageBox.Show("Enter an ID for the device to create.", "Create device", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                idTB.Focus();
                return;
            }

            var devname = nameTB.Text.Trim();
            string devtype = typeCB.Text.ToLower();
            IODevice newDevice = null;
            switch (devtype)
            {
                case "file":
                    // Need to get file path for this guy. Show special dialog for creating file device.
                    // Let's just use a built-in file browser dialog for now.
                    var fileBrowserDlg = new OpenFileDialog();
                    fileBrowserDlg.Title = $"Choose path for file device 0x{id.Value.ToString("X")} (\"{devname}\")";
                    fileBrowserDlg.CheckFileExists = false;
                    if (fileBrowserDlg.ShowDialog() == DialogResult.OK)
                    {
                        var path = fileBrowserDlg.FileName;
                        try
                        {
                            newDevice = new FileDevice(id.Value, path);
                            
                        }
                        catch (System.IO.IOException ex)
                        {
                            MessageBox.Show(ex.Message, "Error creating file device", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    break;
                case "console":
                    // No special options needed for this type of device? simply create it
                    newDevice = new ConsoleDevice(id.Value);

                    break;
                case "graphics":
                    // Not implemented yet.
                    return;
                    break;

                default:
                    MessageBox.Show($"\"{typeCB.Text}\" is not a valid device type.", "Create device", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    typeCB.Focus();
                    return;
            }

            newDevice.Name = nameTB.Text;
            try
            {
                Machine.AddDevice(id.Value, newDevice);
                
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Could not add the device to the machine:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            devLV.Items.Add(new ListViewItem(new string[] { newDevice.ID.ToString("X2"), newDevice.Type, newDevice.Name}));
        }

        private void onIdTBKeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            int cursorIndex = idTB.SelectionStart;
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
                    if (cursorIndex < idTB.MaxLength) // Do nothing if caret is after last character.
                    {
                        // Overwrite the character following the caret.
                        idTB.SelectionLength = 1;
                        idTB.SelectedText = char.ToUpper(c).ToString();
                    }
                    break;
                case (char)Keys.Back:
                    if (cursorIndex > 0)
                        idTB.SelectionStart = cursorIndex - 1;
                    break;
            }

            e.Handled = true;
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
