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

namespace EMR
{
 
        public partial class ThisAddIn
        {
            
            private static int GetAuditSystemLevel()
            {
                return Convert.ToInt32(Globals.auditSystem.Substring(1));
            }
             private static string CompletedTime(string noteID, XmlNodeList emrNotes)
            {
                string writenTime = null;

                foreach (XmlNode emrNote in emrNotes)
                {
                    if (emrNote.Attributes[AttributeNames.NoteID].Value == noteID)
                    {
                        writenTime = emrNote.Attributes[AttributeNames.WrittenDate].Value + Delimiters.Space +
                            emrNote.Attributes[AttributeNames.WrittenTime].Value;
                        switch (GetAuditSystemLevel())
                        {
                            case 1:
                                break;
                            case 2:
                                if (emrNote.Attributes[AttributeNames.Sign2] != null)
                                    writenTime = emrNote.Attributes[AttributeNames.CheckedDate].Value;
                                break;
                            case 3:
                                if (emrNote.Attributes[AttributeNames.Sign1] != null)
                                {
                                    writenTime = emrNote.Attributes[AttributeNames.FinalCheckedDate].Value;
                                    break;
                                }
                                if (emrNote.Attributes[AttributeNames.Sign2] != null)
                                    writenTime = emrNote.Attributes[AttributeNames.CheckedDate].Value;
                                break;
                        }
                        break;
                    }
                }
                if (writenTime == string.Empty) return null;
                return writenTime;
            }
         private static string NoteExitsEx(string noteID, XmlNodeList emrNotes, XmlNode outEmrNote,
         ref double dbLimit, ref string substituteNoteName)
            {
                string writenTime = CompletedTime(noteID, emrNotes);
                if (writenTime != null) return writenTime;

                string substitutes =Globals.emrPattern.GetSubstitutes(noteID);
                if (substitutes == null) return null;
                string[] noteIDs = substitutes.Split(Delimiters.Colon);
                for (int k = 0; k < noteIDs.Length; k++)
                {
                    writenTime = CompletedTime(noteIDs[k], emrNotes);
                    if (writenTime != null)
                    {
                        substituteNoteName = Globals.emrPattern.GetNoteNameFromNoteID(noteIDs[k]);
                        outEmrNote.Attributes[AttributeNames.NoteName].Value = substituteNoteName;
                        dbLimit = udt.jj.MyToDoubleZero(Globals.emrPattern.TimeLimitAttribute(noteIDs[k]));

                        break;
                    }
                }

                return writenTime;
            }
             private static XmlNode SetBaseOutResult(XmlDocument doc, XmlNode emrNotePattern, DateTime dtStart, double dbLimit)
            {
                XmlElement outEmrNote = doc.CreateElement(ElementNames.EmrNote);
                outEmrNote.SetAttribute(AttributeNames.NoteName, emrNotePattern.Attributes[AttributeNames.NoteName].Value);
                string sdate = dtStart.ToString(StringGeneral.DateFormat);
                string stime = dtStart.ToShortTimeString();
                outEmrNote.SetAttribute(AttributeNames.StartTime, sdate + " " + stime);
                outEmrNote.SetAttribute(AttributeNames.WrittenTime, StringGeneral.NullCode);
              //  outEmrNote.SetAttribute(AttributeNames.CommitTime, StringGeneral.NullCode);
                outEmrNote.SetAttribute(AttributeNames.TimeLimit, dbLimit.ToString());
                outEmrNote.SetAttribute(AttributeNames.Score, StringGeneral.EmrNotCompleted);
                outEmrNote.SetAttribute(AttributeNames.RestTime, "0");

               
                return outEmrNote;
            }

            private static void SetScore(XmlNode outEmrNote, string writenTime, DateTime dtStart, double dbLimit)
            {
                outEmrNote.Attributes[AttributeNames.TimeLimit].Value = dbLimit.ToString();
                outEmrNote.Attributes[AttributeNames.WrittenTime].Value = writenTime;
                if (dbLimit == 0.0)
                {
                    outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompletedOntime;
                    return;
                }
                     double dbRest = 0;
                if (writenTime != null && writenTime.Trim() != "")
                {
                    DateTime dtWritenTime = Convert.ToDateTime(writenTime);
                    TimeSpan tsLimit = dtWritenTime.Subtract(dtStart);
                    if (tsLimit.TotalHours <= dbLimit)
                        outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompletedOntime;
                    else
                        outEmrNote.Attributes[AttributeNames.Score].Value = StringGeneral.EmrCompleted;
          
                    TimeSpan wentby = Convert.ToDateTime(writenTime).Subtract(dtStart);
                    dbRest = wentby.TotalHours - dbLimit;

                }
                else
                {
                    TimeSpan wentby = Today().Subtract(dtStart);
                    dbRest = wentby.TotalHours - dbLimit;
                }
                if (dbRest > 0)
                {
                    int rest = (int)(dbRest * 1);
                   
                    outEmrNote.Attributes[AttributeNames.RestTime].Value = rest.ToString();
                }
                //else
                //{
                //    outEmrNote.SetAttribute(AttributeNames.RestTime, "0");
                //}
            }
            private static string GetInHospitalRegistryID(XmlNode patient)
            {
                for (int i = 0; i < patient.ChildNodes.Count; i++)
                {
                    if (patient.ChildNodes[i].Attributes[AttributeNames.BedNum].Value != StringGeneral.NullCode)
                        return patient.ChildNodes[i].Attributes[AttributeNames.RegistryID].Value;
                }
                return null;
            }
            private static double TimeOutForDegree(XmlNode patternNote, int degree)
            {
                double dbLimit = 0;
                IllDegree illDegree = (IllDegree)degree;
                switch (illDegree)
                {
                    case IllDegree.Normal:
                        dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.Normal].Value);
                        break;
                    case IllDegree.Chronic:
                        dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.Chronic].Value);
                        break;
                    case IllDegree.Bad:
                        dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.Bad].Value);
                        break;
                    case IllDegree.Critical:
                        dbLimit = Convert.ToDouble(patternNote.Attributes[AttributeNames.Critical].Value);
                        break;
                }

                return dbLimit;
            }
            private static XmlNode GetCongener(string noteID, DateTime dot, XmlNodeList emrNotes)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement congener = doc.CreateElement("Congener");
                foreach (XmlNode emrNote in emrNotes)
                {
                    if (emrNote.Attributes[AttributeNames.StartTime] == null) continue;
                    if (emrNote.Attributes[AttributeNames.NoteID].Value == noteID &&
                        Convert.ToDateTime(emrNote.Attributes[AttributeNames.WrittenDate].Value + " " + emrNote.Attributes[AttributeNames.WrittenTime].Value).Equals(dot))
                    //emrNote.Attributes[AttributeNames.Start].Value
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

            public static void GetLocalOptionValues()
            {
                if (!inStyle)
                {
                    Globals.outDefaultFirstVisitNoteID = Globals.myConfig.GetVisitNoteID(ElementNames.FirstVisitNoteID);
                    Globals.outDefaultReturnVisitNoteID = Globals.myConfig.GetVisitNoteID(ElementNames.ReturnVisitNoteID);
                    Globals.cardUseWay = Globals.myConfig.GetCardUseWay();
                    Globals.defaultPriceUnitMode = (PriceUnitMode)Globals.myConfig.GetDrugUnitMode();
                    Globals.mustByPoorPatient = Globals.myConfig.GetMustByPoorPatient();
                    Globals.limitInDays = Globals.myConfig.GetLimitDays();
                    //myConfig.GetPageMargin(ref leftMargin, ref topMargin, ref rightMargin, ref bottomMargin);
                }
            }
            public static void GetIndividualOptions()
            {
                Globals.showOpDoneTime = Globals.myConfig.GetShowTime(Globals.DoctorID, ElementNames.ShowOpdownTime, Globals.showOpDoneTime);
                Globals.showPatientInfoTime = Globals.myConfig.GetShowTime(Globals.DoctorID, ElementNames.ShowPatientInfoTime, Globals.showPatientInfoTime);
                Globals.timeout = Globals.myConfig.GetShowTime(Globals.DoctorID, ElementNames.Timeout, Globals.timeout);
                //Globals.qualityInfo = Globals.myConfig.GetShowQCI(Globals.DoctorID);
                Globals.expandPatientTree = Globals.myConfig.GetExpand(Globals.DoctorID);
                Globals.autoCheckSync = Globals.myConfig.GetAutoSyncCheck(Globals.DoctorID);
                Globals.mergeCode = Globals.myConfig.GetMergeCode(Globals.DoctorID);
                 //ThisAddIn.CanOption(ElementNames.EnterCommitTime) = Globals.myConfig.GetEnterCommitTime(Globals.DoctorID);
                //if (!inStyle) GetLocalOptionValues();

            }
            public static void GetOptionValues()
            {
                XmlNode options = null;
                if (Globals.offline)
                {
                    options = Globals.myConfig.GetOptions();
                    if (options == null) return;
                }
                else
                {
                    #region not offline
                    XmlNode config = GetConfig();
                    if (config == null) return;
                    options = config.SelectSingleNode(ElementNames.Options);
                    if (options == null) return;
                    /* Save to local file for offline. */
                    Globals.myConfig.SetOptions(options);
                    #endregion
                }
                if (OptionValue(options, ElementNames.OrderByMode))
                    orderByMode = gjtEmrPatients.OrderByMode.ByBed;
                else
                    orderByMode = gjtEmrPatients.OrderByMode.BySexAndName;
                 Globals.Sign = OptionValue(options, ElementNames.Sign);
                Globals.commitedAsEnd = OptionValue(options, ElementNames.CommitTime);
            }
            public static bool CanOption(string ElementNames)
            {
                return OptionValue(GetOps(), ElementNames);
            }
            public static string CanOptionText(string ElementNames)
            {
                return OptionValueTxt(GetOps(), ElementNames);
            }
            public static XmlNode GetOps()
            {
                XmlNode options = null;
                if (Globals.offline)
                {
                    options = Globals.myConfig.GetOptions();
                    if (options == null) return null;
                }
                else
                {
                    #region not offline
                    XmlNode config = GetConfig();
                    if (config == null) return null;
                    options = config.SelectSingleNode(ElementNames.Options);
                    if (options == null) return null;
                    /* Save to local file for offline. */
                    Globals.myConfig.SetOptions(options);
                    #endregion
                }
                return options;
            }
            private static string OptionValueTxt(XmlNode options, string optionName)
            {
                XmlNode option = options.SelectSingleNode(optionName);
                if (option == null) return "";
                return option.InnerText.Trim();
            }
            public static void Deputed()
            {
                Trust trust = new Trust(EmrConstant.Trust.Deputed);                
                trust.Location = new Point(100,180);
                trust.ShowDialog();                
            }
            public static void Deputing()
            {
                Trust trust = new Trust(EmrConstant.Trust.Deputing);
                trust.Location = new Point(100, 180);
                trust.ShowDialog();                
            }
            public static void InitForNonQC()
            {              
                /* Init auditSystem and auditLevelSystem */
                GetAuditSys();
                /* Lock emrDocuments for discharged patients */
                if (!Globals.offline && ThisAddIn.CanOption(ElementNames.AutoArchive)) Archive(Today(), null);
       
            }
            private static void GetAuditSys()
            {
                if (Globals.offline)
                {
                    #region Get audit system from local storage
                    string auditSys = Globals.myConfig.GetAuditSystem();
                    if (auditSys == null) return;
                    Globals.auditSystem = auditSys;
                    Globals.auditLevelSystem = (AuditLevelSystem)Convert.ToInt32(Globals.myConfig.GetAuditLevelSystem());
                    #endregion
                }
                else
                {
                    #region Get audit system from dadabase
                    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                    {
                        try
                        {
                            es.UseDefaultCredentials = true;
                            XmlNode root = null;
                            string msg = es.GetSecurityConfig(ref root);
                            if (msg == null)
                            {
                                XmlNode audit = root.SelectSingleNode(ElementNames.AuditSystem);
                                Globals.auditSystem = audit.Attributes[AttributeNames.Level].Value;
                                int auditMode = Convert.ToInt32(audit.Attributes[AttributeNames.Mode].Value);
                                Globals.auditLevelSystem = (AuditLevelSystem)auditMode;
                                Globals.myConfig.SetAuditSystem(Globals.auditSystem, audit.Attributes[AttributeNames.Mode].Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.logAdapter.Record("EX7569874517", ex.Message + ">>" + ex.ToString(), true);            
             
                        }
                    }
                    #endregion
                }

            }
            public static  bool ExamStatus(string name, string Status, string SignCount)
            {
                if (Status == "0")
                {
                    MessageBox.Show(name + " 为草稿状态，不能归档！", "提示:");
                    return false;
                }
                if (SignCount == "0") return true;
                if (Convert.ToInt32(Status) >= Convert.ToInt32(SignCount) * 2 - 1)
                    return true;
                else
                {
                    MessageBox.Show(name + " 需要" + SignCount + "级审核才能归档！", "提示:");
                    return false;
                }

            }
            public static void Archive_Single(DateTime endDischargedDate, String RegistryID, ListBox archiveList)
            {
                /* Get a set of registryIDs from local storage. */
                //XmlNode inRegistryIDs = GetInRegistryIDs();

                XmlDocument doc = new XmlDocument();
                XmlNode inRegistryIDs = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
                XmlElement inRegistryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                inRegistryID.InnerText = RegistryID;
                inRegistryIDs.AppendChild(inRegistryID);

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
                        Globals.logAdapter.Record("EX7569874508", ex.Message + ">>" + ex.ToString(), true);            
             
                        return;
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
                        Globals.logAdapter.Record("EX756987451988", ex.Message + ">>" + ex.ToString(), true);            
             
                        return;
                    }
                }
                /* Clear the loacl storage. */
                foreach (XmlNode outRegistryID in outRegistryIDs)
                {
                    Directory.Delete(Path.Combine(Globals.workFolder, outRegistryID.InnerText), true);
                    if (archiveList != null)
                    {
                        archiveList.Items.Remove(outRegistryID.InnerText);
                        archiveList.Update();
                    }
                }
            }
            public static void Archive_Single(DateTime endDischargedDate, String RegistryID)
            {
                /* Get a set of registryIDs from local storage. */
                //XmlNode inRegistryIDs = GetInRegistryIDs();

                XmlDocument doc = new XmlDocument();
                XmlNode inRegistryIDs = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
                XmlElement inRegistryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                inRegistryID.InnerText = RegistryID;
                inRegistryIDs.AppendChild(inRegistryID);
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
                        Globals.logAdapter.Record("EX7569871", ex.Message + ">>" + ex.ToString(), true);            
             
                        return;
                    }
                }
                /* No discharged patients */
                if (outRegistryIDs.ChildNodes.Count == 0)
                {
                    MessageBox.Show("只有出院患者才能归档！", "提示");
                    return;
                }
                /* Set lock flag of the emrDocuments for these patients. */
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        es.ArchiveBatch(outRegistryIDs);
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX7569874231", ex.Message + ">>" + ex.ToString(), true);            
             
                        return;
                    }
                }
                /* Clear the loacl storage. */
                foreach (XmlNode outRegistryID in outRegistryIDs)
                {
                    if (Directory.Exists(Path.Combine(Globals.workFolder, outRegistryID.InnerText)) == true)
                    {
                        Directory.Delete(Path.Combine(Globals.workFolder, outRegistryID.InnerText), true);
                        //if (archiveList != null)
                        //{
                        //    archiveList.Items.Remove(outRegistryID.InnerText);
                        //    archiveList.Update();
                        //}
                    }
                }

                MessageBox.Show("执行完成", "提示");

            }


            public static void Archive_Search(DateTime endDischargedDate, ListBox archiveList)
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
                        Globals.logAdapter.Record("EX756987457749", ex.Message + ">>" + ex.ToString(), true);            
             
                        return;
                    }
                }
                /* No discharged patients */
                if (outRegistryIDs.ChildNodes.Count == 0) return;

                /* Add to archiveList. */
                archiveList.Items.Clear();
                foreach (XmlNode outRegistryID in outRegistryIDs)
                {
                    if (archiveList != null)
                    {
                        archiveList.Items.Add(outRegistryID.InnerText);
                        archiveList.Update();
                    }
                }
            }
            
        }
    }

