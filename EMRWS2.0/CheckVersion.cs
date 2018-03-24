using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EmrConstant;
using HuronControl;
using CommonLib;


namespace EMR
{
    
    public class CheckVersion
    {
        private string[] components = { "udt.dll", "WordAddInEmrw.dll", "EMR.exe.config" };
        public CheckVersion()
        {
        }
        public void GetComponentList(out string[] list)
        {
            list = (string[])components.Clone();
        }
        public bool IsThereHigherVersion(string oldVersion)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode component = doc.CreateElement(ElementNames.Componet);
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                     string msg ="";

                            msg = es.GetComponet(components[2], ref component);
                    if (msg != null)
                    {
                        if (msg != EmrConstant.ErrorMessage.NoNewVersion)
                            MessageBox.Show(msg, EmrConstant.ErrorMessage.Error);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741852963", ex.Message + ">>" + ex.ToString(), true);       
                    return false;
                }
            }
            Byte[] byteStream = Convert.FromBase64String(component.InnerText);
            File.WriteAllBytes("aaa", byteStream);
            XmlReader reader = XmlReader.Create("aaa");
            reader.ReadToFollowing("assemblyIdentity");
            string version = reader.GetAttribute("version");
            reader.Close();
            File.Delete("aaa");
            return oldVersion != version;

        }

        public string DownloadNewVersion(string folder)
        {
            for (int i = 0; i < components.Length; i++)
            {
                string msg = "";
              msg = FileFromComponentInDatabase(components[i], folder);
                if (msg != null) return msg;
            }
            return null;
        }

        private string FileFromComponentInDatabase(string componentName, string oldVersionFolder)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode component = doc.CreateElement(EmrConstant.ElementNames.Componet);
            gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml();
            string msg = es.GetComponet(componentName, ref component);
            if (msg != null) return msg;

            Byte[] byteStream = Convert.FromBase64String(component.InnerText);
            try
            {
                File.WriteAllBytes(Path.Combine(oldVersionFolder, componentName), byteStream);
                return null;
            }
            catch (FieldAccessException ex)
            {
                Globals.logAdapter.Record("EX741852964", ex.Message + ">>" + ex.ToString(), true);       
                 
                return ex.Message + " - " + ex.Source;
            }
        }
    }
}
