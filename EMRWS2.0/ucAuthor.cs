using System;
using System.Data;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using CommonLib;
using EmrConstant;

namespace EMR
{
    public partial class UCAuthor
    {
        //test by sunfan 2009-06-30
        private Word.ContentControl ccWriter = null;
        private Word.ContentControl ccChecker = null;
        private Word.ContentControl ccFinalChecker = null;
        private Word.ContentControl ccWrittenDate = null;
        private Word.ContentControl ccCheckedDate = null;
        private Word.ContentControl ccFinalCheckedDate = null;
        public string writerLable;
        public string checkerLable;
        public string finalCheckerLable;
        private MainForm emrTaskPane;

        /* Display emrNote foot */
        public UCAuthor(AuthorInfo authorInfo, bool newNote,MainForm etp)
        {
            writerLable = authorInfo.WriterLable;
            checkerLable = authorInfo.CheckerLable;
            finalCheckerLable = authorInfo.FinalCheckerLable;
            //if (newNote) DrawAuthor(authorInfo);
            emrTaskPane = etp;
        }

        /* ----------------------------------------------------------------------------------------------------*/
        public void BecomeNormalNote()

        {
            AuthorInfo authorInfo=new AuthorInfo();
            authorInfo.Writer = Globals.DoctorName;
            authorInfo.WrittenDate = ThisAddIn.Today().ToString(StringGeneral.DateFormat);
            authorInfo.Checker = "";
            authorInfo.CheckedDate = "";
            authorInfo.FinalChecker = "";
            authorInfo.FinalCheckedDate = "";
            authorInfo.TemplateType = StringGeneral.NoneTemplate;
            authorInfo.TemplateName = "";
            authorInfo.NoteID = "";
            authorInfo.NoteName = "";
            authorInfo.WriterLable = emrTaskPane.currentNote.author.writerLable;
            authorInfo.CheckerLable = emrTaskPane.currentNote.author.checkerLable;
            authorInfo.FinalCheckerLable = emrTaskPane.currentNote.author.finalCheckerLable;
                       
            //DrawAuthor(authorInfo);

        }
        //public void BecomeNormalNoteOld()
        //{
        //    AuthorInfo authorInfo;
        //    authorInfo.Writer = Globals.DoctorName;
        //    authorInfo.WrittenDate = ThisAddIn.Today().ToString(StringGeneral.DateFormat);
        //    authorInfo.Checker = "";
        //    authorInfo.ChildID = string.Empty;
        //    authorInfo.CheckedDate = "";
        //    authorInfo.FinalChecker = "";
        //    authorInfo.FinalCheckedDate = "";
        //    authorInfo.TemplateType = StringGeneral.NoneTemplate;
        //    authorInfo.TemplateName = "";
        //    authorInfo.NoteID = "";
        //    authorInfo.NoteName = "";
        //    authorInfo.WriterLable = emrTaskPane.currentNote.author.writerLable;
        //    authorInfo.CheckerLable = emrTaskPane.currentNote.author.checkerLable;
        //    authorInfo.FinalCheckerLable = emrTaskPane.currentNote.author.finalCheckerLable;
                        
        //    DrawAuthor(authorInfo);

        //}
       
        public void DrawAuthor1(AuthorInfo authorInfo)
        {            
            Word.WdColor fontColor = Globals.headerFontColor;
            float fontSize = Globals.headerFontSize;
            Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            string writer = writerLable + authorInfo.Writer;
            if (writerLable == "»¼Õß£º")
            {
                writer = " ";
            }
            range.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;
            ccWriter = emrTaskPane.AddContentControlText(range, writer, null, true, fontSize, fontColor);
            ccWriter.Title = StringGeneral.Label; // as end of emrNote content
            ccWriter.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 

            if (writer.Length > 1)
            {
                range.Start = ccWriter.Range.End + 1;
                ccWrittenDate = emrTaskPane.AddContentControlText(range, " " + authorInfo.WrittenDate, null,
                    true, fontSize, fontColor);
                ccWrittenDate.Tag = StringGeneral.RemoveFlag;
                ccWrittenDate.Title = AttributeNames.WrittenDate;
            }

            string checker = checkerLable + authorInfo.Checker;
            if (checker.Length > 0)
            {
                range.Start = ccWrittenDate.Range.End + 1;
                ccChecker = emrTaskPane.AddContentControlText(range, Globals.space6 + checker, null,
                    true, fontSize, fontColor);
                ccChecker.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccChecker.Title = AttributeNames.Checker;
                range.Start = ccChecker.Range.End + 1;
                ccCheckedDate = emrTaskPane.AddContentControlText(range, " " + authorInfo.CheckedDate, null,
                    true, fontSize, fontColor);
                ccCheckedDate.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccCheckedDate.Title = AttributeNames.CheckedDate;
            }
            string finalChecker = finalCheckerLable + authorInfo.FinalChecker;
            if (finalChecker.Length > 0)
            {
                if (ccChecker == null)
                {
                    range.Start = ccWriter.Range.End + 1;                    
                }
                else
                {
                    range.Start = ccChecker.Range.End + 1;                    
                }

                ccFinalChecker = emrTaskPane.AddContentControlText(range, Globals.space6 +
                    finalChecker, null, true, fontSize, fontColor);
                ccFinalChecker.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccFinalChecker.Title = AttributeNames.FinalChecker;
                range.Start = ccFinalChecker.Range.End + 1;
                ccFinalCheckedDate = emrTaskPane.AddContentControlText(range, " " + authorInfo.FinalCheckedDate,
                    null, true, fontSize, fontColor);
                ccFinalCheckedDate.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccFinalCheckedDate.Title = AttributeNames.FinalCheckedDate;
            }
        }
        public void DrawAuthor2(AuthorInfo authorInfo)
        {
            Word.WdColor fontColor = Globals.headerFontColor;
            float fontSize = Globals.headerFontSize;
            int len = 0;

            #region Writter
            Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            string writer = writerLable + authorInfo.Writer;
            if (writerLable == string.Empty) writer = writerLable;

            range.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;
            Word.ContentControl ccNull = emrTaskPane.AddSubTitle(range, " ", Word.WdColor.wdColorWhite);
            ccNull.Title = StringGeneral.Label; // as end of emrNote content
            ccNull.Tag = StringGeneral.RemoveFlag;
            ccNull.LockContents = false;

            if (writer.Length > 1)
            {
                if (ThisAddIn.CanOption(ElementNames.ManualSign)) writer = writer + StringGeneral.SignSpace;

                if (!ThisAddIn.CanOption(ElementNames.ShowWriter)) writer = " ";

                if (!ThisAddIn.CanOption(ElementNames.AutoSign)) writer = writerLable + StringGeneral.SignSpace;

                range.Start = ccNull.Range.End + 1;
                ccWriter = emrTaskPane.AddContentControlTextRight(range, Globals.space6 + writer,
                    null, false, fontSize, fontColor);
                ccWriter.Tag = StringGeneral.RemoveFlag;  // as a note template, it will be removed 
                ccWriter.Title = AttributeNames.Writer;
                ccWriter.Range.Font = Globals.contentFont;


                range.Start = ccWriter.Range.End + 1;
                //if (Globals.EncryptSign == true)
                //{
                //    ccWriter.Range.Text = ccWriter.Range.Text.Trim();
                //    emrTaskPane.ThisAddIn.CanOption(ElementNames.ShowWriter)Sign(range, fontSize, fontColor, "WSign");
                //}
                ccWriter.LockContentControl = true;
                ccWriter.LockContents = true;

                string writtenDate = " ";
                if (ThisAddIn.CanOption(ElementNames.ShowWritenDate)) writtenDate += authorInfo.WrittenDate;
                ccWrittenDate = emrTaskPane.AddContentControlTextRight(range, writtenDate, null,
                    false, fontSize, fontColor);
                ccWrittenDate.Tag = StringGeneral.RemoveFlag;
                ccWrittenDate.Title = AttributeNames.WrittenDate;
                ccWrittenDate.Range.Font = Globals.contentFont;
                ccWrittenDate.LockContentControl = true;
                ccWrittenDate.LockContents = true;

                len += ccWriter.Range.Text.Length + ccWrittenDate.Range.Text.Length;
            }
            #endregion
            #region Checker
            string checker = checkerLable + authorInfo.Checker;
            if (checkerLable == string.Empty) checker = checkerLable;
            if (checker.Length > 0)
            {
                if (ThisAddIn.CanOption(ElementNames.ManualSign)) checker = checker + StringGeneral.SignSpace;
                if (!ThisAddIn.CanOption(ElementNames.ShowWriter)) checker = " ";
                if (!ThisAddIn.CanOption(ElementNames.AutoSign)) checker = checkerLable + StringGeneral.SignSpace;

                range.Start = ccWriter.Range.Start - 1;
                range.End = range.Start;
                ccChecker = emrTaskPane.AddContentControlTextRight(range, Globals.space6 + checker, null,
                    false, fontSize, fontColor);
                ccChecker.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccChecker.Title = AttributeNames.Checker;
                ccChecker.Range.Font = Globals.contentFont;
                ccChecker.LockContentControl = true;
                ccChecker.LockContents = true;

                range.Start = ccChecker.Range.End + 1;
                string checkedDate = " ";
                if (ThisAddIn.CanOption(ElementNames.ShowWritenDate)) checkedDate += authorInfo.CheckedDate;
                ccCheckedDate = emrTaskPane.AddContentControlTextRight(range, checkedDate, null,
                    false, fontSize, fontColor);
                ccCheckedDate.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccCheckedDate.Title = AttributeNames.CheckedDate;
                ccCheckedDate.Range.Font = Globals.contentFont;
                ccCheckedDate.LockContentControl = true;
                ccCheckedDate.LockContents = true;

                len += ccChecker.Range.Text.Length + ccCheckedDate.Range.Text.Length;
            }

            #endregion
            #region FinalChecker
            string finalChecker = finalCheckerLable + authorInfo.FinalChecker;
            if (finalCheckerLable == string.Empty) finalChecker = finalCheckerLable;
            if (finalChecker.Length > 0)
            {               
                if (ThisAddIn.CanOption(ElementNames.ManualSign)) finalChecker = finalChecker + StringGeneral.SignSpace;

                if (!ThisAddIn.CanOption(ElementNames.ShowWriter)) finalChecker = " ";
              
                if (!ThisAddIn.CanOption(ElementNames.AutoSign)) finalChecker = finalCheckerLable + StringGeneral.SignSpace;


                if (ccChecker == null) range.Start = ccWriter.Range.Start - 1;
                else range.Start = ccChecker.Range.Start - 1;
                range.End = range.Start;
                ccFinalChecker = emrTaskPane.AddContentControlTextRight(range, finalChecker,
                    null, false, fontSize, fontColor);
                ccFinalChecker.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccFinalChecker.Title = AttributeNames.FinalChecker;
                ccFinalChecker.Range.Font = Globals.contentFont;
                ccFinalChecker.LockContentControl = true;
                ccFinalChecker.LockContents = true;

                range.Start = ccFinalChecker.Range.End + 1;
                string finalCheckedDate = " ";
                if (ThisAddIn.CanOption(ElementNames.ShowWritenDate)) finalCheckedDate += authorInfo.FinalCheckedDate;
                ccFinalCheckedDate = emrTaskPane.AddContentControlTextRight(range, finalCheckedDate,
                    null, false, fontSize, fontColor);
                ccFinalCheckedDate.Tag = StringGeneral.RemoveFlag;      // as a note template, it will be removed 
                ccFinalCheckedDate.Title = AttributeNames.FinalCheckedDate;
                ccFinalCheckedDate.Range.Font = Globals.contentFont;
                ccFinalCheckedDate.LockContentControl = true;
                ccFinalCheckedDate.LockContents = true;

                len += ccFinalChecker.Range.Text.Length + ccFinalCheckedDate.Range.Text.Length;
            }

            #endregion
            /////guojt test sourcesafe
            //int padSpace = Globals.countPerLine - len;
            //if (Globals.auditSystem == AuditSystem.A1)
            //{

            //    for (int i = 0; i < padSpace; i++) ccNull.Range.Text += "  ";
            //}

        }
    }
}
