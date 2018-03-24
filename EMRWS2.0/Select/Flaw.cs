using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;

namespace EMR
{
    public partial class Flaw : UserControl
    {
        private int height = 0;
        private decimal knock = 0;
        public Flaw(XmlNode flaws)
        {
            InitializeComponent();

            lbNoteName.Text = flaws.Attributes[AttributeNames.NoteName].Value;
            lbNoteName.Tag = flaws.Attributes[AttributeNames.NoteID].Value;
            lbScore.Text = "得分：" + flaws.Attributes[AttributeNames.Score].Value;
            lbScore.Tag = flaws.Attributes[AttributeNames.Score].Value;

            foreach (XmlNode flaw in flaws.ChildNodes)
            {
                int index = dgvFlaws.Rows.Add();
                dgvFlaws.Rows[index].Cells[0].Value = flaw.InnerText;
                dgvFlaws.Rows[index].Cells[1].Value = flaw.Attributes[AttributeNames.Knockoff].Value;
                knock += Convert.ToDecimal(flaw.Attributes[AttributeNames.Knockoff].Value);
                height += dgvFlaws.Rows[index].Height;

            }
            dgvFlaws.Height = height + 2;
            //this.height = dgvFlaws.Top + dgvFlaws.Height + 1;
        }

        private void Flaw_Resize(object sender, EventArgs e)
        {
            dgvFlaws.Width = this.Width;
            lbScore.Left = this.Width - lbScore.Width - 1;
            dgvFlaws.Columns[0].Width = this.Width - dgvFlaws.Columns[1].Width - 2;
        }

        public int GetControlHeight()
        {
            return dgvFlaws.Top + dgvFlaws.Height + 1;
        }

        public decimal GetScore()
        {
            return Convert.ToDecimal(lbScore.Tag);
        }
        public decimal GetKnockoff()
        {
            return knock;
        }
        public string GetNoteID()
        {
            return lbNoteName.Tag.ToString();
        }
        public void NewFlaw(string flawText, decimal knockoff)
        {
            int index = dgvFlaws.Rows.Add();
            dgvFlaws.Rows[index].Cells[0].Value = flawText;
            dgvFlaws.Rows[index].Cells[1].Value = knockoff.ToString();
            knock += knockoff;
            decimal score = Convert.ToDecimal(lbScore.Tag) - knockoff;
            lbScore.Text = "得分：" + score.ToString();
            lbScore.Tag = score.ToString();
            height += dgvFlaws.Rows[index].Height;
            dgvFlaws.Height += dgvFlaws.Rows[index].Height;
            this.Height += dgvFlaws.Rows[index].Height;
        }
      
    }
}
