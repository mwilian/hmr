using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EmrConstant;
using System.Data;
using CommonLib;
using System.Drawing;
using Word = Microsoft.Office.Interop.Word;
using UserInterface;
using System.Threading;
using MSXML2;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HuronControl;

namespace EMR
{

    public partial class ThisAddIn
    {
        public static DataTable dtBlocks = null;
        public static Logon logon = null;
        public static bool inStyle = true;
        public static gjtEmrPatients.OrderByMode orderByMode = gjtEmrPatients.OrderByMode.ByBed;
        private static gjtEmrPatients.PatientGettingMode patientGettingMode = gjtEmrPatients.PatientGettingMode.MyPatients;
        private static TreeView _tvPatients = null;
        private static XmlNode _xmlPatients = null;
        private static Thread tLoadTreeview;
        delegate TreeNode AddTreeNode(string node);
        public bool UseOldTemplate = false;
      

        public static bool GetOperatorInf(string Code, ref string Name, ref string passwd, ref string departmentCode, ref string departName)
        {
            try
            {
                using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                {
                    //ep.AuthenticChek(Code, ref  Name, ref passwd);

                    bool ret = ep.GetOperatorName(Code, ref  Name, ref passwd, ref departmentCode, ref  departName);

                    return ret;
                }
            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX922511256895", ex.Message + ">>" + ex.ToString(), true);            
               
                return false;
            }
        }
        public static XmlNode GetOperatorList()
        {
            XmlNode ops = null;
            try
            {

                using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                {
                    string msg = ep.Getoperators(ref ops);
                    if (msg != null)
                    {
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX922311256895", ex.Message + ">>" + ex.ToString(), true);            
             

            }
            return ops;
        }
        /*----------------------------------------------------------------------------
         * Get from HIS system a xml document(see patients.xsd) of patient information
         * in hospital and save it as xml file in local client. And then load treeview
         * with the file
         * Parameters:
         *      code -- doctorID, departmentCode, areaCode, or null
        ------------------------------------------------------------------------------ */
        public static void xmlPatientWriter(string code, TreeView tvPatients, int n, string patientFile)
        {
            Cursor.Current = Cursors.WaitCursor;
            patientGettingMode = (gjtEmrPatients.PatientGettingMode)n;
            XmlNode patients = GetPatients(patientGettingMode, code);
            if (patients != null)
            {
                XmlWriter writer = XmlWriter.Create(patientFile);
                patients.WriteTo(writer);
                writer.Close();
            }
            tvPatients.Nodes.Clear();
            LoadTreeviewWithPatients(tvPatients, patients);
            tvPatients.Tag = EmrConstant.InpatientStatus.Stay;
            Cursor.Current = Cursors.Default;
            return;

        }
        public static void xmlPatientWriterQL(string code, int n, string patientFile)
        {
            Cursor.Current = Cursors.WaitCursor;
            patientGettingMode = (gjtEmrPatients.PatientGettingMode)n;
            XmlNode patients = GetPatients(patientGettingMode, code);
            if (patients != null)
            {
                XmlWriter writer = XmlWriter.Create(patientFile);
                patients.WriteTo(writer);
                writer.Close();
            }
          

        }
        public static XmlNode GetPatients(gjtEmrPatients.PatientGettingMode mode, string code)
        {

            if (mode != gjtEmrPatients.PatientGettingMode.Consult)
            {

                using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
                {
                    return pi.PatientListEx(mode, code, orderByMode);
                }
            }

            else
            {
                using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
                {
                    return pi.PatientListExConsult(mode, code, orderByMode);
                }

            }

        }
        /* --------------------------------------------------------------------------------------
         * Normalize opcode into simple ASIIC if it is input at Chinese input.
         * -------------------------------------------------------------------------------------- */
        public static string NormalizeOpcode(string opcode)
        {
            string mycode = null;
            char[] bs = opcode.ToCharArray();
            int len = bs.Length;
            for (int i = 0; i < bs.Length; i++)
            {
                if (bs[i] > 65247)
                {
                    mycode += Convert.ToChar(bs[i] - 65248);
                }
                else
                {
                    mycode += bs[i];
                }
            }
            return mycode;
        }
        public static string GetAreaID(string depatID)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string ID = "";
                try
                {
                    ID = pi.GetAreaCodeOfDepartment(depatID);
                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX922512136895", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return ID;
            }
        }
        public static XmlNode GetDepartList()
        {
            XmlNode departs = null;
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    gjtEmrPatients.WorkMode mode = gjtEmrPatients.WorkMode.InHospital;
                    departs = pi.GetDepartmentListByMode(mode);
                    if (departs.FirstChild.Name == "error") return null;

                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX922512131111", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return departs;
            }
        }
        public static XmlNode GetAreaList()
        {
            XmlNode areas = null;
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                try
                {

                    areas = pi.GetAreas();
                    if (areas.FirstChild.Name == "error") return null;

                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX922512131122", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return areas;
            }
        }
        public static void ResetBeginTime()
        {
            Globals.beginTime = 0;
        }

        //public static void GetEmrDocument(string RegistryID, ref  XmlNode node, ref  XmlNode emrNotes)
        //{
        //    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
        //    {
        //        es.GetEmrDocument(RegistryID, ref node, ref emrNotes);

        //    }
        //}


        private static void NewLevel2(string noteID, string noteName, string unique, TreeNode nodeLevel1,
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
            nodeLevel2.ImageIndex = 6;
        }
        public static bool RescueTime(string registryID, ref XmlNode restime)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.DoneTimeForRescues(registryID, ref restime);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
            }
            return true;
        }
        public static bool TakeOverTime(string registryID, ref XmlNode taketime)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.DoneTimeForTakeOver(registryID, ref taketime);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
            }
            return true;
        }
        public static bool OperationTime(string registryID, ref XmlNode opetime)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.DoneTimeForOperations(registryID, ref opetime);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
            }
            return true;
        }
        public static bool TransferInTime(string registryID, ref XmlNode tintime)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.DoneTimeForTransferIn(registryID, ref tintime);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
            }
            return true;
        }
        public static bool TransferOutTime(string registryID, ref XmlNode touttime)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.DoneTimeForTransferOut(registryID, ref touttime);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
            }
            return true;
        }
        public static string NoteExistsWithSequence(string sequencAttribute, string sequence, XmlNode congeners, ref string commitT)
        {
            foreach (XmlNode congener in congeners.ChildNodes)
            {
                if (congener.Attributes[sequencAttribute] == null) continue;
                if (congener.Attributes[sequencAttribute].Value == sequence)
                {
                    commitT = congener.Attributes[AttributeNames.CommitDate].Value +
                                    " " + congener.Attributes[AttributeNames.CommitTime].Value;
                    return congener.Attributes[AttributeNames.WrittenDate].Value +
                                    " " + congener.Attributes[AttributeNames.WrittenTime].Value;
                }
            }
            return null;
        }
        public static XmlNode GetCongener0(string noteID, XmlNodeList emrNotes)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement congener = doc.CreateElement("Congener");
            foreach (XmlNode emrNote in emrNotes)
            {
                if (emrNote.Attributes[AttributeNames.NoteID].Value == noteID)
                {
                    XmlElement congenerNote = doc.CreateElement(ElementNames.EmrNote);
                    foreach (XmlAttribute attr in emrNote.Attributes)
                    {
                        congenerNote.SetAttribute(attr.Name, attr.Value);
                    }
                    congener.AppendChild(congenerNote);
                }
            }
            return (XmlNode)congener;
        }
        private static string NoteState(string state)
        {
            int index = Convert.ToInt32(state);
            EmrConstant.NoteStateText nst = new EmrConstant.NoteStateText();
            return " " + nst.Text[index];
        }
        private static void NewLevel2(string noteID, string noteName, string unique,
  out TreeNode nodeLevel2, TreeNode treeView1)
        {

            foreach (TreeNode node in treeView1.Nodes)
            {
                /* Node level 2 of the noteID exists? */
                if (node.Name == noteID)
                {
                    nodeLevel2 = node;
                    return;
                }
            }
            /* Make a new Node level 2 for noteID. */
            treeView1.Nodes.Add(noteName);
            nodeLevel2 = treeView1.Nodes[treeView1.Nodes.Count - 1];
            nodeLevel2.Name = noteID;
            nodeLevel2.Tag = unique;
            nodeLevel2.ForeColor = Color.CornflowerBlue;
            nodeLevel2.ImageIndex = 6;
        }
        public static string GetNoteNameFromNoteID(string noteID)
        {
            XmlNode pattern = null;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = null;

                try
                {
                    msg = es.GetEmrPattern(StringGeneral.NullCode, ref pattern);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX922512131113", ex.Message + ">>" + ex.ToString(), true);            
             
                }
            }
            if (pattern == null) return null;
            foreach (XmlNode emrNote in pattern.SelectNodes("EmrNote"))
            {
                if (emrNote.Attributes["NoteID"].Value == noteID)
                    return emrNote.Attributes["NoteName"].Value;
            }
            return null;
        }
        public static int NewOneEmrNote(string registryID, string archiveNum, XmlNode emrNote, XmlNode noteDoc, XmlNode WordXml)
        {
            int series = -1;

            XmlDocument ret = new XmlDocument();

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    string msg = "";
                    emrNote.InnerXml = emrNote.InnerXml.Replace("&", "");
                    //XmlNode xnode = ThisAddIn.GetXmlfile();
                    string strErrText = "";
                    bool bOK = false;
                    DateTime dtStart = DateTime.Now;
                    DateTime dtEnd = dtStart;
                    int iErrorCount = 0;
                    int iMaxErrorCount = 5;
                    for(iErrorCount =0;iErrorCount<iMaxErrorCount;iErrorCount++)
                    {
                        if (WordXml != null)
                            msg = es.NewEmrNoteExZ(registryID, archiveNum, emrNote, noteDoc, WordXml, ref series);
                        else msg = es.NewEmrNoteEx(registryID, archiveNum, emrNote, noteDoc, ref series);
                        if(msg == null && series > 0){
                            bOK = true;
                            strErrText = "";
                            dtEnd = DateTime.Now;
                            break;
                        }else{
                            if(strErrText.Length == 0){
                                if (msg == null) {
                                    msg = "无错误具体消息。添加：Series:" + series.ToString();
                                }
                               strErrText = msg;
                            }
                        }
                    }
                    if (bOK == false)
                    {
                        dtEnd = DateTime.Now;
                        TimeSpan ts = dtEnd - dtStart;
                        msg = "2013-6-4，，添加：" + strErrText + "失败，提示错误消息。耗时：" + ts.TotalSeconds.ToString();
                        EmrDocInfo.setDBError(msg);
                        if (msg != null)
                        {
                            MessageBox.Show(msg, ErrorMessage.Error);
                        }
                    }
                    else if (iErrorCount > 0)
                    {
                        TimeSpan ts = dtEnd - dtStart;
                        string strSendMessage = "2013-6-4，添加："+iErrorCount.ToString()+"次 成功。耗时："+ts.TotalSeconds.ToString();
                        EmrDocInfo.setDBError(strSendMessage);
                    }
                  
                    return series;
                }

                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX922512132222", ex.Message + ">>" + ex.ToString(), true);            
             
                    return series;
                }
            }
        }
        public static bool UpdateOneEmrNote(string registryID, XmlNode emrNote, XmlNode noteDoc)
        {

            XmlDocument ret = new XmlDocument();

            string msg = "";
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string strErrText = "";
                bool bOK = false;
                DateTime dtStart = DateTime.Now;
                DateTime dtEnd = dtStart;
                int iErrorCount = 0;
                int iMaxErrorCount = 5;
                for (iErrorCount = 0; iErrorCount < iMaxErrorCount; iErrorCount++)
                {
                    msg = es.UpdateEmrNoteEx(registryID, emrNote, noteDoc);

                    if (msg == null)
                    {
                        bOK = true;
                        strErrText = "";
                        dtEnd = DateTime.Now;
                        break;
                    }
                    else
                    {                       
                            strErrText = msg;                        
                    }
                }
                if (bOK == false)
                {
                    dtEnd = DateTime.Now;
                    TimeSpan ts = dtEnd - dtStart;
                    msg = "2013-6-4，更新：" + strErrText + "失败，提示错误消息。耗时：" + ts.TotalSeconds.ToString();
                    EmrDocInfo.setDBError(msg);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return Return.Failed;
                    }
                }
                else if (iErrorCount > 0)
                {
                    TimeSpan ts = dtEnd - dtStart;
                    string strSendMessage = "2013-6-4，更新：" + iErrorCount.ToString() + "次 成功。耗时：" + ts.TotalSeconds.ToString();
                    EmrDocInfo.setDBError(strSendMessage);
                }
            }
          

            return Return.Successful;
        }

        public static bool IsPermission(string opcode, string RoleID)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                bool perm = es.GetRolesPermission(opcode, RoleID);
                return perm;
            }

        }
        public static bool GetFlaws(string noteID, bool isEndRule, ref XmlNode rules)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {

                if (isEndRule == false)
                {
                    es.GetValuateRules(noteID, ref rules);
                }
                else
                {
                    es.GetValuateRulesEnd(noteID, ref rules);
                }
                if (rules == null) return false;
                //dgvFlaw.Rows.Clear();


                return true;
            }
        }
        public static bool IsInHospital(string RegistryID)
        {
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                if (ep.IsInHospital(RegistryID) == false)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
        }
        public static string SaveValuate(string registryID, string noteID, decimal score, string opcode, XmlNode note, bool self, bool isEndRule)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {



                try
                {
                    string msg = "";
                    if (isEndRule == false)
                    {
                        msg = es.NewValuateDetail(self, registryID, noteID, score, opcode, note);
                    }
                    else
                    {
                        msg = es.NewValuateDetailEnd(self, registryID, noteID, score, opcode, note);
                    }
                    return msg;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX922512132233", ex.Message + ">>" + ex.ToString(), true);            
             
                    return ex.Message;
                }
            }
        }
        public static DataSet GetPatientData(string RegistryID)
        {
            DataSet dst = new DataSet();
            try
            {
                using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                {
                    dst = ep.GetPatientInf(RegistryID);

                }
                return dst;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9225111232233", ex.Message + ">>" + ex.ToString(), true);
                return null;
            }
        }
        public static bool ImportWordDoc(string wdDocName, Word.Application WordApp, bool decode,bool isblock)
        {
            if (!File.Exists(wdDocName)) return false;
            object oMissing = System.Reflection.Missing.Value;
            if (WordApp.Documents.Count == 0)
            {
                MessageBox.Show("无效的操作，请先打开一个病例！");
                return false;
            }
            Word.Range range=null;
            if(isblock)
              range = WordApp.Selection.Range;
            else
              range = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range.Start = range.End;
            range.Select();
            object False = (object)false;
            object psd = (object)"jwsj";

            //try
            //{
            //    if (!isblock)
            //    range.Application.ActiveDocument.Unprotect(ref psd);
            //}
            //catch(Exception ex)
            //{
            //    Globals.logAdapter.Record("EX9225144659826", ex.Message + ">>" + ex.ToString(), true);            
             
            //}

            if (decode)
            {
                try
                {
                    string tmpfile = Path.Combine(Directory.GetCurrentDirectory(), ResourceName.MargeDoc);
                    udt.jj.DecodeEmrDocument(wdDocName, tmpfile);
                    range.InsertFile(tmpfile, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                }
                catch (Exception ex)
                {
                     Globals.logAdapter.Record("EX9225144232233", ex.Message + ">>" + ex.ToString(), true);            
             
                    WordApp = null;
                }
            }
            else
            {
                try
                {
                    range.InsertFile(wdDocName, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                }
                catch(Exception ex)
                {
                    Globals.logAdapter.Record("EX9225145659826", ex.Message + ">>" + ex.ToString(), true);            
             
                    MessageBox.Show("文档已被锁定不能插入数据！");
                }
            }
            return true;
        }
        public static string GetValuate(string registryID, ref XmlNode flaws, ref  XmlNode rules, bool self, bool isEndRule)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {

                string msg = "";
                if (isEndRule == false)
                {
                    msg = es.GetValuateDetail(self, registryID, ref flaws);
                }
                else
                {
                    msg = es.GetValuateDetailEnd(self, registryID, ref flaws);

                }
                if (msg == null) msg = es.GetNoteIDsWithValuateRules(ref rules);
                return msg;
            }

        }
        public static string SaveValuate(string registryID, string patientDoctor, string patientDepartment, string Opcode, bool self, bool isEndRule, decimal scorePercent, int vi)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {

                string msg = "";
                if (isEndRule == false)
                {
                    msg = es.NewValuateScore(self, registryID, patientDoctor, patientDepartment,
                    Opcode, scorePercent, vi);
                }
                else
                {
                    msg = es.NewValuateScoreEnd(self, registryID, patientDoctor, patientDepartment,
                                      Opcode, scorePercent, vi);
                }
                return msg;
            }

        }
        public static DataTable GetData(string strStart, string strEnd, string Depart, string Doctor)
        {

            DataSet dt = new DataSet();
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                dt = pi.GetQCTableByMonthNew(strStart, strEnd, Depart, Doctor);


            }
            if (dt.Tables.Count != 0)
                return dt.Tables[0];
            else
                return null;
        }
        public static XmlNode GetPatientsList(gjtEmrPatients.QueryMode mode, string criteria)
        {
            XmlNode patients = null;
            Cursor.Current = Cursors.WaitCursor;
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    patients = pi.QueryPatientList(mode, criteria, inStyle);
                }
                catch(Exception ex)
                {
                    Globals.logAdapter.Record("EX9225145659827", ex.Message + ">>" + ex.ToString(), true);            
             
                }
            }
            Cursor.Current = Cursors.Default;
            return patients;
        }
        public static string GetConfig(ref XmlNode Config)
        {
            string msg = null;
            try
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    msg = es.GetSecurityConfig(ref Config);
                }
            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX92257745659827", ex.Message + ">>" + ex.ToString(), true);            
             
            }

            return msg;
        }

        public static bool CanExeutiveCommand(string command)
        {
            foreach (string func in Globals.myfunctions)
            {
                if (command == func) return true;
            }
            MessageBox.Show(ErrorMessage.NoPrivilege, ErrorMessage.Warning);
            return false;
        }
        public static bool CanExeutiveCommandQuiet(string command)
        {
            foreach (string func in Globals.myfunctions)
            {
                if (command == func) return true;
            }
            return false;
        }
        /* --------------------------------------------------------------------------------------
         * Get bool value for an option parameter. 
         * -------------------------------------------------------------------------------------- */
        private static bool OptionValue(XmlNode options, string optionName)
        {
            XmlNode option = options.SelectSingleNode(optionName);
            if (option == null) return false;
            return option.InnerText == EmrConstant.StringGeneral.Yes;
        }
        public static string GetPattern(string code, ref XmlNode pattern)
        {
            string msg = null;

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {

                try
                {
                    msg = es.GetEmrPattern(code, ref pattern);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9225696659827", ex.Message + ">>" + ex.ToString(), true);            
             
                }
            }
            return msg;
        }
        /* ---------------------------------------------------------------------------------
         * Lock the emrDocument for all the patients discharged before a given date
         * and clear relative local storage.
         * ------------------------------------------------------------------------------- */
        public static void Archive(DateTime endDischargedDate, ListBox archiveList)
        {
            /* Get a set of registryIDs from local storage. */
            XmlNode inRegistryIDs = GetInRegistryIDs();

            XmlDocument doc = new XmlDocument();
            XmlNode outRegistryIDs = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);

            /* Are there discharged in this set. */
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    ep.IsDischarged(inRegistryIDs, endDischargedDate, ref outRegistryIDs);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9225699525", ex.Message + ">>" + ex.ToString(), true);            
             
                }
            }
            /* No discharged patients */
            if (outRegistryIDs.ChildNodes.Count == 0) return;
            /* Set lock flag of the emrDocuments for these patients. */
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    es.ArchiveBatch(outRegistryIDs);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX99852674163", ex.Message + ">>" + ex.ToString(), true);            
             
                }
            }
            /* Clear the loacl storage. */
            foreach (XmlNode outRegistryID in outRegistryIDs)
            {
                Directory.Delete(Path.Combine(Globals.workFolder, outRegistryID.InnerText), true);
                if (archiveList != null)
                {
                    archiveList.Items.Add(outRegistryID.InnerText);
                    archiveList.Update();
                }
            }
        }
        private static XmlNode GetInRegistryIDs()
        {
            /* Get a set of registryIDs from local storage. */
            string[] registryIDs = Directory.GetDirectories(Globals.workFolder);
            XmlDocument doc = new XmlDocument();
            XmlNode inRegistryIDs = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
            foreach (string registryID in registryIDs)
            {
                XmlElement inRegistryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                inRegistryID.InnerText = Path.GetFileName(registryID);
                inRegistryIDs.AppendChild(inRegistryID);
            }
            return inRegistryIDs;
        }
        public static DataSet GetInf(string registryID)
        {
            DataSet dst = new DataSet();
            DataSet dst1 = new DataSet();

            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                dst = ep.GetPatientInf(registryID);
                dst1 = ep.GetCPatientInf(registryID);
            }
           // ds = dst1;
            string dd;
            try
            {
                dd = dst.Tables[0].Rows[0]["xb"].ToString();
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX99852674164", ex.Message + ">>" + ex.ToString(), true);            
             
                MessageBox.Show("病人已出院，不能新建病例");
                return null;
            }

            return dst;
        }

        public static bool ConsultTime(string registryID, string Sequence, ref XmlNode consults)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.DoneTimeForConsult(registryID, Sequence, ref consults);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
            }
            return true;
        }
        //public static string StartTimeAttribute(string noteID)
        //{
        //    XmlNode docPattern=null;
        //    GetPattern(ref docPattern);
        //    if (docPattern == null) return null;
        //    XmlNodeList notes = docPattern.SelectNodes(ElementNames.EmrNote);
        //    foreach (XmlNode note in notes)
        //    {
        //        if (note.Attributes[AttributeNames.NoteID].Value == noteID)
        //            return note.Attributes[AttributeNames.StartTime].Value;
        //    }
        //    return null;
        //}
        public static int IsGrade(string registryID)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    int isg = es.IsValuate(registryID);
                    return isg;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return -1;
                }
            }
        }
        public static void LoadTreeviewWithPatients(TreeView tvPatients, XmlNode patients)
        {
            _tvPatients = tvPatients;
            _xmlPatients = patients;

            if (tLoadTreeview != null && tLoadTreeview.IsAlive)
            {
                tLoadTreeview.Abort();
                _tvPatients.Parent.Parent.Parent.Enabled = true;
                _tvPatients.FindForm().Text = Globals.frmMainText;
            }
            tLoadTreeview = new Thread(LoadTreeview);
            tLoadTreeview.Start();
          
        }

        private static void LoadTreeview()
        {
            if (_tvPatients.FindForm() != null)
            {
                _tvPatients.FindForm().Text = Globals.frmMainText + " - 载入中...";
                _tvPatients.Parent.Parent.Parent.Enabled = false;
                _tvPatients.BeginUpdate();
            }


            XmlDocument doc = new XmlDocument();
            int i = 0;
            int n = 0;
            DeIsValuate deis = IsGrade;           
            foreach (XmlNode pnt in _xmlPatients.ChildNodes)
            {
                bool inStyle = true;
                string sex = pnt.Attributes[AttributeNames.Sex].Value;
                XmlNodeList li = pnt.SelectNodes("Registry");
                string DepartmentCode = li[0].Attributes[AttributeNames.DepartmentCode].Value;
                string linklist = Path.Combine(Globals.workFolder, EmrConstant.ResourceName.LinkListFolder);
                string departName = Path.Combine(linklist, EmrConstant.ResourceName.DepartmentsXml);
                string DepartmentName = udt.jj.GetDepartmentNameFromCode(DepartmentCode, departName);
                string patientInfo = li[0].Attributes[AttributeNames.BedNum].Value.Trim() + Delimiters.Space + pnt.Attributes[AttributeNames.PatientName].Value.Trim() + Delimiters.Space +
                DepartmentName.Trim() + Delimiters.Space + sex.Trim() + Delimiters.Space;
                patientInfo += pnt.Attributes[AttributeNames.Age].Value;
                patientInfo += pnt.Attributes[AttributeNames.AgeUnit].Value;
                //patientInfo += "岁";
                _tvPatients.Invoke(new AddTreeNode(_tvPatients.Nodes.Add), patientInfo);
                _tvPatients.Nodes[i].Tag = pnt.Attributes[AttributeNames.ArchiveNum].Value;
                _tvPatients.Nodes[i].Name = sex;
                if (sex == "男") _tvPatients.Nodes[i].ImageIndex = 0;
                else _tvPatients.Nodes[i].ImageIndex = 1;

                bool firstRegistry = true;
                for (int j = 0; j < pnt.ChildNodes.Count; j++)
                {
                    string registryID = pnt.ChildNodes[j].Attributes[EmrConstant.AttributeNames.RegistryID].Value;
                    string bedNum = pnt.ChildNodes[j].Attributes[EmrConstant.AttributeNames.BedNum].Value;
                    string doctorID = pnt.ChildNodes[j].Attributes[EmrConstant.AttributeNames.DoctorID].Value;
                    string itemtext = null;
                    string patientStatus = null;
                    string Sequence = "";
                    if (pnt.ChildNodes[j].Attributes[EmrConstant.AttributeNames.Sequence] != null)
                        Sequence = pnt.ChildNodes[j].Attributes[EmrConstant.AttributeNames.Sequence].Value;
                    if (inStyle)
                    {
                        itemtext = bedNum + ":" + registryID + ":" + doctorID;
                        //patientStatus = pnt.ChildNodes[j].Attributes[AttributeNames.PatientStatus].Value;
                        //if (patientStatus != null && patientStatus.Length > 0)
                        //{
                        //    itemtext += ":" + patientStatus;
                        //    if (firstRegistry)
                        //    {
                        //        _tvPatients.Nodes[i].Text += " " + patientStatus;
                        //        firstRegistry = false;
                        //    }
                        //}
                        //else
                        //{
                        //    if (firstRegistry) firstRegistry = false;
                        //}
                        patientStatus = pnt.ChildNodes[j].Attributes[AttributeNames.PatientStatus].Value;
                        if (patientStatus != null && patientStatus.Length > 0)
                        {
                            itemtext += ":" + patientStatus;
                            if (patientStatus == "出院:已归档")
                            {
                                patientStatus = "出院";
                            }
                            if (firstRegistry)
                            {
                                _tvPatients.Nodes[i].Text += " " + patientStatus;
                                firstRegistry = false;
                            }
                        }
                        else
                        {
                            if (firstRegistry) firstRegistry = false;
                        }
                    }
                    else
                    {
                        if (pnt.ChildNodes[j].Attributes[EmrConstant.AttributeNames.State].Value == "1")
                            itemtext += ":" + EmrConstant.StringGeneral.MedicalFinished;
                    }

                    itemtext = itemtext + ":" + Sequence;
                    _tvPatients.Invoke(new AddTreeNode(_tvPatients.Nodes[i].Nodes.Add), itemtext + "          ");
                    _tvPatients.Nodes[i].LastNode.Tag = "-1";                    
                    _tvPatients.Nodes[i].LastNode.Name = registryID;
                    _tvPatients.Nodes[i].LastNode.ImageIndex = 3;
                    if (!string.IsNullOrEmpty(patientStatus) && patientStatus.Length > 0)
                    {
                        _tvPatients.Nodes[i].LastNode.ForeColor = Color.DimGray;
                        _tvPatients.Nodes[i].LastNode.ImageIndex = 1;
                    }
                    else
                    {
                        _tvPatients.Nodes[i].LastNode.ForeColor = Color.FromArgb(55, 55, 55);
                        _tvPatients.Nodes[i].LastNode.ImageIndex = 7;
                    }
                    n = deis(registryID);
                    if (n == 0 || n == 1 || n == 2)
                    {
                        _tvPatients.Nodes[i].ForeColor = Color.Red;
                        _tvPatients.Nodes[i].LastNode.BackColor = Color.LemonChiffon;
                    }
                    else if (n == 3)
                    {
                        _tvPatients.Nodes[i].ForeColor = Color.Red;
                        _tvPatients.Nodes[i].LastNode.BackColor = Color.PeachPuff;
                    }
                    _tvPatients.Nodes[i].LastNode.Tag = EmrConstant.StringGeneral.RegistryClosed;
                }
                i++;
            }
            _tvPatients.EndUpdate();
            _tvPatients.Parent.Parent.Parent.Enabled = true;
            _tvPatients.FindForm().Text = Globals.frmMainText;
            if (Globals.inoutmode != 0)
            {
                for (int k = _tvPatients.Nodes.Count - 1; k >= 0; k--)
                {
                    TreeNode patient = _tvPatients.Nodes[k];
                    if (patient.Text.Split(Delimiters.Space).Length == Globals.inoutmode) patient.Remove();
                }
                Globals.inoutmode = 0;
            }
        }

        /*----------------------------------------------------------------------------
         * Return a xml document of doctor information in hospital 
         *    <Doctors xmlns="">
         *       <Doctor Code="1092" Name="安凤岭" Spell="AFL" TecqTitle="副主任医师" TitleLevel="2" />
         *       <Doctor Code="1310" Name="白大鹏" Spell="BDP" TecqTitle="见习医师" TitleLevel="4" />
         *       <Doctor Code="1042" Name="白婕" Spell="BJ" TecqTitle="主任医师" TitleLevel="1" />
         *       <Doctor Code="1416" Name="白莹" Spell="BY" TecqTitle="主治医师" TitleLevel="3" />
         *    </Doctor>
         ------------------------------------------------------------------------------ */
        private static XmlNode GetDoctors()
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    if (ThisAddIn.CanOption(ElementNames.AssignCheckRight) == true)
                    {
                        return pi.DoctorListEmr("");
                    }
                    else
                    {
                        return pi.DoctorList();
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX99852674456", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }
        public static bool SaveWordDoc(Microsoft.Office.Interop.Word.Document doc, string fileName, bool encode)
        {
            if (fileName.Length == 0) return false;
            object oMissing = System.Reflection.Missing.Value;
            object wdDocName = fileName;
            doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
               ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
               ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            /* Unlink the filename from acctive documenet. */
            wdDocName = EmrConstant.ResourceName.MyDocName;
            doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            if (encode) udt.EncodeEmrDocument(fileName);
           
            return true;
        }
        public static XmlNode GetPhrase(string DepartID, string DoctorID)
        {
            #region Read phrases from database.
            XmlDocument dptDoc = new XmlDocument();
            XmlNode Phrases = dptDoc.CreateElement(EmrConstant.ElementNames.Phrases);
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {

                try
                {
                    string msg = es.GetNotePhrases(DepartID,
                        DoctorID, ref Phrases);
                    if (msg == null)
                    {
                        dptDoc.LoadXml(Phrases.OuterXml);

                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX99852963852", ex.Message + ">>" + ex.ToString(), true);            
             
                }

            }
            #endregion
            return Phrases;
        }

        public static void ShowMessage(string msg, double d, MsgTpe mt)
        {

            Emr_Message emsg = new Emr_Message(msg, d, mt);
            emsg.HeightMax = 170;
            emsg.WidthMax = 203;
            emsg.ScrollShow();

        }
        /*----------------------------------------------------------------------------
         * Get from SQL server a xml document of picture information which defined
         * in advance and save it as xml file in local client.
         * file content is :
         *    <PicGallery>
         *       <Picture Name="肺">
         *         ..... image data
         *       </Picture>
         *       <Picture Name="肝">
         *         ..... image data
         *       </Picture>
         *    </PicGallery>
        ------------------------------------------------------------------------------ */
        public static XmlNode xmlPicGalleryWriter(string Code)
        {

            XmlNode picGallery = null;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    picGallery = es.GetPictures(Code);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX99852852963", ex.Message + ">>" + ex.ToString(), true);            
             
                }
            }
            return picGallery;

        }
        public static bool updateTheEmrElementOfEmrDocument(string registryID, XmlElement emrEle)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    string strError = es.InsertOrReplaceAttributeOfEmrdocument_Emr(registryID, emrEle);
                    if (strError != null)
                    {
                        MessageBox.Show(strError, ErrorMessage.Error);
                        return EmrConstant.Return.Failed;
                    }
                    return EmrConstant.Return.Successful;

                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX99852963741", ex.Message + ">>" + ex.ToString(), true);            
             
                    return Return.Failed;
                }
            }
        }
        public static void WebError(Exception ex)
        {
            Emr_Message msg = new Emr_Message(ex.ToString(), 2, MsgTpe.Error);           
            msg.HeightMax = 170;
            msg.WidthMax = 203;
            msg.ScrollShow();
        }

        public static DataTable GetEmrBlock(string departID)
        {
            XmlNode pks = GetEmrBlockKeys(departID);
            if (pks == null) return null;
            dtBlocks = new DataTable();
            DataColumn dc1 = new DataColumn("Category");
            DataColumn dc2 = new DataColumn("Block");
            DataColumn dc3 = new DataColumn("PK");
            DataColumn dc4 = new DataColumn("Path");
            dtBlocks.Columns.Add(dc1);
            dtBlocks.Columns.Add(dc2);
            dtBlocks.Columns.Add(dc3);
            dtBlocks.Columns.Add(dc4);
            for (int i = 0; i < pks.ChildNodes.Count; i++)
            {
                XmlNode pk = pks.ChildNodes[i];
                int pkValue = Convert.ToInt32(pk.InnerText);
                LoadBlockDatatable(pkValue);
            }
            return dtBlocks;
        }
        public static void LoadBlockDatatable(int pk)
        {
            XmlNode block = GetEmrBlock(pk);
            if (block != null)
            {
                DataRow dr = dtBlocks.NewRow();
                string category = block.Attributes[AttributeNames.Category].Value;
                string name = block.Attributes[AttributeNames.Name].Value;
                dr["Category"] = category;
                dr["Block"] = name;
                dr["PK"] = pk;
                string bname = udt.MakeBlockDocumentPath(pk, name, category, Globals.blockFolder);
                dr["Path"] = bname;
                udt.jj.StringToWordDocument(bname, block);
                //udt.jj.EncodeEmrDocument(bname);
                dtBlocks.Rows.Add(dr);
            }
        }
        public static XmlNode GetEmrBlock(int pk)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                XmlDocument doc = new XmlDocument();
                XmlNode block = doc.CreateElement(ElementNames.Block);
                try
                {
                    if (es.GetEmrBlock(pk, ref block) == Return.Failed) return null;
                    return block;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9985852852", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }

        public static XmlNode GetEmrBlockKeys(string departCode)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                XmlNode pks = null;
                try
                {
                    //if (UseOldPic == false)
                    //{
                    if (es.GetEmrBlockKeys(departCode, ref pks) == Return.Failed) return null;
                    return pks;
                    //}
                    //else
                    //{
                    //    if (es.GetEmrBlockKeysNew(ref pks) == Return.Failed) return null;
                    //    return pks;
                    //}
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX998529663852", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }

        /* ------------------------------------------------------------------------------ 
        * Check if servers are ok, public offline = false if so, else offline = true.
        * ------------------------------------------------------------------------------ */
        public static string CheckOffline()
        {
            Globals.addinConfig = Path.Combine(Globals.currentDirectory, "EMR.exe.config");
            XmlReader addinConfigReader = XmlReader.Create(Globals.addinConfig);
            //  if (addinConfigReader == null) AppError();
            Globals.offline = false;
            while (addinConfigReader.ReadToFollowing("value"))
            {
                string valueInnerText = addinConfigReader.ReadElementContentAsString();
              
                string[] items = valueInnerText.Split('/');
                string item = items[items.Length - 1];
                if ((!item.Equals("emrPatients.asmx")) && (!item.Equals("EmrOrder.asmx")) && (!item.Equals("emrServiceXml.asmx")))
                    continue;
                ServerUrl(valueInnerText);
               // MessageBox.Show(valueInnerText);
                if (!IsOkWebService(valueInnerText))
                {
                    Globals.offline = true;
                    return GetServerName(valueInnerText);
                }
            }
            #region Check dadabase servers.
            Globals.offline = true;
            Boolean retOracle, retSql;
           
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                //MessageBox.Show("ConnectSql");
                retSql = es.ConnectSql();
            }
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                retOracle = pi.ConnectHisdb();
            }
            if (retOracle == Return.Successful && retSql == Return.Successful)
                Globals.offline = false;
            #endregion
            return null;
            //offline = true;
        }
        private static void ServerUrl(string value)
        {
            string[] fields = value.Split(Delimiters.Cut);
            switch (fields[fields.Length - 1])
            {
                case StringGeneral.hisService:
                    Globals.hisServer = string.Empty;
                    for (int k = 0; k < fields.Length - 1; k++) Globals.hisServer += fields[k] + Delimiters.Cut;
                    break;
                case StringGeneral.emrService:
                    Globals.emrServer = string.Empty;
                    for (int k = 0; k < fields.Length - 1; k++) Globals.emrServer += fields[k] + Delimiters.Cut;
                    break;
            }
        }
        /* ------------------------------------------------------------------------------ 
         * Check if a web service is ok, return true if so, else return false.
         * ------------------------------------------------------------------------------ */
        public static bool IsOkWebService(string url)
        {
            XMLHTTP http = new XMLHTTP();
            try
            {
                http.open("GET", url, false, null, null);
                http.send(null);
                int iStatus = http.status;
                //如果取得的网页状态不正确,   就是不存在或没权访问   
                return iStatus == 200;
                //MessageBox.Show(url);
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX97894563852", ex.Message + ">>" + ex.ToString(), true);            
             
                return false;
            }
        }
        private static string GetServerName(string value)
        {
            string serverName = null;
            string[] fields = value.Split(Delimiters.Cut);
            switch (fields[fields.Length - 1])
            {
                case StringGeneral.hisService:
                    serverName = StringGeneral.HisServer;
                    break;
                case StringGeneral.emrService:
                    serverName = StringGeneral.EmrServer;
                    break;
                case StringGeneral.ordService:
                    serverName = StringGeneral.HisServer;
                    break;
            }
            return serverName;
        }



        public static void CreateWorkFolders(CreateWorkFolderMode buildMode)
        {
            Globals.DepartID = Globals.myConfig.GetDepartmentCode();
            Globals.DepartName = Globals.myConfig.GetDepartmentName();
            Globals.workFolder = Globals.myConfig.GetWorkFolder();
            CreateLocalStorage(true);
        }
        private static void CreateLocalStorage(bool refresh)
        {
            Globals.linkListFolder = Path.Combine(Globals.workFolder, EmrConstant.ResourceName.LinkListFolder);
            if (!Directory.Exists(Globals.linkListFolder)) Directory.CreateDirectory(Globals.linkListFolder);
            Globals.patientListFolder = Path.Combine(Globals.workFolder, EmrConstant.ResourceName.PatientFolder);
            if (!Directory.Exists(Globals.patientListFolder)) Directory.CreateDirectory(Globals.patientListFolder);
            Globals.templateFolder = Path.Combine(Globals.workFolder, EmrConstant.ResourceName.TemplateFolder);
            if (!Directory.Exists(Globals.templateFolder)) Directory.CreateDirectory(Globals.templateFolder);

            Globals.blockFolder = Path.Combine(Globals.workFolder, Globals.DoctorID + "blocks");
            if (!Directory.Exists(Globals.blockFolder)) Directory.CreateDirectory(Globals.blockFolder);

            /* Create local storage name. */
            Globals.doctorsFile = Path.Combine(Globals.linkListFolder, EmrConstant.ResourceName.DoctorsXml);
            Globals.doctorNamesFile = Path.Combine(Globals.linkListFolder, EmrConstant.ResourceName.DoctorNamesXml);
            Globals.departmentFile = Path.Combine(Globals.linkListFolder, EmrConstant.ResourceName.DepartmentsXml);
            Globals.picGalleryFile = Path.Combine(Globals.templateFolder, Globals.DepartID + EmrConstant.ResourceName.PicGalleryXml);

            if (refresh)
            {
                xmlDoctorWriter();
                xmlDoctorNameWriter();
                xmlDepartWriter();
                xmlPatternWriter();
                xmlPicGalleryWriter();
            }
        }
        public static void xmlDoctorWriter()
        {
            if (!Globals.offline)
            {
                XmlNode doctors = GetDoctors();
                if (doctors != null)
                {
                    XmlWriter writer = XmlWriter.Create(Globals.doctorsFile);
                    doctors.WriteTo(writer);
                    writer.Close();
                }
            }
        }
        public static void xmlDoctorNameWriter()
        {
            if (!Globals.offline)
            {
                XmlNode doctors = GetDoctorNames();
                if (doctors != null)
                {
                    XmlWriter writer = XmlWriter.Create(Globals.doctorNamesFile);
                    doctors.WriteTo(writer);
                    writer.Close();
                }
            }
        }
        public static void xmlDepartWriter()
        {
            if (Globals.offline) return;
            gjtEmrPatients.WorkMode mode = gjtEmrPatients.WorkMode.OutHospital;
            if (inStyle) mode = gjtEmrPatients.WorkMode.InHospital;
            XmlNode departs = GetDepartmentsByMode(mode);
            if (departs == null) return;

            foreach (XmlNode depart in departs.ChildNodes)
            {
                string area = GetAreaCodeOfDepartment(depart.Attributes[AttributeNames.Code].Value);
                XmlElement departE = (XmlElement)depart;
                departE.SetAttribute(AttributeNames.AreaCode, area);
            }
            XmlWriter writer = XmlWriter.Create(Globals.departmentFile);
            departs.WriteTo(writer);
            writer.Close();
        }
        public static void xmlPatternWriter()
        {
            if (Globals.offline) return;
            XmlNode emrPattern = GetPattern();
            if (emrPattern == null) return;

            XmlWriter writer = XmlWriter.Create(Globals.emrPatternFile);
            emrPattern.WriteTo(writer);
            writer.Close();
            emrPattern = GetChildPattern();
            XmlWriter writers = XmlWriter.Create(Globals.ChildPatternFile);
            emrPattern.WriteTo(writers);
            writers.Close();
        }
        public static void xmlPicGalleryWriter()
        {
            if (Globals.offline) return;
            XmlNode picGallery = null;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    if (ThisAddIn.CanOption(ElementNames.UseOldPic) == false)
                        picGallery = es.GetPictures(Globals.DepartID);
                    else picGallery = es.GetPicturesNew();
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX99836985241", ex.Message + ">>" + ex.ToString(), true);            
             
                    return;
                }
            }

            if (picGallery == null) return;
            XmlWriter writer;
            writer = XmlWriter.Create(Globals.picGalleryFile);
            picGallery.WriteTo(writer);
            writer.Close();
        }
        public static void InitRoles(string configString)
        {

            if (Globals.offline) return;
            XmlNode config = GetConfig();
            if (config == null) return;
            XmlNode roles = config.SelectSingleNode(ElementNames.Roles);
            if (roles == null)
            {
                #region Create roles base on build-in roles from resource
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configString);
                roles = config.OwnerDocument.CreateElement(ElementNames.Roles);
                roles.InnerXml = doc.DocumentElement.SelectSingleNode(ElementNames.Roles).InnerXml;
                config.AppendChild(roles);
                /* Write the build-in roles into DB. */
                PutConfig(config);
                #endregion

                /* Assign writer and QC roles for all operators. */
                AssignInitialRolesForAllOperators();
            }
            /* Save to local file for offline. */
            Globals.myConfig.SetRoles(roles);
        }

        public static bool IsUpperDoctor(string operatorID, string chargingDoctorID)
        {
            if (Globals.auditLevelSystem == AuditLevelSystem.GroupHeader)
                return Globals.doctors.GetGroupHeader(chargingDoctorID) == operatorID;
            else
                return Globals.doctors.GetTitleLevel(operatorID) == TitleLevel.AttendingDoctor;
        }
        public static bool IsUpperUpperDoctor(string operatorID, string chargingDoctorID)
        {
            if (Globals.auditLevelSystem == AuditLevelSystem.GroupHeader)
            {
                string upperDoctorID = Globals.doctors.GetGroupHeader(chargingDoctorID);
                if (upperDoctorID == null) return false;
                return Globals.doctors.GetGroupHeader(upperDoctorID) == operatorID;
            }
            else
            {
                return (Globals.doctors.GetTitleLevel(operatorID) == TitleLevel.ViceChiefDoctor) ||
                      (Globals.doctors.GetTitleLevel(operatorID) == TitleLevel.ChiefDoctor);
            }

        }
        public static bool HasNotesInLocal(string registryID, ref DateTime lwt)
        {
            string emrdocFile = udt.MakeEmrDocumentFileName(registryID, Globals.workFolder);
            if (!File.Exists(emrdocFile)) return false;

            //string tmpfile = Path.Combine(currentDirectory, EmrConstant.ResourceName.Mytmp);
            string tmpfile = emrdocFile.Substring(0, emrdocFile.Length - 1);
            udt.jj.DecodeEmrDocument(emrdocFile, tmpfile);
            try
            {
                XmlDocument emrdoc = new XmlDocument();
                emrdoc.Load(tmpfile);
                XmlNodeList notes = emrdoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
                foreach (XmlNode note in notes)
                {
                    int series = Convert.ToInt32(note.Attributes[AttributeNames.Series].Value);
                    string noteID = note.Attributes[AttributeNames.NoteID].Value;
                    string noteWdDocName = udt.MakeWdDocumentFileName(registryID, noteID, series, Globals.workFolder);
                    if (!File.Exists(noteWdDocName)) return false;
                }
                if (emrdoc.DocumentElement.Attributes[AttributeNames.LastWriteTime] == null)
                {
                    lwt = DateTime.FromFileTime(0);
                }
                else
                {
                    long lastWriteTime =
                        Convert.ToInt64(emrdoc.DocumentElement.Attributes[AttributeNames.LastWriteTime].Value);
                    lwt = DateTime.FromFileTime(lastWriteTime);
                }
                File.Delete(tmpfile);
                return true;
            }
            catch (XmlException ex)
            {
                Globals.logAdapter.Record("EX99875315946", ex.Message + ">>" + ex.ToString(), true);            
             
                File.Delete(tmpfile);
                return false;
            }
        }
        public static string SynchronizeOne(string registryID, string archiveNum, XmlNode emrRemote)
        {
            /* Get local emr document. */
            string emrdocFile = udt.MakeEmrDocumentFileName(registryID, Globals.workFolder);
            XmlDocument emrdoc = new XmlDocument();
            string tmpfile = emrdocFile + "t";
            udt.jj.DecodeEmrDocument(emrdocFile, tmpfile);
            emrdoc.Load(tmpfile);
            /* Create local emr notes for loading word documents that are new or have been updated. */
            XmlDocument notesNew = new XmlDocument();
            notesNew.LoadXml("<" + ElementNames.EmrNotes + "/>");
            notesNew.DocumentElement.SetAttribute(AttributeNames.RegistryID, registryID);
            notesNew.DocumentElement.SetAttribute(AttributeNames.ArchiveNum, archiveNum);
            foreach (XmlNode note in emrdoc.DocumentElement.SelectNodes(ElementNames.EmrNote))
            {
                long lwtLocal = Convert.ToInt64(note.Attributes[AttributeNames.LastWriteTime].Value);
                int series = Convert.ToInt32(note.Attributes[AttributeNames.Series].Value);
                if (IsNewerThanRemote(lwtLocal, series, emrRemote))
                {
                    string noteID = note.Attributes[EmrConstant.AttributeNames.NoteID].Value;
                    string noteWdDocName = udt.MakeWdDocumentFileName(registryID, noteID, series, Globals.workFolder);
                    string noteIDSeries = udt.MakeNoteIDSeries(noteID, series);
                    XmlElement noteElement = notesNew.CreateElement(ElementNames.EmrNote);
                    noteElement.InnerText = udt.jj.WordDocumentToString(noteWdDocName);
                    noteElement.SetAttribute(AttributeNames.NoteIDSeries, noteIDSeries);
                    notesNew.DocumentElement.AppendChild(noteElement);
                }
            }
            /* Update database */
            //if (notesNew.DocumentElement.ChildNodes.Count == 0) return null;
            //int status = Convert.ToInt32(emrdoc.DocumentElement.Attributes[AttributeNames.EmrStatus].Value);
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                File.Delete(tmpfile);
                return es.CjjSyncEmrDocument(emrdoc.DocumentElement, notesNew.DocumentElement);
            }
        }
        private static bool IsNewerThanRemote(long lwtLocal, int series, XmlNode emrRemote)
        {
            if (emrRemote == null) return true;
            XmlNodeList notesRemote = emrRemote.SelectNodes(ElementNames.EmrNote);
            if (notesRemote.Count == 0) return true;

            foreach (XmlNode noteRemote in notesRemote)
            {
                int seriesRemote = Convert.ToInt32(noteRemote.Attributes[AttributeNames.Series].Value);
                if (series == seriesRemote)
                {
                    long lwtRemote = Convert.ToInt64(noteRemote.Attributes[AttributeNames.LastWriteTime].Value);
                    if (lwtLocal > lwtRemote) return true;
                    else return false;
                }
            }
            return true;
        }
        public static string GetDocLocation(string registryID)
        {
            string location = Path.Combine(Globals.workFolder, registryID);
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
            return location;
        }
        public static void ChangeDepartment(string operatorDepartmentCode)
        {
            if (operatorDepartmentCode == Globals.DepartID) return;
            Globals.DepartID = operatorDepartmentCode;
            Globals.DepartName = Globals.departments.GetDepartmentName(Globals.DepartID);
            Globals.myConfig.SetDepartmentCode(operatorDepartmentCode);
            Globals.myConfig.SetDepartmentName(Globals.DepartName);

            //SetMainTitle(StyleName() + "：");
            //SetApplicationTitle("");

            //xmlBlocksWriter();
            //emrTemplate.LoadDepartmentTemplate();
            //emrTemplate.LoadHospitalTemplate();
            //emrPhraseUse.LoadDepartmentPhrases();
            //xmlPicGalleryWriter();
        }

        //Kill所有winword.exe进程
        public static void killWordProcess()
        {
            IntPtr WinHandler = IntPtr.Zero;
            WinHandler = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, null);
            while (true)
            {
                WinHandler = Win32.FindWindowEx(IntPtr.Zero, WinHandler, "OpusApp", null);
                if (WinHandler == IntPtr.Zero) break;

                StringBuilder buffer = new StringBuilder(1024);
                Win32.GetWindowText(WinHandler, buffer, 1024);

                if (buffer.ToString().Trim().StartsWith("病历记录"))//用到的Word文档标题
                {
                    MessageBox.Show("最近一次文档操作出现无法处理的错误，即将关闭所有Word文档，若其中包含您的个人文档，请保存并关闭后单击确定按钮。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    break;
                }
            }

            Process[] process;
            process = Process.GetProcesses();
            foreach (Process p in process)
            {
                try
                {
                    if (p.Id != 0 && p.Modules != null && p.Modules.Count > 0)
                    {
                        System.Diagnostics.ProcessModule pm = p.Modules[0];
                        if (pm.ModuleName.ToLower() == "winword.exe")
                        {
                            p.Kill();
                            break;
                        }
                    }
                }
                catch { }
            }
        }



        public static Boolean GetQualityInfo(XmlNode inPatients, ref XmlNode quantityInfo, string startDate, string endDate)
        {
            DateTime dtStart = DateTime.MinValue;
            int illDegree = 0;
            double dbLimit = 0;
            XmlNode degreeOrders = null;
            XmlNode operations = null;
            XmlNode rescues = null;
            XmlNode congeners = null;
            XmlNode outEmrNote = null;
            //string writenTime = null;
            //string commitTime = "";

            /* Find from emrPattern those emr notes that has required in time */
            XmlNodeList patternNotes = Globals.emrPattern.GetNotesRequiredInTime();
            //if (patternNotes == null) return Return.Failed;
            //刘伟加入评分子节点
            XmlDocument doca = new XmlDocument();
            doca.LoadXml(patternNotes.Item(0).OwnerDocument.InnerXml);
            patternNotes = Globals.childPattern.GetNotesRequiredInTime();
            XmlDocument docb = new XmlDocument();
            docb.LoadXml(patternNotes.Item(0).OwnerDocument.InnerXml);
            XmlElement rootA = doca.DocumentElement;
            XmlElement rootB = docb.DocumentElement;
            XmlDocument result = new XmlDocument();
            XmlElement root = result.CreateElement("root");
            result.AppendChild(root);
            foreach (XmlNode node in rootA.ChildNodes)
            {
                XmlNode n = result.ImportNode(node, true);
                root.AppendChild(n);
            }
            foreach (XmlNode node in rootB.ChildNodes)
            {
                XmlNode n = result.ImportNode(node, true);
                root.AppendChild(n);
            }

            patternNotes = result.DocumentElement.SelectNodes(ElementNames.EmrNote);
            XmlDocument doc = new XmlDocument();
            XmlNode emrInDatabase = doc.CreateElement(ElementNames.Emr);
            XmlNode infoTmp = doc.CreateElement(ElementNames.QualityInfo);

            XmlNodeList patients = inPatients.SelectNodes(ElementNames.Patient);
            foreach (XmlNode patient in patients)
            {
                #region Get emr notes in emrDocument of the patient.
                string registryID = GetInHospitalRegistryID(patient);
                if (registryID == StringGeneral.NullCode) continue;
                emrInDatabase = GetEmrDocumentForOneRegistry(registryID);
                if (emrInDatabase == null) continue;
                XmlNodeList emrNotes = emrInDatabase.SelectNodes(ElementNames.EmrNote);
                #endregion

                #region Create out result for one patient.
                XmlElement outPatient = doc.CreateElement(ElementNames.Patient);
                outPatient.SetAttribute(AttributeNames.RegistryID, registryID);
                outPatient.SetAttribute(AttributeNames.PatientName, patient.Attributes[AttributeNames.PatientName].Value);
                string registryDate = patient.ChildNodes[0].Attributes[AttributeNames.RegistryDate].Value;

                if (startDate != null)
                {
                    if ((Convert.ToDateTime(registryDate) < Convert.ToDateTime(startDate)) || (Convert.ToDateTime(registryDate) > Convert.ToDateTime(endDate)))
                        continue;
                }
                foreach (XmlNode patternNote in patternNotes)
                {
                    string noteID = null;
                    if (patternNote.Attributes["ParentID"] != null) noteID = patternNote.Attributes[AttributeNames.ParentID].Value + ":" + patternNote.Attributes[AttributeNames.NoteID].Value;
                    else noteID = patternNote.Attributes[AttributeNames.NoteID].Value;
                    //string noteID = patternNote.Attributes[AttributeNames.NoteID].Value;
                    string stime = patternNote.Attributes[AttributeNames.StartTime].Value;
                    string secondtime = "";
                    string writenTime = null;
                    string commitTime = "";
                    if (patternNote.Attributes[AttributeNames.SecondTime] != null)
                        secondtime = patternNote.Attributes[AttributeNames.SecondTime].Value;
                    switch (stime)
                    {
                        case EmrConstant.StartTime.Routine:
                            #region Routine
                            if (!DegreeOrderTime(registryID, ref degreeOrders)) continue;
                            if (degreeOrders == null) continue;
                            if (degreeOrders.ChildNodes.Count == 0) continue;

                            foreach (XmlNode degreeOrder in degreeOrders.ChildNodes)
                            {
                                dtStart = Convert.ToDateTime(degreeOrder.Attributes[AttributeNames.StartTime].Value);
                                illDegree = Convert.ToInt16(degreeOrder.Attributes[AttributeNames.Critical].Value);                        
                                dbLimit = TimeOutForDegree(patternNote, illDegree);
                                congeners = GetCongener(noteID, dtStart, emrNotes);

                                foreach (XmlNode congener in congeners.ChildNodes)
                                {
                                    writenTime = congener.Attributes[AttributeNames.WrittenDate].Value +
                                        " " + congener.Attributes[AttributeNames.WrittenTime].Value;
                                    if (congener.Attributes[AttributeNames.CommitDate] != null)
                                    {
                                        commitTime = congener.Attributes[AttributeNames.CommitDate].Value +
                                                                                " " + congener.Attributes[AttributeNames.CommitTime].Value;
                                    }
                                    outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);
                                    SetScore(outEmrNote, writenTime, dtStart, dbLimit, commitTime);
                                    outPatient.AppendChild(outEmrNote);
                                    dtStart = Convert.ToDateTime(writenTime);
                                }
                            }
                            /* The Next note will be prompted */
                            XmlNode outEmrNote2 = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);
                            outPatient.AppendChild(outEmrNote2);
                            #endregion
                            break;
                        case EmrConstant.StartTime.Monthly:
                            #region Monthly
                            dtStart = StartTime(noteID, EmrConstant.StartTime.Registry, registryID);
                            if (dtStart == DateTime.MinValue) continue;
                            dbLimit = 720;
                            congeners = GetCongener0(noteID, emrNotes);
                            if (congeners.ChildNodes.Count == 0)
                            {
                                outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);
                                outPatient.AppendChild(outEmrNote);
                            }
                            else
                            {
                                foreach (XmlNode congener in congeners.ChildNodes)
                                {
                                    writenTime = congener.Attributes[AttributeNames.WrittenDate].Value +
                                        " " + congener.Attributes[AttributeNames.WrittenTime].Value;
                                    if (congener.Attributes[AttributeNames.CommitDate] != null)
                                    {
                                        commitTime = congener.Attributes[AttributeNames.CommitDate].Value +
                                                                                " " + congener.Attributes[AttributeNames.CommitTime].Value;
                                    }
                                    outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);

                                    SetScore(outEmrNote, writenTime, dtStart, dbLimit, commitTime);

                                    outPatient.AppendChild(outEmrNote);

                                    dtStart = Convert.ToDateTime(writenTime);
                                }
                                /* The next note will be prompted */
                                outEmrNote2 = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);
                                outPatient.AppendChild(outEmrNote2);
                            }
                            #endregion
                            break;
                        case EmrConstant.StartTime.Operation:
                            #region Operation
                            /* Get operaton info from HIS. */
                            if (operations != null)
                            {
                                operations.RemoveAll();
                                operations = null;
                            }
                            OperationTime(registryID, ref operations);
                            if (operations == null) continue;
                            /* Get the value of timeout for this emr note. */
                            dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.TimeLimit].Value);
                            if (dbLimit == 0.0) continue;
                            /* Get all emr notes you have written for this noteID */
                            congeners = GetCongener0(noteID, emrNotes);

                            foreach (XmlNode operation in operations.ChildNodes)
                            {

                                dtStart = Convert.ToDateTime(operation.Attributes[AttributeNames.DateTime].Value);

                                string sequence = operation.Attributes[AttributeNames.Sequence].Value;
                                outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);

                                writenTime = NoteExistsWithSequence(AttributeNames.Sequence, sequence, congeners, ref commitTime);
                                if (writenTime != null) SetScore(outEmrNote, writenTime, dtStart, dbLimit, commitTime);

                                outPatient.AppendChild(outEmrNote);

                            }
                            #endregion
                            break;
                        case EmrConstant.StartTime.Rescued:
                            #region Rescued
                            /* Get rescue info from HIS. */
                            RescueTime(registryID, ref rescues);
                            if (rescues == null) continue;
                            /* Get the value of timeout for this emr note. */
                            dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.TimeLimit].Value);
                            /* Get all emr notes you have written for this noteID */
                            congeners = GetCongener0(noteID, emrNotes);

                            foreach (XmlNode rescue in rescues.ChildNodes)
                            {
                                dtStart = Convert.ToDateTime(rescue.Attributes[AttributeNames.DateTime].Value);
                                string sequence = "";
                                if (rescue.Attributes[AttributeNames.RescueSequence] != null)
                                {

                                    sequence = rescue.Attributes[AttributeNames.RescueSequence].Value;
                                }
                                outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);

                                writenTime = NoteExistsWithSequence(AttributeNames.RescueSequence, sequence, congeners, ref commitTime);
                                if (writenTime != null) SetScore(outEmrNote, writenTime, dtStart, dbLimit, commitTime);

                                outPatient.AppendChild(outEmrNote);

                            }
                            #endregion
                            break;
                        case EmrConstant.StartTime.TransferIn:
                            #region Transfer in
                            /* Get transferin info from HIS. */
                            TransferInTime(registryID, ref rescues);
                            if (rescues == null) continue;
                            /* Get the value of timeout for this emr note. */
                            dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.TimeLimit].Value);
                            /* Get all emr notes you have written for this noteID */
                            congeners = GetCongener0(noteID, emrNotes);

                            foreach (XmlNode rescue in rescues.ChildNodes)
                            {
                                dtStart = Convert.ToDateTime(rescue.Attributes[AttributeNames.DateTime].Value);
                                string sequence = rescue.Attributes[AttributeNames.TransferInSequence].Value;
                                outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);

                                writenTime = NoteExistsWithSequence(AttributeNames.TransferInSequence, sequence, congeners, ref commitTime);
                                if (writenTime != null) SetScore(outEmrNote, writenTime, dtStart, dbLimit, commitTime);

                                outPatient.AppendChild(outEmrNote);

                            }
                            #endregion
                            break;
                        case EmrConstant.StartTime.TakeOver:
                            #region Take over
                            /* Get transferin info from HIS. */
                            TakeOverTime(registryID, ref rescues);
                            if (rescues == null) continue;
                            /* Get the value of timeout for this emr note. */
                            dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.TimeLimit].Value);
                            /* Get all emr notes you have written for this noteID */
                            congeners = GetCongener0(noteID, emrNotes);

                            foreach (XmlNode rescue in rescues.ChildNodes)
                            {
                                dtStart = Convert.ToDateTime(rescue.Attributes[AttributeNames.DateTime].Value);
                                string sequence = rescue.Attributes[AttributeNames.TakeOverSequence].Value;
                                outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);

                                writenTime = NoteExistsWithSequence(AttributeNames.TakeOverSequence, sequence, congeners, ref commitTime);
                                if (writenTime != null) SetScore(outEmrNote, writenTime, dtStart, dbLimit, commitTime);

                                outPatient.AppendChild(outEmrNote);

                            }
                            #endregion
                            break;
                        case EmrConstant.StartTime.Dead:

                             dtStart = StartTime(noteID, stime, registryID);
                            if (dtStart == DateTime.MinValue) continue;
                            dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.TimeLimit].Value);

                            /* Create base out result for one emrNote */
                            outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);
                            /* Find emrNote */
                            string substituteNoteNamed = null;
                            string writenTimesd = null;
                            //string[] commitTimesd = new string[100];
                            //string[] secondTimesd = new string[2];
                            int countd = 0;
                            writenTimesd = NoteExitsEx(noteID, emrNotes, outEmrNote, ref dbLimit, ref substituteNoteNamed);
                            bool findOutd = false;
                            if (substituteNoteNamed != null)
                            {
                                foreach (XmlNode outen in outPatient.ChildNodes)
                                {
                                    if (outen.Attributes[AttributeNames.NoteName].Value == substituteNoteNamed)
                                    {
                                        findOutd = true;
                                        break;
                                    }
                                }
                            }
                            /* Score emrNote */
                           
                            SetScore(outEmrNote, writenTimesd, dtStart, dbLimit);
                              
                         
                            if (!findOutd) outPatient.AppendChild(outEmrNote);
                            break;
                        case EmrConstant.StartTime.Registry:
                            #region Single
                            dtStart = StartTime(noteID, stime, registryID);
                            if (dtStart == DateTime.MinValue) continue;

                            dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.TimeLimit].Value);

                            /* Create base out result for one emrNote */
                            outEmrNote = SetBaseOutResult(doc, patternNote, dtStart, dbLimit);

                            /* Find emrNote */
                            string substituteNoteName = null;
                            //string[] writenTimes = null;
                            //string[] commitTimes = new string[100];
                            //string[] secondTimes = new string[2];
                            //secondTimes[0] = secondtime;
                            //int count = 0;
                            writenTime = NoteExitsEx(noteID, emrNotes, outEmrNote, ref dbLimit, ref substituteNoteName);
                            bool findOut = false;
                            if (substituteNoteName != null)
                            {
                                foreach (XmlNode outen in outPatient.ChildNodes)
                                {
                                    if (outen.Attributes[AttributeNames.NoteName].Value == substituteNoteName)
                                    {
                                        findOut = true;
                                        break;
                                    }
                                }
                            }
                            XmlNode outEmrNote3 = null;
                            /* Score emrNote */
                            //if (writenTime != null)
                            //{
                            //    if (writenTimes[0] != null)
                            //    {
                            //        if (commitTimes!=null && commitTimes[0] != null)
                            //            commitTime = commitTimes[0];

                                    SetScore(outEmrNote, writenTime, dtStart, dbLimit);
                                //}
                                //if (count > 1 && secondTimes[0].Trim() != "")
                                //{
                                //    if (commitTimes!=null&&commitTimes[count - 1] != null)
                                //        commitTime = commitTimes[count - 1];
                                //    double db = Convert.ToInt32(secondTimes[0]) * 24;

                                //    outEmrNote3 = SetBaseOutResult(doc, patternNote, Convert.ToDateTime(secondTimes[1]), db);

                                //    SetScoreEx(outEmrNote3, writenTimes[count - 1], secondTimes, commitTime);

                                //}
                            //}
                            //  if (writenTime != null) SetScore(outEmrNote, writenTime, dtStart, dbLimit, commitTime);
                            if (!findOut)
                            {
                                outPatient.AppendChild(outEmrNote);
                                //if (outEmrNote3 != null) outPatient.AppendChild(outEmrNote3);
                            }
                            #endregion
                            break;
                        default:
                         
                            break;
                    }
                }
                #endregion

                infoTmp.AppendChild(outPatient);
            }
            quantityInfo = infoTmp.Clone();
            return EmrConstant.Return.Successful;
        }

        private static void SetScoreEx(XmlNode outEmrNote, string writenTime, string[] secondTime, string commitTime)
        {
            // outEmrNote.Attributes[AttributeNames.TimeLimit].Value = dbLimit.ToString();
            outEmrNote.Attributes[AttributeNames.WrittenTime].Value = writenTime;
            outEmrNote.Attributes[AttributeNames.CommitTime].Value = commitTime;

            if (commitTime != "")
            {
                //    DateTime dtWritenTime = Convert.ToDateTime(commitTime);
                //    TimeSpan tsLimit = dtWritenTime.Subtract(dtStart);
                //    if (tsLimit.TotalHours <= dbLimit)
                //        outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompletedOntime;
                //    else
                //        outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompleted;

                if (secondTime != null && secondTime[1] != "")
                {
                    DateTime dtWritenTime = Convert.ToDateTime(commitTime);
                    DateTime dtsecondTime = Convert.ToDateTime(secondTime[1]);
                    TimeSpan tsLimit = dtWritenTime.Subtract(dtsecondTime);
                    int sc = Convert.ToInt32(secondTime[0]);
                    if (tsLimit.TotalHours <= sc * 24)
                        outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompletedOntime;
                    else
                        outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompleted;

                }
            }
        }
        private static void SetScore(XmlNode outEmrNote, string writenTime, DateTime dtStart, double dbLimit, string commitTime)
        {
            outEmrNote.Attributes[AttributeNames.TimeLimit].Value = dbLimit.ToString();
            outEmrNote.Attributes[AttributeNames.WrittenTime].Value = writenTime;
            outEmrNote.Attributes[AttributeNames.CommitTime].Value = commitTime;
            if (dbLimit == 0.0)
            {
                outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompletedOntime;
                return;
            }
            if (commitTime != "")
            {
                DateTime dtWritenTime = Convert.ToDateTime(commitTime);
                TimeSpan tsLimit = dtWritenTime.Subtract(dtStart);
                if (tsLimit.TotalHours <= dbLimit)
                    outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompletedOntime;
                else
                    outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompleted;
            }
            else 
            {
                DateTime dtWritenTime = Convert.ToDateTime(writenTime);
                TimeSpan tsLimit = dtWritenTime.Subtract(dtStart);
                if (tsLimit.TotalHours <= dbLimit)
                    outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompletedOntime;
                else
                    outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompleted;
     
            }
        }
        private static string CompletedTimeEx(string noteID, XmlNodeList emrNotes)
        {

            //string[] writenTime = new string[100];
            string writenTime = null;
            string noteID_left = null;
            //string commitDates = "";
            int num = 0;

            if (noteID.IndexOf("-") > 0) noteID = noteID.Replace("-", ":");
            foreach (XmlNode emrNote in emrNotes)
            {
                string status = emrNote.Attributes[AttributeNames.NoteStatus].Value;
                if (status != "0")
                {
                    if (emrNote.Attributes[AttributeNames.ChildID] == null || emrNote.Attributes[AttributeNames.ChildID].Value == "" ||    //刘伟20111114修改获得noteID的错误
                        emrNote.Attributes[AttributeNames.ChildID].Value == "0" ||
                        emrNote.Attributes[AttributeNames.ChildID].Value == emrNote.Attributes[AttributeNames.NoteID].Value)
                        noteID_left = emrNote.Attributes[AttributeNames.NoteID].Value;
                    else noteID_left = emrNote.Attributes[AttributeNames.NoteID].Value + ":" + emrNote.Attributes[AttributeNames.ChildID].Value;
                    if (noteID_left == noteID)
                    {

                        writenTime = emrNote.Attributes[AttributeNames.WrittenDate].Value + Delimiters.Space +
                            emrNote.Attributes[AttributeNames.WrittenTime].Value;

                        //if (emrNote.Attributes[AttributeNames.CommitDate] != null)
                        //{
                        //    // if (num == 1)
                        //    //{
                        //    commitDate[num] = emrNote.Attributes[AttributeNames.CommitDate].Value + Delimiters.Space +
                        //      emrNote.Attributes[AttributeNames.CommitTime].Value;
                        //    //  commitDates = emrNote.Attributes[AttributeNames.CommitDate].Value;
                        //    // }
                        //    // else
                        //    // {
                        //    if (secondTime != null)
                        //    {
                        //        if (secondTime[0].Trim() != "" && num != 0)
                        //        {
                        //            secondTime[1] = commitDate[num - 1];
                        //        }
                        //    }
                        //    //commitDate[1] = emrNote.Attributes[AttributeNames.CommitDate].Value + Delimiters.Space +
                        //    // emrNote.Attributes[AttributeNames.CommitTime].Value;

                        //    //}
                        //}
                        //else
                        //{
                        //    commitDate = null;
                        //}

                        //num++;
                    }
                }
            }
            //count = num;
           // if (writenTime[0] == null) return null;
            return writenTime;
        }
        private static string NoteExitsEx(string noteID, XmlNodeList emrNotes, XmlNode outEmrNote,
        ref double dbLimit, ref string substituteNoteName, ref string[] commitDate, ref string[] secondTime, ref int count)
        {
            string[] noteIDs = null;

            string writenTimes = CompletedTimeEx(noteID, emrNotes);
            if (writenTimes != null) return writenTimes;

            string substitutes = Globals.emrPattern.GetSubstitutes(noteID);
            if (substitutes == null) return null;
            if (substitutes.IndexOf("|") > 0)
                noteIDs = substitutes.Split(Delimiters.Seperator);
            else
                noteIDs = substitutes.Split(Delimiters.Colon);
            for (int k = 0; k < noteIDs.Length; k++)
            {
                writenTimes = CompletedTimeEx(noteIDs[k], emrNotes);
                if (writenTimes != null)
                {
                    if (noteIDs[k].IndexOf("-") > 0)
                    {
                        substituteNoteName = Globals.childPattern.GetNoteNameFromNoteIDEX(noteIDs[k]);
                        outEmrNote.Attributes[AttributeNames.NoteName].Value = substituteNoteName;
                        dbLimit = udt.jj.MyToDoubleZero(Globals.childPattern.TimeLimitAttribute(noteIDs[k]));
                    }
                    else
                    {
                        substituteNoteName = Globals.emrPattern.GetNoteNameFromNoteIDEX(noteIDs[k]);
                        outEmrNote.Attributes[AttributeNames.NoteName].Value = substituteNoteName;
                        dbLimit = udt.jj.MyToDoubleZero(Globals.emrPattern.TimeLimitAttribute(noteIDs[k]));
                    }



                    break;
                }
            }

            return writenTimes;
        }
        
    }
}