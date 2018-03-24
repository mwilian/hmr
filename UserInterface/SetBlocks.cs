using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;


namespace UserInterface
{
    public partial class SetBlocks : Form
    {

        public SetBlocks(DataTable dt)
        {
            InitializeComponent();
            LoadDgvBlock(dt);
        }
        private string pk;
        public string GetBlockPk()
        {
            return pk;
        }
        private string blockName;
        public string GetBlockName()
        {
            return blockName;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            if (dgvBlocks.Rows.Count == 0) return;
            pk = dgvBlocks.SelectedRows[0].Cells[2].Value.ToString();

            blockName = dgvBlocks.SelectedRows[0].Cells[3].Value.ToString();

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void LoadDgvBlock(DataTable dt)
        {
            dgvBlocks.AllowUserToAddRows = false;

            dgvBlocks.AutoGenerateColumns = false;

            dgvBlocks.DataSource = dt;
        }      

        private void dgvBlocks_CellContentDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
             btnOK_Click(sender, e);
        }
    }
}
