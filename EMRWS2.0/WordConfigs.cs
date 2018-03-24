using System;
using System.Collections.Generic;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
namespace EMR
{
    class WordConfigs
    {
        public static void setCopyConfig(Word.Application wordApp)
        {
            try
            {   //设置格式“匹配目标格式”
                wordApp.Options.PasteFormatBetweenDocuments = Word.WdPasteOptions.wdMatchDestinationFormatting;
                wordApp.Options.PasteFormatBetweenStyledDocuments = Word.WdPasteOptions.wdMatchDestinationFormatting;
                wordApp.Options.PasteFormatFromExternalSource = Word.WdPasteOptions.wdMatchDestinationFormatting;
                wordApp.Options.PasteFormatWithinDocument = Word.WdPasteOptions.wdMatchDestinationFormatting;
                wordApp.Options.PasteAdjustParagraphSpacing = true;
                ActiveDocumentManager.getDefaultAD().ActiveWindow.View.ShowFieldCodes = false;//显示域代码而非域值
                ActiveDocumentManager.getDefaultAD().ActiveWindow.DisplayRulers = true;//标尺显示 
                ActiveDocumentManager.getDefaultAD().ActiveWindow.DisplayVerticalRuler = true;//标尺显示 
                setAutoSave(wordApp);
                setGrammarCheck(wordApp);
            }
            catch (Exception ex)
            {

            }
        }

        private static void setAutoSave(Word.Application wordApp)
        {
            try
            {
                wordApp.Options.SaveInterval = 0;

            }
            catch (Exception)
            {
            }
        }
        private static void setGrammarCheck(Word.Application wordApp)
        {
            try
            {
                wordApp.ShowWindowsInTaskbar = false;
                ActiveDocumentManager.getDefaultAD().SpellingChecked = false;
                wordApp.Options.CheckGrammarWithSpelling = false;
                wordApp.CheckLanguage = false;
            }
            catch (Exception)
            {

            }
        }
    }
}
