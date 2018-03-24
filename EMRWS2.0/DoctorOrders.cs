using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Word = Microsoft.Office.Interop.Word;
using EmrConstant;
using HuronControl;
using CommonLib;

namespace EMR
{
    public partial class DoctorOrders : Form
    {
        private string copyText="";
        public Boolean existError;
        private XmlNode testsAndExams = null;
        private XmlNode orders = null;
        private Point dgvOrderLocation = new Point(0, 0);
        private Size dgvOrderSize = new Size(0, 0);
        Word.Application WordApp;
  
        public DoctorOrders(string registryID, Word.Application WordApplication, string status)
        {
            InitializeComponent();
            WordApp = WordApplication;
            existError = false;
            dgvOrderLocation = dgvOrder.Location;
            dgvOrderSize.Width = dgvOrder.Width;
            dgvOrderSize.Height = dgvOrder.Height;
            dgvOrder.Tag = StringGeneral.Zero;
            //string status = Globals.ThisAddIn.emrTaskPane.GetTreeviewPatientsStatus();

           
            #region Get tests, examine, drug, treat
            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
            {
                try
                {
                    testsAndExams = pi.GetTestsAndExams(registryID, true);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX741852965", ex.Message + ">>" + ex.ToString(), true);       
                 
                    existError = true;
                    return;
                }
                if (testsAndExams.HasChildNodes)
                {
                    if (testsAndExams.FirstChild.Name == ErrorMessage.XmlErr)
                    {
                        string msg = testsAndExams.FirstChild.Attributes[AttributeNames.Message].Value;
                        MessageBox.Show(msg, ErrorMessage.Warning);
                        existError = true;
                        return;
                    }
                }

                try
                {
                    if (status == InpatientStatus.Stay)
                        orders = pi.GetDoctorOrders(registryID, true);
                    else
                        orders = pi.GetDoctorOrdersDischarged(registryID);
                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX741852966", ex.Message + ">>" + ex.ToString(), true);       
                 
                    existError = true;
                    return;
                }
                if (orders.HasChildNodes)
                {
                    if (orders.FirstChild.Name == EmrConstant.ErrorMessage.XmlErr)
                    {
                        string msg = orders.FirstChild.Attributes[AttributeNames.Message].Value;
                        MessageBox.Show(msg, ErrorMessage.Warning);
                        existError = true;
                        return;
                    }
                }
            }
            #endregion

            if (!testsAndExams.HasChildNodes && !orders.HasChildNodes)
            {
                MessageBox.Show(ErrorMessage.NoOrderRecord, ErrorMessage.Warning);
                existError = true;
                return;
            }

            /* display laboratory tests and examinations*/
            if (testsAndExams != null)
            {
                LoadTestsAndExams();
                if (lbTest.Items.Count > 0) lbTest.SelectedIndex = 0;
                if (lbExam.Items.Count > 0) lbExam.SelectedIndex = 0;
            }
            /* load dgvOrder control */
            if (orders != null) LoadOrders();

        }
        /* -------------------------------------------------------------------------------------------
         * fill lbTest and lbExam control with Xmlnode testAndExam attributes 
         ---------------------------------------------------------------------------------------------*/
        private void LoadTestsAndExams()
        {
            XmlNodeList forms = testsAndExams.SelectNodes(EmrConstant.ElementNames.Form);
            foreach (XmlNode form in forms)
            {
                string name = form.Attributes[EmrConstant.AttributeNames.Name].Value;
                string doctor = form.Attributes[EmrConstant.AttributeNames.Doctor].Value;
                string date = form.Attributes[EmrConstant.AttributeNames.Date].Value;
                string united = name + "(医师:" + doctor + " 时间:" + date + ")";

                /* type="test" --> lbTest, type="exam" --> lbExam */
                switch (form.Attributes[EmrConstant.AttributeNames.Type].Value)
                {
                    case EmrConstant.StringGeneral.Test:
                        lbTest.Items.Add(united);
                        break;
                    case EmrConstant.StringGeneral.Exam:
                        lbExam.Items.Add(united);
                        break;
                }
            }
        }
        /*--------------------------------------------------------------------------------------------
         * fill dgvOrder control with Xmlnode orders attributes
         ---------------------------------------------------------------------------------------------*/
        private void LoadOrders()
        {
            //XmlNodeList longOrTempOrders = orders.ChildNodes;
            foreach (XmlNode order in orders.ChildNodes)
            {
                XmlNodeList items = order.SelectNodes(EmrConstant.ElementNames.Item);
                if (items.Count <= 0) continue;
                string doctor = order.Attributes[EmrConstant.AttributeNames.Doctor].Value;
                string date = order.Attributes[EmrConstant.AttributeNames.Date].Value;
                string style = "临时";
                if (order.Attributes[AttributeNames.Style].Value == ElementNames.LongOrder) 
                {
                    style = "长期";

                    foreach (XmlNode item in items)
                    {
                        string name = item.Attributes[EmrConstant.AttributeNames.Name].Value;
                        string quantity = item.Attributes[EmrConstant.AttributeNames.Quantity].Value;
                        string unit = item.Attributes[EmrConstant.AttributeNames.Unit].Value;
                        string howUse = item.Attributes[EmrConstant.AttributeNames.HowUse].Value;
                        string howOften = item.Attributes[EmrConstant.AttributeNames.HowOften].Value;
                        string stopDate = item.Attributes[EmrConstant.AttributeNames.StopDate].Value;
                        string howLong = item.Attributes[EmrConstant.AttributeNames.HowLong].Value;
                        date = item.Attributes[EmrConstant.AttributeNames.Start].Value;
                        if (order.Attributes[AttributeNames.Style].Value == ElementNames.TempOrder) howLong = null;
                        int index = dgvOrder.Rows.Add(name, quantity, unit, howOften, howUse, date, stopDate, howLong, doctor);
                        dgvOrder.Rows[index].HeaderCell.Value = style;
                        if (order.Attributes[AttributeNames.Style].Value == ElementNames.TempOrder)
                            dgvOrder.Rows[index].DefaultCellStyle.ForeColor = Color.Blue;
                    }
                }
                else
                {
                    style = "临时";

                    foreach (XmlNode item in items)
                    {
                        string name1 = item.Attributes[EmrConstant.AttributeNames.Name].Value;
                        string quantity1 = item.Attributes[EmrConstant.AttributeNames.Quantity].Value;
                        string unit1= item.Attributes[EmrConstant.AttributeNames.Unit].Value;
                        string howUse1 = item.Attributes[EmrConstant.AttributeNames.HowUse].Value;
                        string howOften1 = item.Attributes[EmrConstant.AttributeNames.HowOften].Value;
                        string stopDate1 = item.Attributes[EmrConstant.AttributeNames.StopDate].Value;
                        string howLong1 = item.Attributes[EmrConstant.AttributeNames.HowLong].Value;
                        string date1 = item.Attributes[EmrConstant.AttributeNames.Start].Value.ToString();
                        if (order.Attributes[AttributeNames.Style].Value == ElementNames.TempOrder) howLong = null;
                        int index = dgvOrder1.Rows.Add(name1, quantity1, unit1, howOften1, howUse1, date1, stopDate1, howLong1, doctor);
                        dgvOrder1.Rows[index].HeaderCell.Value = style;
                        //if (order.Attributes[AttributeNames.Style].Value == ElementNames.TempOrder)
                        //    dgvOrder.Rows[index].DefaultCellStyle.ForeColor = Color.Blue;
                    }
                }
            }
        }
        private void lbTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvTest.Rows.Clear();
            XmlNode form = FindFormNote(EmrConstant.StringGeneral.Test, lbTest.SelectedItem.ToString());
            if (form == null) return;

            XmlNodeList items = form.ChildNodes;
            foreach (XmlNode item in items)
            {
                string name = item.Attributes[EmrConstant.AttributeNames.Name].Value;
                string value = item.Attributes[EmrConstant.AttributeNames.Value].Value;
                string unit = item.Attributes[EmrConstant.AttributeNames.Unit].Value;
                string valueunit = item.Attributes[EmrConstant.AttributeNames.ValueUnit].Value;
                string result = item.Attributes[EmrConstant.AttributeNames.Result].Value;
                dgvTest.Rows.Add(name, value, unit,valueunit, result);
            }
            SelectAllTestItems();

           ThisAddIn.ResetBeginTime();
        }
        private void lbExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* display examination reports without images */
            rtbExam.Clear();
            XmlNode form = FindFormNote(EmrConstant.StringGeneral.Exam, lbExam.SelectedItem.ToString());
            if (form == null) return;

            XmlNodeList items = form.ChildNodes;
            foreach (XmlNode item in items)
            {
                rtbExam.Text += item.Attributes[EmrConstant.AttributeNames.Name].Value + ":" + item.InnerText;
                rtbExam.Text += "\n";
            }
           ThisAddIn.ResetBeginTime();
        }
        /*---------------------------------------------------------------------------------------------------
         * get the exact node in Xmlnode testAndExam 
         * parameters: string type -- "test" or "exam"
         *             string text --  text of selected item in lbTest or lbExam
         ----------------------------------------------------------------------------------------------------*/
        private XmlNode FindFormNote(string type, string text)
        {
            XmlNodeList forms = testsAndExams.SelectNodes(EmrConstant.ElementNames.Form);
            if (forms.Count <= 0) return null;
            foreach (XmlNode form in forms)
            {
                string name = form.Attributes[EmrConstant.AttributeNames.Name].Value;
                string doctor = form.Attributes[EmrConstant.AttributeNames.Doctor].Value; 
                string date = form.Attributes[EmrConstant.AttributeNames.Date].Value;
                string united = name + "(医师:" + doctor + " 时间:" + date + ")";
                if (text == united && type == form.Attributes[EmrConstant.AttributeNames.Type].Value)
                    return form;
            }
            return null;
        }
        /*----------------------------------------------------------------------------------------------------
         * copy test result onto the word document 
         ------------------------------------------------------------------------------------------------------*/
        private void btCopyTest_Click(object sender, EventArgs e)
        {
           
            copyText = "\r" + lbTest.SelectedItem.ToString() + "结果:\r";

            for (int i = dgvTest.SelectedRows.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < dgvTest.ColumnCount; j++)
                {
                    //copyText += " ";
                    copyText += "  " + dgvTest.SelectedRows[i].Cells[j].Value.ToString();
                }
                copyText += "\r";
            }

            //WordApp.Selection.InsertAfter(copyText);
            ///* close current form */
            //this.Close();
            //FocusOnEnd();
            this.DialogResult = DialogResult.OK;
           ThisAddIn.ResetBeginTime();
        }
        /*----------------------------------------------------------------------------------------------------
         * copy examination reports onto the word document 
         ------------------------------------------------------------------------------------------------------*/
        private void btCopyExam_Click(object sender, EventArgs e)
        {
            copyText = rtbExam.Text.Replace("\n", " ");
            this.DialogResult = DialogResult.OK;
            //WordApp.Selection.InsertAfter(text);
            ///* close current form */
            //this.Close();
            //FocusOnEnd();
            ThisAddIn.ResetBeginTime();
        }
        /*----------------------------------------------------------------------------------------------------
         * copy doctor's order onto the word document 
         ------------------------------------------------------------------------------------------------------*/
        private void btCopyOrder_Click(object sender, EventArgs e)
        {
            char padChar = ' ';
           
            for (int i = dgvOrder.SelectedRows.Count - 1; i >= 0; i--)
            {
                string name = dgvOrder.SelectedRows[i].Cells[0].Value.ToString();
                int length = TextLenght(name);
                int spaces = 20 - length;
                copyText += name.PadRight(spaces + spaces + length, padChar);
                string quantity = dgvOrder.SelectedRows[i].Cells[1].Value.ToString()
                    + dgvOrder.SelectedRows[i].Cells[2].Value.ToString();
                copyText += quantity.PadRight(8);
                copyText += dgvOrder.SelectedRows[i].Cells[3].Value.ToString().PadRight(12);
                copyText += dgvOrder.SelectedRows[i].Cells[4].Value.ToString();
                copyText += "\r";
            }
            //try
            //{
                
            //    WordApp.Selection.InsertAfter(copyText);
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(error.Message, error.Source);
            //}
            ///* close current form */
            //this.Close();
            //FocusOnEnd();
            this.DialogResult = DialogResult.OK;
           ThisAddIn.ResetBeginTime();
        }
        private int TextLenght(string text)
        {
            byte[] btext = System.Text.Encoding.Default.GetBytes(text);
            int length = btext.Length / 2;
            if ( (length + length) < btext.Length)
            {
                length++;
                text += " ";
            }
            return length;
        }
        private int PadLength(int j)
        {
            switch (j)
            {
                case 2:
                    return 30;
                case 3:
                    return 10;
                case 4:
                    return 6;
                case 5:
                    return 6;
                case 6:
                    return 4;
                default:
                    return 10;
            }
        }
        private void FocusOnEnd()
        {
            Word.Range range = WordApp.Selection.Range;
            int start = range.End;
            range.Start = start;
            range.End = start;
            range.Select();
        }
        public void SelectAllTestItems()
        {
            dgvTest.SelectAll();
        }
        public void SelectAllOrderItems()
        {
            dgvOrder.SelectAll();
        }

        private void DoctorOrders_SizeChanged(object sender, EventArgs e)
        {
            //gbxTest.Width = this.Width - 20;
            //gbxExam.Width = gbxTest.Width;

            //gbxDrug.Width = gbxTest.Width;
            //gbxDrug.Height = this.Height - 40 - gbxDrug.Top;
        }

        private void gbxDrug_SizeChanged(object sender, EventArgs e)
        {
             
            //dgvOrder.Width = gbxDrug.Width - btCopyOrder.Width - 20 ;
            //dgvTest.Width = gbxDrug.Width - btCopyOrder.Width - dgvTest.Left - 20;
            //rtbExam.Width = gbxDrug.Width - btCopyOrder.Width - rtbExam.Left - 10;

            //btCopyOrder.Left = dgvOrder.Left + dgvOrder.Width;
            //btCopyExam.Left = btCopyOrder.Left;
            //btCopyTest.Left = btCopyOrder.Left;

            //dgvOrder.Height = gbxDrug.Height - 20;
        }

        private void dgvOrder_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) return;
            if (dgvOrder.Tag.ToString() == StringGeneral.Zero)
            {
                dgvOrder.Tag = StringGeneral.One;
                dgvOrder.BringToFront();
                dgvOrder.Location = new Point(0, 0);
                dgvOrder.Width = this.Width;
                dgvOrder.Height = this.Height;
            }
            else
            {
                dgvOrder.Location = dgvOrderLocation;
                dgvOrder.Width = dgvOrderSize.Width;
                dgvOrder.Height = dgvOrderSize.Height;
                dgvOrder.Tag = StringGeneral.Zero;
            }
           ThisAddIn.ResetBeginTime();
        }


        public string GetCopyText()
        {
            return copyText;
        }
        private void btnCopyOrder1_Click(object sender, EventArgs e)
        {
            char padChar = ' ';
            //string copyText = "";
            for (int i = dgvOrder1.SelectedRows.Count - 1; i >= 0; i--)
            {
                string name = dgvOrder1.SelectedRows[i].Cells[0].Value.ToString();
                int length = TextLenght(name);
                int spaces = 20 - length;
                copyText += name.PadRight(spaces + spaces + length, padChar);
                string quantity = dgvOrder1.SelectedRows[i].Cells[1].Value.ToString()
                    + dgvOrder1.SelectedRows[i].Cells[2].Value.ToString();
                copyText += quantity.PadRight(8);
                copyText += dgvOrder1.SelectedRows[i].Cells[3].Value.ToString().PadRight(12);
                copyText += dgvOrder1.SelectedRows[i].Cells[4].Value.ToString();
                copyText += "\r";
            }
            //try
            //{
            //    WordApp.Selection.InsertAfter(copyText);
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(error.Message, error.Source);
            //}
            ///* close current form */
            //this.Close();
            //FocusOnEnd();
            this.DialogResult = DialogResult.OK;
           ThisAddIn.ResetBeginTime();
        }


    }
}