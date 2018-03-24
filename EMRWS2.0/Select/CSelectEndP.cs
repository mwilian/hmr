using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using CommonLib;

namespace EMR
{
    public partial class CSelectEndP : Form
    {
        MainForm emrTaskPane = null;

        public CSelectEndP(MainForm etp)
        {
            InitializeComponent();
            emrTaskPane = etp;
            this.Location = new Point(450,120);
            
        }
       
        private void btnReport_Click(object sender, EventArgs e)
        {
            Globals.Cselectd = true;
            emrTaskPane.NewWordDoc();
            //emrTaskPane.btnSelectExit.Visible = true;
            this.Hide();
        }
        public DataTable GetDataNew()
        {
            string strStart = dateTimePickerStart.Value.ToString();
            string[] strListStart = strStart.Split(' ');
            strStart = strListStart[0].ToString() + " 00:00:00";

            string strEnd = dateTimePickerEnd.Value.ToString() + " 23:59:59";
            string[] strListEnd = strEnd.Split(' ');
            strEnd = strListEnd[0].ToString() + " 23:59:59";

            DataTable dt = new DataTable();
            dt.TableName = "getqc";
            DataSet dst = new DataSet();

            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                dst = pi.GetQCTableByMonthEndNew(strStart, strEnd, "$", Globals.DoctorID);

                dt = dst.Tables[0];
            }
            return dt;
        }

        public void GetNewReport(DataTable dt)
        {
           
            object oMissing = System.Reflection.Missing.Value;
            object unknow = Type.Missing;
            object start = 0;
            object end = 0;

            Microsoft.Office.Interop.Word.Range tableLocation = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
            // 添加页眉
            emrTaskPane.wordApp.ActiveWindow.View.Type = Word.WdViewType.wdOutlineView;
            emrTaskPane.wordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryHeader;
            emrTaskPane.wordApp.ActiveWindow.ActivePane.Selection.InsertAfter("");
            emrTaskPane.wordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;//设置右对齐
            emrTaskPane.wordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekMainDocument;//跳出页眉设置
            emrTaskPane.wordApp.Selection.ParagraphFormat.LineSpacing = 15f;//设置文档的行间距
            //移动焦点并换行
            object count = 14;
            object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            emrTaskPane.wordApp.Selection.MoveDown(ref WdLine, ref count, ref unknow);//移动焦点
            emrTaskPane.wordApp.Selection.MoveDown(ref WdLine, ref count, ref unknow);//移动焦点
            emrTaskPane.wordApp.Selection.TypeParagraph();//插入段落

            Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;
            range.Select();
            ///range.Text = "终末病历质量评分汇总表" + "(" + comboBox2.Text + "年" + comboBox1.Text + "月)";
            range.Text = "终末病历质量评分汇总表";
            range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            range.Font.Name = "黑体";
            range.Font.Size = 14.5f;
            range.Start = range.End;
            range.Select();
            Microsoft.Office.Interop.Word.Table newTable = ActiveDocumentManager.getDefaultAD().Tables.Add(emrTaskPane.wordApp.Selection.Range, dt.Rows.Count + 1, 8, ref unknow, ref unknow);

            //设置表格样式
            newTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            newTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            emrTaskPane.wordApp.Selection.ParagraphFormat.LineSpacing = 15f;//设置文档的行间距
            newTable.Cell(1, 1).Range.Text = "出院日期";
            newTable.Cell(1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 2).Range.Text = "科室";
            newTable.Cell(1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 3).Range.Text = "主管医师";
            newTable.Cell(1, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 4).Range.Text = "病案号";
            newTable.Cell(1, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 5).Range.Text = "扣分原因";
            newTable.Cell(1, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 6).Range.Text = "扣分";
            newTable.Cell(1, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 7).Range.Text = "得分";
            newTable.Cell(1, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 8).Range.Text = "病历级别";
            newTable.Cell(1, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Columns[1].Width = 40f;
            newTable.Columns[2].Width = 36f;
            newTable.Columns[3].Width = 45f;//能显示三个字
            newTable.Columns[4].Width = 43f;
            newTable.Columns[5].Width = 150f;
            newTable.Columns[6].Width = 40f;
            newTable.Columns[7].Width = 40f;
            newTable.Columns[8].Width = 36f;
            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    newTable.Cell(i + 2, 1).Range.Text = dt.Rows[i]["出院日期"].ToString();
                    newTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 2).Range.Text = dt.Rows[i]["科室"].ToString();
                    newTable.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 3).Range.Text = dt.Rows[i]["主管医师"].ToString();
                    newTable.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 4).Range.Text = dt.Rows[i]["病案号"].ToString();
                    newTable.Cell(i + 2, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 5).Range.Text = dt.Rows[i]["扣分原因"].ToString();
                    newTable.Cell(i + 2, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    newTable.Cell(i + 2, 6).Range.Text = dt.Rows[i]["扣分"].ToString();
                    newTable.Cell(i + 2, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 7).Range.Text = dt.Rows[i]["得分"].ToString();
                    newTable.Cell(i + 2, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 8).Range.Text = dt.Rows[i]["病历级别"].ToString();
                    newTable.Cell(i + 2, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                }
            }
            object Line1 = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            //object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            emrTaskPane.wordApp.Selection.MoveDown(ref Line1, ref count, ref unknow);//移动焦点
            emrTaskPane.wordApp.Selection.MoveDown(ref Line1, ref count, ref unknow);//移动焦点
            emrTaskPane.wordApp.Selection.TypeParagraph();//插入段落
            Word.Range range1 = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range1.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;
            range1.Select();
            range1.Text = "记录数共计" + dt.Rows.Count.ToString() + "条";
            range1.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            range1.Font.Name = "黑体";
            range1.Font.Size = 10f;
            range1.Start = range.End;
            //Globals.ThisAddIn.ShowNoteMenu();
            //Globals.ThisAddIn.noteMenu.OperationEnableForStat(true);

        }

        private void CSelectEndP_Deactivate(object sender, EventArgs e)
        {

        }
       
    }
}
