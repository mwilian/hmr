using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EmrConstant;

namespace EMR
{
    public partial class UnlockEmr : Form
    {
        public UnlockEmr()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (rtbReasion.Text.Length == 0)
            {
                MessageBox.Show("������д�������ɣ�", ErrorMessage.Warning);
                return;
            }
            if (MessageBox.Show("ȷ�Ͽ��������", ErrorMessage.Warning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    == DialogResult.No) return;
        
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (rtbReasion.Text.Length > 0)
            {
                if (MessageBox.Show("�������������", ErrorMessage.Warning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    == DialogResult.No) return;
            }
            DialogResult = DialogResult.Cancel;
            Close();
        }
        public string GetReasion()
        {
            return rtbReasion.Text;
        }
        public bool ForPublic()
        {
            return chxPublic.Checked;
        }
        public int GetExpireDays()
        {
            return (int)nupExpire.Value;
        }
    }
}