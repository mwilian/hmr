using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EmrConstant;

namespace UserInterface
{
    public partial class Uncommit : Form
    {
        public Uncommit()
        {
            InitializeComponent();
        }
        public string GetReasion()
        {
            return tbxInfo.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            if (tbxInfo.Text.Trim().Length == 0)
            {
                MessageBox.Show("必须填写返修理由！", ErrorMessage.Warning);
                return;
            }
            if (MessageBox.Show("确认返修申请操作？", ErrorMessage.Warning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    == DialogResult.No) return;

            DialogResult = DialogResult.OK;
            Close();
   
        }
    }
}
