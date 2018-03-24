using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using EmrConstant;

namespace CommonLib
{
    public class EmrPattern
    {
        private XmlDocument docPattern = null;
        private XmlNode rules = null;
        private PutEmrPatternIntoDB putEmrPatternIntoDB = null;
        private string filePattern = null;
        //private string filePattern = null;

        public EmrPattern(string patternFile, XmlNode rls, PutEmrPatternIntoDB putEmrPattern)
        {
            if (!File.Exists(patternFile)) return;
            docPattern = new XmlDocument();
            try { docPattern.Load(patternFile); }
            catch (XmlException ex) {
                Globals.logAdapter.Record("EX756987455525", ex.Message + ">>" + ex.ToString(), true);            
             
                return; }
            rules = rls;
            putEmrPatternIntoDB = putEmrPattern;
            filePattern = patternFile;
        }
        public EmrPattern(XmlNode Pattern)
        {
            //emrPattern = Pattern;            
        }
      
        public string GetNoteNameFromNoteID(string noteID)
        {
            if (docPattern == null) return null;
            foreach (XmlNode emrNote in docPattern.DocumentElement.SelectNodes(ElementNames.EmrNote))
            {
                if (emrNote.Attributes[AttributeNames.NoteID].Value == noteID)
                    return emrNote.Attributes[AttributeNames.NoteName].Value;
            }
            return null;
        }
        public XmlNodeList GetAllEmrNotes()
        {
            if (docPattern == null) return null;
            return docPattern.DocumentElement.SelectNodes(ElementNames.EmrNote);
        }
        public string StartTimeAttribute(string noteID)
        {

            if (docPattern == null) return null;
            XmlNodeList notes = docPattern.DocumentElement.SelectNodes(ElementNames.EmrNote);
            foreach (XmlNode note in notes)
            {
                if (note.Attributes[AttributeNames.NoteID].Value == noteID)
                    return note.Attributes[AttributeNames.StartTime].Value;
            }
            return null;
        }
        public XmlNode GetEmrNote(string noteID)
        {
            if (docPattern == null) return null;

            foreach (XmlNode emrNote in GetAllEmrNotes())
            {
                if (emrNote.Attributes[AttributeNames.NoteID].Value == noteID) return emrNote;
            }
            return null;
        }
        public string EndPrintGroup(string noteID)
        {
            string groupCode = StringGeneral.Zero;
            if (docPattern == null) return groupCode;

            foreach (XmlNode emrNote in GetAllEmrNotes())
            {
                if (emrNote.Attributes[AttributeNames.NoteID].Value == noteID)
                {
                    groupCode = emrNote.Attributes[AttributeNames.Merge].Value;
                    break;
                }
            }
            return groupCode;
        }

        public XmlNodeList GetNotesRequiredInTime()
        {
            if (docPattern == null) return null;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(docPattern.OuterXml);

            XmlNodeList notes = doc.DocumentElement.SelectNodes(ElementNames.EmrNote);
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                if (notes[i].Attributes[AttributeNames.StartTime].Value == AttributeNames.None)
                {
                    doc.DocumentElement.RemoveChild(notes[i]);
                    continue;
                }
                if (notes[i].Attributes[AttributeNames.Valid] != null)
                {
                    if (notes[i].Attributes[AttributeNames.Valid].Value == StringGeneral.No)
                        doc.DocumentElement.RemoveChild(notes[i]);
                }
            }
            return doc.DocumentElement.SelectNodes(ElementNames.EmrNote);
        }

        public string TimeLimitAttribute(string noteID)
        {
            if (docPattern == null) return null;
            XmlNodeList notes = docPattern.DocumentElement.SelectNodes(ElementNames.EmrNote);
            foreach (XmlNode note in notes)
            {
                if (note.Attributes[AttributeNames.NoteID].Value == noteID)
                    return note.Attributes[AttributeNames.TimeLimit].Value;
            }
            return null;
        }
        public XmlNodeList GetEmrNotesForOutpatients()
        {
            if (docPattern == null) return null;
            XmlNode root = docPattern.DocumentElement.Clone();
            XmlNodeList notes = root.SelectNodes(ElementNames.EmrNote);
            for (int k = notes.Count - 1; k >= 0; k--)
            {
                if (notes[k].Attributes[AttributeNames.Style].Value == StringGeneral.In)
                {
                    root.RemoveChild(notes[k]);
                    continue;
                }
                if (notes[k].Attributes[AttributeNames.Valid] != null &&
                    notes[k].Attributes[AttributeNames.Valid].Value == StringGeneral.No)
                    root.RemoveChild(notes[k]);
            }
            notes = root.SelectNodes(ElementNames.EmrNote);
            return notes;
        }
        public XmlNode[] GetChildNote(string noteID)
        {
            if (docPattern == null) return null;
            int i = 0;
            XmlNode[] emrNotes = new XmlNode[GetAllEmrNotes().Count];
            foreach (XmlNode emrNote in GetAllEmrNotes())
            {
                if (emrNote.Attributes[AttributeNames.ParentID].Value == noteID)
                {
                    emrNotes[i] = emrNote;
                    i++;
                }
            }
            return emrNotes;
        }
        public string GetSubstitutes(string noteID)
        {
            foreach (XmlNode rule in rules)
            {
                if (rule.Attributes[AttributeNames.NoteID].Value == noteID)
                {
                    if (rule.Attributes[AttributeNames.Substitute] == null) return null;
                    else return rule.Attributes[AttributeNames.Substitute].Value;
                }
            }
            return null;
        }
        public XmlNode GetOneGroup(string code)
        {
            foreach (XmlNode group in GetAllEmrGroups())
            {
                if (group.Attributes[AttributeNames.Code].Value == code) return group;
            }
            return null;
        }
        public string GetNoteNameFromNoteIDEX(string noteID)
        {
            string noteID_left = null;
            if (noteID.IndexOf("-") > 0) noteID = noteID.Replace("-", ":");
            if (docPattern == null) return null;
            foreach (XmlNode emrNote in docPattern.DocumentElement.SelectNodes(ElementNames.EmrNote))
            {
                if (emrNote.Attributes["ParentID"] != null)
                {
                    noteID_left = emrNote.Attributes[AttributeNames.ParentID].Value + ":" + emrNote.Attributes[AttributeNames.NoteID].Value;
                    if (noteID_left == noteID)
                        return emrNote.Attributes[AttributeNames.NoteName].Value;
                }
                else
                {
                    if (emrNote.Attributes[AttributeNames.NoteID].Value == noteID)
                        return emrNote.Attributes[AttributeNames.NoteName].Value;
                }
            }
            return null;
        }
        public XmlNodeList GetAllEmrGroups()
        {
            if (docPattern == null) return null;
            XmlNodeList groups = docPattern.DocumentElement.SelectNodes(ElementNames.Group);
            if (groups.Count == 1)
            {
                XmlElement group = docPattern.CreateElement(ElementNames.Group);
                group.SetAttribute(AttributeNames.Code, "2");
                group.SetAttribute(AttributeNames.NoteID, "##");
                group.SetAttribute(AttributeNames.NoteName, "护理记录");
                XmlNode group2 = docPattern.DocumentElement.InsertAfter(group, groups.Item(0));

                group = docPattern.CreateElement(ElementNames.Group);
                group.SetAttribute(AttributeNames.Code, "3");
                group.SetAttribute(AttributeNames.NoteID, "##");
                group.SetAttribute(AttributeNames.NoteName, "危重护理");
                docPattern.InsertAfter(group, group2);

                putEmrPatternIntoDB(docPattern.DocumentElement);
                //emrPattern.Save(filePattern);
            }
            else if (groups.Count == 2)
            {
                XmlElement group = docPattern.CreateElement(ElementNames.Group);
                group.SetAttribute(AttributeNames.Code, "3");
                group.SetAttribute(AttributeNames.NoteID, "##");
                group.SetAttribute(AttributeNames.NoteName, "危重护理");
                docPattern.DocumentElement.InsertAfter(group, groups.Item(1));

                putEmrPatternIntoDB(docPattern);
                //docPattern.Save(filePattern);
            }
            
            foreach (XmlNode group in groups)
            {
                if (group.Attributes[AttributeNames.NoteName] == null)
                {
                    string noteID = group.Attributes[AttributeNames.NoteID].Value;
                    string noteName = GetNoteNameFromNoteID(noteID);
                    XmlAttribute attr = group.OwnerDocument.CreateAttribute(AttributeNames.NoteName);
                    attr.Value = noteName;
                    group.Attributes.Append(attr);
                    putEmrPatternIntoDB(docPattern.DocumentElement);
                   // docPattern.Save(filePattern);
                }
            }
            return docPattern.DocumentElement.SelectNodes(ElementNames.Group);
        }
    }

}
