using System;
using System.Collections.Generic;

using System.Text;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using CommonLib;
using EmrConstant;
using System.Collections;
using UserInterface;

namespace EMR
{
   public partial class EmrNote
    {
       private MainForm emrTaskPane = null;
       public NoteInfo noteInfo;
       public UCAuthor author;
       public XmlNode header = null;
       public XmlNode lastConsult = null;
       public XmlNode lastOperation = null;
       public long pk;           // Be set when the note template is opening.
       public string sexOption;  // = '女', '男' and 'Both' for template sex attribute.
       public string merge;
       public string startTime = StartTime.None;
      // Word.Document myDoc = null;
      // private Word.ContentControl ccX = null;

       public EmrNote()
       {
           noteInfo = new NoteInfo();

       }
       public EmrNote(AuthorInfo authorInfo, string  regstryID, MainForm etp)
       {
           emrTaskPane = etp;

           /* Display note pattern on word window. */
           EmrPattern(authorInfo, regstryID);

           /* set writing mode */
           noteInfo.SetEditMode(NoteEditMode.Writing);

           /* Set focus */
           ResetFocus();

           /* Before some new words have been input, no save operation can be executed */
           //ActiveDocumentManager.getDefaultAD().Saved = true;
           //myDoc = ActiveDocumentManager.getDefaultAD();
           //myDoc.Saved = true;
       }

       private void ResetFocus()
       {
           foreach (Word.ContentControl cc in ActiveDocumentManager.getDefaultAD().ContentControls)
           {
               if (cc.Title == StringGeneral.Label)
               {
                   object start = cc.Range.End + 1;
                   object end = start;
                   Word.Range rg = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
                   rg.Select();
                   break;
               }
           }
       }
       private void EmrPattern(AuthorInfo authorInfo, string  registryID)
       {
           XmlNode emrNote = Globals.emrPattern.GetEmrNote(authorInfo.NoteID);
           if (emrNote == null) return;
           if (emrNote.Attributes[AttributeNames.ShowName] != null)
           {
               if (emrNote.Attributes[AttributeNames.ShowName].Value != "")
                   authorInfo.NoteName = emrNote.Attributes[AttributeNames.ShowName].Value;
           }


           emrTaskPane.wordApp.ScreenUpdating = false;
           if (emrNote.Attributes[AttributeNames.Sign3] != null)
               authorInfo.WriterLable = emrNote.Attributes[AttributeNames.Sign3].Value;
           if (emrNote.Attributes[AttributeNames.Sign2] != null)
               authorInfo.CheckerLable = emrNote.Attributes[AttributeNames.Sign2].Value;
           if (emrNote.Attributes[AttributeNames.Sign1] != null)
               authorInfo.FinalCheckerLable = emrNote.Attributes[AttributeNames.Sign1].Value;
           startTime = emrNote.Attributes[AttributeNames.StartTime].Value;
           merge = emrNote.Attributes[AttributeNames.Merge].Value;

           noteInfo = new NoteInfo(authorInfo, registryID, true,
               emrNote.Attributes[AttributeNames.Header].Value,
               emrNote.Attributes[AttributeNames.Unique].Value);

         
           //if (startTime == StartTime.Consult)
           //{
           //    /* private property lastConsult has the result. */
           //    if (Globals.PatientConsultSequence != null && Globals.PatientConsultSequence != "")
           //    {
           //        GetLastConsultFiled(noteID.GetNoteID(), Globals.PatientConsultSequence, patientInfo.RegistryID);
           //    }
           //}
           //else if (startTime == StartTime.Operation)
           //{
           //    /* private property lastConsult has the result. */


           //    GetLastOperationInfo(noteID.GetNoteID(), patientInfo.RegistryID);
           //}
          
           author = new UCAuthor(authorInfo, true,emrTaskPane);
           emrTaskPane.wordApp.ScreenRefresh();
           emrTaskPane.wordApp.ScreenUpdating = true;
           return;
       }
       //public bool AsTemplate(string doctorID, string departmentCode, int Flag)
       //{
       //    if (Globals.offline)
       //    {
       //        MessageBox.Show(ErrorMessage.NoNewTemplate, ErrorMessage.Warning);
       //        return Return.Failed;
       //    }
       //    string templateType = StringGeneral.PersonTemplate;
       //    string fullPath = null;

       //    #region Create a xml doc.
       //    XmlDocument doc = new XmlDocument();
       //    /* Load xml doc with local note template. */
       //    if (doctorID == EmrConstant.StringGeneral.NullCode)
       //    {
       //        /* As a department template */
       //        templateType = StringGeneral.DepartTemplate;
       //        fullPath = emrTaskPane.NoteTemplate.MakeDepartNoteTemplateCategory();
       //        if (File.Exists(fullPath)) doc.Load(fullPath);
       //        else doc.LoadXml("<NoteTemplates DepartmentCode=\"" + departmentCode + "\" />");
       //    }
       //    if (doctorID == EmrConstant.StringGeneral.AllCode)
       //    {
       //        /* As a hospital department template */
       //        templateType = StringGeneral.HospitalTemplate;
       //        fullPath = emrTaskPane.NoteTemplate.MakeHospitalNoteTemplateCategory();
       //        if (File.Exists(fullPath)) doc.Load(fullPath);
       //        else doc.LoadXml("<NoteTemplates HospitalCode=\"" + StringGeneral.AllCode + "\" />");
       //    }
       //    if (doctorID != EmrConstant.StringGeneral.NullCode && doctorID != EmrConstant.StringGeneral.AllCode)
       //    {
       //        /* As a person template */
       //        templateType = StringGeneral.PersonTemplate;
       //        fullPath = emrTaskPane.NoteTemplate.MakePersonNoteTemplateCategory(doctorID);
       //        if (File.Exists(fullPath)) doc.Load(fullPath);
       //        else doc.LoadXml("<NoteTemplates DoctorID=\"" + doctorID + "\" />");
       //    }
       //    #endregion

       //    #region Get tempmate name from the operator
       //    ArrayList names = new ArrayList();
       //    emrTaskPane.NoteTemplate.LoadTemplateNames(names, templateType);
       //    string NodeID1 = ThisAddIn.GetNoteIDFromWordWindow();
       //    string defaultName =
       //        Globals.emrPattern.GetNoteNameFromNoteID(NodeID1);

       //    SaveTemplate tempName = new SaveTemplate(names, defaultName, Flag);
       //    tempName.StartPosition = FormStartPosition.CenterScreen;
       //    if (tempName.ShowDialog() == DialogResult.Cancel) return false;

       //    string templateName = tempName.Get();
       //    string Creator = Globals.DoctorID;
       //    string CreateDate = DateTime.Now.ToString();
       //    string IllType = tempName.GetIllType();
       //    string IllName = tempName.GetIllName();
       //    string Type = tempName.GetTypes();
       //    string TypeName = tempName.GetTypeName();
       //    string Sex = tempName.GetSex();
       //    string NoteName = Globals.emrPattern.GetNoteNameFromNoteID(NodeID1);
       //    string TemplateName = tempName.GetTemplateName();

       //    if (noteInfo.GetEditMode() == NoteEditMode.Reading)
       //    {
       //        ThisAddIn.SetReadWrite();
       //        noteInfo.SetEditMode(NoteEditMode.Writing);
       //    }
       //    #endregion
       //    #region Create attributs for the new template
       //    XmlElement noteTemplate = doc.CreateElement(EmrConstant.ElementNames.NoteTemplate);
       //    noteTemplate.SetAttribute(AttributeNames.Name, templateName);
       //    noteTemplate.SetAttribute(AttributeNames.NoteID, NodeID1);
       //    noteTemplate.SetAttribute(AttributeNames.NoteName, NoteName);
       //    noteTemplate.SetAttribute(AttributeNames.Header, noteInfo.GetNoteHeaderType());
       //    noteTemplate.SetAttribute(AttributeNames.Unique, noteInfo.GetNoteUniqueFlag());
       //    if (author.finalCheckerLable != string.Empty)
       //        noteTemplate.SetAttribute(AttributeNames.Sign1, author.finalCheckerLable);
       //    if (author.checkerLable != string.Empty)
       //        noteTemplate.SetAttribute(AttributeNames.Sign2, author.checkerLable);
       //    noteTemplate.SetAttribute(AttributeNames.Sign3, author.writerLable);
       //    noteTemplate.SetAttribute(AttributeNames.Merge, merge);

       //    /// Special note treatment
       //    noteTemplate.SetAttribute(AttributeNames.StartTime, startTime);

       //    if (noteInfo.sexOption)
       //        noteTemplate.SetAttribute(AttributeNames.Sex, emrTaskPane.GetPatientSex());
       //    else
       //        noteTemplate.SetAttribute(AttributeNames.Sex, StringGeneral.Both);

       //    noteTemplate.SetAttribute(AttributeNames.IllName, IllName);
       //    noteTemplate.SetAttribute(AttributeNames.IllType, IllType);
       //    noteTemplate.SetAttribute(AttributeNames.Type, Type);
       //    noteTemplate.SetAttribute(AttributeNames.TypeName, TypeName);
       //    if (noteInfo.GetNoteHeaderType() == StringGeneral.None)
       //    {
       //        XmlNode notePattern = Globals.emrPattern.GetEmrNote(noteInfo.GetNoteID());
       //        if (notePattern != null)
       //        {
       //            XmlElement header = doc.CreateElement(ElementNames.Header);
       //            foreach (XmlNode subtitle in notePattern.SelectNodes(ElementNames.SubTitle))
       //            {
       //                if (subtitle.Attributes[AttributeNames.Value] == null) continue;

       //                XmlElement newSubtitle = doc.CreateElement(ElementNames.SubTitle);
       //                foreach (XmlAttribute att in subtitle.Attributes)
       //                {
       //                    newSubtitle.SetAttribute(att.Name, att.Value);
       //                }
       //                newSubtitle.InnerXml = subtitle.InnerXml;
       //                header.AppendChild(newSubtitle);
       //            }
       //            noteTemplate.AppendChild(header);
       //        }
       //    }
       //    #endregion
       //    #region Save the new template into dadabase and add new template to the template tree.
       //    if (emrTaskPane.NoteTemplate.AddNewTemplate(emrTaskPane, templateType, noteTemplate, IllType, IllName, Creator, CreateDate, Sex, Type, TypeName, NodeID1, NoteName, templateName) < 0) return Return.Failed;
       //    #endregion

       //    ActiveDocumentManager.getDefaultAD().Saved = true;
       //    emrTaskPane.TemplateOperationEnable(true);
       //    return Return.Successful;
       //}
      
       public bool AsTemplateOld(string doctorID, string departmentCode)
       {
           if (Globals.offline)
           {
               MessageBox.Show(ErrorMessage.NoNewTemplate, ErrorMessage.Warning);
               return Return.Failed;
           }
           string templateType = StringGeneral.PersonTemplate;
           string fullPath = null;
           #region Create a xml doc.
           XmlDocument doc = new XmlDocument();
           /* Load xml doc with local note template. */
           if (doctorID == EmrConstant.StringGeneral.NullCode)
           {
               /* As a department template */
               templateType = StringGeneral.DepartTemplate;
               fullPath = emrTaskPane.NoteTemplate.MakeDepartNoteTemplateCategory();
               if (File.Exists(fullPath)) doc.Load(fullPath);
               else doc.LoadXml("<NoteTemplates DepartmentCode=\"" + departmentCode + "\" />");
           }
           if (doctorID == EmrConstant.StringGeneral.AllCode)
           {
               /* As a hospital department template */
               templateType = StringGeneral.HospitalTemplate;
               fullPath = emrTaskPane.NoteTemplate.MakeHospitalNoteTemplateCategory();
               if (File.Exists(fullPath)) doc.Load(fullPath);
               else doc.LoadXml("<NoteTemplates HospitalCode=\"" + StringGeneral.AllCode + "\" />");
           }
           if (doctorID != EmrConstant.StringGeneral.NullCode && doctorID != EmrConstant.StringGeneral.AllCode)
           {
               /* As a person template */
               templateType = StringGeneral.PersonTemplate;
               fullPath = emrTaskPane.NoteTemplate.MakePersonNoteTemplateCategory(doctorID);
               if (File.Exists(fullPath)) doc.Load(fullPath);
               else doc.LoadXml("<NoteTemplates DoctorID=\"" + doctorID + "\" />");
           }
           #endregion

           #region Get tempmate name from the operator
           ArrayList names = new ArrayList();
           emrTaskPane.NoteTemplate.LoadTemplateNames(names, templateType);
           string childID = "";
           string NodeID1 = emrTaskPane.GetNoteIDFromWordWindow(ref childID);
           if (NodeID1 == childID || childID == "") childID = "0";
           string NoteName = Globals.emrPattern.GetNoteNameFromNoteID(NodeID1);
           string defaultName = "";
           if (NodeID1 == "0") defaultName = NoteName;
           else
           {
               defaultName = Globals.childPattern.GetNoteNameFromNoteID(childID);
               if (defaultName == "" || defaultName == null) defaultName = NoteName;
           }
           TemplateName tempName = new TemplateName(names, defaultName);
           tempName.StartPosition = FormStartPosition.CenterScreen;
           if (tempName.ShowDialog() == DialogResult.Cancel) return false;

           string templateName = tempName.Get();
           //templateName += " *";
           if (noteInfo.GetEditMode() == NoteEditMode.Reading)
           {
               emrTaskPane.SetReadWrite();
               noteInfo.SetEditMode(NoteEditMode.Writing);
           }

           /* Remove revisions. */
           //  ActiveDocumentManager.getDefaultAD().AcceptAllRevisions();
           #endregion

           #region Create attributs for the new template
           XmlElement noteTemplate = doc.CreateElement(EmrConstant.ElementNames.NoteTemplate);
           noteTemplate.SetAttribute(AttributeNames.Name, templateName);
           noteTemplate.SetAttribute(AttributeNames.NoteID, NodeID1);
           noteTemplate.SetAttribute(AttributeNames.NoteName, NoteName);
           noteTemplate.SetAttribute(AttributeNames.Header, noteInfo.GetNoteHeaderType());
           noteTemplate.SetAttribute(AttributeNames.Unique, noteInfo.GetNoteUniqueFlag());
           noteTemplate.SetAttribute(AttributeNames.ChildID, childID);

           if (author.finalCheckerLable != string.Empty)
               noteTemplate.SetAttribute(AttributeNames.Sign1, author.finalCheckerLable);
           if (author.checkerLable != string.Empty)
               noteTemplate.SetAttribute(AttributeNames.Sign2, author.checkerLable);
           noteTemplate.SetAttribute(AttributeNames.Sign3, author.writerLable);
           noteTemplate.SetAttribute(AttributeNames.Merge, merge);

           /// Special note treatment
           noteTemplate.SetAttribute(AttributeNames.StartTime, startTime);

           if (noteInfo.sexOption)
               noteTemplate.SetAttribute(AttributeNames.Sex, emrTaskPane.GetPatientSex());
           else
               noteTemplate.SetAttribute(AttributeNames.Sex, StringGeneral.Both);

           if (noteInfo.GetNoteHeaderType() == StringGeneral.None)
           {
               XmlNode notePattern = Globals.emrPattern.GetEmrNote(noteInfo.GetNoteID());
               if (notePattern != null)
               {
                   XmlElement header = doc.CreateElement(ElementNames.Header);
                   foreach (XmlNode subtitle in notePattern.SelectNodes(ElementNames.SubTitle))
                   {
                       if (subtitle.Attributes[AttributeNames.Value] == null) continue;

                       XmlElement newSubtitle = doc.CreateElement(ElementNames.SubTitle);
                       foreach (XmlAttribute att in subtitle.Attributes)
                       {
                           newSubtitle.SetAttribute(att.Name, att.Value);
                       }
                       newSubtitle.InnerXml = subtitle.InnerXml;
                       header.AppendChild(newSubtitle);
                   }
                   noteTemplate.AppendChild(header);
               }
           }
           #endregion 
           #region Save the new template into dadabase and add new template to the template tree.
           if (emrTaskPane.NoteTemplate.AddNewTemplateOld(emrTaskPane, templateType, noteTemplate) < 0) return Return.Failed;
           #endregion

           ActiveDocumentManager.getDefaultAD().Saved = true;
           //emrTaskPane.TemplateOperationEnable(true);
           return Return.Successful;
       }
    }
}
