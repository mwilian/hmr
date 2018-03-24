using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Rendering;
using System.Xml;
using EmrConstant;

namespace UserInterface
{
    public partial class EmrNoteInfo : Form
    {
        public EmrNoteInfo(XmlNode emrNote)
        {
            InitializeComponent();
            Color tedColor = Color.FromArgb(((int)(((byte)(143)))), ((int)(((byte)(164)))), ((int)(((byte)(190)))));
            //RibbonPredefinedColorSchemes.ChangeOffice2007ColorTable(this, eOffice2007ColorScheme.Silver);
            RibbonPredefinedColorSchemes.ChangeOffice2007ColorTable(this, eOffice2007ColorScheme.Black, tedColor);
            gpInfo.Text = emrNote.Attributes[AttributeNames.NoteName].Value;
            tbWriteTime.Text=emrNote.Attributes[AttributeNames.WrittenDate].Value
              +" "+emrNote.Attributes[AttributeNames.WrittenTime].Value;
            Writer.Text = emrNote.Attributes[AttributeNames.Writer].Value;
            Checker.Text = emrNote.Attributes[AttributeNames.Checker].Value;
           LastChecker.Text = emrNote.Attributes[AttributeNames.FinalChecker].Value;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
