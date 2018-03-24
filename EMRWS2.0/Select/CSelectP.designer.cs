namespace EMR
{
    partial class CSelectP
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
            this.btnReport = new DevComponents.DotNetBar.ButtonX();
            this.btnfindExit = new DevComponents.DotNetBar.ButtonX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbDate = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboQy = new System.Windows.Forms.RadioButton();
            this.cboHz = new System.Windows.Forms.RadioButton();
            this.cboDepart = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReport
            // 
            this.btnReport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnReport.Location = new System.Drawing.Point(250, 184);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(74, 24);
            this.btnReport.TabIndex = 28;
            this.btnReport.Text = "查询";
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // btnfindExit
            // 
            this.btnfindExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnfindExit.Location = new System.Drawing.Point(355, 184);
            this.btnfindExit.Name = "btnfindExit";
            this.btnfindExit.Size = new System.Drawing.Size(74, 24);
            this.btnfindExit.TabIndex = 33;
            this.btnfindExit.Text = "退出";
            this.btnfindExit.Click += new System.EventHandler(this.btnfindExit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbDate);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.dateTimePickerEnd);
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Location = new System.Drawing.Point(23, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(406, 68);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "住院日期范围";
            // 
            // lbDate
            // 
            this.lbDate.AutoSize = true;
            this.lbDate.Location = new System.Drawing.Point(23, 32);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(65, 12);
            this.lbDate.TabIndex = 40;
            this.lbDate.Text = "时间范围：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label6.Location = new System.Drawing.Point(224, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 14);
            this.label6.TabIndex = 37;
            this.label6.Text = "至";
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Location = new System.Drawing.Point(262, 28);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(112, 21);
            this.dateTimePickerEnd.TabIndex = 36;
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Location = new System.Drawing.Point(94, 28);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(112, 21);
            this.dateTimePickerStart.TabIndex = 35;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboQy);
            this.groupBox2.Controls.Add(this.cboHz);
            this.groupBox2.Controls.Add(this.cboDepart);
            this.groupBox2.Location = new System.Drawing.Point(25, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(404, 90);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "查询条件";
            // 
            // cboQy
            // 
            this.cboQy.AutoSize = true;
            this.cboQy.Location = new System.Drawing.Point(23, 47);
            this.cboQy.Name = "cboQy";
            this.cboQy.Size = new System.Drawing.Size(47, 16);
            this.cboQy.TabIndex = 49;
            this.cboQy.TabStop = true;
            this.cboQy.Text = "全院";
            this.cboQy.UseVisualStyleBackColor = true;
            // 
            // cboHz
            // 
            this.cboHz.AutoSize = true;
            this.cboHz.Location = new System.Drawing.Point(23, 69);
            this.cboHz.Name = "cboHz";
            this.cboHz.Size = new System.Drawing.Size(173, 16);
            this.cboHz.TabIndex = 48;
            this.cboHz.Text = "汇总 （各科室病历等级率）";
            this.cboHz.UseVisualStyleBackColor = true;
            this.cboHz.Visible = false;
            // 
            // cboDepart
            // 
            this.cboDepart.AutoSize = true;
            this.cboDepart.Checked = true;
            this.cboDepart.Location = new System.Drawing.Point(23, 25);
            this.cboDepart.Name = "cboDepart";
            this.cboDepart.Size = new System.Drawing.Size(185, 16);
            this.cboDepart.TabIndex = 47;
            this.cboDepart.TabStop = true;
            this.cboDepart.Text = "科室 （本科室病历缺陷查询）";
            this.cboDepart.UseVisualStyleBackColor = true;
            // 
            // CSelectP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(448, 214);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnfindExit);
            this.Controls.Add(this.btnReport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CSelectP";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "现岗查询";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnReport;
        private DevComponents.DotNetBar.ButtonX btnfindExit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton cboHz;
        private System.Windows.Forms.RadioButton cboDepart;
        private System.Windows.Forms.RadioButton cboQy;
    }
}