using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;
using CommonLib;

namespace EMR
{
    public partial class FilingUC : UserControl
    {
        public bool isEndRole = false;
        int delta = 1;

        public FilingUC(string ID, string noteID, string noteName)
        {
            InitializeComponent();

            lbNote.Text = noteName;
            lbNote.Name = noteID;
            lbNote.Tag = ID;
        }
        public string GetNoteID()
        {
            return lbNote.Name;
        }
     
        private void FilingUC_MouseEnter(object sender, EventArgs e)
        {
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        private void FilingUC_MouseLeave(object sender, EventArgs e)
        {
            this.BorderStyle = BorderStyle.None;
        }

        private void lbNote_MouseClick(object sender, MouseEventArgs e)
        {

            XmlNode rules = null;


            #region Get rules for this note
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                 string msg  = "";
                try
                {
                    if (Globals.isEndRule == false)
                    {
                        msg = es.GetValuateRules(lbNote.Name, ref rules);
                    }
                    else
                    {
                         msg = es.GetValuateRulesEnd(lbNote.Name, ref rules);
                    }
                    if (msg != null)
                    {
                        MessageBox.Show(msg, ErrorMessage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925521256895", ex.Message + ">>" + ex.ToString(), true);            
               
                    return;
                }
            }
            #endregion
            #region Show rules
            if (rules != null)
            {
                foreach (XmlNode rule in rules.ChildNodes)
                {
                }

            }
            #endregion

            VisbleOrNot(true);

            #region Remove a rule
            if (e.Button == MouseButtons.Right)
            {
                if (MessageBox.Show("确认删除此归档项目？", ErrorMessage.Warning,
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                   == DialogResult.No) return;
                #region Remove note from database
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {
                    try
                    {
                        string msg = "";
                        if (Globals.isEndRule == false)
                        {
                            //string RegistryID = Globals.ThisAddIn.emrTaskPane.GetRegistryID();
                            string RegistryID = lbNote.Tag.ToString();
                            msg = es.RemoveNoteWithFilingSetup(RegistryID, lbNote.Name);
                        }
                        else
                        {
                            //string RegistryID = Globals.ThisAddIn.emrTaskPane.GetRegistryID();
                            string RegistryID = lbNote.Tag.ToString();
                            msg = es.RemoveNoteWithFilingSetup(RegistryID, lbNote.Name);
                        }
                        if (msg != null)
                        {
                            MessageBox.Show(msg, ErrorMessage.Error);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX925512256895", ex.Message + ">>" + ex.ToString(), true);            
               
                        return;
                    }

                }
                #endregion

               // dgvFlaws.Rows.Clear();
                VisbleOrNot(false);

                delta *= -1;
                //this.ParentForm.Tag = StringGeneral.One;
                this.ParentForm.Tag = lbNote.Name;
                this.ParentForm.Width += delta;

            }
            #endregion

        }
        private void VisbleOrNot(bool value)
        {
            //cbxCondition.Visible = value;
        }
    }
}
