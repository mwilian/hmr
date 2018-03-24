using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Management;

using Microsoft.Office.Interop.Word;
using CommonLib;
namespace UserInterface
{
    public partial class PrintSetup : Form
    {
        object PrintType = WdPrintOutPages.wdPrintAllPages;
        object PrintOutItem = WdPrintOutItem.wdPrintDocumentContent;
        object PrintOutRange = WdPrintOutRange.wdPrintAllDocument;
        object PrintToFile = false;
        object Copies = 1;
        object Collate = false;

        string printerName = "";
        private string strDefaultPrinterName = null;

        bool ok = false;
        bool no = false;
        bool twoPrint = false;
        bool printPageRange = false;
        int printPageStart = 0;
        int printPageEnd = 0;

        [DllImport("Winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string printerName);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int pcchBuffer);
        Microsoft.Office.Interop.Word.Application WordApp;
        static string GetDefaultPrinter()
        {
            const int ERROR_FILE_NOT_FOUND = 2;
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            int pcchBuffer = 0;
            if (GetDefaultPrinter(null, ref pcchBuffer))
            {
                return null;
            }
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error == ERROR_INSUFFICIENT_BUFFER)
            {
                StringBuilder pszBuffer = new StringBuilder(pcchBuffer);
                if (GetDefaultPrinter(pszBuffer, ref pcchBuffer))
                {
                    return pszBuffer.ToString();
                    
                }
                lastWin32Error = Marshal.GetLastWin32Error();
            }
            if (lastWin32Error == ERROR_FILE_NOT_FOUND)
            {
                return null;
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        public PrintSetup(Microsoft.Office.Interop.Word.Application WordApps)
        {
            InitializeComponent();
            WordApp = WordApps;
            GetPrintName();
            SetValue();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void SetValue()
        {
            printContent.SelectedIndex = 0;
            print.SelectedIndex = 0;
            cbxFormat.SelectedIndex = 0;
            cbxZoom.SelectedIndex = 0;
        }
        private void GetPrintName()
        {
            string[] printers = new string [PrinterSettings.InstalledPrinters.Count];
            string s = GetDefaultPrinter();
            PrinterSettings.InstalledPrinters.CopyTo(printers, 0);
            int n = 0;
            if (s != null && s.Length > 0)
            {
                LQPrinter.Items.Add(s);
            }
            foreach (string printer in printers)
            {
                if (printer == s)
                {
                    continue;
                }
                LQPrinter.Items.Add(printer);
                n = LQPrinter.Items.Count - 1;
               
            }
            if (n>=0) LQPrinter.SelectedItem = LQPrinter.Items[n];
            string status="";
            string DriverName = "", PortName = "";
            getPrinterInfo(LQPrinter.SelectedItem.ToString(),ref status, ref  DriverName, ref PortName);
            this.PortName.Text = PortName;
            this.DriverName.Text = DriverName;
            this.PrinterStatus.Text = status;
        }
        private void getPrinterInfo(string name, ref string Status, ref string DriverName, ref string PortName)
        {
            string searchQuery = "SELECT   *   FROM   Win32_Printer ";
            ManagementObjectSearcher searchPrinters =
            new ManagementObjectSearcher(searchQuery);
            ManagementObjectCollection printerCollection = searchPrinters.Get();
            //ManagementObject currentPrinter = null;
            foreach (ManagementObject printer in printerCollection)
            {
               
                if (string.Compare(printer["Name"].ToString(), name, true) == 0)
                {
                Status=printer.Properties["PrinterStatus"].Value.ToString();
                DriverName = printer.Properties["DriverName"].Value.ToString();
                PortName=printer.Properties["PortName"].Value.ToString();
                if ( printer.Properties["PrinterState"].Value.ToString()=="0") Status = "空闲";
                else Status = "打印 ：有" + Status + "篇文档正在等待打印";
                }
            } 

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxHand.Checked == true) groupBox3.Enabled = false;
            else groupBox3.Enabled = true;
        }

        private void printName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LQPrinter.Enabled == false)
            {
                return;
            }
            printerName = LQPrinter.SelectedItem.ToString();
            //string status = "";
            //string DriverName = "", PortName = "";
            //getPrinterInfo(LQPrinter.SelectedItem.ToString(), ref status, ref  DriverName, ref PortName);
            //this.PortName.Text = PortName;
            //this.DriverName.Text = DriverName;
            //this.PrinterStatus.Text = status;
            //WordApp.ActivePrinter = LQPrinter.SelectedItem.ToString();
        }

        private void rbNow_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNow.Checked == true)
            {
                PrintOutRange = WdPrintOutRange.wdPrintCurrentPage;
                cbxHand.Checked = false;
                cbxHand.Enabled = false;
            }
            else cbxHand.Enabled = true;
        }

        private void print_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (print.SelectedIndex)
            {
                case 0:
                    PrintType = WdPrintOutPages.wdPrintAllPages;
                    cbxHand.Enabled = true;
                    break;
                case 1:
                    cbxHand.Enabled = false;
                    PrintType = WdPrintOutPages.wdPrintOddPagesOnly;
                    break;
                case 2:
                    PrintType = WdPrintOutPages.wdPrintEvenPagesOnly;
                    cbxHand.Enabled = false;
                    break;
            }
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            PrintOutRange = WdPrintOutRange.wdPrintAllDocument;
        }

        private void rbArea_CheckedChanged(object sender, EventArgs e)
        {
            PrintOutRange = WdPrintOutRange.wdPrintRangeOfPages;
        }

        private void btnOn_Click(object sender, EventArgs e)
        {
            object Pages = "";
            if (rbArea.Checked == true)
            {
                if (tbPage.Text.Trim() == "")
                {
                    PrintError pr = new PrintError();
                    pr.ShowDialog();
                    return;
                }

                string[] Str = tbPage.Text.Trim().Split('-');
                if (Str.Length > 2)
                {
                    PrintError pr = new PrintError();
                    pr.ShowDialog();
                    return;
                }

                foreach (string s in Str)
                {
                    if (!udt.IsValidByte(s))
                    {
                        PrintError pr = new PrintError();
                        pr.ShowDialog();
                        return;
                    }
                }
                Pages = tbPage.Text.Trim();
            }
            object oMissing = System.Reflection.Missing.Value;
            object Copies = nudCopy.Value.ToString();
            object Background = true, PrintZoomColumn = 0, PrintZoomRow = 0, PrintZoomPaperWidth = 0,
       PrintZoomPaperHeight = 0, Append = oMissing, FileName = oMissing, From = oMissing, To = oMissing, ManualDuplexPrint = false;
            WordApp.PrintOut(ref Background, ref Append, ref PrintOutRange, ref FileName, ref oMissing, ref oMissing, ref PrintOutItem, ref Copies,
    ref Pages, ref PrintType, ref PrintToFile, ref Collate, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
    ref oMissing, ref oMissing, ref PrintZoomPaperHeight);
            //WordApp.PrintOut(ref Background, ref Append, ref PrintOutRange, ref FileName, ref From, ref To, ref PrintOutItem, ref Copies,
            //    ref Pages, ref PrintType, ref PrintToFile, ref Collate, ref oMissing, ref oMissing, ref ManualDuplexPrint, ref PrintZoomColumn,
            //    ref PrintZoomRow, ref PrintZoomPaperWidth, ref PrintZoomPaperHeight);
            this.Close();
        }
        private void tbPage_TextChanged(object sender, EventArgs e)
        {
            rbArea.Checked = true;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                Collate = true;
                //this.pictureBox1.Image = global::EMR.Prints.Resources._1;
            }
            else
            {
                Collate = false;
                //this.pictureBox1.Image = global::UserInterface.Properties.Resources._2;
            }
        }

        private void cbxPrintToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxPrintToFile.Checked == true) PrintToFile = true;
            else PrintToFile = false;
        }

        private void DefaultPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (DefaultPrint.Checked)
            {
                LQPrinter.Enabled = false;
                this.printerName = this.strDefaultPrinterName;
            }
            else
            {
                LQPrinter.Enabled = true;
                if (LQPrinter.SelectedItem != null)
                {
                    this.printerName = LQPrinter.SelectedItem.ToString();
                }
            }

        }

        private void LQInitPrint()
        {
            LQPrinter.DropDownStyle = ComboBoxStyle.DropDownList;
            List<string> printers = PrinterManager.GetLocalPrinters();
            PrintDocument pc = new PrintDocument();


            this.printerName = strDefaultPrinterName;
            string pn = pc.PrinterSettings.PrinterName;
            foreach (string p in printers)
            {
                if (p.Trim() == pn)
                    LQPrinter.SelectedItem = p;
                //LQPrinter.Items.Add(p);
            }
            if (LQPrinter.Items.Count > 0)
            {
                LQPrinter.SelectedIndex = 0;
            }
        }
        private void LQContiPrint_Load(object sender, EventArgs e)
        {
            LQInitPrint();
        }

     
    }
}