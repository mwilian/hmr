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
    public partial class GetTime : Form
    {
        private string sdate = null;
        private string stime = null;
        public GetTime()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            sdate = dtCommit.Value.ToString(StringGeneral.DateFormat);
            stime = numHour.Value.ToString() + Delimiters.Colon + numMinute.Value.ToString();           
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void GetTime_Load(object sender, EventArgs e)
        {
            string stime = dtCommit.Value.ToString(StringGeneral.TimeFormat);
            string[] items = stime.Split(Delimiters.Colon);            
            numHour.Value = DateTime.Now.Hour;
            numMinute.Value = DateTime.Now.Minute;
        }
        public DateTime GetCommitDateTime()
        {
            return Convert.ToDateTime(sdate + Delimiters.Space + stime);
        }
        public string GetCommitDate()
        {
            return sdate;
        }
        public string GetCommitTime()
        {
            return stime;
        }
    }
}
