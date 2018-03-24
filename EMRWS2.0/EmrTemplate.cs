using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;
using CommonLib;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Collections;
namespace EMR
{
    public partial class EmrTemplate : UserControl
    {
        private OpenTemp OpenTemplate;
        private XmlDocument personCategory = new XmlDocument();
        private XmlDocument departCategory = new XmlDocument();
        private XmlDocument hospitalCategory = new XmlDocument();
        private string personCategoryFile = null;
        private string departCategoryFile = null;
        private string hospitalCategoryFile = null;
        EmrPattern childPattern;
        //delegate TreeNode AddTreeNode(string node);
        public EmrTemplate(OpenTemp template)
        {
            InitializeComponent();
            OpenTemplate = template;
            childPattern = Globals.childPattern;
        }
        public EmrTemplate()
        {
            InitializeComponent();
            childPattern = Globals.childPattern;
            if (tvTemplate.Nodes.Count > 0) tvTemplate.Nodes.Clear();
        }
        public void LoadHospitalTemplate()
        {
            //tvTemplate.Nodes.Clear();
            DataTable dtType = new DataTable();
            dtType.TableName = "getillname"; 
            TreeNode hospital = null;
            //if (tvTemplate.Nodes.Count == 0)
            //{
            //    tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Hospital);
            //    hospital = tvTemplate.Nodes[0];  
            //}
            //if (tvTemplate.Nodes.Count == 3)
            //{
            //    hospital = tvTemplate.Nodes[0].Nodes[0];
            //}
            if (tvTemplate.Nodes.Count != 0)
            {
                for (int i = 0; i < tvTemplate.Nodes.Count; i++)
                {
                    if (tvTemplate.Nodes[i].Text.Trim() != EmrConstant.StringGeneral.Hospital)
                    {
                        continue;
                    }
                    else
                    {
                        hospital = tvTemplate.Nodes[i].Nodes[0];
                        break;
                    }
                }
                if (hospital == null)
                {
                    tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Hospital);
                    hospital = tvTemplate.Nodes[0];  
                }
            }
            else 
            {
                tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Hospital);
                hospital = tvTemplate.Nodes[0];  
            }
            hospital.Nodes.Clear();
            hospital.ForeColor = Color.Black;
            hospital.ImageIndex = 3;
            hospitalCategoryFile = MakeHospitalNoteTemplateCategory();
            if (!Globals.offline)
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        XmlNode pks = null;
                        string msg = "";
                        msg = es.GetHospitalTemplatePksZlg(ref pks);
                        if (msg == null)
                        {
                            XmlElement templates = hospitalCategory.CreateElement(ElementNames.NoteTemplates);
                            foreach (XmlNode pk in pks.ChildNodes)
                            {
                                DataTable dt = new DataTable();
                                dt.TableName = "gettemplateillname";
                                DataSet dst = new DataSet();
                                dst = es.GetTemplateIllNameZlg(pk.InnerText.Trim());
                                dt = dst.Tables[0];
                                long primarykey = Convert.ToInt32(pk.InnerText);
                                #region Create template category
                                XmlNode template = null;
                                msg = es.GetNoteTemplateZlg(primarykey, ref template);

                                if (msg != null)
                                {
                                    MessageBox.Show(msg, EmrConstant.ErrorMessage.Warning);
                                    continue;
                                }
                                XmlElement eleTemplate = hospitalCategory.CreateElement(ElementNames.NoteTemplate);
                                foreach (XmlAttribute att in template.Attributes)
                                {
                                    eleTemplate.SetAttribute(att.Name, att.Value);
                                }

                                if (dt != null && dt.Rows.Count != 0)
                                {
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        eleTemplate.SetAttribute("IllType", dt.Rows[i]["IllType"].ToString());
                                        eleTemplate.SetAttribute("IllName", dt.Rows[i]["IllName"].ToString());
                                        eleTemplate.SetAttribute("Creator", dt.Rows[i]["Creator"].ToString());
                                        eleTemplate.SetAttribute("CreateDate", dt.Rows[i]["CreateDate"].ToString());
                                        if (dt.Rows[i]["Sex"].ToString() != "")
                                        {
                                            eleTemplate.SetAttribute("Sex", dt.Rows[i]["Sex"].ToString());
                                        }

                                        eleTemplate.SetAttribute("Type", dt.Rows[i]["Type"].ToString());
                                        eleTemplate.SetAttribute("TypeName", dt.Rows[i]["TypeName"].ToString());
                                    }
                                }
                                eleTemplate.SetAttribute(AttributeNames.Pk, pk.InnerText);
                                XmlNode theader = template.SelectSingleNode(ElementNames.Header);
                                if (theader != null)
                                {
                                    XmlElement eleHeader = hospitalCategory.CreateElement(ElementNames.Header);
                                    eleHeader.InnerXml = theader.InnerXml;
                                    eleTemplate.AppendChild(eleHeader);
                                }
                                templates.AppendChild(eleTemplate);
                                #endregion
                                string templateDocName = MakeHospitalNoteTemplateDocName(pk.InnerText);
                                udt.StringToWordDocument(templateDocName, template);
                            }
                            XmlWriter writer = XmlWriter.Create(hospitalCategoryFile);
                            templates.WriteTo(writer);
                            writer.Close();
                        }
                        else
                        {
                            MessageBox.Show(msg, EmrConstant.ErrorMessage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9812374977", ex.Message + ">>" + ex.ToString(), true);            
              
                    }
                }
            }
            if (File.Exists(hospitalCategoryFile))
            {
                try
                {
                    hospitalCategory.Load(hospitalCategoryFile);
                    XmlNodeList templates = hospitalCategory.DocumentElement.SelectNodes(ElementNames.NoteTemplate);

                    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                    {
                        DataSet dst = es.GetIllName(Globals.OpDepartID, 1);
                        dtType = dst.Tables[0];
                    }
                    foreach (XmlNode template in templates)
                    {
                        string name = template.Attributes[EmrConstant.AttributeNames.Name].Value;
                        string noteID = template.Attributes[EmrConstant.AttributeNames.NoteID].Value;
                        string noteName = template.Attributes[EmrConstant.AttributeNames.NoteName].Value;
                        string childID;
                        if (template.Attributes[EmrConstant.AttributeNames.ChildID] == null || template.Attributes[EmrConstant.AttributeNames.ChildID].Value == "") childID = "0";
                        else childID = template.Attributes[EmrConstant.AttributeNames.ChildID].Value;
                        if (template.Attributes[EmrConstant.AttributeNames.ChildID] == null || template.Attributes[EmrConstant.AttributeNames.ChildID].Value == "") childID = "0";
                        TreeNode nodeNoteID = GetNodeNoteID(hospital, noteID, noteName);
                        string childName = "";
                        if (childID == "0" || childID == noteID || childID == "") childName = noteName;
                        else childName = childPattern.GetNoteNameFromNoteID(childID);
                        TreeNode nodeNoteID2 = GetNodeNoteID(nodeNoteID, childID, childName);
                        nodeNoteID2.Nodes.Add(name);
                        //tvTemplate.Invoke(new AddTreeNode(nodeNoteID2.Nodes.Add), name);
                        nodeNoteID2.LastNode.ForeColor = Color.BlueViolet;
                        nodeNoteID2.LastNode.ImageIndex = WhichImageIndex(template.Attributes[AttributeNames.Sex].Value);
                        nodeNoteID2.LastNode.Name = EmrConstant.StringGeneral.HospitalTemplate;
                        nodeNoteID2.LastNode.Tag = template.Attributes[AttributeNames.Pk].Value;
                        
                    }
                }
                catch (XmlException ex)
                {
                    Globals.logAdapter.Record("EX9812774977", ex.Message + ">>" + ex.ToString(), true);            
              
                }
            }
            int x = hospital.Nodes.Count;
            for (int i = 0; i < x; i++)
            {
                if (hospital.Nodes[i].Nodes.Count == 0)
                {

                    hospital.Nodes[i].Remove();
                    x--;
                    i = 0;
                }
            }

        }
 
        public void LoadDepartmentTemplate()
        {
            
            tvTemplate.ImageList = imageList1;
            DataTable dtType = new DataTable();
            dtType.TableName = "getillname";
            TreeNode depart = null;
            //if (tvTemplate.Nodes.Count == 0)
            //{
            //    tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Department);
            //    depart = tvTemplate.Nodes[0];
            //}
            //else if (tvTemplate.Nodes.Count >= 1 && tvTemplate.Nodes.Count == 3)
            //{
            //   // tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Department);
            //    depart = tvTemplate.Nodes[1];
            //}
            //else 
            //{
            //    tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Department);
            //    depart = tvTemplate.Nodes[1];
            //}
            //if (tvTemplate.Nodes.Count != 0)
            //{
            //    for (int i = 0; i < tvTemplate.Nodes.Count; i++)
            //    {
            //        if (tvTemplate.Nodes[i].Text.Trim() != EmrConstant.StringGeneral.Department)
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            depart = tvTemplate.Nodes[i].Nodes[0];
            //            break;
            //        }
            //    }
            //    if (depart == null)
            //    {
            //        tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Department);
            //        if (tvTemplate.Nodes.Count==2)
            //        depart = tvTemplate.Nodes[1];
                    
            //    }
            //}
            //else
            //{
                tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Department);
                depart = tvTemplate.Nodes[1];
            //}
            depart.Nodes.Clear();
            depart.ForeColor = Color.Black;
            depart.ImageIndex = 3;
            departCategoryFile = MakeDepartNoteTemplateCategory();

            if (!Globals.offline)
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        XmlNode pks = null;
                        string msg = "";

                        msg = es.GetDepartTemplatePksZlg(ref pks, Globals.OpDepartID);

                        if (msg == null)
                        {
                            XmlElement templates = departCategory.CreateElement(ElementNames.NoteTemplates);
                            foreach (XmlNode pk in pks.ChildNodes)
                            {
                                DataTable dt = new DataTable();
                                dt.TableName = "getill";
                                DataSet dst = new DataSet();
                                dst = es.GetTemplateIllNameZlg(pk.InnerText.Trim());

                                dt = dst.Tables[0];
                                long primarykey = Convert.ToInt32(pk.InnerText);
                                #region Create template category
                                XmlNode template = null;
                                msg = es.GetNoteTemplateZlg(primarykey, ref template);

                                if (msg != null)
                                {
                                    MessageBox.Show(msg, EmrConstant.ErrorMessage.Warning);
                                    continue;
                                }
                                XmlElement eleTemplate = departCategory.CreateElement(ElementNames.NoteTemplate);
                                foreach (XmlAttribute att in template.Attributes)
                                {
                                    eleTemplate.SetAttribute(att.Name, att.Value);
                                }
                                ///////
                                if (dt != null && dt.Rows.Count != 0)
                                {
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        eleTemplate.SetAttribute("IllType", dt.Rows[i]["IllType"].ToString());
                                        eleTemplate.SetAttribute("IllName", dt.Rows[i]["IllName"].ToString());
                                        eleTemplate.SetAttribute("Creator", dt.Rows[i]["Creator"].ToString());
                                        eleTemplate.SetAttribute("CreateDate", dt.Rows[i]["CreateDate"].ToString());
                                        if (dt.Rows[i]["Sex"].ToString() != "")
                                        {
                                            eleTemplate.SetAttribute("Sex", dt.Rows[i]["Sex"].ToString());
                                        }

                                        eleTemplate.SetAttribute("Type", dt.Rows[i]["Type"].ToString());
                                        eleTemplate.SetAttribute("TypeName", dt.Rows[i]["TypeName"].ToString());
                                    }
                                }
                                /////////////////
                                eleTemplate.SetAttribute(AttributeNames.Pk, pk.InnerText);
                                XmlNode theader = template.SelectSingleNode(ElementNames.Header);
                                if (theader != null)
                                {
                                    XmlElement eleHeader = departCategory.CreateElement(ElementNames.Header);
                                    eleHeader.InnerXml = theader.InnerXml;
                                    eleTemplate.AppendChild(eleHeader);
                                }
                                templates.AppendChild(eleTemplate);
                                #endregion
                                string templateDocName = MakeDepartNoteTemplateDocName(pk.InnerText);
                                udt.StringToWordDocument(templateDocName, template);
                            }
                            XmlWriter writer = XmlWriter.Create(departCategoryFile);
                            templates.WriteTo(writer);
                            writer.Close();
                        }
                        else
                        {
                            MessageBox.Show(msg, EmrConstant.ErrorMessage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9612374977", ex.Message + ">>" + ex.ToString(), true);            
              
                    }
                }

            }
            if (File.Exists(departCategoryFile))
            {
                try
                {
                    departCategory.Load(departCategoryFile);
                    XmlNodeList templates = departCategory.DocumentElement.SelectNodes(ElementNames.NoteTemplate);
                    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                    {
                        DataSet dstt = es.GetIllName(Globals.OpDepartID, 0);
                        dtType = dstt.Tables[0];
                    }
                    foreach (XmlNode template in templates)
                    {
                        string name = template.Attributes[EmrConstant.AttributeNames.Name].Value;
                        string noteID = template.Attributes[EmrConstant.AttributeNames.NoteID].Value;
                        string noteName = template.Attributes[EmrConstant.AttributeNames.NoteName].Value;

                        string childID;
                        if (template.Attributes[EmrConstant.AttributeNames.ChildID] == null || template.Attributes[EmrConstant.AttributeNames.ChildID].Value == "") childID = "0";
                        else childID = template.Attributes[EmrConstant.AttributeNames.ChildID].Value;
                        if (template.Attributes[EmrConstant.AttributeNames.ChildID] == null || template.Attributes[EmrConstant.AttributeNames.ChildID].Value == "") childID = "0";
                        TreeNode nodeNoteID = GetNodeNoteID(depart, noteID, noteName);

                        string childName = "";
                        if (childID == "0" || childID == noteID || childID == "") childName = noteName;
                        else childName = childPattern.GetNoteNameFromNoteID(childID);
                        TreeNode nodeNoteID2 = GetNodeNoteID(nodeNoteID, childID, childName);
                        nodeNoteID2.Nodes.Add(name); nodeNoteID2.LastNode.ForeColor = Color.BlueViolet;
                        nodeNoteID2.LastNode.ImageIndex = WhichImageIndex(template.Attributes[AttributeNames.Sex].Value);
                        nodeNoteID2.LastNode.Name = EmrConstant.StringGeneral.DepartTemplate;
                        nodeNoteID2.LastNode.Tag = template.Attributes[AttributeNames.Pk].Value;
                    }

                }
                catch (XmlException ex)
                {
                    Globals.logAdapter.Record("EX9814374977", ex.Message + ">>" + ex.ToString(), true);            
              
                }

            }
            int x = depart.Nodes.Count;
            for (int i = 0; i < depart.Nodes.Count; i++)
            {
                if (depart.Nodes[i].Nodes.Count == 0)
                {

                    depart.Nodes[i].Remove();
                    x--;
                    i = 0;

                }
            }
        }
        public bool UpdateTemplate(Word.Application WordApp)
        {
            long pk = Convert.ToInt32(tvTemplate.SelectedNode.Tag);
          string templateType= tvTemplate.SelectedNode.Name;
          XmlNode template = FindTemplate(pk, templateType);
            if (template == null) return false;
            /* Create xml element witch InnerText from Word window.*/
            XmlNode newTemplate = template.Clone();
            XmlElement content = template.OwnerDocument.CreateElement(ElementNames.Content);
            newTemplate.AppendChild(content);
            WordToElement(content, WordApp);
            /* Update the template in database. */
            newTemplate.Attributes.Remove(template.Attributes[AttributeNames.Pk]);
            if (!UpdateNoteTemplate(pk, newTemplate)) return false;
            /* Set this template to be selected node of template treeview. */
            string templateName = template.Attributes[AttributeNames.Name].Value;
            SetTvTemplateSelectedNode(templateType, templateName);
            /* Make docment for this template from word window. */
            WordDocFromWordWindow(pk, templateType,WordApp);
            return true;
        }
        public void SetTvTemplateSelectedNode(string templateType, string templateName)
        {
            TreeNode rootNode = tvTemplate.Nodes[0];
            if (templateType == EmrConstant.StringGeneral.PersonTemplate) rootNode = tvTemplate.Nodes[1];
            foreach (TreeNode noteID in rootNode.Nodes)
            {
                foreach (TreeNode nameNode in noteID.Nodes)
                {
                    if (nameNode.Text == templateName)
                    {
                        tvTemplate.SelectedNode = nameNode;
                        return;
                    }
                }
            }
            return;
        }
        private bool UpdateNoteTemplate(long pk, XmlNode newTemplate)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    bool ret = false;
                    ret = es.UpdateNoteTemplateZlg(pk, newTemplate);
                   
                    if (!ret)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9822374977", ex.Message + ">>" + ex.ToString(), true);            
              
                }
                return true;
            }

        }
        private void WordDocFromWordWindow(long pk, string templateType, Word.Application wordApp)
        {
            string wdDocName = null;
            if (templateType == EmrConstant.StringGeneral.DepartTemplate)
                wdDocName = MakeDepartNoteTemplateDocName(pk.ToString());
            if (templateType == EmrConstant.StringGeneral.PersonTemplate)
                wdDocName = MakePersonNoteTemplateDocName(Globals.DoctorID, pk.ToString());
            if (templateType == EmrConstant.StringGeneral.HospitalTemplate)
                wdDocName = MakeHospitalNoteTemplateDocName(pk.ToString());
            try
            {
                udt.jj.SaveWordDoc(ActiveDocumentManager.getDefaultAD(), wdDocName, false);
                //Globals.docPath = this.ActiveDocumentManager.getDefaultAD().Path;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9912374977", ex.Message + ">>" + ex.ToString(), true);            
              
            }
        }
        private void WordToElement(XmlNode noteTemplate, Word.Application wordApp)
        {
            /* Save the word window as temparory word document.*/
            string mytmp = Path.Combine(Globals.workFolder, "mytmp.docx");
            ThisAddIn.SaveWordDoc(ActiveDocumentManager.getDefaultAD(), mytmp, false);
            //Globals.docPath = this.ActiveDocumentManager.getDefaultAD().Path;
            //udt.ExportWordDoc(ActiveDocumentManager.getDefaultAD(), mytmp);
            /* Fill xml element with this stream. */
            noteTemplate.InnerText = udt.WordDocumentToString(mytmp);
        }
        public void LoadPersonTemplate()
        {
            string doctorID = Globals.DoctorID;
            TreeNode person=null;
            //if (tvTemplate.Nodes.Count == 0)
            //{
            //    tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Person);
            //    person = tvTemplate.Nodes[0];
            //}
            //else if (tvTemplate.Nodes.Count == 1)
            //{
            //    tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Person);
            //    person = tvTemplate.Nodes[1];
            //}
            //else if (tvTemplate.Nodes.Count > 2 && tvTemplate.Nodes.Count == 3)
            //{
            //    //tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Person);
            //    person = tvTemplate.Nodes[2];
            //}
            //else
            //{
            //    tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Person);
            //    person = tvTemplate.Nodes[2];
            //}
            //if (tvTemplate.Nodes.Count != 0)
            //{
            //    for (int i = 0; i < tvTemplate.Nodes.Count; i++)
            //    {
            //        if (tvTemplate.Nodes[i].Text.Trim() != EmrConstant.StringGeneral.Person)
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            person = tvTemplate.Nodes[i].Nodes[0];
            //            break;
            //        }
            //    }
            //    if (person == null)
            //    {
            //        tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Person);
            //        if(tvTemplate.Nodes.Count==3)
            //        person = tvTemplate.Nodes[2];
            //        if(tvTemplate.Nodes.Count==2)
            //        person = tvTemplate.Nodes[1];
            //    }
            //}
            //else
            //{
                tvTemplate.Nodes.Add(EmrConstant.StringGeneral.Person);
                person = tvTemplate.Nodes[2];
           // }
            person.Nodes.Clear();
            person.ForeColor = Color.Black;
            person.ImageIndex = 4;
            personCategoryFile = MakePersonNoteTemplateCategory();
            DataTable dtType = new DataTable();
            dtType.TableName = "getillname";
            if (!Globals.offline)
            {
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        XmlNode pks = null;
                        string msg = "";

                        if (ThisAddIn.CanOption(ElementNames.PersonTemplateRight) == true)
                        {
                            msg = es.GetPersonTemplatePksExZlg(ref pks, doctorID);
                        }
                        else
                        {
                            msg = es.GetPersonTemplatePksZlg(ref pks, doctorID);
                        } 
                        if (msg == null)
                        {
                            XmlElement templates = personCategory.CreateElement(ElementNames.NoteTemplates);
                            foreach (XmlNode pk in pks.ChildNodes)
                            {

                                DataTable dt = new DataTable();
                                dt.TableName = "getill";
                                DataSet dst = es.GetTemplateIllName(pk.InnerText.Trim());
                                dt = dst.Tables[0];
                                long primarykey = Convert.ToInt32(pk.InnerText);
                                #region Create template category
                                XmlNode template = null;
                                msg = es.GetNoteTemplateZlg(primarykey, ref template);

                                if (msg != null)
                                {
                                    MessageBox.Show(msg, EmrConstant.ErrorMessage.Warning);
                                    continue;
                                }
                                XmlElement eleTemplate = personCategory.CreateElement(EmrConstant.ElementNames.NoteTemplate);
                                foreach (XmlAttribute att in template.Attributes)
                                {
                                    eleTemplate.SetAttribute(att.Name, att.Value);
                                }

                                if (dt != null && dt.Rows.Count != 0)
                                {
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        eleTemplate.SetAttribute("IllType", dt.Rows[i]["IllType"].ToString());
                                        eleTemplate.SetAttribute("IllName", dt.Rows[i]["IllName"].ToString());
                                        eleTemplate.SetAttribute("Creator", dt.Rows[i]["Creator"].ToString());
                                        eleTemplate.SetAttribute("CreateDate", dt.Rows[i]["CreateDate"].ToString());
                                        if (dt.Rows[i]["Sex"].ToString() != "")
                                        {
                                            eleTemplate.SetAttribute("Sex", dt.Rows[i]["Sex"].ToString());
                                        }

                                        eleTemplate.SetAttribute("Type", dt.Rows[i]["Type"].ToString());
                                        eleTemplate.SetAttribute("TypeName", dt.Rows[i]["TypeName"].ToString());
                                    }
                                }
                                eleTemplate.SetAttribute(EmrConstant.AttributeNames.Pk, pk.InnerText);

                                XmlNode theader = template.SelectSingleNode(EmrConstant.ElementNames.Header);
                                if (theader != null)
                                {
                                    XmlElement eleHeader = personCategory.CreateElement(EmrConstant.ElementNames.Header);
                                    eleHeader.InnerXml = theader.InnerXml;
                                    eleTemplate.AppendChild(eleHeader);
                                }
                                templates.AppendChild(eleTemplate);


                                #endregion
                                string templateDocName = MakePersonNoteTemplateDocName(doctorID, pk.InnerText);
                                udt.StringToWordDocument(templateDocName, template);
                            }
                            XmlWriter writer = XmlWriter.Create(personCategoryFile);
                            templates.WriteTo(writer);
                            writer.Close();
                        }
                        else
                        {
                            MessageBox.Show(msg, EmrConstant.ErrorMessage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9912574977", ex.Message + ">>" + ex.ToString(), true);            
              
                    }
                }

            }
            #region Load tree
            if (File.Exists(personCategoryFile))
            {
                try
                {
                    personCategory.Load(personCategoryFile);
                    XmlNodeList templates = personCategory.DocumentElement.SelectNodes(ElementNames.NoteTemplate);



                    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                    {
                        DataSet dst = es.GetIllName(Globals.OpDepartID, 0);
                        dtType = dst.Tables[0];
                    }
                    foreach (XmlNode template in templates)
                    {
                        string name = template.Attributes[AttributeNames.Name].Value;
                        string noteID = template.Attributes[AttributeNames.NoteID].Value;
                        string noteName = template.Attributes[AttributeNames.NoteName].Value;
                        string childID;
                        if (template.Attributes[EmrConstant.AttributeNames.ChildID] == null || template.Attributes[EmrConstant.AttributeNames.ChildID].Value == "") childID = "0";
                        else childID = template.Attributes[EmrConstant.AttributeNames.ChildID].Value;
                        if (template.Attributes[EmrConstant.AttributeNames.ChildID] == null || template.Attributes[EmrConstant.AttributeNames.ChildID].Value == "") childID = "0";
                        TreeNode nodeNoteID = GetNodeNoteID(person, noteID, noteName);

                        string childName = "";
                        if (childID == "0" || childID == noteID || childID == "") childName = noteName;
                        else childName = childPattern.GetNoteNameFromNoteID(childID);
                        TreeNode nodeNoteID2 = GetNodeNoteID(nodeNoteID, childID, childName);
                        nodeNoteID2.Nodes.Add(name); 
                        nodeNoteID2.LastNode.ForeColor = Color.BlueViolet;
                        nodeNoteID2.LastNode.ImageIndex = WhichImageIndex(template.Attributes[AttributeNames.Sex].Value);
                        nodeNoteID2.LastNode.Name = EmrConstant.StringGeneral.PersonTemplate;
                        nodeNoteID2.LastNode.Tag = template.Attributes[AttributeNames.Pk].Value;
                    }                   


                }
                catch (XmlException ex)
                {
                    Globals.logAdapter.Record("EX9912574978", ex.Message + ">>" + ex.ToString(), true);            
              
                }
            }
            #endregion

            int x = person.Nodes.Count;
            for (int i = 0; i < x; i++)
            {
                if (person.Nodes[i].Nodes.Count == 0)
                {

                    person.Nodes[i].Remove();
                    x--;
                    i = 0;

                }
            }
        }
        public string MakeHospitalNoteTemplateCategory()
        {
         
         string fullPath = Path.Combine(Globals.templateFolder,
                StringGeneral.AllCode + EmrConstant.ResourceName.HospitalNoteTemplate);
            return fullPath;
        }
        public string MakeDepartNoteTemplateCategory()
        {
            string fullPath = Path.Combine(Globals.templateFolder,
                Globals.OpDepartID + EmrConstant.ResourceName.DepartNoteTemplate);
            return fullPath;
        }
        public string MakePersonNoteTemplateCategory()
        {
            string fullPath = Path.Combine(Globals.templateFolder,
                Globals.DoctorID + EmrConstant.ResourceName.PersonNoteTemplate);
            return fullPath;
        }
        private TreeNode GetNodeNoteID(TreeNode parent, string noteID, string noteName)
        {
           
            for (int i = 0; i < parent.Nodes.Count; i++)
            {
                //if(noteName!="病程记录")
                if (parent.Nodes[i].Name == noteID) return parent.Nodes[i];
            }

            parent.Nodes.Add(noteName);
            parent.LastNode.ForeColor = Color.DarkSlateBlue;
            parent.LastNode.ImageIndex = 6;
            parent.LastNode.Name = noteID;
            return parent.LastNode;
            
        }
        private int WhichImageIndex(string sexOption)
        {
            switch (sexOption)
            {
                case EmrConstant.StringGeneral.Both:
                    return 5;
                case "女":
                    return 2;
                case "男":
                    return 1;
            }
            return 5;
        }
        private string MakeHospitalNoteTemplateDocName(string pk)
        {
            string fullPath = Path.Combine(Globals.templateFolder,
                StringGeneral.AllCode + pk + EmrConstant.ResourceName.NoteTemplateDoc);
            return fullPath;
        }
        private string MakeDepartNoteTemplateDocName(string pk)
        {
            string fullPath = Path.Combine(Globals.templateFolder,
                Globals.OpDepartID + pk + EmrConstant.ResourceName.NoteTemplateDoc);
            return fullPath;
        }
        private string MakePersonNoteTemplateDocName(string doctorID, string pk)
        {
            string fullPath = Path.Combine(Globals.templateFolder,
                doctorID + pk + EmrConstant.ResourceName.NoteTemplateDoc);
            return fullPath;
        }
        private void open_Click(object sender, EventArgs e)
        {
            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Open)) return;
            
            if (tvTemplate.SelectedNode == null) return;
            if (tvTemplate.SelectedNode.Name == StringGeneral.PersonTemplate && Globals.DoctorID != StringGeneral.supperUser)
            {
                //if (ThisAddIn.CanOptions(ElementNames.PersonTemplateRight))
               // return;
            }
            if (tvTemplate.SelectedNode.Name == StringGeneral.HospitalTemplate && Globals.DoctorID != StringGeneral.supperUser)
            {
                if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.AsHospitalTemplates)) 
                return;
            }
            if (tvTemplate.SelectedNode.Name == StringGeneral.DepartTemplate && Globals.DoctorID != StringGeneral.supperUser)
            {
                if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.AsDepartTemplates)) 
                return;
            }
            if (tvTemplate.SelectedNode.Level == 2 || tvTemplate.SelectedNode.Level == 3)
            {
                if (tvTemplate.SelectedNode.Level == 2 && tvTemplate.SelectedNode.Nodes.Count != 0)
                    return;
                string strNodeID = tvTemplate.SelectedNode.Parent.Parent.Name;
                Globals.NoteID = strNodeID;
                string childID = tvTemplate.SelectedNode.Parent.Name;
                string noteName = tvTemplate.SelectedNode.Parent.Text;
                NoteInfo NoteID = new NoteInfo(strNodeID, childID, noteName, true);
                XmlNode template = null;
                long pk = 0;
                string type = "";
                GetTemplateName(ref template,ref pk,ref type);
                OpenTemplate(template, pk, type, true);
                ThisAddIn.ResetBeginTime();
            }

        }

        private void EmrTemplate_Load(object sender, EventArgs e)
        {
            LoadHospitalTemplate();
            LoadDepartmentTemplate();
            LoadPersonTemplate();
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (tvTemplate.SelectedNode.Level == 3)
            {
                if (tvTemplate.SelectedNode.Name == StringGeneral.HospitalTemplate && Globals.DoctorID != StringGeneral.supperUser)
                {
                    if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.AsHospitalTemplates))
                        return;
                }
                if (tvTemplate.SelectedNode.Name == StringGeneral.DepartTemplate && Globals.DoctorID != StringGeneral.supperUser)
                {
                    if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.AsDepartTemplates))
                        return;
                }
            }
            else return;
            /* If tvTemplate level = 2, the selected node is a note template. */
            ThisAddIn.ResetBeginTime();
            if (tvTemplate.SelectedNode == null) return;

            if (tvTemplate.SelectedNode.Level == 0) return;
            if (tvTemplate.SelectedNode.Level == 1) return;
            if (tvTemplate.SelectedNode.Level == 2 && tvTemplate.SelectedNode.Nodes.Count != 0) return;
         

            long pk = Convert.ToInt32(tvTemplate.SelectedNode.Tag);
            XmlNode template = FindTemplate(pk, tvTemplate.SelectedNode.Name);
            if (template == null) return;

            if (MessageBox.Show(EmrConstant.ErrorMessage.ConfirmDeleteTemplate, EmrConstant.ErrorMessage.Warning,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Cancel) return;


            if (RemoveNoteTemplate(pk))
            {
                XmlNode parent = template.ParentNode;
                parent.RemoveChild(template);
                string category = null;
                string wdDocname = null;
                MakeTemplateCategory(tvTemplate.SelectedNode, ref category, ref wdDocname);
                parent.OwnerDocument.Save(category);
                File.Delete(wdDocname);
                if (tvTemplate.SelectedNode.Parent.Nodes.Count == 1)
                {
                    if (tvTemplate.SelectedNode.Level == 2) tvTemplate.SelectedNode.Parent.Remove();
                    else
                    {
                        if (tvTemplate.SelectedNode.Parent.Parent.Nodes.Count == 1)
                            tvTemplate.SelectedNode.Parent.Parent.Remove();
                        else tvTemplate.SelectedNode.Parent.Remove();
                    }
                }
                else
                tvTemplate.SelectedNode.Remove();
            }
            ThisAddIn.ResetBeginTime();
        }
        public bool RemoveNoteTemplate(long pk)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {

                  es.RemoveNoteTemplateZlg(pk);
                    return true;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9112574977", ex.Message + ">>" + ex.ToString(), true);            
              
                    return false;
                }
            }

        }
        private void MakeTemplateCategory(TreeNode templateNode, ref string templateCategory, ref string wdDocname)
        {
            if (templateNode.Name == StringGeneral.DepartTemplate)
            {
                //templateCategory = MakeDepartNoteTemplateCategory();
                templateCategory = departCategoryFile;
                wdDocname = MakeDepartNoteTemplateDocName(templateNode.Tag.ToString());
            }
            if (templateNode.Name == StringGeneral.PersonTemplate)
            {
                //templateCategory = MakePersonNoteTemplateCategory(Globals.ThisAddIn.logon.GetOpcode());
                templateCategory = personCategoryFile;
                wdDocname = MakePersonNoteTemplateDocName(Globals.DoctorID,
                    templateNode.Tag.ToString());
            }
            if (templateNode.Name == StringGeneral.HospitalTemplate)
            {
                //templateCategory = MakePersonNoteTemplateCategory(Globals.ThisAddIn.logon.GetOpcode());
                templateCategory = hospitalCategoryFile;
                wdDocname = MakeHospitalNoteTemplateDocName(
                    templateNode.Tag.ToString());
            }
        }
        private void menuTemplate_Opening(object sender, CancelEventArgs e)
        {
            if (tvTemplate.SelectedNode.Level == 2 | tvTemplate.SelectedNode.Level == 3)
            {
                menuTemplate.Items[EmrConstant.MyMenuItems.Open].Enabled = true;
                menuTemplate.Items[EmrConstant.MyMenuItems.Use].Enabled = true;
                menuTemplate.Items[EmrConstant.MyMenuItems.Delete].Enabled = true;
      
            }
            else
            {
                menuTemplate.Items[EmrConstant.MyMenuItems.Open].Enabled = false;
                menuTemplate.Items[EmrConstant.MyMenuItems.Use].Enabled = false;
                menuTemplate.Items[EmrConstant.MyMenuItems.Delete].Enabled = false;
             

            }
        }
        private XmlElement FindTemplate(long pk, string templateType)
        {
            XmlNodeList templates = null;
            if (templateType == StringGeneral.DepartTemplate)
                templates = departCategory.DocumentElement.SelectNodes(ElementNames.NoteTemplate);
            if (templateType == StringGeneral.PersonTemplate)
            {
              templates = personCategory.DocumentElement.SelectNodes(ElementNames.NoteTemplate);
               
            }
            if (templateType == StringGeneral.HospitalTemplate)
                templates = hospitalCategory.DocumentElement.SelectNodes(ElementNames.NoteTemplate);
            if (templates == null) return null;

            foreach (XmlNode template in templates)
            {
                if (pk == Convert.ToInt32(template.Attributes[AttributeNames.Pk].Value))
                    return (XmlElement)template;
            }
            return null;
        }

        private void use_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Use)) return;
            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.NewNote))
            {
                MessageBox.Show(ErrorMessage.NoPrivilegeNew, ErrorMessage.Warning);
                return;
            }
            /* If tvTemplate level = 2, the selected node is a note template. */
            if (tvTemplate.SelectedNode == null) return;
            if (tvTemplate.SelectedNode.Level == 2 || tvTemplate.SelectedNode.Level == 3)
            {
                UseNewMethod();                
            }            
        }       
        private void UseNewMethod()
        {           
            string strNodeID = tvTemplate.SelectedNode.Parent.Parent.Name;
            string childID = tvTemplate.SelectedNode.Parent.Name;
            string noteName = tvTemplate.SelectedNode.Parent.Text;
            NoteInfo NoteID = new NoteInfo(strNodeID,childID,noteName,true);
            Globals.NoteID = strNodeID;
            if (Globals.RegistryID == "")
            {
                MessageBox.Show(ErrorMessage.NoOpeningEmr, ErrorMessage.Error);
                return;
            }
            XmlNode template = null;
            long pk = 0;
            string type = "";
            GetTemplateName(ref template, ref pk, ref type);
            OpenTemplate(template,pk, type, false);
            ThisAddIn.ResetBeginTime();          
        }
        public void GetTemplateName(ref XmlNode template,ref long pk,ref string tmplateType)
        {
            /* Get the template content for given pk and type. */
             pk = Convert.ToInt32(tvTemplate.SelectedNode.Tag);
            template = FindTemplate(pk, tvTemplate.SelectedNode.Name);
            if (template == null) return;
            tmplateType = tvTemplate.SelectedNode.Name;
            string templateFile = null;
            if (tvTemplate.SelectedNode.Name == StringGeneral.DepartTemplate)
                templateFile = MakeDepartNoteTemplateDocName(pk.ToString());
            if (tvTemplate.SelectedNode.Name == StringGeneral.PersonTemplate)
                templateFile = MakePersonNoteTemplateDocName(Globals.DoctorID, pk.ToString());
            if (tvTemplate.SelectedNode.Name == StringGeneral.HospitalTemplate)
                templateFile = MakeHospitalNoteTemplateDocName(pk.ToString());
            Globals.templateFile = templateFile;
        }
        private void tvTemplate_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            use_Click(sender, e);
        }

        private void tvTemplate_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvTemplate.SelectedNode = e.Node;
        }

        private void tvTemplate_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void tvTemplate_ItemDrag(object sender, ItemDragEventArgs e)
        {
            
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void tvTemplate_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(StringGeneral.TreeNode, false))
            {
                TreeNode sourceNode = (TreeNode)e.Data.GetData(StringGeneral.TreeNode);
                if (sourceNode.Level == 0) return;
                Point pt = tvTemplate.PointToClient(new Point(e.X, e.Y));
                TreeNode destinationNode = tvTemplate.GetNodeAt(pt);
                if (destinationNode.Level == 0) return;

                if (sourceNode.Level == 1 && destinationNode.Level == 1)
                {
                    if (destinationNode.Parent.Index == 0)
                    {
                        if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Rename)) return;
                    }
                    if (!sourceNode.Parent.Equals(destinationNode.Parent)) return;
                    bool ret = false;
                    if (sourceNode.Nodes[0].Nodes.Count == 0)//3层结构
                        ret = ExchangeTemplatePk(sourceNode.Nodes[0], destinationNode.Nodes[0]);
                    else ret = ExchangeTemplatePk(sourceNode.Nodes[0].Nodes[0], destinationNode.Nodes[0].Nodes[0]);//4层结构
                   
                    #region Exchange tree nodes
                    if (ret) ReorderTreeNode(sourceNode, destinationNode);
                    #endregion
                }
                else if (sourceNode.Level == 2 && destinationNode.Level == 2)
                {
                    if (destinationNode.Parent.Parent.Index == 0)
                    {
                        if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Rename)) return;
                    }
                    if (!sourceNode.Parent.Equals(destinationNode.Parent)) return;
                    bool ret = false;
                    if (sourceNode.Nodes.Count == 0)//3层结构
                        ret = ExchangeTemplatePk(sourceNode, destinationNode);

                    else ret = ExchangeTemplatePk(sourceNode.Nodes[0], destinationNode.Nodes[0]);//4层结构
            
                    #region Exchange tree nodes
                    if (ret) ReorderTreeNode(sourceNode, destinationNode);
                    #endregion
                }
                else if (sourceNode.Level == 3 && destinationNode.Level == 3)
                {
                    if (destinationNode.Parent.Parent.Parent.Index == 0)
                    {
                        if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Rename)) return;
                    }
                    if (!sourceNode.Parent.Equals(destinationNode.Parent)) return;

                    bool ret = ExchangeTemplatePk(sourceNode, destinationNode);


                    #region Exchange tree nodes
                    if (ret) ReorderTreeNode(sourceNode, destinationNode);
                    #endregion
                }

            }

        }
        private void ReorderTreeNode(TreeNode sourceNode, TreeNode destinationNode)
        {
            if (destinationNode.Index == destinationNode.Parent.Nodes.Count - 1)
                destinationNode.Parent.Nodes.Add((TreeNode)sourceNode.Clone());
            else
                destinationNode.Parent.Nodes.Insert(destinationNode.Index, (TreeNode)sourceNode.Clone());
            sourceNode.Remove(); //Remove original node 
        }
        private bool ExchangeTemplatePk(TreeNode sourceNode, TreeNode destinationNode)
        {
            #region Exchange template word document name based pk
            string templateCategory = null;
            string wdSourceDocName = null;
            string wdDestinationDocName = null;
            MakeTemplateCategory(sourceNode, ref templateCategory, ref wdSourceDocName);
            MakeTemplateCategory(destinationNode, ref templateCategory, ref wdDestinationDocName);
            string middleDocName = wdSourceDocName + ".mid";
            if (!File.Exists(wdSourceDocName))
            {
                MessageBox.Show(EmrConstant.ErrorMessage.SystemError, EmrConstant.ErrorMessage.Error);
                return false;
            }
            if (!File.Exists(wdDestinationDocName))
            {
                MessageBox.Show(EmrConstant.ErrorMessage.SystemError, EmrConstant.ErrorMessage.Error);
                return false;
            }
            if (File.Exists(middleDocName)) File.Delete(middleDocName);
            File.Move(wdSourceDocName, middleDocName);
            File.Move(wdDestinationDocName, wdSourceDocName);
            File.Move(middleDocName, wdDestinationDocName);
            #endregion

            #region Exchange template pk in template category
            if (!File.Exists(templateCategory))
            {
                MessageBox.Show(EmrConstant.ErrorMessage.SystemError, EmrConstant.ErrorMessage.Error);
                return false;
            }
            XmlDocument doc = new XmlDocument();
            XmlNode sourceTemplate = null;
            XmlNode destinationTemplate = null;
            try
            {
                doc.Load(templateCategory);

                XmlNodeList templates =
                    doc.DocumentElement.SelectNodes(EmrConstant.ElementNames.NoteTemplate);
                foreach (XmlNode template in templates)
                {
                    if (template.Attributes[EmrConstant.AttributeNames.Pk].Value
                        == sourceNode.Tag.ToString())
                    {
                        sourceTemplate = template;
                        break;
                    }
                }
                foreach (XmlNode template in templates)
                {
                    if (template.Attributes[EmrConstant.AttributeNames.Pk].Value
                        == destinationNode.Tag.ToString())
                    {
                        destinationTemplate = template;
                        break;
                    }
                }
                sourceTemplate.Attributes[EmrConstant.AttributeNames.Pk].Value = destinationNode.Tag.ToString();
                destinationTemplate.Attributes[EmrConstant.AttributeNames.Pk].Value = sourceNode.Tag.ToString();

            }
            catch (XmlException ex)
            {
                Globals.logAdapter.Record("EX9112474977", ex.Message + ">>" + ex.ToString(), true);            
              
                return false;
            }

            #endregion

            #region Update database
            object pkSource = sourceNode.Tag;
            sourceNode.Tag = destinationNode.Tag;
            destinationNode.Tag = pkSource;
            long sourcePK =
                Convert.ToInt32(sourceTemplate.Attributes[EmrConstant.AttributeNames.Pk].Value);
            XmlNode sourceTemplateDB = sourceTemplate.Clone();
            sourceTemplateDB.Attributes.Remove(sourceTemplate.Attributes[EmrConstant.AttributeNames.Pk]);
            XmlNode sourceContent = sourceTemplateDB.SelectSingleNode(ElementNames.Content);
            if (sourceContent == null)
            {
                sourceContent = sourceTemplateDB.OwnerDocument.CreateElement(ElementNames.Content);
                sourceTemplateDB.AppendChild(sourceContent);
            }
            sourceContent.InnerText = udt.WordDocumentToString(wdDestinationDocName);
            long destinationPK =
                Convert.ToInt32(destinationTemplate.Attributes[AttributeNames.Pk].Value);
            XmlNode destinationTemplateDB = destinationTemplate.Clone();
            destinationTemplateDB.Attributes.Remove(sourceTemplate.Attributes[AttributeNames.Pk]);
            XmlNode destinationContent = destinationTemplateDB.SelectSingleNode(ElementNames.Content);
            if (destinationContent == null)
            {
                destinationContent = destinationTemplateDB.OwnerDocument.CreateElement(ElementNames.Content);
                destinationTemplateDB.AppendChild(destinationContent);
            }
            destinationContent.InnerText = udt.WordDocumentToString(wdSourceDocName);
            bool ret1 = false;
            bool ret2 = false;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                   
                        ret1 = es.UpdateNoteTemplateZlg(sourcePK, sourceTemplateDB);
                        ret2 = es.UpdateNoteTemplateZlg(destinationPK, destinationTemplateDB);
                  

                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9112474979", ex.Message + ">>" + ex.ToString(), true);            
                                                    
                }
            }
            if (!ret1 || !ret2)
            {
                MessageBox.Show(EmrConstant.ErrorMessage.WebServiceError, EmrConstant.ErrorMessage.Error);
                return false;
            }
            #endregion

            doc.Save(templateCategory);
            return true;
        }
        public long AddNewTemplate(MainForm emrTaskPane,string templateType, XmlNode template, string IllType, string IllName, string Creator, string CreateDate, string Sex, string Type, string TypeName, string NoteID, string NoteName, string TemplateName)
        {
            TreeNode parent = null;
            XmlDocument templateCategory = null;
            string templateCategoryFile = null;
            string doctorID = null;
            string departmentCode = null;

            if (templateType == StringGeneral.DepartTemplate)
            {
                parent = tvTemplate.Nodes[0];
                templateCategoryFile = departCategoryFile;
                templateCategory = departCategory;
                doctorID = StringGeneral.NullCode;
                departmentCode = Globals.OpDepartID;
            }
            if (templateType == StringGeneral.HospitalTemplate)
            {
                parent = tvTemplate.Nodes[1];
                templateCategoryFile = hospitalCategoryFile;
                templateCategory = hospitalCategory;
                doctorID = StringGeneral.AllCode;
                departmentCode = StringGeneral.AllCode;
            }
            if (templateType != StringGeneral.DepartTemplate && templateType != StringGeneral.HospitalTemplate)
            {
                parent = tvTemplate.Nodes[2];
                templateCategoryFile = personCategoryFile;
                templateCategory = personCategory;
                doctorID = Globals.DoctorID;
                departmentCode = StringGeneral.NullCode;
            }
            /* Create xml element with InnerText from  the word window.*/
            XmlElement templateContent = template.OwnerDocument.CreateElement(ElementNames.Content);
            template.AppendChild(templateContent);
            ////  ActiveDocumentManager.getDefaultAD().AcceptAllRevisions();
            WordToElement(templateContent, emrTaskPane.wordApp);
            /* Save the new note template into database. */
            long pk = ThisAddIn.PutNoteTemplate(doctorID, departmentCode, template, IllType, IllName, Creator, CreateDate, Sex, Type, TypeName, NoteID, NoteName, TemplateName);
            if (pk < 0) return pk;
            /* Add the new tree node for the new template. */
            bool Flag = false;
            int i;
            for (i = 0; i < parent.Nodes.Count; i++)
            {

                if (IllName == parent.Nodes[i].Text)
                {

                    TreeNode nodeNoteID1 = GetNodeNoteID(parent.Nodes[i], template.Attributes[EmrConstant.AttributeNames.NoteID].Value, template.Attributes[EmrConstant.AttributeNames.NoteName].Value);
                    TreeNode nodeNoteID = new TreeNode();
                    nodeNoteID.Text = template.Attributes[EmrConstant.AttributeNames.Name].Value.ToString();
                    nodeNoteID.ForeColor = Color.BlueViolet;
                    nodeNoteID.ImageIndex =
                        WhichImageIndex(template.Attributes[EmrConstant.AttributeNames.Sex].Value);
                    nodeNoteID.Name = templateType;
                    nodeNoteID.Tag = pk.ToString();
                    nodeNoteID1.Nodes.Add(nodeNoteID);
                    tvTemplate.SelectedNode = nodeNoteID;
                    Flag = false;
                    i = parent.Nodes.Count;
                    break;
                }
                else
                {
                    Flag = true;
                }
            }
            if (i == 0)
            {
                Flag = true;
            }
            if (Flag == true)
            {

                TreeNode nodeNoteID2 = new TreeNode();
                nodeNoteID2.Text = IllName;
                parent.Nodes.Add(nodeNoteID2);
                tvTemplate.SelectedNode = nodeNoteID2;
                TreeNode nodeNoteID1 = GetNodeNoteID(tvTemplate.SelectedNode, NoteID, NoteName);
                TreeNode nodeNoteID = new TreeNode();
                nodeNoteID.Text = TemplateName;
                nodeNoteID.ForeColor = Color.BlueViolet;
                nodeNoteID.ImageIndex =
                    WhichImageIndex(Sex);
                nodeNoteID.Name = templateType;
                nodeNoteID.Tag = pk.ToString();
                nodeNoteID1.Nodes.Add(nodeNoteID);
                tvTemplate.SelectedNode = nodeNoteID;
            }
            /* Add the new template into category file in local starage. */
            XmlElement newTemplate = templateCategory.CreateElement(ElementNames.NoteTemplate);
            foreach (XmlAttribute att in template.Attributes)
            {
                newTemplate.SetAttribute(att.Name, att.Value);
            }
            XmlNode theader = template.SelectSingleNode(ElementNames.Header);
            if (theader != null)
            {
                XmlElement eleHeader = templateCategory.CreateElement(ElementNames.Header);
                eleHeader.InnerXml = theader.InnerXml;
                newTemplate.AppendChild(eleHeader);
            }
            newTemplate.SetAttribute(AttributeNames.Pk, pk.ToString());
            templateCategory.DocumentElement.AppendChild(newTemplate);
            templateCategory.Save(templateCategoryFile);
            /* Create word document for the new template.*/

            LoadCurrentNote(emrTaskPane);
            return pk;
        }       
        private void LoadCurrentNote(MainForm emrTaskPane)
        {
            long pk;
            pk = Convert.ToInt32(tvTemplate.SelectedNode.Tag);
            XmlNode template = FindTemplate(pk, tvTemplate.SelectedNode.Name);
            if (template == null) return ;
            EmrConstant.AuthorInfo authorInfo = new AuthorInfo(); ;
            EmrConstant.PatientInfo patientInfo = new PatientInfo();
            authorInfo.NoteID = template.Attributes[AttributeNames.NoteID].Value;
            authorInfo.NoteName = template.Attributes[AttributeNames.NoteName].Value;
            authorInfo.Writer = Globals.DoctorName;
            authorInfo.WrittenDate = ThisAddIn.Today().ToString(StringGeneral.DateFormat);
            authorInfo.Checker = string.Empty;
            authorInfo.CheckedDate = string.Empty;
            authorInfo.FinalChecker = string.Empty;
            authorInfo.FinalCheckedDate = string.Empty;
            authorInfo.TemplateType = tvTemplate.SelectedNode.Name;
            authorInfo.TemplateName = template.Attributes[AttributeNames.Name].Value;
            authorInfo.WriterLable = template.Attributes[AttributeNames.Sign3].Value;
            if (template.Attributes[AttributeNames.Sign2] != null)
                authorInfo.CheckerLable = template.Attributes[AttributeNames.Sign2].Value;
            else
                authorInfo.CheckerLable = string.Empty;
            if (template.Attributes[AttributeNames.Sign1] != null)
                authorInfo.FinalCheckerLable = template.Attributes[AttributeNames.Sign1].Value;
            else
                authorInfo.FinalCheckerLable = string.Empty;           
          
            emrTaskPane.currentNote =
                new EmrNote(authorInfo, (XmlElement)template, NoteEditMode.Writing, emrTaskPane.GetRegistryID(), emrTaskPane);
            emrTaskPane.currentNote.pk = pk;
        }
        public void TvTemplateEnable(bool status) // 2012-03-27 LiuQi 模板列表显示功能
        {
            tvTemplate.Enabled = status;
        }
        public string MakePersonNoteTemplateCategory(string doctorID)
        {
            string fullPath = Path.Combine(Globals.templateFolder,
                doctorID + EmrConstant.ResourceName.PersonNoteTemplate);
            return fullPath;
        }
        public void LoadTemplateNames(ArrayList templateNames, string templateType)
        {
            TreeNode rootNode = tvTemplate.Nodes[0];
            if (tvTemplate.Nodes.Count > 2)
            {
                if (templateType == EmrConstant.StringGeneral.PersonTemplate) rootNode = tvTemplate.Nodes[2];
                if (templateType == EmrConstant.StringGeneral.DepartTemplate) rootNode = tvTemplate.Nodes[1];
            }
            foreach (TreeNode noteID in rootNode.Nodes)
            {
                foreach (TreeNode nameNode in noteID.Nodes)
                {
                    if (nameNode.Nodes.Count == 0)
                    {
                        templateNames.Add(nameNode.Text);
                    }
                    else
                    {
                        foreach (TreeNode Leaf in nameNode.Nodes)
                        {
                            templateNames.Add(Leaf.Text);
                        }
                    }
                }
            }
        }
        public long AddNewTemplateOld(MainForm emrTaskPane,string templateType, XmlNode template)
        {
            TreeNode parent = null;
            XmlDocument templateCategory = null;
            string templateCategoryFile = null;
            string doctorID = null;
            string departmentCode = null;

            if (templateType == StringGeneral.DepartTemplate)
            {
                parent = tvTemplate.Nodes[1];
                templateCategoryFile = departCategoryFile;
                templateCategory = departCategory;
                doctorID = StringGeneral.NullCode;
                departmentCode = Globals.OpDepartID;
            }
            if (templateType == StringGeneral.HospitalTemplate)
            {
                //if (tvTemplate.Nodes.Count > 1)
                //    parent = tvTemplate.Nodes[1];
                //else 
                parent = tvTemplate.Nodes[0];
                templateCategoryFile = hospitalCategoryFile;
                templateCategory = hospitalCategory;
                doctorID = StringGeneral.AllCode;
                departmentCode = StringGeneral.AllCode;
            }
            if (templateType == StringGeneral.PersonTemplate)
            {
                //if (tvTemplate.Nodes.Count > 2)
                //    parent = tvTemplate.Nodes[2];
                //else 
                parent = tvTemplate.Nodes[2];
                templateCategoryFile = personCategoryFile;
                templateCategory = personCategory;
                doctorID = Globals.DoctorID;
                departmentCode = StringGeneral.NullCode;
            }
            /* Create xml element with InnerText from  the word window.*/
            XmlElement templateContent = template.OwnerDocument.CreateElement(ElementNames.Content);
            template.AppendChild(templateContent);
            ///////// ActiveDocumentManager.getDefaultAD().AcceptAllRevisions();
            WordToElement(emrTaskPane.wordApp,templateContent);
            /* Save the new note template into database. */
            long pk = ThisAddIn.PutNoteTemplateOld(doctorID, departmentCode, template);
            if (pk < 0) return pk;
            /* Add the new tree node for the new template. */
            
            if (templateType == StringGeneral.PersonTemplate && ThisAddIn.CanOption(ElementNames.PersonTemplateRight) == true)
            {
            }
            else
            {
                //2012-03-12修改，保存模板节点错位，保存到相应的节点下
                string temNoteId = "";
                string temNoteName = "";
                if (template != null)
                {
                    //if (template.Attributes[EmrConstant.AttributeNames.ChildID] != null &&
                    //    template.Attributes[EmrConstant.AttributeNames.ChildID].Value!="" && 
                    //    template.Attributes[EmrConstant.AttributeNames.ChildID].Value != "0")
                    //{
                    //    temNoteId = template.Attributes[EmrConstant.AttributeNames.ChildID].Value;
                    //}
                    //else
                    if (template.Attributes[EmrConstant.AttributeNames.NoteID] != null)
                    {
                        temNoteId = template.Attributes[EmrConstant.AttributeNames.NoteID].Value;
                    }
                    temNoteName = template.Attributes[EmrConstant.AttributeNames.NoteName].Value;
                }
                TreeNode nodeNoteID = GetNodeNoteID(parent,temNoteId, temNoteName);
                nodeNoteID.Nodes.Add(template.Attributes[EmrConstant.AttributeNames.Name].Value);
                nodeNoteID.LastNode.ForeColor = Color.BlueViolet;
                nodeNoteID.LastNode.ImageIndex =
                    WhichImageIndex(template.Attributes[EmrConstant.AttributeNames.Sex].Value);
                nodeNoteID.LastNode.Name = templateType;
                nodeNoteID.LastNode.Tag = pk.ToString();
                tvTemplate.SelectedNode = nodeNoteID.LastNode;
            }
            /* Add the new template into category file in local starage. */
            XmlElement newTemplate = templateCategory.CreateElement(ElementNames.NoteTemplate);
            foreach (XmlAttribute att in template.Attributes)
            {
                newTemplate.SetAttribute(att.Name, att.Value);
            }
            XmlNode theader = template.SelectSingleNode(ElementNames.Header);
            if (theader != null)
            {
                XmlElement eleHeader = templateCategory.CreateElement(ElementNames.Header);
                eleHeader.InnerXml = theader.InnerXml;
                newTemplate.AppendChild(eleHeader);
            }
            newTemplate.SetAttribute(AttributeNames.Pk, pk.ToString());
            templateCategory.DocumentElement.AppendChild(newTemplate);
            templateCategory.Save(templateCategoryFile);
            /* Create word document for the new template.*/
            WordDocFromWordWindow(emrTaskPane.wordApp,pk, templateType);

            /* Refresh word window.*/           
            LoadCurrentNote(emrTaskPane);
            return pk;
        }
        private void WordDocFromWordWindow(Word.Application app,long pk, string templateType)
        {
            string wdDocName = null;
            if (templateType == EmrConstant.StringGeneral.DepartTemplate)
                wdDocName = MakeDepartNoteTemplateDocName(pk.ToString());
            if (templateType == EmrConstant.StringGeneral.PersonTemplate)
                wdDocName = MakePersonNoteTemplateDocName(Globals.DoctorID, pk.ToString());
            if (templateType == EmrConstant.StringGeneral.HospitalTemplate)
                wdDocName = MakeHospitalNoteTemplateDocName(pk.ToString());
            udt.jj.ExportWordDoc(ActiveDocumentManager.getDefaultAD(), wdDocName);
        }
        private void WordToElement(Word.Application wordApp,XmlNode noteTemplate)
        {
            /* Save the word window as temparory word document.*/
            string mytmp = Path.Combine(Globals.workFolder, "mytmp.docx");
            udt.jj.ExportWordDoc(ActiveDocumentManager.getDefaultAD(), mytmp);
            /* Fill xml element with this stream. */
            noteTemplate.InnerText = udt.jj.WordDocumentToString(mytmp);
        }

        private void rename_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (tvTemplate.SelectedNode.Level == 3)
            {
                if (tvTemplate.SelectedNode.Name == StringGeneral.HospitalTemplate && Globals.DoctorID == StringGeneral.supperUser)
                {
                    if (tvTemplate.SelectedNode.Parent.Parent.Parent.Index == 0 || tvTemplate.SelectedNode.Parent.Parent.Parent.Index == 1)
                    {
                        /* to rename a department template needs privelege. */
                        if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Rename)) return;
                    }
                }
                if (tvTemplate.SelectedNode.Name == StringGeneral.DepartTemplate && Globals.DoctorID == StringGeneral.supperUser)
                {
                    if (tvTemplate.SelectedNode.Parent.Parent.Parent.Index == 0 || tvTemplate.SelectedNode.Parent.Parent.Parent.Index == 1)
                    {
                        /* to rename a department template needs privelege. */
                        if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Rename)) return;
                    }
                }
            }
           
            //if (ThisAddIn.offline)
            //{
            //    MessageBox.Show(EmrConstant.ErrorMessage.NoUpdateTemplate, EmrConstant.ErrorMessage.Warning);
            //    return;
            //}

            if (tvTemplate.SelectedNode == null) return;

            //if (ThisAddIn.UseOldTemplate != true)
            //{
            //    if (tvTemplate.SelectedNode.Level != 2) return;
            //}
            //else
            //{
            //    if (tvTemplate.SelectedNode.Level == 0) return;
            //    if (tvTemplate.SelectedNode.Level == 1) return;
            //    if (tvTemplate.SelectedNode.Level == 2 && tvTemplate.SelectedNode.Nodes.Count != 0) return;
            //}

            ThisAddIn.ResetBeginTime();
            tvTemplate.LabelEdit = true;
            tvTemplate.SelectedNode.BeginEdit();

        }
    }
}
