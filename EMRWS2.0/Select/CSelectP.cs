using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using System.Collections;
using UserInterface;
using EmrConstant;
using System.Xml;
using HuronControl;

using CommonLib;
using System.IO;

namespace EMR
{
    public partial class CSelectP : Form
    {
        MainForm emrTaskPane = null;

        bool hz = false;
        bool ks = false;
        bool qy = false;
        public CSelectP(MainForm etp)
        {
            InitializeComponent();
            emrTaskPane = etp;
            this.Location = new Point(450, 120);


           if (ThisAddIn.CanOption(ElementNames.ArchiveDepartment))
            {
                cboQy.Enabled = !Globals.ArchiveDepartment;
                string str = ThisAddIn.CanOptionText(ElementNames.ArchiveDepartmentText).Trim();
                if (ThisAddIn.CanOption(ElementNames.ArchiveDepartment) && ThisAddIn.CanOptionText(ElementNames.ArchiveDepartmentText).Trim() != "")
                {
                    if (str.Contains(",") == true)
                    {
                        string[] strlist = str.Split(',');
                        foreach (string strEach in strlist)
                        {
                            if (strEach.Trim() == Globals.OpDepartID)
                            {
                                cboQy.Enabled = true;
                                cboHz.Enabled = true;
                                return;
                            }
                            else
                            {
                                cboQy.Enabled = false;
                                cboHz.Enabled = false;
                            }
                            return; 
                        }
                    }
                }
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            Globals.Cselect = true;
            if (cboDepart.Checked) ks = true;
            if (cboHz.Checked) hz = true;
            if (cboQy.Checked) qy = true;
            Cursor.Current = Cursors.WaitCursor;
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
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                // DataSet dst = pi.GetQCTableByMonthNew(strStart, strEnd, "$", Globals.DoctorID);

                if (ks)
                   // dt = pi.ValuateNowPSSelect(strStart, strEnd, Globals.OpDepartID);
                    dt = pi.ValuateNowPSSelectKs(strStart, strEnd, Globals.OpDepartName);
                
                else if (hz)
                    // dt = pi.ValuateNowPjL(strStart, strEnd);
                    dt = pi.ValuateNowPSSelectHz(strStart, strEnd);
                else if (qy)
                    dt = pi.ValuateNowPSSelectQy(strStart, strEnd);
                //dt = dst.Tables[0];

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

            //2012-07-29 LiuQi 删除页眉
            int c = ActiveDocumentManager.getDefaultAD().Paragraphs[1].Borders.Count;
            emrTaskPane.wordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;
            Microsoft.Office.Interop.Word.Application WordApp;
            Microsoft.Office.Interop.Word.Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            if (c != 0)
            {
                Word.HeaderFooter hf = ActiveDocumentManager.getDefaultAD().Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                Word.Range r = hf.Range;
                //r.Start = r.End;
                r.Select();
                Word.Border border = r.Borders[Word.WdBorderType.wdBorderBottom];
                border.Visible = false;
            }
            //移动焦点并换行
            object count = 14;
            object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            emrTaskPane.wordApp.Selection.MoveDown(ref WdLine, ref count, ref unknow);//移动焦点
            emrTaskPane.wordApp.Selection.MoveDown(ref WdLine, ref count, ref unknow);//移动焦点
            emrTaskPane.wordApp.Selection.TypeParagraph();//插入段落
            Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;

            range.Select();
            //// range.Text = "病历质量月度评分表" + "(" + comboBox2.Text + "年" + comboBox1.Text + "月)";
            if (hz)
                range.Text = "各科室现岗病历质检情况一览表";
            else range.Text = "现岗病历质量评分科室汇总表";
            range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            range.Font.Name = "黑体";
            range.Font.Size = 15.5f;
            range.Start = range.End;
            range.Select();

  
            range.Text = dateTimePickerStart.Value.ToString() + "至" + dateTimePickerEnd.Value.ToString();
            range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            range.Font.Name = "黑体";
            range.Font.Size = 10f;
            range.Start = range.End;
            range.Select();

            ActiveDocumentManager.getDefaultAD().PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait;
            ActiveDocumentManager.getDefaultAD().PageSetup.PageWidth = emrTaskPane.wordApp.CentimetersToPoints(29.7F);
            ActiveDocumentManager.getDefaultAD().PageSetup.PageHeight = emrTaskPane.wordApp.CentimetersToPoints(21F);
            int rows = 0;
            if (ks || qy) rows = 9;
            else rows = 8;
            Microsoft.Office.Interop.Word.Table newTable = ActiveDocumentManager.getDefaultAD().Tables.Add(emrTaskPane.wordApp.Selection.Range, dt.Rows.Count + 1, rows, ref unknow, ref unknow);
            //设置表格样式
            newTable.Range.Font.Size = 11.0f;
            newTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            newTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            emrTaskPane.wordApp.Selection.ParagraphFormat.LineSpacing = 15f;//设置文档的行间距
            if (ks || qy)
            {
                newTable.Cell(1, 1).Range.Text = "科室";
                newTable.Cell(1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 2).Range.Text = "主治医师";
                newTable.Cell(1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                newTable.Cell(1, 3).Range.Text = "病案号";
                newTable.Cell(1, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 4).Range.Text = "住院日期";
                newTable.Cell(1, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                newTable.Cell(1, 5).Range.Text = "住院号";
                newTable.Cell(1, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 6).Range.Text = "扣分项目";
                newTable.Cell(1, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 7).Range.Text = "扣分原因";
                newTable.Cell(1, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 8).Range.Text = "总得分";
                newTable.Cell(1, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 9).Range.Text = "等级";
                newTable.Cell(1, 9).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                newTable.Columns[1].Width = 60f;
                newTable.Columns[2].Width = 60f;
                newTable.Columns[3].Width = 70f;//能显示三个字
                newTable.Columns[4].Width = 65f;
                newTable.Columns[5].Width = 60f;
                newTable.Columns[6].Width = 70f;
                newTable.Columns[7].Width = 220f;
                newTable.Columns[8].Width = 50f;
                newTable.Columns[9].Width = 40f;

                //if (dt.Rows.Count > 0)
                //{

                //    try
                //    {
                //        for (int i = 0; i < dt.Rows.Count; i++)
                //        {

                //            newTable.Cell(i + 2, 4).Range.Text = dt.Rows[i]["zyrq"].ToString();
                //            newTable.Cell(i + 2, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //            newTable.Cell(i + 2, 1).Range.Text = dt.Rows[i]["ksmc"].ToString();
                //            newTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //            newTable.Cell(i + 2, 2).Range.Text = dt.Rows[i]["ysm"].ToString();
                //            newTable.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //            newTable.Cell(i + 2, 3).Range.Text = dt.Rows[i]["bah"].ToString();
                //            newTable.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //            newTable.Cell(i + 2, 5).Range.Text = dt.Rows[i]["zyh"].ToString();
                //            newTable.Cell(i + 2, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;


                //            newTable.Cell(i + 2, 6).Range.Text = dt.Rows[i]["kfxm"].ToString();
                //            newTable.Cell(i + 2, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                //            newTable.Cell(i + 2, 7).Range.Text = dt.Rows[i]["kfyy"].ToString();
                //            newTable.Cell(i + 2, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //            newTable.Cell(i + 2, 8).Range.Text = dt.Rows[i]["sdf"].ToString();
                //            newTable.Cell(i + 2, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //            newTable.Cell(i + 2, 9).Range.Text = dt.Rows[i]["pj"].ToString();
                //            newTable.Cell(i + 2, 9).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                //        }

                //        verticalCellMerge(newTable, 2, 1);
                //        verticalCellMerge(newTable, 2, 2);
                //        verticalCellMerge(newTable, 2, 3);
                //        verticalCellMerge(newTable, 2, 4);                      
                //        verticalCellMerge(newTable, 2, 8);
                //        verticalCellMerge(newTable, 2, 9);
                //        verticalCellMerge(newTable, 2, 5);
                //        // verticalCellMerge(newTable,2,5);
                //    }
                //    catch (Exception ex)
                //    {

                //        MessageBox.Show(ex.ToString());
                //    }
                //}
                if (dt.Rows.Count > 0)
                {

                    try
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            newTable.Cell(i + 2, 4).Range.Text = dt.Rows[i]["住院日期"].ToString();
                            newTable.Cell(i + 2, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            newTable.Cell(i + 2, 1).Range.Text = dt.Rows[i]["科室"].ToString();
                            newTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            newTable.Cell(i + 2, 2).Range.Text = dt.Rows[i]["质检医师"].ToString();
                            newTable.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            newTable.Cell(i + 2, 3).Range.Text = dt.Rows[i]["病案号"].ToString();
                            newTable.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            newTable.Cell(i + 2, 5).Range.Text = dt.Rows[i]["住院号"].ToString();
                            newTable.Cell(i + 2, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;


                            newTable.Cell(i + 2, 6).Range.Text = dt.Rows[i]["扣分项目"].ToString();
                            newTable.Cell(i + 2, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            newTable.Cell(i + 2, 7).Range.Text = dt.Rows[i]["扣分原因"].ToString();
                            newTable.Cell(i + 2, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            newTable.Cell(i + 2, 8).Range.Text = dt.Rows[i]["总得分"].ToString();
                            newTable.Cell(i + 2, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            newTable.Cell(i + 2, 9).Range.Text = dt.Rows[i]["评审结果"].ToString();
                            newTable.Cell(i + 2, 9).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                        }

                        verticalCellMerge(newTable, 2, 1);
                        verticalCellMerge(newTable, 2, 2);
                        verticalCellMerge(newTable, 2, 3);
                        verticalCellMerge(newTable, 2, 4);
                        verticalCellMerge(newTable, 2, 8);
                        verticalCellMerge(newTable, 2, 9);
                        verticalCellMerge(newTable, 2, 5);
                        // verticalCellMerge(newTable,2,5);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                newTable.Cell(1, 1).Range.Text = "科室";
                newTable.Cell(1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 2).Range.Text = "病历质检数";
                newTable.Cell(1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 3).Range.Text = "甲级";
                newTable.Cell(1, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 4).Range.Text = "甲级病历率";
                newTable.Cell(1, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 5).Range.Text = "乙级";
                newTable.Cell(1, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 6).Range.Text = "乙级病历率";
                newTable.Cell(1, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 7).Range.Text = "丙级";
                newTable.Cell(1, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                newTable.Cell(1, 8).Range.Text = "丙级病历率";
                newTable.Cell(1, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                newTable.Columns[1].Width = 70f;
                newTable.Columns[2].Width = 70f;
                newTable.Columns[3].Width = 70f;//能显示三个字
                newTable.Columns[4].Width = 70f;
                newTable.Columns[5].Width = 70f;
                newTable.Columns[6].Width = 70f;

                if (dt.Rows.Count > 0)
                {
                    int k = 2;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        decimal bl = Convert.ToDecimal(dt.Rows[i]["病历率"].ToString());
                        string strbl = (Convert.ToDecimal(bl.ToString("0.00")) * 100).ToString() + "%";

                        if (i > 0)
                        {
                            if (dt.Rows[i]["科室"].ToString() == dt.Rows[i - 1]["科室"].ToString())
                            {
                                newTable.Cell(k, 1).Range.Text = dt.Rows[i]["科室"].ToString();
                                newTable.Cell(k, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                newTable.Cell(k, 2).Range.Text = dt.Rows[i]["科室质检数"].ToString();
                                newTable.Cell(k, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                                if (dt.Rows[i]["评级"].ToString() == "甲")
                                {
                                    newTable.Cell(k, 3).Range.Text = dt.Rows[i]["病历数"].ToString();
                                    newTable.Cell(k, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    newTable.Cell(k, 4).Range.Text = strbl;
                                    newTable.Cell(k, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else if (dt.Rows[i]["评级"].ToString() == "乙")
                                {
                                    newTable.Cell(k, 5).Range.Text = dt.Rows[i]["病历数"].ToString();
                                    newTable.Cell(k, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    newTable.Cell(k, 6).Range.Text = strbl;
                                    newTable.Cell(k, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    
                                }
                                else if (dt.Rows[i]["评级"].ToString() == "丙")
                                {
                                    newTable.Cell(k, 7).Range.Text = dt.Rows[i]["病历数"].ToString();
                                    newTable.Cell(k, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    newTable.Cell(k, 8).Range.Text = strbl;
                                    newTable.Cell(k, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                            }
                            else
                            {
                                k++;
                                newTable.Cell(k, 1).Range.Text = dt.Rows[i]["科室"].ToString();
                                newTable.Cell(k, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                newTable.Cell(k, 2).Range.Text = dt.Rows[i]["科室质检数"].ToString(); ;
                                newTable.Cell(k, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                                if (dt.Rows[i]["评级"].ToString() == "甲")
                                {
                                    newTable.Cell(k, 3).Range.Text = dt.Rows[i]["病历数"].ToString();
                                    newTable.Cell(k, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    newTable.Cell(k, 4).Range.Text = strbl;
                                    newTable.Cell(k, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else if (dt.Rows[i]["评级"].ToString() == "乙")
                                {
                                    newTable.Cell(k, 5).Range.Text = dt.Rows[i]["病历数"].ToString();
                                    newTable.Cell(k, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    newTable.Cell(k, 6).Range.Text = strbl;
                                    newTable.Cell(k, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                                }
                                else if (dt.Rows[i]["评级"].ToString() == "丙")
                                {
                                    newTable.Cell(k, 7).Range.Text = dt.Rows[i]["病历数"].ToString();
                                    newTable.Cell(k, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    newTable.Cell(k, 8).Range.Text = strbl;
                                    newTable.Cell(k, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                            }
                        }
                        else
                        {
                            newTable.Cell(i + 2, 1).Range.Text = dt.Rows[i]["科室"].ToString();
                            newTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            newTable.Cell(i + 2, 2).Range.Text = dt.Rows[i]["科室质检数"].ToString();
                            newTable.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                            if (dt.Rows[i]["评级"].ToString() == "甲")
                            {
                                newTable.Cell(i + 2, 3).Range.Text = dt.Rows[i]["病历数"].ToString();
                                newTable.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                newTable.Cell(i + 2, 4).Range.Text = strbl;
                                newTable.Cell(i + 2, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            }
                            else if (dt.Rows[i]["评级"].ToString() == "乙")
                            {
                                newTable.Cell(i + 2, 5).Range.Text = dt.Rows[i]["病历数"].ToString();
                                newTable.Cell(i + 2, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                newTable.Cell(i + 2, 6).Range.Text = strbl;
                                newTable.Cell(i + 2, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                            }
                            else if (dt.Rows[i]["评级"].ToString() == "丙")
                            {
                                newTable.Cell(i + 2, 7).Range.Text = dt.Rows[i]["病历数"].ToString();
                                newTable.Cell(i + 2, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                newTable.Cell(i + 2, 8).Range.Text = strbl;
                                newTable.Cell(i + 2, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            }
                        }
                    }
                    //删除多余的行
                    for (int i = 2; i <= newTable.Rows.Count; i++)
                    {
                        if (newTable.Cell(i, 1).Range.Text == "\r\a")
                        {
                            newTable.Rows[i].Delete();
                            i = i - 2;
                        }
                    }
                }

            }
            object Line1 = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            //object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            emrTaskPane.wordApp.Selection.MoveDown(ref Line1, ref count, ref unknow);//移动焦点
            emrTaskPane.wordApp.Selection.MoveDown(ref Line1, ref count, ref unknow);//移动焦点

            emrTaskPane.wordApp.Selection.TypeParagraph();//插入段落
            Word.Range range1 = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range1.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;
            int count2 = newTable.Rows.Count - 1;
            range1.Select();
            range1.Text = "记录数共计" + count2 + "条";

            range1.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            range1.Font.Name = "黑体";
            range1.Font.Size = 10f;
            range1.Start = range.End;
            //Globals.ThisAddIn.ShowNoteMenu();
            //OperationEnableForStat(true);
            Cursor.Current = Cursors.Default;
        }

        private void verticalCellMerge(Word.Table table, int startRowIndex, int columnIndex)
        {
            string previousText = table.Cell(startRowIndex++, columnIndex).Range.Text;    // 保存对比文字  
            string zyhText = table.Cell(startRowIndex, 5).Range.Text;
            int previousRowIndex = startRowIndex - 1;    // 因刚已经+1了，所以再减回去    
            for (int i = startRowIndex; i <= table.Rows.Count; ++i) // 遍历所有行的columnIndex列，发现相同的合并，从起始行的下一行开始对比   
            {
                string currentText = table.Cell(i, columnIndex).Range.Text;
                string currentZyhText = table.Cell(i, 5).Range.Text;
                if (columnIndex == 8 || columnIndex == 9)
                {
                    if (previousText.Equals(currentText) && zyhText.Equals(currentZyhText))
                    {
                        table.Cell(previousRowIndex, columnIndex).Merge(table.Cell(i, columnIndex)); // 合并先前单元格和当前单元格   
                        table.Cell(previousRowIndex, columnIndex).Range.Text = currentText;    // 因为合并后并没有将单元格内容去除，需要手动修改         
                        table.Cell(previousRowIndex, columnIndex).Select();
                        emrTaskPane.wordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;    // 水平居中显示            
                        table.Cell(previousRowIndex, columnIndex).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter; // 垂直居中        
                    }
                    else
                    {
                        previousText = currentText; // 将对比文字替换为当前的内容      
                        zyhText = currentZyhText;
                        previousRowIndex = i;   // 检索到不同的内容，将当前行下标置为先前行下标，用于合并   
                    }
                    table.Cell(previousRowIndex, columnIndex).Range.Text = currentText;
                }
                else 
                {
                      if (previousText.Equals(currentText))
                        {
                            table.Cell(previousRowIndex, columnIndex).Merge(table.Cell(i, columnIndex)); // 合并先前单元格和当前单元格   
                            table.Cell(previousRowIndex, columnIndex).Range.Text = currentText;    // 因为合并后并没有将单元格内容去除，需要手动修改         
                            table.Cell(previousRowIndex, columnIndex).Select();
                            emrTaskPane.wordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;    // 水平居中显示            
                            table.Cell(previousRowIndex, columnIndex).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter; // 垂直居中        
                        }
                        else
                        {
                            previousText = currentText; // 将对比文字替换为当前的内容           
                            previousRowIndex = i;   // 检索到不同的内容，将当前行下标置为先前行下标，用于合并   
                        }
                        table.Cell(previousRowIndex, columnIndex).Range.Text = currentText;
                    
                }
            }
        }
        private void CSelectP_Deactivate(object sender, EventArgs e)
        {

        }

        private void btnfindExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
