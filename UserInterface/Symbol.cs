using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class Symbol : Form
    {
        string sign = "";
        public Symbol()
        {
            InitializeComponent();
        }

        private void Symbol_Load(object sender, EventArgs e) //2012-11-26 合并程序
        {
            DataTable dtN = new DataTable();
            dtN.Columns.Add("a", typeof(string));
            dtN.Columns.Add("b", typeof(string));
            dtN.Columns.Add("c", typeof(string));
            dtN.Columns.Add("d", typeof(string));
            dtN.Columns.Add("e", typeof(string));
            dtN.Columns.Add("f", typeof(string));
            dtN.Columns.Add("g", typeof(string));
            dtN.Columns.Add("h", typeof(string));
            dtN.Columns.Add("i", typeof(string));
            dtN.Columns.Add("j", typeof(string));
            dtN.Columns.Add("k", typeof(string));
            dtN.Columns.Add("l", typeof(string));
            DataRow drTemp1 = dtN.NewRow();
            drTemp1["a"] = "①.";
            drTemp1["b"] = "②.";
            drTemp1["c"] = "③.";
            drTemp1["d"] = "④.";
            drTemp1["e"] = "⑤.";
            drTemp1["f"] = "⑥.";
            drTemp1["g"] = "⑦.";
            drTemp1["h"] = "⑧.";
            drTemp1["i"] = "⑨.";
            drTemp1["j"] = "⑩.";
            drTemp1["k"] = "1).";
            drTemp1["l"] = "2).";

            dtN.Rows.Add(drTemp1);
            DataRow drTemp2 = dtN.NewRow();
            drTemp2["a"] = "3).";
            drTemp2["b"] = "4).";
            drTemp2["c"] = "5).";
            drTemp2["d"] = "6).";
            drTemp2["e"] = "7).";
            drTemp2["f"] = "8).";
            drTemp2["g"] = "9).";
            drTemp2["h"] = "10).";
            drTemp2["i"] = "⑴.";
            drTemp2["j"] = "⑵.";
            drTemp2["k"] = "⑶.";
            drTemp2["l"] = "⑷.";

            dtN.Rows.Add(drTemp2);
            DataRow drTemp3 = dtN.NewRow();
            drTemp3["a"] = "⑸.";
            drTemp3["b"] = "⑹.";
            drTemp3["c"] = "⑺.";
            drTemp3["d"] = "⑻.";
            drTemp3["e"] = "⑼.";
            drTemp3["f"] = "⑽.";
            drTemp3["g"] = "Ⅰ";
            drTemp3["h"] = "Ⅱ";
            drTemp3["i"] = "Ⅲ";
            drTemp3["j"] = "Ⅳ";
            drTemp3["k"] = "Ⅴ";
            drTemp3["l"] = "Ⅵ";

            dtN.Rows.Add(drTemp3);
            DataRow drTemp4 = dtN.NewRow();
            drTemp4["a"] = "Ⅶ";
            drTemp4["b"] = "Ⅷ";
            drTemp4["c"] = "Ⅸ";
            drTemp4["d"] = "Ⅹ";
            drTemp4["e"] = "Ⅺ";
            drTemp4["f"] = "Ⅻ";
            drTemp4["g"] = "α";
            drTemp4["h"] = "β";
            drTemp4["i"] = "γ";
            drTemp4["j"] = "δ";
            drTemp4["k"] = "ε";
            drTemp4["l"] = "ζ";

            dtN.Rows.Add(drTemp4);
            DataRow drTemp5 = dtN.NewRow();
            drTemp5["a"] = "η";
            drTemp5["b"] = "θ";
            drTemp5["c"] = "ι";
            drTemp5["d"] = "κ";
            drTemp5["e"] = "λ";
            drTemp5["f"] = "μ";
            drTemp5["g"] = "ν";
            drTemp5["h"] = "ξ";
            drTemp5["i"] = "ο";
            drTemp5["j"] = "π";
            drTemp5["k"] = "ρ";
            drTemp5["l"] = "σ";
            dtN.Rows.Add(drTemp5);

            DataRow drTemp6 = dtN.NewRow();
            drTemp6["a"] = "τ";
            drTemp6["b"] = "υ";
            drTemp6["c"] = "φ";
            drTemp6["d"] = "χ";
            drTemp6["e"] = "ψ";
            drTemp6["f"] = "ω";
            drTemp6["g"] = "℃";
            drTemp6["h"] = "℉";
            drTemp6["i"] = "％";
            drTemp6["j"] = "‰";
            drTemp6["k"] = "㎎";
            drTemp6["l"] = "㎏";
            dtN.Rows.Add(drTemp6);

            DataRow drTemp7 = dtN.NewRow();
            drTemp7["a"] = "㎜";
            drTemp7["b"] = "㎝";
            drTemp7["c"] = "㎞";
            drTemp7["d"] = "㎡";
            drTemp7["e"] = "㏄";
            drTemp7["f"] = "㏎";
            drTemp7["g"] = "㏑";
            drTemp7["h"] = "㏒";
            drTemp7["i"] = "ml";
            drTemp7["j"] = "mol";
            drTemp7["k"] = "㏕";
            drTemp7["l"] = "±";
            dtN.Rows.Add(drTemp7);


            DataRow drTemp8 = dtN.NewRow();
            drTemp8["a"] = "nmol/L";
            drTemp8["b"] = "μmol/L  ";
            drTemp8["c"] = "ng/dl";
            drTemp8["d"] = "ug/dl";
            drTemp8["e"] = "pg/ml";
            drTemp8["f"] = "IU/ml";
            drTemp8["g"] = "ng/mL";
            drTemp8["h"] = "g/L";
            drTemp8["i"] = "U/L";
            drTemp8["j"] = "uIU/mL";
            drTemp8["k"] = "mm";
            drTemp8["l"] = "/L";
            dtN.Rows.Add(drTemp8);


            dataGridViewX1.DataSource = dtN;

        }

        private void dataGridViewX1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            sign = dataGridViewX1.SelectedCells[0].Value.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sign = dataGridViewX1.SelectedCells[0].Value.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public string GetSign()
        {
            return sign;
        }
    }
}
