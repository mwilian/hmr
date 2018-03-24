using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using UserInterface;
using Word = Microsoft.Office.Interop.Word;
using EmrConstant;
using CommonLib;

namespace EMR
{
    public partial class ValuateNow : Form
    {
        DGVPrinter dgvPrinter;
        private decimal knockoffTotal = 0;
        private decimal mark = 0;
        private string noteID = null;
        private MainForm emrTaskPane = null;
        private string ValuateOpcode = null;
        private string registryID = "";
        private string shbz = "";
        private decimal knockTotal = 0;
        private int vi = 0;    // index of valuate level
        private decimal scorePercent = 0;
        private ValuateText valuateText = new ValuateText();
        private string departID = "";
        private bool Update = false;



        public ValuateNow(MainForm etp)
        {
            emrTaskPane = etp;
            InitializeComponent();

            dgvPrinter = new DGVPrinter();
            dgvPrinter.SourceDGV = dgvValuate;
        }
        public void InitControls(EmrConstant.PatientInfo pi, string depName, string doctorName, DateTime todays)
        {
            if (pi == null) return;
            departID = pi.DepartmentCode;
            //静海修改病案号去掉0000
            string archive = "";
            if (pi.ArchiveNum.Length == 10)
                archive = pi.ArchiveNum.Remove(0, 4);
            else archive = pi.ArchiveNum;

            lblBH.Text = archive;
            lblName.Text = pi.PatientName;
            lblSex.Text = pi.Sex;
            lblZyDate.Text = pi.RegistryDate;
            lblCyDate.Text = pi.DischargedDate;
            //lbRegistryID.Text = pi.RegistryID;
            // lbOriginal.Text = pi.NativePlace;
            //lbBirth.Text = pi.Birth;
            DateTime today = todays;
            int age = today.Year - Convert.ToDateTime(pi.Birth).Year;
            lblAge.Text = age.ToString() + "岁";
            //lbAddr.Text = pi.Address;
            //lbBed.Text = pi.BedNum;
            lblHosName.Text = Globals.hospitalName;
            lblDep.Text = depName;
            lblZz.Text = null;    //主任医师ChiefDoctor
            lblZg.Text = null;    //主管医师
            lblZr.Text = null;    //责任医师
            registryID = pi.RegistryID;


        }
        private void SetYsbm()
        {
            DataSet ds = null;
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                ds = ep.GetValuateNowYsm(lblBH.Text);
                if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                {
                    lblZz.Text = ds.Tables[0].Rows[0][1].ToString();    //主任医师ChiefDoctor
                    lblZg.Text = ds.Tables[0].Rows[0][0].ToString();    //主管医师
                    lblZr.Text = ds.Tables[0].Rows[0][2].ToString();    //责任医师
                }
            }

        }



        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 对第1列相同单元格进行合并
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                using
                    (
                    Brush gridBrush = new SolidBrush(this.dgvValuate.GridColor),
                    backColorBrush = new SolidBrush(e.CellStyle.BackColor)
                    )
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        // 清除单元格
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                        // 画 Grid 边线（仅画单元格的底边线和右边线）
                        //   如果下一行和当前行的数据不同，则在当前的单元格画一条底边线
                        if (e.RowIndex < dgvValuate.Rows.Count - 1 &&
                        dgvValuate.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value.ToString() != e.Value.ToString())
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                            e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                            e.CellBounds.Bottom - 1);
                        // 画右边线
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                            e.CellBounds.Top, e.CellBounds.Right - 1,
                            e.CellBounds.Bottom);

                        // 画（填写）单元格内容，相同的内容的单元格只填写第一个
                        if (e.Value != null)
                        {
                            if (e.RowIndex > 0 && dgvValuate.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value.ToString() == e.Value.ToString())
                            { }
                            else
                            {
                                e.Graphics.DrawString((String)e.Value, e.CellStyle.Font,
                                    Brushes.Black, e.CellBounds.X + 2,
                                    e.CellBounds.Y + 5, StringFormat.GenericDefault);
                            }
                        }
                        e.Handled = true;
                    }
                }

            }

            // 对第2列相同单元格进行合并
            if (e.ColumnIndex == 1 && e.RowIndex != -1)
            {
                using
                    (
                    Brush gridBrush = new SolidBrush(this.dgvValuate.GridColor),
                    backColorBrush = new SolidBrush(e.CellStyle.BackColor)
                    )
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        // 清除单元格
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                        // 画 Grid 边线（仅画单元格的底边线和右边线）
                        //   如果下一行和当前行的数据不同，则在当前的单元格画一条底边线
                        if (e.RowIndex < dgvValuate.Rows.Count - 1 &&
                        dgvValuate.Rows[e.RowIndex + 1].Cells[0].Value.ToString() != dgvValuate.Rows[e.RowIndex].Cells[0].Value.ToString())
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                            e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                            e.CellBounds.Bottom - 1);
                        // 画右边线
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                            e.CellBounds.Top, e.CellBounds.Right - 1,
                            e.CellBounds.Bottom);

                        // 画（填写）单元格内容，相同的内容的单元格只填写第一个
                        if (e.Value != null)
                        {
                            if (e.RowIndex > 0 && dgvValuate.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value.ToString() == e.Value.ToString())
                            { }
                            else
                            {
                                e.Graphics.DrawString((String)e.Value, e.CellStyle.Font,
                                    Brushes.Black, e.CellBounds.X + 2,
                                    e.CellBounds.Y + 5, StringFormat.GenericDefault);
                            }
                        }
                        e.Handled = true;
                    }
                }

            }
        }

        private void btnUpate_Click(object sender, EventArgs e)
        {
            this.knockTotal = 0;
            this.dgvValuate.Enabled = true;
            this.btnUpate.Enabled = false;
            this.btnPS.Enabled = true;
            this.btnChecked.Enabled = false;
            Update = true;

        }

        private void btnChecked_Click(object sender, EventArgs e)
        {
            if (lblCyDate.Text.Trim() == "")
            {
                MessageBox.Show("请确认该患者已经出院再审核评分！！！", ErrorMessage.Warning,
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show("确认审核吗！审核后无法再次修改评分定级！！！", ErrorMessage.Warning,
               MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
               == DialogResult.Cancel) return;

            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {

                try
                {
                    //
                    XmlDocument doc = new XmlDocument();
                    XmlElement note = doc.CreateElement(ElementNames.EmrNote);
                    // note.SetAttribute(AttributeNames.RegistryID, registryID);
                    note.SetAttribute(AttributeNames.ArchiveNum, lblBH.Text);
                    note.SetAttribute(AttributeNames.PSCYRQ, lblCyDate.Text);
                    // note.SetAttribute(AttributeNames.SHRQ, DateTime.Now.ToString());
                    // note.SetAttribute(AttributeNames.DepartmentCode, departID);
                    note.SetAttribute(AttributeNames.PSZJYS, Globals.DoctorName);
                    note.SetAttribute(AttributeNames.BAPSBZ, "1");
                    note.SetAttribute(AttributeNames.PSRQ, DateTime.Now.ToString());
               
                    //note.SetAttribute(AttributeNames.SHBZ, shbz);
                    //note.SetAttribute(AttributeNames.SHRQ, "");
                    //note.SetAttribute(AttributeNames.SHRY, "");
                    note.SetAttribute(AttributeNames.RegistryID, registryID);
                    //if (noteID != "00") note.SetAttribute(AttributeNames.Series, series);

                    foreach (DataGridViewRow row in dgvValuate.Rows)
                    {
                        if (row.Cells[4].Value == null || row.Cells[4].Value.ToString() == "" || row.Cells[4].Value.ToString() == "------------") continue;

                        this.knockTotal += Convert.ToDecimal(row.Cells[4].Value);
                        XmlElement flaw = doc.CreateElement(ElementNames.Flaw);
                        flaw.SetAttribute(AttributeNames.KFXM, row.Cells[0].Value.ToString());
                        string strqxmc = row.Cells[3].Value.ToString();
                        string[] mcs = strqxmc.Split('-');
                        flaw.SetAttribute(AttributeNames.KFYY, mcs[0].ToString());
                        flaw.SetAttribute(AttributeNames.KF, mcs[1].ToString());
                        flaw.SetAttribute(AttributeNames.GLMC, row.Cells[5].Value.ToString());
                        note.AppendChild(flaw);
                    }
                    // MakeScore(this.knockTotal);
                    note.SetAttribute(AttributeNames.PJ, lblPJ.Text);
                    note.SetAttribute(AttributeNames.SDF, lblZDF.Text);
                    decimal knoc = 0;
                    if (lblZDF.Text != "")
                        knoc = (100 - Convert.ToDecimal(lblZDF.Text));
                    note.SetAttribute(AttributeNames.GKF, knoc.ToString());
                    bool msg = false;
                    msg = ep.ValuateNowSH(registryID, Globals.DoctorName, DateTime.Now.ToString(), note);
                    if (msg)
                    {
                        btnChecked.Enabled = false;
                        btnPS.Enabled = false;
                        btnUpate.Enabled = false;
                        btnPrintPreview.Enabled = true;
                        btnValuatPrint.Enabled = true;
                    }
                }
                catch
                {
                }
            }
        }


        private void ValuateNow_Load(object sender, EventArgs e)
        {
            SetYsbm();
            DataTable dt1 = new DataTable();
            XmlNode testsAndExams = null;
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                testsAndExams = ep.ValuateNowEx();
            }
            XmlNodeList archives = testsAndExams.SelectNodes(EmrConstant.ElementNames.archive);
            DataGridViewComboBoxColumn dgvComboBoxColumn = new DataGridViewComboBoxColumn();
            dgvComboBoxColumn.HeaderText = "科级扣分原因";

            DataGridViewTextBoxColumn KF = new DataGridViewTextBoxColumn();
            KF.HeaderText = "科级扣分";
            DataGridViewTextBoxColumn MC = new DataGridViewTextBoxColumn();
            MC.HeaderText = "项目内容";

            DataGridViewTextBoxColumn SZF = new DataGridViewTextBoxColumn();
            SZF.HeaderText = "所占分";
            DataGridViewTextBoxColumn Content = new DataGridViewTextBoxColumn();
            Content.HeaderText = "检查内容与评分标准";
            DataGridViewTextBoxColumn GLMC = new DataGridViewTextBoxColumn();
            Content.HeaderText = "评分标准";
            //   DataGridViewTextBoxColumn YGKF = new DataGridViewTextBoxColumn();
            //YGKF.HeaderText = "缺陷应该扣分";
            //   DataGridViewTextBoxColumn XH = new DataGridViewTextBoxColumn();
            //XH.HeaderText = "缺陷序号";
            dgvValuate.Columns.Add(MC);

            dgvValuate.Columns.Add(SZF);
            dgvValuate.Columns.Add(Content);
            dgvValuate.Columns.Add(dgvComboBoxColumn);
            dgvValuate.Columns.Add(KF);
            dgvValuate.Columns.Add(GLMC);
            GLMC.Visible = false;

            XmlNodeList items = null;
            foreach (XmlNode archive in archives)
            {

                string ProName = archive.Attributes[EmrConstant.AttributeNames.ProName].Value;
                string SZFv = archive.Attributes[EmrConstant.AttributeNames.SZF].Value;
                string SZFHJ = archive.Attributes[EmrConstant.AttributeNames.SZFHJ].Value;
                string Contentv = archive.Attributes[EmrConstant.AttributeNames.Content].Value;
                string glmc = archive.Attributes[EmrConstant.AttributeNames.GLMC].Value;
                int index = dgvValuate.Rows.Add();

                items = archive.ChildNodes;
                DataGridViewComboBoxCell dcc = null;
                //if (ProName == "") ProName = "首页病程";
                dgvValuate.Rows[index].Cells[0].Value = ProName;
                dgvValuate.Rows[index].Cells[1].Value = SZFHJ;
                dgvValuate.Rows[index].Cells[2].Value = Contentv;
                dgvValuate.Rows[index].Cells[5].Value = glmc;
                dcc = (DataGridViewComboBoxCell)dgvValuate.Rows[index].Cells[3];


                foreach (XmlNode item in items)
                {
                    // string XH = item.Attributes[EmrConstant.AttributeNames.XH].Value;
                    string KFYY = item.Attributes[EmrConstant.AttributeNames.KFYY].Value;
                    string CKKF = item.Attributes[EmrConstant.AttributeNames.CKKF].Value;
                    int dex = dcc.Items.Add(KFYY + "\n -" + CKKF);

                }
                dcc.Items.Add("------------");
            }
            dgvValuate.Columns[4].ReadOnly = true;
            //判断是否已经审核了
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                string flag = ep.ValuateNowISSH(registryID);
                DataTable dt = ep.ValuateNowBAPSBZNew(registryID);
                string oldStr = "";
                string newStr = "";

                if (dt.Rows.Count != 0 && dt.Rows[0][0].ToString() == "1")
                {
                    btnUpate.Enabled = false;
                    btnPS.Enabled = false;
                    btnChecked.Enabled = false;
                    btnPrintPreview.Enabled = true;
                    btnValuatPrint.Enabled = true;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgvValuate.Rows.Count; j++)
                        {
                            if (dgvValuate.Rows[j].Cells[5].Value.ToString() == dt.Rows[i][2].ToString())
                            {
                                DataGridViewComboBoxCell dcc = (DataGridViewComboBoxCell)dgvValuate.Rows[j].Cells[3];
                                for (int k = 0; k < dcc.Items.Count; k++)
                                {
                                    oldStr = dcc.Items[k].ToString().Replace(" ","");
                                    newStr = dt.Rows[i][1].ToString().Trim() + "\n-" + dt.Rows[i][5].ToString().Trim();
                                    if (oldStr==newStr)
                                    {
                                        dcc.Value = dcc.Items[k].ToString();

                                        // cb_SelectedIndexChanged(sender, e);
                                    }
                                }
                                string[] strdcc = dcc.Value.ToString().Split('-');
                                dgvValuate.Rows[j].Cells[4].Value = strdcc[1].ToString();
                            }
                        }
                    }
                    lblZDF.Text = dt.Rows[0][4].ToString();
                    lblPJ.Text = dt.Rows[0][3].ToString();
                }
                else
                {
                    btnUpate.Enabled = false;
                    btnPS.Enabled = true;
                    btnChecked.Enabled = false;
                    btnPrintPreview.Enabled = false;
                    btnValuatPrint.Enabled = false;
                    return;
                }

                if (flag == "1")
                {
                    btnUpate.Enabled = false;
                    btnChecked.Enabled = false;
                    btnPS.Enabled = false;
                    btnPrintPreview.Enabled = true;
                    btnValuatPrint.Enabled = true;
                    return;
                }
            }
            btnUpate.Enabled = true;
            btnChecked.Enabled = true;
            btnPrintPreview.Enabled = false;
            btnValuatPrint.Enabled = false;
        }

        private void btnPS_Click(object sender, EventArgs e)
        {
            this.knockTotal = 0;
            if ((ValuateOpcode != Globals.DoctorName) && (ValuateOpcode != null))
            {
                MessageBox.Show("不能给别人评过分的病历评分！", ErrorMessage.Warning);
                return;
            }
            if (MessageBox.Show("确认保存评分结果！", ErrorMessage.Warning,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                == DialogResult.Cancel) return;

            XmlDocument doc = new XmlDocument();
            XmlElement note = doc.CreateElement(ElementNames.EmrNote);
            // note.SetAttribute(AttributeNames.RegistryID, registryID);
            note.SetAttribute(AttributeNames.ArchiveNum, lblBH.Text);
            note.SetAttribute(AttributeNames.PSCYRQ, lblCyDate.Text);
            note.SetAttribute(AttributeNames.PSRQ, DateTime.Now.ToString());
            // note.SetAttribute(AttributeNames.DepartmentCode, departID);
            note.SetAttribute(AttributeNames.PSZJYS, Globals.DoctorName);
            note.SetAttribute(AttributeNames.BAPSBZ, "1");
            note.SetAttribute(AttributeNames.RegistryID, registryID);
            //note.SetAttribute(AttributeNames.SHBZ, shbz);
            //note.SetAttribute(AttributeNames.SHRQ, "");
            //note.SetAttribute(AttributeNames.SHRY, "");

            //if (noteID != "00") note.SetAttribute(AttributeNames.Series, series);

            foreach (DataGridViewRow row in dgvValuate.Rows)
            {
                if (row.Cells[4].Value == null || row.Cells[4].Value.ToString() == "" || row.Cells[4].Value.ToString() == "------------") continue;

                this.knockTotal += Convert.ToDecimal(row.Cells[4].Value);
                XmlElement flaw = doc.CreateElement(ElementNames.Flaw);
                flaw.SetAttribute(AttributeNames.KFXM, row.Cells[0].Value.ToString());
                string strqxmc = row.Cells[3].Value.ToString();
                string[] mcs = strqxmc.Split('-');
                flaw.SetAttribute(AttributeNames.KFYY, mcs[0].ToString());
                flaw.SetAttribute(AttributeNames.KF, mcs[1].ToString());
                flaw.SetAttribute(AttributeNames.GLMC, row.Cells[5].Value.ToString());

                note.AppendChild(flaw);
            }
            MakeScore(this.knockTotal);
            note.SetAttribute(AttributeNames.PJ, lblPJ.Text);
            note.SetAttribute(AttributeNames.SDF, lblZDF.Text);
            note.SetAttribute(AttributeNames.GKF, knockTotal.ToString());
            // note.SetAttribute(AttributeNames.KFXM, lblPJ.Text);
            bool msg = false;
            int counts = 0;
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {

                try
                {

                    if (Update)
                    {
                        //counts = ep.DelValuateNow(registryID);
                        //if (counts != 0)
                        //{

                        //    msg = ep.ValuateNowPS(note);
                        //    if (msg)
                        //    {
                        //        btnPS.Enabled = false;
                        //        btnUpate.Enabled = true;
                        //        btnChecked.Enabled = true;
                        //        Update = false;
                        //    }
                        //    else
                        //    {
                        //        MessageBox.Show("插入更新修改失败！！");
                        //    }
                        //}
                        //else
                        //{
                        //    MessageBox.Show("删除更新修改失败！");
                        //}
                        msg = ep.DeleteAndInsertValue(note);
                        if (msg)
                        {
                            btnPS.Enabled = false;
                            btnUpate.Enabled = true;
                            btnChecked.Enabled = true;
                            Update = false;
                        }
                        else
                        {
                            MessageBox.Show("修改评分失败！！");
                        }
                    }
                    else
                    {

                        msg = ep.ValuateNowPS(note);
                        if (msg)
                        {
                            btnPS.Enabled = false;
                            btnUpate.Enabled = true;
                            btnChecked.Enabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741852969", ex.Message + ">>" + ex.ToString(), true);
                }
            }
        }
        private void MakeScore(decimal knockTotal)
        {

            decimal OtherScoreOff = 100 - knockTotal;
            lblZDF.Text = OtherScoreOff.ToString();
            //lbKnock.Text += knockTotal.ToString();         
            lblPJ.Text = ValuateLevel(OtherScoreOff, knockTotal);
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
        ComboBox cb = null;
        private void dgvValuate_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            cb = e.Control as ComboBox;

            if (cb != null)
            {

                // 首先移除事件处理程序以防止多重触发附加事件

                cb.SelectedIndexChanged -= new

                EventHandler(cb_SelectedIndexChanged);


                // 附加事件处理程序

                cb.SelectedIndexChanged += new

                EventHandler(cb_SelectedIndexChanged);

            }


        }
        void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb.SelectedItem != null)
            {
                string value = cb.SelectedItem.ToString();
                string[] str = value.Split('-');
                dgvValuate.CurrentRow.Cells[4].Value = str[1].ToString();
            }

        }

        private void btnExit__Click(object sender, EventArgs e)
        {
            string v = dgvValuate.Rows[1].Cells[1].Value.ToString();
            this.Close();
        }


        //打印初检评分表
        private void btnValuatPrint_Click(object sender, EventArgs e)
        {
            dgvPrinter.mainTitle = "病历质量检查评分标准";
            dgvPrinter.subTitle = "医院名称： " + Globals.hospitalName + "  病案号：" + lblBH.Text + "  姓名： " + lblName.Text + "  性别 " + lblSex.Text + "  年龄 " + lblAge.Text + "  \r\n住院日期：" + lblZyDate.Text + " 出院日期：" + lblCyDate.Text + " 科室： " + lblDep.Text + "  \r\n主管医师：" + lblZg.Text + "     " + "  主治医师：" + lblZz.Text + "     " + " 主任医师 " + lblZr.Text;

            //int zkf = 0;
            Double zkf = 0.0f;
            //if (lblZDF.Text != "") zkf = 100 - Convert.ToInt32(lblZDF.Text);
            if (lblZDF.Text != "") zkf = 100.00f - Convert.ToDouble(lblZDF.Text.ToString());
            dgvPrinter.subTitle2 = "合计：100分 科级扣分：" + zkf.ToString() + "分  科级实得分：" + lblZDF.Text + "分 病历级别： " + lblPJ.Text + "  科级评审人：" + Globals.DoctorName + "  \r\n \r\n" + labbz1.Text + "\r\n" + labbz2.Text;

            dgvPrinter.PrintDataGridView(dgvValuate);
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            dgvPrinter.mainTitle = "病历质量检查评分标准";
            dgvPrinter.subTitle = "医院名称： " + Globals.hospitalName + "  病案号：" + lblBH.Text + "  姓名： " + lblName.Text + "  性别 " + lblSex.Text + "  年龄 " + lblAge.Text + "  \r\n住院日期：" + lblZyDate.Text + " 出院日期：" + lblCyDate.Text + " 科室： " + lblDep.Text + "  \r\n主管医师：" + lblZg.Text + "     " + "  主治医师：" + lblZz.Text + "     " + " 主任医师 " + lblZr.Text;
            Double zkf = 0.0f;
            if (lblZDF.Text != "") zkf = 100.00f - Convert.ToDouble(lblZDF.Text.ToString());
            dgvPrinter.subTitle2 = "合计：100分 科级扣分：" + zkf.ToString() + "分  科级实得分：" + lblZDF.Text + "分 病历级别： " + lblPJ.Text + "  科级评审人：" + Globals.DoctorName + "  \r\n \r\n" + labbz1.Text + "\r\n" + labbz2.Text;

            dgvPrinter.PrintPreview();
        }

    }
}

