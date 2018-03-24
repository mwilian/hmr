using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
using EmrConstant;
namespace UserInterface
{
    public partial class FontSetting : Form
    {
        //string familyName = "宋体";
        float emSize = 10.5F;
        FontStyle style = System.Drawing.FontStyle.Regular;
        FontStyle FontShape = System.Drawing.FontStyle.Regular;

        public FontSetting()
        {
            InitializeComponent();
            lbxShape.SelectedItem = lbxShape.Items[0];
            lbxSize.SelectedItem = lbxSize.Items[1];
            GetFont();
        }
        private void GetFont()
        {
            InstalledFontCollection insFont = new InstalledFontCollection();
            FontFamily[] families = insFont.Families;
            int n = 0;
            foreach (FontFamily family in families)
            {
                //只显示支持普通文本风格的字体系列
                if (family.IsStyleAvailable(FontStyle.Regular))
                {
                    lbFont.Items.Add(family.Name);
                    if (family.Name == "宋体") n = lbFont.Items.Count;
                }
            }
            if (n > 0) lbFont.SelectedItem = lbFont.Items[n - 1];
            else lbFont.SelectedItem = lbFont.Items[0];
        }

        private void lbShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lbxShape.SelectedIndex)
            {
                case 0:
                    tbxShape.Text = "常规";
                    FontShape = System.Drawing.FontStyle.Regular;
                    break;
                case 1:
                    tbxShape.Text = "倾斜";
                    FontShape = System.Drawing.FontStyle.Italic;
                    break;
                case 2:
                    tbxShape.Text = "加粗";
                    FontShape = System.Drawing.FontStyle.Bold;
                    break;
                case 3:
                    tbxShape.Text = "倾斜 加粗";
                    FontShape = (System.Drawing.FontStyle.Italic) | (System.Drawing.FontStyle.Bold);
                    break;
            }
            SetView();
        }
        private void SetView()
        {
            if (cbxUnderline.Checked) style = FontShape | System.Drawing.FontStyle.Underline;
            else style = FontShape ;
            if (cbxStrikethrough.Checked) style = style | System.Drawing.FontStyle.Strikeout;
            lbView.Font = new System.Drawing.Font(tbxFont.Text, emSize, style);
        }
        public float GetemSize()
        {
            return emSize;
        }
        public int GetStrikethrough()
        {
            return cbxStrikethrough.Checked == true ? -1 : 0;
        }
        public int GetBold()
        {
            if (lbxShape.SelectedIndex == 2 || lbxShape.SelectedIndex == 3)
                return -1;
            else return 0;
        }
        public int GetItalic()
        {
            if (lbxShape.SelectedIndex == 1 || lbxShape.SelectedIndex == 3)
                return -1;
            else return 0;
        }
        public Color GetColor()
        {
            return this.lbView.ForeColor;
        }
        public bool GetUnderline()
        {
            return cbxUnderline.Checked ;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cbStrikethrough_CheckedChanged(object sender, EventArgs e)
        {
            SetView();
        }

        private void cbUnderline_CheckedChanged(object sender, EventArgs e)
        {
            SetView();
        }

        private void lbxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lbxSize.SelectedIndex)
            {
                case 0:
                    tbxSize.Text = "小四";
                    emSize = 12F;
                    break;
                case 1:
                    tbxSize.Text = "五号";
                    emSize = 10.5F;
                    break;
            }
            SetView();
        }

        private void lbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbxFont.Text = lbFont.SelectedItem.ToString();
            SetView();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {

            this.Close();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
     
            DialogResult = DialogResult.OK;
            this.Close();
       
        }

        private void colorPickerButton1_SelectedColorChanged(object sender, EventArgs e)
        {
            colorPickerButton1.Text = "";
            //colorPickerDropDown1.SelectedColor;
            this.lbView.ForeColor = colorPickerButton1.SelectedColor;
                 //this.lbView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        }
        public string GetfamilyName()
        {
            return tbxFont.Text;
        }

    }
}
