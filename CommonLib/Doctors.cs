using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;
using EmrConstant;
using System.IO;

namespace CommonLib
{
   public class Doctors
    {
        private XmlDocument docDoctors = null;
        public Doctors(string doctorFile)
        {
            if (!File.Exists(doctorFile)) return;
            docDoctors = new XmlDocument();
            docDoctors.Load(doctorFile);
        }
        public TitleLevel GetTitleLevel(string doctorID)
        {
            TitleLevel tl = TitleLevel.Nothing;
            if (docDoctors == null) return tl;
            foreach (XmlNode doctor in docDoctors.DocumentElement.SelectNodes(ElementNames.Doctor))
            {
                if (doctor.Attributes[AttributeNames.Code].Value == doctorID)
                {
                    string titleLevel = doctor.Attributes[AttributeNames.TitleLevel].Value;
                    if (titleLevel.Length > 0) tl = (TitleLevel)Convert.ToInt32(titleLevel);
                    break;
                }
            }

            return tl;
        }

        public string GetDoctorName(string doctorID)
        {
            if (docDoctors == null) return null;
            foreach (XmlNode doctor in docDoctors.DocumentElement.SelectNodes(ElementNames.Doctor))
            {
                if (doctor.Attributes[AttributeNames.Code].Value == doctorID)
                {
                    return doctor.Attributes[AttributeNames.Name].Value;
                }
            }
            return null;
        }

        public string GetGroupHeader(string doctorID)
        {
            if (docDoctors == null) return null;
            foreach (XmlNode doctor in docDoctors.DocumentElement.SelectNodes(ElementNames.Doctor))
            {
                if (doctor.Attributes[AttributeNames.Code].Value == doctorID)
                {
                    return doctor.Attributes[AttributeNames.Header].Value;
                }
            }
            return null;
        }

        public XmlNodeList GetAllDoctors()
        {
            if (docDoctors == null) return null;
            return docDoctors.DocumentElement.SelectNodes(ElementNames.Doctor);
        }
    }
}
