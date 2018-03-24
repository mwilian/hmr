using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Xml;
using UserInterface;
using EmrConstant;
using CommonLib;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Collections;
using Office = Microsoft.Office.Core;
using HuronControl;


namespace EMR
{
    public partial class MainForm 
    {
        private bool ChooseCommitTime(XmlNode emrNote, string registryID, int series)
        {
            DateTime now = ThisAddIn.Today();
            GetTime getTime = new GetTime();
            //getTime.Location = ThisAddIn.FitLocation(getTime.Size);
            if (registryID != "" && series != -1)
            {
                using (gjtEmrService.emrServiceXml Service = new gjtEmrService.emrServiceXml())
                {

                    string msg = Service.IsEnabledCommit(registryID, series);
                    if (msg == StringGeneral.One) return true;
                }
            }
            if (getTime.ShowDialog() == DialogResult.OK)
            {
                 now = getTime.GetCommitDateTime();
            }

           // ThisAddIn.SetDateTimeForNote(now);
            if (Globals.commitTimeOut > 0)
            {
                TimeSpan timeSpan = ThisAddIn.Today() - now;
                int Span = Convert.ToInt32(timeSpan.Days * 24 + timeSpan.Hours);
                return (Span <= Globals.commitTimeOut);
            }
            return true;
        }
        public string GetNoteStatus()
        {
            if (tvPatients.SelectedNode.Level != 3) return "0";
            return tvPatients.SelectedNode.Name.Split(':')[0];
        }
        //private void ResetDocument()
        //{           
           
        //    TvPatientsEnable(true);
        //    string Subjective = "";
        //    string InSituation = "";
        //    string Diagnose = "";
        //    string noteID = currentNote.noteInfo.GetNoteID();
        //    currentNote = null;
        //    if (noteID == "01" && ThisAddIn.CanOption(ElementNames.GetValue) == true)
        //    {
        //        foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
        //        {
        //            if (xn.BaseName == "主诉")
        //            {
        //                Subjective = xn.Text;
        //            }


        //        }
        //        if (Subjective != "" || InSituation != "" || Diagnose != "")
        //        {

        //            Subjective = GernerateXml("Subjective", Subjective);

        //            InSituation = GernerateXml("InSituation", InSituation);

        //            Diagnose = GernerateXml("Diagnose", Diagnose);

        //            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
        //            {

        //                string strRegistryID = GetRegistryID();

        //                es.Save(strRegistryID, InSituation, Subjective, null, Diagnose);
        //            }
        //        }
        //    }
        //    if (ActiveDocumentManager.getDefaultAD().Bookmarks.Count != 0)
        //    {

        //        try
        //        {
        //            object b1 = "Subjective1";
        //            object b2 = "Subjective2";

        //            Word.Bookmark book1 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b1);
        //            Word.Bookmark book2 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b2);
        //            object start = book1.Range.Start;
        //            object end = book2.Range.End;
        //            Word.Range range = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
        //            Subjective = range.Text.Trim();
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.logAdapter.Record("EX961952973", ex.Message + ">>" + ex.ToString(), true); 
           
        //        }
        //        try
        //        {
        //            object b1 = "Diagnose1";
        //            object b2 = "Diagnose2";

        //            Word.Bookmark book1 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b1);
        //            Word.Bookmark book2 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b2);
        //            object start = book1.Range.Start;
        //            object end = book2.Range.End;
        //            Word.Range range = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
        //            Diagnose = range.Text.Trim();
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.logAdapter.Record("EX961952976", ex.Message + ">>" + ex.ToString(), true); 
           
        //        }
        //        try
        //        {
        //            object b1 = "InSituation1";
        //            object b2 = "InSituation2";

        //            Word.Bookmark book1 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b1);
        //            Word.Bookmark book2 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b2);
        //            object start = book1.Range.Start;
        //            object end = book2.Range.End;
        //            Word.Range range = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
        //            InSituation = range.Text.Trim();
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.logAdapter.Record("EX961952978", ex.Message + ">>" + ex.ToString(), true); 
           
        //        }
        //        try
        //        {
        //            object b1 = "Exam1";
        //            object b2 = "Exam2";

        //            Word.Bookmark book1 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b1);
        //            Word.Bookmark book2 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b2);

        //            object start = book1.Range.Start;
        //            object end = book2.Range.End;
        //            Word.Range range = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
        //            Clipboard.Clear();
        //            range.Select();
        //            range.Copy();
        //           // CloseDocumentNoSave();

        //            object oMissing = System.Reflection.Missing.Value;

        //            object start1 = 0;
        //            object end1 = 0;

        //           // NewWordDoc();
        //            Microsoft.Office.Interop.Word.Range tableLocation = ActiveDocumentManager.getDefaultAD().Range(ref start1, ref end1);
        //            tableLocation.Select();
        //            tableLocation.Paste();

        //            XmlDocument xmldoc = new XmlDocument();
        //            xmldoc.LoadXml(@"<Exam/>");
        //            //GenerateXmlContent(xmldoc.DocumentElement, "Exam");
                   
        //            string a = "";
        //            string b = "";
        //            string writerID = GetWriterInf(ref  a, ref  b);
        //            if (!writerID.Equals(Globals.DoctorID))
        //            {
        //                foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
        //                {
        //                    if (cc.Title == "Writer")
        //                    {
        //                        cc.LockContentControl = false;
        //                        cc.LockContents = false;
        //                        cc.Range.Text = Globals.DoctorID + " / " + cc.Range.Text;
        //                        break;
        //                    }
        //                }
        //            }
        //            object filename = Path.Combine(Globals.workFolder + @"\Link\" + GetRegistryID() + @"\", "Exam" + ".docx");
        //            ActiveDocumentManager.getDefaultAD().SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //                ref oMissing, ref oMissing, ref oMissing);
        //            //udt.jj.SaveWordDoc(ActiveDocumentManager.getDefaultAD(), filename.ToString(), true);

        //            //CloseDocumentNoSave();

        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.logAdapter.Record("EX961052978", ex.Message + ">>" + ex.ToString(), true); 
           
        //        }

        //        if (Subjective != "" || InSituation != "" || Diagnose != "")
        //        {

        //            //Subjective = GernerateXml("Subjective", Subjective);

        //            //InSituation = GernerateXml("InSituation", InSituation);

        //            //Diagnose = GernerateXml("Diagnose", Diagnose);

        //            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
        //            {

        //                string strRegistryID = GetRegistryID();

        //                es.Save(strRegistryID, InSituation, Subjective, null, Diagnose);
        //            }
        //        }
        //    }
            
        //    //CloseDocumentNoSave();

        //    //NoteInEditing(); // invalidate controls for ribbon
        //}
        public string GetPatientSex()
        {
            if (tvPatients.SelectedNode == null) return null;

            switch (tvPatients.SelectedNode.Level)
            {
                case 1:
                    return tvPatients.SelectedNode.Parent.Name;
                case 2:
                    return tvPatients.SelectedNode.Parent.Parent.Name;
                case 3:
                    return tvPatients.SelectedNode.Parent.Parent.Parent.Name;
            }
            return null;
        }

        public void GetPoint()
        {
            try
            {
                int cjjleft, cjjtop, cjjwidth, cjjheight;
                Word.Paragraph para = ActiveDocumentManager.getDefaultAD().Paragraphs.Last;
                Word.Range range = wordApp.Selection.Range;
                range.Start = range.End - 1;
                range.Select();
                wordApp.ActiveWindow.GetPoint(out cjjleft, out cjjtop, out cjjwidth, out cjjheight, range);
                MessageBox.Show(cjjtop.ToString());
                wordApp.ActiveWindow.ActivePane.View.SeekView = Word.WdSeekView.wdSeekPrimaryHeader;
                wordApp.Selection.WholeStory();
                wordApp.Selection.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                wordApp.Selection.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX961052978", ex.Message + ">>" + ex.ToString(), true); 
           
                throw;
            }

        }

        public string GetNoteIDFromWordWindow(ref string childID)
        {
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                if (udt.jj.IsNumaric(cc.Title))
                {
                    childID = cc.Tag;
                    return cc.Title;
                }
            }
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.ContentControls)
            {
                if (udt.jj.IsNumaric(cc.Title))
                {
                    childID = cc.Tag;
                    return cc.Title;
                }
            }
            return null;
        }
        private void SaveTemplate(string templateType)
        {
            ThisAddIn.ResetBeginTime();
            string doctorID="";
            string general="";

            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.DeleteNote)) return;
            if (NoteTemplate == null)
            {
                OpenTemplates();         
            } 
          
             if (templateType == StringGeneral.HospitalTemplate)
             {
                 //NoteTemplate.LoadHospitalTemplate();
                 doctorID = StringGeneral.AllCode;
                 general = StringGeneral.AllCode;

             }
             else if (templateType == StringGeneral.PersonTemplate)
             {
                 //NoteTemplate.LoadPersonTemplate();
                 doctorID = Globals.DoctorID;
                 general = StringGeneral.NullCode;
             }
             else if (templateType == StringGeneral.DepartTemplate)
             {
                // NoteTemplate.LoadDepartmentTemplate();
                 doctorID = StringGeneral.NullCode;
                 general = Globals.OpDepartID;
             }
             Cursor.Current = Cursors.Default;
            if (currentNote.AsTemplateOld(doctorID, general))
            {
                OpDone opDone = new OpDone("保存成功");
                opDone.Show();
            }
            TemplateOperationEnable(true);
        }
        public void TemplateOperationEnable(bool status)
        {
            if (status)
            {
                noteSave.Visible = false;
                RefreshInfo.Visible = false;
                noteCommit.Visible = false;
                noteRef.Visible = false;
                // noteValuate.Visible = false;
                //referBlock.Visible = !Globals.offline;
                Icd10.Visible = false;
               // signature.Visible = false;
                referOrder.Visible = false;
                referPhrase.Visible = !Globals.offline;
                //btnCongener.Enabled = false;
                asPersonTemplate.Visible = false;
                if (ThisAddIn.CanOption(ElementNames.TemplateRight))
                {
                    TitleLevel tl = Globals.doctors.GetTitleLevel(Globals.DoctorID);
                    if ((tl == TitleLevel.ChiefDoctor) ||
                        (tl == TitleLevel.ViceChiefDoctor) || (tl == TitleLevel.AttendingDoctor))
                    {
                        asDepartTemplate.Enabled = true;
                        asHospitalTemplate.Enabled = true;
                    }
                    else
                    {
                        asDepartTemplate.Enabled = false;
                        asHospitalTemplate.Enabled = false;
                    }
                }
                else
                {
                    if (ThisAddIn.CanExeutiveCommandQuiet(MyMenuItems.AsDepartTemplates))
                        asDepartTemplate.Enabled = true;
                    else asDepartTemplate.Enabled = false;
                    if (ThisAddIn.CanExeutiveCommandQuiet(MyMenuItems.AsHospitalTemplates))
                        asHospitalTemplate.Enabled = true;
                    else asHospitalTemplate.Enabled = false;
                }
                saveTemplate.Enabled = !Globals.offline;
            }
        }
        public void OnlineCheck()
        {
            if (!Globals.offline) return;

            string serverName = ThisAddIn.CheckOffline();
            if (Globals.offline)
            {
                MessageBox.Show(serverName + ErrorMessage.OfflineAgain, ErrorMessage.Warning);
            }
            else
            {
                int pos = wordApp.Caption.Length - EmrConstant.StringGeneral.offlineTitle.Length;
                string offlineTitle = wordApp.Caption.Substring(pos);
                if (offlineTitle == EmrConstant.StringGeneral.offlineTitle)
                    wordApp.Caption = wordApp.Caption.Substring(0, pos);
                //ribbon.ribbon.Invalidate();
                MessageBox.Show(EmrConstant.ErrorMessage.OnlineAgain, EmrConstant.ErrorMessage.Warning);
            }

        }
        public string NewWordDoc()
        {
            
            axWbDoCView.Dock = DockStyle.None;
            axWbDoCView.Size = new Size(1, 1);
            bar1.Visible = true;
            ReportClose.Visible = true;
            PrintReport.Visible = true;
            try
            {
                if (File.Exists(Globals.NullDoc))
                {
                    ThisAddIn.killWordProcess();
                    File.Delete(Globals.NullDoc);
                }
            FileStream noteStream = File.Create(Globals.NullDoc);
            noteStream.Close();
            object oMissing = System.Reflection.Missing.Value;
            string filename = Globals.NullDoc;

                          
                plMain.Visible = true;
                axWbDoCView.Navigate(filename, ref  oMissing, ref   oMissing, ref   oMissing, ref   oMissing);
               
                return "success";
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX961052979", ex.Message + ">>" + ex.ToString(), true); 
           
                wordApp = null;
                ThisAddIn.killWordProcess();
                //MessageBox.Show("文件已经损坏！" + ex.Message + Delimiters.Colon + ex.Source);
                return null;
            }
        }
       void  _newBlock()
        {
            object oMissing = System.Reflection.Missing.Value;

            Word.Range r = ActiveDocumentManager.getDefaultAD().Range(0, ActiveDocumentManager.getDefaultAD().Sections[1].Range.End);
            r.Delete(ref oMissing, ref oMissing);
            _Enble();
        }
        public string NewEmrBlock()
        {
            if (Globals.offline)
            {
                  return null;
            }
            if (!ThisAddIn.CanExeutiveCommand(RibbonID.NewBlock)) return null;
            if (wordApp!=null) return null;            
            string meg = NewWordDoc();
            if (meg == null) return null;
            Globals.BnewBlock = true;
            return "success";
        }
        private void SaveNewEmrBlock()
        {
            BlockName blockNames = new BlockName(GetBlockList());
            blockNames.StartPosition = FormStartPosition.CenterScreen;
            if (blockNames.ShowDialog() == DialogResult.OK)
            {
                #region Convert the word window into XmlNode.
                string category = blockNames.GetCategory();
                string blockName = blockNames.GetBlockName();
                if (category.Trim() == "" || blockName.Trim() == "")
                {
                    MessageBox.Show("名字不能为空请改正！", "提示：");
                    return;
                }
                blockNames.Dispose();
                XmlDocument doc = new XmlDocument();
                XmlElement block = doc.CreateElement(ElementNames.Block);
                block.SetAttribute(AttributeNames.Category, category);
                block.SetAttribute(AttributeNames.Name, blockName);
                block.InnerText = WordWindowToString();
                #endregion

                int pk = ThisAddIn.PutEmrBlock(Globals.OpDepartID, block);
                #region Get pk of the new block.
                for (int k = pk; k > 0; k--)
                {
                    XmlNode newBlock = ThisAddIn.GetEmrBlock(k);
                    if (newBlock == null) continue;
                    if (newBlock.Attributes[EmrConstant.AttributeNames.Category].Value ==
                        block.Attributes[EmrConstant.AttributeNames.Category].Value &&
                        newBlock.Attributes[EmrConstant.AttributeNames.Name].Value ==
                        block.Attributes[EmrConstant.AttributeNames.Name].Value) { pk = k; break; }
                }
                #endregion

                #region Cteate local storage for this block.
                string docName = udt.MakeBlockDocumentPath(pk, blockName, category, Globals.blockFolder);
                udt.jj.ExportWordDoc(ActiveDocumentManager.getDefaultAD(), docName);                
                #endregion
                Globals.currentBlockPk = pk;
                Globals.currentBlockName = docName;
                //CloseDocumentNoSave();
                //EnableOperationBlock();
                //AcctiveTabBlock();
            }

        }
        public string WordWindowToString()
        {
            //string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Block);
            string tmpfile = Globals.NullDoc;
            udt.jj.SaveBlock(ActiveDocumentManager.getDefaultAD(), tmpfile, false);
            return udt.jj.WordDocumentToString(tmpfile);
        }
        private DataTable GetBlockList()
        {
            DataTable dt = ThisAddIn.GetEmrBlock(Globals.OpDepartID);
            return dt;
        }
        public bool ValidBlockPk(int pk, out string catagory, out string name)
        {
            DataTable dt = GetBlockList();
            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                if (Convert.ToInt32(dt.Rows[i]["PK"]) == pk)
                {
                    catagory = dt.Rows[i]["Catagory"].ToString();
                    name = dt.Rows[i]["Block"].ToString();
                    return true;
                }               
            }
            catagory = "";
            name = "";
            return false;
        }
        public void SaveEmrBlock()
        {
            if (wordApp.Documents.Count == 0) return;

          if (Globals.currentBlockIsNew)
            {
                SaveNewEmrBlock();
                return;
            }
            string category, name;
            if (!ValidBlockPk(Globals.currentBlockPk, out category, out name)) return;

            #region Set title of content controls to 'Control'
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                cc.Title = StringGeneral.Control;
            }
            #endregion

            /* Update the block in database. */
            XmlDocument doc = new XmlDocument();
            XmlElement block = doc.CreateElement(EmrConstant.ElementNames.Block);
            block.InnerText = WordWindowToString();
            block.SetAttribute(AttributeNames.Category, category);
            block.SetAttribute(AttributeNames.Name, name);
            if (ThisAddIn.UpdateEmrBlock(Globals.currentBlockPk, block) == Return.Successful)
            {
                /* Update the block in local. */
                udt.jj.ExportWordDoc(ActiveDocumentManager.getDefaultAD(), Globals.currentBlockName);
                CloseDocumentNoSave();
                //EnableOperationBlock();
            }
        }
        public void CloseEmrBlock()
        {
            if(wordApp==null)
            return;
            if (!ActiveDocumentManager.getDefaultAD().Saved)
            {
                if (MessageBox.Show(ErrorMessage.NotSaved, ErrorMessage.Warning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    == DialogResult.Yes) SaveEmrBlock();
            }
            Globals.currentBlockPk = 0;
            Globals.currentBlockName = "";
            CloseDocumentNoSave();
        }
        private void CloseDocumentNoSave()
        {
            ActiveDocumentManager.getDefaultAD().Save();
            axWbDoCView.Navigate("about:blank");
            wordApp = null;
        }
        private void BlockEnable(string status)
        { 
            switch (status)
            {
                case BlockStatus.Bnone:
                    btnCloseBlock.Visible = false;
                    btnSaveBlock.Visible = false;
                    btnNewBlock.Visible = true;                  
                    break;
                case BlockStatus.Bnew:
                    _Enble();
                    btnSaveBlock.Visible = true;
                    btnCloseBlock.Visible = true;
                    //btnBlockWin.Enabled = false;
                    break;
                case BlockStatus.Blist:
                    _Enble();
                    btnSaveBlock.Visible = true;
                    btnCloseBlock.Visible = true;
                    btnNewBlock.Visible = false;
                    break;             

                default:
                    break;
            }
            
        }
        private void InsertPic(string templateFile, object linkToFile, object saveWithDocument)
        {
            Word.Range range = wordApp.Selection.Range;
            object orange = (object)range;
            Object pic = "System.Windows.Forms.CheckBox.1";
            //object oMissing = Type.Missing;
            try
            {

                range.InlineShapes.AddPicture(templateFile, ref linkToFile, ref saveWithDocument, ref orange);
                //range = (Word.Range)orange;
                //range.Select();
                ////range.Cut();
                //range.ContentControls.Add(Word.WdContentControlType.wdContentControlPicture,ref orange);               
                //range.Select();
               // range.Paste();
                //ccHeader.Range.Text = "templateFile";
           
                  }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX961152979", ex.Message + ">>" + ex.ToString(), true);            
            }

            File.Delete(templateFile);
        }
        private void InsertPicOle(object file, object linkToFile)
        {
            Word.Range range = wordApp.Selection.Range;
            object orange = (object)range;
            object oMissing = System.Reflection.Missing.Value;
            object oClassType = "Paint.Picture";
            Object refmissing = System.Reflection.Missing.Value;
            try
            {
                 range.InlineShapes.AddOLEObject(ref oClassType, ref file, ref linkToFile, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref orange);
                 //axWbDoCView.ExecWB(SHDocVw.OLECMDID.OLECMDID_HIDETOOLBARS, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref refmissing, ref refmissing);
               
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX971152979", ex.Message + ">>" + ex.ToString(), true); 
           
            }
            // File.Delete(file);
        }
        private TreeNode GetPatientNode()
        {
            TreeNode selected = tvPatients.SelectedNode;
            if (selected == null) return null;
            switch (selected.Level)
            {
                case 0:
                    return selected;
                case 1:
                    return selected.Parent;
                case 2:
                    return selected.Parent.Parent;
                case 3:
                    return selected.Parent.Parent.Parent;
            }
            return null;
        }
        public string GetWordDocNames(TreeNode node)
        {
            string wordDocName = "";
            int index = Convert.ToInt32(node.Parent.Parent.Tag);
            EmrDocument emrDoc = emrDocuments.GetEmrDocument(index);
            wordDocName = emrDoc.GetDecodedWordDocForOneEmrNote(Convert.ToInt32(node.Tag));
            WordToHTML wordToHTML = new WordToHTML();
            string htmlName = wordToHTML.convert(wordDocName);
            return htmlName;
            return wordDocName;
        }
        private object WordToHtml(string wordDocName)
        {
            object savefilename = null;
            try
            {
                Microsoft.Office.Interop.Word.ApplicationClass appclass = new Microsoft.Office.Interop.Word.ApplicationClass();//实例化一个Word 
                Type wordtype = appclass.GetType();
                Microsoft.Office.Interop.Word.Documents docs = appclass.Documents;//获取Document 
                Type docstype = docs.GetType();
                object filename = wordDocName;//Word文件的路径
                Microsoft.Office.Interop.Word.Document doc = (Microsoft.Office.Interop.Word.Document)docstype.InvokeMember("Open",

                System.Reflection.BindingFlags.InvokeMethod, null, docs, new object[] { filename, true, true });//打开文件 
                Type doctype = doc.GetType();
                savefilename = Application.StartupPath + @"\NoteRef.html";//生成HTML的路径和名子 
                doctype.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, doc, new object[] { savefilename, 

            Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML });//另存为Html格式 
                //wordtype.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, appclass, null);//退出 
                //wordtype
                //doc
                object saveOption = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                object oMissing = System.Reflection.Missing.Value;
                Word.Application app = docs.Application;
                docs.Close(ref saveOption, ref oMissing, ref oMissing);
                appclass.Quit(ref saveOption, ref oMissing, ref oMissing); //注销试验一下
                //oWord.Quit(ref saveOption, ref oMissing, ref oMissing);
                //docs.Close(
            }
            catch
            {


            }
            return savefilename;
        }

        public void LoadDeparts()
        {
             gjtEmrPatients.WorkMode mode = gjtEmrPatients.WorkMode.InHospital;
            XmlNode depart = ThisAddIn.GetDepartmentsByMode(mode);
            if (depart == null) return;
            XmlNodeList departments = depart.SelectNodes(EmrConstant.ElementNames.Department);
            cbxArea.Controls.Clear();
            cbxArea.Items.Clear();
            cbxArea.Items.Add("--请选择科室--");
            foreach (XmlNode department in departments)
            {               
                string text = department.Attributes[EmrConstant.AttributeNames.Code].Value + " "
                    + department.Attributes[EmrConstant.AttributeNames.Name].Value;
                cbxArea.Items.Add(text);
            }
            
            if (cbxArea.Items.Count > 2)
            {
                if (Globals.selectIndex != 0)
                    cbxArea.SelectedIndex = Globals.selectIndex;
                else
                    cbxArea.SelectedIndex = 1;
            }
            
            tvPatients.Nodes.Clear();
            tabControlPanel3.Controls.Clear();
            panel1.Controls.Add(tvPatients);
            tvPatients.Location = new Point(0, 34);
            tvPatients.Size = new Size(200, 590);
            tvPatients.Dock = DockStyle.Fill;                   
            
        }
        private void LoadTvPatient()
        {
            panel1.Controls.Remove(tvPatients);
            tabControlPanel3.Controls.Clear();
            tabControlPanel3.Controls.Add(tvPatients);            
            tvPatients.Dock = DockStyle.Fill;
            tvPatients.Location = new Point(0,0);
            tvPatients.Size = new Size(196, 583);
        }
        private void SortNote(int n, ref  XmlNode[] emrNotes)
        {
            if (emrNotes[n - 1].Attributes[AttributeNames.NoteID].Value == emrNotes[n].Attributes[AttributeNames.NoteID].Value)
            {
                DateTime DTime1 = Convert.ToDateTime(emrNotes[n - 1].Attributes[EmrConstant.AttributeNames.WrittenDate].Value + " " + emrNotes[n - 1].Attributes[EmrConstant.AttributeNames.WrittenTime].Value);
                DateTime DTime2 = Convert.ToDateTime(emrNotes[n].Attributes[EmrConstant.AttributeNames.WrittenDate].Value + " " + emrNotes[n].Attributes[EmrConstant.AttributeNames.WrittenTime].Value);
                if (DTime1 > DTime2)
                {
                    XmlNode Emrnote = emrNotes[n];
                    emrNotes[n] = emrNotes[n - 1];
                    emrNotes[n - 1] = Emrnote;
                }
                if (n - 1 > 1) SortNote(n - 1, ref emrNotes);

            }
        }
        private void PrintInfo()
        {           
            string departmentParameter = null;           
            departmentParameter =Globals.OpDepartID;
            if (!ThisAddIn.CanExeutiveCommand(RibbonID.QCReportSelf)) return;
            
            SelectDepartment sd =
                new SelectDepartment(Globals.departmentFile, departmentParameter, ThisAddIn.GetPrintInfo, ThisAddIn.ResetBeginTime
              );
            sd.Location = new Point(650,70);
            sd.Show();
        }
        private void SetOp()
        {
           
            if (ThisAddIn.CanOption(ElementNames.ConsultationBHSY) == true)
            {
                RemindConsultation();
            }
            //麻醉科查看全院病历
            //if (Globals.Anesthesia == true && Globals.AnesthesiaText.Trim() == Globals.OpDepartID)
            //{
            //    rbAll.Enabled = true;
            //}
            //油田新建屏蔽
            if (ThisAddIn.CanOption(ElementNames.RealName) && (Globals.DoctorID != StringGeneral.supperUser)) newNote.Visible = false;
            //病案室只能看到归档的病历
            if (!ThisAddIn.CanOption(ElementNames.ArchiveManagement)) return;
            //SetArchive();
            //生成同意书
            if (ThisAddIn.CanOption(ElementNames.Consent))
                menuPatient.Items["CreateConsent"].Visible = true;
            else menuPatient.Items["CreateConsent"].Visible = false;

            if (ThisAddIn.CanOption(ElementNames.OrderByMode))
                ThisAddIn.orderByMode = gjtEmrPatients.OrderByMode.ByBed;
            else
                ThisAddIn.orderByMode = gjtEmrPatients.OrderByMode.BySexAndName;
            if (ThisAddIn.CanOption(ElementNames.Sign))
            {
                Globals.Sign = true;
            }
            else
            {
                Globals.Sign = false;
            }
            if (ThisAddIn.CanOption(ElementNames.CommitedAsEnd))
                Globals.commitedAsEnd = true;
            else Globals.commitedAsEnd = false;

            //个人设置
            Globals.expandPatientTree = Globals.myConfig.GetExpand(Globals.DoctorID);
            //Globals.showOpDoneTime = Globals.myConfig.GetShowTime(Globals.DoctorID,
                //ElementNames.ShowOpdownTime, Globals.showOpDoneTime);
            Globals.showPatientInfoTime = Globals.myConfig.GetShowTime(Globals.DoctorID,
                ElementNames.ShowPatientInfoTime, Globals.showPatientInfoTime);
            Globals.timeout = Globals.myConfig.GetShowTime(Globals.DoctorID,
                ElementNames.Timeout, Globals.timeout);
            //Globals.beCarefulSeePatient = Globals.myConfig.GetBecarefulSeePatient(Globals.DoctorID);
            //Globals.autoCheckSync = Globals.myConfig.GetAutoSyncCheck(Globals.DoctorID);
            Globals.autoCheckSync = false;
        }
        private void OpenMedical()
        {            
            string RegistryID = GetRegistryID();
            //rdlc.Medical f1 = new WordAddInEmrw.rdlc.Medical(RegistryID);
            EMR.BA.Form1 f1 = new EMR.BA.Form1(RegistryID);
            f1.Show();
        }
        private void Del()
        {
            Microsoft.Office.Interop.Word.Document WordDoc = ActiveDocumentManager.getDefaultAD();           
            UnProtectDoc();
            try
            {
                wordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekPrimaryHeader;
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX961252979", ex.Message + ">>" + ex.ToString(), true); 
           
            }
            finally
            {

                string strWordXML = WordDoc.Sections.Application.Selection.WordOpenXML;
                WordDoc.Sections.Application.Selection.WholeStory();

                ArrayList ll = new ArrayList();
                for (int i = 1; i <= wordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Shapes.Count; i++)
                {
                    object j = i;
                    IDAndRef iAr = new IDAndRef();
                    iAr.Ref = wordApp.ActiveWindow.ActivePane.Selection.HeaderFooter.Shapes.get_Item(ref j);
                    iAr.ID = iAr.Ref.ID;
                    ll.Add(iAr);
                }
                ArrayList llRemove = new ArrayList();
                bool bFind = false;
                for (int i = 0; i < ll.Count; i++)
                {
                    bFind = false;

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
                //2012-03-20 王顶堤页眉底纹删除
                int c = ActiveDocumentManager.getDefaultAD().Paragraphs[1].Borders.Count;
                wordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;

                try
                {
                    if (c != 0) 
                    {
                        Word.HeaderFooter hf = ActiveDocumentManager.getDefaultAD().Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Word.Range r = hf.Range;
                        //r.Start = r.End;
                        r.Select();
                        Word.Border border = r.Borders[Word.WdBorderType.wdBorderBottom];
                        border.Visible = false;

                    }
                }
                catch (Exception)
                {                  
                }
                ProtectDoc();
            }
        }
        private void RemindConsultation()
        {
            int sum = 0;
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                DataSet ds = pi.GetDepartmentByDoctor(Globals.DoctorID);
                if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                {
                    Globals.OpDepartID = ds.Tables[0].Rows[0]["ksbm"].ToString();
                    Globals.OpDepartName = ds.Tables[0].Rows[0]["ksmc"].ToString();
                }
                if (Globals.OpDepartID.Trim() == "")
                {
                    sum = pi.RemindConsultEx(Globals.DoctorID, Globals.DepartID);
                }
                else
                {
                    sum = pi.RemindConsultEx(Globals.DoctorID, Globals.OpDepartID);
                }
            }
            if (sum != 0 && sum != -1)
            {
                MessageBox.Show(Globals.OpDepartName + "有" + sum.ToString() + "条会诊信息");
                GetPatientsConsult();
            }
        }
        //添加分隔符 以修改页眉
        private void Split()
        {
            Word.Range rg1 = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            rg1.InsertAfter(" ");
            Word.Range rg = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
         
            //rg.Start = 0;
            //Word.Range lastRange = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            //lastRange.Start = 0;
            ////deleteNewLine(lastRange, 1);
            Word.Pane paneTemp = wordApp.ActiveWindow.Panes[1];
            int iPageCount = paneTemp.Pages.Count;
            //for (int i = iPageCount; i > 0; i--)
            //{
            //    rg.Start = paneTemp.Pages[i].Breaks[1].Range.Start;
            //    if (rg.Start > 0)
            //    {
            //        rg.Start = rg.Start - 1;
            //        rg.InsertBreak(Word.WdSectionStart.wdSectionNewPage);
            //    }
            //}
            rg.InsertBreak(Word.WdSectionStart.wdSectionNewPage);

            for (int i = 1; i <= ActiveDocumentManager.getDefaultAD().Sections.Count; i++)
            {
                ActiveDocumentManager.getDefaultAD().Sections[i].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;
            }
        }
        private void SetFooterPage(int count,bool split)
        {
            try
            {
                int num = 1;
                if (split)
                {
                    num = 2;
                    count++;
                    Word.Pane paneTemp = wordApp.ActiveWindow.Panes[1];
                    int iPageCount = paneTemp.Pages.Count;
                    int scount = ActiveDocumentManager.getDefaultAD().Sections.Count;
                    // for (int i = 2; i <= scount; i++)
                    //{
                    // count++;

                    //  }
                }

                Word.HeaderFooter hf = ActiveDocumentManager.getDefaultAD().Sections[num].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                hf.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                hf.Range.Text = "第  页";

                hf.PageNumbers.NumberStyle = Word.WdPageNumberStyle.wdPageNumberStyleArabic;
                hf.PageNumbers.RestartNumberingAtSection = true;
                hf.PageNumbers.StartingNumber = count;
                object alignment = Word.WdPageNumberAlignment.wdAlignPageNumberCenter;
                object oFirstPage = true;
                Word.PageNumber pn = hf.PageNumbers.Add(ref alignment, ref oFirstPage);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }
        public void GetPatientsConsult()
        { 
            Cursor.Current = Cursors.WaitCursor;
            if (Globals.inStyle && !Globals.offline)
            {
                XmlNode patients = ThisAddIn.GetPatients(
                                   gjtEmrPatients.PatientGettingMode.Consult,
                                   Globals.DoctorID);
                if (patients != null)
                {
                    string patientFile = Globals.patientFile;
                    ThisAddIn.xmlPatientWriter(Globals.DoctorID, tvPatients, 6, Globals.patientFile);
                }
            }
            Cursor.Current = Cursors.Default;
        }
        //不能寄存方法 2012-09-06
        public bool CheckSave()
        {
            string name = ActiveDocumentManager.getDefaultAD().Name;
            string path = ActiveDocumentManager.getDefaultAD().Path;
            bool check = false;
            if (path != "")
            {
                if (Globals.docPath == "")
                {
                    if (Globals.currentDirectory != path)
                    {
                        MessageBox.Show("①.先关闭当前其他文档\n\r②.然后再双击当前病历黄色可编辑区域\n\r③.最后再寄存或提交！", "温馨提示");
                    }
                    else
                    {
                        check = true;
                    }
                }
                else
                {
                    if (path != Globals.currentDirectory && Globals.docPath != path)
                    {
                        MessageBox.Show("①.先关闭当前其他文档\n\r②.然后再双击当前病历黄色可编辑区域\n\r③.最后再寄存或提交！", "温馨提示");
                    }
                    else
                    {
                        check = true;
                    }
                }
            }
            else
            {
                check = true;
            }
            return check;
        }
        public bool IsCommitedSelectedNote()
        {
            int index = GetDocmentIndex();
            bool flag = false;
            int series = -1;   
            if (this.mns != null)
            {
                foreach (TreeNode noteNode in this.mns.GetNotes().Nodes)
                {
                    if (!noteNode.Checked) continue;

                    series = Convert.ToInt32(noteNode.Tag);

                    flag = emrDocuments.GetEmrDocument(index).IsCommited(series);

                    if (!flag)
                    {
                        break;
                    }
                }
            }
            else
            {
                series = GetSeriesOfNote();

                flag = emrDocuments.GetEmrDocument(index).IsCommited(series);
            }
            return flag;
        }
        public int GetSeriesOfNote()
        {
            if (tvPatients.SelectedNode == null) return -1;
            if (tvPatients.SelectedNode.Tag.ToString() == "No") return 0;
            return Convert.ToInt32(tvPatients.SelectedNode.Tag);
        }
        private TreeNode m_LastMouseOverNode = null;
        private void tvPatients_MouseMove(object sender, MouseEventArgs e)
        {
        //    TreeNode nodeAt = tvPatients.GetNodeAt(e.X, e.Y);
        //    if (nodeAt == null || nodeAt.Level != 0)
        //    {
        //        m_LastMouseOverNode = null;
        //        timer1.Stop();
        //        HidePatientInfoForm();
        //    }
        //    else
        //    {

        //        if (nodeAt != m_LastMouseOverNode)
        //        {
        //            if (patientInfoForm != null) patientInfoForm.Dispose();
        //            if (nodeAt != null)
        //                m_LastMouseOverNode = nodeAt;
        //        }
        //        else return;
        //        EmrConstant.PatientInfo pi = GetPatientInfo(m_LastMouseOverNode);

        //        if (!ThisAddIn.CanOption(ElementNames.NoPatientInfo))
        //        {
        //            string itemText = Globals.doctors.GetDoctorName(nodeAt.Nodes[0].Text.Split(':')[2]) + EmrConstant.Delimiters.Seperator;
        //            if (nodeAt.Nodes[0].Text.Split(':').Length < 5 || nodeAt.Nodes[0].Text.Split(':')[4] != "已归档") itemText += "未归档" + EmrConstant.Delimiters.Seperator;
        //            else itemText += "已归档" + EmrConstant.Delimiters.Seperator;
        //            string ifRef = "未接收";
        //            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
        //            {
        //                DataSet dst = es.GetTransferInfoExEx(GetRegistryID(m_LastMouseOverNode), StringGeneral.NullCode, true);
        //                if (dst.Tables[0].Rows.Count > 0) ifRef = "已接收";
        //            }
        //            itemText += ifRef + EmrConstant.Delimiters.Seperator;
        //            patientInfoForm = new PatientInfoForm(pi, itemText, Globals.showPatientInfoTime, ThisAddIn.Today());
        //            patientInfoForm.Show();
        //            patientInfoForm.Location = MousePosition;
        //            timer1.Interval = 500;
        //            timer1.Start();
        //        }
        //    }
        }
        public string GetRegistryID(TreeNode selectedNode)
        {

                if (selectedNode == null) return null;
            string registryIDText = null;
            switch (selectedNode.Level)
            {
                case 0:
                    registryIDText = selectedNode.FirstNode.Text;
                    break;
                case 1:
                    registryIDText = selectedNode.Text;
                    break;
                case 2:
                    registryIDText = selectedNode.Parent.Text;
                    break;
                case 3:
                    registryIDText = selectedNode.Parent.Parent.Text;
                    break;
            }
            string registryID, chargeDoctorID;
            ParserRegistry(registryIDText, out registryID, out chargeDoctorID);
            return registryID;
        }
        private void HidePatientInfoForm()
        {
            if (patientInfoForm != null)
                patientInfoForm.Visible = false;
        }
        private string GernerateXml(string strLabel, string strText)
        {
            XmlDocument oDocument = new XmlDocument();
            XmlElement oElem = oDocument.CreateElement(strLabel);
            oElem.InnerText = strText;
            return oElem.OuterXml.ToString();
        }
        XmlElement cemrNote = null;
        private void CreateConsent_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (tvPatients.SelectedNode == null) return;
            if (tvPatients.SelectedNode.Level != 3) return;
            if (IsLocked()) return;
            int index = Convert.ToInt32(tvPatients.SelectedNode.Parent.Parent.Tag);

            #region Check permission
            /* Do you have access privilege ? */
            PermissionLevel pl = emrDocuments.GetPermission(index);
            if (pl == PermissionLevel.ReadOnly)
            {
                MessageBox.Show(ErrorMessage.NoPrivilegeNew, ErrorMessage.Warning);
                return;
            }
            if (Globals.auditSystem.Substring(0, 1) == EmrConstant.Auditing.LevelB)
            {
                if (pl != EmrConstant.PermissionLevel.ReadWrite && pl != EmrConstant.PermissionLevel.Trust)
                {
                    MessageBox.Show(EmrConstant.ErrorMessage.NoPrivilegeNew, EmrConstant.ErrorMessage.Warning);
                    return;
                }
            }         
            #endregion
            #region Show source note on word window
            int series = Convert.ToInt32(tvPatients.SelectedNode.Tag);
            cemrNote = GetEmrNote(index, series);
            string registryID = cemrNote.OwnerDocument.DocumentElement.Attributes[AttributeNames.RegistryID].Value;
            Globals.RegistryID = registryID;
            string noteDoc = udt.MakeWdDocumentFileName(registryID,
                tvPatients.SelectedNode.Parent.Name, series,Globals.workFolder);
            OpenWordDoc(noteDoc);
            Globals.CreateConsent = true;
                     
        }
        private void _CreateCon()
        {
            if (ActiveDocumentManager.getDefaultAD().Bookmarks.Count > 1)
            {
                try
                {
                    object b1 = "同意书1";
                    object b2 = "同意书2";
                    Word.Bookmark book1 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b1);
                    Word.Bookmark book2 = ActiveDocumentManager.getDefaultAD().Bookmarks.get_Item(ref b2);
                    UnProtectDoc();
                    object start = book1.Range.Start;
                    object end = book2.Range.End;
                    Word.Range range = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
                    range.Text = "我已认真阅读了上述患方提供的患者病史及个人信息，确认无误。\n                        与患者关系：                确认签字：";
                }
                catch(Exception ex)
                {
                    Globals.logAdapter.Record("EX961352979", ex.Message + ">>" + ex.ToString(), true); 
           
                    CloseDocumentNoSave();
                    MessageBox.Show("不能形成告知书！", "警告：");
                    return;

                }

            }
            else
            {
                CloseDocumentNoSave();
                MessageBox.Show("不能形成告知书！", "警告：");
                return;
            }
            string noteID = ThisAddIn.CanOptionText(ElementNames.ConsentID);

            XmlNode[] emrNotes = Globals.childPattern.GetChildNote(noteID);
            if (emrNotes[0] == null)
            {

                CloseDocumentNoSave();
                MessageBox.Show("请先维护子样式！", "警告：");
                return;

            }
            ConsentForm cf = new ConsentForm(emrNotes);
            if (cf.ShowDialog() != DialogResult.OK)
            {
                CloseDocumentNoSave();
                return;
            }
            string childID = cf.GetID();
            string noteName = Globals.childPattern.GetNoteNameFromNoteID(childID);
            cf.Close();
            bool IK = false;
            foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
            {
                if (cc.Title.Length < 3 && udt.jj.IsNumaric(cc.Title))
                {
                    cc.Title = noteID;
                    cc.Tag = childID;
                    cc.Range.Text = noteName;
                    IK = true;
                    break;
                }
            }
            if (!IK)
            {
                foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
                {
                    if (xn.BaseName == "病历名称")
                    {
                        xn.Range.Text = noteName;
                        break;
                    }
                }
            }
            ActiveDocumentManager.getDefaultAD().Revisions.AcceptAll();
            //InitialFocus();
            #endregion
            #region Construct attributes
            AuthorInfo authorInfo;
            authorInfo.NoteID = noteID;
            authorInfo.ChildID = childID;
            authorInfo.NoteName =
            authorInfo.Writer = Globals.DoctorName;
            authorInfo.WrittenDate = ThisAddIn.Today().ToString(StringGeneral.DateFormat);
            authorInfo.Checker = "";
            authorInfo.CheckedDate = "";
            authorInfo.FinalChecker = "";
            authorInfo.FinalCheckedDate = "";
            authorInfo.TemplateType = StringGeneral.NoneTemplate;
            authorInfo.TemplateName = "";
            authorInfo.WriterLable = cemrNote.GetAttribute(AttributeNames.Sign3);
            authorInfo.CheckerLable = cemrNote.GetAttribute(AttributeNames.Sign2);
            authorInfo.FinalCheckerLable = cemrNote.GetAttribute(AttributeNames.Sign1);
           // PatientInfo patientInfo = GetPatientInfo(tvPatients.SelectedNode);
            currentNote = new EmrNote(authorInfo, cemrNote, NoteEditMode.Writing, Globals.RegistryID, this);
            currentNote.noteInfo.SetNewNoteFlag(true);

            #endregion

            tvPatients.SelectedNode = tvPatients.SelectedNode.Parent.Parent;
            TvPatientsEnable(false);  
        }
        private void filingsetup_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            TitleLevel tl = Globals.doctors.GetTitleLevel(Globals.DoctorID);
            if ((int)tl > (int)TitleLevel.AttendingDoctor || tl == TitleLevel.Nothing)
            {
                MessageBox.Show("权限不足！", "提示");
                return;
            }
            FilingSetup();
            ThisAddIn.ResetBeginTime();
        }
        public void FilingSetup()
        {
            string RegistryID = GetRegistryID();
            string operatorID = Globals.DoctorID;
            FilingSetup FS = new FilingSetup(RegistryID);
            //FS.Location = FitLocation(FS.Size);
            FS.ShowDialog();
        }
        private void Archieve_Click(object sender, EventArgs e)
        {
            //if (!Globals.ThisAddIn.logon.CanExeutiveCommand(MyMenuItems.NoteValuate)) return;
            if (tvPatients.SelectedNode.Level != 1) return;

            TitleLevel tl = Globals.doctors.GetTitleLevel(Globals.DoctorID);
            if ((int)tl > (int)TitleLevel.AttendingDoctor || tl == TitleLevel.Nothing)
            {
                MessageBox.Show("权限不足！", "提示");
                return;
            }

            if (openEmr.Enabled == true) openEmr_Click(sender, e);
            string strRegistryID = GetRegistryID();
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string strStatus = es.GetEmrDocumentStatus(strRegistryID);
                if (strStatus == "1")
                {
                    MessageBox.Show("病历已经执行归档！", "提示");
                    return;
                }
            }           
            EmrDocument emrdoc = emrDocuments.GetEmrDocument(GetDocmentIndex());
            XmlNodeList emrNotes = emrdoc.GetEmrNotes();
            foreach (XmlNode emrNote in emrNotes)
            {
                if (emrNote.Attributes["sign"] == null) continue;
                if (!ThisAddIn.ExamStatus(emrNote.Attributes[AttributeNames.NoteName].Value, emrNote.Attributes[AttributeNames.NoteStatus].Value, emrNote.Attributes["sign"].Value))
                {

                    return;
                }
            }
            if (JudgeNoteFilingSetup(strRegistryID) == false) return;
            ArchiveEmr oEmr = new ArchiveEmr();
            if (oEmr.JudgeNoteStatus(strRegistryID) == 0) return;


            DateTime endDate = DateTime.Now;
            ThisAddIn.Archive_Single(endDate, strRegistryID);
            ThisAddIn.ResetBeginTime();
            
        }
        public bool JudgeNoteFilingSetup(string RegistryID)
        {
            DataSet notes = new DataSet();
            string msg = null;
            msg = GetNotesWithFiling(RegistryID, ref notes);
            if (msg != null) return false;
            if (notes.Tables[0].Rows.Count == 0)
            {
                string DepartCode = ThisAddIn.GetDepartCodeByRegistryID(RegistryID);
                msg = GetNotesWithFiling(DepartCode, ref notes);
                if (msg != null) return false;

                if (notes.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("请先维护归档设置");
                    return false;
                }
            }
            string NoteMsg = "";
            foreach (DataRow note in notes.Tables[0].Rows)
            {
                string noteID = note[0].ToString().Split(':')[0];

                string childID = "";
                if (note[0].ToString().Split(':').Length > 1)
                    childID = note[0].ToString().Split(':')[1];
                else childID = "0";

                JudgeNoteID(noteID, childID, ref NoteMsg);
            }
            if (NoteMsg == "") return true;
            else
            {
                MessageBox.Show("病历中有缺项，不能归档！\n缺少的病历包括：\n" + NoteMsg, "提示");
                return false;

            }
        }
        private string GetNotesWithFiling(string RegistryID, ref DataSet notes)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = null;
                try
                {
                    msg = es.GetNotesWithFilingSetup(RegistryID, ref notes);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);

                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX981152979", ex.Message + ">>" + ex.ToString(), true); 
           
                    return ex.Message;
                }
                return msg;
            }
        }
        private bool JudgeNoteID(string NoteID, string childID, ref string msg)
        {           
            EmrDocument emrdoc = emrDocuments.GetEmrDocument(GetDocmentIndex());
            XmlNodeList emrNotes = emrdoc.GetEmrNotes();
            string noteName;
            foreach (XmlNode emrNote in emrNotes)
            {

                if (NoteID == emrNote.Attributes[AttributeNames.NoteID].Value)
                {
                    if (NoteID == childID || childID == "0") return true;
                    if (emrNote.Attributes[AttributeNames.ChildID] != null)
                    {
                        if (childID == emrNote.Attributes[AttributeNames.ChildID].Value) return true;
                    }
                }

            }
            if (NoteID == childID || childID == "0")
                noteName = Globals.emrPattern.GetNoteNameFromNoteID(NoteID);
            else noteName = Globals.childPattern.GetNoteNameFromNoteID(childID);
            msg += noteName + "\n";
            return false;
        }
        private void SetRequiedNotes_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            DocTransfer dt = new DocTransfer(GetRegistryID(), GetPatientName());
            dt.ShowDialog();
            ThisAddIn.ResetBeginTime();
        }
        public void SetReadonly()
        {
            
                ActiveDocumentManager.getDefaultAD().ShowRevisions = false;
                ActiveDocumentManager.getDefaultAD().ShowRevisions = false;
                if (ActiveDocumentManager.getDefaultAD().ProtectionType != Word.WdProtectionType.wdNoProtection)
                    ActiveDocumentManager.getDefaultAD().Unprotect(ref Globals.passwd);
                ProtectDoc();
           
        }
        public void SetRevision2()
        {
            if (wordApp.Documents.Count == 0) return;
           
                try
                {
                    ProtectDoc();
                }
                catch(Exception ex)
                {
                    Globals.logAdapter.Record("EX9812352979", ex.Message + ">>" + ex.ToString(), true); 
           
                }
                foreach (Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
                {
                    if (xn.BaseName == "审")
                    {
                        object ob = Word.WdEditorType.wdEditorEveryone;
                        xn.Range.Editors.Add(ref ob);
                    }
                }
                ActiveDocumentManager.getDefaultAD().ShowRevisions = true;
            
            wordApp.Options.DeletedTextMark = Word.WdDeletedTextMark.wdDeletedTextMarkDoubleStrikeThrough;
        }
        private void unlockEmr_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            UnlockEmr ue = new UnlockEmr();
            //.Location = Globals.ThisAddIn.FitLocation(ue.Size);
            if (ue.ShowDialog() == DialogResult.Cancel) return;
            ThisAddIn.ResetBeginTime();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Reason />");
            doc.DocumentElement.InnerText = ue.GetReasion();

            if (ThisAddIn.UnlockEmrDocument(GetRegistryID(), GetPatientName(),
                    ue.GetExpireDays(), ue.ForPublic(), doc.DocumentElement))
            {
                OpDone opd = new OpDone("病历开封完毕！");
                opd.Show();
            }

        }
        public void _Valuate()
        {
            vs.ShowVs();
            vs.Close();
        }
        private void valuate_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            try
            {
                if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Valuate)) return;
                string RegistryID = GetRegistryID();
                bool blResult = false;
                if (RegistryID != "")
                {
                    using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                    {
                        blResult = ep.IsInHospital(RegistryID);
                    }
                    if (blResult == true)
                    {
                        Globals.isEndRule = false;
                    }
                    else
                    {
                        Globals.isEndRule = true;
                    }
                }
                vs = new ValuateScore();
               // vs.Location = Globals.ThisAddIn.FitLocation(vs.Size);

                bool self =
                    Globals.DepartID == GetPatientDepartmentCode();
                int index = Convert.ToInt32(tvPatients.SelectedNode.Tag);
                XmlDocument emrdoc = emrDocuments.Get(index);
                if (vs.Init(self, GetRegistryID(), GetChargingDoctorID(), GetPatientDepartmentCode(),
                    emrdoc.DocumentElement,this))
                {
                    if (vs.ShowDialog() == DialogResult.Cancel) return;
                }
                emrDocuments.GetEmrDocument(index).SetGrading();
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9812352977", ex.Message + ">>" + ex.ToString(), true);            
                
            }
        }

        private void btnUnPro_Click(object sender, EventArgs e)
        {
            if (wordApp != null)
                UnProtectDoc();
        }
        private void btnPrintView_Click(object sender, EventArgs e)
        {
            if (wordApp != null)
            {
                ActiveDocumentManager.getDefaultAD().ActiveWindow.View.Type = Word.WdViewType.wdPrintPreview;
               // ActiveDocumentManager.getDefaultAD().PrintPreview();
               // wordApp.PrintPreview = true;
                btnExitView.Visible = true;
                btnPrintView.Visible = false; 
            }
        }

        private void btnExitView_Click(object sender, EventArgs e)
        {          
            
            ActiveDocumentManager.getDefaultAD().ActiveWindow.View.Type = Word.WdViewType.wdPrintView;
            //ActiveDocumentManager.getDefaultAD().ClosePrintPreview();
            //wordApp.PrintPreview = false;
            btnExitView.Visible = false;
            btnPrintView.Visible = true;
        }

        private void barBack_Click(object sender, EventArgs e)
        {
            try
            {
                Word.Document wordDoc = ActiveDocumentManager.getDefaultAD();
                if (wordDoc != null)
                {
                    wordDoc.Undo();
                }
                return;
            }
            catch (Exception ex) { }
            //byte key_z = 90;
            //byte key_ctrl = 17;
            //Win32.keybd_event(key_ctrl, 0, 0, 0);
            //Win32.keybd_event(key_z, 0, 0, 0);
            //Win32.keybd_event(key_ctrl, 0, 2, 0);
            //Win32.keybd_event(key_z, 0, 2, 0);
        }
        private void barForward_Click(object sender, EventArgs e)
        {
            try
            {
                Word.Document wordDoc = ActiveDocumentManager.getDefaultAD();
                if (wordDoc != null)
                {
                    wordDoc.Redo();
                }
                return;
            }
            catch (Exception ex) { }
            
            //byte key_y = 89;
            //byte key_ctrl = 17;
            //Win32.keybd_event(key_ctrl, 0, 0, 0);
            //Win32.keybd_event(key_y, 0, 0, 0);
            //Win32.keybd_event(key_ctrl, 0, 2, 0);
            //Win32.keybd_event(key_y, 0, 2, 0);
        }
        private void btnProtect_Click(object sender, EventArgs e)
        {
            if (wordApp != null)
                ProtectDoc();

        }

        private void btnUnProtect_Click(object sender, EventArgs e)
        {
            //2012-08-24 LiuQi 新增病历解密和加密时输入密码
            Password password = new Password();
            password.ShowDialog();
            if (password.DialogResult != DialogResult.OK) return;
            string pwd = password.passwd;

            if (pwd == "jwsj")
            {
                if (wordApp != null)
                    UnProtectDoc();
            }
            else
            {
                MessageBox.Show("密码错误!请重新输入或请于医务科或病案室部分联系!");
            }
        }
        private void AddButton()
        {
            //记录操作
            lbrole.Add(noteSave);
            lbrole.Add(noteCommit);
            lbrole.Add(noteRef);
            lbrole.Add(referOrder);
            lbrole.Add(referPhrase);
            //lbrole.Add(referBlock);
            lbrole.Add(asPersonTemplate);
            lbrole.Add(asDepartTemplate);
            lbrole.Add(asHospitalTemplate);
            lbrole.Add(Icd10);
            lbrole.Add(MyPic2);
            //lbrole.Add(btnNoteValuate);
            lbrole.Add(btnPrintPic);
            lbrole.Add(noteClose);

            //病历维护
            lbrole.Add(OpenTemplate);
            lbrole.Add(noteCommit);
            lbrole.Add(MyPic);
            lbrole.Add(btnProtect);
            lbrole.Add(btnUnProtect);
            lbrole.Add(btnReadExit);

            //查询
            lbrole.Add(PatientSearch);
            lbrole.Add(PatientStayin);
            lbrole.Add(PatientGone);
            lbrole.Add(PatientsConsult);
            lbrole.Add(btnQualityQuery);
            lbrole.Add(btnQCSelfReport);
            lbrole.Add(OpenTemplate);
            lbrole.Add(btnEmrQueryScore);
            lbrole.Add(btnPrintInfo);
            lbrole.Add(btnEmrQueryScoreEnd);
            //lbrole.Add(btnSelectExit);

            //设置
            lbrole.Add(btnSOption);
            lbrole.Add(PatientStayin);
            lbrole.Add(btnArchiveSettings);
            lbrole.Add(btnOnDeputed);
            lbrole.Add(btnOnDeputing);
            lbrole.Add(btnArea);

            //输出
            lbrole.Add(btnConvert);
            lbrole.Add(btnPrint);
            lbrole.Add(btnPrintView);         
           
            //离线设置
            lbrole.Add(btnOnlineCheck);

            //构件
            lbrole.Add(btnNewBlock);
            lbrole.Add(btnSaveBlock);
            lbrole.Add(btnBlockWin);
            lbrole.Add(btnCloseBlock);

            //注销
            lbrole.Add(btnExit);
            lbrole.Add(btnExexit);

        }
        private void _NewInsertLc()
        {
            for (int i = 0; i < Globals._docname.Length; i++)
            {
                if (i == 0) continue;
               //插入文档     

                NewInsertLc(Globals._docname[i], Globals.RegistryID);
                
            }
            
        }
        private void _InsertLc()
        {
            insertLc(null, Globals.RegistryID);
        }

        //2012-04-11 重写临床路径方法

        private void NewOpenClinicPath(string registryID, string noteID)
        {

            PatientInfo patientInfo = null;
            string zyh = "";
            if (registryID.Length < 3)
            {
                patientInfo = GetPatientInfo(tvPatients.SelectedNode);
                zyh = patientInfo.RegistryID;
            }
            else
            {
                zyh = registryID;
            }
            CheckLcljUpdate(zyh);
            currentNote = new EmrNote();
            currentNote.noteInfo.SetNoteID(noteID);
            currentNote.noteInfo.SetNewNoteFlag(false);         
            TvPatientsEnable(false);
            ShowMenu(MenuStatus.Reader);
           

        }
        
        private void OpenClinicPath(string registryID, string noteID)
        {
            /**插入临床表单 zzl 20111010**/
            if (Globals.newPath)
            {
                //string zyh = "00009639"; 
                string zyh = registryID;
                Globals.RegistryID = registryID;
                string[] docname = null, jdxh = null;
                GetLc(zyh, ref jdxh, ref docname);
                if (docname != null)
                {

                    int count = docname.Length;
                    Globals._docname = docname;
                    for (int i = 0; i < docname.Length; i++)
                    {
                        if (i == 0)
                        {
                            OpenWordTem(docname[i]);
                        }
                        else
                        {
                            //插入文档     
                            Globals.NewInsertLc = true;
                           // insertLc(docname[i], zyh);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("无临床路径表单数据！"); return;
                }
                currentNote = new EmrNote();
                currentNote.noteInfo.SetNoteID(noteID);
                currentNote.noteInfo.SetNewNoteFlag(true);
            }
            else
            {
                PatientInfo patientInfo = null;
                string zyh = "";
                if (registryID.Length < 3)
                {
                    patientInfo = GetPatientInfo(tvPatients.SelectedNode);
                    zyh = patientInfo.RegistryID;
                }
                else
                {
                    zyh = registryID;
                }

                string wdDocName =
                    udt.MakeWdDocumentFileName(zyh, noteID, Convert.ToInt32(tvPatients.SelectedNode.Tag), Globals.workFolder);
                string mes = OpenWordDoc(wdDocName);
                //Globals.ThisAddIn.insertLc(null, "00009639");
                Globals.InsertLc = true;
                //insertLc(null, registryID);
                Globals.RegistryID = registryID;
                currentNote = new EmrNote();
                currentNote.noteInfo.SetNoteID(noteID);
                currentNote.noteInfo.SetNewNoteFlag(false);
            }
            //wordApp.ActiveWindow.View.Zoom.Percentage = 100;
            TvPatientsEnable(false);
            ShowMenu(MenuStatus.Reader);
            // Globals.ThisAddIn.NoteInEditing();

        }
      
        public void GetLc(string zyh, ref string[] jdxh, ref string[] docnames)
        {
            DataSet ds = null;
            byte[] bdata = null;

            using (gjtEmrService.emrServiceXml pi = new gjtEmrService.emrServiceXml())
            {
                ds = pi.DownLoadWord(zyh);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    int count = ds.Tables[0].Rows.Count;
                    jdxh = new string[count];
                    docnames = new string[count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        bdata = (byte[])ds.Tables[0].Rows[i]["doc"];
                        docnames[i] = ds.Tables[0].Rows[i]["doc_name"].ToString();
                        jdxh[i] = ds.Tables[0].Rows[i]["jdxh"].ToString();
                        string DocZipName = Path.Combine(Globals.workFolder + "\\", docnames[i] + ".rar");
                        string docPath = Path.Combine(Globals.workFolder + "\\", zyh);
                        FileInfo info = new FileInfo(DocZipName);
                        using (FileStream fs = info.Create())
                        {
                            fs.Write(bdata, 0, bdata.Length);
                        }
                        //解压 open doc   
                        string zip = "zip.dll";
                        if (!File.Exists(Path.Combine(Globals.currentDirectory + "\\", zip)))
                        {
                            MessageBox.Show("程序少zip.dll文件"); return;
                        }
                       
                        UnZipTo(DocZipName, docPath);
                        docnames[i] = Path.Combine(docPath + "\\", docnames[i]);

                        if (info.Exists) info.Delete();
                    }
                    Globals.jdxh = jdxh;
                }
            }
        }
        //检查临床路径表单更新
        public bool CheckLcljUpdate(string zyh)
        {
            using (gjtEmrService.emrServiceXml Service = new gjtEmrService.emrServiceXml())
            {
                DataSet ds = null;
                bool flag = false;
                try
                {
                    //bool result = false;
                    //result = Service.CheckUpdate(zyh);
                    //if (result)
                    ds = Service.ReDownLoadWord(zyh);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        string[] docnames = null, jdxh = null;
                        byte[] bdata = null;
                        int count = ds.Tables[0].Rows.Count;
                        jdxh = new string[count];
                        docnames = new string[count];
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            bdata = (byte[])ds.Tables[0].Rows[i]["doc"];
                            docnames[i] = ds.Tables[0].Rows[i]["doc_name"].ToString();
                            jdxh[i] = ds.Tables[0].Rows[i]["jdxh"].ToString();
                            string DocZipName = Path.Combine(Globals.workFolder + "\\", docnames[i] + ".rar");
                            string docPath = Path.Combine(Globals.workFolder + "\\", zyh);
                            FileInfo info = new FileInfo(DocZipName);
                            using (FileStream fs = info.Create())
                            {
                                fs.Write(bdata, 0, bdata.Length);
                            }


                            UnZipTo(DocZipName, docPath);
                            docnames[i] = Path.Combine(docPath + "\\", docnames[i]);

                            if (info.Exists) info.Delete();
                        }
                        Globals.jdxh = jdxh;
                        Globals._docname = docnames;
                        //解压 open doc
                        //if (!File.Exists(Globals.currentDirectory + "zip.dll"))
                        //{
                        //    XmlDocument doc = new XmlDocument();
                        //    XmlNode component = doc.CreateElement(EmrConstant.ElementNames.Componet);
                        //    Service.GetComponet("zip.dll", ref component);
                        //    Byte[] byteStream = Convert.FromBase64String(component.InnerText);
                        //    File.WriteAllBytes(Path.Combine(Globals.currentDirectory, "zip.dll"), byteStream);
                        //}
                        //合并文档
                        if (docnames != null)
                        {
                            for (int i = 0; i < docnames.Length; i++)
                            {
                                if (i == 0)
                                {
                                    OpenWordTem(docnames[i]);
                                }
                                else
                                {
                                    //插入文档
                                    Globals.NewInsertLc = true;
                                    //_NewInsertLc();
                                }
                            }
                            //Globals.ThisAddIn.ProtectDoc();
                            flag = true;
                        }

                    }
                    return flag;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9812354977", ex.Message + ">>" + ex.ToString(), true);            
              
                    return flag;
                }
            }
        }
        public void NewInsertLc(string docname, string zyh)
        {
            /**插入新的临床路径表单**/
            object oMissing = System.Reflection.Missing.Value;
            //Globals.ThisAddIn.ProtectDoc();
            Word.Range r = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            r.Start = r.End;
            r.Select();

            if (docname != null)
            {
                UnProtectDoc();
                wordApp.Selection.InsertNewPage();
                Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
                range.Start = range.End;
                range.Select();
                range.InsertFile(docname, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                //Globals.ThisAddIn.ProtectDoc();
            }
            else
            {
                Globals.jdxh = null;
            }

        }
        public void insertLc(string docname, string zyh)
        {
            /**插入新的临床路径表单**/
            string[] jdxh = null;
            object oMissing = System.Reflection.Missing.Value;
            string[] docnames = null;
            int count = 0;
            Word.Pane pane = wordApp.ActiveWindow.Panes[1];
            Word.Range r = ActiveDocumentManager.getDefaultAD().Sections.Last.Range;
            r.Start = r.End;
            r.Select();
            count = pane.Pages.Count;


            if (docname == null)
            {
                GetLc(zyh, ref jdxh, ref docnames);
                if (docnames != null)
                {
                    object psd = (object)"jwsj";
                    //r.Application.ActiveDocument.Unprotect(ref psd);
                    for (int i = 0; i < docnames.Length; i++)
                    {
                        if (i != 0)
                        {
                            count++;
                        }
                        while (pane.Pages.Count == count)
                        {
                            r.InsertAfter("\r");
                        }
                        Word.Range range = wordApp.Selection.Sections.Last.Range;
                        range.Start = range.End;
                        range.Select();
                        range.InsertFile(docnames[i], ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                    }
                    ProtectDoc();
                }
                else
                {
                    Globals.jdxh = null;
                }
            }
            else
            {
                while (pane.Pages.Count == count)
                {
                    r.InsertAfter("\r");
                }
                Word.Range range = wordApp.Selection.Sections.Last.Range;
                range.Start = range.End;
                range.Select();
                range.InsertFile(docname, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }

        }


        private void btnArea_Click(object sender, EventArgs e)
        {
            Area area = new Area();

            area.Show();
        }
        public void SetApplicationTitle(string doctorName)
        {
            wordApp.Caption = Globals.mainTitle +Globals.DepartName + ": " + doctorName;
            if (Globals.offline) wordApp.Caption += EmrConstant.StringGeneral.offlineTitle;
        }

        private void _DuplicateNote()
        {
            UnProtectDoc();
            ActiveDocumentManager.getDefaultAD().Revisions.AcceptAll();
            ResetAuthor();
            #region Construct attributes
            currentNote.SetEditModes();
            #endregion
            
        }
        //复制记录
        private void duplicateNote_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();

            //2009-8-11 L
            string namex = "";
            namex = tvPatients.SelectedNode.Text;
            if (namex.EndsWith("已审核") || namex.EndsWith("已终审"))
            {
                MessageBox.Show("此记录已审核，不能复制！");
                return;
            }

            //权限
            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.DuplicateNote)) return;
            if (tvPatients.SelectedNode.Level != 3) return;
            if (IsLocked()) return;

            int index = Convert.ToInt32(tvPatients.SelectedNode.Parent.Parent.Tag);
            if (!CheckPrecontition(tvPatients.SelectedNode.Parent.Name, index))
            {
                MessageBox.Show(ErrorMsg(tvPatients.SelectedNode.Parent.Name), ErrorMessage.Error);
                return;
            }

            #region Check permission
            /* Do you have access privilege ? */
            PermissionLevel pl = emrDocuments.GetPermission(index);
            if (pl == PermissionLevel.ReadOnly)
            {
                MessageBox.Show(ErrorMessage.NoPrivilegeNew, ErrorMessage.Warning);
                return;
            }
            if (Globals.auditSystem.Substring(0, 1) == EmrConstant.Auditing.LevelB)
            {
                if (pl != EmrConstant.PermissionLevel.ReadWrite && pl != EmrConstant.PermissionLevel.Trust)
                {
                    MessageBox.Show(EmrConstant.ErrorMessage.NoPrivilegeNew, EmrConstant.ErrorMessage.Warning);
                    return;
                }
            }

            #endregion
            #region Show source note on word window
            int series = Convert.ToInt32(tvPatients.SelectedNode.Tag);
            emrNoteDup = GetEmrNote(index, series);

            string registryID = emrNoteDup.OwnerDocument.DocumentElement.Attributes[AttributeNames.RegistryID].Value;
            string noteDoc = udt.MakeWdDocumentFileName(registryID,
                tvPatients.SelectedNode.Parent.Name, series, Globals.workFolder);
            OpenWordDoc(noteDoc);
            #endregion

            tvPatients.SelectedNode = tvPatients.SelectedNode.Parent;
            TvPatientsEnable(false);
            ShowMenu(MenuStatus.Writer);
            AuthorInfo authorInfo;
            authorInfo.NoteID = tvPatients.SelectedNode.Name;
            if (emrNoteDup.Attributes[AttributeNames.ChildID] == null) authorInfo.ChildID = "0";
            else authorInfo.ChildID = emrNoteDup.Attributes[AttributeNames.ChildID].Value;
            authorInfo.NoteName = tvPatients.SelectedNode.Parent.Text;
            authorInfo.Writer = Globals.DoctorID;
            authorInfo.WrittenDate = ThisAddIn.Today().ToString(StringGeneral.DateFormat);
            authorInfo.Checker = "";
            authorInfo.CheckedDate = "";
            authorInfo.FinalChecker = "";
            authorInfo.FinalCheckedDate = "";
            authorInfo.TemplateType = StringGeneral.NoneTemplate;
            authorInfo.TemplateName = "";
            authorInfo.WriterLable = emrNoteDup.GetAttribute(AttributeNames.Sign3);
            authorInfo.CheckerLable = emrNoteDup.GetAttribute(AttributeNames.Sign2);
            authorInfo.FinalCheckerLable = emrNoteDup.GetAttribute(AttributeNames.Sign1);
            //PatientInfo patientInfo = GetPatientInfo(tvPatients.SelectedNode);
            currentNote = new EmrNote(authorInfo, emrNoteDup, NoteEditMode.Writing, GetRegistryID(), this);
            currentNote.noteInfo.SetNewNoteFlag(true);
            Globals._DuplicateNote = true;
            //#endregion
        }
    }
}
