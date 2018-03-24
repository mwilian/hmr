using System;
using System.Collections.Generic;

using System.Text;
using CommonLib;
using System.Xml;
using System.Windows.Forms;
using EmrConstant;
using System.Data;
using System.IO;


namespace EMR
{
   public partial class ThisAddIn
    {
        /* ------------------------------------------------------------------------------------
         * Get now time from the database server or local machine.
         * ------------------------------------------------------------------------------------ */
        public static DateTime Today()
        {
            if (Globals.offline)
            {
                return DateTime.Now;
            }
            else
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        return es.SysTime();
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX7853622963852", ex.Message + ">>" + ex.ToString(), true);            
             
                        return DateTime.Now;
                    }
                }
            }
        }
       //评分规则
        public static XmlNode GetRules()
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                XmlNode rules = null;
                string msg = es.GetNoteIDsWithValuateRules(ref rules);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                }
                return rules;
            }
        }
        public static void fanxiu(string registryID, int series)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                es.Setfanxiu(registryID, series);
            }
        }
        public static bool PutPattern(XmlNode pattern)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = es.AddEmrPattern(Globals.DoctorID, StringGeneral.NullCode, pattern);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
                pattern.OwnerDocument.Save(Globals.emrPatternFile);
                return true;
            }
        }      

        public static bool PutChildPattern(XmlNode pattern)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = es.AddEmrPattern(Globals.DoctorID, StringGeneral.ChildCode, pattern);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
                pattern.OwnerDocument.Save(Globals.emrPatternFile);
                return true;
            }
        }

        public static XmlNode GetConfig()
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                XmlNode config = null;
                string msg = es.GetSecurityConfig(ref config);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return null;
                }
                return config;
            }
        }

        public static bool PutConfig(XmlNode config)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                bool success = es.UpdateSecuriy(config);
                if (!success)
                {
                    MessageBox.Show(ErrorMessage.SystemError, ErrorMessage.Error);
                }
                return success;
            }
        }
        public static bool UnlockEmrDocument(string registryID, string patientName, int expireDays,
            bool forPublic, XmlNode reasion)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = es.UnlockEmrdocument(registryID, patientName,
                    Globals.DoctorID, Globals.DoctorName, expireDays, forPublic, reasion);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return Return.Failed;
                }
                return Return.Successful;
            }
        }
        public static void SetGrade(string registryID)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                es.SetGrade(registryID);
            }
        }
        private static void AssignInitialRolesForAllOperators()
        {
            XmlNode ops = null;
            string msg = null;
            #region Get operators
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                msg = ep.Getoperators(ref ops);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
            }
            #endregion

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {

                XmlNode roles = null;
                msg = es.GetRolesForOneOperator(StringGeneral.supperUser, ref roles);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
                /* If hi has been assigned as roles, initial can not be done again. */
                if (roles != null) return;

                foreach (XmlNode op in ops)
                {
                    XmlElement roleid0 = op.OwnerDocument.CreateElement(ElementNames.RoleID);
                    roleid0.InnerText = "01";
                    op.AppendChild(roleid0);
                    XmlElement roleid1 = op.OwnerDocument.CreateElement(ElementNames.RoleID);
                    roleid1.InnerText = "03";
                    op.AppendChild(roleid1);
                    msg = es.SetOperatorRoles(op);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return;
                    }
                }
            }
        }
        public static DataSet GetIcd10()
        {
            using (gjtEmrOrder.EmrOrder eo = new gjtEmrOrder.EmrOrder())
            {
                try
                {
                    return eo.Icd10();
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX932159512852", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }
        public static bool UncommitConsultNote(string sequence, string registryID)
        {
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    string msg = ep.ReturnConsultNote(sequence, registryID);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return Return.Failed;

                    }
                    return Return.Successful;

                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX123584274152", ex.Message + ">>" + ex.ToString(), true);            
             
                    return Return.Failed;

                }
            }
        }

        public static string GetTruster(string operatorID, string chargingDoctorID)
        {
            string msgTruster = "";
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    msgTruster = es.IsTruster(operatorID, chargingDoctorID);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX123584274151", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return msgTruster;
            }
        }
        public static bool UncommitEmrNote(string registryID, string noteID, int series,
            XmlNode reasion, NoteStatus status)
        {
            gjtEmrService.NoteStatus gjtStatus = (gjtEmrService.NoteStatus)status;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = es.UncommitEmrNoteEx(registryID, Globals.DoctorID,
                    Globals.DepartID, noteID, series, reasion, gjtStatus);
              
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return Return.Failed;
                }
                return Return.Successful;
            }
        }
        public static EmrConstant.PermissionLevel GetPermissionLevel(string registryID)
        {
            PermissionLevel permissionLevel = PermissionLevel.ReadWrite;
            #region Granted read privilege?
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    if (es.HavePrivilegeForRegistry(registryID, Globals.DoctorID))
                    {
                        permissionLevel = EmrConstant.PermissionLevel.ReadOnly;
                    }                   
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX123584271152", ex.Message + ">>" + ex.ToString(), true);            
                               
                }
            }
            return permissionLevel;
            #endregion
        }
        public static void RetrieveMyroles(string opcode)
        {
            Globals.myroles = null;

            XmlNode roles = null;
            if (Globals.offline)
            {
                roles = Globals.myConfig.GetMyRoles(opcode);
            }
            else
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    string msg = es.GetRolesForOneOperator(opcode, ref roles);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return;
                    }
                    if (roles == null) return;
                    /* Store my roles for you to use on offline. */
                    Globals.myConfig.SetMyRoles(opcode, roles);
                }
            }

            for (int k = 0; k < roles.ChildNodes.Count; k++)
            {
                Globals.myroles += Globals.myConfig.GetRoleName(roles.ChildNodes[k].InnerText) + Delimiters.Slash;
                string[] functios = Globals.myConfig.GetFunctions(roles.ChildNodes[k].InnerText);
            }
            /* Remove end delimiter. */
            if (Globals.myroles != null) Globals.myroles = Globals.myroles.Substring(0, Globals.myroles.Length - 1);

        }
       
        //his读菜单代码
        public static void RetrieveMyrolesHIS(string opcode)
        {
            Globals.myfunctions.Clear();
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                XmlNode roles = null;
                string msg = es.GetRolesForOneOperatorHIS(opcode, Globals.zxtdm, ref roles);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
                if (roles == null) return;

                foreach (XmlNode node in roles.ChildNodes)
                {
                    Globals.myfunctions.Add(node.InnerText);
                }
            }

            //int k = 0;
            //while (k < roles.ChildNodes.Count)
            //{
            //    //string[] functios = GetFunctions(roles.ChildNodes[k].InnerText);
            //    string fun = roles.ChildNodes[k].InnerText;
            //    Globals.myfunctions.Add(fun);
            //   // k = k + 2;
            //    k++;
            //}
        }
        public static string[] GetFunctions(string roleid)
        {
            XmlNode roles = GetRoles();
            foreach (XmlNode role in roles.ChildNodes)
            {
                if (role.Attributes[AttributeNames.RoleID].Value == roleid)
                {
                    string[] functions = new string[role.ChildNodes.Count];
                    for (int k = 0; k < role.ChildNodes.Count; k++)
                    {
                        functions[k] = role.ChildNodes[k].InnerText;
                    }
                    return functions;
                }
            }
            return null;
        }
        public static XmlNode GetRoles()
        {
            XmlNode roles = Globals.Config.SelectSingleNode(ElementNames.Roles);
            return roles;
        }

        public static bool IsUnlockedEmr(string registryID)
        {
            if (Globals.offline)
            {
                return false;
            }
            else
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    string yesno = null;
                    string msg = es.CanEditLockedEmrdocument(registryID, Globals.DoctorID,
                        ref yesno);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return false;
                    }
                    return yesno == StringGeneral.One;
                }
            }
        }
        public static string GetPatientNameByRegistryID(string registryID)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string archiveNum = null;
                string pname = null;
                pi.GetNameAndArchiveNumberForInpatient(registryID, ref pname, ref archiveNum);
                return pname;
            }
        }
        public static XmlNode GetEmrDocumentForOneRegistry(string registryID)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                XmlNode emrdoc = null;
                bool ret = es.GetEmrDocumentWithoutNote(registryID, ref emrdoc);
                if (ret == Return.Failed) return null;
                else return emrdoc;
            }
        }
        public static string GetDepartCodeByRegistryID(string registryID)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string DepartCode = null;
                XmlNode patients = pi.QueryPatientList(gjtEmrPatients.QueryMode.RegistryID, registryID, Globals.inStyle);
                if (patients == null) return null;
                if (patients.SelectNodes(ElementNames.Patient).Count == 0) return null;
                foreach (XmlNode patient in patients.SelectNodes(ElementNames.Patient)[0])
                {
                    if (patient.Attributes[AttributeNames.RegistryID].Value == registryID)
                    {
                        return patient.Attributes[AttributeNames.DepartmentCode].Value;
                    }
                }
                return DepartCode;
            }
        }
     
        public static bool DegreeOrderTime(string registryID, ref XmlNode degreeOrder)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.DegreeOrderTime(registryID, ref degreeOrder);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return false;
                }
            }
            return true;
        }
      
        public static bool TimeOutEmr(string noteID, string registryID, DateTime finishiedTime)
        {
            string stime = Globals.emrPattern.StartTimeAttribute(noteID);
            DateTime fromtime = StartTime(noteID, stime, registryID);
            double hours = Convert.ToDouble(Globals.emrPattern.TimeLimitAttribute(noteID));
            TimeSpan hoursLimit = TimeSpan.FromHours(hours);
            return finishiedTime.Subtract(fromtime) >= hoursLimit;
        }
        public static DateTime StartTime(string noteID, string stime, string registryID)
        {
            DateTime fromTime = DateTime.MinValue;
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                string msg = pi.GetStartTime(noteID, stime, registryID, ref fromTime);
                if (msg != null)
                {
                    MessageBox.Show(msg, EmrConstant.ErrorMessage.Error);
                    return fromTime;
                }
                return fromTime;
            }
        }
        private static XmlNode GetDoctorNames()
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    return pi.DoctorNameList();
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX121584274152", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }
        public static XmlNode GetDepartmentsByMode(gjtEmrPatients.WorkMode mode)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    XmlNode node = pi.GetDepartmentListByMode(mode);
                    if (node.FirstChild.Name == "error") return null;
                    else return node;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX123582274152", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }
        public static string GetAreaCodeOfDepartment(string departmentCode)
        {
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                return pi.GetAreaCodeOfDepartment(departmentCode);
            }
        }
        private static XmlNode GetPattern()
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    XmlNode node = null;
                    string ret = es.GetEmrPattern(Globals.DepartID, ref node);
                    if (ret == null) return node;
                    ret = es.GetEmrPattern(StringGeneral.NullCode, ref node);
                    if (ret == null) return node;
                    MessageBox.Show(ret, ErrorMessage.Warning);
                    return node;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX123484274152", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }
        private static XmlNode GetChildPattern()
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    XmlNode node = null;

                    string ret = es.GetEmrPattern(StringGeneral.ChildCode, ref node);
                    if (ret == null) return node;
                    MessageBox.Show(ret, ErrorMessage.Warning);
                    return node;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX123584271152", ex.Message + ">>" + ex.ToString(), true);            
             
                    return null;
                }
            }
        }
        public static string SehPatternDoc(ref XmlNode pattern)
        {
            string msg = "";
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                bool exam = es.SehPatternDoc(Globals.NoteID);
                if (exam == true)
                {
                    msg = "不存在此式样的模板！";
                    return msg;
                }                
                es.GetPatternDoc(Globals.NoteID, ref pattern);               
            }
            return msg;
        }
        public static DateTime LastDegreeOrderTime(string registryID)
        {
            DateTime dot = DateTime.MinValue;
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {

                string msg = pi.LastStratTimeForDegreeOrder(registryID, ref dot);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                }
            }
            return dot;
        }
        public static bool DeleteOneEmrNote(string registryID, string noteID, int series)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    string msg = "";
                    if (Globals.Sign == false)
                    {
                        msg = es.DeleteEmrNoteEx(registryID, noteID, series);
                    }
                    else
                    {
                        msg = es.DeleteEmrNoteExEx(registryID, noteID, series);
                    }

                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return Return.Failed;
                    }
                    return Return.Successful;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX123584284152", ex.Message + ">>" + ex.ToString(), true);            
             
                    return Return.Failed;
                }
            }
        }
        public static bool FinishConsultNote(string sequence, string registryID)
        {
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    string msg = ep.FinishConsultNote(sequence, registryID, Globals.DoctorID);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return Return.Failed;

                    }
                    return Return.Successful;

                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX4563584274152", ex.Message + ">>" + ex.ToString(), true);            
             
                    return Return.Failed;

                }
            }
        }
        public static long PutNoteTemplate(string doctorID, string departmentCode, XmlNode newTemplate, string IllType, string IllName, string Creator, string CreateDate, string Sex, string Type, string TypeName, string NoteID, string NoteName, string TemplateName)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                long pk = -1;
                try
                {
                    pk = es.InSertNoteTemplateExZlg(newTemplate, doctorID, departmentCode, IllType, IllName, Creator, CreateDate, Sex, Type, TypeName, NoteID, NoteName, TemplateName);
                    if (pk < 0) MessageBox.Show(ErrorMessage.WebServiceError, ErrorMessage.Error);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX96321584274152", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return pk;
            }
        }
        public static  long PutNoteTemplateOld(string doctorID, string departmentCode, XmlNode newTemplate)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                long pk = -1;
                try
                {
                    pk = es.NewNoteTemplateZlg(doctorID, departmentCode, newTemplate);
                    if (pk < 0) MessageBox.Show(ErrorMessage.WebServiceError, ErrorMessage.Error);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX7569874512", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return pk;
            }

        }
        public static bool UpdateEmrBlock(int pk, XmlNode block)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    if (es.UpdateEmrBlock(pk, block) == Return.Successful) return Return.Successful;
                    MessageBox.Show(ErrorMessage.SystemError, ErrorMessage.Error);
                    return Return.Failed;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX7569874511", ex.Message + ">>" + ex.ToString(), true);            
             
                    return Return.Failed;
                }
            }
        }
        public static int PutEmrBlock(string departCode, XmlNode block)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    return es.AddEmrBlock(departCode, block);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX7569874513", ex.Message + ">>" + ex.ToString(), true);            
             
                    return -1;
                }
            }
        }
        public static bool SavePic(string picName, string departCode,XmlNode nPicture)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    if (es.AddPicture(picName, departCode, nPicture) == EmrConstant.Return.Failed)
                    {
                        MessageBox.Show(EmrConstant.ErrorMessage.WebServiceError, EmrConstant.ErrorMessage.Error);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX7569874515", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return true;
            }
        }
        public static string GetPrintInfo(int printCount, ref XmlNode printInfo)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = es.GetNotePrinting(printCount, ref printInfo);
                if (msg != null) MessageBox.Show(msg, ErrorMessage.Error);
                return msg;
            }
        }
    }
}
