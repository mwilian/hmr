using System;
using System.Collections.Generic;
using System.Text;
using CommonLib;
namespace EMR
{
    public static class EmrDocInfo
    {
        private static int iSeries = -1;
        private static bool bIsNew = false;
        private static string strNoteID = "";

        private static string strUID = "";

        private static bool bIsNewForCheck = false;
        private static string strNoteIDForCheck = "";
        public static void newOpen(string strNoteID)
        {
            EmrDocInfo.strUID = Guid.NewGuid().ToString("D");
            EmrDocInfo.Series = -1;
            EmrDocInfo.IsNew = true;
            EmrDocInfo.NoteID = strNoteID;
        }
        public static void existOpen(string strNoteID, int iSeries)
        {
            EmrDocInfo.strUID = Guid.NewGuid().ToString("D");
            EmrDocInfo.Series = iSeries;
            EmrDocInfo.IsNew = false;
            EmrDocInfo.NoteID = strNoteID;

        }
        public static void setIsNewAndNoteIDForCheck(bool bIsNewForCheck,string strNoteIDForCheck) {
            EmrDocInfo.bIsNewForCheck = bIsNewForCheck;
            EmrDocInfo.strNoteIDForCheck = strNoteIDForCheck;
        }
        public static void setIsNewForCheck(bool bIsNewForCheck)
        {
            EmrDocInfo.bIsNewForCheck = bIsNewForCheck;
            
        }
        public static void setNoteIDForCheck( string strNoteIDForCheck)
        {
            
            EmrDocInfo.strNoteIDForCheck = strNoteIDForCheck;
        }
        public static void setSeriesIDForCheck(int iSID) {
            if (iSID == 0) {
                Globals.logAdapter.Record("EmrDocInfo setSeriesIDForCheck(int iSID)", "EmrDocInfo setSeriesIDForCheck(int iSID)iSeries 检测到错误 iSeries==0#" + EmrDocInfo.strUID, true);
                SendMessageToMonitor.sendMessage("EmrDocInfo setSeriesIDForCheck(int iSID)iSeries 检测到错误 iSeries==0");
            }
        }
        public static void checkSeries(int series) {
            try
            {
                if (EmrDocInfo.iSeries != series || EmrDocInfo.IsNew != EmrDocInfo.bIsNewForCheck || EmrDocInfo.NoteID != EmrDocInfo.strNoteIDForCheck)
                {
                    string strErrorData = "iSeries检测错误EmrDocInfo.iSeries:" + EmrDocInfo.iSeries.ToString() + ",series:" + series.ToString() + ";" +
                        "EmrDocInfo.NoteID:" + EmrDocInfo.NoteID + ",EmrDocInfo.bIsNewForCheck:" + EmrDocInfo.bIsNewForCheck.ToString() + ";" +
                        "EmrDocInfo.bIsNew:" + EmrDocInfo.IsNew.ToString() + ",EmrDocInfo.strNoteIDForCheck:" + EmrDocInfo.strNoteIDForCheck.ToString() + "#" + EmrDocInfo.strUID;
                    Globals.logAdapter.Record("EmrDocInfo checkSeries(int series)", strErrorData, true);
                    SendMessageToMonitor.sendMessage(strErrorData);
                }
            }
            catch (Exception ex) { }
        }
        public static void setDBError(string strErrorText) {
            try
            {
                if (strErrorText == null) {
                    return;
                }
                if (strErrorText.Length > 0)
                {
                    strErrorText = "setDBError:" + strErrorText + "#" + EmrDocInfo.strUID;
                    Globals.logAdapter.Record("setDBError checkSeries(int series)", strErrorText, true);
                    SendMessageToMonitor.sendMessage(strErrorText);
                }
            }
            catch (Exception ex) { }
        }
        public static void insertOK(int iSeries)
        {
            EmrDocInfo.IsNew = false;
            EmrDocInfo.Series = iSeries;
        }
        public static int Series {
            get {
                return iSeries;
            }
            set {
                iSeries = value;
            }
        }
        public static bool IsNew
        {
            get
            {
                return bIsNew;
            }
            set
            {
                bIsNew = value;
            }
        }
        public static string NoteID
        {
            get {
                return strNoteID;
            }
            set {
                strNoteID = value;
            }
        }

    }
}
