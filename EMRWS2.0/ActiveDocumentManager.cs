using System;
using System.Collections.Generic;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
namespace EMR
{
    /*
     * 设置和获取当前程序的wordDocument，使用时获取本Document
     * 后激活该Document。
     */
    public class ActiveDocumentManager
    {
        private static Word.Document _wordActiveDocument = null;
        private static Word.Window _wordWindow = null;
        public static void setDefaultAD(Word.Document wordActiveDocument)
        {
            _wordActiveDocument = wordActiveDocument;
            _wordWindow = wordActiveDocument.ActiveWindow;
        }
        public static Word.Document getDefaultAD()
        {
            return _wordActiveDocument;
        }
        public static Word.Window getWindow()
        {
            return _wordWindow;
        }
    }
}
