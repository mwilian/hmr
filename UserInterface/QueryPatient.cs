using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;

namespace UserInterface
{
    public partial class QueryPatient : Form
    {
        //LoadPatientTree LoadPatient;
        string text;
        int mode;
        private FindPatient findPatient;
        public QueryPatient(XmlNode ops, XmlNode departs)
        {
            InitializeComponent();
            //LoadPatient = LP;
            this.findPatient = new UserInterface.FindPatient(ops, departs);
            this.Controls.Add(this.findPatient);
            this.findPatient.Location = new System.Drawing.Point(0, 0);
            this.findPatient.Name = "findPatient";
            this.findPatient.Size = new System.Drawing.Size(535, 328);
            this.findPatient.TabIndex = 0;
            this.findPatient.Load += new System.EventHandler(this.findPatient_Load);
        }

        private void QueryPatient_SizeChanged(object sender, EventArgs e)
        {
            //XmlNode patients = findPatient.GetFindResult();
            //if (patients == null) return;

            //LoadPatient(patients);
            //Close();
            text=findPatient.GetText();
            mode = findPatient.GetMode();
            this.DialogResult = DialogResult.OK;
        }
        public string GetText()
        {
            return text;
        }
        public int GetMode()
        {
            return mode;
        }
        private void findPatient_Load(object sender, EventArgs e)
        {

        }

        private void QueryPatient_Load(object sender, EventArgs e)
        {
            this.Show();
            findPatient.FindPatientLoad();
        }
    }
}