using System;
using System.Collections.Generic;
using System.Text;
using EmrConstant;

namespace CommonLib
{
       
   public class NoteInfo
    {
        private Boolean newNote;
        private NoteEditMode editMode;
        private string noteID;
        private string childID;
        private string noteName;
        private string registryID;
        private string headerText;
        private string uniqueText;
        private string templateType;
        private string templateName;
        public bool sexOption = false;

        public NoteInfo() { }
       /****zlg****/
        public NoteInfo(string NoteID, string ChildID, string NoteName, bool newEmrNote)
        {
            childID = ChildID;
            noteID = NoteID;
            noteName =NoteName;
            newNote = newEmrNote;
            editMode = NoteEditMode.Writing;
            //registryID = patientInfo.RegistryID;
        }
        public NoteInfo(AuthorInfo authorInfo, string  registryID, Boolean newEmrNote, 
            string header, string unique)
        {
            childID = authorInfo.ChildID;
            noteID = authorInfo.NoteID;
            noteName = authorInfo.NoteName;       
            newNote = newEmrNote;
            editMode = NoteEditMode.Writing;
            templateType = authorInfo.TemplateType;
            templateName = authorInfo.TemplateName;
            this.registryID = registryID;
            headerText = header;
            uniqueText = unique;
          
        }
        public void SetNoteID(string noteid)
        {
            this.noteID = noteid;
        }
        public void SetRegistryID(string RegistryID)
        {
            registryID = RegistryID;
        }
        public void SetNewNoteFlag(bool flag)
        {
            newNote = flag;
        }
        public void SetNewNote(bool IsNew)
        {
            newNote = IsNew;
        }
        public void SetEditMode(NoteEditMode mode)
        {
            editMode = mode;
        }
        public NoteEditMode GetEditMode()
        {
            return  editMode;
        }
        public string GetRegistryID()
        {
            return registryID;
        }
        public string GetNoteID()
        {
            return noteID;
        }
        public string GetChildID()
        {
            return childID;
        }
        public string GetNoteName()
        {
            return noteName;
        }
        public bool GetNewNote()
        {
            return newNote;
        }
        public string GetNoteUniqueFlag()
        {
            return uniqueText;
        }
        public string GetNoteHeaderType()
        {
            return headerText;
        }
    }
}
