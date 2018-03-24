using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EmrConstant;
namespace UserInterface
{
    public partial class Emr_Message : Form
    {
        //添加一个StayTime属性设置窗体停留时间（默认为0.5秒）： 
        public int StayTime = 500;
        string messg = "";
        public Emr_Message(string Mess, double STime, MsgTpe type)
        {
            InitializeComponent();
            messg = Mess;
            lbMsg.Text = Mess;
            StayTime =Convert.ToInt32( STime * 1000);
            switch (type)
            {
                case MsgTpe.Error:
                    lbInfo.Text = "错误：";
                    lbMsg.Text = "程序异常错误";
                    this.panelEx1.Style.BackColor1.Color = System.Drawing.Color.OrangeRed;
                    this.panelEx1.Style.BackColor2.Color = System.Drawing.Color.MistyRose;
     break;
                case MsgTpe.InfoPrompt:
                    lbInfo.Text = "提示：";
                    this.panelEx1.Style.BackColor1.Color = System.Drawing.Color.SkyBlue;
                    this.panelEx1.Style.BackColor2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                    break;
                case MsgTpe.Warning:
                    lbInfo.Text = "警告：";
                    this.panelEx1.Style.BackColor1.Color = System.Drawing.Color.Yellow;
                    this.panelEx1.Style.BackColor2.Color = System.Drawing.Color.PapayaWhip;
                    break;
            }
        }



        private int heightMax, widthMax;
        public int HeightMax
        {
            set
            {
                heightMax = value;
            }
            get
            {
                 
                return heightMax;
            }
        }
        public int WidthMax
        {
            set
            {
                widthMax = value;
            }
            get
            {
                return widthMax;
            }
        }


        public void ScrollShow()
        {
            this.Width = widthMax;
            this.Height = 0;
            this.Show();
            this.timer1.Enabled = true;
 
        }
        public void ScrollShow1()
        {
            this.Width = widthMax;
            this.Height = 0;
            this.Show();
            int i=0;
            bool isSc = true;
            do
            {
              
                DateTime dt = DateTime.Now;
                do
                {
                 
                    
                }
                while (dt.ToString("mm-ss").Equals(DateTime.Now.ToString("mm-ss")));
                if (Height < heightMax && isSc)
                {
                    this.Height += 70;
                    this.Location = new Point(this.Location.X, this.Location.Y - 70);
                }
                else
                {
                    if (isSc == true)
                    {
                        isSc = false;
                    }
                    else
                    {
                        if (Height > 3)
                        {
                            this.Height -= 15;
                            this.Location = new Point(this.Location.X, this.Location.Y + 15);
                        }
                        else return;
                    }
                }
            }
            while (i < 100000);
        }
        //添加ScrollUp和ScrollDown方法来编写窗体如何滚出和滚入
        private void ScrollUp()
        {
            if (Height < heightMax)
            {
                this.Height += 30;
                this.Location = new Point(this.Location.X, this.Location.Y - 30);
            }
            else
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = true;
            }
        }
        private void ScrollDown()
        {
            if (Height > 3)
            {
                this.Height -= 15;
                this.Location = new Point(this.Location.X, this.Location.Y + 15);
            }
            else
            {
                this.timer3.Enabled = false;
                this.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ScrollUp();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            timer3.Enabled = true;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            ScrollDown();
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void Emr_Message_Load(object sender, EventArgs e)
        {
            Screen[] screens = Screen.AllScreens;
            Screen screen = screens[0];//获取屏幕变量
            this.Location = new Point(screen.WorkingArea.Width - widthMax - 20, screen.WorkingArea.Height - 34);//WorkingArea为Windows桌面的工作区
            this.timer2.Interval = StayTime;
        }

        private void panelEx1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelEx1_MouseMove(object sender, MouseEventArgs e)
        {
            timer3.Stop();
            timer2.Stop();
        }

        private void lbMsg_Click(object sender, EventArgs e)
        {
            this.Close();
        }      

        private void lbMsg_MouseMove(object sender, MouseEventArgs e)
        {
            timer3.Stop();
            timer2.Stop();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(messg);
        }
    }
}