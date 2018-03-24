using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;

namespace AboutConfig
{


    /// <summary>
    /// Summary description for myClass
    /// this file is for the configuration of the web.config
    /// the class----ConfigClass includes seven public static methords
    /// GetConfigString(string ,string)
    /// GetConfigDecimal(string,string)
    /// GetConfigInt(string,string)
    /// SetConfigKeyValue(string,string,string)
    /// RemoveSectionKey(string,string)
    /// LoadConfigDocument()
    /// getConfigFilePath()
    /// author:Guo Jiangtao
    /// date:2007-02-09 ----2007-02-09
    /// </summary>
    public  class ConfigClass
    {
        //methord:GetConfigString(string SectionName,string Key)
        //function:get the value of "Key" in "SectionName" 
        //parameters:
        //SectionName: there are seven sections in the file:web.config
        //<configuration>,<appSettings>,<complilation>,<customError>,<globalizaion>,<sessionstate>,<authentication>
        //Key: the element of Section
        //Return a string

        public static string GetConfigString(string SectionName, string key)
        {

            if (SectionName == null || SectionName == "")
            {
                NameValueCollection cfgName = (NameValueCollection)ConfigurationManager.GetSection("appSettings");
                if (cfgName[key] == null || cfgName[key] == "")
                {
                    throw (new Exception("in web.config do not discover config:\"" + key.ToString() + "\""));

                }

                else
                {
                    return cfgName[key];
                }
            }
            else
            {

                NameValueCollection cfgName = (NameValueCollection)ConfigurationManager.GetSection(SectionName);
                if (cfgName[key] == null || cfgName[key] == "")
                {
                    throw (new Exception("in web.config do not discover config:\"" + key.ToString() + "\""));
                }
                else

                {
                    return cfgName[key]; 
                }                
            }
        }

        //methord: GetConfigDecimal(string,string)
        //parameters:just like the above function
        //return a decimal

        public static decimal GetConfigDecimal(string SectionName, string key)
        {
            decimal result = 0;
            string cfgVal = GetConfigString(SectionName, key);
            if (null != cfgVal || string.Empty != cfgVal)
            {
                result = decimal.Parse(cfgVal);
            }
            return result;

        }

        //methord: GetConfigInt(string,string)
        //parameters:just like the above function
        //return a Int

        public static int GetConfigInt(string SectionName, string key)
        {
            int result = 0;
            string cfgVal = GetConfigString(SectionName, key);
            if (null != cfgVal || string.Empty != cfgVal)
            {
                result = int.Parse(cfgVal);
            }
            return result;

        }
        //methord: SetConfigKeyValue(string,string,string)
        //parameters:just like the above function       

        public static void SetConfigKeyValue(string SectionName, string key, string value)
        {
            XmlDocument doc = LoadConfigDocument();
            XmlNodeList topM = doc.DocumentElement.ChildNodes;
            foreach (XmlElement element in topM)
            {
                if (element.Name == SectionName)
                {
                    
                    XmlElement elem = (XmlElement)element.SelectSingleNode(string.Format("//add[@key = '{0}']", key));
                    if (null != elem)
                    {
                        elem.SetAttribute("value", value);
                    }
                    else
                    {
                        elem = doc.CreateElement("add");
                        elem.SetAttribute("key", key);
                        elem.SetAttribute("value", value);
                        element.AppendChild(elem);
                    }
                    doc.Save(getConfigFilePath());

                }
            }
             
            
        }

        //methord: RemoveSectionKey(string,string)
        //parameters:just like the above function    

        public static void RemoveSectionKey(string SectionName, string key)
        {
            XmlDocument doc = LoadConfigDocument();
            XmlNodeList topM = doc.DocumentElement.ChildNodes;
            foreach (XmlElement element in topM)
            {
                if (element.Name == SectionName)
                {

                    XmlElement elem = (XmlElement)element.SelectSingleNode(string.Format("//add[@key = '{0}']", key));
                    if (null != elem)
                    {
                        elem.RemoveAll();
                    }
                    else
                    {

                    }
                    doc.Save(getConfigFilePath());

                }
            }
        }

        //methord: LoadConfigDocument()
        //return xmlDocument
       
   
        public static XmlDocument LoadConfigDocument()
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(getConfigFilePath());
                return doc;

            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("no file", e);
            }
        }

        //methord: getConfigFilePath()
        //return a string      

        public static string getConfigFilePath()
        {
            return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        }

    }
}
    

