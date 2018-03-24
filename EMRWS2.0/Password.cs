using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using UserInterface;
using EmrConstant;
using CommonLib;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Collections;
using Office = Microsoft.Office.Core;
using HuronControl;
namespace EMR
{
    public partial class Password : Form
    {
        public string passwd = "";

        public int pp = 0;

        public Password()
        {
            InitializeComponent();
        }

        private void tbPasswd_KPress(object sender, KeyPressEventArgs e)
        {
            if (tBPassword.Text == "") return;
            if (tBPassword.Text == "jwsj")
            {
                try
                {
                    object False = (object)false;
                    object psd = (object)"jwsj";
                }
                catch (Exception ex)
                {

                }
            }
          
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tBPassword.Text == "") return;
            if (tBPassword.Text == "jwsj")
            {
                passwd = tBPassword.Text.Trim();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

      
    }
}
