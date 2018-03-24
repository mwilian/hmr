using System;
using System.Collections.Generic;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using CommonLib;
namespace EMR
{
    public class DocProtectManager
    {
        public static bool unprotect(Word.Document myDoc)
        {
            object psd = (object)"jwsj";
            myDoc.Unprotect(ref psd);
            return true;
        }
        public static bool unprotectSave(Word.Document myDoc)
        {
            try
            {
                return unprotect(myDoc);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool protect(Word.Document myDoc)
        {
            object False = (object)false;
            object psd = (object)"jwsj";
            if (myDoc == null) return false;
            myDoc.Protect(Word.WdProtectionType.wdAllowOnlyReading, ref False, ref psd, ref False, ref False);
            return true;
        }
        public static bool protectSave(Word.Document myDoc)
        {
            try
            {
                return protect(myDoc);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9134474888", ex.Message + ">>" + ex.ToString(), true);
                return false;
            }
        }
    }
}
