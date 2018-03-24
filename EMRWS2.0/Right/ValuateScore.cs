using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;
using System.Collections;


using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
//using Tools = Microsoft.Office.Tools;
using CommonLib;
using UserInterface;




namespace EMR
{
    public partial class ValuateScore : Form
    {
        public ArrayList ArrayListNoteID = new ArrayList();
        public ArrayList ArrayListOff = new ArrayList();
        public ArrayList ArrayListReason = new ArrayList();
        private ValuateText valuateText = new ValuateText();
        MainForm emrtaskpane = null;
        private int count = 0;
        private int newLocation = 1;
        private string patientDoctor = null;
        private string patientDepartment = null;
        private bool selfValuate = false;
        private string regID = null;
        private decimal scorePercent = 0;
        private int vi = 0;    // index of valuate level
        private string registryIDTemp = "";
        public ValuateScore()
        {
            InitializeComponent();
        }
        public bool Init(bool self, string registryID, string doctorID, string department, XmlNode emrdoc,MainForm etp)
        {
            try
            {

                emrtaskpane = etp;
                registryIDTemp = registryID;
                patientDepartment = department;
                patientDoctor = doctorID;
                selfValuate = self;
                regID = registryID;
                lbPatient.Text = emrtaskpane.GetPatientName();

                XmlNode flaws = null;
                XmlNode rules = null;
                #region Get rulus and flaws from database
              
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {

                    string msg = "";
                    if (Globals.isEndRule == false)
                    {
                        msg = es.GetValuateDetail(self, registryID, ref flaws);
                    }
                    else
                    {
                        msg = es.GetValuateDetailEnd(self, registryID, ref flaws);
                    }
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return false;
                    }
                    msg = es.GetNoteIDsWithValuateRules(ref rules);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return false;
                    }

                }
                if (flaws == null)
                {
                    MessageBox.Show("尚未对记录评分，先评分再定级！", ErrorMessage.Error);
                    return false;
                }
                #endregion
                foreach (XmlNode note in flaws.ChildNodes)
                {
                    NewFlaw(note);
                }

                if (AbsenceNotes(rules, emrdoc))
                {
                    this.Tag = StringGeneral.One;
                    return true;
                }

                LateNotes(rules, emrdoc);

                MakeScore(rules);

                this.Tag = StringGeneral.One;


                return true;
            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX925511256760", ex.Message + ">>" + ex.ToString(), true);
                return false;
            }

        }
        private void NewFlaw(XmlNode note)
        {
            Flaw ucFlaw = new Flaw(note);
            split.Panel1.Controls.Add(ucFlaw);
            ucFlaw.Width = split.Panel1.Width;
            ucFlaw.Height = ucFlaw.GetControlHeight();
            ucFlaw.Top = newLocation;
            newLocation += ucFlaw.Height + 1;
            count++;
            ucFlaw.Visible = true;
        }
        private void LateNotes(XmlNode rules, XmlNode emrdoc)
        {
            foreach (Control ctrl in split.Panel1.Controls)
            {
                if (ctrl.Name == "lbValuate" || ctrl.Name == "lbScore" || ctrl.Name == "lbLevel" || ctrl.Name == "lbKnock")
                { }
                else
                {
                    Flaw flaw = (Flaw)ctrl;
                    string noteID = flaw.GetNoteID();
                    decimal lateKnockoff = LateKnockoff(noteID, rules);
                    if (lateKnockoff == 0) continue;

                    if (!Late(noteID, emrdoc)) continue;

                    flaw.NewFlaw("书写不及时", lateKnockoff);


                    ArrayListNoteID.Add(noteID);
                    ArrayListOff.Add(lateKnockoff);
                    ArrayListReason.Add("书写不及时");

                }

            }

            RerangeControls();
        }
        private void RerangeControls()
        {
            int newLocation = 1;
            foreach (Control ctrl in split.Panel1.Controls)
            {
                ctrl.Top = newLocation;
                newLocation += ctrl.Height + 1;
            }
        }
        private decimal LateKnockoff(string noteID, XmlNode rules)
        {
            foreach (XmlNode rule in rules.ChildNodes)
            {
                if (rule.Attributes[AttributeNames.NoteID].Value != noteID) continue;

                if (rule.Attributes[AttributeNames.Late] == null) return 0;
                else return Convert.ToDecimal(rule.Attributes[AttributeNames.Late].Value);
            }
            return 0;
        }
        private bool Late(string noteID, XmlNode emrdoc)
        {
            DateTime lwt = DateTime.Now;
            foreach (XmlNode note in emrdoc.SelectNodes(ElementNames.EmrNote))
            {
                if (noteID == note.Attributes[AttributeNames.NoteID].Value)
                {
                    long lastWritenTime = Convert.ToInt64(note.Attributes[AttributeNames.LastWriteTime].Value);
                    lwt = DateTime.FromFileTime(lastWritenTime);
                    break;
                }

            }

            return
                ThisAddIn.TimeOutEmr(noteID, emrdoc.Attributes[AttributeNames.RegistryID].Value, lwt);
        }
        private int NewControlLocation()
        {
            int height = 0;
            foreach (Control ctrl in split.Panel1.Controls)
            {
                height += ctrl.Height + 1;
            }
            return height;
        }
        private void MakeScore(XmlNode rules)
        {
            decimal scoreTotal = 0;
            decimal knockTotal = 0;
            foreach (XmlNode rule in rules.ChildNodes)
            {
                scoreTotal += GetNoteScore(rule.Attributes[AttributeNames.NoteID].Value);
                knockTotal += GetKnockoff(rule.Attributes[AttributeNames.NoteID].Value);
            }
            lbScore.Text += scoreTotal.ToString();
            lbKnock.Text += knockTotal.ToString();
            lbValuate.Text = ValuateLevel(scoreTotal, knockTotal);

            lbScore.Visible = true;
            lbKnock.Visible = true;
        }
        private decimal GetNoteScore(string noteID)
        {

            foreach (Control ctrl in split.Panel1.Controls)
            {
                if (ctrl.Name == "lbValuate" || ctrl.Name == "lbScore" || ctrl.Name == "lbLevel" || ctrl.Name == "lbKnock")
                { }
                else
                {
                    Flaw flaw = (Flaw)ctrl;
                    if (noteID == flaw.GetNoteID()) return flaw.GetScore();
                }
            }

            return 0;
        }
        private decimal GetKnockoff(string noteID)
        {
            foreach (Control ctrl in split.Panel1.Controls)
            {
                if (ctrl.Name == "lbValuate" || ctrl.Name == "lbScore" || ctrl.Name == "lbLevel" || ctrl.Name == "lbKnock")
                { }
                else
                {
                    Flaw flaw = (Flaw)ctrl;
                    if (noteID == flaw.GetNoteID()) return flaw.GetKnockoff();
                }
            }
            return 0;
        }
        private string ValuateLevel(decimal scoreTotal, decimal knockTotal)
        {
            decimal levelC = Convert.ToDecimal(0.6);
            decimal levelB = Convert.ToDecimal(0.8);
            vi = 0;
            scorePercent = scoreTotal / (scoreTotal + knockTotal);
            if (scorePercent <= levelC) vi = ValuateIndex.C;
            else if (scorePercent <= levelB) vi = ValuateIndex.B;
            else vi = ValuateIndex.A;

            return valuateText.Text[vi];
        }
        private bool AbsenceNotes(XmlNode rules, XmlNode emrdoc)
        {
            TreeView absenceNotes = new TreeView();

            foreach (XmlNode rule in rules.ChildNodes)
            {
                if (rule.Attributes[AttributeNames.Required].Value == StringGeneral.Yes)
                {
                    if (!Absence(rule.Attributes[AttributeNames.NoteID].Value, emrdoc)) continue;
                    if (rule.Attributes[AttributeNames.Substitute] != null)
                    {
                        if (!SubstituteAbsence(rule.Attributes[AttributeNames.Substitute].Value, emrdoc)) continue;
                    }
                }
                else
                {
                  
                    if (rule.Attributes[AttributeNames.Condition] == null) continue;
                    if (!SatisfiedCondition(rule.Attributes[AttributeNames.Condition].Value,
                        rule.Attributes[AttributeNames.NoteID].Value, emrdoc)) continue;
                }
     
                string noteName =
                    Globals.emrPattern.GetNoteNameFromNoteID(rule.Attributes[AttributeNames.NoteID].Value);
                absenceNotes.Nodes.Add(noteName);
       
            }
            #region Unqualified emr
            if (absenceNotes.Nodes.Count > 0)
            {
                split.Panel2.Controls.Add(absenceNotes);
                absenceNotes.Top = lbAbsence.Top + lbAbsence.Height;
                absenceNotes.Width = split.Panel2.Width - 2;
                absenceNotes.Height = absenceNotes.Nodes.Count * (int)(Math.Ceiling(absenceNotes.Font.Size) + 5)
                    + absenceNotes.Margin.Top + absenceNotes.Margin.Bottom;
                absenceNotes.ShowLines = false;
                absenceNotes.Visible = true;
                lbAbsence.Visible = true;

                lbValuate.Text = valuateText.Text[ValuateIndex.D];
                lbValuate.Tag = ValuateIndex.D;
                vi = ValuateIndex.D;
                return true;
            }
            #endregion
            return false;
        }
        private bool SatisfiedCondition(string condition, string noteID, XmlNode emrdoc)
        {

            bool ret = false;
            switch (condition)
            {
                case Condition.Dead:
                    DateTime deadtime = ThisAddIn.StartTime(noteID, StartTime.Dead, regID);
                    if (deadtime == DateTime.MinValue) ret = false;
                    else ret = Absence(noteID, emrdoc);
                    break;
                case Condition.Operation:
                    #region Operation
                    XmlNode opetime = null;
                    ThisAddIn.OperationTime(regID, ref opetime);
                    if (opetime == null) ret = false;
                    else if (opetime.HasChildNodes)
                    {
                        foreach (XmlNode item in opetime.ChildNodes)
                        {
                            string seq = item.Attributes[AttributeNames.Sequence].Value;
                            if (Absence(noteID, seq, emrdoc))
                            {
                                ret = true;
                                break;
                            }
                        }
                    }
                    #endregion
                    break;
                case Condition.Hours48:
                    if (StayInHospitalHours(noteID, regID).TotalHours > 48) ret = Absence(noteID, emrdoc);
                    break;
                case Condition.Hours72:
                    if (StayInHospitalHours(noteID, regID).TotalHours > 72) ret = Absence(noteID, emrdoc);
                    break;
            }
            return ret;
        }
        private TimeSpan StayInHospitalHours(string noteID, string regID)
        {
            DateTime regdate = ThisAddIn.StartTime(noteID, StartTime.Registry, regID);
            DateTime dischagedDate = ThisAddIn.StartTime(noteID, StartTime.Discharged, regID);
            TimeSpan passHours = dischagedDate.Subtract(regdate);
            return passHours;
        }
        private bool Absence(string noteID, XmlNode emrdoc)
        {
            XmlNodeList emrNotes = emrdoc.SelectNodes(ElementNames.EmrNote);
            foreach (XmlNode emrNote in emrNotes)
            {
                if (noteID == emrNote.Attributes[AttributeNames.NoteID].Value) return false;
            }
            return true;
        }
        private bool Absence(string noteID, string seq, XmlNode emrdoc)
        {
            XmlNodeList emrNotes = emrdoc.SelectNodes(ElementNames.EmrNote);
            foreach (XmlNode emrNote in emrNotes)
            {
                if (noteID == emrNote.Attributes[AttributeNames.NoteID].Value &&
                    seq == emrNote.Attributes[AttributeNames.Sequence].Value) return false;
            }
            return true;
        }
        private bool SubstituteAbsence(string substitutes, XmlNode emrdoc)
        {
            string[] subs = substitutes.Split(Delimiters.Colon);
            for (int k = 0; k < subs.Length; k++)
            {
                if (!Absence(subs[k], emrdoc)) return false;
            }
            return true;
        }
        private void split_Panel1_Resize(object sender, EventArgs e)
        {
            foreach (Control ctrl in split.Panel1.Controls)
            {
                ctrl.Width = split.Panel1.Width;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (this.Tag.ToString() == StringGeneral.One)
            {
                if (MessageBox.Show("评定结果没有保存，如果继续将放弃结果。\r继续吗？", ErrorMessage.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    == DialogResult.No) return;
            }
            Close();
        }

        private void ValuateScore_Resize(object sender, EventArgs e)
        {
            split.Width = this.Width - 2;
            split.Top = btnSave.Height + 3;
            split.Height = this.Height - split.Top - 30;
            btnSave.Left = this.Width - btnSave.Width - 10;


        }

        private void split_Panel2_Resize(object sender, EventArgs e)
        {
            foreach (Control ctrl in split.Panel2.Controls)
            {
                string type = ctrl.GetType().ToString();
                if (type == "System.Windows.Forms.TreeView")
                {
                    ctrl.Width = split.Panel2.Width - 1;
                    break;
                }
            }
            lbValuate.Top = split.Panel2.Height - lbValuate.Height - 1;
            lbLevel.Top = lbValuate.Top;
            lbScore.Top = lbLevel.Top;
            lbKnock.Top = lbLevel.Top - lbKnock.Height - 2;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = "";
                if (Globals.isEndRule == false)
                {
                    msg = es.NewValuateScore(selfValuate, regID, patientDoctor, patientDepartment,
                    Globals.DoctorID, scorePercent, vi);
                }
                else
                {
                    msg = es.NewValuateScoreEnd(selfValuate, regID, patientDoctor, patientDepartment,
                                      Globals.DoctorID, scorePercent, vi);
                }
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
                using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                {
                    //string strRegistryID = Globals.ThisAddIn
                    for(int i = 0;i<ArrayListNoteID.Count;i++)
                    {
                        ep.InSertValueOff(registryIDTemp, ArrayListNoteID[i].ToString(), ArrayListReason[i].ToString(), ArrayListOff[i].ToString());
                    }
                }
                OpDone opd = new OpDone("保存成功！");
                opd.Show();

                this.Tag = StringGeneral.Zero;
            }


        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            //ReportForm reportFrom = new ReportForm();
            //reportFrom.registryID = regID;
            //reportFrom.blSelf = selfValuate;
            //reportFrom.strLevel = this.lbValuate.Text.Trim();
            //reportFrom.strSumScore = this.lbScore.Text.Trim();
            //reportFrom.strPaitientName = this.lbPatient.Text.Trim();
            //reportFrom.ShowDialog();
        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetNewReport(DataTable dt, string Score, string Knock, string strDep)
        {
           
            object oMissing = System.Reflection.Missing.Value;
            object unknow = Type.Missing;

            object start = 0;
            object end = 0;

            Microsoft.Office.Interop.Word.Range tableLocation = ActiveDocumentManager.getDefaultAD().Range(ref start, ref end);
            // 添加页眉
            emrtaskpane.wordApp.ActiveWindow.View.Type = Word.WdViewType.wdOutlineView;
            emrtaskpane.wordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryHeader;
            emrtaskpane.wordApp.ActiveWindow.ActivePane.Selection.InsertAfter("");
            emrtaskpane.wordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;//设置右对齐
            emrtaskpane.wordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekMainDocument;//跳出页眉设置

            emrtaskpane.wordApp.Selection.ParagraphFormat.LineSpacing = 15f;//设置文档的行间距

            //移动焦点并换行
            object count = 14;
            object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            emrtaskpane.wordApp.Selection.MoveDown(ref WdLine, ref count, ref unknow);//移动焦点
            emrtaskpane.wordApp.Selection.MoveDown(ref WdLine, ref count, ref unknow);//移动焦点

            emrtaskpane.wordApp.Selection.TypeParagraph();//插入段落

            Word.Range range = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;

            range.Select();
            range.Text = "扣分情况明细表";
            range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            range.Font.Name = "黑体";
            range.Font.Size = 14.5f;
            range.Start = range.End;
            range.Select();

            /////////Microsoft.Office.Interop.Word.Table newTable = ActiveDocumentManager.getDefaultAD().Tables.Add(emrtaskpane.wordApp.Selection.Range, dt.Rows.Count+1, 9, ref unknow, ref unknow);
            Microsoft.Office.Interop.Word.Table newTable = ActiveDocumentManager.getDefaultAD().Tables.Add(emrtaskpane.wordApp.Selection.Range, dt.Rows.Count + 1, 4, ref unknow, ref unknow);



            //设置表格样式
            newTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            newTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            emrtaskpane.wordApp.Selection.ParagraphFormat.LineSpacing = 15f;//设置文档的行间距

            newTable.Cell(1, 1).Range.Text = "记录名称";
            newTable.Cell(1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 2).Range.Text = "扣分原因";
            newTable.Cell(1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 3).Range.Text = "扣分";
            newTable.Cell(1, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            newTable.Cell(1, 4).Range.Text = "记录得分";
            newTable.Cell(1, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;


            newTable.Columns[1].Width = 80f;
            newTable.Columns[2].Width = 220f;
            newTable.Columns[3].Width = 35f;
            newTable.Columns[4].Width = 70f;


            decimal dLose = 0;
            decimal GotScotr = 0;

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    newTable.Cell(i + 2, 1).Range.Text = dt.Rows[i]["NoteName"].ToString();
                    newTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 2).Range.Text = dt.Rows[i]["LoseReason"].ToString();
                    newTable.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 3).Range.Text = dt.Rows[i]["LoseScore"].ToString();
                    if (dt.Rows[i]["LoseScore"].ToString().Trim() != "")
                    {
                        dLose = dLose + Convert.ToDecimal(dt.Rows[i]["LoseScore"].ToString());
                    }
                    newTable.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    newTable.Cell(i + 2, 4).Range.Text = dt.Rows[i]["Score"].ToString();
                    if (dt.Rows[i]["Score"].ToString().Trim() != "")
                    {
                        GotScotr = GotScotr + Convert.ToDecimal(dt.Rows[i]["Score"].ToString());
                    }
                    newTable.Cell(i + 2, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                }
            }

            // */



            object Line1 = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            //object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;//换一行;
            emrtaskpane.wordApp.Selection.MoveDown(ref Line1, ref count, ref unknow);//移动焦点
            emrtaskpane.wordApp.Selection.MoveDown(ref Line1, ref count, ref unknow);//移动焦点

            emrtaskpane.wordApp.Selection.TypeParagraph();//插入段落



            Word.Range range1 = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range;
            range1.Start = ActiveDocumentManager.getDefaultAD().Paragraphs.Last.Range.End;

            range1.Select();
            //range1.Text = "姓名：" + this.valuateText + this.selfValuate + this.scorePercent + "44" + this.panel1 + this.panel2 + "33" + this.lbPatient + "55" + this.lbValuate + "11" + this.count + "66" + this.components + "22" + this.btnReport + "等级：" + "得分：" + this.regID;
            string temp1, temp2, temp3, temp4;
            temp1 = this.lbPatient.ToString();
            temp2 = temp1.Substring(34);
            temp3 = this.lbValuate.ToString();
            temp4 = temp3.Substring(34);


       
            range1.Text = "科室：" + strDep + "  " + "患者姓名：" + temp2 + "  " + "得分：" + GotScotr.ToString() + "  " + "扣分：" + dLose.ToString() + "  " + "等级：" + temp4;

            range1.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            range1.Font.Name = "黑体";
            range1.Font.Size = 10f;
            range1.Start = range.End;




            //Globals.ThisAddIn.HideEmrTaskPane();
            //Globals.ThisAddIn.ShowNoteMenu();
            //Globals.ThisAddIn.noteMenu.OperationEnableForStat(true);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            emrtaskpane.NewWordDoc();           
            
        }
        public void ShowVs()
        {
            string Knock = this.lbKnock.Text;
            string Score = this.lbScore.Text;
            string strDep = patientDepartment;
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                strDep = ep.GetDepartmentName(patientDepartment);
            }
            bool blSelf = false;
            string strRegistryID = regID;
            blSelf = true;
            GetNewReport(GetData(blSelf, strRegistryID), Score, Knock, strDep);

        }

        ////////////////////////////////////////////

        private bool AbsenceNotes(XmlNode rules,string strNoteID)
        {
            TreeView absenceNotes = new TreeView();

            foreach (XmlNode rule in rules.ChildNodes)
            {
                if (rule.Attributes[AttributeNames.Required].Value == StringGeneral.Yes)
                {
                    if (rule.Attributes[AttributeNames.NoteID].Value!=strNoteID) continue;
                    if (rule.Attributes[AttributeNames.Substitute] != null)
                    {
                        //if (!SubstituteAbsence(rule.Attributes[AttributeNames.Substitute].Value, emrdoc)) continue;
                    }
                }
                else
                {
                    if (rule.Attributes[AttributeNames.Condition] == null) continue;
                    //if (!SatisfiedCondition(rule.Attributes[AttributeNames.Condition].Value,
                    //    rule.Attributes[AttributeNames.NoteID].Value, emrdoc)) continue;
                }

                string noteName =
                    Globals.emrPattern.GetNoteNameFromNoteID(rule.Attributes[AttributeNames.NoteID].Value);
                absenceNotes.Nodes.Add(noteName);
            }

            return false;
        }
        public DataTable GetData(bool blSelf, string strRegistryID)
        {

            gjtEmrService.emrServiceXml oService = new gjtEmrService.emrServiceXml();

            DataSet ds = oService.GetValuateDetailDTEx(blSelf, strRegistryID);

            DataTable dtScore = new DataTable("DTScore");
            //decimal sumScore = 0;        


            if (ds != null && ds.Tables.Count != 0)
            {
                dtScore.Columns.Add("NoteID");
                dtScore.Columns.Add("NoteName");
                dtScore.Columns.Add("LoseReason");
                dtScore.Columns.Add("LoseScore");
                dtScore.Columns.Add("Score");
                dtScore.Columns.Add("SumScore");
                dtScore.Columns.Add("LoseSumScore");
                dtScore.Columns.Add("Degree");

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                   
                    

                    DataRow dr = dtScore.NewRow();
                    dr["NoteID"] = ds.Tables[0].Rows[i]["NoteID"];
                    dr["NoteName"] = ds.Tables[0].Rows[i]["NoteName"];
                    dr["LoseReason"] = "";
                    dr["LoseScore"] = "";
                    if (ArrayListNoteID.Count != 0)
                    {
                        for (int k = 0; k < ArrayListNoteID.Count; k++)
                        {
                            if (dr["NoteID"].ToString() == ArrayListNoteID[k].ToString().Trim())
                            {
                                dr["Score"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Score"]) - Convert.ToDecimal(ArrayListOff[k]);
                                break;
                            }
                            else
                            {
                                dr["Score"] = ds.Tables[0].Rows[i]["Score"];
                            }

                        }

                    }
                    else
                    {
                        dr["Score"] = ds.Tables[0].Rows[i]["Score"];
                    }


                    dr["SumScore"] = this.lbScore.Text.Trim();
                    dr["LoseSumScore"] = "";
                    dr["Degree"] = this.lbValuate.Text.Trim();



                    dtScore.Rows.Add(dr);
                    //sumScore = sumScore + Convert.ToDecimal( dr["Score"] = ds.Tables[0].Rows[i]["Score"].ToString().Trim());
                    if (ds.Tables[0].Rows[i]["Flaws"].ToString() != "")
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(ds.Tables[0].Rows[i]["Flaws"].ToString());
                        XmlNode nodeRules = doc.DocumentElement.Clone();
                        XmlNodeList ListNode = nodeRules.ChildNodes;
                        if (ListNode.Count != 0)
                        {
                            foreach (XmlNode node in ListNode)
                            {
                                DataRow dr1 = dtScore.NewRow();
                                dr1["NoteName"] = "";
                                dr1["LoseReason"] = node.InnerText;
                                dr1["LoseScore"] = node.Attributes["Knockoff"].Value.ToString();
                                dr1["Score"] = "";
                                dr1["SumScore"] = this.lbScore.Text.Trim();
                                dr1["LoseSumScore"] = "";
                                dr1["Degree"] = this.lbValuate.Text.Trim();

                                dtScore.Rows.Add(dr1);
                            }
                        }
                    }

                    for (int k = 0; k < ArrayListNoteID.Count; k++)
                    {
                        if (dr["NoteID"].ToString() == ArrayListNoteID[k].ToString().Trim())
                        {
                            DataRow dr1 = dtScore.NewRow();
                            dr1["NoteName"] = "";
                            dr1["LoseReason"] = ArrayListReason[k].ToString();
                            dr1["LoseScore"] = ArrayListOff[k].ToString();
                            dr1["Score"] = "";
                            dr1["SumScore"] = this.lbScore.Text.Trim();
                            dr1["LoseSumScore"] = "";
                            dr1["Degree"] = this.lbValuate.Text.Trim();

                            dtScore.Rows.Add(dr1);
                        }
                    }                 

             
                }

            }



            ///////////////////////////////////////////


            return dtScore;

        }

        private void ValuateScore_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = "";
  
                if (Globals.isEndRule == false)
                {
                    msg = es.NewValuateScore(selfValuate, regID, patientDoctor, patientDepartment,
                    Globals.DoctorID, scorePercent, vi);
                }
                else
                {
                    msg = es.NewValuateScoreEnd(selfValuate, regID, patientDoctor, patientDepartment,
                                      Globals.DoctorID, scorePercent, vi);
                }
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
                using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                {
                    //string strRegistryID = Globals.ThisAddIn
                    for (int i = 0; i < ArrayListNoteID.Count; i++)
                    {
                        ep.InSertValueOff(registryIDTemp, ArrayListNoteID[i].ToString(), ArrayListReason[i].ToString(), ArrayListOff[i].ToString());
                    }
                }
                OpDone opd = new OpDone("保存成功！");                opd.Show();

                this.Tag = StringGeneral.Zero;
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

