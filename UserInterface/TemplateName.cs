using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Collections;
using CommonLib;

namespace UserInterface
{
    public partial class TemplateName : Form
    {
        public TemplateName(ArrayList templateNames, string defaultName)
        {
            InitializeComponent();
            tbName.Text = defaultName;
            lbNames.Items.Clear();
            foreach (string templateName in templateNames)
            {               
                lbNames.Items.Add(templateName);
            }
        }
        public string Get()
        {
            return tbName.Text;
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            bool closeForm = true;
            char[] charList = new char[9];
            charList[0] = '?';
            charList[1] = ':';
            charList[2] = '*';
            charList[3] = '"';
            charList[4] = '<';
            charList[5] = '>';
            charList[6] = '|';
            charList[7] = '/';

            charList[8] = Convert.ToChar(@"\");

            if (tbName.Text.IndexOfAny(charList) != -1)
            {
                MessageBox.Show(@"命名中不允许含有\ / * : ?  < > | 双引号等字符 ");
                return;
            }
            foreach (string name in lbNames.Items)
            {

                if (name == tbName.Text)
                {
                    MessageBox.Show(EmrConstant.ErrorMessage.NoSameTemplateName, EmrConstant.ErrorMessage.Error);
                    closeForm = false;
                    break;
                }
            }
            if (closeForm)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void lbNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                tbName.Text = lbNames.Items[lbNames.SelectedIndex].ToString();
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX7569874555256", ex.Message + ">>" + ex.ToString(), true);            
             
            }
        }
    }
}
