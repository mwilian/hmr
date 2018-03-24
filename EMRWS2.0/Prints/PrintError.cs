using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class PrintError : Form
    {
        bool ch = false;
        public PrintError()
        {
            InitializeComponent();
            this.Height = this.Height - panel3.Height;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ch = !ch;
            if (ch == true)
            {
                button2.Text = "Òþ²Ø°ïÖú(E) <<";
                panel3.Visible = true;
                this.Height = this.Height + 168;
                button3.Visible = true;
            }
            else
            {
                button2.Text = "ÏÔÊ¾°ïÖú(E) >>";
                panel3.Visible = false;
                this.Height = this.Height - 168;
                button3.Visible = false;
            }
        }
    }
}