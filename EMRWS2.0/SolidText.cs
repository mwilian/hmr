using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using Word=Microsoft.Office.Interop.Word;
using CommonLib;

namespace EMR
{
    public partial class SolidTextChange : Form
    {
        Word.Application WordApp = null;
        int cjjleft, cjjtop, cjjwidth, cjjheight;
        public SolidTextChange(Word.Application wordApp)
        {
            InitializeComponent();
            WordApp = wordApp;
        }
        
        public void Init()
        {
            Word.Range range = WordApp.Selection.Range;
            WordApp.ActiveWindow.GetPoint(out cjjleft, out cjjtop, out cjjwidth, out cjjheight, range);
            this.Location = new Point(cjjleft, cjjtop + 30);
        }

        private void cboInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                WordApp.Selection.Range.InsertAfter(cboInfo.SelectedItem.ToString());
                this.Close();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925512156895", ex.Message + ">>" + ex.ToString(), true);            
               
                MessageBox.Show(this, "此处为非编辑区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
        }

        private void SolidTextChange_Deactivate(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}