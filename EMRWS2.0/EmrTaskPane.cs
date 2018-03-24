using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Xml;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using EmrConstant;
using CommonLib;
using System.Drawing.Text;
using UserInterface;
using System.Collections;
using System.Threading;
using HuronControl;
using System.Reflection;

namespace EMR
{
    public partial class MainForm
    {

        private void openEmr_Click(object sender, EventArgs e)
        {
            try
            {
                ThisAddIn.ResetBeginTime();
                if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.OpenEmr)) return;
                if (tvPatients.SelectedNode.Level != 1) return;
                OpenEmrDocument(tvPatients.SelectedNode);
                tvPatients.SelectedNode.Expand();
                //tvPatients.SelectedNode.Tag = -1;            
                ThisAddIn.ResetBeginTime();

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX741852968", ex.Message + ">>" + ex.ToString(), true);                
            }

        }
        private void Marge2Word_Click(object sender, EventArgs e)
        {
            MergeAsWord(sender, e);
            FlashFolder = false;
            saveFileDialog.FilterIndex = 1;
            SaveAsFile(sender, e);
            CloseDoc_General();

        }
        private void CloseDoc_General()
        {
            plMain.Visible = false;
            patient1.Visible = true;
            od = new OpenDoc();
            od.Show();
            object objMissing = System.Reflection.Missing.Value;
            wordApp = null;
            od.Dispose();
            axWbDoCView.Navigate("about:blank");
        }
        private void MergeAsWord(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            thisText = this.Text;
            this.Text += " - 正在合并...";

            loading = new RunningLoadFlag();
            loading.SuspendLayout();
            loading.StartPosition = FormStartPosition.CenterScreen;
            loading.Size = new Size(123, 123);
            loading.OpacityTo = 0.8d;
            loading.EnterOpacitySecend = 2;
            loading.ExitOpacitySecend = 2;
            loading.InnerCircleRadius = 15;
            loading.OutnerCircleRadius = 40;
            loading.SpokesMember = 15;
            loading.ResumeLayout(false);
            loading.PerformLayout();
            loading.StartMarquee();

            Thread tMerge = new Thread(Merge2Word);
            tMerge.Start();

            loading.ShowDialog(this);
        }
        private void margeDoc()
        {
            bool IsFirstPage = true;
            bool IsHasEmptyPage = false;
            XmlElement mergeNotes = Globals.Mdoc.CreateElement(ElementNames.EmrNotes);
            AuthorInfo author = MakeAuthorInfo(tvPatients.SelectedNode.Name, tvPatients.SelectedNode.Text);
            bool split1 = false;
            IsHasEmptyPage = false;
            int lastPageNumber = ActiveDocumentManager.getDefaultAD().ActiveWindow.Panes[1].Pages.Count;
            int sections = lastPageNumber;
            ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.Select();
            int currentPageCount = ActiveDocumentManager.getDefaultAD().ActiveWindow.Panes[1].Pages.Count;
            Word.Range range1 = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            Word.Range r1 = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            r1.Start = r1.End;
            r1.Select();
            if (Globals.MIsSingle == "yes" || Globals.MIsSingle == "Yes")
            {
                #region
             
                if (lastPageNumber != currentPageCount)
                {
                    //PatientInfo patientInfo = GetPatientInfo(tvPatients.SelectedNode);
                    Word.Pages pages = ActiveDocumentManager.getDefaultAD().ActiveWindow.Panes[1].Pages;
                    string NoteID = "";
                    string NoteName = "";
                    foreach (Word.ContentControl cc in range1.ContentControls)
                    {
                        if (udt.jj.IsNumaric(cc.Title))
                        {
                            NoteID = cc.Title;
                            NoteName = cc.Range.Text;
                        }
                    }
                    for (int i = lastPageNumber + 1; i <= currentPageCount; i++)
                    {
                        Word.Range rangett = pages[i].Breaks[1].Range;
                        Word.Range ranget = pages[i].Breaks[1].Range;
                        ranget.End = ranget.Start + 1;
                        if (ranget.Text != null && ranget.Text != "\r")
                        {
                            ranget.InsertBefore("\r");
                            rangett.Start++;
                        }
                        Word.ContentControl cc = DrawNoteName(NoteID,
                            NoteName, rangett);
                        rangett.Start = cc.Range.End + 1;
                        rangett.InsertAfter("\r");
                        rangett.Start = cc.Range.End + 2;
                        //cc = DrawSmallHeader(patientInfo, rangett);
                        rangett.Start = cc.Range.End + 1;
                        rangett.InsertAfter("\r");
                    }
                    if (ActiveDocumentManager.getDefaultAD().ActiveWindow.Panes[1].Pages.Count > currentPageCount)
                    {
                        Word.Range rangett = pages[currentPageCount + 1].Breaks[1].Range;
                        Word.Range ranget = pages[currentPageCount + 1].Breaks[1].Range;
                        ranget.End = ranget.Start + 1;
                        if (ranget.Text != null && ranget.Text != "\r")
                        {
                            ranget.InsertBefore("\r");
                            rangett.Start++;
                        }
                        Word.ContentControl cc = DrawNoteName(NoteID,
                            NoteName, rangett);
                        rangett.Start = cc.Range.End + 1;
                        rangett.InsertAfter("\r");
                        rangett.Start = cc.Range.End + 2;
                        // cc = DrawSmallHeader(patientInfo, rangett);
                        rangett.Start = cc.Range.End + 1;
                        rangett.InsertAfter("\r");
                    }
                    range1.Select();
                }
                #endregion
                if (TotalRecordsNumber > 0 && CheckedNumber > 1)
                {
                    UnProtectDoc();
                    range1.InsertAfter("1");
                    range1.Select();
                    IsHasEmptyPage = true;
                }
                CheckedNumber = 0;               
                split1 = true;
            }   
            UnProtectDoc();
            XmlElement mergeNote = Globals.Mdoc.CreateElement(ElementNames.EmrNote);
            mergeNote.SetAttribute(AttributeNames.NoteID, Globals.MnoteID);
            //2012-12-04 新方法注释
            mergeNote.SetAttribute(AttributeNames.NoteName, Globals.Mnode.Text.Split(Delimiters.Space)[1]);
            //mergeNote.SetAttribute(AttributeNames.Series, Globals.Mnode.Tag.ToString());
            mergeNotes.AppendChild(mergeNote);

            bool up = false;
            //第二个文档           
            foreach (TreeNode noteNode in mns.GetNotes().Nodes)
            {
                TotalRecordsNumber--;                           
              
                if (!noteNode.Checked) continue;
                if (IsFirstPage)
                {
                    IsFirstPage = !IsFirstPage;
                    continue;
                }
                if (split1)
                {
                   SplitPagesBySection.split2(wordApp);
                  // Split();
                   Word.Range rn = wordApp.Selection.Range;
                   deleteNewLine(rn);
                   split1 = false;
                }
                int series = Convert.ToInt32(noteNode.Tag);
                string noteID = noteNode.Name;
                string wdNote = udt.MakeWdDocumentFileName(GetRegistryID(), noteID, series, Globals.workFolder);

                string IsSingle = "no";
                if (Globals.emrPattern.GetEmrNote(noteID).Attributes[AttributeNames.SingleContinue] != null)
                    IsSingle = Globals.emrPattern.GetEmrNote(noteID).Attributes[AttributeNames.SingleContinue].Value.ToString();
                
                if (up)
                {
                   
                    SplitPagesBySection.split2(wordApp);
                    Word.Range rn = wordApp.Selection.Range;
                    deleteNewLine(rn);
                    //split = true;
                    // wordApp.Selection.InsertNewPage();
                    //2012-03-16
                    //Object objMissing = Missing.Value;
                    //Object filename = "Doc2.dotx";
                    //wordApp.Documents.Add(filename, objMissing, objMissing, objMissing);
                    if (IsSingle == "yes")singlePageNoteID.Add(noteID);                  
                    else up = false;
                    
                }
                else
                {
                    if (IsSingle == "yes")
                    {
                        UnProtectDoc();
                        singlePageNoteID.Add(noteID);
                        if (!IsFirstPage)
                        {
                            if (!IsHasEmptyPage)
                            {
                                UnProtectDoc();
                                range1.InsertAfter("1");
                                IsHasEmptyPage = true;
                                SplitPagesBySection.split2(wordApp);
                                Word.Range rn = wordApp.Selection.Range;
                                deleteNewLine(rn);
                                //split = true;                               
                                //wordApp.Selection.InsertNewPage();
                                //Object objMissing = Missing.Value;
                                //Object filename = "Doc2.dotx";
                                //wordApp.Documents.Add(filename, objMissing, objMissing, objMissing);
                            }
                        }
                        if (IsFirstPage) up = false;  
                        else  up = true;                        
                    }                  
                }
               
                ThisAddIn.ImportWordDoc(wdNote, wordApp, true,false);              
                //ActiveDocumentManager.getDefaultAD().Save(); 2012-12-07 新方法注释
                IsHasEmptyPage = false;
                lastPageNumber = ActiveDocumentManager.getDefaultAD().ActiveWindow.Panes[1].Pages.Count;
                Word.Range r = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                r.Start = r.End;
                r.Select();    
                currentPageCount = ActiveDocumentManager.getDefaultAD().ActiveWindow.Panes[1].Pages.Count;
                range1 = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;               
                mergeNote.SetAttribute(AttributeNames.NoteID, noteID);
                mergeNote.SetAttribute(AttributeNames.NoteName, noteNode.Text.Split(Delimiters.Space)[1]);
                mergeNote.SetAttribute(AttributeNames.Series, noteNode.Tag.ToString());
                mergeNotes.AppendChild(mergeNote);
               
            }             
            //修改页码 2012-09-06
            // SetFooterPage(sections,split);  
            SplitPagesBySection.setPageNum(wordApp);
            currentNote = new EmrNote(author, GetRegistryID(), this);
            ProtectDoc();
            Globals.Mmarge = false;       
        }
        private void menuPatient_Opening(object sender, CancelEventArgs e)
        {
            if (tvPatients.SelectedNode == null) return;
            if (tvPatients.SelectedNode.Level == 0)
            {
                editNote.Visible = false;
                uncommit.Visible = false;
                deleteNote.Visible = false;
                noteInfo.Visible = false;
                openEmr.Visible = false;
                closeEmr.Visible = false;
                newNote.Visible = false;
                marge.Visible = false;
                unlockEmr.Visible = false;
                valuate.Visible = false;
                CreateConsent.Visible = false;
                SaveAsPdf.Visible = false;
            }
            if (tvPatients.SelectedNode.Level == 1)
            {
                editNote.Visible = false;
                uncommit.Visible = false;
                deleteNote.Visible = false;
                noteInfo.Visible = false;
                marge.Visible = false;
                //if (Globals.RegistryID == "" || Globals.RegistryID != tvPatients.SelectedNode.Text.Split(':')[1].Trim())
                if (Convert.ToInt32(tvPatients.SelectedNode.Tag) < 0)
                {                   
                    openEmr.Visible = true;
                    closeEmr.Visible = false;
                    newNote.Visible = false;
                    unlockEmr.Visible = false;
                    valuate.Visible = false;
                    SaveAsPdf.Visible = false;
                }
                else
                {
                    closeEmr.Visible = true;
                    openEmr.Visible = true;
                    newNote.Visible = true;
                    SaveAsPdf.Visible = true;
                    //unlockEmr.Visible = true;
                    //valuate.Visible = true;
                }
               
                CreateConsent.Visible = false;
            }
            if (tvPatients.SelectedNode.Level == 2)
            {
                editNote.Visible = false;
                uncommit.Visible = false;
                deleteNote.Visible = false;
                noteInfo.Visible = false;
                openEmr.Visible = false;
                closeEmr.Visible = false;
                newNote.Visible = false;
                marge.Visible = true;
                unlockEmr.Visible = false;
                valuate.Visible = false;
                SaveAsPdf.Visible = false;               
                //告知书
                if (ThisAddIn.CanOption(ElementNames.Consent))
                    CreateConsent.Visible = true;
                else CreateConsent.Visible = false;
            }
            if (tvPatients.SelectedNode.Level == 3)
            {
                if (tvPatients.SelectedNode.Parent.Text.Contains("临床路径"))
                {
                    editNote.Visible = false;
                    uncommit.Visible = false;
                    deleteNote.Visible = false;
                    noteInfo.Visible = false;
                    openEmr.Visible = false;
                    closeEmr.Visible = false;
                    newNote.Visible = false;
                    marge.Visible = false;
                    unlockEmr.Visible = false;
                    valuate.Visible = false;
                    CreateConsent.Visible = false;
                    SaveAsPdf.Visible = false;
                    return;
                }
                NoteStatus status = (NoteStatus)Convert.ToInt32(tvPatients.SelectedNode.Name.Split(':')[0]);
                if (status == NoteStatus.Draft)
                {
                    uncommit.Visible = false;//返修记录
                    deleteNote.Visible = true;//删除记录
                }
                else if (status == NoteStatus.Commited)
                {
                    uncommit.Visible = true;//返修记录
                    deleteNote.Visible = false;//删除记录
                }
                else if (status == NoteStatus.Checked || status == NoteStatus.FinallyChecked)
                {
                    uncommit.Visible = false;//返修记录
                    deleteNote.Visible = false;//删除记录
                }
                editNote.Visible = true;
                noteInfo.Visible = true;
                openEmr.Visible = false;
                closeEmr.Visible = false;
                newNote.Visible = false;
                marge.Visible = false;
                unlockEmr.Visible = false;
                valuate.Visible = false;
                CreateConsent.Visible = false;
                SaveAsPdf.Visible = false;
               
            }
        }
        private void closeEmr_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            tnNode = null;
            Globals.RegistryID = "";
           // tvPatients.SelectedNode.Tag = 0;
            tvPatients.SelectedNode.Nodes.Clear();
            closeEmr.Visible = false;
            openEmr.Visible = true;
        }
        private void SetValue(DataSet dst)
        {
            UnProtectDoc();
            SetRange(dst, Globals.RegistryID);
            ProtectDoc();
        }
        private void NewDoc()
        {
            if (!string.IsNullOrEmpty(Globals.RegistryID))
            {
                DataSet dst = ThisAddIn.GetInf(Globals.RegistryID);
                if (dst == null)
                {
                    axWbDoCView.Navigate("about:blank");
                    return;
                }
                SetValue(dst);
                currentNote = new EmrNote(Globals.EauthorInfo, Globals.RegistryID, this);
                string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
                try
                {
                    udt.jj.SaveWordDoc(ActiveDocumentManager.getDefaultAD(), tmpfile, false);
                    Globals.docPath = ActiveDocumentManager.getDefaultAD().Path;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741852969", ex.Message + ">>" + ex.ToString(), true);                
         
                }

            }

        }
        private void newNote_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (tvPatients.SelectedNode.Level != 1) return;
            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.NewNote))
            {
                MessageBox.Show(ErrorMessage.NoPrivilegeNew, ErrorMessage.Warning);
                return;
            }
          
            TreeNode registryIDNode;
            TreeNode tvPatientsSelectedNode = tvPatients.SelectedNode;

            int index1 = OpeningEmr(tvPatientsSelectedNode, out registryIDNode);
            if (index1 < 0)
            {
                MessageBox.Show(ErrorMessage.NoOpeningEmr, ErrorMessage.Error);
                return;
            }
           
            string str = tvPatients.SelectedNode.Text;
            if (str.Split(':').Length == 4)
                Globals.PatientConsultSequence = str.Split(':')[3].ToString();
            if (str.Split(':').Length == 5)
                Globals.PatientConsultSequence = str.Split(':')[4].ToString();
            int index = Convert.ToInt32(tvPatients.SelectedNode.Tag);
            
            PatternList pl = new PatternList(Globals.emrPattern.GetAllEmrNotes());
            if (pl.ShowDialog() != DialogResult.OK) return;
            Globals.NoteID = pl.GetNoteID();
            EmrDocInfo.newOpen(Globals.NoteID);
            //新建检查唯一性
            TreeNode noteIDNodeExist = FindNoteIDNode(
                   registryIDNode, Globals.NoteID);
            if (noteIDNodeExist != null)
            {
                /* Check unique? noteIDNodeExist.Tag is unique flag. */
                if (noteIDNodeExist.Tag.ToString() == StringGeneral.Yes && noteIDNodeExist.Nodes.Count > 0)
                {
                    MessageBox.Show(ErrorMessage.ExistNoteContent, ErrorMessage.Error);
                    return;
                }
            }
            if (!CheckPrecontition(Globals.NoteID, index))
            {
                MessageBox.Show(ErrorMsg(Globals.NoteID), ErrorMessage.Error);
                return;
            }
            string noteName = Globals.emrPattern.GetNoteNameFromNoteID(Globals.NoteID);
            AuthorInfo authorInfo = MakeAuthorInfo(Globals.NoteID, noteName);
            //PatientInfo patientInfo = GetPatientInfo(tvPatients.SelectedNode);
            Globals.NnewNote = true;
            Globals.EauthorInfo = authorInfo;

            XmlDocument doc = new XmlDocument();
            XmlNode pattern = doc.CreateElement(EmrConstant.ElementNames.NoteTemplate);
            string msg = ThisAddIn.SehPatternDoc(ref pattern);
            if (msg != "") return;
            string templateDocName = Path.Combine(Globals.workFolder + "\\", "ptn.docx");
           //2012-03-14
            bool flag = udt.jj.StringToWordDocument(templateDocName, pattern);
            if (!flag)
            {
                ThisAddIn.killWordProcess(); return;
            }
            
            string registryID = "", chargeDoctorID;
            ParserRegistry(tvPatients.SelectedNode.Text, out registryID, out chargeDoctorID);
            Globals.RegistryID = registryID;
            if (Globals.chuyuan == true)
            {
                Globals.chuyuan = false;
                return;
            }
            XmlNode NodePattern = Globals.emrPattern.GetEmrNote(Globals.NoteID);
            if (NodePattern.Attributes[AttributeNames.Record] != null && NodePattern.Attributes[AttributeNames.Record].Value == "yes")
            {
                NewLineVisble(sender, e, true);
            }

            OperationEnable(false);
            TvPatientsEnable(false);
            ShowMenu(MenuStatus.Writer);
            ThisAddIn.ResetBeginTime();
            LoadWord(NewNoteWord);
           // _Enble();
        }
        private void NewNoteWord()
        {
            OpenWordTem(Path.Combine(Globals.workFolder + "\\", "ptn.docx"));
            CloseLoad();
        }
        private void EditDoc()
        {
            try
            {
                if (!IsSigned(ActiveDocumentManager.getDefaultAD()))
                {
                   
            switch (operatorRole)
            {
                case EmrConstant.OperatorRole.Writer:
                    if (Globals.EeditMode == EmrConstant.NoteEditMode.Writing)
                    {
                        SetWrittenDate(ThisAddIn.Today());
                        if (Globals.Eshixi == true)
                            SetWriter(Globals.space6 + Globals.EauthorInfo.CheckerLable + Globals.EauthorInfo.Writer);
                    }
                    break;
                case EmrConstant.OperatorRole.Checker:
                    if (Globals.EeditMode == EmrConstant.NoteEditMode.Checking)
                    {
                        SetChecker(Globals.space6 + Globals.EauthorInfo.CheckerLable +
                            Globals.DoctorName);


                        SetCheckedDate();
                        SetCheckInfo(Globals.EemrNote);
                    }
                    break;
                case EmrConstant.OperatorRole.FinalChecker:
                    if (Globals.EeditMode == EmrConstant.NoteEditMode.FinallyCkecking)
                    {
                        SetFinalChecker(Globals.space6 + Globals.EauthorInfo.FinalCheckerLable +
                            Globals.DoctorName);
                        SetFinalCheckedDate();
                        SetFinalCheckInfo(Globals.EemrNote);
                    }
                    break;
            }

                      ActiveDocumentManager.getDefaultAD().Saved = true;
                    currentNote = new EmrNote(Globals.EauthorInfo, Globals.EemrNote, Globals.EeditMode, GetRegistryID(), this);

                    if ((Globals.ENodePattern.Attributes["Nurse"] == null && Globals.EdoctorType == "1") ||
                        ((Globals.ENodePattern.Attributes["Nurse"] != null) && (Globals.ENodePattern.Attributes["Nurse"].Value == "Both" && Globals.EdoctorType == "1")) ||
                        ((Globals.ENodePattern.Attributes["Nurse"] != null) && (Globals.ENodePattern.Attributes["Nurse"].Value == "Yes" && Globals.EdoctorType == "2")) ||
                       ((Globals.ENodePattern.Attributes["Nurse"] != null) && (Globals.ENodePattern.Attributes["Nurse"].Value == "Both" && Globals.EdoctorType == "2")))
                    {
                        OperationEnable(Globals.EeditMode == NoteEditMode.Reading);
                        CanValuateNote(Globals.EeditMode != NoteEditMode.Writing);
                        ShowRevision();
                    }
                    //Globals.ThisAddIn.InitialFocus();
                }
                else
                {
                    //DisableAllButValuate();
                }


                if ((Globals.ENodePattern.Attributes["Nurse"] == null && Globals.EdoctorType == "2")
                      || ((Globals.ENodePattern.Attributes["Nurse"] != null) &&
                        (Globals.ENodePattern.Attributes["Nurse"].Value == "Yes" && Globals.EdoctorType == "1")))
                {


                    OperationEnable(true);
                    //CanValuateNote(editMode != NoteEditMode.Writing);
                    ShowRevision();
                }
                if (Globals.EdoctorType == "")
                {

                    OperationEnable(true);
                    //CanValuateNote(editMode != NoteEditMode.Writing);
                    ShowRevision();
                }

                string str = ThisAddIn.CanOptionText(ElementNames.SpecialText).Trim();
                if (str.Contains(",") == true)
                {
                    string[] strlist = str.Split(',');
                    foreach (string strEach in strlist)
                    {
                        if (ThisAddIn.CanOption(ElementNames.Special) == true && strEach.Trim() == Globals.OpDepartID)
                        {
                           // OperationEnableSpecial();
                        }
                    }
                }
                else
                {
                    if (ThisAddIn.CanOption(ElementNames.Special) == true && ThisAddIn.CanOptionText(ElementNames.SpecialText).Trim() == Globals.OpDepartID)
                    {
                        // OperationEnableSpecial();
                    }
                }
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX741852970", ex.Message + ">>" + ex.ToString(), true);                
         
            }
            currentNote.SetEditModes();
     
            //if (Globals.UseNewOp == true)
            //{               
            //    wordApp.ActiveWindow.View.ShowRevisionsAndComments = true;
            //    wordApp.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;
            //    if (NewL)
            //    {
            //        NewLineVisble(sender, e, true);
            //    }
            //}  
        }
        private void editNote_Click(object sender, EventArgs e)
        {
            LoadWord(EditNoteWord);
          
        }
        private delegate void DELNoteWordOperate();
        private void LoadWord(DELNoteWordOperate method)
        {
            this.Cursor = Cursors.WaitCursor;
            thisText = this.Text;
            this.Text += " - 正在打开病历请稍等...";
            this.Invoke(method);
            //loading = new RunningLoadFlag();
            //loading.SuspendLayout();
            //loading.StartPosition = FormStartPosition.CenterScreen;
            //loading.Size = new Size(123, 123);
            //loading.OpacityTo = 0.8d;
            //loading.EnterOpacitySecend = 1;
            //loading.ExitOpacitySecend = 1;
            //loading.InnerCircleRadius = 15;
            //loading.OutnerCircleRadius = 40;
            //loading.SpokesMember = 15;
            //loading.ResumeLayout(false);
            //loading.PerformLayout();
            //loading.StartMarquee();
            //Thread tMerge = new Thread(method);
            //tMerge.Start();

            //loading.ShowDialog(this);
        }
        private void EditNoteWord()
        {
            bool shixi = false;
            //2012-03-29
            if (mns != null) mns = null;
            string workFolder = Globals.workFolder;
            string linkListFolder = Path.Combine(workFolder, EmrConstant.ResourceName.LinkListFolder);
            string doctorsFile = Path.Combine(linkListFolder, EmrConstant.ResourceName.DoctorsXml);
            XmlDocument docDoctors = new XmlDocument();
            docDoctors.Load(doctorsFile);
            string doctorType = "";
            foreach (XmlNode doctor in docDoctors.DocumentElement.SelectNodes(ElementNames.Doctor))
            {
                if (doctor.Attributes[AttributeNames.Code].Value == Globals.DoctorID)
                {
                    if (doctor.Attributes[AttributeNames.DoctorType] != null)
                    {

                        doctorType = doctor.Attributes[AttributeNames.DoctorType].Value;
                    }
                    break;
                }
            }
            TreeNode selectedNode = tvPatients.SelectedNode;
            string NoteID = selectedNode.Parent.Name;
            Globals.NoteID = NoteID;
           
            if (NoteID == "0" && ThisAddIn.CanOption(ElementNames.BA))
            {

                OpenMedical();
                return;
            }
            if (ThisAddIn.CanOption(ElementNames.ClinicPath) && ThisAddIn.CanOptionText(ElementNames.PathID).Trim() != "")
            {
                if (NoteID == ThisAddIn.CanOptionText(ElementNames.PathID).Trim())
                {

                    Globals.NoteID = ThisAddIn.CanOptionText(ElementNames.PathID).Trim();
                    // OpenClinicPath(GetRegistryID(), NoteID);
                    NewOpenClinicPath(GetRegistryID(), NoteID);
                    return;
                }
            }
            XmlNode NodePattern = Globals.emrPattern.GetEmrNote(NoteID);
            ThisAddIn.ResetBeginTime();
            int series = -1;
            try
            {
                series = Convert.ToInt32(selectedNode.Tag);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX741852971", ex.Message + ">>" + ex.ToString(), true);

            }
            EmrDocInfo.existOpen(Globals.NoteID, series);
            int index = Convert.ToInt32(selectedNode.Parent.Parent.Tag);
            XmlElement emrNote = GetEmrNote(index, series);

            if (emrNote == null)
            {
                MessageBox.Show(ErrorMessage.EmrDocumentError, ErrorMessage.Error);
                return;
            }
            if (emrNote.Attributes["fanxiu"] == null) Globals.Isfanxiu = false;
            else Globals.Isfanxiu = true;

            #region Check permission

            PermissionLevel myPermission = emrDocuments.GetPermission(index);
            Permission = myPermission;
            NoteStatus noteStatus = (NoteStatus)Convert.ToInt32(selectedNode.Name.Split(':')[0]);
            NoteEditMode editMode = NoteEditMode.Nothing;
            string finalCheckerID = ThisAddIn.NormalizeOpcode(emrNote.GetAttribute(AttributeNames.FinalCheckerID));
            string checkerID = ThisAddIn.NormalizeOpcode(emrNote.GetAttribute(AttributeNames.CheckerID));
            string writerID = ThisAddIn.NormalizeOpcode(emrNote.GetAttribute(AttributeNames.WriterID));
            bool NewL = false;
            bool mp1 = (GetEmrStatus() == EmrStatus.Locked && !ThisAddIn.CanOption(ElementNames.CanEditLockedEmr)
                && !ThisAddIn.IsUnlockedEmr(GetRegistryID()));
            if (myPermission == PermissionLevel.ReadOnly || mp1
               )
            {
                /* Whole emrDocument is locked */
                editMode = NoteEditMode.Reading;
            }
            else
            {
                #region Determine edit mode and operator role

                switch (noteStatus)
                {
                    case EmrConstant.NoteStatus.Draft:
                        string SaveID = writerID;
                        using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
                        {
                            DataSet dst = pi.GetOpInf(writerID);
                            DataSet dsts = pi.GetOpInf(Globals.DoctorID);
                            if (dst.Tables[0].Rows.Count > 0)
                            {
                                if (dst.Tables[0].Rows[0]["TecqTitle"].ToString().Trim().Equals("实习医师"))
                                {
                                    if ((!dsts.Tables[0].Rows[0]["TecqTitle"].ToString().Equals("实习医师")) && dsts.Tables[0].Rows[0]["DoctorType"].ToString().Equals("1"))
                                    {
                                        SaveID = Globals.DoctorID;
                                        shixi = true;
                                    }
                                }
                            }
                        }
                        editMode = EditModeWhenNoteDraft(myPermission, SaveID);
                        if (editMode == EmrConstant.NoteEditMode.Writing)
                            operatorRole = EmrConstant.OperatorRole.Writer;
                        break;
                    case EmrConstant.NoteStatus.Commited:
                        if (NodePattern.Attributes[AttributeNames.Record] != null && NodePattern.Attributes[AttributeNames.Record].Value == "yes")
                        {
                            editMode = EmrConstant.NoteEditMode.Writing;
                            operatorRole = EmrConstant.OperatorRole.Writer;
                            NewL = true;
                        }
                        else
                        {
                            editMode = EditModeWhenNoteCommited(myPermission, writerID);
                            if (editMode == EmrConstant.NoteEditMode.Checking)
                                operatorRole = EmrConstant.OperatorRole.Checker;
                        }

                        break;
                    case EmrConstant.NoteStatus.Checking:
                        editMode = EditModeWhenNoteChecking(myPermission, checkerID);
                        if (editMode == EmrConstant.NoteEditMode.Checking)
                            operatorRole = EmrConstant.OperatorRole.Checker;
                        break;
                    case EmrConstant.NoteStatus.Checked:
                        editMode = EditModeWhenNoteChecked(myPermission, writerID, checkerID);
                        if (editMode == EmrConstant.NoteEditMode.FinallyCkecking)
                            operatorRole = EmrConstant.OperatorRole.FinalChecker;
                        break;
                    case EmrConstant.NoteStatus.FinallyCkecking:
                        editMode = EditModeWhenNoteFinalChecking(myPermission, finalCheckerID);
                        if (editMode == EmrConstant.NoteEditMode.FinallyCkecking)
                            operatorRole = EmrConstant.OperatorRole.FinalChecker;
                        break;
                    case EmrConstant.NoteStatus.FinallyChecked:
                        editMode = EditModeWhenNoteFinalChecked(myPermission);
                        break;
                }
                #endregion
            }
            #endregion

            #region Show msg about no privilege and do nothing
            if (editMode == NoteEditMode.Nothing)
            {
                MessageBox.Show(ErrorMessage.NoPrivilege, ErrorMessage.Warning);
                return;
            }
            #endregion
            // #region Set attributes
            AuthorInfo authorInfo;
            authorInfo.NoteID = selectedNode.Parent.Name;
            authorInfo.NoteName = selectedNode.Parent.Text;
            if (shixi == false) authorInfo.Writer = emrNote.GetAttribute(AttributeNames.Writer);
            else authorInfo.Writer = Globals.DoctorName + " / " + emrNote.GetAttribute(AttributeNames.Writer);
            if (emrNote.Attributes[AttributeNames.ChildID] == null) authorInfo.ChildID = "0";
            else authorInfo.ChildID = emrNote.Attributes[AttributeNames.ChildID].Value;

            authorInfo.WrittenDate = emrNote.GetAttribute(AttributeNames.WrittenDate);
            authorInfo.Checker = emrNote.GetAttribute(AttributeNames.Checker);
            authorInfo.CheckedDate = emrNote.GetAttribute(AttributeNames.CheckedDate);
            authorInfo.FinalChecker = emrNote.GetAttribute(AttributeNames.FinalChecker);
            authorInfo.FinalCheckedDate = emrNote.GetAttribute(AttributeNames.FinalCheckedDate);
            authorInfo.TemplateType = EmrConstant.StringGeneral.NoneTemplate;
            authorInfo.TemplateName = "";
            authorInfo.WriterLable = emrNote.GetAttribute(AttributeNames.Sign3);
            authorInfo.CheckerLable = emrNote.GetAttribute(AttributeNames.Sign2);
            authorInfo.FinalCheckerLable = emrNote.GetAttribute(AttributeNames.Sign1);
            PatientInfo patientInfo = GetPatientInfo(selectedNode);
            #region Update sign and date on note in word window
            // currentNote = new EmrNote(authorInfo, emrNote, editMode, GetRegistryID(), this);         
            Globals.RegistryID = GetRegistryID();
            #endregion

            #region Show note in word window

            if (editMode == NoteEditMode.Reading)
            {
                ShowMenu(MenuStatus.Reader);
            }
            else if (editMode == EmrConstant.NoteEditMode.Writing)
            {
                ShowMenu(MenuStatus.Writer);
            }
            else
            {
                ShowMenu(MenuStatus.Manager);
            }
            #endregion
            Globals.ENodePattern = NodePattern;
            Globals.EeditMode = editMode;
            Globals.EdeditNote = true;
            Globals.EemrNote = emrNote;
            Globals.EauthorInfo = authorInfo;
            Globals.Eshixi = shixi;
            Globals.EdoctorType = doctorType;
            TvPatientsEnable(false);
            OpenTemplate.Visible = false;//2012-03-27 LiuQi模板列表显示功能
            //查询，构件 false
            // _Enble();

           
            string wdDocName =
               udt.MakeWdDocumentFileName(Globals.RegistryID, authorInfo.NoteID, series, Globals.workFolder);
            string mes = OpenWordDoc(wdDocName);

            CloseLoad();
        }
        private void CloseLoad()
        {
            this.Text = thisText;
            this.Cursor = Cursors.Default;
            //loading.StopClose();
           // Thread.Sleep(1000);
        }
        private void uncommit_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            string strRegistryID = GetRegistryID();
            int index = Convert.ToInt32(tvPatients.SelectedNode.Parent.Parent.Tag);
            string ArchiveN = tvPatients.SelectedNode.Parent.Parent.Parent.Tag.ToString();
            //静海归档设置
             string strStatus="";
            if (ThisAddIn.CanOption(ElementNames.ArchieveLocked))
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    strStatus = es.GetArchiveStatus(ArchiveN);                    
                }
            }
            else
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    strStatus = es.GetEmrDocumentStatus(strRegistryID);
                }
            }

            if (strStatus == "1")
            {
                MessageBox.Show("病历已经执行归档不能返修！", "提示");
                return;
            }               
            


            #region Get document index
            string registryID = GetRegistryID();
            string noteID = tvPatients.SelectedNode.Parent.Name;
            //int index = Convert.ToInt32(tvPatients.SelectedNode.Parent.Parent.Tag);
            int series = GetNoteSeries();
            #endregion

            if (ThisAddIn.CanOption(ElementNames.CannotUncommit))
            {
                XmlNode note = emrDocuments.GetEmrDocument(index).GetEmrNoteBySeries(series);
                NoteStatus ns = (NoteStatus)Convert.ToInt32(note.Attributes[AttributeNames.NoteStatus].Value);
                string writerID = note.Attributes[AttributeNames.WriterID].Value;
                if (ns == NoteStatus.Checked) writerID = note.Attributes[AttributeNames.CheckerID].Value;
                else if (ns == NoteStatus.FinallyChecked) writerID = note.Attributes[AttributeNames.FinalCheckerID].Value;
                if (Globals.DoctorID == writerID)
                {
                    MessageBox.Show(ErrorMessage.SelfCannotUncommit, ErrorMessage.Warning);
                    return;
                }
            }


            string checker = "";
            string Finalchecker = "";
            string writer = GetWriterInf(ref checker, ref Finalchecker);
            if (Finalchecker == "")
            {
                if (checker == "")
                {
                    if (!writer.Equals(Globals.DoctorID))
                    {

                        MessageBox.Show("不是病例书写者,不能返修！", ErrorMessage.Warning);
                        return;
                    }
                    //else if (Globals.ThisAddIn.Trace==true) Globals.ThisAddIn.SetRevisions();
                }
                else if (!checker.Equals(Globals.DoctorID))
                {
                    MessageBox.Show("只有病例的审核者才能返修！", ErrorMessage.Warning);
                    return;
                }
            }
            else if (!Finalchecker.Equals(Globals.DoctorID))
            {
                MessageBox.Show("只有病例的终审者才能返修！", ErrorMessage.Warning);
                return;
            }
            ThisAddIn.RetrieveMyroles(Globals.DoctorID);
            if (Globals.myroles != "")
            {
                if (Globals.myroles.IndexOf("书写") == -1)
                {
                    MessageBox.Show("没有书写权限,不能返修", ErrorMessage.Warning);
                    return;
                }
            }

            #region Check if the document is signed
            //Word.Application wapp = wordApp;
            //object missing = System.Reflection.Missing.Value;
            //string filename = udt.MakeWdDocumentFileName(registryID, noteID, series,Globals.workFolder);
            //string tmpfile = Path.Combine(Directory.GetCurrentDirectory(), ResourceName.Mytmp);
            //if (Globals.ThisAddIn.localDocumentEncode) udt.DecodeEmrDocument(filename, tmpfile);
            //else File.Copy(filename, tmpfile);
            //object ofilename = tmpfile;
            //object myFalse = false;
            //object myTrue = true;
            //Word.Document wdDoc = wapp.Documents.Open2000(ref ofilename, ref myFalse, ref myTrue,
            //               ref myFalse, ref missing, ref missing, ref myTrue, ref missing, ref missing,
            //               ref missing, ref missing, ref myFalse);

            //if (Globals.ThisAddIn.IsSigned(wdDoc))
            //{
            //    MessageBox.Show(ErrorMessage.SignedNoUncommit, ErrorMessage.Error);
            //    wdDoc.Close(ref myFalse, ref missing, ref missing);
            //    return;
            //}
            //wdDoc.Close(ref myFalse, ref missing, ref missing);
            #endregion

            #region Get uncommit reasion
            Uncommit uc = new Uncommit();
            if (uc.ShowDialog() == DialogResult.Cancel) return;
            #endregion

            #region Create reasion element
            XmlDocument doc = new XmlDocument();
            XmlElement reason = doc.CreateElement(ElementNames.Reason);
            reason.SetAttribute(AttributeNames.PatientName, GetPatientName());
            reason.SetAttribute(AttributeNames.Doctor, Globals.DoctorName);
            reason.SetAttribute(AttributeNames.DepartmentName, tvPatients.SelectedNode.Parent.Parent.Text.Split(' ')[2]);
            reason.SetAttribute(AttributeNames.NoteName, tvPatients.SelectedNode.Parent.Text);
            reason.InnerText = uc.GetReasion();
            #endregion

            if (!emrDocuments.GetEmrDocument(index).UncommitEmrNote(series, reason)) return;
            XmlNode notePattern = Globals.emrPattern.GetEmrNote(noteID);
            if (notePattern.Attributes[AttributeNames.StartTime].Value == StartTime.Consult)
                emrDocuments.GetEmrDocument(index).UncommitConsult(series);

            int status = Convert.ToInt32(tvPatients.SelectedNode.Name.Split(':')[0]) - 1;
            tvPatients.SelectedNode.Name = status.ToString();
            string[] items = tvPatients.SelectedNode.Text.Split(' ');
            if (items.Length == 3)
                tvPatients.SelectedNode.Text = items[0] + NoteState(tvPatients.SelectedNode.Name.Split(':')[0]) + items[2];
            else tvPatients.SelectedNode.Text = items[0] + NoteState(tvPatients.SelectedNode.Name.Split(':')[0]) + items[2]; ;


            OpDone opd = new OpDone("返修申请成功！");
            opd.Show();
            ThisAddIn.ResetBeginTime();

        }
        private int GetNoteSeries()
        {
            #region Get emrNote in *.note document
            TreeNode selectedNode = tvPatients.SelectedNode;
            int series = -1;
            try
            {
                series = Convert.ToInt32(selectedNode.Tag);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX741852972", ex.Message + ">>" + ex.ToString(), true); 
            }
            return series;
        }
        private int GetIndex()
        {
            TreeNode selectedNode = tvPatients.SelectedNode;
            int index = Convert.ToInt32(selectedNode.Parent.Parent.Tag);
            return index;
        }
        public string GetWriterInf(ref string getchecker, ref string getFinalChecker)
        {
            if (tvPatients.SelectedNode.Level != 3) return null;
            TreeNode selectedNode = tvPatients.SelectedNode;
            int series = GetNoteSeries();
            //int index = GetIndex();
            /* Find out the xml node */
            int index = Convert.ToInt32(selectedNode.Parent.Parent.Tag);
            XmlElement emrNote = GetEmrNote(index, series);
            if (emrNote == null)
            {
                MessageBox.Show(ErrorMessage.EmrDocumentError, ErrorMessage.Error);
                return null;
            }
            #endregion
            string getwriter = emrNote.Attributes[AttributeNames.WriterID].Value;

            //getwriter += emrNote.Attributes[AttributeNames.Writer].Value;
            if (emrNote.Attributes[AttributeNames.Checker] != null)
            {
                getchecker = emrNote.Attributes[AttributeNames.CheckerID].Value;
            }
            if (emrNote.Attributes[AttributeNames.FinalChecker] != null)
            {
                getFinalChecker = emrNote.Attributes[AttributeNames.FinalCheckerID].Value;
            }
            return getwriter;
        }
        private void noteInfo_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (tvPatients.SelectedNode.Level != 3) return;
            TreeNode selectedNode = tvPatients.SelectedNode;
            int series = GetNoteSeries();
            int index = Convert.ToInt32(selectedNode.Parent.Parent.Tag);
            XmlNode emrNote = emrDocuments.GetEmrDocument(index).GetEmrNoteBySeries(series);
            if (emrNote == null)
            {
                MessageBox.Show(ErrorMessage.EmrDocumentError, ErrorMessage.Error);
                return;
            }
            EmrNoteInfo eni = new EmrNoteInfo(emrNote);
            eni.ShowDialog();
        }
        private void exit_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            menuPatient.Close();
        }
        private void deleteNote_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.DeleteNote)) return;
            if (IsLocked()) return;
            TreeNode selectNode = tvPatients.SelectedNode;
            if (selectNode == null) return;
            if (selectNode.Level != 3) return;
            #region confirm privilege
            /* Find out the emrNote . */
            int index = Convert.ToInt32(tvPatients.SelectedNode.Parent.Parent.Tag);
            int series = Convert.ToInt32(tvPatients.SelectedNode.Tag);
            string noteID = tvPatients.SelectedNode.Parent.Name;
            XmlElement emrNote = GetEmrNote(index, series);
            /* Do you have access privilege ? */
            if (emrDocuments.GetPermission(index) == PermissionLevel.ReadOnly)
            {
                MessageBox.Show(ErrorMessage.NoPrivilegeDelete, ErrorMessage.Error);
                return;
            }
            if (emrNote.Attributes[AttributeNames.WriterID].Value != Globals.DoctorID)
            {
                MessageBox.Show(ErrorMessage.NoPrivilegeDelete, ErrorMessage.Error);
                return;
            }
            NoteStatus status = (NoteStatus)Convert.ToInt32(emrNote.Attributes[AttributeNames.NoteStatus].Value);
            if (status != NoteStatus.Draft)
            {
                MessageBox.Show(ErrorMessage.OnlyDraftCanDelete, ErrorMessage.Warning);
                return;
            }

            /* Confirm */
            if (MessageBox.Show(ErrorMessage.ConfirmDeleteNote, ErrorMessage.Warning,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                == DialogResult.No) return;
            #endregion
            //if (Globals.Trace && Globals.DoctorID != StringGeneral.supperUser)
            //{
            //    if (emrNote.Attributes["fanxiu"] != null)
            //    {
            //        MessageBox.Show("返修过的病历不允许删除！", "警告");
            //        return;
            //    }

            //}
            /* Update database */
            if (emrDocuments.GetEmrDocument(index).DeleteEmrNote(series))
            {
                /* Update tree view */
                TreeNode parentNode = tvPatients.SelectedNode.Parent;
                TreeNode tn = tvPatients.SelectedNode.Parent.Parent;
                tvPatients.SelectedNode.Remove();
                if (parentNode.Nodes.Count == 0) parentNode.Remove();
                tvPatients.SelectedNode = tn;

            }
        }
        private void tvPatients_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            // 2012-5-10
            TreeNode tn = tvPatients.SelectedNode;
            if (tn == null)
            {
                return;
            }
            if (tn.Level == 1) openEmr_Click(sender, e);
            if (tn.Level == 3)
            {
                patient1.Visible = false;
                editNote_Click(sender, e);
            }
            return;
            if (e.Node.Level == 3)
            {
                patient1.Visible = false;           
                editNote_Click(sender, e);
                Globals.saveOk = false;
            }
        }
        private void tcPatients_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
        {
            tcPatients_s();
        }
        private void tcPatients_s()
        {
            string frmMainTitle = this.Text;
            this.Text = frmMainTitle + " - 载入中...";
            switch (tcPatients.SelectedTab.Text)
            {
                case "个人":
                    tnNode = null;
                    Globals.RegistryID = "";
                    LoadTvPatient();
                    ThisAddIn.xmlPatientWriter(Globals.DoctorID, tvPatients, 3, Globals.patientFile);
                    break;
                case "科室":
                    tnNode = null;
                    Globals.RegistryID = "";
                    LoadTvPatient();
                    ThisAddIn.xmlPatientWriter(Globals.OpDepartID, tvPatients, 2, Globals.patientFile);
                    break;
                case "病区":
                    tnNode = null;
                    Globals.RegistryID = "";
                    LoadTvPatient();
                    ThisAddIn.xmlPatientWriter(Globals.AreaID, tvPatients, 1, Globals.patientFile);
                    break;
                case "全院":
                    LoadDeparts();
                    break;
                default: break;
            }
            this.Text = frmMainTitle;
        }
       
        /* ---------------------------------------------------------------------------------------------
         * Find or add tree node level 2 (NoteName)
         * Parameters:
         *  in  noteID is an EmrNote's code
         *  in  noteName is an EmrNote's name
         *  in  nodeLevel1 is tree node level 1 (RegistryID)
         *  out nodeLevel2 is tree node level 2 (NoteName)
         * -----------------------------------------------------------------------------------------------*/
        private void NewLevel2(string noteID, string noteName, string unique, TreeNode nodeLevel1,
            out TreeNode nodeLevel2)
        {
            foreach (TreeNode node in nodeLevel1.Nodes)
            {
                /* Node level 2 of the noteID exists? */
                if (node.Name == noteID)
                {
                    nodeLevel2 = node;
                    return;
                }
            }
            /* Make a new Node level 2 for noteID. */
            nodeLevel1.Nodes.Add(noteName);
            nodeLevel2 = nodeLevel1.LastNode;
            nodeLevel2.Name = noteID;
            nodeLevel2.Tag = unique;
            nodeLevel2.ForeColor = Color.CornflowerBlue;
            nodeLevel2.ImageIndex = 5;
        }
        
        /* ---------------------------------------------------------------------------------------
         * Check if emr is opening.If so, return index for this emrDocument and
         * relative treenode registry node (level = 1)
   -----------------------------------------------------------------------------------------*/
        public int OpeningEmr(TreeNode selectedNode,out TreeNode registryNode)
        {
            int index = -1;
            registryNode = selectedNode;
            switch (selectedNode.Level)
            {
                case 2:
                    registryNode = selectedNode.Parent;
                    break;
                case 3:
                    registryNode = selectedNode.Parent.Parent;
                    break;
            }
            if (registryNode.Tag.ToString() != StringGeneral.RegistryClosed)
                index = Convert.ToInt32(registryNode.Tag);
            return index;
        }
        private void tvPatients_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvPatients.SelectedNode = e.Node;
            menuPatient.Close();
            if (e.Node.Level == 0)
            {
                plMain.Visible = true;
                plContain.Visible = true;
                patient1.Visible = true;
                patient1.Dock = DockStyle.Fill;
                EmrConstant.PatientInfo pi = GetPatientInfo(e.Node);

               
                    string itemText = Globals.doctors.GetDoctorName(e.Node.Nodes[0].Text.Split(':')[2]) + EmrConstant.Delimiters.Seperator;
                    if (e.Node.Nodes[0].Text.Split(':').Length < 5 || e.Node.Nodes[0].Text.Split(':')[4] != "已归档") itemText += "未归档" + EmrConstant.Delimiters.Seperator;
                    else itemText += "已归档" + EmrConstant.Delimiters.Seperator;
                    string ifRef = "未接收";
                    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                    {
                        DataSet dst = es.GetTransferInfoExEx(GetRegistryID(m_LastMouseOverNode), StringGeneral.NullCode, true);
                        if (dst.Tables[0].Rows.Count > 0) ifRef = "已接收";
                    }
                    itemText += ifRef + EmrConstant.Delimiters.Seperator;
                    patient1.InitControls(pi, itemText,ThisAddIn.Today());
                   
                
            }          
        }
       
        /* ---------------------------------------------------------------------------------------------
         * Create a new note that is xml node
         * Parameter:
         *      status is Draft or Commited.
         * Attribute:
         *      
         ----------------------------------------------------------------------------------------------- */
        public bool tvPatientsNoteAdd(string unique, NoteStatus status, EmrConstant.Button button)
        {
           
            TreeNode tvPatientsSelectedNode = tvPatients.SelectedNode;
            if (tvPatientsSelectedNode == null) return Return.Failed;
            #region Add new node in tree
            TreeNode noteIDNode, nodeLevel1;
            /* Is opening the relative emrDocument? If so return index of emrDocument*/
            int index = OpeningEmr(tvPatientsSelectedNode, out nodeLevel1);
            string noteID = currentNote.noteInfo.GetNoteID();
            #endregion
            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            if (!CheckSave()) return false;
            #region Add new note in EmrDocument
            //if (Globals.Sign)
            //{
            //    if (!ThisAddIn.ImageSign("医师")) Globals.Sign = false;
            //}
            //else if (Globals.showWriter) ThisAddIn.WriterSign("医师");
            NoteStatus statusEnd = status;
            string realname = "";
            int series = NewEmrNote(index, noteID, status, button, ref statusEnd, ref realname);
            if (series < 0)
            {
                string startTime = Globals.emrPattern.StartTimeAttribute(noteID);
                OpDone opDone;
                if (startTime == StartTime.Consult)
                {
                    opDone = new OpDone("此次会诊记录已被书写完成!寄存失败！");
                }
                else
                {
                    opDone = new OpDone("离线，寄存失败！");
                }

                opDone.Show();
                return EmrConstant.Return.Failed;
            }
            EmrDocInfo.setSeriesIDForCheck(series);
       
            EmrDocInfo.insertOK(series);

            //if (ThisAddIn.CanOption(ElementNames.ClinicPath) && ThisAddIn.CanOptionText(ElementNames.PathID).Trim() != "" && (ThisAddIn.CanOptionText(ElementNames.PathID).Trim() == Globals.NoteID))
            //{
            //    tvPatients.SelectedNode.Tag = series;
            //    tvPatients.SelectedNode.Text = "临床路径表单 已终审 " + ThisAddIn.Today().ToString(StringGeneral.DateFormat);
            //}
            //else
            //{
            try
            {
                string noteNameInPattern = Globals.emrPattern.GetNoteNameFromNoteID(noteID);
                NewLevel2(noteID, noteNameInPattern, unique, nodeLevel1, out noteIDNode);

                noteIDNode.Nodes.Add(ThisAddIn.Today().ToString(StringGeneral.DateFormat));
                noteIDNode.LastNode.Name = statusEnd.ToString("d");
                /**ZZL**/
                if (string.IsNullOrEmpty(realname))
                {
                    noteIDNode.LastNode.Text = noteNameInPattern + NoteState(noteIDNode.LastNode.Name) + noteIDNode.LastNode.Text;
                }
                else
                {
                    noteIDNode.LastNode.Text = realname + NoteState(noteIDNode.LastNode.Name) + noteIDNode.LastNode.Text;
                }
                noteIDNode.LastNode.ImageIndex = 5;
                noteIDNode.LastNode.ForeColor = Color.DimGray;
                tvPatients.SelectedNode = noteIDNode.LastNode;
                noteIDNode.LastNode.Tag = series.ToString();
                emrDocuments.SetOld(index);
            }
            catch (Exception ex)
            {

                MessageBox.Show("treeView 生成出问题:"+ex);
                return Return.Failed;
            }
           // }
            #endregion

            return Return.Successful;
        }
        public bool tvPatientsNoteUpdate(NoteStatus status, EmrConstant.Button button)
        {
            TreeNode selectedNode = tvPatients.SelectedNode;

            int series = Convert.ToInt32(selectedNode.Tag);
            EmrDocInfo.checkSeries(series);
            int index = Convert.ToInt32(selectedNode.Parent.Parent.Tag);
            string[] strList = selectedNode.Parent.Parent.Text.Split(EmrConstant.Delimiters.Colon);
            string registryID = strList[1].Trim();
            XmlNode emrNote = emrDocuments.GetEmrDocument(index).GetEmrNoteBySeries(series);
            EmrDocInfo.setNoteIDForCheck(emrNote.Attributes["NoteID"].Value);
      
            if (emrNote == null) return Return.Failed;
            //if (currentNote.ElementFromBookmarks((XmlElement)emrNote, false, button) == Return.Failed)
            //{                
            //    return Return.Failed;
            //}            
              string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
              if (!CheckSave()) return false;
           
            try
            {
                udt.jj.SaveWordDoc(ActiveDocumentManager.getDefaultAD(), tmpfile, false);

            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX9134474780", ex.Message + ">>" + ex.ToString(), true);
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
            Globals.docPath = ActiveDocumentManager.getDefaultAD().Path;
            NoteStatus statusEnd = status;
            if (emrDocuments.GetEmrDocument(index).UpdateEmrNote(series, status, ref statusEnd) == Return.Failed)
                return Return.Failed;

            selectedNode.Name = statusEnd.ToString("d");
            selectedNode.Text = selectedNode.Text.Split(' ')[0] + NoteState(selectedNode.Name.Split(':')[0]) +  emrNote.Attributes[AttributeNames.WrittenDate].Value;
            return Return.Successful;
        }
        public TreeNode FindNoteIDNode(TreeNode registryIDNode, string noteID)
        {
            for (int i = 0; i < registryIDNode.Nodes.Count; i++)
                if (registryIDNode.Nodes[i].Name == noteID) return registryIDNode.Nodes[i];
            return null;
        }
       
        /* ------------------------------------------------------------------------------------
       * Create a new EmrNote that is xml node
       * Parameters:
       *      int index                     -- directs an EmrDocument that is xml document.
       *      EmrConstant.NoteStatus status -- is Draft, Commited, Checking, Checked
       *                                          FinalChecking or FinalChecked
       *      string noteID                 -- identifies a kind of progress note.
       *      string noteName               -- is the Chinese name of this progress note.
       *      string header                 -- Big or Small header
       *      tring unique                  -- Yes or No 
       * Atrribute:
       *      currentNote is bridge between xml node and word window.
       * Return:
       *      series is the index of emrNote for searching.
       -------------------------------------------------------------------------------------- */
        private int NewEmrNote(int index, string noteID, NoteStatus status, EmrConstant.Button button,
            ref NoteStatus statusEnd, ref string RealName)
        {
            Cursor.Current = Cursors.WaitCursor;
            bool canCommit = true;
            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            try
            {
                udt.jj.SaveWordDoc(ActiveDocumentManager.getDefaultAD(), tmpfile, false);
                Globals.docPath = ActiveDocumentManager.getDefaultAD().Path;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX741852980", ex.Message + ">>" + ex.ToString(), true); 
           
            }

            /* Create elements of SubTitle and its child elements and fill them with word window. */
            XmlElement emrNote = emrDocuments.Get(index).CreateElement(ElementNames.EmrNote);
            DateTime now = ThisAddIn.Today();
            emrNote.SetAttribute(AttributeNames.WrittenDate, now.ToString(StringGeneral.DateFormat));
            emrNote.SetAttribute(AttributeNames.WrittenTime, now.ToShortTimeString());
            string childID = currentNote.noteInfo.GetChildID();
            emrNote.SetAttribute(AttributeNames.ChildID, childID);

            string notename = "";
            if (childID == null || childID == "0" || childID == noteID)
            {
                notename = Globals.emrPattern.GetNoteNameFromNoteID(noteID);
            }
            else notename = Globals.childPattern.GetNoteNameFromNoteID(childID);
            emrNote.SetAttribute("RealName", notename);
            RealName = notename;
            if (status == NoteStatus.Commited && ThisAddIn.CanOption(ElementNames.EnterCommitTime) &&
               ThisAddIn.CanOption(ElementNames.EnterCommitTime))
            {
                string registryID = "";
                int iseries = -1; ;
                canCommit = ChooseCommitTime(emrNote, registryID, iseries);
                if (!canCommit)
                {
                    status = NoteStatus.Draft;
                    MessageBox.Show("已经超时，只能保存不能提交，请与质管部门联系！", ErrorMessage.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                }

            }
            //if (currentNote.ElementFromBookmarks(emrNote, true, button) == Return.Failed)
            //{
            //    return -1;
            //}

            XmlNode notePattern = Globals.emrPattern.GetEmrNote(noteID);
            EmrDocument emrDoc = emrDocuments.GetEmrDocument(index);
            string archiveNum = GetPatientArchiveNum();
            XmlNode WordXml = null;
            if (button == EmrConstant.Button.CommitNote) WordXml = GetXmlfile();
            int series = emrDoc.AddEmrNote(notePattern, emrNote, status, currentNote.noteInfo.sexOption, WordXml);
            if (!canCommit)
            {
                using (gjtEmrService.emrServiceXml Service = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        string strResult = Service.EnableCommitLog(Globals.DoctorID, GetRegistryID(), series, emrNote.Attributes[AttributeNames.NoteName].Value, GetPatientName());
                        if (strResult != null)
                            return -1;
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX741852070", ex.Message + ">>" + ex.ToString(), true); 
           
                        return -1;
                    }
                }
            }
            if (status == NoteStatus.Commited && emrNote.Attributes[AttributeNames.StartTime].Value == StartTime.Consult)
            {
                ThisAddIn.FinishConsultNote(emrNote.Attributes[AttributeNames.Sequence].Value, emrDoc.GetRegistryID());
            }
            Cursor.Current = Cursors.Default;
            statusEnd = status;
            return series;
        }
        private void DeleteInfs()
        {
            int i = 0;
            //Word.XMLNode bc = ActiveDocumentManager.getDefaultAD().XMLNodes[1];

            try
            {
                foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
                {

                    if (xn.BaseName == "合并")
                    {
                        if (i != 0)
                        {
                            xn.Text = "";
                            xn.Delete();
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX741851970", ex.Message + ">>" + ex.ToString(), true); 
           
            }
        }
        private NoteEditMode EditModeWhenNoteChecked(PermissionLevel permission, string writerID, string checkerID)
        {
            string opcode = Globals.DoctorID;
            switch (permission)
            {
                case EmrConstant.PermissionLevel.ReadWrite:
                    if (ThisAddIn.IsUpperUpperDoctor(opcode, checkerID) && opcode != checkerID)
                        return EmrConstant.NoteEditMode.FinallyCkecking;
                    else
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.ReadOnly:
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.RevisionOnly:
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.FinalRevisionOnly:
                    if ((opcode == writerID || opcode == checkerID) && ThisAddIn.CanOption(ElementNames.RevisionBy) == false)
                        return EmrConstant.NoteEditMode.Reading;
                    else
                    return EmrConstant.NoteEditMode.FinallyCkecking;
                case EmrConstant.PermissionLevel.Trust:
                    if (ThisAddIn.IsUpperUpperDoctor(opcode, writerID))
                        return EmrConstant.NoteEditMode.FinallyCkecking;
                    else
                        return EmrConstant.NoteEditMode.Reading;
                default:
                    return EmrConstant.NoteEditMode.Nothing;
            }
        }

        /* ------------------------------------------------------------------------------------ */
        private NoteEditMode EditModeWhenNoteFinalChecking(PermissionLevel permission, string finalCheckerID)
        {
            string opcode = Globals.DoctorID;
            if (opcode == finalCheckerID) return EmrConstant.NoteEditMode.FinallyCkecking;
            else return EmrConstant.NoteEditMode.Reading;

        }
        private NoteEditMode EditModeWhenNoteFinalChecked(PermissionLevel permission)
        {
            switch (permission)
            {
                case EmrConstant.PermissionLevel.ReadWrite:
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.ReadOnly:
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.RevisionOnly:
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.FinalRevisionOnly:
                    return EmrConstant.NoteEditMode.Reading;
                case EmrConstant.PermissionLevel.Trust:
                    return EmrConstant.NoteEditMode.Reading;
                default:
                    return EmrConstant.NoteEditMode.Nothing;
            }
        }
        public string GetLevelCode(string doctorID)
        {
            if (Doctors == null) return null;
            foreach (XmlNode doctor in Doctors.SelectNodes(ElementNames.Doctor))
            {
                if (doctor.Attributes[AttributeNames.Code].Value == doctorID)
                {
                    return doctor.Attributes[AttributeNames.LevelCode].Value;
                }
            }
            return null;
        }
        public PermissionLevel GetPermission(string userID, string WriterID)
        {
            if (userID == WriterID) return PermissionLevel.ReadOnly;
            string level1 = GetLevelCode(userID);
            string level2 = GetLevelCode(WriterID);
            if (level1.Substring(0, 2).Equals(level2.Substring(0, 2)) && Convert.ToInt32(level1) <= Convert.ToInt32(level2))
                return PermissionLevel.FinalRevisionOnly;
            else
                return PermissionLevel.ReadOnly;

        }
        private void SetRevision()
        {
            ActiveDocumentManager.getDefaultAD().ShowRevisions = true;
        }
        private void SetCheckInfo(XmlElement emrNote)
        {
            emrNote.Attributes[AttributeNames.CheckerID].Value = Globals.DoctorID;
            emrNote.Attributes[AttributeNames.Checker].Value = Globals.DoctorName;
            emrNote.Attributes[AttributeNames.CheckedDate].Value =
               ThisAddIn.Today().ToString(StringGeneral.DateFormat) + Delimiters.Space +
               ThisAddIn.Today().ToShortTimeString();
        }
        private void SetFinalCheckInfo(XmlElement emrNote)
        {
            emrNote.Attributes[AttributeNames.FinalCheckerID].Value = Globals.DoctorID;
            emrNote.Attributes[AttributeNames.FinalChecker].Value = Globals.DoctorName;
            emrNote.Attributes[AttributeNames.FinalCheckedDate].Value =
               ThisAddIn.Today().ToString(StringGeneral.DateFormat) + Delimiters.Space +
                ThisAddIn.Today().ToShortTimeString();
        }
        private void SetWriteInfo(XmlElement emrNote)
        {
            emrNote.Attributes[AttributeNames.WrittenTime].Value = ThisAddIn.Today().ToShortTimeString();
            emrNote.Attributes[AttributeNames.WrittenDate].Value =
               ThisAddIn.Today().ToString(StringGeneral.DateFormat);
        }
        public bool IsLocked()
        {
            //return tcPatients.Enabled;
            return false;
        }
        public void SetLastTop(string groupCode, long top)
        {
            int index = GetDocmentIndex();
            emrDocuments.GetEmrDocument(index).SetLastTop(groupCode, top);
        }
        public long GetLastTop(string groupCode)
        {
            int index = GetDocmentIndex();
            return emrDocuments.GetEmrDocument(index).GetLastTop(groupCode);
        }
        public void SetLastPageNumber(string groupCode, int pageNumber)
        {
            int index = GetDocmentIndex();
            emrDocuments.GetEmrDocument(index).SetLastPageNumber(groupCode, pageNumber);
        }
        public int GetLastPageNumber(string groupCode)
        {
            int index = GetDocmentIndex();
            return emrDocuments.GetEmrDocument(index).GetLastPageNumber(groupCode);
        }
        public int GetDocmentIndex()
        {
            if (tvPatients.SelectedNode == null) return -1;
            switch (tvPatients.SelectedNode.Level)
            {
                case 1:
                    return Convert.ToInt32(tvPatients.SelectedNode.Tag);
                case 2:
                    return Convert.ToInt32(tvPatients.SelectedNode.Parent.Tag);
                case 3:
                    return Convert.ToInt32(tvPatients.SelectedNode.Parent.Parent.Tag);
            }
            return -1;
        }
        private void RePrintEndSelf(Word.Pane pane, int space, bool printEnd, string groupCode,string isSingle)
        {
            Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.First.Range;
            Word.Range lastRange = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range.Start = 0;
            range.End = 0;
            if (space > 1)
            {
                for (int i = 1; i < space - 1; i++)
                {
                    range.InsertAfter("\r");
                }
            }
            if (space == 2)
            {
                range.InsertAfter("\r");
                deleteNewLine(range, 1);
            }
            string RegistryID = GetRegistryID();
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                Word.Range rg = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                rg.Start = 0;
                lastRange = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                lastRange.Start = 0;
                deleteNewLine(lastRange, 1);
                Word.Pane paneTemp = wordApp.ActiveWindow.Panes[1];
                int pageCount = pane.Pages.Count;
                
                if (pageCount > 1)
                {
                    rg.Start = paneTemp.Pages[pageCount].Breaks[1].Range.Start;
                }
                else
                {
                    rg.Start = 0;
                }
                string strWordXML = rg.WordOpenXML;
                try
                {
                    rg.InsertAfter("\r");
                    lastRange = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                    lastRange.Start = 0;
                    deleteNewLine(lastRange, 1);
                    int pageCount2 = paneTemp.Pages.Count;
                    deleteNewLine(lastRange, 1);

                    if (!printEnd)
                    {
                        //if (pageCount2 > pageCount)
                        //{
                        //    es.DeletRange(RegistryID);
                        //}
                        //else
                        //{
                        //    es.PutRange(RegistryID, strWordXML);
                        //}
                        if (pageCount2 > pageCount)
                        {
                            //es.DeletRange(RegistryID);
                            if (groupCode == "1")
                            {
                                es.DeletRange(RegistryID);

                            }
                            else if (groupCode == "2")
                            {
                                es.DeletRangex1(RegistryID);
                            }
                            else if (groupCode == "3")
                            {
                                es.DeletRangex2(RegistryID);
                            }
                        }
                        else
                        {
                            // es.PutRange(RegistryID, strWordXML);
                            /**zzl 断续*/
                            if (isSingle != "yes")
                            {

                                if (groupCode == "1")
                                {
                                    es.PutRange(RegistryID, strWordXML);

                                }
                                else if (groupCode == "2")
                                {
                                    es.PutRangex1(RegistryID, strWordXML);
                                }
                                else if (groupCode == "3")
                                {
                                    es.PutRangex2(RegistryID, strWordXML);
                                }
                            }
                            else
                            {
                                if (groupCode == "1")
                                {
                                    es.DeletRange(RegistryID);

                                }
                                else if (groupCode == "2")
                                {
                                    es.DeletRangex1(RegistryID);
                                }
                                else if (groupCode == "3")
                                {
                                    es.DeletRangex2(RegistryID);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741812970", ex.Message + ">>" + ex.ToString(), true); 
           
                }
            }
        }
        private void RePrintEnd(ref int cjjLastPageNumber, ref bool IsRP,
            string pageHeader, string endPrintNoteName, string endPrintNoteID,
            Word.Pane pane, Word.Font pageHeaderFont, string groupCode, string isSingle)
        {
            

            object Unit = (int)Word.WdUnits.wdCharacter;
            object Count = 1;
            string RegistryID = GetRegistryID();
            string DocRange = "";
            /**zzl 断续*/
            if (isSingle == "yes")
            {
                using (gjtEmrService.emrServiceXml ess = new gjtEmrService.emrServiceXml())
                {
                    ess.DeletRange(GetRegistryID());
                    cjjLastPageNumber++;
                    return;
                }
            }

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                if (groupCode == "2")
                {
                    IsRP = es.IsRemberPoint1(RegistryID, ref DocRange);

                }
                else if (groupCode == "1")
                {
                    IsRP = es.IsRemberPoint(RegistryID, ref DocRange);
                }
                else if (groupCode == "3")
                {
                    IsRP = es.IsRemberPoint2(RegistryID, ref DocRange);
                }
               // IsRP = es.IsRemberPoint(RegistryID, ref DocRange);

                if (!IsRP)
                {
                    cjjLastPageNumber++;
                    MessageBox.Show(ErrorMessage.MustUseBlankPaper, ErrorMessage.Warning,
                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.First.Range;
                    range.Start = 0;
                    range.End = 0;
                    DocRange = DocRange.Replace('$', '\v');
                    if (DocRange.IndexOf("<?xml version=\"1.0\" standalone=\"yes\"?>") == 0)
                    {
                        object obj = null;
                        string strXML = ActiveDocumentManager.getDefaultAD().WordOpenXML;
                        ActiveDocumentManager.getDefaultAD().Select();
                        ActiveDocumentManager.getDefaultAD().ActiveWindow.Selection.Delete(ref Unit, ref Count);
                        range.InsertXML(DocRange, ref obj);
                        ActiveDocumentManager.getDefaultAD().Select();
                        ActiveDocumentManager.getDefaultAD().ActiveWindow.Selection.Font.Color = Word.WdColor.wdColorWhite;
                        Word.Range rTemp = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                        rTemp.Start = rTemp.End;
                        rTemp.InsertXML(strXML, ref obj);
                    }
                    else
                    {
                        range.Text = DocRange;
                        for (int k = ActiveDocumentManager.getDefaultAD().Paragraphs.Count; k > 1; k--)
                        {
                            Word.Range ranges = ActiveDocumentManager.getDefaultAD().Paragraphs[k].Range;
                            if (IsNullParagraph(ranges.Text))
                            {
                                ActiveDocumentManager.getDefaultAD().Paragraphs[k - 1].Range.Text = "";
                                break;
                            }
                        }
                        range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpace1pt5;
                        range.Font.Color = Word.WdColor.wdColorWhite;
                    }
                }
                Word.Range rg = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                rg.Start = 0;
                Word.Range lastRange = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                lastRange.Start = 0;
                deleteNewLine(lastRange, 1);
                Word.Pane paneTemp = wordApp.ActiveWindow.Panes[1];
                int pageCount = pane.Pages.Count;
                Word.Page p = wordApp.ActiveWindow.Panes[1].Pages[1];
                if (pageCount > 1)
                {
                    rg.Start = paneTemp.Pages[pageCount].Breaks[1].Range.Start;
                    //rg.InsertBreak(Word.WdSectionStart.wdSectionNewPage);
                   // SplitPagesBySection.split2(wordApp);
                }
                else
                {
                    rg.Start = 0;
                }
                string strWordXML = rg.WordOpenXML;
                try
                {
                    rg.InsertAfter("\r");
                    lastRange = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                    lastRange.Start = 0;
                    deleteNewLine(lastRange, 1);
                    int pageCount2 = paneTemp.Pages.Count;
                    deleteNewLine(lastRange, 1);
                    if (pageCount2 > pageCount)
                    {
                        //es.DeletRange(RegistryID);
                        if (groupCode == "1")
                        {
                            es.DeletRange(RegistryID);

                        }
                        else if (groupCode == "2")
                        {
                            es.DeletRangex1(RegistryID);
                        }
                        else if (groupCode == "3")
                        {
                            es.DeletRangex2(RegistryID);
                        }
                    }
                    else
                    {
                        // es.PutRange(RegistryID, strWordXML);
                        if (groupCode == "1")
                        {
                            es.PutRange(RegistryID, strWordXML);

                        }
                        else if (groupCode == "2")
                        {
                            es.PutRangex1(RegistryID, strWordXML);
                        }
                        else if (groupCode == "3")
                        {
                            es.PutRangex2(RegistryID, strWordXML);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741252970", ex.Message + ">>" + ex.ToString(), true);

                }
            }
        }
        private void deleteNewLine(Word.Range range, int lineMarkCount)
        {
            object Unit = (int)Word.WdUnits.wdCharacter;
            object Count = 1;
            for (int i = range.Characters.Count; i > 0; i--)
            {
                if (range.Characters[i].Text.Equals("\r"))
                {
                    lineMarkCount--;
                    range.Characters[i].Delete(ref Unit, ref Count);
                    if (lineMarkCount <= 0)
                    {
                        break;
                    }
                }
            }
        }
        private void deleteNewLine(Word.Range range)
        {
            object Unit = (int)Word.WdUnits.wdCharacter;
            object Count = 1;
            for (int i = range.Characters.Count; i > 0; i--)
            {
                if (range.Characters[i].Text.Equals("1"))
                {                   
                    range.Characters[i].Delete(ref Unit, ref Count);                    
                }
               
            }
        }
        public bool IsNullParagraph(string rangeText)
        {
            if (rangeText == null) return true;

            char[] chs = rangeText.ToCharArray();
            for (int i = 0; i < chs.Length; i++)
            {
                if (chs[i] != ' ' && chs[i] != '\r') return false;
            }
            return true;
        }
        public void SetFooterForEndPrint(string noteName, int beginNumber, bool firstPage)
        {
            Word.HeaderFooter hf = ActiveDocumentManager.getDefaultAD().Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
            hf.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            hf.Range.Text = "第   页";

            hf.PageNumbers.NumberStyle = Word.WdPageNumberStyle.wdPageNumberStyleArabic;
            hf.PageNumbers.RestartNumberingAtSection = true;
            hf.PageNumbers.StartingNumber = beginNumber;
            object alignment = Word.WdPageNumberAlignment.wdAlignPageNumberCenter;
            object oFirstPage = true;
            Word.PageNumber pn = hf.PageNumbers.Add(ref alignment, ref oFirstPage);
            //return;
            if (!firstPage)
                DelPageHeader();
        }
        private void DelPageHeader()
        {
            object objNULL = null;
            Microsoft.Office.Interop.Word.Document WordDoc = ActiveDocumentManager.getDefaultAD();
            Word.Application WordApp = wordApp;
            UnProtectDoc();
            try
            {
                WordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekPrimaryHeader;
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX741452970", ex.Message + ">>" + ex.ToString(), true); 
           
            }
            
            ArrayList llObj = new ArrayList();
            for (int i = 1; i <= WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Shapes.Count; i++)
            {
                object j = i;
                llObj.Add(WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Shapes.get_Item(ref j).ID);
            }


           // WordDoc.Sections.Application.Selection.WholeStory();
            string strWordXML = WordDoc.Sections.Application.Selection.WordOpenXML;
            ActiveDocumentManager.getDefaultAD().PageSetup.DifferentFirstPageHeaderFooter = -1;
            WordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekFirstPageHeader;
            WordDoc.Sections.Application.Selection.WholeStory();

            WordDoc.Sections.Application.Selection.InsertXML(strWordXML, ref objNULL);
            ArrayList ll = new ArrayList();
            for (int i = 1; i <= WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Shapes.Count; i++)
            {
                object j = i;
                IDAndRef iAr = new IDAndRef();
                iAr.Ref = WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Shapes.get_Item(ref j);
                iAr.ID = iAr.Ref.ID;
                ll.Add(iAr);
            }
            Word.HeaderFooter hf = ActiveDocumentManager.getDefaultAD().Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
            //if (hf.Shapes.Count != 0)
               // hf.Shapes[0].Delete();
            ArrayList llRemove = new ArrayList();
            bool bFind = false;
            for (int i = 0; i < ll.Count; i++)
            {
                bFind = false;
                for (int j = 0; j < llObj.Count; j++)
                {
                    if ((int)llObj[j] == ((IDAndRef)ll[i]).ID)
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                {
                    llRemove.Add(ll[i]);
                }
            }
            foreach (IDAndRef obj in llRemove)
            {
                Microsoft.Office.Interop.Word.Shape shape = ((IDAndRef)obj).Ref;
                shape.Delete();
            }
            try
            {
                for (int i = 1; i <= WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Range.ContentControls.Count; i++)
                {
                    object objTemp = i;
                    WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Range.ContentControls.get_Item(ref objTemp).LockContents = false;
                }
                WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Range.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                //isLastTwoNewLineMark(WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Range);
            }
            catch (Exception ex)
            {
                WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekMainDocument;
                Globals.logAdapter.Record("EX751852970", ex.Message + ">>" + ex.ToString(), true); 
           
            }
            object Unit = (int)Word.WdUnits.wdCharacter;
            object Count = 10;

            //2012-03-20页眉底纹删除 横线
            int c = ActiveDocumentManager.getDefaultAD().Paragraphs[1].Borders.Count;
            if (c != 0)
            {
                Word.Border border = ActiveDocumentManager.getDefaultAD().Paragraphs[1].Borders[Word.WdBorderType.wdBorderTop];
                border.Visible = false;
                Word.Border border2 = ActiveDocumentManager.getDefaultAD().Paragraphs[1].Borders[Word.WdBorderType.wdBorderBottom];
                border2.Visible = false;
            
            }
            //Microsoft.Office.Interop.Word.Range r = WordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Range;
            //int start = r.Characters.Count + 1;
            //r.SetRange(start, start + 1);
            //r.Delete(ref Unit, ref Count);
            //WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekMainDocument;
        }
        private class IDAndRef
        {
            private int _ID;
            private Microsoft.Office.Interop.Word.Shape _Ref;
            public int ID { get { return _ID; } set { _ID = value; } }
            public Microsoft.Office.Interop.Word.Shape Ref { get { return _Ref; } set { _Ref = value; } }
        }
        public void ChangeRange(Word.ContentControl cc, DataSet dst, string registryID)
        {
            string str;
            string title = cc.Title;
            switch (title)
            {
                case "PclinicID":
                    using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                    {
                        if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + ep.GetPatientClinicID(registryID);
                        else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + ep.GetPatientClinicID(registryID);
                    }
                    break;
                case "":
                    cc.LockContentControl = true;
                    break;
                case "Control":
                    cc.LockContentControl = true;
                    break;
                case "Label":
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Writer":
                    //if (Globals.ThisAddIn.showWriter != false)
                    //{
                    //    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                    //        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + logon.GetOpName();
                    //    else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + logon.GetOpName();
                    //}
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Checker":

                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "FinalChecker":

                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Bffh":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["BFFH"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + dst.Tables[0].Rows[0]["BFFH"].ToString();
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Bqmc":
                    cc.Range.Text = dst.Tables[0].Rows[0]["BQMC"].ToString() + " 区";
                    cc.LockContents = true;
                    break;
                case "Bqbm":
                    cc.Range.Text = dst.Tables[0].Rows[0]["BQBM"].ToString() + " 区";
                    cc.LockContents = true;
                    break;
                case "WrittenDate":
                    // if (Globals.ThisAddIn.showWriter != false)
                    //MessageBox.Show("时间获取失败！");
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "CheckedDate":

                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "FinalCheckedDate":

                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Dname":
                    string strs;
                    using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                    {
                        strs = ep.GetDepartmentName(dst.Tables[0].Rows[0]["ksbm"].ToString());
                    }
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + strs;
                    else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + strs;
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Pname":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["xm"].ToString();
                    else
                    {
                        if (cc.Range.Text.Split(':')[0] != cc.Range.Text)
                            cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + dst.Tables[0].Rows[0]["xm"].ToString();
                        else cc.Range.Text = dst.Tables[0].Rows[0]["xm"].ToString();
                    }
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Parchive":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["bah"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + dst.Tables[0].Rows[0]["bah"].ToString();
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Psex":

                    if (dst.Tables[0].Rows[0]["xb"].ToString() == "1") str = "男";
                    else str = "女";

                    cc.Range.Text = "性别：" + str;

                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Birthday":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["csny"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + dst.Tables[0].Rows[0]["csny"].ToString();
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Pnation":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = "民族：" + dst.Tables[0].Rows[0]["mz"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + dst.Tables[0].Rows[0]["mz"].ToString();
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Pmarriage":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["hf"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + dst.Tables[0].Rows[0]["hf"].ToString();
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Pland":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = "籍贯：" + dst.Tables[0].Rows[0]["jg"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + dst.Tables[0].Rows[0]["jg"].ToString();
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Pjob":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["zy"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + dst.Tables[0].Rows[0]["zy"].ToString();
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Paddr":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["jtzz"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + dst.Tables[0].Rows[0]["jtzz"].ToString();
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Preg":
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = "住院号：" + dst.Tables[0].Rows[0]["zyh"].ToString();
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + dst.Tables[0].Rows[0]["zyh"].ToString();
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Pbed":
                    if (cc.Range.Text.Trim().Equals("床"))
                        cc.Range.Text = dst.Tables[0].Rows[0]["ch"].ToString() + " 床";
                    else cc.Range.Text = "床号：" + dst.Tables[0].Rows[0]["ch"].ToString();
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Pdayin":
                    str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("f");
                    str = str.Split(' ')[0];
                    if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                        cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                    else cc.Range.Text = cc.Range.Text.Split(':')[0].Trim() + "：" + str;
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Pdayout":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        if (dst.Tables[0].Rows[0]["cyrq"].ToString() == null || dst.Tables[0].Rows[0]["cyrq"].ToString() == "")
                        {
                            cc.LockContentControl = true;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                            if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                                cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                            else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + str;

                            cc.LockContentControl = true;
                        }
                    }
                    else
                    {

                        if (dst.Tables[0].Rows[0]["cyrq"].ToString() == null || dst.Tables[0].Rows[0]["cyrq"].ToString() == "")
                        {
                            cc.LockContentControl = true;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                            str = str.Replace(":", "时");
                            str = str + "分";
                            if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                                cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                            else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + str;
                            cc.LockContentControl = true;
                        }
                    }
                    break;
                case "Ptimeout":
                    if (dst.Tables[0].Rows[0]["cyrq"].ToString() == null || dst.Tables[0].Rows[0]["cyrq"].ToString() == "")
                    {
                        cc.LockContentControl = true;
                    }
                    else
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                        //if (!ShowTime)
                        //{
                        //    str = str.Replace(":", "时");
                        //    str = str + "分";
                        //}
                        if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                        else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + str;

                        cc.LockContentControl = true;
                    }
                    break;
                case "Ptimeouts":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        if (dst.Tables[0].Rows[0]["cyrq"].ToString() == null || dst.Tables[0].Rows[0]["cyrq"].ToString() == "")
                        {
                            cc.LockContentControl = true;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                            str = str.Split(' ')[1];
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                            cc.LockContentControl = true;
                        }
                    }
                    else
                    {
                        if (dst.Tables[0].Rows[0]["cyrq"].ToString() == null || dst.Tables[0].Rows[0]["cyrq"].ToString() == "")
                        {
                            cc.LockContentControl = true;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                            str = str.Split(' ')[1];
                            str = str.Replace(":", "时");
                            str = str + "分";
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                            cc.LockContentControl = true;
                        }

                    }
                    break;
                case "Page":

                    string nldw = dst.Tables[0].Rows[0]["nldw"].ToString();
                    cc.Range.Text = "年龄：" + dst.Tables[0].Rows[0]["nl"].ToString().Split('.')[0] + nldw;
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    break;
                case "Phone":
                    cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["dh"].ToString();
                    cc.LockContents = true;
                    cc.LockContentControl = true;
                    break;
                case "Ptimein":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("f");
                        if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                        else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + str;
                        cc.LockContents = true;
                        cc.LockContentControl = true;
                    }
                    else
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("f");
                        str = dst.Tables[0].Rows[0]["zyrq"].ToString();
                        str = str.Replace(":", "时");
                        str = str + "分";
                        if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                        else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + str;
                        cc.LockContents = true;
                        cc.LockContentControl = true;

                    }
                    break;
                case "Ptimeins":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("f");
                        str = str.Split(' ')[1];
                        if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                        else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + str;

                        cc.LockContentControl = true;
                    }
                    else
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("f");
                        str = dst.Tables[0].Rows[0]["zyrq"].ToString();
                        str = str.Replace(":", "时");
                        str = str + "分";
                        str = str.Split(' ')[1];
                        if (cc.Range.Text.Split('：')[0] != cc.Range.Text)
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + str;
                        else cc.Range.Text = cc.Range.Text.Split(':')[0] + "：" + str;

                        cc.LockContentControl = true;

                    }
                    break;
                case "Ptoday":
                    str = DateTime.Now.Date.ToString("f");
                    str = str.Split(' ')[0];
                    cc.Range.Text = str;

                    cc.LockContentControl = true;
                    break;
                case "Pnow":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = DateTime.Now.ToString("f");
                        cc.Range.Text = str;
                        cc.LockContentControl = true;
                    }
                    else
                    {
                        str = DateTime.Now.ToString("f");
                        str = str.Split(':')[0];
                        str = str + "时";
                        cc.Range.Text = str;
                        cc.LockContentControl = true;

                    }
                    break;
                case "Psnow":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = DateTime.Now.ToString("f");
                        cc.Range.Text = str;
                        cc.LockContentControl = true;
                    }
                    else
                    {
                        str = DateTime.Now.ToString("f");
                        str = str.Split(':')[0];
                        str = str + "时";
                        cc.Range.Text = str;
                        cc.LockContentControl = true;

                    }
                    break;
               
                default:
                    break;
            }
        }
        
        /* ------------------------------------------------------------------------------ 
      * Quit the application.
      * ------------------------------------------------------------------------------ */
        public void ExitWord()
        {
            oDocument = null;
            axWbDoCView.Navigate("about:blank");
        }
    }
}