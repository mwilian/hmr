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

namespace UserInterface
{
    public partial class PrintInfo : Form
    {
        private Color oneColor = Color.Black;
        private Color twoColor = Color.DarkGreen;
        private Color threeColor = Color.Purple;
        private Color fourColor = Color.Red;
        private int height0 = 626;

        private DeResetBeginTime resetBeginTime = null;
        private XmlNode PrintInfoInfo = null;


        public PrintInfo(XmlNode info, DeResetBeginTime aResetBeginTime)
        {
            InitializeComponent();

            PrintInfoInfo = info;
            resetBeginTime = aResetBeginTime;
        }

        private void PrintInfo_Load(object sender, EventArgs e)
        {
            int height = 0;
            foreach (XmlNode depart in PrintInfoInfo.ChildNodes)
            {
                int departBegin = dgvPrintInfo.Rows.Count;
                string department = depart.Attributes[AttributeNames.DepartmentName].Value;
                foreach (XmlNode doctor in depart.ChildNodes)
                {
                    int doctorBegin = dgvPrintInfo.Rows.Count;
                    string doctorName = doctor.Attributes[AttributeNames.Doctor].Value;
                    foreach (XmlNode patient in doctor.ChildNodes)
                    {
                        //string registryID = patient.Attributes[AttributeNames.RegistryID].Value;
                        string patientName = patient.Attributes[AttributeNames.PatientName].Value;
                        int noteBegin = dgvPrintInfo.Rows.Count;
                        foreach (XmlNode note in patient.ChildNodes)
                        {
                            Color foreColor = WhichColor(note.Attributes[AttributeNames.PrintCount].Value);
                            int index1 = dgvPrintInfo.Rows.Add(department, doctorName, patientName,
                                note.Attributes[AttributeNames.NoteName].Value,
                                note.Attributes[AttributeNames.PrintCount].Value);
                            dgvPrintInfo.Rows[index1].DefaultCellStyle.ForeColor = foreColor;
                            height += dgvPrintInfo.Rows[index1].Height;
                        }
                        for (int i = noteBegin; i < dgvPrintInfo.Rows.Count; i++)
                        {
                            dgvPrintInfo.Rows[i].Cells[2].Value = string.Empty;
                        }
                    }
                    for (int j = doctorBegin; j < dgvPrintInfo.Rows.Count; j++)
                    {
                        dgvPrintInfo.Rows[j].Cells[1].Value = string.Empty;
                    }
                }
                for (int k = departBegin; k < dgvPrintInfo.Rows.Count; k++)
                {
                    dgvPrintInfo.Rows[k].Cells[0].Value = string.Empty;
                }
            }

            if (dgvPrintInfo.Rows.Count == 0) return;

            dgvPrintInfo.Height = height + dgvPrintInfo.ColumnHeadersHeight + dgvPrintInfo.Rows[0].Height;
            dgvPrintInfo.Height = Math.Min(dgvPrintInfo.Height, height0);

            dgvPrintInfo.Rows[0].Cells[0].Selected = false;
            this.Height = dgvPrintInfo.Height + 12;

            this.Top = (768 - this.Height) / 2;
            resetBeginTime();
            
        }
        private Color WhichColor(string printCount)
        {
            switch (Convert.ToInt32(printCount))
            {
                case 1:
                    return oneColor;
                case 2:
                    return twoColor;
                case 3:
                    return threeColor;
                case 4:
                    return fourColor;
                default:
                    return fourColor;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}