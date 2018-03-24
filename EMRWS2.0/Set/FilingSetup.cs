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
    public partial class FilingSetup : Form
    {
        private int noteCount = 0;
        public string RegistryID;
        public string szlx;
        public string DepartCode;
       
        public FilingSetup(string ID)
        {
           
            InitializeComponent();
            if (ID.Length > 4)
            {
                lbNoteName.Text = "病人住院号：" + ID;
                this.Text = "个人归档项目设置";
                szlx = "个人";
                DepartCode = ThisAddIn.GetDepartCodeByRegistryID(ID);
            }
            else
            {
                lbNoteName.Text = "科室编码：" + ID;
                this.Text = "科室归档项目设置";
                szlx = "科室";
            }
            RegistryID = ID;
            LoadPattern();
            LoadNotesWithRules(); 
        }

        private void LoadPattern()
        {                      
             #region Load tree with pattern

            XmlNodeList emrNotes = Globals.emrPattern.GetAllEmrNotes();
                foreach (XmlNode emrNote in emrNotes)
                {
                    /* Filter out the invalid emr note */
                    if (emrNote.Attributes[AttributeNames.Valid] != null &&
                        emrNote.Attributes[AttributeNames.Valid].Value == StringGeneral.No) continue;
                    if (emrNote.Attributes[AttributeNames.BelongDepartment] != null && emrNote.Attributes[AttributeNames.BelongDepartment].Value != Globals.myConfig.GetDepartmentCode())
                        continue;
                    if (emrNote.Attributes[AttributeNames.NoteID].Value == "02"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "05"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "06"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "09"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "10"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "11"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "17"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "18"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "29"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "30"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "38"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "39"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "40"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "46"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "52"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "53"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "54"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "55"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "56"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "59"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "71"
                        || emrNote.Attributes[AttributeNames.NoteID].Value == "76"

                        ) continue;

                    /* Load tree level 0 */
                    TreeNode note = tvNotes.Nodes.Add(emrNote.Attributes[AttributeNames.NoteName].Value);
                    note.Name = emrNote.Attributes[AttributeNames.NoteID].Value + ":0";
                   
                    
                }
                #endregion
              #region Load tree with childpattern
                 emrNotes =Globals.childPattern.GetAllEmrNotes();
                 foreach (XmlNode emrNote in emrNotes)
                 {
                     if (emrNote.Attributes[AttributeNames.BelongDepartment] != null && emrNote.Attributes[AttributeNames.BelongDepartment].Value != Globals.myConfig.GetDepartmentCode())
                         continue;
                     foreach (TreeNode tn in tvNotes.Nodes)
                     {
                         if (tn.Name.Split(':')[0] == emrNote.Attributes[AttributeNames.ParentID].Value)
                         {
                             TreeNode note = tn.Nodes.Add(emrNote.Attributes[AttributeNames.NoteName].Value);
                    
                             note.Name = emrNote.Attributes[AttributeNames.ParentID].Value+":"+emrNote.Attributes[AttributeNames.NoteID].Value;
                         }
                     }
                 }
                #endregion
                 tvNotes.ExpandAll();
        }
        private void LoadNotesWithRules()
        {
            DataSet notes = null;

            if (szlx == "个人")
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    #region Get FilingSetup from Department
                    string msg = null;
                    try
                    {

                        msg = es.GetFilingSetupfromDepartment(RegistryID, DepartCode);
                        if (msg != null)
                        {
                            MessageBox.Show(msg, ErrorMessage.Error);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX925511256775", ex.Message + ">>" + ex.ToString(), true);            
               
                        return;
                    }
                    #endregion
                }
            }

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                #region Get notes from database
                string msg = null;
                try
                {                   
                    msg = es.GetNotesWithFilingSetup(RegistryID, ref notes);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925511256795", ex.Message + ">>" + ex.ToString(), true);            
               
                    return;
                }
                #endregion

                #region Add notes on pnlNote
                foreach (DataRow note in notes.Tables[0].Rows)
                {
                    string noteID = note[0].ToString();
                    if (noteID.Split(':').Length == 1)
                        noteID = noteID + ":0";
                    string noteName = GetNoteNameFromTree(noteID);
                    if (noteName == null) continue;
                    NewFilingUC(noteID, noteName);
                }
                #endregion
            }
        }
        private string GetNoteNameFromTree(string noteID)
        {
            foreach (TreeNode note in tvNotes.Nodes)
            {
                if (note.Name.Trim().Split(':')[0] == noteID.Trim().Split(':')[0])
                {
                    if (note.Name.Trim() == noteID.Trim())
                    {

                        return note.Text;
                    }
                    else if (note.Nodes.Count > 0)
                    {
                        foreach (TreeNode tn in note.Nodes)
                        {
                            if (tn.Name.Trim() == noteID.Trim())
                            {

                                return tn.Text;
                            }
                        }
                    }
                }
            }
            return null;
        }
        private void FilingSetup_Resize(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            split.Height = this.Height - split.Top - 34;
            split.Width = this.Width - 8;
            

            if (this.Tag != null)
            {
                RemoveFilingUC(this.Tag.ToString());
                this.Tag = null;
            }
            RerangeFilingUC();
        }
        private void RemoveFilingUC(string noteID)
        {
            foreach (Control ctrl in pnlNote.Controls)
            {
                FilingUC nuc = (FilingUC)ctrl;
                if (noteID == nuc.GetNoteID())
                {
                    pnlNote.Controls.Remove(ctrl);
                    noteCount--;
                    return;
                }
            }
        }

        private void newFlaw_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            //dgvMark1.Rows.Add();
        }

        private void tvNotes_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            TreeNode note = (TreeNode)e.Item;
            if (note.Nodes.Count > 0) return;
            object item = note.Text + ":" + note.Name;
            DoDragDrop(item, DragDropEffects.Copy);            
        }

        private void pnlNote_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.StringFormat);
            string[] items = data.ToString().Split(Delimiters.Colon);
            NewFilingUC(items[1]+":"+items[2], items[0]);
            ThisAddIn.ResetBeginTime();
        }
        private void pnlNote_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None; 
        }
        private void NewFilingUC(string noteID, string noteName)
        {
            if (IsExist(noteID)) return;

            FilingUC note = new FilingUC(RegistryID, noteID, noteName);
                pnlNote.Controls.Add(note);
                pnlNote.Controls[noteCount].Location = NewFilingUCLocation(note);
                noteCount++;

        }
        private Point NewFilingUCLocation(FilingUC note)
        {
            int colCount = pnlNote.Width / (note.Width + 2);
            int top = 0;
            int left = 0;

            if (colCount == 0)
            {
                top = noteCount * (note.Height + 2);
            }
            else
            {
                top = noteCount / colCount * (note.Height + 2);
                left = noteCount % colCount * (note.Width + 2);
            }
            return new Point(left, top);
        }
        private bool IsExist(string noteID)
        {
            foreach (Control cc in pnlNote.Controls)
            {
                FilingUC FilingUC = (FilingUC)cc;
                if (noteID.Trim() == FilingUC.GetNoteID().Trim()) return true;
            }
            return false;
        }

        private void RerangeFilingUC()
        {
            noteCount = 0;
            foreach (Control FilingUC in pnlNote.Controls)
            {
                FilingUC.Location = NewFilingUCLocation((FilingUC)FilingUC);
                noteCount++;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            XmlDocument doc = new XmlDocument();
            XmlElement rules = doc.CreateElement(ElementNames.Rules);
        
            #region Put into database
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    string msg = "";


                    foreach (Control cc in pnlNote.Controls)
                    {
                        FilingUC FilingUC = (FilingUC)cc;
                        msg = es.NewFilingSetup(RegistryID, FilingUC.GetNoteID().Trim(), rules, Globals.DoctorID);

                    }
                    
                    if (pnlNote.Controls.Count == 0)
                    {
                        if (szlx == "个人")
                        {
                            MessageBox.Show("个人归档项目设置为空，将自动引用此人所属科室归档项目", "提示");
                            return;
                        }
                        else
                        {
                            MessageBox.Show("设置完成", "提示");
                            return;
                        }
                    }

                    if (msg == null)
                    {
                        MessageBox.Show("设置完成", "提示");
                        return;
                    }
                    else
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925511256895", ex.Message + ">>" + ex.ToString(), true);            
               
                    return;
                }

            }
            #endregion 
            ThisAddIn.ResetBeginTime();
        }


        private void lstNotes_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None; 
        }

        private void FilingSetup_Load(object sender, EventArgs e)
        {
            if (Globals.DoctorID == StringGeneral.supperUser && RegistryID.Length < 8)
            {
                cbDepartment.Visible = true;
                button1.Visible = true;
                XmlNode depart = ThisAddIn.GetDepartmentsByMode(gjtEmrPatients.WorkMode.InHospital);
                if (depart == null) return;
                XmlNodeList departments = depart.SelectNodes(EmrConstant.ElementNames.Department);
                cbDepartment.Items.Clear();
                //cbDepartment.Items.Add("");
                foreach (XmlNode department in departments)
                {
                    string text = department.Attributes[EmrConstant.AttributeNames.Code].Value + ":"
                        + department.Attributes[EmrConstant.AttributeNames.Name].Value;
                    cbDepartment.Items.Add(text);
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbDepartment.Text == "") return;
            string code = cbDepartment.Text.Split(':')[0];
            lbNoteName.Text = lbNoteName.Text = "科室编码：" + code;
            RegistryID = code;
            pnlNote.Controls.Clear();
            noteCount = 0;
            LoadNotesWithRules(); 
        }

    }   

}