using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using EmrConstant;
using CommonLib;

namespace UserInterface
{
    public partial class SelectDepartment : Form
    {
        string departFile = null;
        string departmentCode;

       EmrConstant.DeGetPrintInfo getPrintInfo = null;
        DeResetBeginTime resetBeginTime = null;        
        public SelectDepartment(string departList, string departCode,
            DeGetPrintInfo aGetPrintInfo, DeResetBeginTime aResetbeginTime
            )
        {
            InitializeComponent();

            getPrintInfo = aGetPrintInfo;
            resetBeginTime = aResetbeginTime;           
            departFile = departList;
            departmentCode = departCode;
        }

        private void SelectDoctor_Load(object sender, EventArgs e)
        {

            XmlReader departs = XmlReader.Create(departFile);
            dgvDeparts.Rows.Clear();
            if (!departs.ReadToFollowing(ElementNames.Department)) return;
            do
            {
                int index = dgvDeparts.Rows.Add();
                DataGridViewRow newrow = dgvDeparts.Rows[index];
                newrow.Cells[0].Value = departs.GetAttribute(AttributeNames.Code);
                newrow.Cells[1].Value = departs.GetAttribute(AttributeNames.Name);
            } while (departs.ReadToNextSibling(ElementNames.Department));

            if (departmentCode != StringGeneral.NullCode)
            {
                btnAll.Text = "查看本科";
                dgvDeparts.Enabled = false;
                dgvDeparts.ForeColor = Color.LightGray;
            }
        }

        private string GetDepartmentName(string code)
        {
            foreach (DataGridViewRow item in dgvDeparts.Rows)
            {
                if (item.Cells[0].Value == null) return null;
                if (item.Cells[0].Value.ToString() == code) return item.Cells[1].Value.ToString(); 
            }
            return null;
        }

        private void dgvDeparts_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string departCode = dgvDeparts.Rows[e.RowIndex].Cells[0].Value.ToString();
            XmlNode department = OneDepartment(departCode, GetDepartmentName(departCode));
            XmlElement departments = department.OwnerDocument.CreateElement(ElementNames.Departments);
            departments.AppendChild(department);
            ShowPrintInfo(departments);
            Cursor.Current = Cursors.Default;
        }
        private XmlNode OneDepartment(string code, string name)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement department = doc.CreateElement(ElementNames.Department);
            doc.AppendChild(department);
            string from = dtpFrom.Value.Date.ToString(StringGeneral.DateFormat) + " 00:00:00";
            string to = dtpTo.Value.Date.ToString(StringGeneral.DateFormat) + " 23:59:59";
            department.SetAttribute(AttributeNames.DayFrom, from);
            department.SetAttribute(AttributeNames.DayTo, to);
            department.SetAttribute(AttributeNames.DepartmentCode, code);
            department.SetAttribute(AttributeNames.DepartmentName, name);
            XmlNode printInfo = (XmlNode)department;
            getPrintInfo((int)numCount.Value, ref printInfo);
            return printInfo;
        }
        private void SelectDepartment_Resize(object sender, EventArgs e)
        {
            dgvDeparts.Height = this.Height - dgvDeparts.Top;
            dgvDeparts.Width = this.Width - dgvDeparts.Left - 2;
            dtpFrom.Width = this.Width - dtpFrom.Left - 2;
            dtpTo.Width = this.Width - dtpFrom.Left - 2;
            numCount.Width = this.Width - numCount.Left - 2;
            btnAll.Width = this.Width - btnAll.Left - 2;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            resetBeginTime();
            if (departmentCode == StringGeneral.NullCode)
            {
                if (dgvDeparts.Rows.Count == 0) return;

                string firstDepartment = dgvDeparts.Rows[0].Cells[0].Value.ToString();
                XmlNode department = OneDepartment(firstDepartment, GetDepartmentName(firstDepartment));
                XmlElement departments = department.OwnerDocument.CreateElement(ElementNames.Departments);
                if (department.ChildNodes.Count > 0) departments.AppendChild(department);
                for (int k = 1; k < dgvDeparts.Rows.Count - 1; k++)
                {
                    //if (dgvDeparts.Rows[k].Cells[0].Value == null) continue;
                    string code = dgvDeparts.Rows[k].Cells[0].Value.ToString();
                    department = OneDepartment(code, GetDepartmentName(code));
                    if (department.ChildNodes.Count == 0) continue;
                    XmlElement newDepart = departments.OwnerDocument.CreateElement(ElementNames.Department);
                    foreach (XmlAttribute att in department.Attributes) newDepart.SetAttribute(att.Name, att.Value);
                    newDepart.InnerXml = department.InnerXml;
                    departments.AppendChild(newDepart);
                }
                ShowPrintInfo(departments);
            }
            else
            {
                XmlNode department = OneDepartment(departmentCode, GetDepartmentName(departmentCode));
                XmlElement departments = department.OwnerDocument.CreateElement(ElementNames.Departments);
                departments.AppendChild(department);
                ShowPrintInfo(departments);
            }
            resetBeginTime();
            Cursor.Current = Cursors.Default;
        }
        private void ShowPrintInfo(XmlNode departments)
        {
            PrintInfo pi = new PrintInfo(departments, resetBeginTime);
            pi.Left = this.Width + 20;           
            pi.ShowDialog();
            
        }
    }
}
