using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EMR
{
    public partial class PrintForm : Form
    {
        public PrintForm()
        {
            InitializeComponent();
        }
        bool ok = false;       
        bool printAll = false;
        bool printThis = false;
        bool thiscursor = false;
        string rePageNum = "";
        string pageStart = "";
        string pageEnd = "";

        private void cbkRePageNum_TextChanged(object sender, EventArgs e)
        {

        }

        private void rbtnCheckPageNum_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtnCursor_CheckedChanged(object sender, EventArgs e)
        {
            thiscursor = true;
        }

        private void rbtnThisPageNum_CheckedChanged(object sender, EventArgs e)
        {
            printThis = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ok = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ok = false;
            this.Close();
        }
        public bool ShowMsg(out bool printAll, out bool printThis, out bool thiscursor)
        {
            this.ShowDialog();
            printAll = this.printAll;
            printThis = this.printThis;
            thiscursor = this.thiscursor;
            return ok;
        }

        private void rbtnAll_CheckedChanged(object sender, EventArgs e)
        {
            printAll = true;
        }
    }
}
