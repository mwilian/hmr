using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;

namespace EMR
{
    public class DGVPrinter
    {
        PrintDocument printDoc;
        PageSetupDialog pageSetupDlg;
        PrintPreviewDialog previewDlg;
        PrintDialog printDlg;

        DataGridView sourceDGV;
        public DataGridView SourceDGV
        {
            get { return sourceDGV; }
            set { sourceDGV = value; }
        }

        int printedRowsCount=0;
        public Font printFont;

        public string mainTitle = "";
        public string subTitle = "";
        public string subTitle2 = "";
        public string footer = "";


        public string subArchiveNum = "";
        public string subPatientName = "";
        public string subSex = "";
        public string subRegistryDate = "";
        public string subDischargedDate = "";
        public string subHosName = "";
        public string subZz = "";
        public string subZg = "";//主管医师
        public string subZr = "";//责任医师

       

  
        public DGVPrinter()
        {
            printFont = SystemFonts.DefaultFont;

            printDoc = new PrintDocument();
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);

            pageSetupDlg = new PageSetupDialog();
            pageSetupDlg.Document = printDoc;
            pageSetupDlg.MinMargins=new Margins(10, 10, 10, 10);
            pageSetupDlg.PrinterSettings.DefaultPageSettings.Landscape = true; //横向打印


            previewDlg = new PrintPreviewDialog();
            previewDlg.Document = printDoc;
            previewDlg.Width = 600;
            previewDlg.Height = 450;
            previewDlg.PrintPreviewControl.Zoom = 1.0;

            printDlg = new PrintDialog();
            printDlg.Document = printDoc;

        }
        void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;

            Pen myPen=new Pen(Color.Gray);
            Brush brush=Brushes.LightGray;

            //打印表头
            //if (printedRowsCount == 0)
            //{
                if (mainTitle != "")
                {
                    Font mainTitleFont = new Font(new FontFamily("宋体"), 20, FontStyle.Bold);
                    SizeF strSize = e.Graphics.MeasureString(mainTitle, mainTitleFont, int.MaxValue);
                    if (e.PageBounds.Width > strSize.Width)
                    {
                        x = (e.PageBounds.Width - Convert.ToInt32(strSize.Width)) / 2;
                    }
                    e.Graphics.DrawString(mainTitle, mainTitleFont, Brushes.Black, x, y);
                    y += Convert.ToInt32(strSize.Height) + 5;
                    x = e.MarginBounds.Left;
                }
                if (subTitle != "")
                {
                    Font subTitleFont = new Font(new FontFamily("宋体"), 9, FontStyle.Regular);
                    SizeF strSize = e.Graphics.MeasureString(subTitle, subTitleFont, int.MaxValue);
                    if (e.PageBounds.Width > strSize.Width + 80)
                    {
                       // x = (e.PageBounds.Width - Convert.ToInt32(strSize.Width)) * 2 / 5;
                        x = 100;
                    }
                    e.Graphics.DrawString(subTitle, subTitleFont, Brushes.Black, x, y);
                    y += Convert.ToInt32(strSize.Height) + 5;
                    x = e.MarginBounds.Left;
                }
           // }

            //打印列标题
            for (int i = 0; i < sourceDGV.Columns.Count-1; i++)
            {
                int columnWidth = sourceDGV.Columns[i].Width;
                int headerHeight = sourceDGV.ColumnHeadersHeight;
                
                Rectangle rect=new Rectangle(x,y,columnWidth,headerHeight);
                
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(myPen, rect);

                DrawStringCenter(e.Graphics, sourceDGV.Columns[i].HeaderText, rect);
                x += rect.Width;
            }
            x = e.MarginBounds.Left;
            y += sourceDGV.ColumnHeadersHeight;

            //打印表格内容
            for (int i = printedRowsCount; i < sourceDGV.Rows.Count; i++)
            {
                int rowHeight=sourceDGV.Rows[i].Height;
                for(int j=0;j<sourceDGV.Columns.Count-1;j++)
                {
                    int columnWidth=sourceDGV.Columns[j].Width;
                    Rectangle rect = new Rectangle(x, y, columnWidth, rowHeight);
                    e.Graphics.DrawRectangle(myPen, rect);



                    if (sourceDGV.Rows[i].Cells[j].Value != null)
                            DrawStringCenter(e.Graphics, sourceDGV.Rows[i].Cells[j].Value.ToString(), rect);
                        x += columnWidth;
                   
                }
                printedRowsCount++;
                x = e.MarginBounds.Left;
                y += rowHeight;
                if (y > e.PageBounds.Height - 80 && printedRowsCount < sourceDGV.Rows.Count)
                {
                    e.HasMorePages = true;//允许多页打印
                    return;
                }
            }
            if (subTitle2 != "")
            {
                Font subTitleFont = new Font(new FontFamily("宋体"), 9, FontStyle.Regular);
                SizeF strSize = e.Graphics.MeasureString(subTitle2, subTitleFont, int.MaxValue);
                if (e.PageBounds.Width > strSize.Width + 80)
                {
                    // x = (e.PageBounds.Width - Convert.ToInt32(strSize.Width)) * 2 / 5;
                    x = 100;
                }
                e.Graphics.DrawString(subTitle2, subTitleFont, Brushes.Black, x, y+10);
                y += Convert.ToInt32(strSize.Height) + 5;
                x = e.MarginBounds.Left;
            }

            printedRowsCount = 0;
        }

        /// <summary>
        /// 将字符串打印在指定矩形的中间
        /// </summary>
        /// <param name="g"></param>
        /// <param name="str">要打印的字符串</param>
        /// <param name="rect">目标位置矩形</param>
        private void  DrawStringCenter( Graphics g,string str,Rectangle rect)
        {
            PointF resPos=new PointF(rect.X,rect.Y);
            SizeF strSize = g.MeasureString(str, printFont, int.MaxValue);
            if (rect.Width > strSize.Width)
            {
                resPos.X = rect.X + (rect.Width - Convert.ToInt32(strSize.Width)) / 2;
            }
            if (rect.Height > strSize.Height)
            {
                resPos.Y = rect.Y + (rect.Height - Convert.ToInt32(strSize.Height)) / 2;
            }
            Rectangle newRect = new Rectangle(Convert.ToInt32(resPos.X), Convert.ToInt32(resPos.Y), rect.Width, rect.Height);
            g.DrawString(str, printFont, Brushes.Black, newRect,StringFormat.GenericTypographic);
        }
        
        public void PrintDataGridView(DataGridView dgv)
        {
            sourceDGV = dgv;
            if (printDlg.ShowDialog() == DialogResult.OK)
            {
                previewDlg.ShowDialog();
            }
        }
        public void SetupPage()
        {
            pageSetupDlg.ShowDialog();
        }
        public DialogResult PrintPreview()
        {
            return previewDlg.ShowDialog();
        }
        public DialogResult ShowPrintDialog()
        {
             return printDlg.ShowDialog();
        }
    }
}
