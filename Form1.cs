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

    }
}
