using System;
using System.Collections.Generic;

using System.Text;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using CommonLib;
using EmrConstant;
using System.Data;
using System.Collections;

namespace EMR
{
   public partial class EmrNote
    {

       NoteEditMode editModes = 0;
      
        public EmrNote(AuthorInfo authorInfo, XmlElement emrNote, NoteEditMode editMode,
            string registryID, MainForm etp)
        {
            emrTaskPane = etp;
            this.editModes = editMode;
            merge = emrNote.Attributes[AttributeNames.Merge].Value;
            if (emrNote.Attributes[AttributeNames.StartTime] != null)
                startTime = emrNote.Attributes[AttributeNames.StartTime].Value;

            noteInfo = new NoteInfo(authorInfo, registryID, false,
                emrNote.Attributes[AttributeNames.Header].Value,
                emrNote.Attributes[AttributeNames.Unique].Value);
            noteInfo.sexOption = (emrNote.Attributes[AttributeNames.Sex].Value == StringGeneral.Yes);
            sexOption = emrNote.Attributes[AttributeNames.Sex].Value;
            author = new UCAuthor(authorInfo, false,emrTaskPane);

            if (emrNote.Attributes[AttributeNames.Header].Value == StringGeneral.None)
            {
                XmlNode theader = emrNote.SelectSingleNode(ElementNames.Header);
                if (theader != null) header = theader.Clone();
            }

            noteInfo.SetEditMode(editMode);
           // SetEditModes();
        }

       public void SetEditModes()
        {
            switch (editModes)
            {
                case EmrConstant.NoteEditMode.Nothing:
                    emrTaskPane.SetReadonly();
                    break;
                case EmrConstant.NoteEditMode.Checking:
                    emrTaskPane.SetRevision2();
                    //Globals.ThisAddIn.SetRevisionColor("审核者");
                    break;
                case EmrConstant.NoteEditMode.FinallyCkecking:
                    emrTaskPane.SetRevision2();
                    //Globals.ThisAddIn.SetRevisionColor("终身者");
                    break;
                case EmrConstant.NoteEditMode.Writing:
                    if (ThisAddIn.CanOption(ElementNames.chxTrace) == true && Globals.Isfanxiu)
                    {
                        emrTaskPane.SetRevision2();
                        Globals.Isfanxiu = false;
                    }
                    else emrTaskPane.SetReadWrite();
                    break;
                case EmrConstant.NoteEditMode.Reading:
                    emrTaskPane.SetReadonly();
                    break;
            }         
        }
        public void BecomeNormalNoteNew()
        {
            noteInfo.SetNewNoteFlag(true);
        }
        public bool ElementFromBookmarks(XmlElement emrNote, Boolean newNote, EmrConstant.Button button)
        {
            if (!newNote)
            {
                for (int k = emrNote.ChildNodes.Count - 1; k >= 0; k--) emrNote.RemoveChild(emrNote.ChildNodes[k]);
            }

            string RegistryID = emrNote.OwnerDocument.ChildNodes[0].Attributes[EmrConstant.AttributeNames.RegistryID].Value;
            if (ThisAddIn.CanOption(ElementNames.EHRInterface) == true)
            {
                #region 定义健康档案接口变量
                string PatientID = "";
                string StateIn = "";
                string DiagnosisIn = "";
                string Treatment = "";
                string StateOut = "";
                string DiagnosisOut = "";
                string OrderOut = "";
                string ResidentPhysician = "";
                string PhysicianInCharge = "";
                string Department = "";
                string Bah = "";
               
                string zyh = "";
                DateTime AdmitDateTime = new DateTime();
                DateTime OutDateTime = new DateTime();
                #endregion
                string Hospital = "东方地球物理公司第二职工医院";

                using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())

                    try
                    {
                        DataSet dst = ep.GetPatientInf(RegistryID);
                        PatientID = dst.Tables[0].Rows[0]["sfzh"].ToString();

                        //Department = dst.Tables[0].Rows[0]["ksbm"].ToString();
                        Bah = dst.Tables[0].Rows[0]["bah"].ToString();
                        zyh = dst.Tables[0].Rows[0]["zyh"].ToString();
                        AdmitDateTime = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString());
                        OutDateTime = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString());

                        if (PatientID == null || PatientID == "")
                            Globals.health = false;
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX741852968", ex.Message + ">>" + ex.ToString(), true);      

                    }
                if (PatientID == null || PatientID == "")
                    Globals.health = false;
                if (Globals.health == true)
                {
                    foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
                    {

                        string content = xn.BaseName;

                        switch (content)
                        {
                            case "科室":
                                Department = xn.Range.Text.Trim();
                                break;
                            case "入院情况":
                                StateIn = xn.Range.Text.Trim();
                                break;
                            case "入院诊断":
                                DiagnosisIn = xn.Range.Text.Trim();
                                break;
                            case "诊疗经过":
                                Treatment = xn.Range.Text.Trim();
                                break;
                            case "出院情况":
                                StateOut = xn.Range.Text.Trim();
                                break;
                            case "出院诊断":
                                DiagnosisOut = xn.Range.Text.Trim();
                                break;
                            case "出院医嘱":
                                OrderOut = xn.Range.Text.Trim();
                                break;
                            case "住院医师":
                                if (xn.Range.Text != null)
                                    ResidentPhysician = xn.Range.Text.Trim();
                                break;
                            case "医师":
                                PhysicianInCharge = Globals.DoctorName;
                                break;
                            default:
                                break;
                        }

                    }
                }
                if (Globals.health == true)
                {
                    //WebReference.Hiss h = new WebReference.Hiss();
                    //{
                    //    try
                    //    {
                    //        int result = h.Insert_InpatientInfo_EMR(PatientID, AdmitDateTime, OutDateTime, zyh, StateIn, DiagnosisIn, Treatment, StateOut, DiagnosisOut, OrderOut,
                    //            ResidentPhysician, PhysicianInCharge, Department, Hospital, Bah, ref err);
                    //        result++;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //    }

                    //}
                    Globals.health = false;
                }
            }
            Word.ContentControl title = null;
            Word.ContentControl titleNext = null;
            Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.First.Range;
            ArrayList lables = new ArrayList();
            foreach (Word.ContentControl lable in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                if (lable.Title == StringGeneral.Label) lables.Add(lable);
                else if (lable.Title == StringGeneral.Control)
                {
                    //lable.LockContentControl = false;
                    //lable.LockContents = false;
                    //lable.Delete(false);
                }
            }
            #region Check empty subtitle
            /* If the content of a subtitle is empty, you should delete it, otherwise, the insignificant 
             value of a xml element will be removed by sql 2005 for a xml column. */
            if (button == EmrConstant.Button.CommitNote)
            {
                /* When commit note, required subtitle must not be empty. */
                for (int i = lables.Count - 1; i > 0; i--)
                {
                    title = (Word.ContentControl)lables[i - 1];
                    string required = title.Tag.ToString();
                    titleNext = (Word.ContentControl)lables[i];
                    range.Start = title.Range.End + 1;
                    range.End = titleNext.Range.Start - 1;


                    //20100128
                    if (required == StringGeneral.Yes)
                    {
                        if (range.Text != null)
                        {
                            //if (range.Text.Contains("选择一项"))
                            //{
                            //    MessageBox.Show(title.Range.Text + " -- 是必须要填写的！", ErrorMessage.Warning);
                            //    range.Select();
                            //    return Return.Failed;
                            //}
                        }
                    }




                    if (!IsSignificant(range.Text))
                    {
                        #region Empty subtitle
                        if (required == StringGeneral.Yes)
                        {
                            /* Required subtitle must has content. */
                            //MessageBox.Show(title.Range.Text + " -- 是必须要填写的！", ErrorMessage.Warning);
                            //range.Select();
                            //return Return.Failed;
                        }
                        else
                        {
                            /* Not required empty subtitle is removed. */
                            //range.Text = "";
                            //title.LockContentControl = false;
                            //title.LockContents = false;
                            //title.Delete(true);
                            //lables.RemoveAt(i - 1);
                            title.LockContents = false;
                            ///guojt 2009/7/15
                            title.Range.Font.Color = Globals.labelFont.Color;
                            title.LockContents = true;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Not required empty subtitle needs change font color with that of the required
                        if (required == EmrConstant.StringGeneral.No)
                        {
                            title.LockContents = false;
                            title.Range.Font.Color = Globals.labelFont.Color;
                            title.LockContents = true;
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region Fill subtitel into emrNote
            for (int i = 0; i < lables.Count - 1; i++)
            {
                title = (Word.ContentControl)lables[i];
                titleNext = (Word.ContentControl)lables[i + 1];
                range.Start = title.Range.End + 1;
                range.End = titleNext.Range.Start - 1;
                XmlElement subTitle = emrNote.OwnerDocument.CreateElement(ElementNames.SubTitle);
                ElementFromBookmarkMe(subTitle, range);
                subTitle.SetAttribute(AttributeNames.TitleName, title.Range.Text);
                /* 2007-07-20 using doubale storage: word document and xml emrDocument. In emrDocuments,
                 only text is allowed. */
                //ElementFromTables(doc, subTitle, range);
                //ElementFromImages(doc, subTitle, range);

                emrNote.AppendChild(subTitle);
            }
            #endregion

            //XmlElement revisions = doc.CreateElement(EmrConstant.ElementNames.Revisions);
            //range = ActiveDocumentManager.getDefaultAD().Content;
            //RevisionFromBookmark(doc, range, ref revisions);
            //emrNote.AppendChild(revisions);

            return EmrConstant.Return.Successful;
        }
        private void ElementFromBookmarkMe(XmlElement subtitle, Word.Range range)
        {
            Word.Range rangeTmp = null;

            if (subtitle.ChildNodes.Count > 0) subtitle.IsEmpty = true;

            if (range.Tables.Count > 0)
            {
                #region Has tables
                ArrayList tableRangeStart = new ArrayList();
                ArrayList tableRangeEnd = new ArrayList();
                foreach (Word.Table table in range.Tables)
                {
                    int start = table.Range.Start;
                    tableRangeStart.Add(start);
                    tableRangeEnd.Add(table.Range.End);
                }
                tableRangeStart.Sort();
                tableRangeEnd.Sort();
                /* Text of this range from start of range to end of first table range. */
                object textStart = range.Start;
                object textEnd = tableRangeStart[0];
                rangeTmp = ActiveDocumentManager.getDefaultAD().Range(ref textStart, ref textEnd);
                if (rangeTmp.Text.Length > 0) subtitle.InnerText = TrimLineFeed(rangeTmp.Text);
                /* If there are more table in the range. */
                for (int k = 1; k < tableRangeStart.Count; k++)
                {
                    /* Text of range between two tables.*/
                    rangeTmp.Start = (int)tableRangeEnd[k - 1];
                    rangeTmp.End = (int)tableRangeStart[k];
                    if (rangeTmp.Text.Length > 0) subtitle.InnerText += TrimLineFeed(rangeTmp.Text);
                }
                /* Text of from start of first table range to end of this range. */
                rangeTmp.Start = (int)tableRangeEnd[tableRangeStart.Count - 1];
                rangeTmp.End = range.End;
                if (rangeTmp.Text != string.Empty) subtitle.InnerText += TrimLineFeed(rangeTmp.Text);
                #endregion
            }
            else
            {
                if (range.Text != string.Empty) subtitle.InnerText = TrimLineFeed(range.Text);
            }
        }
        private string TrimLineFeed(string orgtext)
        {
            string result = null;
            if (orgtext != null)
            {
                char[] tmp = orgtext.ToCharArray();
                for (int k = 0; k < tmp.Length; k++)
                {
                    if (tmp[k] == '\r') continue;
                    if (tmp[k] == ' ') continue;
                    result += tmp[k];
                }
            }
            return result;
        }
        private bool IsSignificant(string text)
        {
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] != ' ' && text[i] != '\r') return true;
                }
            }
            return false;
        }
    }
}
