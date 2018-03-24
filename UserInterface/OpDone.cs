using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class OpDone : Form
    {
        int i = 0;
        public OpDone(string msg)
        {
            InitializeComponent();
            lbMsg.Text = msg;
            timer1.Interval = 3;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            i++;
            if(i==18)    this.Close();
        }

    }
}