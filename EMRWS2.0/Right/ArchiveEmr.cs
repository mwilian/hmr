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
    public partial class ArchiveEmr : Form
    {
        public string RegistryID;
        public ArchiveEmr()
        {
            InitializeComponent();
        }

        private void ArchiveEmr_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            DateTime endDate = dtpDischargedDate.Value;
            RegistryID = lsbArchiveList.Text;
            if (RegistryID == "")
            {
                MessageBox.Show("请选中需要归档的患者！", "提示");
                return;
            }

            if (JudgeNoteStatus(RegistryID) == 0) return;
            if (JudgeNoteFilingSetup(RegistryID) == 0) return;
            //JudgeNoteSign();
            
            ThisAddIn.Archive_Single(endDate, RegistryID,lsbArchiveList);
            ThisAddIn.ResetBeginTime();
        }


        public int JudgeNoteFilingSetup(string RegistryID)
        {
            DataSet notes = null;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                #region Get notes from database
                string msg = null;
                try
                {
                    if (Globals.isEndRule == false)
                    {
                        msg = es.GetNotesWithFilingSetup(RegistryID, ref notes);
                    }
                    else
                    {
                        msg = es.GetNotesWithFilingSetup(RegistryID, ref notes);
                    }
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return 0;
                    }
                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX925511256765", ex.Message + ">>" + ex.ToString(), true);            
               
                    return 0;
                }
                #endregion

                #region Add notes on pnlNote
                foreach (DataRow note in notes.Tables[0].Rows)
                {
                    string noteID = note[0].ToString().Split(':')[0];
                    string noteName = GetNoteNameFromTree(noteID);
                    //if (noteName == null) continue;
                    if (JudgeNoteID(RegistryID, noteID, noteName) == 0) return 0;
                }
                #endregion
            }
            return 1;
        }
        private string GetNoteNameFromTree(string noteID)
        {
            //foreach (TreeNode note in tvNotes.Nodes)
            //{
            //    if (note.Name.Trim() == noteID.Trim()) return note.Text;
            //}
            return null;
        }

        private void ArchiveEmr_SizeChanged(object sender, EventArgs e)
        {
            //btnOk.Left = this.Width - 10 - btnOk.Width;
            //btnOk.Top = this.Height - 35 - btnOk.Height;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            DateTime endDate = dtpDischargedDate.Value;
            ThisAddIn.Archive_Search(endDate, lsbArchiveList);
            ThisAddIn.ResetBeginTime();
        }

        public int JudgeNoteStatus(string RegistryID)
        {

            DataSet notes = null;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                #region Get notes from database
                //string msg = null;
                try
                {
                    notes = es.GetNoteStatusWithArchive(RegistryID);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925511256764", ex.Message + ">>" + ex.ToString(), true);            
               
                    return 0;
                }

                if (notes.Tables[0].Rows.Count != 0 && notes.Tables[0].Rows[0]["note"].ToString() != "")
                {
                    MessageBox.Show("病历中存在草稿状态，不能归档！", "提示");
                    return 0;
                }

                #endregion
            }
            return 1;
        }
        //public void JudgeNoteStatus()
        //{
            
        //    DataSet notes = null;
        //    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
        //    {
        //        #region Get notes from database
        //        string msg = null;
        //        try
        //        {
        //            notes = es.GetNoteStatusWithArchive(RegistryID);
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ThisAddIn.WebError(ex);
        //            return;
        //        }

        //        if (notes.Tables[0].Rows.Count != 0 && notes.Tables[0].Rows[0]["note"].ToString()!="")
        //        {
        //            MessageBox.Show("病历中存在草稿状态，不能归档！", "提示");
        //                return;
        //        }

        //        #endregion
        //    }
        //}
        private int JudgeNoteID(string RegistryID, string NoteID, string noteName)
        {

            DataSet notes = null;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                #region Get notes from database
                string msg = null;
                try
                {
                    msg = es.GetNoteIDWithArchive(RegistryID, NoteID, ref notes);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925511256763", ex.Message + ">>" + ex.ToString(), true);            
               
                    return 0;
                }

                noteName =Globals.emrPattern.GetNoteNameFromNoteID(NoteID);

                if (notes.Tables[0].Rows.Count != 0 && notes.Tables[0].Rows[0]["note"].ToString() == "")
                {
                    MessageBox.Show("病历中有缺项-" + noteName + "，不能归档！", "提示");
                    return 0;
                }

                return 1;

                #endregion
            }
        }


        private void JudgeNoteSign()
        {


//select   Document.value('(Emr/EmrNote/@noteID)[0]','varchar(50)') as noteID
//                                from EmrDocument with (rowlock)   
//where Document.value('(Emr/EmrNote/@CheckerID)[0]','varchar(50)') is null and Document.value('(Emr/EmrNote/@sign2)[0]','varchar(50)') is not null

                //string NoteID;
                //string noteName;

            DataSet notes = null;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                #region Get notes from database



                string msg = null;
                try
                {
                    //msg = es.GetNoteIDWithArchive(RegistryID, NoteID, ref notes);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925511256762", ex.Message + ">>" + ex.ToString(), true);            
               
                    return;
                }

                if (notes.Tables[0].Rows.Count != 0 && notes.Tables[0].Rows[0]["note"].ToString() == "")
                {
                    //MessageBox.Show("病历中有没有签字项-" + noteName + "，不能归档！", "提示");
                    return;
                }

                #endregion
            }
        }
    }
}