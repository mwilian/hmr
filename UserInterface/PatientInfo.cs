using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class PatientInfoForm : Form
    {
        public PatientInfoForm(EmrConstant.PatientInfo pi, int interval, DateTime todays)
        {
            InitializeComponent();
            patient1.InitControls(pi, todays);

            timer1.Interval = interval;
            timer1.Enabled = true;
        }
        public PatientInfoForm(EmrConstant.PatientInfo pi, string itemText, int interval, DateTime todays)
        {
            InitializeComponent();
            patient1.InitControls(pi, itemText, todays);

            timer1.Interval = interval;
            timer1.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void patient1_MouseLeave(object sender, EventArgs e)
        {

        }
    }
}