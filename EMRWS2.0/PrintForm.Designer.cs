namespace EMR
{
    partial class PrintForm
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbtnThisPageNum = new System.Windows.Forms.RadioButton();
            this.rbtnAll = new System.Windows.Forms.RadioButton();
            this.rbtnCursor = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.rbtnCheckPageNum = new System.Windows.Forms.RadioButton();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.cbkRePageNum = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtReStartPage = new System.Windows.Forms.TextBox();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbtnThisPageNum);
            this.groupBox4.Controls.Add(this.rbtnAll);
            this.groupBox4.Controls.Add(this.rbtnCursor);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.rbtnCheckPageNum);
            this.groupBox4.Controls.Add(this.txtEnd);
            this.groupBox4.Controls.Add(this.txtStart);
            this.groupBox4.Location = new System.Drawing.Point(24, 42);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(378, 124);
            this.groupBox4.TabIndex = 41;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "打印范围";
            // 
            // rbtnThisPageNum
            // 
            this.rbtnThisPageNum.AutoSize = true;
            this.rbtnThisPageNum.Location = new System.Drawing.Point(14, 51);
            this.rbtnThisPageNum.Name = "rbtnThisPageNum";
            this.rbtnThisPageNum.Size = new System.Drawing.Size(59, 16);
            this.rbtnThisPageNum.TabIndex = 48;
            this.rbtnThisPageNum.Text = "当前页";
            this.rbtnThisPageNum.UseVisualStyleBackColor = true;
            this.rbtnThisPageNum.CheckedChanged += new System.EventHandler(this.rbtnThisPageNum_CheckedChanged);
            // 
            // rbtnAll
            // 
            this.rbtnAll.AutoSize = true;
            this.rbtnAll.Checked = true;
            this.rbtnAll.Location = new System.Drawing.Point(14, 29);
            this.rbtnAll.Name = "rbtnAll";
            this.rbtnAll.Size = new System.Drawing.Size(47, 16);
            this.rbtnAll.TabIndex = 47;
            this.rbtnAll.TabStop = true;
            this.rbtnAll.Text = "全部";
            this.rbtnAll.UseVisualStyleBackColor = true;
            this.rbtnAll.CheckedChanged += new System.EventHandler(this.rbtnAll_CheckedChanged);
            // 
            // rbtnCursor
            // 
            this.rbtnCursor.AutoSize = true;
            this.rbtnCursor.Location = new System.Drawing.Point(14, 73);
            this.rbtnCursor.Name = "rbtnCursor";
            this.rbtnCursor.Size = new System.Drawing.Size(119, 16);
            this.rbtnCursor.TabIndex = 41;
            this.rbtnCursor.Text = "光标位置之后内容";
            this.rbtnCursor.UseVisualStyleBackColor = true;
            this.rbtnCursor.CheckedChanged += new System.EventHandler(this.rbtnCursor_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(249, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 45;
            this.label6.Text = "页";
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(175, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 44;
            this.label5.Text = "—";
            this.label5.Visible = false;
            // 
            // rbtnCheckPageNum
            // 
            this.rbtnCheckPageNum.AutoSize = true;
            this.rbtnCheckPageNum.Location = new System.Drawing.Point(14, 97);
            this.rbtnCheckPageNum.Name = "rbtnCheckPageNum";
            this.rbtnCheckPageNum.Size = new System.Drawing.Size(107, 16);
            this.rbtnCheckPageNum.TabIndex = 40;
            this.rbtnCheckPageNum.Text = "选择打印页码：";
            this.rbtnCheckPageNum.UseVisualStyleBackColor = true;
            this.rbtnCheckPageNum.Visible = false;
            this.rbtnCheckPageNum.CheckedChanged += new System.EventHandler(this.rbtnCheckPageNum_CheckedChanged);
            // 
            // txtEnd
            // 
            this.txtEnd.Enabled = false;
            this.txtEnd.Location = new System.Drawing.Point(199, 96);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(39, 21);
            this.txtEnd.TabIndex = 43;
            this.txtEnd.Visible = false;
            // 
            // txtStart
            // 
            this.txtStart.Enabled = false;
            this.txtStart.Location = new System.Drawing.Point(127, 96);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(42, 21);
            this.txtStart.TabIndex = 42;
            this.txtStart.Visible = false;
            // 
            // cbkRePageNum
            // 
            this.cbkRePageNum.AutoSize = true;
            this.cbkRePageNum.Location = new System.Drawing.Point(24, 17);
            this.cbkRePageNum.Name = "cbkRePageNum";
            this.cbkRePageNum.Size = new System.Drawing.Size(132, 16);
            this.cbkRePageNum.TabIndex = 40;
            this.cbkRePageNum.Text = "重新定义起始页码：";
            this.cbkRePageNum.UseVisualStyleBackColor = true;
            this.cbkRePageNum.Visible = false;
            this.cbkRePageNum.TextChanged += new System.EventHandler(this.cbkRePageNum_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(213, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 39;
            this.label7.Text = "页";
            this.label7.Visible = false;
            // 
            // txtReStartPage
            // 
            this.txtReStartPage.Enabled = false;
            this.txtReStartPage.Location = new System.Drawing.Point(161, 15);
            this.txtReStartPage.Name = "txtReStartPage";
            this.txtReStartPage.Size = new System.Drawing.Size(42, 21);
            this.txtReStartPage.TabIndex = 38;
            this.txtReStartPage.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.Font = new System.Drawing.Font("宋体", 9F);
            this.btnCancel.Location = new System.Drawing.Point(251, 185);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 23);
            this.btnCancel.TabIndex = 37;
            this.btnCancel.Text = "取消打印";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Font = new System.Drawing.Font("宋体", 9F);
            this.btnPrint.Location = new System.Drawing.Point(63, 185);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(69, 23);
            this.btnPrint.TabIndex = 36;
            this.btnPrint.Text = "确认打印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(422, 230);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.cbkRePageNum);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtReStartPage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPrint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PrintForm";
            this.Text = "打印记录";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbtnThisPageNum;
        private System.Windows.Forms.RadioButton rbtnAll;
        private System.Windows.Forms.RadioButton rbtnCursor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbtnCheckPageNum;
        private System.Windows.Forms.TextBox txtEnd;
        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.CheckBox cbkRePageNum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtReStartPage;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.ButtonX btnPrint;

    }
}