using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using EmrConstant;
using System.Xml;

namespace EMR
{
    public partial class ConsentForm : Form
    {
        public ConsentForm(XmlNode[] emrNotes)
        {
            InitializeComponent();
            SetItem(emrNotes);
            cbxID.SelectedItem = cbxID.Items[0];
        }
        private void SetItem(XmlNode[] emrNotes)
        {
            for (int i = 0; i < emrNotes.Length; i++)
            {
                if (emrNotes[i] == null) return;
                cbxID.Items.Add(emrNotes[i].Attributes[AttributeNames.NoteID].Value + ":" + emrNotes[i].Attributes[AttributeNames.NoteName].Value);
            }

        }
        public string GetID()
        {
            return cbxID.SelectedItem.ToString().Split(':')[0];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
