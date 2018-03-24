using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using EmrConstant;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using CommonLib;
using System.Xml;
using System.IO;
using HuronControl;
using HuronControl.Document;
using System.Threading;

namespace EMR
{
    public partial class MainForm
    {
        private AuthorInfo MakeAuthorInfo(string noteID, string noteName)
        {
            EmrConstant.AuthorInfo authorInfo;
            authorInfo.Writer = Globals.DoctorName;
            authorInfo.WrittenDate = ThisAddIn.Today().ToString(EmrConstant.StringGeneral.DateFormat);
            authorInfo.Checker = "";
            authorInfo.CheckedDate = "";
            authorInfo.FinalChecker = "";
            authorInfo.FinalCheckedDate = "";
            authorInfo.TemplateType = EmrConstant.StringGeneral.NoneTemplate;
            authorInfo.TemplateName = "";
            authorInfo.NoteID = noteID;
            authorInfo.ChildID = string.Empty;
            authorInfo.NoteName = noteName;
            authorInfo.WriterLable = "";
            authorInfo.CheckerLable = "";
            authorInfo.FinalCheckerLable = "";
            return authorInfo;
        }

        #region Header and footer
        public Word.ContentControl DrawNoteName(string noteID, string noteName, Word.Range range)
        {
            Word.ContentControl ccX = null;
            range.Start = range.End;
            range.Select();

            if (Globals.patientInfoAtPageHeader)
            {
                ActiveDocumentManager.getDefaultAD().Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = noteName;
            }
            else
            {
                string noteNamePad = NoteNamePad(noteName);
                ccX = AddContentControlText(range, noteNamePad, noteID,
                    false, Globals.headerFontSize, Globals.headerFontColor);
                ccX.Tag = StringGeneral.RemoveFlag;
                ccX.Range.Font = Globals.noteNameFont;
                ccX.LockContents = !ThisAddIn.CanOption(ElementNames.CanEditHeader);
                ccX.LockContentControl = !ThisAddIn.CanOption(ElementNames.CanEditHeader);
            }
            return ccX;
        }

        public string NoteNamePad(string noteName)
        {
            Globals.countPerLine = CountPerLine(Globals.noteNameFont.Size);
            int spaceCount = (Globals.countPerLine - noteName.Length);
            return noteName.PadLeft(spaceCount + noteName.Length, Globals.padChar);
        }

        public Word.ContentControl AddContentControlText(Word.Range range, string text, string title,
         bool locking, float size, Word.WdColor color)
        {
            range.Select();
            Word.ContentControl cc = null;
            object orange = (object)range;
            cc = ActiveDocumentManager.getDefaultAD().ContentControls.Add(
                Word.WdContentControlType.wdContentControlText, ref orange);
            cc.Range.Text = text;
            cc.Title = title;
            cc.Range.Font.Size = size;
            cc.Range.Font.Color = color;
            cc.LockContentControl = locking;
            cc.LockContents = locking;
            return cc;
        }
        public int CountPerLine(float fontSize)
        {
            int width = wordApp.ActiveWindow.Panes[1].Pages[1].Width;
            //width = (int)pageWidth16k;
            float margin = ActiveDocumentManager.getDefaultAD().PageSetup.LeftMargin +
                ActiveDocumentManager.getDefaultAD().PageSetup.RightMargin;
            return (int)(((float)(width) - margin) / (fontSize));
        }
        #endregion

        //public ContentControl DrawSmallHeader(PatientInfo patientInfo, Range range)
        //{
        //    string headerText = MakeSmallHedaerText(patientInfo);
        //    if (Globals.patientInfoAtPageHeader)
        //    {
        //        #region paitent info as page header
        //        ActiveDocumentManager.getDefaultAD().Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text += "\r" + headerText;

        //        return null;
        //        #endregion
        //    }
        //    else
        //    {
        //        #region patient info. as note content
        //        ContentControl ccX = null;
        //        ccX = AddContentControlText(range, headerText, AttributeNames.Header, false, Globals.headerFontSize, Globals.headerFontColor);
        //        ccX.Tag = StringGeneral.RemoveFlag;
        //        ccX.Range.Font = Globals.headerFont;
        //        ccX.LockContents = !ThisAddIn.CanOption(ElementNames.CanEditHeader);
        //        ccX.LockContentControl = !ThisAddIn.CanOption(ElementNames.CanEditHeader);
        //        return ccX;
        //        #endregion
        //    }
        //}

    
        public string MakeSmallHedaerText(PatientInfo patientInfo)
        {

            string headerText = null;
            if (!ThisAddIn.CanOption(ElementNames.ArchiveNum))
            {
                if (ThisAddIn.CanOption(ElementNames.RegistryID)) headerText = "住院号：" + patientInfo.RegistryID + " ";
                else headerText = "病案号：" + patientInfo.ArchiveNum + " ";
            }
            
            headerText += "姓名：" + patientInfo.PatientName;
            headerText += " 性别：" + patientInfo.Sex;
            headerText += " 年龄：" + patientInfo.Age;
            headerText += patientInfo.AgeUnit;
            string departmentName = udt.jj.GetDepartmentNameFromCode(patientInfo.DepartmentCode, Globals.departmentFile);
            headerText += " 科室：" + departmentName;
            if (Globals.inStyle)
            {
                headerText += " 床号：" + patientInfo.BedNum;
            }
            return headerText;
        }
        public Word.ContentControl AddSubTitle(Word.Range range, string text, Word.WdColor color)
        {
            range.Select();
            Word.ContentControl cc = null;
            object orange = (object)range;
            cc = ActiveDocumentManager.getDefaultAD().ContentControls.Add(
                Word.WdContentControlType.wdContentControlText, ref orange);
            cc.Range.Text = text;
            cc.Title = StringGeneral.Label;
            cc.Range.Font = Globals.labelFont;
            cc.Range.Font.Color = color;
            cc.LockContentControl = true;
            cc.LockContents = true;
            return cc;
        }

        public Word.ContentControl AddContentControlTextRight(Word.Range range, string text, string title,
          bool locking, float size, Word.WdColor color)
        {
            range.Select();
            if (ThisAddIn.CanOption(ElementNames.SignRight) == true)
            {
                range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            }
            Word.ContentControl cc = null;
            object orange = (object)range;
            cc = ActiveDocumentManager.getDefaultAD().ContentControls.Add(
                Word.WdContentControlType.wdContentControlText, ref orange);
            cc.Range.Text = text;
            cc.Title = title;
            cc.Range.Font.Size = size;
            cc.Range.Font.Color = color;
            cc.LockContentControl = locking;
            cc.LockContents = locking;
            return cc;
        }

        public void OpenEmrDocument(TreeNode registryNode)
        {
            /* index(registryNode.Tag) >= 0 directs an emrDocument */
            //if (Convert.ToInt32(registryNode.Tag) >= 0) return;  // is open
            registryNode.Nodes.Clear();
            string registryID, chargingDoctorID;
            if (!ParserRegistry(registryNode.Text, out registryID, out chargingDoctorID)) return;          
            string operatorID = Globals.DoctorID;
            Globals.RegistryID = registryID;
            /* Determine the permission level of the operator. */
            PermissionLevel permissionLevel = PermissionLevel.ReadWrite;
            if (Globals.inStyle && !Globals.offline)
            {
                #region Inpatient
                #region Truster info
                string msgTruster = null;
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        msgTruster = es.IsTruster(operatorID, chargingDoctorID);
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX841852970", ex.Message + ">>" + ex.ToString(), true);            
                        return;
                    }
                }
                #endregion
                if (msgTruster == null)
                {
                    permissionLevel = EmrConstant.PermissionLevel.Trust;
                }
                else
                {                   
                    /* not charging */
                    if (Globals.offline)
                    {
                        /* In offline, only charging doctor(or his truster) can edit notes.*/
                        permissionLevel = PermissionLevel.ReadOnly;                      
                    }
                    else
                    {
                        #region Determine upper doctor
                        if (ThisAddIn.IsUpperDoctor(operatorID, chargingDoctorID))
                        {
                            permissionLevel = PermissionLevel.RevisionOnly;
                        }
                        else
                        {
                            #region Determin upper upper doctor
                            if (ThisAddIn.IsUpperUpperDoctor(operatorID, chargingDoctorID))
                            {
                                permissionLevel = PermissionLevel.FinalRevisionOnly;
                            }
                            else
                            {
                                #region Not the upper doctor and not upper upper doctor
                                if (Globals.auditSystem.Substring(0, 1) == EmrConstant.Auditing.LevelA)
                                {
                                    /* Any one can read and write the emr for any patients. */
                                    //permissionLevel = EmrConstant.PermissionLevel.ReadWrite;
                                }
                                else if (Globals.auditSystem.Substring(0, 1) == EmrConstant.Auditing.LevelB)
                                {
                                    permissionLevel = EmrConstant.PermissionLevel.ReadOnly;
                                }
                                else
                                {
                                    #region Granted read privilege?
                                    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                                    {
                                        try
                                        {
                                            if (es.HavePrivilegeForRegistry(registryID, operatorID))
                                            {
                                                permissionLevel = EmrConstant.PermissionLevel.ReadOnly;
                                            }
                                            else
                                            {
                                                /* Level B, level D and level F can not read for others */
                                                permissionLevel = EmrConstant.PermissionLevel.NoPrivilege;
                                                MessageBox.Show(EmrConstant.ErrorMessage.NoPrivilege,
                                                    EmrConstant.ErrorMessage.Warning);
                                                return;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Globals.logAdapter.Record("EX771852970", ex.Message + ">>" + ex.ToString(), true); 
           
                                            return;
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    //}
                }
                #endregion
                //}
            }

            if (!ThisAddIn.CanExeutiveCommandQuiet(MyMenuItems.NewNote))
                permissionLevel = PermissionLevel.ReadOnly;

            /* Create a xml document and fill it with data from dadabase. */
            int index = emrDocuments.open(registryID, permissionLevel, registryNode.Parent.Tag.ToString());
            if (index < 0)
                 return;            
            /* level 1 node.name is emr status 
             * level 1 node.tag directs an emr document */
            registryNode.Tag = index;            
            registryNode.ForeColor = Globals.emrOpeningColor;
            XmlDocument doc = emrDocuments.Get(index);
            registryNode.Name = doc.DocumentElement.Attributes[AttributeNames.EmrStatus].Value;
            /* Treeview adds nodes for level 2 and level 3 if any. */
            tvPatientsNoteIDLoad(registryNode);       
 
        }
        private void LoadMedical(TreeNode node)
        {
            node.Nodes.Add("住院病案首页");
            node.Nodes[0].Tag = "Yes";
            node.Nodes[0].Name = "0";
            node.Nodes[0].Nodes.Add("住院病案首页");
            node.Nodes[0].Nodes[0].Name = "5";
        }
        public void tvPatientsNoteIDLoad(TreeNode node)
        {
            /* node.Tag directs to an emrDocument */
            int index = (int)node.Tag;
            XmlDocument emrDoc = emrDocuments.Get(index);
            if (emrDoc == null) return;
            XmlNodeList emrNotes = emrDoc.DocumentElement.SelectNodes(EmrConstant.ElementNames.EmrNote);
          //  MessageBox.Show(emrNotes.Count.ToString());
            //按提交时间排序
            if (ThisAddIn.CanOption(ElementNames.CommitTime) && emrNotes.Count > 0)
            {
                XmlNode[] Notes = new XmlNode[emrNotes.Count];
                Notes[0] = emrNotes[0];
                for (int i = 1; i < emrNotes.Count; i++)
                {
                    Notes[i] = emrNotes[i];
                    SortNote(i, ref Notes);
                }
                if (ThisAddIn.CanOption(ElementNames.BA))
                LoadMedical(node);//加载病案首页               
                return;
            }
            if (ThisAddIn.CanOption(ElementNames.BA))             
            LoadMedical(node);//加载病案首页
            foreach (XmlNode emrNote in emrNotes)
            {

                TreeNode nodeLevel2 = null;
                string noteNameInPattern =
                    Globals.emrPattern.GetNoteNameFromNoteID(emrNote.Attributes[AttributeNames.NoteID].Value);
                if (noteNameInPattern == null)
                {
                    //MessageBox.Show(noteNameInPattern = emrNote.Attributes[AttributeNames.NoteName].Value);
                    noteNameInPattern = emrNote.Attributes[AttributeNames.NoteName].Value;
                }
                    
                NewLevel2(emrNote.Attributes[AttributeNames.NoteID].Value, noteNameInPattern,
                    emrNote.Attributes[AttributeNames.Unique].Value, node, out nodeLevel2);
                nodeLevel2.Nodes.Add(emrNote.Attributes[EmrConstant.AttributeNames.WrittenDate].Value);
                nodeLevel2.LastNode.Name = emrNote.Attributes[EmrConstant.AttributeNames.NoteStatus].Value;
                ////刘伟加入作废标示显示已作废
                //if (emrNote.Attributes["RealName"] != null)
                //{
                //    if (emrNote.Attributes["zuofei"] == null) nodeLevel2.LastNode.Text = emrNote.Attributes["RealName"].Value + NoteState(nodeLevel2.LastNode.Name) + " " + nodeLevel2.LastNode.Text;
                //    else
                //    {
                //        nodeLevel2.LastNode.Text = emrNote.Attributes["RealName"].Value + " " + "已作废" + " " + nodeLevel2.LastNode.Text;
                //    }
                //}
                //else
                //{
                //    if (emrNote.Attributes["zuofei"] == null) nodeLevel2.LastNode.Text += NoteState(nodeLevel2.LastNode.Name);
                //    else
                //    {
                //        nodeLevel2.LastNode.Text += " 已作废";
                //    }
                //} 
              
                if (emrNote.Attributes["RealName"] != null && !string.IsNullOrEmpty(emrNote.Attributes["RealName"].Value.ToString()))
                {
                    nodeLevel2.LastNode.Text = emrNote.Attributes["RealName"].Value + NoteState(nodeLevel2.LastNode.Name) + nodeLevel2.LastNode.Text;
                }
                else
                {
                    nodeLevel2.LastNode.Text = noteNameInPattern + NoteState(nodeLevel2.LastNode.Name) + nodeLevel2.LastNode.Text;
                }
                nodeLevel2.LastNode.Tag = emrNote.Attributes[EmrConstant.AttributeNames.Series].Value;
                nodeLevel2.LastNode.ImageIndex = 5;
            }
            try
            {
                if (ThisAddIn.CanOption(ElementNames.ClinicPath) && ThisAddIn.CanOptionText(ElementNames.ClinicPath).Trim() != "")//如果开启临床路径
                    LoadClinicPath(node, emrDoc.DocumentElement.Attributes[AttributeNames.RegistryID].Value);//加载临床路径表单

            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX744852970", ex.Message + ">>" + ex.ToString(), true); 
           
            }
        }
        private void LoadClinicPath(TreeNode node, string registryID)
        {
            string noteID = ThisAddIn.CanOptionText(ElementNames.PathID).Trim();
            TreeNode level2 = FindNoteIDNode(tvPatients.SelectedNode, noteID);
            if (level2 == null)
            {
                //查询该患者下是否有临床路径表单
                //string zyh = "00009639";
                string zyh = registryID;
                string[] docname = null, jdxh = null;
                GetLc(zyh, ref jdxh, ref docname);
                if (docname != null)
                {
                   // Globals.newPath = true;
                    node.Nodes.Add("临床路径表单");
                    node.Nodes[node.LastNode.Index].Tag = registryID;
                    node.Nodes[node.LastNode.Index].Name = ThisAddIn.CanOptionText(ElementNames.PathID).Trim();
                    node.Nodes[node.LastNode.Index].Nodes.Add("临床路径表单");
                    if (ThisAddIn.CanOptionText(ElementNames.ClinicPath).Trim() != "")
                    {
                        node.Nodes[node.LastNode.Index].Nodes[0].Name = ThisAddIn.CanOptionText(ElementNames.ClinicPath).Trim();
                        //node.Nodes[node.LastNode.Index].Nodes[0].Tag = "";
                    }
                }
            }
        }
        public bool Updatelclj(string zyh, string[] jdxh, DateTime datetime)
        {
            using (gjtEmrService.emrServiceXml Service = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    bool result = false;
                    result = Service.Updatelclj(zyh, jdxh, datetime);
                    return result;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741862970", ex.Message + ">>" + ex.ToString(), true); 
           
                    return false;
                }
            }

        }
        public XmlElement GetEmrNote(int index, int series)
        {
            XmlNode emrNote = emrDocuments.GetEmrDocument(index).GetEmrNoteBySeries(series);
            return (XmlElement)emrNote;
        }
        private EmrStatus GetEmrStatus()
        {
            switch (tvPatients.SelectedNode.Level)
            {
                case 1:
                    return (EmrConstant.EmrStatus)Convert.ToInt32(tvPatients.SelectedNode.Name);

                case 2:
                    return (EmrConstant.EmrStatus)Convert.ToInt32(tvPatients.SelectedNode.Parent.Name);

                case 3:
                    return (EmrConstant.EmrStatus)Convert.ToInt32(tvPatients.SelectedNode.Parent.Parent.Name);
                default:
                    return (EmrConstant.EmrStatus)Convert.ToInt32(tvPatients.SelectedNode.FirstNode.Name);

            }
        }
        private NoteEditMode EditModeWhenNoteDraft(PermissionLevel permission, string writerID)
        {
            if (Globals.DoctorID == writerID)
            {
                return EmrConstant.NoteEditMode.Writing;
            }
            else
            {
                if (Globals.auditSystem.Substring(0, 1) == EmrConstant.Auditing.LevelA)
                    return EmrConstant.NoteEditMode.Reading;
                else
                    return EmrConstant.NoteEditMode.Nothing;
            }
        }
        private NoteEditMode EditModeWhenNoteCommited(PermissionLevel permission, string writerID)
        {
            string opcode = Globals.DoctorID;

            switch (permission)
            {
                case EmrConstant.PermissionLevel.ReadWrite:                    
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.ReadOnly:
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.RevisionOnly:
                    if (opcode == writerID && ThisAddIn.CanOption(ElementNames.RevisionBy) == false) return EmrConstant.NoteEditMode.Reading;
                    else return EmrConstant.NoteEditMode.Checking;
                case EmrConstant.PermissionLevel.FinalRevisionOnly:
                    if (Globals.auditSystem.Substring(0, 1) == EmrConstant.Auditing.LevelA)
                    {
                        if (opcode == writerID && ThisAddIn.CanOption(ElementNames.RevisionBy) == false) return EmrConstant.NoteEditMode.Reading;
                        else return EmrConstant.NoteEditMode.Checking;
                    }
                    else
                    {
                        return EmrConstant.NoteEditMode.Reading;
                    }
                case EmrConstant.PermissionLevel.Trust:
                    if (ThisAddIn.IsUpperDoctor(Globals.DoctorID, writerID))
                        return EmrConstant.NoteEditMode.Checking;
                    else
                        return EmrConstant.NoteEditMode.Reading;
                default:
                    return EmrConstant.NoteEditMode.Nothing;
            }
        }
        /* ------------------------------------------------------------------------------------ */
        private NoteEditMode EditModeWhenNoteChecking(PermissionLevel permission, string checkerID)
        {
            string opcode = Globals.DoctorID;
            if (opcode == checkerID) return EmrConstant.NoteEditMode.Checking;
            else return EmrConstant.NoteEditMode.Reading;
            
        }
        public  bool IsSigned(Word.Document doc)
        {
            foreach (Office.Signature sign in doc.Signatures)
            {
                if (sign.IsSigned) return true;
            }
            return false;
        }
        private void SetWrittenDate(DateTime date)
        {
            if (!ThisAddIn.CanOption(ElementNames.ShowWritenDate)) return;

            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                if (cc.Title == AttributeNames.WrittenDate)
                {
                    cc.LockContents = false;
                    cc.Range.Text = " " + date.ToString(StringGeneral.DateFormat);
                    cc.LockContents = true;
                    return;
                }
            }
        }
        private void SetWriter(string writer)
        {
            if (!ThisAddIn.CanOption(ElementNames.ShowWriter)) return;
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                if (cc.Title == AttributeNames.Writer)
                {
                    SetReadWrite();
                    cc.LockContents = false;
                    cc.Range.Text = writer;
                    cc.LockContents = true;
                    SetRevision();
                    return;
                }
            }
        }
        public void SetReadWrite()
        {
            
            if (wordApp.Documents.Count == 0) return;
            ActiveDocumentManager.getDefaultAD().ShowRevisions = false;
            if (ActiveDocumentManager.getDefaultAD().ProtectionType != Word.WdProtectionType.wdNoProtection)
                ActiveDocumentManager.getDefaultAD().Unprotect(ref Globals.passwd);
            ProtectDoc();
          
        }
        private void SetChecker(string checker)
        {            
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                try
                {
                    if (cc.Title == AttributeNames.Checker)
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowWriter))
                        {
                            SetReadWrite();
                            cc.LockContents = false;
                            cc.Range.Text = checker;
                            cc.LockContents = true;
                            SetRevision();
                        }
                        if (ThisAddIn.CanOption(ElementNames.EncryptSign) == true)
                        {
                            SetReadWrite();
                            cc.LockContents = false;
                            cc.Range.Text = cc.Range.Text.Trim();
                            Word.WdColor fontColor = Globals.headerFontColor;
                            float fontSize = Globals.headerFontSize;
                            Word.Range range = wordApp.Selection.Range;
                            range.Start = cc.Range.End + 1;
                            //Globals.ThisAddIn.emrTaskPane.ShowWriterSign(range, fontSize, fontColor, "CSign");
                            cc.LockContents = true;
                            object count = 1;
                            object dummy = System.Reflection.Missing.Value;
                            object Unit = Word.WdUnits.wdCharacter;
                            wordApp.Selection.MoveLeft(ref Unit, ref count, ref dummy);
                            SetRevision();
                        }
                    }
                }
                catch(Exception ex)
                {
                    Globals.logAdapter.Record("EX941852970", ex.Message + ">>" + ex.ToString(), true);            
                }
            }
        }
        private void SetCheckedDate()
        {
            if (!ThisAddIn.CanOption(ElementNames.ShowWritenDate)) return;

            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                if (cc.Title == AttributeNames.CheckedDate)
                {
                    SetReadWrite();
                    cc.LockContents = false;
                    cc.Range.Text = " " + ThisAddIn.Today().ToString(StringGeneral.DateFormat);
                    cc.LockContents = true;
                    SetRevision();
                    return;
                }
            }
        }
        private void SetFinalChecker(string finalChecker)
        {           
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {

                if (cc.Title == EmrConstant.AttributeNames.FinalChecker)
                {
                    if (ThisAddIn.CanOption(ElementNames.ShowWriter))
                    {
                        SetReadWrite();
                        cc.LockContents = false;
                        cc.Range.Text = finalChecker;
                        cc.LockContents = true;
                        SetRevision();
                    }
                    if (ThisAddIn.CanOption(ElementNames.EncryptSign) == true)
                    {
                        SetReadWrite();
                        cc.LockContents = false;
                        cc.Range.Text = cc.Range.Text.Trim();
                        Word.WdColor fontColor = Globals.headerFontColor;
                        float fontSize = Globals.headerFontSize;
                        Word.Range range = wordApp.Selection.Range;
                        range.Start = cc.Range.End + 1;
                        //Globals.ThisAddIn.emrTaskPane.ShowWriterSign(range, fontSize, fontColor, "FSign");
                        cc.LockContents = true;
                        object count = 1;
                        object dummy = System.Reflection.Missing.Value;
                        object Unit = Word.WdUnits.wdCharacter;
                        wordApp.Selection.MoveLeft(ref Unit, ref count, ref dummy);
                        SetRevision();
                    }
                    break;
                }
            }
        }
        private void SetFinalCheckedDate()
        {
            if (!ThisAddIn.CanOption(ElementNames.ShowWritenDate)) return;
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                if (cc.Title == AttributeNames.FinalCheckedDate)
                {
                    SetReadWrite();
                    cc.LockContents = false;
                    cc.Range.Text = " " + ThisAddIn.Today().ToString(StringGeneral.DateFormat);
                    cc.LockContents = true;
                    SetRevision();
                    break;
                }
            }
        }
        public void TvPatientsEnable(bool status)
        {
            tvPatients.Visible = status;
            
        }
        public bool IsTvPatientsEnable()
        {
            return tvPatients.Enabled;
        }
        //续写记录
        public void NewLineVisble(bool Vis)
        {
           // NewLine.Visible = Vis;
            noteSave.Visible = !Vis;
            RefreshInfo.Visible = !Vis;
        }
        public void NewLineVisble(object sender, EventArgs e, bool Vis)
        {
            //NewLine.Visible = Vis;
            noteSave.Visible = !Vis;
            RefreshInfo.Visible = !Vis;
            //NewLine_Click(sender, e);
        }
        //  质检评分
        public void CanValuateNote(bool value)
        {
            //noteValuate.Enabled = value && Globals.valuateEmr;
        }
        public void ShowRevision()
        {
            cbTrack.Checked = true;
            wordApp.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;
        }

        

        public void OperationEnable(bool readOnly)
        {
            if (readOnly)
            {
                noteSave.Visible = false;
                RefreshInfo.Visible = false;
                noteCommit.Visible = false;
                noteRef.Visible = false;
                //referBlock.Visible = false;
                referOrder.Visible = true;
                referPhrase.Visible = false;
                asPersonTemplate.Visible = true;
                asPersonTemplate2.Visible = true;
                string a = "";
                string b = "";
                string code = GetWriterInf(ref a, ref b);
                using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
                {
                    DataSet dst = pi.GetOpInf(code);
                    DataSet dsts = pi.GetOpInf(Globals.DoctorID);
                    if (dst.Tables[0].Rows.Count > 0)
                    {
                        if (dst.Tables[0].Rows[0]["TecqTitle"].ToString().Trim().Equals("实习医师"))
                        {
                            if ((!dsts.Tables[0].Rows[0]["TecqTitle"].ToString().Equals("实习医师")) && dsts.Tables[0].Rows[0]["DoctorType"].ToString().Equals("1"))
                            {
                                noteSave.Visible = true;
                                RefreshInfo.Visible = true;
                                noteCommit.Visible = true;
                            }
                        }
                    }
                }
                if (ThisAddIn.CanOption(ElementNames.TemplateRight) == true)
                {
                    TitleLevel tl = Globals.doctors.GetTitleLevel(Globals.DoctorID);
                    if ((tl == TitleLevel.ChiefDoctor) ||
                        (tl == TitleLevel.ViceChiefDoctor) || (tl == TitleLevel.AttendingDoctor))
                    {
                        asDepartTemplate.Visible = true;
                        asDepartTemplate2.Visible = true;
                        asHospitalTemplate2.Visible = true;
                        asHospitalTemplate.Visible = true;
                    }
                    else
                    {
                        asDepartTemplate.Visible = false;
                        asDepartTemplate2.Visible = false;
                        asHospitalTemplate.Visible = false;
                        asHospitalTemplate2.Visible = false;
                    }
                }
                else
                {
                    if (ThisAddIn.CanExeutiveCommandQuiet(MyMenuItems.AsDepartTemplates))
                    {
                        asDepartTemplate.Visible = true;
                        asDepartTemplate2.Visible = true;
                    }
                    else
                    {
                        asDepartTemplate.Visible = false;
                        asDepartTemplate2.Visible = false;
                    }
                    if (ThisAddIn.CanExeutiveCommandQuiet(MyMenuItems.AsHospitalTemplates))
                    {
                        asHospitalTemplate.Visible = true;
                        asHospitalTemplate2.Visible = true;
                    }
                    else
                    {
                        asHospitalTemplate.Visible = false;
                        asHospitalTemplate2.Visible = false;
                    }
                }
                saveTemplate.Visible = false;
                Icd10.Visible = false;
                //signature.Enabled = false;              
            }
            else
            {
                noteSave.Visible = true;
                RefreshInfo.Visible = true;
                noteCommit.Visible = true;
                noteRef.Visible = true;
                //referBlock.Visible = true;
                referOrder.Visible = true;
                referPhrase.Visible = true;
                asPersonTemplate.Visible = true;
                asPersonTemplate2.Visible = true;
                using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
                {
                    try
                    {
                        DataSet dst = pi.GetOpInf(Globals.DoctorID);
                        if (dst.Tables[0].Rows.Count > 0)
                        {
                            if (dst.Tables[0].Rows[0]["TecqTitle"].ToString().Trim().Equals("实习医师"))
                                noteCommit.Visible = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX944852970", ex.Message + ">>" + ex.ToString(), true); 
           
                    }
                }
                if (ThisAddIn.CanOption(ElementNames.TemplateRight) == true)
                {
                    TitleLevel tl = Globals.doctors.GetTitleLevel(Globals.DoctorID);
                    if ((tl == TitleLevel.ChiefDoctor) ||
                        (tl == TitleLevel.ViceChiefDoctor) || (tl == TitleLevel.AttendingDoctor))
                    {
                        asDepartTemplate.Visible = true;
                        asHospitalTemplate.Visible = true;
                            asDepartTemplate2.Visible = true;
                        asHospitalTemplate2.Visible = true;
                    }
                    else
                    {
                        asDepartTemplate.Visible = false;
                        asHospitalTemplate.Visible = false;
                        asDepartTemplate2.Visible = false;
                        asHospitalTemplate2.Visible = false;
                    }
                }
                else
                {
                   
                     if (ThisAddIn.CanExeutiveCommandQuiet(MyMenuItems.AsDepartTemplates))
                    {
                        asDepartTemplate.Visible = true;
                        asDepartTemplate2.Visible = true;
                    }
                    else
                    {
                        asDepartTemplate.Visible = false;
                        asDepartTemplate2.Visible = false;
                    }
                    if (ThisAddIn.CanExeutiveCommandQuiet(MyMenuItems.AsHospitalTemplates))
                    {
                        asHospitalTemplate.Visible = true;
                        asHospitalTemplate2.Visible = true;
                    }
                    else
                    {
                        asHospitalTemplate.Visible = false;
                        asHospitalTemplate2.Visible = false;
                    }
                }
                saveTemplate.Visible = false;
                Icd10.Visible = true;
                //signature.Enabled = true && Globals.useDigitalSignature;                          
            }
        }

        public bool IsOpenForReadonly(int index)
        {
            return emrDocuments.GetPermission(index) == PermissionLevel.ReadOnly;
        }
        public bool NoteCommiting()
        {
           if (NoteUpdateCommit() == EmrConstant.Return.Failed) return EmrConstant.Return.Failed;           
            ActiveDocumentManager.getDefaultAD().Saved = true;
            return EmrConstant.Return.Successful;
        }
        private bool NoteUpdateCommit()
        {
            switch (currentNote.noteInfo.GetEditMode())
            {
                case NoteEditMode.Writing:                   
                    if (currentNote.noteInfo.GetNoteID() == "29")
                        Globals.health = true;
                    if (Globals.Sign)
                    {
                        if (!ImageSign("医师")) Globals.Sign = false;
                    }
                    else if (ThisAddIn.CanOption(ElementNames.ShowWriter)) WriterSign("医师");
                    else WriterSign("医师", "");
                    return AddOrUpdate(NoteStatus.Commited);
                case NoteEditMode.Checking:
                    if (Globals.Sign)
                    {
                        if (!ImageSign("审核者")) Globals.Sign = false;
                    }
                    else if (ThisAddIn.CanOption(ElementNames.ShowWriter)) WriterSign("审核者");
                    else WriterSign("审核者", "");
                    return AddOrUpdate(NoteStatus.Checked);
                case NoteEditMode.FinallyCkecking:
                    if (Globals.Sign)
                    {
                        if (!ImageSign("终审者")) Globals.Sign = false;
                    }
                    else if (ThisAddIn.CanOption(ElementNames.ShowWriter)) WriterSign("终审者");
                    else WriterSign("终审者", "");
                    return AddOrUpdate(NoteStatus.FinallyChecked);
            }
            return Return.Failed;
        }
        //2012-03-20
        private bool AddOrUpdate(NoteStatus status)
        {
            bool FaileOrSuccess = false;
            bool newNote = currentNote.noteInfo.GetNewNote();
            EmrDocInfo.setIsNewForCheck(newNote);
             if (newNote)
            {
               // EmrDocInfo.setIsNewAndNoteIDForCheck(newNote, currentNote.noteInfo.GetNoteID());
      
                string noteID = currentNote.noteInfo.GetNoteID();
                EmrDocInfo.setNoteIDForCheck(noteID);
                string uniqueText = currentNote.noteInfo.GetNoteUniqueFlag();
                if (noteID == "29") Globals.health = true;
                if (tvPatientsNoteAdd(uniqueText, NoteStatus.Commited, EmrConstant.Button.CommitNote)
                    == Return.Successful) FaileOrSuccess = Return.Successful;
                else currentNote.noteInfo.SetNewNote(false);                
            }
            else 
            {
                string noteID = currentNote.noteInfo.GetNoteID();
                EmrDocInfo.setNoteIDForCheck(noteID);
                if (tvPatientsNoteUpdate(status, EmrConstant.Button.CommitNote))
                {
                    //会诊问题 暂搁
                    //FinishConsult();
                    FaileOrSuccess = Return.Successful;
                }
            }
            return FaileOrSuccess;
        }
        public void FinishConsult()
        {
            string noteID = currentNote.noteInfo.GetNoteID();
            XmlNode notePattern = Globals.emrPattern.GetEmrNote(noteID);
            if (notePattern.Attributes[AttributeNames.StartTime].Value != StartTime.Consult)
                return;
            TreeNode nodeLevel1;
            TreeNode tvPatientsSelectedNode = tvPatients.SelectedNode;
            /* Is opening the relative emrDocument? If so return index of emrDocument*/
            int index = OpeningEmr(tvPatientsSelectedNode, out nodeLevel1);
            XmlElement emrNote = emrDocuments.Get(index).CreateElement(ElementNames.EmrNote);
            EmrDocument emrDoc = emrDocuments.GetEmrDocument(index);
            emrDoc.GetConsult(notePattern, emrNote);
            if (emrNote.Attributes[AttributeNames.Sequence] == null)
                return;
            ThisAddIn.FinishConsultNote(emrNote.Attributes[AttributeNames.Sequence].Value, emrDoc.GetRegistryID());
        }
        public bool ImageSign(string Signer)
        {
            bool result = false;
            foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
            {
                if (xn.BaseName == Signer)
                {
                    result = FillWriterWithImage(xn);
                }
            }
            return result;
        }
        public void WriterSign(string Signer, string Null)
        {

            foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
            {
                //if (xn.BaseName == Signer)
                //{
                //    UnProtectDoc();
                //    xn.Range.Text = "  ";
                //    ProtectDoc();
                //}
                if (xn.BaseName == "医护签名") //2012-12-17 签名保留
                {
                    if (xn.HasChildNodes)
                    {
                        foreach (Word.XMLNode childXN in xn.ChildNodes)
                        {
                            if (childXN.BaseName == Signer)
                            {
                                UnProtectDoc();
                                childXN.Range.Text = "  ";
                                ProtectDoc();
                                break;
                            }
                        }
                    }
                    //if (xn.ChildNodes[1].BaseName == Signer)
                    //{
                    //    UnProtectDoc();
                    //    xn.ChildNodes[1].Range.Text = "  ";
                    //    ProtectDoc();
                    //}
                }
            }
        }
        public void WriterSign(string Signer)
        {

            foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
            {
                if (xn.BaseName == Signer)
                {
                    UnProtectDoc();
                    xn.Range.Text = Globals.DoctorName;
                    ProtectDoc();
                }
            }
        }
        public bool FillWriterWithImage(Word.XMLNode cc)
        {
            UnProtectDoc();
            string templateFile = "WSign.bmp";          
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    string bmp = es.GetImageSign(Globals.DoctorID);
                    if (bmp == null)
                    {
                        MessageBox.Show("不存在电子签名！");
                        return false;
                    }

                    MemoryStream mem = new MemoryStream(Convert.FromBase64String(bmp));
                    Image image = Image.FromStream(mem);

                    image.Save(templateFile);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX951852970", ex.Message + ">>" + ex.ToString(), true); 
           
                    return false;
                }
                object tTrue = true;
                object tFalse = false;

                object ranges = (object)cc.Range;
                cc.Range.Text = "";
                Word.InlineShape ile = cc.Range.InlineShapes.AddPicture(templateFile, ref tFalse, ref tTrue, ref ranges);
                    ProtectDoc();

            }
            return true;
        }
        string FileName = "";
        private void Merge2Word()
        {
            this.Invoke(new EventHandler(openEmr_Click));

            if (tvPatients.SelectedNode.Nodes.Count != 0)//存在子节点
            {
                List<string> MergeDocs = new List<string>();//某类合并后的文档列表
                WordMerger wordMerger;
                foreach (TreeNode tn in tvPatients.SelectedNode.Nodes)
                {
                    if (tn.Level != 2) return;

                    List<string> NoteList = new List<string>();

                    if (tn.Nodes.Count != 0)//该节点存在对应文档
                    {
                        foreach (TreeNode noteNode in tn.Nodes)//循环各节点，解密对应文档
                        {
                            int series = Convert.ToInt32(noteNode.Tag);
                            string wdNote = udt.MakeWdDocumentFileName(GetRegistryID(), tn.Name, series, Globals.workFolder);//.notx文档地址
                            string WordDoc = FileMethod.GetFilePath(wdNote) + "\\" + FileMethod.GetFileNameOnly(wdNote) + ".docx";//.docx文档地址
                            try
                            {
                                udt.jj.DecodeEmrDocument(wdNote, WordDoc);
                                NoteList.Add(WordDoc);
                            }
                            catch(Exception ex)
                            {
                                Globals.logAdapter.Record("EX951952970", ex.Message + ">>" + ex.ToString(), true); 
           
                                continue;
                            }
                        }

                        string MergeDoc = FileMethod.GetFilePath(NoteList[0]) + "\\" + tn.Name.Trim() + ".docx";//合并后docx文档地址
                        wordMerger = new WordMerger();
                        wordMerger.InsertMerge(null, NoteList.ToArray(), MergeDoc, true);
                        MergeDocs.Add(MergeDoc);

                        foreach (string wordFile in NoteList)
                        {
                            File.Delete(wordFile);
                        }
                    }
                    else continue;
                }

                string MergeDocFinal = FileMethod.GetFilePath(MergeDocs[0]) + "\\" + "合并文档.docx";//文档合并最终结果
                wordMerger = new WordMerger();
                wordMerger.InsertMerge(null, MergeDocs.ToArray(), MergeDocFinal, true);

                foreach (string _MergeDoc in MergeDocs)
                {
                    File.Delete(_MergeDoc);
                }

                this.Text = thisText;
                this.Cursor = Cursors.Default;
                loading.StopClose();
                Thread.Sleep(2500);

                FileName = MergeDocFinal;
                this.Invoke(new EventHandler(OpenDoc_General));
            }
            else
            {
                MessageBox.Show("无任何需要合并的文档。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
        }
        private void OpenDoc_General(object sender, EventArgs e)
        {
            patient1.Visible = false;
            od = new OpenDoc();
            od.Show();
            axWbDoCView.Dock = DockStyle.None;
            axWbDoCView.Size = new Size(1, 1);
            object oMissing = System.Reflection.Missing.Value;

            try
            {
                plMain.Visible = true;
                axWbDoCView.Navigate(FileName, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX961952970", ex.Message + ">>" + ex.ToString(), true); 
           
                ThisAddIn.killWordProcess();
                if (od.Visible)
                {
                    od.Close();
                    od.Dispose();
                }
            }
        }

    }
}
