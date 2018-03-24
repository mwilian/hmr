using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;

namespace EMR.Prints
{
    public class PrinterManager
    {
        private static PrintDocument fPrintDocument = new PrintDocument();
        public static string DefaultPrinter = "";
        public static List<String> GetLocalPrinters()
        {
            List<String> fPrinters = new List<string>();
            //fPrinters.Add(DefaultPrinter); // 默认打印机始终出现在列表的第一项   
            foreach (String fPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                    fPrinters.Add(fPrinterName);
            }
            
            return fPrinters;
        }

    }
}
