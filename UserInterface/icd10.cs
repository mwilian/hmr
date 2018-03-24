using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace UserInterface
{
    public partial class icd10 : Form
    {
        public icd10(string icd10File)
        {
            InitializeComponent();

            if (File.Exists(icd10File))
            {
                DataSet localIcd = new DataSet();
                localIcd.ReadXml(icd10File);
                dgvIcd10.DataSource = localIcd.Tables[0];
            }        
            if (dgvIcd10.ColumnCount == 2)
            {
                dgvIcd10.Columns[0].Width = 150;
                dgvIcd10.Columns[1].Width = dgvIcd10.Width - dgvIcd10.Columns[0].Width;
                //dgvIcd10.Columns[2].Visible = false;
            }
            

            tbxCode.ImeMode = ImeMode.Close;
        }

        private void icd10_Resize(object sender, EventArgs e)
        {
            dgvIcd10.Width = this.Width - 12;
            dgvIcd10.Height = this.Height - dgvIcd10.Top - 30;
            tbxCode.Width = dgvIcd10.Width;
        }

        private void tbxCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgvIcd10 == null) return;
            if (e.KeyCode == Keys.Enter)
            {
                tbxCode.Text = dgvIcd10.CurrentRow.Cells[1].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                string code = tbxCode.Text.ToUpper();
                int cout = code.Trim().Length;
                foreach (DataGridViewRow row in dgvIcd10.Rows)
                {
                    if (row.Cells[0].Value.ToString().Trim().Length < cout)
                        continue;
                    if (row.Cells[0].Value.ToString().Trim().Substring(0,cout).Equals(code))
                    {
                        dgvIcd10.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }
        }

        public string GetIcd10Text()
        {
            return tbxCode.Text;
        }

        private void dgvIcd10_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            tbxCode.Text = dgvIcd10.Rows[e.RowIndex].Cells[1].Value.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tbxCode_Enter(object sender, EventArgs e)
        {
            tbxCode.ImeMode = ImeMode.Close;
        }

    }
}