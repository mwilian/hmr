using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using CommonLib;
using System.Drawing.Printing;


namespace EMR.Prints
{
    public partial class ContiPrint : Form
    {
        private EmrPattern emrPattern=null;
        private string strDefaultPrinterName = null;
        private DocPrintHelper _dch = null;
        //private 
        public ContiPrint(DocPrintHelper dch)
        {
            InitializeComponent();
            _dch = dch;
            iTotalPageCount = _dch.pageCount();
        }

        bool ok = false;
        bool no = false;
        bool cancel = false;
        bool printEnd = false;
        bool rdefault = false;
        bool self = false;
        string page = "";
        string space = "";

        bool twoPrint = false;
        bool printAll = true;
        bool printThis = false;
        bool thiscursor = false;
        bool thiscursorDel = false;
        bool printPageRange = false;
        int printPageStart = 0;
        int printPageEnd = 0;
        string printerName = "";

        int iTotalPageCount = 0;
        StringBuilder bjp = new StringBuilder();

        private void btnOK_Click(object sender, EventArgs e)
        {
            ok = true;
            if (cbkPrintEnd1.Enabled == false)
            {
                printEnd = false;
            }
            else
                if (cbkPrintEnd1.Checked)
                {
                    printEnd = true;
                }


            if (rbSelf.Checked)
            {
                if (txtPage.Text.Trim() == "" || txtSpace.Text.Trim() == "")
                {
                    MessageBox.Show("数据不能为空！");
                    return;
                }
                self = true;

                page = txtPage.Text;

                space = txtSpace.Text;
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
     
        public bool ShowMsg(out bool printEnd1, out bool rdefault1, 
            out bool self1, ref string page1, out string space1,
            out bool no, out bool cancel, out bool printAll, out bool printThis, out bool thiscursor, out bool twoPrint, out string printerName, out bool thiscursorDel,out bool printPageRange, out int printPageStart, out int printPageEnd)
        {
            this.ShowDialog();
            printEnd1 = this.printEnd;
            self1 = this.self;
            rdefault1 = this.rdefault;
            page1 = this.page;
            space1 = this.space;
            no = this.no;
            cancel = this.cancel;
            twoPrint = this.twoPrint;
            printAll = this.printAll;
            printThis = this.printThis;
            thiscursor = this.thiscursor;
            thiscursorDel = this.thiscursorDel;
            printerName = this.printerName;

            printPageRange = this.printPageRange;
            printPageStart = this.printPageStart;
            printPageEnd = this.printPageEnd;

            return ok; 
        }

        private void btnCancelPrint_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
        }

        //判断是不是全角函数
        public bool IsQuanJiao(char checkString)
        {
            if (2 * checkString.ToString().Length == Encoding.Default.GetByteCount(checkString.ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //如果是全角转换为半角
        public string ToBj(string s)
        {
            if (s == null || s.Trim() == string.Empty)
            {
                return s;
            }
            else
            {
                StringBuilder sb = new StringBuilder(s.Length);
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '\u3000')
                    {
                        sb.Append('\u0020');
                    }
                    else if (IsQuanJiao(s[i]))
                    {
                        sb.Append((char)((int)s[i] - 65248));
                    }
                    else
                    {
                        sb.Append(s[i]);
                    }
                }

                return sb.ToString();
            }
        }

        private void txtPage_Leave(object sender, EventArgs e)
        {
            try
            {
                txtPage.Text = ToBj(txtPage.Text.Trim());
                int p = Convert.ToInt32(txtPage.Text);

                if (p == 0) MessageBox.Show("填入数据不能为0！"); return;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX7569874577496", ex.Message + ">>" + ex.ToString(), true);            
             
                MessageBox.Show("填入数据必须为整数数字！");
            }
        }

        private void txtSpace_Leave(object sender, EventArgs e)
        {
            try
            {
                txtSpace.Text = ToBj(txtSpace.Text);
                int p = Convert.ToInt32(txtSpace.Text);
                if (p == 0) MessageBox.Show("填入数据不能为0！"); return;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX7569874339", ex.Message + ">>" + ex.ToString(), true);            
             
                MessageBox.Show("填入数据必须为整数数字！");
            }
        }

        private void rbSelf_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSelf.Checked)
            {
                txtPage.Enabled = true;
                txtSpace.Enabled = true;
                cbkPrintEnd1.Enabled = false;
            }
        }

        private void rbDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDefault.Checked)
            {
                txtPage.Enabled = false;
                txtSpace.Enabled = false;
                cbkPrintEnd1.Enabled = true;
            }
        }

        private void ContiPrint_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancel = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (printerName == "")
            {
                MessageBox.Show("请选择打印机！"); return;
            }

            if (checkPrintRange() == false)
            {
                return;
            }

            ok = true;
            twoPrint = true;
            this.Close();
        }

        private bool checkPrintRange()
        {
            if (printPageRange == false)
            {
                return true;
            }
            if (!checkPageRangeStart())
            {
                return false;
            }
            if (!checkPageRangeEnd())
            {
                return false;
            }
            return true;
        }
        private bool checkPageRangeEnd()
        {
            int p = 0;
            try
            {
                tbPageEnd.Text = ToBj(tbPageEnd.Text);
                p = Convert.ToInt32(tbPageEnd.Text);
                if (p <= 0)
                {
                    MessageBox.Show("填入数据必须大于1！");
                    return false;
                }
                if (p < printPageStart)
                {
                    MessageBox.Show("填入数据必须大于页码起始值！");
                    return false;
                }
                if (p > iTotalPageCount)
                {
                    MessageBox.Show("填入数据必须是有效的页码！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Globals.logAdapter.Record("EX7569874339", ex.Message + ">>" + ex.ToString(), true);
                MessageBox.Show("填入数据必须为整数数字！");
                return false;
            }
            this.printPageEnd = p;
            return true;
        }
        private bool checkPageRangeStart()
        {
            int p = 0;
            try
            {
                tbPageStart.Text = ToBj(tbPageStart.Text);
                p = Convert.ToInt32(tbPageStart.Text);
                if (p <= 0)
                {
                    MessageBox.Show("填入数据必须大于1！");
                    return false;
                }
                if (p > iTotalPageCount)
                {
                    MessageBox.Show("起始值必须小于页码总数！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Globals.logAdapter.Record("EX7569874339", ex.Message + ">>" + ex.ToString(), true);
                MessageBox.Show("填入数据必须为整数数字！");
                return false;
            }
            this.printPageStart = p;
            return true;
        }
        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
        }

        private void rbtnTwoPrint_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = true;
            groupBox2.Enabled = false;
            twoPrint = true;
        }

        private void rbtnOnePrint_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = false;
            groupBox2.Enabled = true;
            twoPrint = false;
        }

        private void rbtnAll_CheckedChanged(object sender, EventArgs e)
        {
            printAll = true;
            printThis = false;
            thiscursor = false;
            thiscursorDel = false;
            printPageRange = false;
            setPageRangeDisable();
        }

        private void rbtnThisPageNum_CheckedChanged(object sender, EventArgs e)
        {
            printThis = true;
            printAll = false;
            thiscursor = false;
            thiscursorDel = false;
            printPageRange = false;
            setPageRangeDisable();
        }

        private void rbtnCursor_CheckedChanged(object sender, EventArgs e)
        {
            thiscursor = true;
            printAll = false;
            printThis = false;
            thiscursorDel = false;
            printPageRange = false;
            setPageRangeDisable();
        }

        private void cboPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPrinter.Enabled == false)
            {
                return;
            }
            printerName = cboPrinter.SelectedItem.ToString();
        }
        private void InitPrint()
        {
            cboPrinter.DropDownStyle = ComboBoxStyle.DropDownList;
            List<string> printers = PrinterManager.GetLocalPrinters();
            PrintDocument pc = new PrintDocument();

            
            this.printerName = strDefaultPrinterName;
            string pn = pc.PrinterSettings.PrinterName;
            foreach (string p in printers)
            {
                if (p.Trim() == pn)
                    cboPrinter.SelectedItem = p;
                cboPrinter.Items.Add(p);
            }
            if (cboPrinter.Items.Count > 0)
            {
                cboPrinter.SelectedIndex = 0;
            }
        }

        private void ContiPrint_Load(object sender, EventArgs e)
        {
            InitPrint();
        }

        private void rbtnCursorDel_CheckedChanged(object sender, EventArgs e)
        {
            thiscursor = false;
            printAll = false;
            printThis = false;
            thiscursorDel = true;
            printPageRange = false;
            setPageRangeDisable();

        }

        private void setPageRangeDisable()
        {
            this.tbPageStart.Enabled = false;
            this.tbPageEnd.Enabled = false;
            this.tbPageStart.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tbPageEnd.BackColor = System.Drawing.SystemColors.ControlDark;
        }
        private void setPageRangeEnable()
        {
            this.tbPageStart.Enabled = true;
            this.tbPageEnd.Enabled = true;
            this.tbPageStart.BackColor = System.Drawing.SystemColors.Window;
            this.tbPageEnd.BackColor = System.Drawing.SystemColors.Window;
        }
        private void rbtnPageRange_CheckedChanged(object sender, EventArgs e)
        {
            setPageRangeEnable();
            thiscursor = false;
            printAll = false;
            printThis = false;
            thiscursorDel = false;
            printPageRange = true;
        }

        private void cbPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPrint.Checked)
            {
                cboPrinter.Enabled = false;
                this.printerName = this.strDefaultPrinterName;
            }
            else
            {
                cboPrinter.Enabled = true;
                if (cboPrinter.SelectedItem != null)
                {
                    this.printerName = cboPrinter.SelectedItem.ToString();
                }
            }

           
        }

        
    }
}
