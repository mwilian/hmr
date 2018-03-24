using System;
using System.Collections.Generic;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using CommonLib;
using EmrConstant;
namespace EMR.Prints
{
    public class DocPrintHelper
    {
        WdColor wdTextColor = WdColor.wdColorBlack;
        Range rangeText = null;
        WdColor wdHeadColor = WdColor.wdColorBlack;
        WdColor wdFooterColor = WdColor.wdColorBlack;
        Range rangeHeader = null;
        Range rangeFooter = null;
        private Word.Document myDoc = null;
        private object objStart = null;
        private object objEnd = null;
        private object objPageNum = null;
        private Pane paneTemp = null;
        private int iPageCount = 0;
        private object objPageStart = null;
        private string strViewPrinterName = "Microsoft Office Document Image Writer";
        public DocPrintHelper(Word.Document myDoc)
        {
            this.myDoc = myDoc;
            Selection selection = myDoc.Application.Selection;
            objStart = selection.Start;
            objEnd = selection.End;
            objPageNum = selection.get_Information(WdInformation.wdActiveEndPageNumber);
            paneTemp = myDoc.ActiveWindow.Panes[1];
            iPageCount = paneTemp.Pages.Count;
            objPageStart = selection.get_Information(WdInformation.wdActiveEndPageNumber);

        }

        private string strBufferDir = @"C:\Users\Administrator\Documents\";
        private string strBufferDir2 = @"C:\Users\Administrator\Documents\buf\";

        private void clearImage(string strBufferDir)
        {
            string[] strFiles = Directory.GetFiles(strBufferDir, "*.tif");
            foreach (string str in strFiles)
            {
                File.Delete(str);
            }
        }

        private bool print(Word.Document myDoc, string strPrinterName, int iPageStart, int iPageEnd)
        {
            DocProtectManager.protectSave(myDoc);
            string strOldPrintName = null;
            //当strPrinterName 为空时使用默认打印机
            if (strPrinterName != null)
            {
                strOldPrintName = myDoc.Application.ActivePrinter;
                myDoc.Application.ActivePrinter = strPrinterName;
            }
            /*
                PrintOut (
	            [OptionalAttribute] ref Object Background,
	            [OptionalAttribute] ref Object Append,
	            [OptionalAttribute] ref Object Range,
	            [OptionalAttribute] ref Object OutputFileName,
	            [OptionalAttribute] ref Object From,
	            [OptionalAttribute] ref Object To,
	            [OptionalAttribute] ref Object Item,
	            [OptionalAttribute] ref Object Copies,
	            [OptionalAttribute] ref Object Pages,
	            [OptionalAttribute] ref Object PageType,
	            [OptionalAttribute] ref Object PrintToFile,
	            [OptionalAttribute] ref Object Collate,
	            [OptionalAttribute] ref Object ActivePrinterMacGX,
	            [OptionalAttribute] ref Object ManualDuplexPrint,
	            [OptionalAttribute] ref Object PrintZoomColumn,
	            [OptionalAttribute] ref Object PrintZoomRow,
	            [OptionalAttribute] ref Object PrintZoomPaperWidth,
	            [OptionalAttribute] ref Object PrintZoomPaperHeight
             * )
             */
            try
            {

                object missing = System.Reflection.Missing.Value;
                object Background = true;
                object Range = Word.WdPrintOutRange.wdPrintRangeOfPages;
                object Copies = 1;
                object PageType = Word.WdPrintOutPages.wdPrintAllPages;
                object PrintToFile = false;
                object Collate = false;
                object ActivePrinterMacGX = missing;
                object ManualDuplexPrint = false;
                object PrintZoomColumn = 1;
                object PrintZoomRow = 1;
                object objPage = iPageStart.ToString() + "-" + iPageEnd.ToString();
                myDoc.PrintOut(ref Background, ref missing, ref Range, ref missing,
                        ref missing, ref missing, ref missing, ref Copies,
                        ref objPage, ref PageType, ref PrintToFile, ref Collate,
                        ref missing, ref ManualDuplexPrint, ref PrintZoomColumn,
                        ref PrintZoomRow, ref missing, ref missing);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                DocProtectManager.unprotectSave(myDoc);
                if (strOldPrintName != null)
                {
                    myDoc.Application.ActivePrinter = strOldPrintName;
                }
            }
        }
        
        private void unprotectControls(Word.Document myDoc)
        {
            foreach (Word.ContentControl cc in myDoc.ContentControls)
            {
                cc.LockContents = false;
                cc.LockContentControl = false;
                //ChangeRange(cc, dst, registryID);
            }
            //cc.LockContents = false;
        }
        private void showText()
        {
            rangeText.Font.Color = wdTextColor;
        }

        private void hideText(Word.Document myDoc, object iPage, object iEnd)
        {
            object missing = System.Reflection.Missing.Value;
            object objWhat = Word.WdGoToItem.wdGoToPage;
            object objWhich = Word.WdGoToDirection.wdGoToAbsolute;
            object objPage = iPage;
            Word.Range rangePage = myDoc.GoTo(ref objWhat, ref objWhich, ref objPage, ref missing);
            rangeText = rangePage;
            wdTextColor = rangePage.Font.Color;
            rangePage.End = (int)iEnd;
            rangePage.Font.Color = WdColor.wdColorWhite;
        }
        private void hideHeader(Word.Document myDoc, object iPage)
        {
            object missing = System.Reflection.Missing.Value;
            object objWhat = Word.WdGoToItem.wdGoToPage;
            object objWhich = Word.WdGoToDirection.wdGoToAbsolute;
            object objPage = iPage;
            int iSelections = myDoc.Sections.Count;
            for (int i = 1; i <= iSelections; i++)
            {
                Word.Range rangePage = myDoc.Sections[i].Range;
                Section sectionInRange = myDoc.Sections[i];
                rangePage = sectionInRange.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                rangeHeader = rangePage;
                hideLines(rangePage);
                wdHeadColor = rangePage.Font.Color;
                setRangeFontColor(rangePage, WdColor.wdColorWhite);
                hideFooter(sectionInRange);
            }
            //return sectionInRange;
        }
        private void hideFooter(Section sectionInRange)
        {
            try
            {
                rangeFooter = sectionInRange.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                wdFooterColor = rangeFooter.Font.Color;
                setRangeFontColor(rangeFooter, WdColor.wdColorWhite);
            }
            catch (Exception)
            {
            }
        }
        private void showHeader()
        {
            int iSelections = myDoc.Sections.Count;
            for (int i = 1; i <= iSelections; i++)
            {
                Word.Range rangePage = myDoc.Sections[i].Range;
                Section sectionInRange = myDoc.Sections[i];
                rangePage = sectionInRange.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                rangeHeader = rangePage;
                showLines(rangeHeader);
                setRangeFontColor(rangeHeader, wdHeadColor);
                showFooter();
            }


            
        }
        private void showFooter()
        {
            setRangeFontColor(rangeFooter, wdFooterColor);
        }
        private string getRangeXML(Range range)
        {
            string strHeaderXML = range.get_XML();
            return strHeaderXML;
        }
        private void setXMLIntoRange(Range range, string strHeaderXML)
        {
            range.InsertXML(strHeaderXML);
        }
        private void setRangeFontColor(Range range, WdColor color)
        {
            range.Font.Color = color;
        }
        private void showLines(Range range)
        {
            try
            {
                foreach (Shape shape in range.ShapeRange)
                {
                    shape.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void hideLines(Range range) //删除底纹
        {
            try
            {
                foreach (Shape shape in range.ShapeRange)
                {
                    shape.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
                }
            }
            catch (Exception ex)
            {
            }
        }
        public bool viewCurrPage(string strPrintName)
        {
            try
            {
                return print(myDoc, strViewPrinterName, (int)objPageNum, (int)objPageNum);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool printCurrPage(string strPrintName)
        {
            try
            {
                return print(myDoc, strPrintName, (int)objPageNum, (int)objPageNum);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                DocProtectManager.protectSave(myDoc);
            }
        }
        private bool viewAfterCursorDoNotRemoveHeader()
        {
            try
            {
                DocProtectManager.unprotectSave(myDoc);
                unprotectControls(myDoc);
                try
                {
                    hideText(myDoc, objPageStart, objEnd);
                }
                catch (Exception) { }
                bool bResult = print(myDoc, strViewPrinterName, (int)objPageStart, (int)objPageStart);
                try
                {
                    showText();
                }
                catch (Exception) { }
                if (moveTIF(strBufferDir, strBufferDir2, 1) == false)
                {
                    return false;
                }
                if ((int)objPageStart < iPageCount)
                {
                    print(myDoc, strViewPrinterName, (int)objPageStart + 1, iPageCount);
                    if (moveTIF(strBufferDir, strBufferDir2, 2) == false)
                    {
                        return false;
                    }
                }
                DocProtectManager.protectSave(myDoc);
                return bResult;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool viewAfterCursorRemoveHeader()
        {
            try
            {
                DocProtectManager.unprotectSave(myDoc);
                unprotectControls(myDoc);
                try
                {
                    hideText(myDoc, objPageStart, objEnd);
                }
                catch (Exception) { }
                try
                {
                    hideHeader(myDoc, objPageStart);
                }
                catch (Exception) { }
                bool bResult = print(myDoc, strViewPrinterName, (int)objPageStart, (int)objPageStart);
                try
                {
                    showHeader();
                }
                catch (Exception) { }
                try
                {
                    showText();
                }
                catch (Exception) { }
                if (moveTIF(strBufferDir, strBufferDir2, 1) == false)
                {
                    return false;
                }
                if ((int)objPageStart < iPageCount)
                {
                    print(myDoc, strViewPrinterName, (int)objPageStart + 1, iPageCount);
                    if (moveTIF(strBufferDir, strBufferDir2, 2) == false)
                    {
                        return false;
                    }
                }
                DocProtectManager.protectSave(myDoc);
                return bResult;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public int pageCount()
        {
            return this.iPageCount;
        }
        public bool printByPageRange(string strPrinterName, int iPageStart, int iPageEnd)
        {
            if (iPageStart <= 0 || iPageEnd > iPageCount)
            {
                return false;
            }
            else
            {
                print(myDoc, strPrinterName, iPageStart, iPageEnd);
                return true;
            }
        }
        public bool viewAfterCursor(bool bRemoveHeader)
        {
            if (bRemoveHeader)
            {
                return viewAfterCursorRemoveHeader();
            }
            else
            {
                return viewAfterCursorDoNotRemoveHeader();
            }
        }
        private bool printAfterCursorDonotRemoveHeader(string strPrintName)
        {
            try
            {
                DocProtectManager.unprotectSave(myDoc);
                unprotectControls(myDoc);
                try
                {
                    hideText(myDoc, objPageStart, objEnd);
                }
                catch (Exception) { }
                bool bResult = print(myDoc, strPrintName, (int)objPageStart, (int)objPageStart);
                try
                {
                    showText();
                }
                catch (Exception) { }

                if ((int)objPageStart < iPageCount)
                {
                    print(myDoc, strPrintName, (int)objPageStart + 1, iPageCount);
                }
                DocProtectManager.protectSave(myDoc);
                return bResult;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                DocProtectManager.protectSave(myDoc);
            }
        }

        private bool printAfterCursorRemoveHeader(string strPrintName)
        {
            try
            {
                DocProtectManager.unprotectSave(myDoc);
                unprotectControls(myDoc);
                try
                {
                    hideText(myDoc, objPageStart, objEnd);
                }
                catch (Exception) { }
                try
                {
                    hideHeader(myDoc, objPageStart);
                }
                catch (Exception) { }
                bool bResult = print(myDoc, strPrintName, (int)objPageStart, (int)objPageStart);
                try
                {
                    showHeader();
                }
                catch (Exception) { }
                try
                {
                    showText();
                }
                catch (Exception) { }
                if ((int)objPageStart < iPageCount)
                {
                    print(myDoc, strPrintName, (int)objPageStart + 1, iPageCount);
                }
                DocProtectManager.protectSave(myDoc);
                return bResult;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                DocProtectManager.protectSave(myDoc);
            }
        }

        public bool printAfterCursor(string strPrintName, bool bRemoveHeader)
        {
            if (bRemoveHeader)
            {
                return printAfterCursorRemoveHeader(strPrintName);
            }
            else
            {
                return printAfterCursorDonotRemoveHeader(strPrintName);
            }
        }
        private bool moveTIF(string strTIFDir, string strBufDir, int iID)
        {
            byte[] bs = null;
            string[] strFiles = Directory.GetFiles(strTIFDir, "*.tif");
            if (strFiles.Length != 1)
            {
                MessageBox.Show("打印与预览发生文件生成错误");
                return false;
            }
            FileStream fs = null;
            while (true)
            {
                try
                {
                    fs = new FileStream(strFiles[0], FileMode.Open);
                }
                catch (IOException)
                {
                    Thread.Sleep(20);
                    continue;
                }
                catch (Exception e)
                {
                    MessageBox.Show("打印与预览发生错误：" + e.Message);
                    return false;
                }
                break;
            }
            bs = new byte[fs.Length];
            fs.Read(bs, 0, bs.Length);
            fs.Close();
            fs = new FileStream(strBufferDir2 + iID.ToString() + ".tif", FileMode.Create);
            fs.Write(bs, 0, bs.Length);
            fs.Close();
            clearImage(strBufferDir);
            return true;
        }
        public bool printAll(string strPrintName)
        {
            try
            {
                return print(myDoc, strPrintName, 1, iPageCount);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool viewAll()
        {
            try
            {
                return print(myDoc, strViewPrinterName, 1, iPageCount);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
