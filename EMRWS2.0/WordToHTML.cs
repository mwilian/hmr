using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
namespace EMR
{
    class WordToHTML
    {
        /*
        private void removeHeaderFooter(HeaderFooter hf)
        {
            if (hf != null)
            {
                hf.Remove();
            }
        }
        private void removeHeaderAndFooter(Document doc)
        {
            HeaderFooter hf = null;
            try
            {
                foreach (Section section in doc)
                {
                    hf = section.HeadersFooters[HeaderFooterType.HeaderPrimary];
                    removeHeaderFooter(hf);
                    hf = section.HeadersFooters[HeaderFooterType.HeaderFirst];
                    removeHeaderFooter(hf);
                    hf = section.HeadersFooters[HeaderFooterType.HeaderEven];
                    removeHeaderFooter(hf);
                    hf = section.HeadersFooters[HeaderFooterType.FooterPrimary];
                    removeHeaderFooter(hf);
                    hf = section.HeadersFooters[HeaderFooterType.FooterFirst];
                    removeHeaderFooter(hf);
                    hf = section.HeadersFooters[HeaderFooterType.FooterEven];
                    removeHeaderFooter(hf);
                }
            }
            catch (Exception ex) { }
        }
        private void deleteComments(Document doc)
        {
            try
            {
                NodeCollection nc = doc.GetChildNodes(NodeType.Comment, true);
                Node[] ns = nc.ToArray();
                foreach (Node n in ns)
                {
                    n.Remove();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public string convert(string docName)
        {
            string tmppath = docName;
            Document doc = new Document(tmppath); //载入模板
            doc.TrackRevisions = true;
            //doc.Revisions.RejectAll();    //拒绝所有修订。相当于不显示修订。

            doc.Revisions.AcceptAll();      //同意所有修订。相当于不显示修订。
            deleteComments(doc);
            removeHeaderAndFooter(doc);
            string savefilename = Application.StartupPath + @"\NoteRef.html";//生成HTML的路径和名子 
            doc.Save(savefilename, SaveFormat.Html);
            return savefilename;
        }
        */

        public string convert(string docName)
        {
            object objFileName = docName;
            object newTemplate = false;
            object docType = 0;
            object readOnly = true;
            object isVisible = true;
            object missing = System.Reflection.Missing.Value;
            Word.ApplicationClass wd = new Word.ApplicationClass();
            wd.Visible = false;
            //wd.DocumentOpen +=new Word.ApplicationEvents4_DocumentOpenEventHandler(wd_DocumentOpen);
            Word.Document doc = wd.Documents.Add(ref objFileName, ref newTemplate, ref docType, ref isVisible);
            object oMissing = System.Reflection.Missing.Value;
            //Word.Application wordApp2 = wd.Application;
            doc.Background.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            wd.ActiveWindow.DisplayRulers = false;
            wd.ActiveWindow.View.ShowRevisionsAndComments = false;   //参照时不显示痕迹 LiuQi 2012-07-03 
            wd.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;  //参照时不显示痕迹 LiuQi 2012-07-03 
            object wdDocName = Application.StartupPath + @"\NoteRef.html";//生成HTML的路径和名子 
            object FileFormat = (object)Word.WdSaveFormat.wdFormatHTML;
            object False = (object)false;
            doc.SaveAs(ref wdDocName, ref FileFormat, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            object objSaveOption = Word.WdSaveOptions.wdDoNotSaveChanges;
            doc.Close(objSaveOption, oMissing, oMissing);
            wd.Quit(objSaveOption, oMissing, oMissing);

            removeMSTarget((string)wdDocName);
            return (string)wdDocName;
        }
        private void removeMSTarget(string strDocName)
        {
            try
            {
                FileStream fs = new FileStream(strDocName, FileMode.Open,FileAccess.ReadWrite,FileShare.None);
                StreamReader sr = new StreamReader(fs,Encoding.GetEncoding("gb2312"));
                string strHTML = sr.ReadToEnd();
                sr.Close();
                fs.Close();

                //sr.Close();
                int iStart = strHTML.IndexOf("<html");
                int iEnd = 0;
                if (iStart >= 0)
                {
                    iEnd = strHTML.IndexOf(">", iStart);
                }
                if (iStart < iEnd && iEnd > 0)
                {
                    strHTML = strHTML.Remove(iStart, iEnd - iStart + 1);
                    strHTML = strHTML.Insert(iStart, "<html>");
                }
                fs = new FileStream(strDocName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                StreamWriter sw = new StreamWriter(fs,Encoding.GetEncoding("gb2312"));
                sw.Write(strHTML);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception) { }
        }
        void wd_DocumentOpen(Word.Document Doc)
        {
            
        }


    }
}
