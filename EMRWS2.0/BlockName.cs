using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace EMR
{
    public partial class BlockName : Form
    {
        private DataTable bdt = null;
        private int selectedIndex = -1;

        public BlockName(DataTable dt)
        {
            InitializeComponent();
            bdt = dt;
            for (int i = 0; i < dt.Rows.Count;i++)
            {
                cbCategory.Items.Add(dt.Rows[i]["Category"]);
            }
            if (dt.Rows.Count > 0)
            {
                cbCategory.SelectedIndex = 0;
                selectedIndex = 0;
                LoadBlockNames();
            }
        }

        private void LoadBlockNames()
        {
            lbNames.Items.Clear();
            for (int i = 0; i < bdt.Rows.Count;i++ )
            {
                lbNames.Items.Add(bdt.Rows[i]["Block"].ToString());
            }
            tbName.Text = EmrConstant.StringGeneral.NewBlockName;
            tbName.Focus();
            tbName.SelectAll();
        }

        public string GetCategory()
        {
            return cbCategory.Text;
        }

        public string GetBlockName()
        {
            return tbName.Text;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {         
         
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
            if (cbCategory.Text.IndexOfAny(charList) != -1)
            {
                MessageBox.Show(@"命名中不允许含有\ / * : ?  < > | 双引号等字符 ");
                return;
            }
            foreach (string name in lbNames.Items)
            {
                if (name == tbName.Text)
                {
                    MessageBox.Show(EmrConstant.ErrorMessage.NoSameBlockName, EmrConstant.ErrorMessage.Warning);
                    tbName.Focus();
                    tbName.SelectAll();
                    return;
                }
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }


        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = cbCategory.SelectedIndex;
            LoadBlockNames();
        }      

        private void cbCategory_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int index = 0;
                foreach (string item in cbCategory.Items)
                {
                    if (item == cbCategory.Text)
                    {
                        selectedIndex = index;                       
                        return;
                    }
                    index++;
                }
                tbName.Text = EmrConstant.StringGeneral.NewBlockName;
                tbName.Focus();
                lbNames.Items.Clear();
            }
        }
    }
}
