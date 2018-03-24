using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using EmrConstant;
using CommonLib;

namespace EMR
{
    public partial class Qualityks : Form // 时限检查 --全院某个病区所有病历时限情况
    {
        private Color completedOntimeColor = Color.DarkGreen;
        private Color completedColor = Color.Purple;
        private Color notCompletedColor = Color.Red;
        XmlNodeList patients = null;
        private int height0 = 626;
        private string doctorID = null;
        private XmlNode qualityInfo = null;
        public string departFile = null;
        public string doctorFile = null;
        private string startDate = null;
        private string endDate = null;
        int height = 0;
        string strss = "";
        private string title = "科室病历书写情况 -- ";

        public Qualityks(string opcode, XmlNodeList Patients)
        {
            InitializeComponent();
            patients = Patients;
            doctorID = opcode;
            // this.Text = title + Globals.doctors.GetDoctorName(opcode) + "医师";
        }
        public Qualityks(string opcode, string strs)
        {
            InitializeComponent();
            doctorID = opcode;
            strss = strs;
            // this.Text = title + Globals.doctors.GetDoctorName(opcode) + "医师";
        }
        public Qualityks()
        {
            InitializeComponent();
        }
        private Color WhichColor(string score, int restTime)
        {
            switch (score)
            {
                case EmrConstant.EmrManagement.EmrCompletedOntime:
                    return completedOntimeColor;
                case EmrConstant.EmrManagement.EmrCompleted:
                    return completedColor;
                case EmrConstant.EmrManagement.EmrNotCompleted:
                    if (restTime > 0) return Color.Gray;
                    else return notCompletedColor;
                default:
                    return notCompletedColor;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvQuality_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void Qualityks_Load(object sender, EventArgs e)
        {
            LoadDepart();
            LoadPattern();

            if (strss == "logon")
            {
                dgvQuality.Location = new Point(0, 0);
                groupBox1.Visible = false;
                LogonQuality();
            }           
        }
        private void LogonQuality()
        {
            XmlDocument doc = new XmlDocument();
            string patientListFileName = udt.MakePatientListFileName(doctorID);
            if (File.Exists(patientListFileName)) File.Delete(patientListFileName);
            ThisAddIn.xmlPatientWriterQL(doctorID, 2, patientListFileName);
            if (!File.Exists(patientListFileName)) return;

            doc.Load(patientListFileName);
            XmlNode qualityInfo = doc.CreateElement(ElementNames.QualityInfo);
            if (!ThisAddIn.GetQualityInfo(doc.DocumentElement, ref qualityInfo, null, null)) return;

            XmlNodeList patients = qualityInfo.SelectNodes(ElementNames.Patient);
            int height = 0;
            //string commT = "";
            foreach (XmlNode patient in patients)
            {
                string registryID = patient.Attributes[AttributeNames.RegistryID].Value;
                string patientName = patient.Attributes[AttributeNames.PatientName].Value;
                int index = dgvQuality.Rows.Add(registryID + " " + patientName, "", "", "", "", "");
                dgvQuality.Rows[index].DefaultCellStyle.BackColor = Color.BurlyWood;
                height += dgvQuality.Rows[index].Height;
                XmlNodeList notes = patient.SelectNodes(ElementNames.EmrNote);
                foreach (XmlNode note in notes)
                {
                    int restTime = Convert.ToInt16(note.Attributes[AttributeNames.RestTime].Value);
                    Color foreColor = WhichColor(note.Attributes[AttributeNames.Score].Value, restTime);
                    ///////////////////////////////////////////20090807 
                    int ResetTime = 0;
                    if (note.Attributes[AttributeNames.StartTime].Value != "")
                    {
                        string strStart = note.Attributes[AttributeNames.StartTime].Value;
                        string strEnd = "";
                        //if (note.Attributes[AttributeNames.CommitTime] != null && note.Attributes[AttributeNames.CommitTime].Value != "")
                        //{
                        //    commT = note.Attributes[AttributeNames.CommitTime].Value;
                        //    if (commT != "####")
                        //    {
                        //        strEnd = note.Attributes[AttributeNames.CommitTime].Value;
                        //    }
                        //    else
                        //    {
                        //        strEnd = DateTime.Now.ToString();
                        //    }
                        //}
                        //else
                        //{
                            strEnd = DateTime.Now.ToString();
                        //}

                        DateTime dtStart = Convert.ToDateTime(strStart);
                        DateTime dtEnd = Convert.ToDateTime(strEnd);
                        TimeSpan spResult = dtEnd - dtStart;
                        int hCount = spResult.Days * 24 + spResult.Hours;
                        if (note.Attributes[AttributeNames.TimeLimit].Value != "")
                        {
                            if (note.Attributes[AttributeNames.TimeLimit].Value != "0")
                            {
                                int x = hCount - Convert.ToInt32(note.Attributes[AttributeNames.TimeLimit].Value);
                                if (x > 0)
                                {
                                    note.Attributes[AttributeNames.RestTime].Value = Convert.ToString(Convert.ToDouble(x));

                                }

                            }
                        }
                    }


                    ///////////////////////////////////////////20090807 
                    int index1 = dgvQuality.Rows.Add("",
                        note.Attributes[AttributeNames.NoteName].Value,
                        note.Attributes[AttributeNames.StartTime].Value,
                        note.Attributes[AttributeNames.WrittenTime].Value,
                         // commT,
                        note.Attributes[AttributeNames.TimeLimit].Value + EmrManagement.TimeLimitUnit,
                        note.Attributes[AttributeNames.RestTime].Value + EmrManagement.TimeLimitUnit,
                        note.Attributes[AttributeNames.Score].Value);
                    dgvQuality.Rows[index1].DefaultCellStyle.ForeColor = foreColor;
                    height += dgvQuality.Rows[index1].Height;
                }
            }
            if (dgvQuality.Rows.Count == 0) return;

            dgvQuality.Height = height + dgvQuality.ColumnHeadersHeight + dgvQuality.Rows[0].Height;
            dgvQuality.Height = Math.Min(dgvQuality.Height, height0);
            dgvQuality.Rows[0].Cells[0].Selected = false;
            this.Height = dgvQuality.Height + 12;

            this.Top = (768 - this.Height) / 2;
        }
        private void LoadDepart()
        {
            string departFile = Path.Combine(Globals.linkListFolder, ResourceName.DepartmentsXml);
            if (!File.Exists(departFile)) return;
            string doctorFile = Path.Combine(Globals.linkListFolder, ResourceName.DoctorsXml);
            if (!File.Exists(doctorFile)) return;
            XmlReader departs = XmlReader.Create(departFile);
            DataTable dtDeparts = new DataTable();
            DataColumn dc = new DataColumn("code");
            DataColumn dc2 = new DataColumn("name");
            dtDeparts.Columns.Add(dc);
            dtDeparts.Columns.Add(dc2);

            if (!departs.ReadToFollowing(ElementNames.Department)) return;
            do
            {

                DataRow dr = dtDeparts.NewRow();
                dr["code"] = departs.GetAttribute(AttributeNames.Code);
                dr["name"] = departs.GetAttribute(AttributeNames.Name);
                dtDeparts.Rows.Add(dr);
            } while (departs.ReadToNextSibling(ElementNames.Department));
            cboDepart.DataSource = dtDeparts;
            cboDepart.DisplayMember = "name";
            cboDepart.ValueMember = "code";


        }
        private void LoadPattern()
        {
            DataTable dtPatt = new DataTable();
            DataColumn dc1 = new DataColumn("noteID");
            DataColumn dc2 = new DataColumn("noteName");
            dtPatt.Columns.Add(dc1);
            dtPatt.Columns.Add(dc2);
            DataRow dr2 = dtPatt.NewRow();
            dr2["noteName"] = "-----";
            dr2["noteID"] = "$";
            dtPatt.Rows.Add(dr2);
            XmlNodeList emrNotes = Globals.childPattern.GetAllEmrNotes();
            XmlNodeList emrNotes2 = Globals.emrPattern.GetAllEmrNotes();
            //maxNoteID = 0;
            foreach (XmlNode emrNote in emrNotes)
            {

                DataRow dr = dtPatt.NewRow();
                dr["noteName"] = emrNote.Attributes[AttributeNames.NoteName].Value;
                dr["noteID"] = emrNote.Attributes[AttributeNames.NoteID].Value;
                dtPatt.Rows.Add(dr);

            }
            foreach (XmlNode emrNote in emrNotes2)
            {

                DataRow dr = dtPatt.NewRow();
                dr["noteName"] = emrNote.Attributes[AttributeNames.NoteName].Value;
                dr["noteID"] = emrNote.Attributes[AttributeNames.NoteID].Value;
                dtPatt.Rows.Add(dr);

            }
            cboEmrNote.DataSource = dtPatt;
            cboEmrNote.DisplayMember = "noteName";
            cboEmrNote.ValueMember = "noteID";

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            startDate = dtpStart.Value.ToString(EmrConstant.StringGeneral.DateFormat);
            //criteria += " 00:00:00" + EmrConstant.Delimiters.Seperator;
            endDate = dpendTime.Value.ToString(EmrConstant.StringGeneral.DateFormat);
            // criteria += " 23:59:59" + EmrConstant.Delimiters.Seperator;
            if (dgvQuality.Rows.Count != 0) dgvQuality.Rows.Clear();
            Cursor.Current = Cursors.WaitCursor;
            //}
            string departCode = cboDepart.SelectedValue.ToString();
            //医师条件 科室全部
            if (!string.IsNullOrEmpty(departCode))
            {
                if (cboDoctor.Text.Equals("-----") && !string.IsNullOrEmpty(cboDoctor.Text.ToString()))
                {
                    string doctorFiles = Path.Combine(Globals.linkListFolder, ResourceName.DoctorsXml);
                    if (!File.Exists(doctorFiles)) return;
                    XmlNode patients =
                    ThisAddIn.GetPatients(gjtEmrPatients.PatientGettingMode.PatientsInDepartment, departCode);

                    XmlNode depart = udt.jj.DoctorsAndTheirPatients(doctorFiles, patients);

                    XmlDocument doc = new XmlDocument();
                    XmlElement qi = doc.CreateElement(ElementNames.QualityControl);
                    doc.AppendChild(qi);

                    ThisAddIn.ResetBeginTime();
                    if (depart != null)
                    {
                        foreach (XmlNode doctor in depart.ChildNodes)
                        {
                            XmlNode qinfo = null;
                            ThisAddIn.GetQualityInfo(doctor, ref qinfo, startDate, endDate);


                            if (qinfo.ChildNodes.Count > 0)
                            {
                                XmlElement qcinfo = qi.OwnerDocument.CreateElement(ElementNames.QualityInfo);
                                qcinfo.SetAttribute(AttributeNames.Name, doctor.Attributes[AttributeNames.Name].Value);
                                qcinfo.InnerXml = qinfo.InnerXml;
                                qi.AppendChild(qcinfo);
                            }

                        }
                    }
                    qualityInfo = qi;
                }
                else
                {
                    ThisAddIn.ResetBeginTime();
                    XmlDocument doc = new XmlDocument();
                    doctorID = cboDoctor.SelectedValue.ToString();
                    string patientListFileName = udt.MakePatientListFileName(doctorID);
                    if (File.Exists(patientListFileName)) File.Delete(patientListFileName);
                    ThisAddIn.xmlPatientWriterQL(doctorID, 2, patientListFileName);
                    if (!File.Exists(patientListFileName)) return;

                    doc.Load(patientListFileName);
                    XmlNode qualityInfos = doc.CreateElement(ElementNames.QualityInfo);
                    if (!ThisAddIn.GetQualityInfo(doc.DocumentElement, ref qualityInfos, startDate, endDate)) return;

                    XmlNodeList patientss = qualityInfos.SelectNodes(ElementNames.Patient);
                    patients = patientss;

                    doctorID = cboDoctor.SelectedValue.ToString();
                    //qualityInfo = qualityInfos;
                    XmlDocument docc = new XmlDocument();
                    XmlElement qi = docc.CreateElement(ElementNames.QualityControl);
                    docc.AppendChild(qi);

                    if (qualityInfos.ChildNodes.Count > 0)
                    {
                        XmlElement qcinfo = qi.OwnerDocument.CreateElement(ElementNames.QualityInfo);
                        //qcinfo.SetAttribute(AttributeNames.Name, "");
                        qcinfo.InnerXml = qualityInfos.InnerXml;
                        qi.AppendChild(qcinfo);
                    }
                    qualityInfo = qi;
                }
            }

            displayPatients();
            Cursor.Current = Cursors.Default;
        }
        private void displayPatients()
        {
            if (qualityInfo != null)
            {
                int doctorCount = 0;
                foreach (XmlNode doctor in qualityInfo.ChildNodes)
                {
                    //#region For one doctor's patients

                    XmlNodeList patients2 = doctor.SelectNodes(ElementNames.Patient);
                    LoadPatient(patients2);
                }

                if (dgvQuality.Rows.Count == 0) return;

            }
            else
            {
                if (patients != null)
                    LoadPatient(patients);

            }
        }
        private void LoadPatient(XmlNodeList patients)
        {
            ///int height = 0;
            foreach (XmlNode patient in patients)
            {
                string registryID = patient.Attributes[AttributeNames.RegistryID].Value;
                string patientName = patient.Attributes[AttributeNames.PatientName].Value;
                int index = dgvQuality.Rows.Add(registryID + " " + patientName, "", "", "", "", "");
                dgvQuality.Rows[index].DefaultCellStyle.BackColor = Color.BurlyWood;
                // height += dgvQuality.Rows[index].Height;
                XmlNodeList notes = patient.SelectNodes(ElementNames.EmrNote);
                string commT = "";
                foreach (XmlNode note in notes)
                {
                    int restTime = Convert.ToInt16(note.Attributes[AttributeNames.RestTime].Value);
                    Color foreColor = WhichColor(note.Attributes[AttributeNames.Score].Value, restTime);

                    int ResetTime = 0;
                    if (note.Attributes[AttributeNames.StartTime].Value != "")
                    {
                        string strStart = note.Attributes[AttributeNames.StartTime].Value;
                        string strEnd = "";
                        //if (note.Attributes[AttributeNames.CommitTime] != null && note.Attributes[AttributeNames.CommitTime].Value != "")
                        //{
                        //    commT = note.Attributes[AttributeNames.CommitTime].Value;
                        //    if (commT != "####")
                        //    {
                        //        strEnd = note.Attributes[AttributeNames.CommitTime].Value;
                        //    }
                        //    else
                        //    {
                        //        strEnd = DateTime.Now.ToString();
                        //    }
                        //}
                        //else
                        //{
                        //    commT = "####";
                            strEnd = DateTime.Now.ToString();
                        //}

                        DateTime dtStart = Convert.ToDateTime(strStart);
                        DateTime dtEnd = Convert.ToDateTime(strEnd);
                        TimeSpan spResult = dtEnd - dtStart;
                        int hCount = spResult.Days * 24 + spResult.Hours;
                        if (note.Attributes[AttributeNames.TimeLimit].Value != "")
                        {
                            if (note.Attributes[AttributeNames.TimeLimit].Value != "0")
                            {
                                int x = hCount - Convert.ToInt32(note.Attributes[AttributeNames.TimeLimit].Value);
                                if (x > 0)
                                {
                                    note.Attributes[AttributeNames.RestTime].Value = Convert.ToString(Convert.ToDouble(x));

                                }

                            }
                        }
                    }

                    if (cboEmrNote.Text != "-----")
                    {
                        if (note.Attributes[AttributeNames.NoteName].Value != cboEmrNote.Text.ToString())
                            continue;
                    }
                    ///////////////////////////////////////////20090807 
                    int index1 = dgvQuality.Rows.Add("",
                        note.Attributes[AttributeNames.NoteName].Value,
                        note.Attributes[AttributeNames.StartTime].Value,
                        note.Attributes[AttributeNames.WrittenTime].Value,
                       //  commT,
                        note.Attributes[AttributeNames.TimeLimit].Value + EmrManagement.TimeLimitUnit,
                        note.Attributes[AttributeNames.RestTime].Value + EmrManagement.TimeLimitUnit,
                        note.Attributes[AttributeNames.Score].Value);
                    dgvQuality.Rows[index1].DefaultCellStyle.ForeColor = foreColor;

                    height += dgvQuality.Rows[index1].Height;
                }
            }
        }

        private void cboDepart_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                DataTable dt = new DataTable();
                dt.TableName = "getdoc";
                DataSet dst = new DataSet();
                if (cboDepart.SelectedValue.ToString() != "$")
                {
                    dst = ep.GetDoctorListByDepartment(cboDepart.SelectedValue.ToString());
                    dt = dst.Tables[0];
                    DataRow dr = dt.NewRow();
                    dr["ysm"] = "-----";
                    dr["ysbm"] = "$";
                    dt.Rows.InsertAt(dr, 0);

                    cboDoctor.DataSource = dt;
                    cboDoctor.DisplayMember = "ysm";
                    cboDoctor.ValueMember = "ysbm";
                    cboDoctor.SelectedIndex = 0;
                }
                else
                {

                    dst = ep.GetDoctorList();
                    dt = dst.Tables[0];
                    DataRow dr = dt.NewRow();
                    dr["ysm"] = "全部";
                    dr["ysbm"] = "$";
                    dt.Rows.InsertAt(dr, 0);

                    cboDoctor.DataSource = dt;
                    cboDoctor.DisplayMember = "ysm";
                    cboDoctor.ValueMember = "ysbm";
                    cboDoctor.SelectedIndex = 0;
                }

            }
        }   

    }
}