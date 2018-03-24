using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommonLib;
using EmrConstant;
using System.Xml;

namespace EMR
{
    public partial class Area : Form
    {
        public Area()
        {
            InitializeComponent();
            cbxAreaLoad();
        }

        private void cbxAreaLoad()
        {
            //cbDepartment.SelectedIndex = -1;
            XmlNode areas = null;
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                areas = pi.GetAreas();
            }
            cbxArea.Items.Clear();
            foreach (XmlNode area in areas.SelectNodes(ElementNames.Area))
            {
                string text = area.Attributes[AttributeNames.Code].Value + Delimiters.Space
                    + area.Attributes[AttributeNames.Name].Value;
                cbxArea.Items.Add(text);
            }
            if (cbxArea.Items.Count == 0) return;
            /* Show current department set*/
            //cbDepartment.SelectedIndex = -1;
            string code = Globals.myConfig.GetAreaCode();
            if (code == StringGeneral.NullCode)
            {
                string departmentCode = Globals.myConfig.GetDepartmentCode();
                code = ThisAddIn.GetAreaCodeOfDepartment(departmentCode);
                if (code == null || code.Length == 0)
                {
                    cbxArea.SelectedIndex = 0;
                    return;
                }
            }

            for (int i = 0; i < cbxArea.Items.Count; i++)
            {
                if (code == cbxArea.Items[i].ToString().Split(' ')[0])
                {
                    cbxArea.SelectedIndex = i;
                    return;
                }
            }
            cbxArea.SelectedIndex = 0;
        }

        private void cbxArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] codename = cbxArea.Items[cbxArea.SelectedIndex].ToString().Split(' ');
            Globals.myConfig.SetAreaCode(codename[0]);
            Globals.AreaID = codename[0];
            Globals.myConfig.SetAreaCode(Globals.AreaID);
          //  SetApplicationTitle(opname);
            this.ControlBox = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
