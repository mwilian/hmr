using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CommonLib;

namespace UserInterface
{
    public partial class FindPatient : UserControl
    {
        private XmlNode patients = null;
        private int negativ = -1;
        private int offset = 1;
        private int mode;
        private string text;
        public FindPatient(XmlNode ops,XmlNode departs)
        {
            InitializeComponent();
            rbBoth.Checked = true;
            rbHouse.Checked = true;
            //XmlNode ops = ThisAddIn.GetOperatorList();
            LoadcbList(cbxDoctor, ops);
            //ops = ThisAddIn.GetDepartList();
            LoadcbList(cbxDepart, departs);
          
        }
        public XmlNode GetFindResult()
        {
            return patients;
        }
        public string GetText()
        {
            return text;
        }
        public int GetMode()
        {
            return mode;
        }
        private void btnFind1_Click(object sender, EventArgs e)
        {
            //string criteria = "";
            //if (cbxIn.Checked)
            //{
            //    criteria += dtpTime1.Value.ToString(EmrConstant.StringGeneral.DateFormat);
            //    criteria += " 00:00:00" + EmrConstant.Delimiters.Seperator;
            //    criteria += dtpTime2.Value.ToString(EmrConstant.StringGeneral.DateFormat);
            //    criteria += " 23:59:59" + EmrConstant.Delimiters.Seperator;
            //}
            if (tbArchiveNum.Text.Trim().Length > 0)
            {
                mode = 0;
                text = tbArchiveNum.Text;
                //patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.ArchiveNum, tbArchiveNum.Text);
            }
            if (tbRegistryID.Text.Trim().Length > 0)
            {
                mode = 1;
                string regID =udt.NormalizeRegistryID(tbRegistryID.Text);
                text = regID;
                //patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.RegistryID, regID);
            }
            if (tbPatientName.Text.Trim().Length > 0)
            {
                mode = 2;
                text = tbPatientName.Text;
                //text = criteria;
                //patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.PatientName, tbPatientName.Text);
            }
            if (txtAllPatientName.Text.Trim().Length > 0)
            {
                mode = 6;               
                text = txtAllPatientName.Text;                
            }
            //if (patients != null)
            //{
            //    ///2009/6/3
            //    //  Globals.ThisAddIn.RemoveNotInThisDepartment(patients);
            //    if (patients.ChildNodes.Count > 0)
            //    {
            offset *= negativ;
            this.Parent.Width += offset;
            //    }
            //    else
            //    {
            //        MessageBox.Show(EmrConstant.ErrorMessage.NoFindResult, EmrConstant.ErrorMessage.Warning);
            //    }
            //}
        }
        public void FindPatientLoad()
        {

            
          //XmlNode  ops = ThisAddIn.GetOperatorList();
          //LoadcbList(cbxDoctor, ops);
          //ops = ThisAddIn.GetDepartList();
          //LoadcbList(cbxDepart, ops);
            this.Show();
            tbPatientName.Focus();
           

        }
        private void LoadcbList(ComboBox cbx, XmlNode ops)
        {
            foreach (XmlNode op in ops.ChildNodes)
            {

                string item = op.Attributes["Code"].Value + ":" + op.Attributes["Name"].Value;
                cbx.Items.Add(item);
            }
        }
        private void btnFind2_Click(object sender, EventArgs e)
        {
            string criteria = tbPName.Text + EmrConstant.Delimiters.Seperator;
            string sex = EmrConstant.StringGeneral.Both;
            if (rbMale.Checked) sex = "1";
            if (rbFemale.Checked) sex = "2";
            criteria += sex + EmrConstant.Delimiters.Seperator;
            criteria += dtBegin.Value.ToString(EmrConstant.StringGeneral.DateFormat);
            criteria += " 00:00:00" + EmrConstant.Delimiters.Seperator;
            criteria += dtEnd.Value.ToString(EmrConstant.StringGeneral.DateFormat);
            criteria += " 23:59:59" + EmrConstant.Delimiters.Seperator;
            if (cbxDoctor.Text.Length > 0)
                criteria += cbxDoctor.Text.Split(EmrConstant.Delimiters.Seperator)[2];
            criteria += EmrConstant.Delimiters.Seperator;
            if (cbxDepart.Text.Length > 0)
                criteria += cbxDepart.Text.Split(EmrConstant.Delimiters.Colon)[1];
            mode = 3;
            text = criteria;
            //patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.Commpose, criteria);
            //if (patients != null)
            //{
            //    ///2009/6/3
            //    //  Globals.ThisAddIn.RemoveNotInThisDepartment(patients);
            //    if (patients.ChildNodes.Count > 0)
            //    {
            offset *= negativ;
            this.Parent.Width += offset;
            //    }
            //    else
            //    {
            //        MessageBox.Show(EmrConstant.ErrorMessage.NoFindResult, EmrConstant.ErrorMessage.Warning);
            //    }
            //}
        }

        private void tbPatientName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panelEx1_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                tbPatientName.Focus();
            }
        }

        private void btnFind1_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void tbArchiveNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "\r")
                btnFind1_Click(sender, e);
        }

        private void tbRegistryID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "\r")
                btnFind1_Click(sender, e);
        }

        private void tbPatientName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "\r")
                btnFind1_Click(sender, e);
        }
  
    }
}
