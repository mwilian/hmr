using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;
using EmrConstant;
using System.IO;

namespace CommonLib
{
   public class Departments
    {
        private XmlDocument docDepartments = null;
        public Departments(string departFile)
        {
            if (!File.Exists(departFile)) return;
            docDepartments = new XmlDocument();
            docDepartments.Load(departFile);
        }

        public string GetDepartmentName(string code)
        {
            if (docDepartments == null) return null;
            foreach (XmlNode depart in docDepartments.DocumentElement.SelectNodes(ElementNames.Department))
            {
                if (depart.Attributes[AttributeNames.Code].Value == code)
                {
                    return depart.Attributes[AttributeNames.Name].Value;
                }
            }
            return null;
        }

        public XmlNodeList GetAllDepartments()
        {
            if (docDepartments == null) return null;
            return docDepartments.DocumentElement.SelectNodes(ElementNames.Department);
        }

        public string GetAreaCodeOfDepartment(string departmentCode)
        {
            foreach (XmlNode depart in docDepartments.DocumentElement.SelectNodes(ElementNames.Department))
            {
                if (departmentCode == depart.Attributes[AttributeNames.Code].Value)
                    return depart.Attributes[AttributeNames.AreaCode].Value;
            }
            return null;
        }
    }
}
