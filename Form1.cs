using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sicsim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        #region unused
        //Label[] registerLabels;
        //TextBox[] registerTboxes;
        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    // Set up labels for registers.
        //    registerLabels = new Label[6];
        //    var regLabel = new Label();
        //    regLabel.Text = "A";
        //    registerLabels[0] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "B";
        //    registerLabels[1] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "S";
        //    registerLabels[2] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "T";
        //    registerLabels[3] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "X";
        //    registerLabels[4] = regLabel;
        //    regLabel = new Label();
        //    regLabel.Text = "L";
        //    registerLabels[5] = regLabel;

        //    // Set up text boxes for registers.
        //    registerTboxes = new TextBox[6];
        //    registerTboxes[0] = new TextBox();
        //    regGrpBox.Controls.AddRange(registerLabels);
        //    var offset = regGrpBox.ClientRectangle.Location;
        //    for (int i = 0; i < registerLabels.Length; ++i)
        //    {
        //        var labelLoc = new Point(offset.X + 2, offset.Y + 14 + (i == 0 ? 0 : i * registerLabels[i - 1].Height));
        //        registerLabels[i].Location = labelLoc;
        //    }

        //}
        #endregion

        Session sess;
        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Creating new machine...");

            new Task(() =>
            {
                sess = new Session();

                Log("Done.");
                SetStatusMessage("Ready");
            }).Start();
        }


        readonly Color COLOR_DEFAULT = Color.Black;
        readonly Color COLOR_ERROR = Color.DarkRed;
        private void Log(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>Log(str, args)));
                return;
            }
            logBox.SelectionColor = COLOR_DEFAULT;
            logBox.AppendText(string.Format(str, args));
            logBox.AppendText("\n");
        }

        private void LogError(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(str, args)));
                return;
            }
            logBox.SelectionColor = COLOR_ERROR;
            logBox.AppendText(string.Format(str, args));
            logBox.AppendText("\n");
        }

        public void SetStatusMessage(string str, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetStatusMessage(str, args)));
                return;
            }
            toolStripStatusLabel1.Text = string.Format(str, args);
        }
    }
}
