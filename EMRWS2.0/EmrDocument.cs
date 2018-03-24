using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EmrConstant;
using CommonLib;
using UserInterface;
using HuronControl;

namespace EMR
{
    class EmrDocument
    {
        private XmlDocument emrDoc = null;
        private string emrDocFile = null;
        private string archiveNum = null;
        

        public EmrDocument()
        {
            emrDoc = new XmlDocument();
            //emrDoc.PreserveWhitespace = true;
        }
           public void GetConsult(XmlNode notePattern, XmlElement emrNote)
        {
            string registryID = emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
            string noteID = notePattern.Attributes[AttributeNames.NoteID].Value;
            string sequence = null;
            #region Treat attribute StartTime
            switch (notePattern.Attributes[AttributeNames.StartTime].Value)
            {
                case StartTime.Consult:
                    sequence = ConsultSequence(registryID, noteID);
                    if (sequence == null) return ;
                    emrNote.SetAttribute(AttributeNames.Sequence, sequence);
                    break;
            }
            #endregion
        }

           public string GetDecodedWordDocForOneEmrNote(int series)
           {
               XmlNode emrNote = GetEmrNoteBySeries(series);
               if (emrNote == null) return null;

               string registryID = emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
               string noteID = emrNote.Attributes[AttributeNames.NoteID].Value;
               string wordDocName = udt.MakeWdDocumentFileName(registryID, noteID, series,Globals.workFolder);
               if (!File.Exists(wordDocName)) return null;

               if (Globals.localDocumentEncode)
               {
                   string cz = Path.Combine(Globals.currentDirectory, "cz");
                   try
                   {
                       if (Directory.Exists(cz))
                           Directory.Delete(cz, true);
                   }
                   catch 
                   {

                       MessageBox.Show("请关闭当前打开的参照记录文档！");
                       return "";
                   }
                   Directory.CreateDirectory(cz);
                   //2012-03-28
                   //if (!Directory.Exists(cz))
                   //    Directory.CreateDirectory(cz);
                   string tmpfile = noteID + series.ToString() + registryID + ".docx";
                   tmpfile = Path.Combine(cz, tmpfile);
                   udt.jj.DecodeEmrDocument(wordDocName, tmpfile);
                   return tmpfile;
               }
               return wordDocName;
           }
        public void UncommitConsult(int series)
        {
            XmlNode emrNote = GetEmrNoteBySeries(series);
            string RegistryID= emrDoc.DocumentElement.Attributes[AttributeNames.RegistryID].Value;
            if (emrNote.Attributes[AttributeNames.Sequence] == null)
                return;
            string Sequence = emrNote.Attributes[AttributeNames.Sequence].Value;
            ThisAddIn.UncommitConsultNote(Sequence,RegistryID);

        }
        public bool UncommitEmrNote(int series, XmlNode reasion)
        {
            XmlNode emrNote = GetEmrNoteBySeries(series);
            if (emrNote == null) return Return.Failed;


            int status = Convert.ToInt32(emrNote.Attributes[AttributeNames.NoteStatus].Value) - 1;
            if (ThisAddIn.UncommitEmrNote(
                emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID),
                emrNote.Attributes[AttributeNames.NoteID].Value,
                series, reasion, (NoteStatus)status) == Return.Failed) return Return.Failed;
            //XmlElement noteDoc = emrDoc.CreateElement(ElementNames.EmrNote);
            if (ThisAddIn.CanOption(ElementNames.chxTrace) == true && emrNote.Attributes["fanxiu"] == null)
            {
                ThisAddIn.fanxiu(emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID), series);
                XmlAttribute fanxiu = emrNote.OwnerDocument.CreateAttribute("fanxiu");
                emrNote.Attributes.Append(fanxiu);
                emrNote.Attributes["fanxiu"].Value = "Yes";
            }
            emrNote.Attributes[AttributeNames.NoteStatus].Value = status.ToString();
            string LastWriteTime = ThisAddIn.Today().ToFileTime().ToString();
            emrNote.Attributes[AttributeNames.LastWriteTime].Value = LastWriteTime;
            emrDoc.DocumentElement.SetAttribute(AttributeNames.LastWriteTime, LastWriteTime);
            emrNote.OwnerDocument.Save(emrDocFile);
            udt.jj.EncodeEmrDocument(emrDocFile);
            return Return.Successful;
        }
        private string DocFile(string registryID, string workFolder)
        {

            emrDocFile = udt.MakeEmrDocumentFileName(registryID,workFolder);
            if (!File.Exists(emrDocFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.LoadXml(StringGeneral.NullEmrDocument);
                doc.DocumentElement.Attributes[AttributeNames.RegistryID].Value = registryID;
                doc.Save(emrDocFile);
                udt.jj.EncodeEmrDocument(emrDocFile);
            }

            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            udt.jj.DecodeEmrDocument(emrDocFile, tmpfile);
            return tmpfile;
        }

        /* ------------------------------------------------------------------------
            Return 0  Successful
                  -1 fatal error
                  -2 no continue operation
                  -3 read only
          ------------------------------------------------------------------------- */
        public int Create(string registryID, string archive)
        {
            int returnState = 0;
            archiveNum = archive;

            #region In offline
            if (Globals.offline)
            {
                string tmpfile = DocFile(registryID);
                emrDoc.Load(tmpfile);
                File.Delete(tmpfile);
                return returnState;
            }
            #endregion

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                #region Get emrDocument and emrNotes
                XmlNode node = emrDoc.DocumentElement;
                XmlNode emrNotes = emrDoc.CreateElement(EmrConstant.ElementNames.EmrNotes);
                Boolean ret;
                try
                {
                    string machineName = Globals.localMachineName;
                    string machineIP = Globals.localMachineIP;
                    string msg = es.SetTrafficLightToRed(registryID, ref machineIP, ref machineName);
                    if (msg != null)
                    {
                        if (machineIP.Trim() != Globals.localMachineIP.Trim())
                        {
                            string error = "病历被他人打开编辑中，一般不要同时操作同一病历\r\rIP地址："
                                + machineIP + "\r\r主机名：" + machineName;
                            if (!ThisAddIn.CanOption(ElementNames.ContinueAsEmrOpened))
                            {
                                error += "\r\r进入只读状态！！！";
                                MessageBox.Show(error, ErrorMessage.Warning);
                                returnState = -3;

                            }                           
                        }
                    }
                    ret = es.GetEmrDocument(registryID, ref node, ref emrNotes);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741852967", ex.Message + ">>" + ex.ToString(), true);       
                 
                    return -1;
                }
                    
                if (ret == EmrConstant.Return.Failed)
                {
                    MessageBox.Show(ErrorMessage.WebServiceError, ErrorMessage.Error);
                    return -1;
                }
                #endregion

                #region Check if local documents are newer than in database
                if (Globals.autoCheckSync && !ThisAddIn.CanOption(ElementNames.ContinueAsEmrOpened))
                {                
                    DateTime localLwt = DateTime.Now;
                    if (ThisAddIn.HasNotesInLocal(registryID, ref localLwt))
                    {
                        long lastWriteTime = 0;
                        if (node.Attributes[AttributeNames.LastWriteTime] != null)
                        {
                            lastWriteTime = Convert.ToInt64(node.Attributes[AttributeNames.LastWriteTime].Value);
                        }
                        DateTime remoteLwt = DateTime.FromFileTime(lastWriteTime);
                        if (localLwt.CompareTo(remoteLwt) > 0)
                        {

                            DialogResult dr =  MessageBox.Show("本地病历记录比数据库的更新：\r\r本地：" + localLwt.ToString() +
                                "  数据库：" + remoteLwt.ToString() +
                                "\r\r同步为本地病历吗？\r如果选择同步应十分小心，要确认本地病历确实是最新的。" +
                                "\r因为有时因各机器时钟不一致，可能造成假象。一旦同步成功，不能恢复！",
                                ErrorMessage.Warning, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1);
                            if (dr == DialogResult.Yes)
                            {
                                #region Do synchronization
                                string msg = ThisAddIn.SynchronizeOne(registryID, archiveNum, node);
                                if (msg == null)
                                {
                                    emrDocFile = udt.MakeEmrDocumentFileName(registryID,Globals.workFolder);
                                    string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
                                    if (File.Exists(tmpfile)) File.Delete(tmpfile);
                                    udt.jj.DecodeEmrDocument(emrDocFile, tmpfile);
                                    emrDoc.Load(tmpfile);
                                    return returnState;
                                }
                                MessageBox.Show(msg, ErrorMessage.Error);
                                return -1;
                                #endregion
                            }
                            else if (dr == DialogResult.Cancel) return -2;
                        }
                    }
                }
                #endregion

                #region Create local storage for documents.
                string docLocation = ThisAddIn.GetDocLocation(registryID);
                foreach (XmlNode emrNote in emrNotes)
                {
                    string noteDoc = 
                        udt.MakeWdDocumentFileName(docLocation, emrNote.Attributes[0].Value);
                    udt.jj.StringToWordDocument(noteDoc, emrNote);

                    if (emrNote.InnerText.Length >= 2 && emrNote.InnerText.Substring(0, 2) == "UE")
                    {
                        if (Globals.localDocumentEncode) udt.jj.EncodeEmrDocument(noteDoc);
                    }
                    else
                    {
                        if (!Globals.localDocumentEncode)
                        {
                            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
                            udt.jj.DecodeEmrDocument(noteDoc, tmpfile);
                            File.Delete(noteDoc);
                            File.Move(tmpfile, noteDoc);
                        }
                    }
                }
                emrDoc.LoadXml(node.OuterXml);
                emrDocFile = udt.MakeEmrDocumentFileName(registryID,Globals.workFolder);
                emrDoc.Save(emrDocFile);
                udt.jj.EncodeEmrDocument(emrDocFile);              
                return returnState;
                #endregion
            }
        }
        private string DocFile(string registryID)
        {

            emrDocFile = udt.MakeEmrDocumentFileName(registryID,Globals.workFolder);
            if (!File.Exists(emrDocFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.LoadXml(StringGeneral.NullEmrDocument);
                doc.DocumentElement.Attributes[AttributeNames.RegistryID].Value = registryID;
                doc.Save(emrDocFile);
                udt.jj.EncodeEmrDocument(emrDocFile);
            }

            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            udt.jj.DecodeEmrDocument(emrDocFile, tmpfile);
            return tmpfile;
        }
        //public void Destroy(string registryID, PermissionLevel permission)
        //{
        //    if (emrDoc == null) return;
        //    if (permission != PermissionLevel.ReadOnly)
        //    {
        //        if (!Globals.offline)
        //        {
        //            ThisAddIn.SetTrafficLightToGreen(registryID);
        //        }
        //    }
        //    if (emrDoc.HasChildNodes) emrDoc.RemoveAll();
        //}
        private string ConsultSequence(string registryID, string noteID)
        {
            XmlNode consults = null;
            ThisAddIn.ConsultTime( registryID,Globals.PatientConsultSequence, ref  consults);
            if (consults == null) return null;
            return consults.FirstChild.Attributes[AttributeNames.Sequence].Value;
        }

        public int AddEmrNote(XmlNode notePattern, XmlElement emrNote, NoteStatus status, bool sexOption,XmlNode wordXml)
        {
            int series = -1;
            string registryID = emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
            string noteID = notePattern.Attributes[AttributeNames.NoteID].Value;

            #region Set attributes for emr note
            string sequence = null;
            #region Treat attribute StartTime
            switch (notePattern.Attributes[AttributeNames.StartTime].Value)
            {
                case StartTime.Routine:
                    if (Globals.offline)
                    {
                        emrNote.SetAttribute(AttributeNames.Start, ThisAddIn.Today().ToString());
                    }
                    else
                    {
                        emrNote.SetAttribute(AttributeNames.Start,
                            ThisAddIn.LastDegreeOrderTime(registryID).ToString());
                    }
                    break;
                case StartTime.Operation:
                    sequence = OperationSequence(registryID, noteID);
                    if (sequence == null) return series;
                    emrNote.SetAttribute(AttributeNames.Sequence, sequence);
                    break;
                case StartTime.Rescued:
                    sequence = RescueSequence(registryID, noteID);
                    if (sequence == null) return series;
                    emrNote.SetAttribute(AttributeNames.RescueSequence, sequence);
                    break;
                case StartTime.TransferIn:
                    sequence = TransferInSequence(registryID, noteID);
                    if (sequence == null) return series;
                    emrNote.SetAttribute(AttributeNames.TransferInSequence, sequence);
                    break;
                case StartTime.TransferOut:
                    sequence = TransferOutSequence(registryID, noteID);
                    if (sequence == null) return series;
                    emrNote.SetAttribute(AttributeNames.TransferOutSequence, sequence);
                    break;
                case StartTime.TakeOver:
                    sequence = TakeoverSequence(registryID, noteID);
                    if (sequence == null) return series;
                    emrNote.SetAttribute(AttributeNames.TakeOverSequence, sequence);
                    break;
                case StartTime.Consult:
                    sequence = ConsultSequence(registryID, noteID);
                    if (sequence == null)
                    {
                        //  MessageBox.Show("此次会诊记录已经书写完成！");
                        return series;
                    }
                    emrNote.SetAttribute(AttributeNames.Sequence, sequence);
                    break;


            }
            #endregion

            int sign = 0;
            //try
            //{
            //    foreach (Microsoft.Office.Interop.Word.XMLNode xn in ActiveDocumentManager.getDefaultAD().XMLNodes)
            //    {

            //        if (xn.BaseName == "电子病历")
            //        {
            //            for (int i = 1; i <= xn.ChildNodes.Count; i++)
            //            {

            //                if (xn.ChildNodes[i].Text == null) continue;
            //                if (xn.ChildNodes[i].BaseName == "医护签名")
            //                {
            //                    sign = xn.ChildNodes[i].ChildNodes.Count;
            //                    break;
            //                }
            //            }
            //        }
            //        else break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "EmrDocument 的317行");
            //}

            emrNote.SetAttribute(AttributeNames.NoteID, noteID);
            emrNote.SetAttribute(AttributeNames.NoteName, notePattern.Attributes[AttributeNames.NoteName].Value);
            emrNote.SetAttribute(AttributeNames.Header, notePattern.Attributes[AttributeNames.Header].Value);
            emrNote.SetAttribute(AttributeNames.Unique, notePattern.Attributes[AttributeNames.Unique].Value);
            emrNote.SetAttribute(AttributeNames.NoteStatus, status.ToString("d"));
            emrNote.SetAttribute(AttributeNames.WriterID, Globals.DoctorID);
            emrNote.SetAttribute(AttributeNames.Writer, Globals.DoctorName);


            DateTime now = ThisAddIn.Today();
            /* because allow the operator to choose commit time */
            emrNote.SetAttribute(AttributeNames.CheckerID, "");
            emrNote.SetAttribute(AttributeNames.Checker, "");
            emrNote.SetAttribute(AttributeNames.CheckedDate, "");
            emrNote.SetAttribute(AttributeNames.FinalCheckerID, "");
            emrNote.SetAttribute(AttributeNames.FinalChecker, "");
            emrNote.SetAttribute(AttributeNames.FinalCheckedDate, "");
            emrNote.SetAttribute("sign", sign.ToString());
            /* 2007-07-11 add sign lables */
            if (notePattern.Attributes[AttributeNames.Sign1] != null)
                emrNote.SetAttribute(AttributeNames.Sign1, notePattern.Attributes[AttributeNames.Sign1].Value);
            if (notePattern.Attributes[AttributeNames.Sign2] != null)
                emrNote.SetAttribute(AttributeNames.Sign2, notePattern.Attributes[AttributeNames.Sign2].Value);
            if (notePattern.Attributes[AttributeNames.Sign3] != null)
                emrNote.SetAttribute(AttributeNames.Sign3, notePattern.Attributes[AttributeNames.Sign3].Value);

            emrNote.SetAttribute(AttributeNames.LastWriteTime, now.ToFileTime().ToString());
            /*  add merge property */
            emrNote.SetAttribute(AttributeNames.Merge, notePattern.Attributes[AttributeNames.Merge].Value);
            emrNote.SetAttribute(AttributeNames.StartTime, notePattern.Attributes[AttributeNames.StartTime].Value);
            /* add sex feature for it to be as a template */
            if (sexOption) emrNote.SetAttribute(AttributeNames.Sex, StringGeneral.Yes);
            else emrNote.SetAttribute(AttributeNames.Sex, StringGeneral.No);
        
            if (emrNote.Attributes[AttributeNames.SingleContinue] == null)
            {
                XmlAttribute newAttribute = emrNote.OwnerDocument.CreateAttribute(AttributeNames.SingleContinue);
                emrNote.Attributes.Append(newAttribute);
            }
            if (notePattern.Attributes[AttributeNames.SingleContinue] != null)
                emrNote.SetAttribute(AttributeNames.SingleContinue, notePattern.Attributes[AttributeNames.SingleContinue].Value);
            else
                emrNote.SetAttribute(AttributeNames.SingleContinue, "no");
            /*----------------------------------------------------------------------------*/

            /* Make a new series as unique id for the EmrNote. */
            #endregion

            /* word window to word document */
            //string tmpfile = Path.GetFullPath("病程记录.dotx");
            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            //udt.jj.SaveWordDoc(ThisAddIn.Application.ActiveDocument, tmpfile, false);

            #region Save document into database
            if (!Globals.offline)
            {
                XmlElement noteDoc = emrDoc.CreateElement(ElementNames.EmrNote);
                noteDoc.InnerText = udt.jj.WordDocumentToString(tmpfile);
                emrNote.SetAttribute(AttributeNames.Series, "0");
                series = ThisAddIn.NewOneEmrNote(registryID, archiveNum, emrNote, noteDoc,wordXml);
                if (series < 0) return series;
            }
            else
            {
                series = Convert.ToInt32(emrDoc.DocumentElement.GetAttribute(AttributeNames.Series)) + 1;
                return -100;
            }

            #endregion

            #region Save document into local storage
            string wdDocName = udt.MakeWdDocumentFileName(registryID, noteID, series, Globals.workFolder);
            File.Copy(tmpfile, wdDocName, true);
            if (Globals.localDocumentEncode) udt.jj.EncodeEmrDocument(wdDocName);

            emrNote.SetAttribute(AttributeNames.Series, series.ToString("d"));
            emrDoc.DocumentElement.AppendChild(emrNote);
            emrDoc.DocumentElement.SetAttribute(AttributeNames.LastWriteTime, now.ToFileTime().ToString());
            emrDoc.DocumentElement.SetAttribute(AttributeNames.Series, series.ToString("d"));
            emrDoc.Save(emrDocFile);
            udt.jj.EncodeEmrDocument(emrDocFile);
            #endregion

            return series;
        }
        private string TransferOutSequence(string registryID, string noteID)
        {
            XmlNode tranferOuts = null;
            ThisAddIn.TransferOutTime(registryID, ref tranferOuts);
            if (tranferOuts == null) return null;

            XmlNodeList emrNotes = emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
            XmlNode congeners = ThisAddIn.GetCongener0(noteID, emrNotes);
            if (congeners.ChildNodes.Count == 0)
                return tranferOuts.FirstChild.Attributes[AttributeNames.TransferOutSequence].Value;

            string sequence = null;
            foreach (XmlNode tranferOut in tranferOuts.ChildNodes)
            {
                string commitT = "";
                sequence = tranferOut.Attributes[AttributeNames.TransferOutSequence].Value;
                if (ThisAddIn.NoteExistsWithSequence(AttributeNames.TransferOutSequence, sequence,
                    congeners, ref commitT) != null && commitT != "") continue;
                break;
            }
            return sequence;
        }
        public void SetGrading()
        {
            if (emrDoc.DocumentElement.Attributes["Grading"] == null)
            {
                ThisAddIn.SetGrade(emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID));

            }
        }
        private string TransferInSequence(string registryID, string noteID)
        {
            XmlNode tranferIns = null;
            ThisAddIn.TransferInTime(registryID, ref tranferIns);
            if (tranferIns == null) return null;

            XmlNodeList emrNotes = emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
            XmlNode congeners = ThisAddIn.GetCongener0(noteID, emrNotes);
            if (congeners.ChildNodes.Count == 0)
                return tranferIns.FirstChild.Attributes[AttributeNames.TransferInSequence].Value;

            string sequence = null;
            foreach (XmlNode tranferIn in tranferIns.ChildNodes)
            {
                string commitT = "";
                sequence = tranferIn.Attributes[AttributeNames.TransferInSequence].Value;
                if (ThisAddIn.NoteExistsWithSequence(AttributeNames.TransferInSequence, sequence, congeners, ref commitT) != null && commitT != "") continue;
                break;
            }
            return sequence;
        }
        private string TakeoverSequence(string registryID, string noteID)
        {
            XmlNode takeovers = null;
            ThisAddIn.TakeOverTime(registryID, ref takeovers);
            if (takeovers == null) return null;

            XmlNodeList emrNotes = emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
            XmlNode congeners = ThisAddIn.GetCongener0(noteID, emrNotes);
            if (congeners.ChildNodes.Count == 0)
                return takeovers.FirstChild.Attributes[AttributeNames.TakeOverSequence].Value;

            string sequence = null;
            foreach (XmlNode takeover in takeovers.ChildNodes)
            {
                string commitT = "";
                sequence = takeovers.Attributes[AttributeNames.TakeOverSequence].Value;
                if (ThisAddIn.NoteExistsWithSequence(AttributeNames.TakeOverSequence, sequence, congeners, ref commitT) != null && commitT != "") continue;
                break;
            }
            return sequence;
        }
        public bool UpdateEmrNote(int series, NoteStatus noteStatus, ref NoteStatus noteStatusEnd, bool CommitFlag)
        {
            noteStatusEnd = noteStatus;
            /* Confirm the note exists. */
            XmlNode emrNote = GetEmrNoteBySeries(series);
            if (emrNote == null) return Return.Failed;


            /* word window to word document */
            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            //udt.jj.SaveWordDoc(ThisAddIn.Application.ActiveDocument, tmpfile, false);

            #region Update the emrDocument in dadabase
            /* Package the word document into a xml elelement. */
            XmlNode noteDoc = emrDoc.CreateElement(ElementNames.EmrNote);
            noteDoc.InnerText = udt.jj.WordDocumentToString(tmpfile);
            string registryID = emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
            /* Update the dadabase. */
            if (!Globals.offline)
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        string strResult = es.IsEnabledCommit(registryID, series);

                        if (strResult == "0")
                        {
                            noteStatusEnd = NoteStatus.Draft;
                            if (CommitFlag == true)
                            {
                                MessageBox.Show("已经超时提交过的文档不能再次提交，请与质管部门联系！", ErrorMessage.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                            }

                        }
                        else
                        {
                            emrNote.Attributes["NoteStatus"].Value = (Convert.ToInt32(noteStatusEnd)).ToString();

                            if ((Convert.ToInt32(noteStatusEnd)).ToString() == "1" && ThisAddIn.CanOption(ElementNames.CommitTime))
                            {

                                if (emrNote.Attributes["fanxiu"] == null || emrNote.Attributes["fanxiu"].Value == "No")
                                {
                                    emrNote.Attributes[AttributeNames.WrittenDate].Value = ThisAddIn.Today().ToString();
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX741852968", ex.Message + ">>" + ex.ToString(), true);       
                    }
                }
                if (ThisAddIn.UpdateOneEmrNote(registryID, emrNote, noteDoc) ==
                    Return.Failed) return Return.Failed;
            }
            else
            {
                OpDone opDone = new OpDone("离线，寄存失败！");
                opDone.Show();
                return Return.Failed;
            }
            #endregion

            #region Update the local storage

            string noteID = emrNote.Attributes[AttributeNames.NoteID].Value;
            string wdDocName = udt.MakeWdDocumentFileName(registryID, noteID, series,Globals.workFolder);
            File.Copy(tmpfile, wdDocName, true);
            if (Globals.localDocumentEncode) udt.jj.EncodeEmrDocument(wdDocName);

            string LastWriteTime = ThisAddIn.Today().ToFileTime().ToString();
            emrNote.Attributes[AttributeNames.NoteStatus].Value = noteStatus.ToString("d");
            emrNote.Attributes[AttributeNames.LastWriteTime].Value = LastWriteTime;
            emrDoc.DocumentElement.SetAttribute(AttributeNames.LastWriteTime, LastWriteTime);
            emrDoc.Save(emrDocFile);
            udt.jj.EncodeEmrDocument(emrDocFile);
            #endregion

            return Return.Successful;
        }

        public bool DeleteEmrNote(int series)
        {
            XmlNode emrNote = GetEmrNoteBySeries(series);
            if (emrNote == null) return Return.Failed;

            string registryID = emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
            string noteID = emrNote.Attributes[AttributeNames.NoteID].Value;
            if (!Globals.offline)
            {
                if (ThisAddIn.DeleteOneEmrNote(registryID, noteID, series)
                    == Return.Failed)
                {
                    return Return.Failed;
                }
            }
            /* Update the local storage. */
            string LastWriteTime = ThisAddIn.Today().ToFileTime().ToString();
            emrDoc.DocumentElement.SetAttribute(AttributeNames.LastWriteTime, LastWriteTime);
            emrDoc.DocumentElement.RemoveChild(emrNote);
            emrDoc.Save(emrDocFile);
            udt.jj.EncodeEmrDocument(emrDocFile);

            string wdDocName = udt.MakeWdDocumentFileName(registryID, noteID, series, Globals.workFolder);
            File.Delete(wdDocName);

            return Return.Successful;
        }

        public int GetLastSeries()
        {
            return Convert.ToInt32(emrDoc.DocumentElement.GetAttribute(AttributeNames.Series));
        }

        public XmlNode GetEmrNoteBySeries(int series)
        {
            foreach (XmlNode emrNote in emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote))
            {
                int noteSeries = Convert.ToInt32(emrNote.Attributes[AttributeNames.Series].Value);
                if (noteSeries == series) return emrNote;
            }
            return null;
        }

    
        public void SetLastTop(string groupCode, long top)
        {
            string attributeLastTop = AttributeNames.LastTop;
            switch (groupCode)
            {
                case Groups.Two:
                    attributeLastTop = AttributeNames.LastTop2;
                    break;
                case Groups.Three:
                    attributeLastTop = AttributeNames.LastTop3;
                    break;
            }
            emrDoc.DocumentElement.SetAttribute(attributeLastTop, top.ToString());
            if (!Globals.offline)
            {
                //20110712 gL  lose emrdoc
                XmlDocument xmlDoc = null;
                XmlElement xmlEle = createEmptyXmlElement("Node", out xmlDoc);
                addXmlAttribute(xmlDoc, xmlEle, attributeLastTop, top.ToString());
                ThisAddIn.updateTheEmrElementOfEmrDocument(
                    emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID), xmlEle);
                /* Save document into database */
                //Globals.ThisAddIn.PutEmrDocumentForOneRegistry(
                //    emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID), emrDoc.DocumentElement);
            }
            emrDoc.Save(emrDocFile);
            udt.jj.EncodeEmrDocument(emrDocFile);
        }

        public long GetLastTop(string groupCode)
        {
            long top = 0;
            string attributeLastTop = AttributeNames.LastTop;
            switch (groupCode)
            {
                case Groups.Two:
                    attributeLastTop = AttributeNames.LastTop2;
                    break;
                case Groups.Three:
                    attributeLastTop = AttributeNames.LastTop3;
                    break;
            }
            if (emrDoc.DocumentElement.Attributes[attributeLastTop] != null)
                top = Convert.ToInt64(emrDoc.DocumentElement.Attributes[attributeLastTop].Value);
            return top;
        }
        private string RescueSequence(string registryID, string noteID)
        {
            XmlNode rescues = null;
            ThisAddIn.RescueTime(registryID, ref rescues);
            if (rescues == null) return null;

            XmlNodeList emrNotes = emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
            XmlNode congeners = ThisAddIn.GetCongener0(noteID, emrNotes);
            if (congeners.ChildNodes.Count == 0)
                return rescues.FirstChild.Attributes[AttributeNames.RescueSequence].Value;

            string sequence = null;
            foreach (XmlNode rescue in rescues.ChildNodes)
            {
                string commitT = "";
                sequence = rescue.Attributes[AttributeNames.RescueSequence].Value;
                if (ThisAddIn.NoteExistsWithSequence(AttributeNames.RescueSequence, sequence, congeners, ref commitT)
                    != null && commitT != "") continue;
                break;
            }
            return sequence;
        }
        public int GetLastPageNumber(string groupCode)
        {
            int pageNumber = 0;
            string attributeLastPageNumber = AttributeNames.LastPageNumber;
            switch (groupCode)
            {
                case Groups.Two:
                    attributeLastPageNumber = AttributeNames.LastPageNumber2;
                    break;
                case Groups.Three:
                    attributeLastPageNumber = AttributeNames.LastPageNumber3;
                    break;
            }
            if (emrDoc.DocumentElement.Attributes[attributeLastPageNumber] != null)
                pageNumber = Convert.ToInt32(emrDoc.DocumentElement.Attributes[attributeLastPageNumber].Value);
            return pageNumber;
        }

        public void SetLastPageNumber(string groupCode, int pageNumber)
        {
            string attributeLastPageNumber = AttributeNames.LastPageNumber;
            switch (groupCode)
            {
                case Groups.Two:
                    attributeLastPageNumber = AttributeNames.LastPageNumber2;
                    break;
                case Groups.Three:
                    attributeLastPageNumber = AttributeNames.LastPageNumber3;
                    break;
            }
            emrDoc.DocumentElement.SetAttribute(attributeLastPageNumber, pageNumber.ToString());
            if (!Globals.offline)
            {
                //20110712 gL  lose emrdoc
                XmlDocument xmlDoc = null;
                XmlElement xmlEle = createEmptyXmlElement("Node", out xmlDoc);
                addXmlAttribute(xmlDoc, xmlEle, attributeLastPageNumber, pageNumber.ToString());
                ThisAddIn.updateTheEmrElementOfEmrDocument(
                    emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID), xmlEle);
                /* Save document into database */
                //Globals.ThisAddIn.PutEmrDocumentForOneRegistry(
                //    emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID), emrDoc.DocumentElement);
            }
            emrDoc.Save(emrDocFile);
            udt.jj.EncodeEmrDocument(emrDocFile);
        }
        private XmlElement createEmptyXmlElement(string strXMLElementName,out XmlDocument xmlDoc)
        {
            xmlDoc = new XmlDocument();
            XmlElement xmlEle = xmlDoc.CreateElement(strXMLElementName);
            return xmlEle;
        }
        private XmlElement createEmptyXmlElement(string strXMLElementName, XmlDocument xmlDoc)
        {
            XmlElement xmlEle = xmlDoc.CreateElement(strXMLElementName);
            return xmlEle;
        }
        private void addXmlAttribute(XmlDocument xmlDoc,XmlElement xmlEle,string strXmlAttrName,string strXmlAttrValue)
        {
            XmlAttribute xmlAttr = xmlDoc.CreateAttribute(strXmlAttrName);
            xmlAttr.Value = strXmlAttrValue;
            xmlEle.Attributes.Append(xmlAttr);
        }
        public string GetRegistryID()
        {
            return emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
        }

        public XmlNodeList GetEmrNotes()
        {
            return emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
        }

        public XmlDocument Get()
        {
            return emrDoc;
        }

        public bool IsCommited(int series)
        {
            foreach (XmlNode emrNote in GetEmrNotes())
            {
                int noteSeries = Convert.ToInt32(emrNote.Attributes[AttributeNames.Series].Value);
                if (noteSeries == series)
                {
                    NoteStatus status =
                        (NoteStatus)(Convert.ToInt32(emrNote.Attributes[AttributeNames.NoteStatus].Value));
                    return (status == NoteStatus.Commited) || (status == NoteStatus.Checked) ||
                        (status == NoteStatus.FinallyChecked);
                }
            }
            return false;
        }

        public NoteStatus GetNoteStatus(int series)
        {
            foreach (XmlNode emrNote in emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote))
            {
                int noteSeries = Convert.ToInt32(emrNote.Attributes[AttributeNames.Series].Value);
                if (noteSeries == series)
                {
                    int status = Convert.ToInt32(emrNote.Attributes[AttributeNames.NoteStatus].Value);
                    return (NoteStatus)status;
                }
            }
            return NoteStatus.Draft;
        }
        private string OperationSequence(string registryID, string noteID)
        {
            XmlNode operations = null;
            ThisAddIn.OperationTime(registryID, ref operations);
            if (operations == null) return null;

            XmlNodeList emrNotes = emrDoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
            XmlNode congeners = ThisAddIn.GetCongener0(noteID, emrNotes);
            if (congeners.ChildNodes.Count == 0)
                return operations.FirstChild.Attributes[AttributeNames.Sequence].Value;

            string sequence = null;
            foreach (XmlNode operation in operations.ChildNodes)
            {
                string commitT ="";
                sequence = operation.Attributes[AttributeNames.Sequence].Value;
                if (ThisAddIn.NoteExistsWithSequence(AttributeNames.Sequence, sequence, congeners, ref commitT)
                    != null && commitT != "") continue;
                break;
            }
            return sequence;
        }

        public bool UpdateEmrNote(int series, NoteStatus noteStatus, ref NoteStatus noteStatusEnd)
        {
            noteStatusEnd = noteStatus;
            /* Confirm the note exists. */
            XmlNode emrNote = GetEmrNoteBySeries(series);
            if (emrNote == null) return Return.Failed;


            /* word window to word document */
            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            //udt.jj.SaveWordDoc(ActiveDocumentManager.getDefaultAD(), tmpfile, false);

            #region Update the emrDocument in dadabase
            /* Package the word document into a xml elelement. */
            XmlNode noteDoc = emrDoc.CreateElement(ElementNames.EmrNote);
            noteDoc.InnerText = udt.jj.WordDocumentToString(tmpfile);
            string registryID = emrDoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
            /* Update the dadabase. */
            if (!Globals.offline)
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        string strResult = es.IsEnabledCommit(registryID, series);

                        if (strResult == "0")
                        {
                            noteStatusEnd = NoteStatus.Draft;
                            //if (CommitFlag == true)
                            //{
                            //    MessageBox.Show("已经超时提交过的文档不能再次提交，请与质管部门联系！", ErrorMessage.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                            //}

                        }
                        else
                        {
                            emrNote.Attributes["NoteStatus"].Value = (Convert.ToInt32(noteStatusEnd)).ToString();

                            if ((Convert.ToInt32(noteStatusEnd)).ToString() == "1" && ThisAddIn.CanOption(ElementNames.CommitTime))
                            {

                                if (emrNote.Attributes["fanxiu"] == null || emrNote.Attributes["fanxiu"].Value == "No")
                                {
                                    emrNote.Attributes[AttributeNames.WrittenDate].Value = ThisAddIn.Today().ToString(StringGeneral.DateFormat);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX741852968", ex.Message + ">>" + ex.ToString(), true);       
 
                    }
                }
                if (ThisAddIn.UpdateOneEmrNote(registryID, emrNote, noteDoc) ==
                    Return.Failed) return Return.Failed;
            }
            else
            {
                OpDone opDone = new OpDone("离线，寄存失败！");
                opDone.Show();
                return Return.Failed;
            }
            #endregion

            #region Update the local storage

            string noteID = emrNote.Attributes[AttributeNames.NoteID].Value;
            string sid = emrNote.Attributes[AttributeNames.Series].Value;
            EmrDocInfo.setSeriesIDForCheck(Convert.ToInt32(sid));
       
            string wdDocName = udt.MakeWdDocumentFileName(registryID, noteID, series,Globals.workFolder);
            File.Copy(tmpfile, wdDocName, true);
            if (Globals.localDocumentEncode) udt.jj.EncodeEmrDocument(wdDocName);

            string LastWriteTime = ThisAddIn.Today().ToFileTime().ToString();
            emrNote.Attributes[AttributeNames.NoteStatus].Value = noteStatus.ToString("d");
            emrNote.Attributes[AttributeNames.LastWriteTime].Value = LastWriteTime;
            emrDoc.DocumentElement.SetAttribute(AttributeNames.LastWriteTime, LastWriteTime);
            emrDoc.Save(emrDocFile);
            udt.jj.EncodeEmrDocument(emrDocFile);
            #endregion

            return Return.Successful;
        }
    }
}
