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
    public partial class PatternList : Form
    {
        
        string NoteID = "";
        string noteName = "";
        public PatternList(XmlNodeList emrNotes)
        {
            InitializeComponent();
            LoadPattern(emrNotes);
        }
        public string GetNoteID()
        {
            return NoteID;
        }
        public string GetNoteName()
        {
            return noteName;
        }
        private void LoadPattern(XmlNodeList emrNotes)
        {

            #region Get pattern from database 
           
            foreach (XmlNode emrNote in emrNotes)
            {
                    DataGridViewRow newrow = new DataGridViewRow();
                    int index = dgvPattern.Rows.Add(newrow);
                    dgvPattern.Rows[index].Cells[0].Value = emrNote.Attributes[AttributeNames.NoteID].Value;
                    dgvPattern.Rows[index].Cells[1].Value = emrNote.Attributes[AttributeNames.NoteName].Value;
            }
            #endregion

        }

        private void dgvPattern_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnOK_Click(sender,e);           
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            NoteID=dgvPattern.SelectedRows[0].Cells[0].Value.ToString();
            noteName = dgvPattern.SelectedRows[0].Cells[1].Value.ToString();
        }
    }
}
