using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;
using System.IO;
using EmrConstant;

namespace CommonLib
{
   public  class MyConfig
    {
        private XmlDocument doc = new XmlDocument();
        private string myConfigFile;

        public MyConfig(string configFile, string myconfig)
        {
            myConfigFile = configFile;
            if (File.Exists(configFile))
            {
                doc.Load(configFile);
                /* For old users, myconfig.xml exists but has no start time elelement. 
                   So must be added from config of resource. */
                XmlNode startTimes = GetStartTimes();
                if (startTimes == null)
                {
                    startTimes = doc.CreateElement(ElementNames.StartTime);
                    doc.DocumentElement.AppendChild(startTimes);
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(myconfig);
                    startTimes.InnerXml = doctmp.DocumentElement.SelectSingleNode(ElementNames.StartTime).InnerXml;
                    doc.Save(configFile);
                }
            }
            else
            {
                doc.LoadXml(myconfig);
                doc.Save(configFile);
            }
        }
        public string GetWorkFolder()
        {
            XmlNode workFolder = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.WorkFolder);
            return workFolder.Attributes[EmrConstant.AttributeNames.Name].Value;
        }
        public XmlNode GetStartTimes()
        {
            return doc.DocumentElement.SelectSingleNode(ElementNames.StartTime);
        }
        public string GetHospitalName()
        {
            XmlNode hname = doc.DocumentElement.SelectSingleNode(ElementNames.HospitalName);
            if (hname == null) return StringGeneral.NullCode;
            return hname.Attributes[AttributeNames.Value].Value;
        }
        public string GetAuditLevelSystem()
        {
            XmlNode audit = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.AuditSystem);
            if (audit == null) return null;
            return audit.Attributes[EmrConstant.AttributeNames.Mode].Value;
        }
        public void SetAuditSystem(string level, string mode)
        {
            XmlNode audit = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.AuditSystem);
            if (audit == null)
            {
                XmlElement auditElement = doc.CreateElement(EmrConstant.ElementNames.AuditSystem);
                auditElement.SetAttribute(EmrConstant.AttributeNames.Level, level);
                auditElement.SetAttribute(EmrConstant.AttributeNames.Mode, mode);
                doc.DocumentElement.AppendChild(auditElement);
            }
            else
            {
                audit.Attributes[EmrConstant.AttributeNames.Level].Value = level;
                audit.Attributes[EmrConstant.AttributeNames.Mode].Value = mode;
            }
            doc.Save(myConfigFile);
        }
        public string GetAuditSystem()
        {
            XmlNode audit = doc.DocumentElement.SelectSingleNode(ElementNames.AuditSystem);
            if (audit == null) return null;
            return audit.Attributes[EmrConstant.AttributeNames.Level].Value;
        }
        public string GetVisitNoteID(string visit)
        {
            XmlNode noteID = doc.DocumentElement.SelectSingleNode(visit);
            if (noteID == null) return "00 ";
            return noteID.InnerText;
        }
        public void SetVisitNoteID(string visit, string ID)
        {
            XmlNode noteID = doc.DocumentElement.SelectSingleNode(visit);
            if (noteID == null)
            {
                noteID = doc.CreateElement(visit);
                doc.DocumentElement.AppendChild(noteID);
            }
            noteID.InnerText = ID;
            doc.Save(myConfigFile);
        }
        public int GetDrugUnitMode()
        {
            XmlNode unitMode = doc.DocumentElement.SelectSingleNode(ElementNames.DrugUnitMode);
            if (unitMode == null) return 1;
            return Convert.ToInt32(unitMode.InnerText);
        }
        public CardUseWay GetCardUseWay()
        {
            XmlNode useWay = doc.DocumentElement.SelectSingleNode(ElementNames.CardUseWay);
            if (useWay == null) return CardUseWay.NoCard;
            return (CardUseWay)Convert.ToInt32(useWay.InnerText);
        }
        public double GetLimitDays()
        {
            XmlNode limit = doc.DocumentElement.SelectSingleNode(ElementNames.LimitInDays);
            if (limit == null) return 3;
            return Convert.ToDouble(limit.InnerText);
        }
        public void SetLimitDays(double value)
        {
            XmlNode limit = doc.DocumentElement.SelectSingleNode(ElementNames.LimitInDays);
            if (limit == null)
            {
                limit = doc.CreateElement(EmrConstant.ElementNames.LimitInDays);
                doc.DocumentElement.AppendChild(limit);
            }
            limit.InnerText = value.ToString();
            doc.Save(myConfigFile);
        }
        private XmlNode GetShowQCINode(string doctorID)
        {
            XmlNodeList showQCIs = doc.DocumentElement.SelectNodes(EmrConstant.ElementNames.ShowQualityControlInfo);
            foreach (XmlNode showQCI in showQCIs)
            {
                if (showQCI.Attributes[EmrConstant.AttributeNames.DoctorID].Value == doctorID)
                    return showQCI;
            }
            return null;
        }
        public bool GetShowQCI(string doctorID)
        {
            XmlNode showQCI = GetShowQCINode(doctorID);
            if (showQCI == null) return false;
            return showQCI.Attributes[EmrConstant.AttributeNames.Value].Value == EmrConstant.StringGeneral.Yes;
        }
        public void SetShowQCI(string doctorID, bool value)
        {
            XmlNode showQCI = GetShowQCINode(doctorID);
            if (showQCI == null)
            {
                XmlElement topElement = doc.CreateElement(EmrConstant.ElementNames.ShowQualityControlInfo);
                topElement.SetAttribute(EmrConstant.AttributeNames.DoctorID, doctorID);
                if (value)
                    topElement.SetAttribute(EmrConstant.AttributeNames.Value, EmrConstant.StringGeneral.Yes);
                else
                    topElement.SetAttribute(EmrConstant.AttributeNames.Value, EmrConstant.StringGeneral.No);
                doc.DocumentElement.AppendChild(topElement);
            }
            else
            {
                if (value)
                    showQCI.Attributes[EmrConstant.AttributeNames.Value].Value = EmrConstant.StringGeneral.Yes;
                else
                    showQCI.Attributes[EmrConstant.AttributeNames.Value].Value = EmrConstant.StringGeneral.No;
            }
            doc.Save(myConfigFile);
        }
        public int GetShowTime(string doctorID, string timeName, int defaultValue)
        {
            int timeSpan = defaultValue;
            XmlNode showTime = GetShowTimeNode(doctorID, timeName);
            if (showTime != null)
                timeSpan = Convert.ToInt32(showTime.Attributes[AttributeNames.Value].Value);
            return timeSpan;
        }
        private XmlNode GetShowTimeNode(string doctorID, string timeName)
        {
            XmlNodeList showTimes = doc.DocumentElement.SelectNodes(timeName);
            foreach (XmlNode showTime in showTimes)
            {
                if (showTime.Attributes[EmrConstant.AttributeNames.DoctorID].Value == doctorID)
                    return showTime;
            }
            return null;
        }
        public void SetShowTime(string doctorID, string timeName, int newValue)
        {
            XmlNode showTime = GetShowTimeNode(doctorID, timeName);
            if (showTime == null)
            {
                XmlElement topElement = doc.CreateElement(timeName);
                topElement.SetAttribute(AttributeNames.DoctorID, doctorID);
                topElement.SetAttribute(AttributeNames.Value, newValue.ToString());
                doc.DocumentElement.AppendChild(topElement);
            }
            else
            {
                showTime.Attributes[AttributeNames.Value].Value = newValue.ToString();
            }
            doc.Save(myConfigFile);
        }
        public bool GetExpand(string doctorID)
        {
            XmlNode expand = GetExpandNode(doctorID);
            if (expand == null) return false;
            return expand.Attributes[EmrConstant.AttributeNames.Value].Value == EmrConstant.StringGeneral.Yes;
        }
        private XmlNode GetExpandNode(string doctorID)
        {
            XmlNodeList expands = doc.DocumentElement.SelectNodes(EmrConstant.ElementNames.Expand);
            foreach (XmlNode expand in expands)
            {
                if (expand.Attributes[EmrConstant.AttributeNames.DoctorID].Value == doctorID)
                    return expand;
            }
            return null;
        }
        public void SetExpand(string doctorID, bool value)
        {
            XmlNode expand = GetExpandNode(doctorID);
            if (expand == null)
            {
                XmlElement topElement = doc.CreateElement(EmrConstant.ElementNames.Expand);
                topElement.SetAttribute(EmrConstant.AttributeNames.DoctorID, doctorID);
                if (value)
                    topElement.SetAttribute(EmrConstant.AttributeNames.Value, EmrConstant.StringGeneral.Yes);
                else
                    topElement.SetAttribute(EmrConstant.AttributeNames.Value, EmrConstant.StringGeneral.No);
                doc.DocumentElement.AppendChild(topElement);
            }
            else
            {
                if (value)
                    expand.Attributes[EmrConstant.AttributeNames.Value].Value = EmrConstant.StringGeneral.Yes;
                else
                    expand.Attributes[EmrConstant.AttributeNames.Value].Value = EmrConstant.StringGeneral.No;
            }
            doc.Save(myConfigFile);
        }
        public bool GetCanWriteNoteInOffline()
        {
            XmlNode writeNote = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.CanWriteNoteInOffline);
            if (writeNote == null) return false;
            if (writeNote.InnerText == EmrConstant.StringGeneral.Yes) return true;
            else return false;
        }
        public void SetCanWriteNoteInOffline(bool value)
        {
            XmlNode writeNote = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.CanWriteNoteInOffline);
            if (writeNote == null)
            {
                writeNote = doc.CreateElement(EmrConstant.ElementNames.CanWriteNoteInOffline);
                doc.DocumentElement.AppendChild(writeNote);
            }
            writeNote.InnerText = EmrConstant.StringGeneral.No;
            if (value) writeNote.InnerText = EmrConstant.StringGeneral.Yes;
            doc.Save(myConfigFile);
        }
        public bool GetPageMargin(ref float left, ref float top, ref float right, ref float bottom)
        {
            XmlNode margin = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.PageMargin);
            if (margin != null)
            {
                left = (float)Convert.ToDouble(margin.Attributes[EmrConstant.AttributeNames.MarginLeft].Value);
                right = (float)Convert.ToDouble(margin.Attributes[EmrConstant.AttributeNames.MarginRight].Value);
                top = (float)Convert.ToDouble(margin.Attributes[EmrConstant.AttributeNames.MarginTop].Value);
                bottom = (float)Convert.ToDouble(margin.Attributes[EmrConstant.AttributeNames.MarginBottom].Value);
                return true;
            }
            else
            {
                SetPageMargin(left, top, right, bottom);
                return false;
            }
        }
        public void SetPageMargin(float left, float top, float right, float bottom)
        {
            XmlNode margin = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.PageMargin);
            if (margin != null)
            {
                margin.Attributes[EmrConstant.AttributeNames.MarginLeft].Value = left.ToString();
                margin.Attributes[EmrConstant.AttributeNames.MarginTop].Value = top.ToString();
                margin.Attributes[EmrConstant.AttributeNames.MarginRight].Value = right.ToString();
                margin.Attributes[EmrConstant.AttributeNames.MarginBottom].Value = bottom.ToString();
            }
            else
            {
                XmlElement marginEle = doc.CreateElement(EmrConstant.ElementNames.PageMargin);
                marginEle.SetAttribute(EmrConstant.AttributeNames.MarginLeft, left.ToString());
                marginEle.SetAttribute(EmrConstant.AttributeNames.MarginTop, top.ToString());
                marginEle.SetAttribute(EmrConstant.AttributeNames.MarginRight, right.ToString());
                marginEle.SetAttribute(EmrConstant.AttributeNames.MarginBottom, bottom.ToString());
                doc.DocumentElement.AppendChild(marginEle);
            }
            doc.Save(myConfigFile);
        }
        private XmlNode GetBecarefulSeePatientNode(string doctorID)
        {
            XmlNodeList becarefulSeePatients = doc.DocumentElement.SelectNodes(ElementNames.BecarefulSeePatient);
            foreach (XmlNode becarefulSeePatient in becarefulSeePatients)
            {
                if (becarefulSeePatient.Attributes[EmrConstant.AttributeNames.DoctorID].Value == doctorID)
                    return becarefulSeePatient;
            }
            return null;
        }
        public bool GetBecarefulSeePatient(string doctorID)
        {
            XmlNode becarefulSeePatient = GetBecarefulSeePatientNode(doctorID);
            if (becarefulSeePatient == null) return false;

            return (becarefulSeePatient.Attributes[AttributeNames.Value].Value == EmrConstant.StringGeneral.Yes);
        }
        public void SetBecarefulSeePatient(string doctorID, bool value)
        {
            XmlNode becarefulSeePatient = GetBecarefulSeePatientNode(doctorID);
            if (becarefulSeePatient == null)
            {
                XmlElement becarefulSeePatientEle = doc.CreateElement(ElementNames.BecarefulSeePatient);
                becarefulSeePatientEle.SetAttribute(AttributeNames.DoctorID, doctorID);
                if (value)
                    becarefulSeePatientEle.SetAttribute(AttributeNames.Value, StringGeneral.Yes);
                else
                    becarefulSeePatientEle.SetAttribute(AttributeNames.Value, StringGeneral.No);
                doc.DocumentElement.AppendChild(becarefulSeePatientEle);
            }
            else
            {
                if (value)
                    becarefulSeePatient.Attributes[AttributeNames.Value].Value = StringGeneral.Yes;
                else
                    becarefulSeePatient.Attributes[AttributeNames.Value].Value = StringGeneral.No;
            }
            doc.Save(myConfigFile);
        }
        private XmlNode GetAutoSyncCheckNode(string doctorID)
        {
            XmlNodeList autoSyncs = doc.DocumentElement.SelectNodes(ElementNames.AutoSyncCheck);
            foreach (XmlNode autoSync in autoSyncs)
            {
                if (autoSync.Attributes[AttributeNames.DoctorID].Value == doctorID)
                    return autoSync;
            }
            return null;
        }

        public bool GetAutoSyncCheck(string doctorID)
        {
            XmlNode autoSync = GetAutoSyncCheckNode(doctorID);
            if (autoSync == null) return true;

            return (autoSync.Attributes[AttributeNames.Value].Value == StringGeneral.Yes);
        }
        public void SetAutoSyncCheck(string doctorID, bool value)
        {
            XmlNode autoSync = GetAutoSyncCheckNode(doctorID);
            if (autoSync == null)
            {
                XmlElement autoSyncE = doc.CreateElement(ElementNames.AutoSyncCheck);
                autoSyncE.SetAttribute(AttributeNames.DoctorID, doctorID);
                if (value) autoSyncE.SetAttribute(AttributeNames.Value, StringGeneral.Yes);
                else autoSyncE.SetAttribute(AttributeNames.Value, StringGeneral.No);
                doc.DocumentElement.AppendChild(autoSyncE);
            }
            else
            {
                if (value) autoSync.Attributes[AttributeNames.Value].Value = StringGeneral.Yes;
                else autoSync.Attributes[AttributeNames.Value].Value = StringGeneral.No;
            }
            doc.Save(myConfigFile);
        }
        public bool GetOperatorDepartment()
        {
            XmlNode od = doc.DocumentElement.SelectSingleNode(ElementNames.OperatorDepartment);
            if (od == null) return false;

            return (od.Attributes[AttributeNames.State].Value == StringGeneral.Yes);
        }
        public bool GetOperatorArchieve()
        {
            XmlNode od = doc.DocumentElement.SelectSingleNode(ElementNames.OperatorArchieve);
            if (od == null) return false;

            return (od.Attributes[AttributeNames.State].Value == StringGeneral.Yes);
        }
        public bool GetEnterCommitTime(string doctorID)
        {
            XmlNode ect = GetEnterCommitTimeNode(doctorID);
            if (ect == null) return true;

            return (ect.Attributes[AttributeNames.Value].Value == StringGeneral.Yes);
        }
        private XmlNode GetEnterCommitTimeNode(string doctorID)
        {
            XmlNodeList ects = doc.DocumentElement.SelectNodes(ElementNames.EnterCommitTime);
            foreach (XmlNode ect in ects)
            {
                if (ect.Attributes[AttributeNames.DoctorID].Value == doctorID) return ect;
            }
            return null;
        }
        public XmlNode GetOptions()
        {
            XmlNode options = doc.DocumentElement.SelectSingleNode(ElementNames.Options);
            return options;
        }
        public void SetOptions(XmlNode options)
        {
            XmlNode optionsOld = doc.DocumentElement.SelectSingleNode(ElementNames.Options);
            if (optionsOld == null)
            {
                optionsOld = doc.CreateElement(ElementNames.Options);
                doc.DocumentElement.AppendChild(optionsOld);
            }
            optionsOld.InnerXml = options.InnerXml;
            doc.Save(myConfigFile);
        }
        public void SetMustByPoorPatient(bool value)
        {
            XmlNode byPoorPatient = doc.DocumentElement.SelectSingleNode(ElementNames.ByPoorPatient);
            if (byPoorPatient == null)
            {
                byPoorPatient = doc.CreateElement(EmrConstant.ElementNames.ByPoorPatient);
                doc.DocumentElement.AppendChild(byPoorPatient);
            }
            byPoorPatient.InnerText = EmrConstant.StringGeneral.No;
            if (value) byPoorPatient.InnerText = EmrConstant.StringGeneral.Yes;
            doc.Save(myConfigFile);
        }
        public void SetEnterCommitTime(string doctorID, bool value)
        {
            XmlNode ect = GetEnterCommitTimeNode(doctorID);
            if (ect == null)
            {
                XmlElement ectE = doc.CreateElement(ElementNames.EnterCommitTime);
                ectE.SetAttribute(AttributeNames.DoctorID, doctorID);
                if (value) ectE.SetAttribute(AttributeNames.Value, StringGeneral.Yes);
                else ectE.SetAttribute(AttributeNames.Value, StringGeneral.No);
                doc.DocumentElement.AppendChild(ectE);
            }
            else
            {
                if (value) ect.Attributes[AttributeNames.Value].Value = StringGeneral.Yes;
                else ect.Attributes[AttributeNames.Value].Value = StringGeneral.No;
            }
            doc.Save(myConfigFile);
        }
        public string GetMergeCode(string doctorID)
        {
            XmlNode mergeCode = GetMergeCodeNode(doctorID);
            if (mergeCode == null) return StringGeneral.One;

            return mergeCode.Attributes[AttributeNames.Value].Value;
        }
        public void SetMergeCode(string doctorID, string code)
        {
            XmlNode mergeCode = GetMergeCodeNode(doctorID);
            if (mergeCode == null)
            {
                XmlElement mergeCodeNew = doc.CreateElement(ElementNames.Merge);
                mergeCodeNew.SetAttribute(AttributeNames.DoctorID, doctorID);
                mergeCodeNew.SetAttribute(AttributeNames.Value, code);
                doc.DocumentElement.AppendChild(mergeCodeNew);
            }
            else
            {
                mergeCode.Attributes[AttributeNames.Value].Value = code;
            }
            doc.Save(myConfigFile);
        }
        public void SetOperatorDepartment(bool value)
        {
            XmlElement od = (XmlElement)doc.DocumentElement.SelectSingleNode(ElementNames.OperatorDepartment);
            if (od == null)
            {
                od = doc.CreateElement(ElementNames.OperatorDepartment);
                od.SetAttribute(AttributeNames.State, StringGeneral.No);
                doc.DocumentElement.AppendChild(od);
            }
            if (value) od.SetAttribute(AttributeNames.State, StringGeneral.Yes);
            else od.SetAttribute(AttributeNames.State, StringGeneral.No);
            doc.Save(myConfigFile);
        }
        public void SetCardUseWay(CardUseWay useWay)
        {
            XmlNode useWayNode = doc.DocumentElement.SelectSingleNode(ElementNames.CardUseWay);
            if (useWayNode == null)
            {
                useWayNode = doc.CreateElement(ElementNames.CardUseWay);
                doc.DocumentElement.AppendChild(useWayNode);
            }
            useWayNode.InnerText = useWay.ToString("d");
            doc.Save(myConfigFile);
        }
        private XmlNode GetMergeCodeNode(string doctorID)
        {
            XmlNodeList mergeCodes = doc.DocumentElement.SelectNodes(ElementNames.Merge);
            foreach (XmlNode mergeCode in mergeCodes)
            {
                if (mergeCode.Attributes[AttributeNames.DoctorID].Value == doctorID) return mergeCode;
            }
            return null;
        }
        public bool GetMustByPoorPatient()
        {
            XmlNode byPoorPatient = doc.DocumentElement.SelectSingleNode(ElementNames.ByPoorPatient);
            if (byPoorPatient == null) return false;
            return (byPoorPatient.InnerText == EmrConstant.StringGeneral.Yes);
        }
        public void SetDrugUnitMode(int value)
        {
            XmlNode unitMode = doc.DocumentElement.SelectSingleNode(ElementNames.DrugUnitMode);
            if (unitMode == null)
            {
                unitMode = doc.CreateElement(ElementNames.DrugUnitMode);
                doc.DocumentElement.AppendChild(unitMode);
            }
            unitMode.InnerText = value.ToString();
            doc.Save(myConfigFile);
        }

        public void SetHospitalName(string hospitalName)
        {
            XmlNode hname = doc.DocumentElement.SelectSingleNode(ElementNames.HospitalName);
            if (hname == null)
            {
                XmlElement hnameNew = doc.CreateElement(ElementNames.HospitalName);
                hnameNew.SetAttribute(AttributeNames.Value, hospitalName);
                doc.DocumentElement.AppendChild(hnameNew);
            }
            else
            {
                hname.Attributes[AttributeNames.Value].Value = hospitalName;
            }
            doc.Save(myConfigFile);
        }
        public void SetDepartmentName(string name)
        {
            XmlNode department = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.Department);
            department.Attributes[EmrConstant.AttributeNames.Name].Value = name;
            doc.Save(myConfigFile);
        }

        public void SetRoles(XmlNode roles)
        {
            XmlNode rolesOld = doc.DocumentElement.SelectSingleNode(ElementNames.Roles);
            if (rolesOld == null)
            {
                rolesOld = doc.CreateElement(ElementNames.Roles);
                doc.DocumentElement.AppendChild(rolesOld);
            }
            rolesOld.InnerXml = roles.InnerXml;
            doc.Save(myConfigFile);

        }

        public string GetDepartmentCode()
        {
            XmlNode department = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.Department);
            return department.Attributes[EmrConstant.AttributeNames.Code].Value;
        }
        public string GetDepartmentName()
        {
            XmlNode department = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.Department);
            return department.Attributes[EmrConstant.AttributeNames.Name].Value;
        }
        public void SetDepartmentCode(string code)
        {
            XmlNode department = doc.DocumentElement.SelectSingleNode(EmrConstant.ElementNames.Department);
            department.Attributes[EmrConstant.AttributeNames.Code].Value = code;
            doc.Save(myConfigFile);
        }

        public string GetAreaCode()
        {
            XmlNode area = doc.DocumentElement.SelectSingleNode(ElementNames.Area);
            if (area == null) return StringGeneral.NullCode;
            return area.Attributes[AttributeNames.Code].Value;
        }
        public void SetAreaCode(string code)
        {
            XmlNode area = doc.DocumentElement.SelectSingleNode(ElementNames.Area);
            if (area == null)
            {
                XmlElement areaNew = doc.CreateElement(ElementNames.Area);
                areaNew.SetAttribute(AttributeNames.Code, code);
                doc.DocumentElement.AppendChild(areaNew);
            }
            else
            {
                area.Attributes[AttributeNames.Code].Value = code;
            }
            doc.Save(myConfigFile);
        }
        public XmlNode GetMyRoles(string doctorID)
        {
            XmlNodeList myRoles = doc.DocumentElement.SelectNodes(ElementNames.MyRoles);
            foreach (XmlNode myRole in myRoles)
            {
                if (myRole.Attributes[AttributeNames.DoctorID].Value == doctorID)
                    return myRole;
            }
            return null;
        }
        public void SetMyRoles(string doctorID, XmlNode roles)
        {
            XmlNode myRoles = GetMyRoles(doctorID);
            if (myRoles == null)
            {
                XmlElement myRolesE = doc.CreateElement(ElementNames.MyRoles);
                myRolesE.SetAttribute(AttributeNames.DoctorID, doctorID);
                XmlElement rolesE = doc.CreateElement(ElementNames.Roles);
                rolesE.InnerXml = roles.InnerXml;
                myRolesE.AppendChild(rolesE);
                doc.DocumentElement.AppendChild(myRolesE);
            }
            else
            {
                myRoles.InnerXml = roles.InnerXml;
            }
            doc.Save(myConfigFile);
        }
        public string GetRoleName(string roleid)
        {
            XmlNode roles = GetRoles();
            foreach (XmlNode role in roles.ChildNodes)
            {
                if (role.Attributes[AttributeNames.RoleID].Value == roleid)
                {
                    return role.Attributes[AttributeNames.RoleName].Value;
                }
            }
            return null;
        }
        public XmlNode GetRoles()
        {
            XmlNode roles = doc.DocumentElement.SelectSingleNode(ElementNames.Roles);
            return roles;
        }
        public string[] GetFunctions(string roleid)
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

    }
}
