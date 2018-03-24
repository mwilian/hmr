using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using MathMLControl;

namespace EMR
{
    public partial class frmEquation : Form
    {
        public frmEquation()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            mathMLControl.MC_saveAsJPEG(Application.StartupPath + "\\Equation.jpg", 12, enum_ImageResolution._300dpi);
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}