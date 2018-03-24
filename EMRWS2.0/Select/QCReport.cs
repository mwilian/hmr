using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using EmrConstant;
using CommonLib;

namespace EMR
{
    public partial class QCReport : Form
    {
        private bool selfReport = false;
        private bool selfInfo = false;
        private int newLocation = 0;
        //private string showState = StringGeneral.Zero;

        public QCReport(bool self)
        {
            InitializeComponent();
            selfReport = self;
        }

        public QCReport(bool self, bool showselfinfo)
        {
            InitializeComponent();
            selfReport = self;
            selfInfo = showselfinfo;
        }

        private void QCReport_Resize(object sender, EventArgs e)
        {
            AdjustSize();
        }
        private void AdjustSize()
        {
            dgvValuate.Width = split.Panel2.Width;
            dgvValuate.Height = split.Panel2.Height;
            dgvPatients.Width = dgvValuate.Width;
            dgvPatients.Height = dgvValuate.Height;
            dgvPatients.Top = 0;

            dgvDepartment.Width = dgvPatients.Width;
            dgvDepartment.Height = dgvPatients.Height;
            dgvDepartment.Top = 0;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            ResetBeginTime();
            dtpValueChanged();
        }
        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            ResetBeginTime();
            dtpValueChanged();
        }
        private void dtpValueChanged()
        {
            if (selfInfo)
            {
                LoadPatientScore(Globals.DoctorID, Globals.DepartID);
            }
            else
            {
                if (selfReport)
                {
                    LoadDoctorScore(Globals.DepartID);
                }
                else
                {
                    LoadDepartmentScore();
                }
            }
        }
        private void QCReport_Load(object sender, EventArgs e)
        {
            TimeSpan oneMonth = TimeSpan.FromDays(daysOneMonth() - 1);
            dtpFrom.Value = dtpTo.Value.Subtract(oneMonth);
            AdjustSize();
        }
        private double daysOneMonth()
        {
            int month = DateTime.Now.Month - 1;
            int year = DateTime.Now.Year;
            if (month == 2)
            {
                if (DateTime.IsLeapYear(year)) return 29;
                else return 28;
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11)
            {
                return 30;
            }
            else
            {
                return 31;
            }

        }
        private void LoadDoctorScore(string departmentCode)
        {
         
            if (selfInfo) return;

            XmlNode scores = null;
            using (EMR.gjtEmrService.emrServiceXml es = new EMR.gjtEmrService.emrServiceXml())
            {
                string msg = es.GetDoctorScoreForDepartment(selfReport, departmentCode,
                    dtpFrom.Value, dtpTo.Value, ref scores);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
            }
            if (scores == null) return;

            dgvValuate.Rows.Clear();
            int index = 0;
            double oneCount = 0;
            double emrCount = 0;
            foreach (XmlNode doctor in scores.ChildNodes)
            {
                emrCount = Convert.ToDouble(doctor.Attributes[AttributeNames.Num].Value);
                index = dgvValuate.Rows.Add();
                dgvValuate.Rows[index].Cells[0].Value = doctor.Attributes[AttributeNames.A].Value;
                oneCount = Convert.ToDouble(doctor.Attributes[AttributeNames.A].Value);
                dgvValuate.Rows[index].Cells[1].Value = Percent(oneCount, emrCount);
                dgvValuate.Rows[index].Cells[2].Value = doctor.Attributes[AttributeNames.B].Value;
                oneCount = Convert.ToDouble(doctor.Attributes[AttributeNames.B].Value);
                dgvValuate.Rows[index].Cells[3].Value = Percent(oneCount, emrCount);
                dgvValuate.Rows[index].Cells[4].Value = doctor.Attributes[AttributeNames.C].Value;
                oneCount = Convert.ToDouble(doctor.Attributes[AttributeNames.C].Value);
                dgvValuate.Rows[index].Cells[5].Value = Percent(oneCount, emrCount);
                dgvValuate.Rows[index].Cells[6].Value = doctor.Attributes[AttributeNames.D].Value;
                oneCount = Convert.ToDouble(doctor.Attributes[AttributeNames.D].Value);
                dgvValuate.Rows[index].Cells[7].Value = Percent(oneCount, emrCount);
                string doctorID = doctor.Attributes[AttributeNames.Code].Value;
         
       
                dgvValuate.Rows[index].HeaderCell.Value = Globals.doctors.GetDoctorName(doctorID);
                dgvValuate.Rows[index].HeaderCell.Tag = doctorID;
            }


            dgvValuate.CurrentCell = dgvValuate.Rows[index + 1].Cells[0];
            dgvValuate.Visible = true;
            dgvDepartment.Visible = false;

            dgvDepartment.Tag = departmentCode;
            lbInfo.Text = Globals.departments.GetDepartmentName(departmentCode);
        }
        private void LoadDepartmentScore()
        {
            dgvDepartment.Rows.Clear();
            //2009-07-02 modify by sunfan
            if (selfInfo) return;


            XmlNode scores = null;
            using (EMR.gjtEmrService.emrServiceXml es = new EMR.gjtEmrService.emrServiceXml())
            {
                string msg = es.GetDepartmentScore(selfReport, dtpFrom.Value, dtpTo.Value, ref scores);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
            }
            if (scores == null) return;

            int index = 0;
            double emrCount = 0;
            double oneCount = 0;
            foreach (XmlNode department in scores.ChildNodes)
            {
                emrCount = Convert.ToDouble(department.Attributes[AttributeNames.Num].Value);
                index = dgvDepartment.Rows.Add();
                dgvDepartment.Rows[index].Cells[0].Value = department.Attributes[AttributeNames.A].Value;
                oneCount = Convert.ToDouble(department.Attributes[AttributeNames.A].Value);
                dgvDepartment.Rows[index].Cells[1].Value = Percent(oneCount, emrCount); 
                dgvDepartment.Rows[index].Cells[2].Value = department.Attributes[AttributeNames.B].Value;
                oneCount = Convert.ToDouble(department.Attributes[AttributeNames.B].Value);
                dgvDepartment.Rows[index].Cells[3].Value = Percent(oneCount, emrCount);
                dgvDepartment.Rows[index].Cells[4].Value = department.Attributes[AttributeNames.C].Value;
                oneCount = Convert.ToDouble(department.Attributes[AttributeNames.C].Value);
                dgvDepartment.Rows[index].Cells[5].Value = Percent(oneCount, emrCount);
                dgvDepartment.Rows[index].Cells[6].Value = department.Attributes[AttributeNames.D].Value;
                oneCount = Convert.ToDouble(department.Attributes[AttributeNames.D].Value);
                dgvDepartment.Rows[index].Cells[7].Value = Percent(oneCount, emrCount);
                string departCode = department.Attributes[AttributeNames.Code].Value;
                dgvDepartment.Rows[index].HeaderCell.Value = Globals.departments.GetDepartmentName(departCode);
                dgvDepartment.Rows[index].HeaderCell.Tag = departCode;
            }
            dgvDepartment.CurrentCell = dgvDepartment.Rows[index + 1].Cells[0];
            dgvDepartment.Visible = true;
        }
        private void LoadPatientScore(string doctorID, string departmentCode)
        {
            XmlNode scores = null;
            using (EMR.gjtEmrService.emrServiceXml es = new EMR.gjtEmrService.emrServiceXml())
            {
                string msg = es.GetPatientScoreForDoctor(selfReport, doctorID, departmentCode,
                    dtpFrom.Value, dtpTo.Value, ref scores);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
            }
            if (scores == null) return;

            dgvPatients.Rows.Clear();
            int index = 0;
            foreach (XmlNode patient in scores.ChildNodes)
            {
                index = dgvPatients.Rows.Add();
                int scoreIndex = Convert.ToInt32(patient.Attributes[AttributeNames.Score].Value);
                dgvPatients.Rows[index].Cells[0].Value = ScoreLevel(scoreIndex);
                string registryID = patient.Attributes[AttributeNames.RegistryID].Value;
                dgvPatients.Rows[index].HeaderCell.Value = ThisAddIn.GetPatientNameByRegistryID(registryID);
                dgvPatients.Rows[index].HeaderCell.Tag = registryID;
            }


            dgvPatients.CurrentCell = null;
            dgvPatients.Visible = true;
            //2009-07-02 modify by sunfan.
            dgvDepartment.Visible = false;
            dgvValuate.Visible = false;

            lbInfo.Text = Globals.doctors.GetDoctorName(doctorID);
        }
        private string ScoreLevel(int index)
        {
            ValuateText vt = new ValuateText();
            return vt.Text[index];
        }
        private string Percent(double oneCount, double emrCount)
        {
            if (oneCount == 0.0) return "";
            if (emrCount == 0.0) return "";

            double ratio = oneCount / emrCount * 100;
            return ratio.ToString("###") + "%";
        }
        private void dgvValuate_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ResetBeginTime();
            if (e.Button == MouseButtons.Right && dgvDepartment.Rows.Count>1)
            {
                dgvValuate.Visible = false;
                dgvDepartment.Visible = true;
            }
            else
            {
                if (dgvValuate.Rows[e.RowIndex].HeaderCell.Tag == null) return;

                string doctorID = dgvValuate.Rows[e.RowIndex].HeaderCell.Tag.ToString();
                LoadPatientScore(doctorID, dgvDepartment.Tag.ToString());

                dgvValuate.Visible = false;
                dgvPatients.Visible = true;
            }

        }

        private void dgvPatients_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ResetBeginTime();
            if (e.Button == MouseButtons.Right && dgvValuate.Rows.Count>1)
            {
                dgvPatients.Visible = false;
                dgvValuate.Visible = true;
            }
            else
            {
                if (dgvPatients.Rows[e.RowIndex].HeaderCell.Tag == null) return;

                string registryID = dgvPatients.Rows[e.RowIndex].HeaderCell.Tag.ToString();              
                LoadScoreDetail(registryID);
            }
        }

        private void dgvDepartment_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ResetBeginTime();
            if (dgvDepartment.Rows[e.RowIndex].HeaderCell.Tag == null) return;
            string departmentCode = dgvDepartment.Rows[e.RowIndex].HeaderCell.Tag.ToString();
            dgvDepartment.Visible = false;
            LoadDoctorScore(departmentCode);
        }

        private void LoadScoreDetail(string registryID)
        {
            XmlNode flaws = null;
            #region Get flaws from database
            using (EMR.gjtEmrService.emrServiceXml es = new EMR.gjtEmrService.emrServiceXml())
            {

                string msg = es.GetValuateDetail(selfReport, registryID, ref flaws);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
            }
            if (flaws == null) return;
            #endregion
            dgvPatients.Visible = false;
            newLocation = 0;
            foreach (XmlNode note in flaws.ChildNodes)
            {
                NewFlaw(note);
            }
            lbInfo.Text = ThisAddIn.GetPatientNameByRegistryID(registryID);
        }
        private void NewFlaw(XmlNode note)
        {
            Flaw ucFlaw = new Flaw(note);
            split.Panel2.Controls.Add(ucFlaw);
            ucFlaw.Width = split.Panel2.Width;
            ucFlaw.Height = ucFlaw.GetControlHeight();
            ucFlaw.Top = newLocation;
            newLocation += ucFlaw.Height + 1;
            ucFlaw.Visible = true;
            ucFlaw.MouseDown += new MouseEventHandler(ucFlaw_MouseDown);
        }

        void ucFlaw_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                foreach (Control ctrl in split.Panel2.Controls)
                {
                    string type = ctrl.GetType().ToString();
                    if (type == "WordAddInEmrw.Flaw") split.Panel2.Controls.Remove(ctrl);
                }
                dgvPatients.Visible = true;
            }
        }

        private void ResetBeginTime()
        {
            ThisAddIn.ResetBeginTime();
        }

    }
}