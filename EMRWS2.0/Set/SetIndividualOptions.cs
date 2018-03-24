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
    public partial class SetIndividualOptions : Form
    {
        public SetIndividualOptions()
        {
            InitializeComponent();
            individualOptions1.Initialize(Globals.DoctorID,
                GetVisitNoteID(), GetGroups(), Globals.usePage16k, Globals.showOpDoneTime,
                Globals.showPatientInfoTime, Globals.timeout);
            this.Location = new Point(72,60);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ThisAddIn.GetIndividualOptions();
            Close();
            
        }
        private string[] GetVisitNoteID()
        {
            XmlNodeList notes = Globals.emrPattern.GetEmrNotesForOutpatients();
            string[] noteNames = new string[notes.Count+1];
            noteNames[0] = "00 不打开病历";
            int n = 1;
            foreach (XmlNode note in notes)
            {               
                noteNames[n++] = note.Attributes[AttributeNames.NoteID].Value + " " +
                    note.Attributes[AttributeNames.NoteName].Value;
            }
            return noteNames;
        }

        private string[] GetGroups()
        {
            XmlNodeList groups = Globals.emrPattern.GetAllEmrGroups();
            if (groups == null) return null;

            string[] codes = new string[groups.Count];
            for (int k = 0; k < groups.Count; k++)
            {
                codes[k] = groups[k].Attributes[AttributeNames.NoteName].Value;
            }
            return codes;
        }
    }
}