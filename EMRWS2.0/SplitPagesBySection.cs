using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using System.Windows.Forms;
namespace EMR
{
    public class SplitPagesBySection
    {
        Microsoft.Office.Interop.Word.Application wordApp;
        public static void setPageNum(Word.Application wordApp)
        {
            Word.Document myDoc = ActiveDocumentManager.getDefaultAD();
            int iPageCount = getTotalPage(myDoc);
            int iSections = ActiveDocumentManager.getDefaultAD().Range().Sections.Count;
            for (int i = 1; i <= iSections; i++)
            {

                Range rg = ActiveDocumentManager.getDefaultAD().Range().Sections[i].Range;
                Word.HeaderFooter hf = rg.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                hf.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                hf.Range.Text = "第   页";

                hf.PageNumbers.NumberStyle = Word.WdPageNumberStyle.wdPageNumberStyleArabic;
                hf.PageNumbers.RestartNumberingAtSection = false;
                //hf.PageNumbers.StartingNumber = 1;
                //hf.LinkToPrevious = true;
                //hf.PageNumbers.
                object alignment = Word.WdPageNumberAlignment.wdAlignPageNumberCenter;
                object oFirstPage = true;
                rg.Sections[1].Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(ref alignment, ref oFirstPage);
            }

            //throw new Exception();

        }
        public static void split3(Word.Application wordApp)
        {
            bool bInsertSplit = false;
            int iPageCount = getTotalPage(ActiveDocumentManager.getDefaultAD());
            if (iPageCount > 1)
            {
                bInsertSplit = true;
            }
            else if (ActiveDocumentManager.getDefaultAD().Characters.Count > 1)
            {
                bInsertSplit = true;
            }
            if (bInsertSplit)
            {
                Range rg = ActiveDocumentManager.getDefaultAD().Range();
                rg.SetRange(rg.End, rg.End);
                rg.InsertBreak(WdSectionStart.wdSectionNewPage);
                int iSectionCount = ActiveDocumentManager.getDefaultAD().Sections.Count;
                ActiveDocumentManager.getDefaultAD().Sections[iSectionCount].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;
            }

        }
        public static void split2(Word.Application wordApp)
        {
            //return;
            Word.Document myDoc = ActiveDocumentManager.getDefaultAD();
            int iPageCount = getTotalPage(myDoc);

            //bool bInsertSplit = false;

            Range rg = myDoc.Range();
            rg.SetRange(rg.End, rg.End);

            int iSectionCount = myDoc.Sections.Count;
           
            //ActiveDocumentManager.getDefaultAD().Sections[iSectionCount].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;
            //ActiveDocumentManager.getDefaultAD().Sections[iSectionCount].Headers[WdHeaderFooterIndex.wdHeaderFooterFirstPage].LinkToPrevious = false;
            rg.InsertBreak(WdSectionStart.wdSectionNewPage);
           
            iSectionCount = myDoc.Sections.Count;
            //ActiveDocumentManager.getDefaultAD().Sections[iSectionCount].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;
            //ActiveDocumentManager.getDefaultAD().Sections[iSectionCount].Headers[WdHeaderFooterIndex.wdHeaderFooterFirstPage].LinkToPrevious = false;
           
            //setPageNum(wordApp);
        }
        private static Range getRangeByPageID(Word.Document myDoc, int iPage)
        {
            object missing = System.Reflection.Missing.Value;
            object objWhat = Word.WdGoToItem.wdGoToPage;
            object objWhich = Word.WdGoToDirection.wdGoToAbsolute;
            object objPage = iPage;
            Word.Range rangePage = myDoc.GoTo(ref objWhat, ref objWhich, ref objPage, ref missing);
            return rangePage;
        }
        private static int getTotalPage(Word.Document myDoc)
        {
            Pane paneTemp = myDoc.ActiveWindow.Panes[1];
            int iPageCount = paneTemp.Pages.Count;
            return iPageCount;
        }
    }
}
