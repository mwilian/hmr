namespace EMR.Prints
{
    partial class ContiPrint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSpace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbSelf = new System.Windows.Forms.RadioButton();
            this.rbDefault = new System.Windows.Forms.RadioButton();
            this.cbkPrintEnd1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancelPrint = new DevComponents.DotNetBar.ButtonX();
            this.btnOK = new DevComponents.DotNetBar.ButtonX();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbPrint = new System.Windows.Forms.CheckBox();
            this.tbPageEnd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbPageStart = new System.Windows.Forms.TextBox();
            this.rbtnPageRange = new System.Windows.Forms.RadioButton();
            this.rbtnCursorDel = new System.Windows.Forms.RadioButton();
            this.cboPrinter = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rbtnThisPageNum = new System.Windows.Forms.RadioButton();
            this.rbtnAll = new System.Windows.Forms.RadioButton();
            this.rbtnCursor = new System.Windows.Forms.RadioButton();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.rbtnOnePrint = new System.Windows.Forms.RadioButton();
            this.rbtnTwoPrint = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSpace);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPage);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(311, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 102);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "自定义续打位置";
            // 
            // txtSpace
            // 
            this.txtSpace.Enabled = false;
            this.txtSpace.Location = new System.Drawing.Point(117, 58);
            this.txtSpace.Name = "txtSpace";
            this.txtSpace.Size = new System.Drawing.Size(118, 21);
            this.txtSpace.TabIndex = 3;
            this.txtSpace.Leave += new System.EventHandler(this.txtSpace_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "第几行开始打印：";
            // 
            // txtPage
            // 
            this.txtPage.Enabled = false;
            this.txtPage.Location = new System.Drawing.Point(117, 23);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(118, 21);
            this.txtPage.TabIndex = 1;
            this.txtPage.Leave += new System.EventHandler(this.txtPage_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "填写打印页码：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbSelf);
            this.panel1.Controls.Add(this.rbDefault);
            this.panel1.Location = new System.Drawing.Point(38, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(267, 31);
            this.panel1.TabIndex = 12;
            // 
            // rbSelf
            // 
            this.rbSelf.AutoSize = true;
            this.rbSelf.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbSelf.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.rbSelf.Location = new System.Drawing.Point(132, 6);
            this.rbSelf.Name = "rbSelf";
            this.rbSelf.Size = new System.Drawing.Size(114, 16);
            this.rbSelf.TabIndex = 1;
            this.rbSelf.Text = "自定义续打位置";
            this.rbSelf.UseVisualStyleBackColor = true;
            this.rbSelf.CheckedChanged += new System.EventHandler(this.rbSelf_CheckedChanged);
            // 
            // rbDefault
            // 
            this.rbDefault.AutoSize = true;
            this.rbDefault.Checked = true;
            this.rbDefault.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbDefault.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.rbDefault.Location = new System.Drawing.Point(4, 8);
            this.rbDefault.Name = "rbDefault";
            this.rbDefault.Size = new System.Drawing.Size(101, 16);
            this.rbDefault.TabIndex = 0;
            this.rbDefault.TabStop = true;
            this.rbDefault.Text = "默认续打位置";
            this.rbDefault.UseVisualStyleBackColor = true;
            this.rbDefault.CheckedChanged += new System.EventHandler(this.rbDefault_CheckedChanged);
            // 
            // cbkPrintEnd1
            // 
            this.cbkPrintEnd1.AutoSize = true;
            this.cbkPrintEnd1.Checked = true;
            this.cbkPrintEnd1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbkPrintEnd1.Location = new System.Drawing.Point(43, 69);
            this.cbkPrintEnd1.Name = "cbkPrintEnd1";
            this.cbkPrintEnd1.Size = new System.Drawing.Size(150, 16);
            this.cbkPrintEnd1.TabIndex = 10;
            this.cbkPrintEnd1.Text = "应用已存的续打位置   ";
            this.cbkPrintEnd1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "续打记录，请注意将纸张放摆放正确";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "续打记录：病历记录组";
            // 
            // btnCancelPrint
            // 
            this.btnCancelPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancelPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.btnCancelPrint.Font = new System.Drawing.Font("宋体", 9F);
            this.btnCancelPrint.Location = new System.Drawing.Point(541, 133);
            this.btnCancelPrint.Name = "btnCancelPrint";
            this.btnCancelPrint.Size = new System.Drawing.Size(82, 23);
            this.btnCancelPrint.TabIndex = 17;
            this.btnCancelPrint.Text = "取消打印";
            this.btnCancelPrint.Click += new System.EventHandler(this.btnCancelPrint_Click);
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOK.Font = new System.Drawing.Font("宋体", 9F);
            this.btnOK.Location = new System.Drawing.Point(409, 133);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(83, 23);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "确认续打";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnCancelPrint);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.cbkPrintEnd1);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(18, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(638, 180);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "逐个续打方式";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbPrint);
            this.groupBox3.Controls.Add(this.tbPageEnd);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.tbPageStart);
            this.groupBox3.Controls.Add(this.rbtnPageRange);
            this.groupBox3.Controls.Add(this.rbtnCursorDel);
            this.groupBox3.Controls.Add(this.cboPrinter);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.rbtnThisPageNum);
            this.groupBox3.Controls.Add(this.rbtnAll);
            this.groupBox3.Controls.Add(this.rbtnCursor);
            this.groupBox3.Controls.Add(this.btnCancel);
            this.groupBox3.Controls.Add(this.btnPrint);
            this.groupBox3.Location = new System.Drawing.Point(23, 218);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(633, 196);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "合并续打方式";
            // 
            // cbPrint
            // 
            this.cbPrint.AutoSize = true;
            this.cbPrint.Checked = true;
            this.cbPrint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPrint.Location = new System.Drawing.Point(116, 26);
            this.cbPrint.Name = "cbPrint";
            this.cbPrint.Size = new System.Drawing.Size(108, 16);
            this.cbPrint.TabIndex = 59;
            this.cbPrint.Text = "使用默认打印机";
            this.cbPrint.UseVisualStyleBackColor = true;
            this.cbPrint.CheckedChanged += new System.EventHandler(this.cbPrint_CheckedChanged);
            // 
            // tbPageEnd
            // 
            this.tbPageEnd.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tbPageEnd.Enabled = false;
            this.tbPageEnd.Location = new System.Drawing.Point(285, 133);
            this.tbPageEnd.Name = "tbPageEnd";
            this.tbPageEnd.Size = new System.Drawing.Size(107, 21);
            this.tbPageEnd.TabIndex = 58;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(258, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 57;
            this.label6.Text = "到：";
            // 
            // tbPageStart
            // 
            this.tbPageStart.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tbPageStart.Enabled = false;
            this.tbPageStart.Location = new System.Drawing.Point(149, 133);
            this.tbPageStart.Name = "tbPageStart";
            this.tbPageStart.Size = new System.Drawing.Size(103, 21);
            this.tbPageStart.TabIndex = 56;
            // 
            // rbtnPageRange
            // 
            this.rbtnPageRange.AutoSize = true;
            this.rbtnPageRange.Location = new System.Drawing.Point(35, 134);
            this.rbtnPageRange.Name = "rbtnPageRange";
            this.rbtnPageRange.Size = new System.Drawing.Size(107, 16);
            this.rbtnPageRange.TabIndex = 55;
            this.rbtnPageRange.Text = "指定页码  从：";
            this.rbtnPageRange.UseVisualStyleBackColor = true;
            this.rbtnPageRange.CheckedChanged += new System.EventHandler(this.rbtnPageRange_CheckedChanged);
            // 
            // rbtnCursorDel
            // 
            this.rbtnCursorDel.AutoSize = true;
            this.rbtnCursorDel.Location = new System.Drawing.Point(35, 112);
            this.rbtnCursorDel.Name = "rbtnCursorDel";
            this.rbtnCursorDel.Size = new System.Drawing.Size(191, 16);
            this.rbtnCursorDel.TabIndex = 54;
            this.rbtnCursorDel.Text = "光标位置之后内容（不去底纹）";
            this.rbtnCursorDel.UseVisualStyleBackColor = true;
            this.rbtnCursorDel.CheckedChanged += new System.EventHandler(this.rbtnCursorDel_CheckedChanged);
            // 
            // cboPrinter
            // 
            this.cboPrinter.Enabled = false;
            this.cboPrinter.FormattingEnabled = true;
            this.cboPrinter.Location = new System.Drawing.Point(230, 24);
            this.cboPrinter.Name = "cboPrinter";
            this.cboPrinter.Size = new System.Drawing.Size(279, 20);
            this.cboPrinter.TabIndex = 53;
            this.cboPrinter.SelectedIndexChanged += new System.EventHandler(this.cboPrinter_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 52;
            this.label5.Text = "选择打印机：";
            // 
            // rbtnThisPageNum
            // 
            this.rbtnThisPageNum.AutoSize = true;
            this.rbtnThisPageNum.Location = new System.Drawing.Point(35, 68);
            this.rbtnThisPageNum.Name = "rbtnThisPageNum";
            this.rbtnThisPageNum.Size = new System.Drawing.Size(107, 16);
            this.rbtnThisPageNum.TabIndex = 51;
            this.rbtnThisPageNum.Text = "光标位置当前页";
            this.rbtnThisPageNum.UseVisualStyleBackColor = true;
            this.rbtnThisPageNum.CheckedChanged += new System.EventHandler(this.rbtnThisPageNum_CheckedChanged);
            // 
            // rbtnAll
            // 
            this.rbtnAll.AutoSize = true;
            this.rbtnAll.Checked = true;
            this.rbtnAll.Location = new System.Drawing.Point(35, 46);
            this.rbtnAll.Name = "rbtnAll";
            this.rbtnAll.Size = new System.Drawing.Size(47, 16);
            this.rbtnAll.TabIndex = 50;
            this.rbtnAll.TabStop = true;
            this.rbtnAll.Text = "全部";
            this.rbtnAll.UseVisualStyleBackColor = true;
            this.rbtnAll.CheckedChanged += new System.EventHandler(this.rbtnAll_CheckedChanged);
            // 
            // rbtnCursor
            // 
            this.rbtnCursor.AutoSize = true;
            this.rbtnCursor.Location = new System.Drawing.Point(35, 90);
            this.rbtnCursor.Name = "rbtnCursor";
            this.rbtnCursor.Size = new System.Drawing.Size(179, 16);
            this.rbtnCursor.TabIndex = 49;
            this.rbtnCursor.Text = "光标位置之后内容（去底纹）";
            this.rbtnCursor.UseVisualStyleBackColor = true;
            this.rbtnCursor.CheckedChanged += new System.EventHandler(this.rbtnCursor_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.Font = new System.Drawing.Font("宋体", 9F);
            this.btnCancel.Location = new System.Drawing.Point(536, 162);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 43;
            this.btnCancel.Text = "取消打印";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Font = new System.Drawing.Font("宋体", 9F);
            this.btnPrint.Location = new System.Drawing.Point(404, 162);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(79, 23);
            this.btnPrint.TabIndex = 42;
            this.btnPrint.Text = "确认打印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // rbtnOnePrint
            // 
            this.rbtnOnePrint.AutoSize = true;
            this.rbtnOnePrint.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnOnePrint.Location = new System.Drawing.Point(75, 5);
            this.rbtnOnePrint.Name = "rbtnOnePrint";
            this.rbtnOnePrint.Size = new System.Drawing.Size(111, 23);
            this.rbtnOnePrint.TabIndex = 22;
            this.rbtnOnePrint.Text = "逐个续打方式";
            this.rbtnOnePrint.UseVisualStyleBackColor = true;
            this.rbtnOnePrint.CheckedChanged += new System.EventHandler(this.rbtnOnePrint_CheckedChanged);
            // 
            // rbtnTwoPrint
            // 
            this.rbtnTwoPrint.AutoSize = true;
            this.rbtnTwoPrint.Checked = true;
            this.rbtnTwoPrint.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnTwoPrint.Location = new System.Drawing.Point(329, 5);
            this.rbtnTwoPrint.Name = "rbtnTwoPrint";
            this.rbtnTwoPrint.Size = new System.Drawing.Size(111, 23);
            this.rbtnTwoPrint.TabIndex = 23;
            this.rbtnTwoPrint.TabStop = true;
            this.rbtnTwoPrint.Text = "合并续打方式";
            this.rbtnTwoPrint.UseVisualStyleBackColor = true;
            this.rbtnTwoPrint.CheckedChanged += new System.EventHandler(this.rbtnTwoPrint_CheckedChanged);
            // 
            // ContiPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(672, 421);
            this.Controls.Add(this.rbtnTwoPrint);
            this.Controls.Add(this.rbtnOnePrint);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ContiPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "打印记录";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ContiPrint_FormClosing);
            this.Load += new System.EventHandler(this.ContiPrint_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSpace;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbSelf;
        private System.Windows.Forms.RadioButton rbDefault;
        private System.Windows.Forms.CheckBox cbkPrintEnd1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DevComponents.DotNetBar.ButtonX btnCancelPrint;
        private DevComponents.DotNetBar.ButtonX btnOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private System.Windows.Forms.RadioButton rbtnThisPageNum;
        private System.Windows.Forms.RadioButton rbtnAll;
        private System.Windows.Forms.RadioButton rbtnCursor;
        private System.Windows.Forms.RadioButton rbtnOnePrint;
        private System.Windows.Forms.RadioButton rbtnTwoPrint;
        private System.Windows.Forms.ComboBox cboPrinter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbtnCursorDel;
        private System.Windows.Forms.RadioButton rbtnPageRange;
        private System.Windows.Forms.TextBox tbPageEnd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbPageStart;
        private System.Windows.Forms.CheckBox cbPrint;
    }
}