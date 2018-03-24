namespace UserInterface
{
    partial class TableInfo
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
            this.iniRow = new DevComponents.Editors.IntegerInput();
            this.iniColumn = new DevComponents.Editors.IntegerInput();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.doubleInput1 = new DevComponents.Editors.DoubleInput();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.checkBoxX1 = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.btnOK = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.iniRow);
            this.groupBox1.Controls.Add(this.iniColumn);
            this.groupBox1.Controls.Add(this.labelX2);
            this.groupBox1.Controls.Add(this.labelX1);
            this.groupBox1.Location = new System.Drawing.Point(7, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(204, 79);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "表格尺寸";
            // 
            // iniRow
            // 
            // 
            // 
            // 
            this.iniRow.BackgroundStyle.Class = "DateTimeInputBackground";
            this.iniRow.Location = new System.Drawing.Point(84, 46);
            this.iniRow.MaxValue = 20;
            this.iniRow.MinValue = 1;
            this.iniRow.Name = "iniRow";
            this.iniRow.ShowUpDown = true;
            this.iniRow.Size = new System.Drawing.Size(80, 21);
            this.iniRow.TabIndex = 3;
            this.iniRow.Value = 2;
            // 
            // iniColumn
            // 
            // 
            // 
            // 
            this.iniColumn.BackgroundStyle.Class = "DateTimeInputBackground";
            this.iniColumn.Location = new System.Drawing.Point(84, 20);
            this.iniColumn.MaxValue = 20;
            this.iniColumn.MinValue = 1;
            this.iniColumn.Name = "iniColumn";
            this.iniColumn.ShowUpDown = true;
            this.iniColumn.Size = new System.Drawing.Size(80, 21);
            this.iniColumn.TabIndex = 2;
            this.iniColumn.Value = 5;
            // 
            // labelX2
            // 
            this.labelX2.Location = new System.Drawing.Point(15, 46);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(44, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "行数：";
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(15, 20);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(44, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "列数：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.doubleInput1);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(7, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(204, 70);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "表格尺寸";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.ForeColor = System.Drawing.Color.MidnightBlue;
            this.radioButton2.Location = new System.Drawing.Point(5, 44);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(119, 16);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "根据内容调整表格";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // doubleInput1
            // 
            // 
            // 
            // 
            this.doubleInput1.BackgroundStyle.Class = "DateTimeInputBackground";
            this.doubleInput1.Enabled = false;
            this.doubleInput1.Increment = 1D;
            this.doubleInput1.Location = new System.Drawing.Point(84, 20);
            this.doubleInput1.LockUpdateChecked = true;
            this.doubleInput1.Name = "doubleInput1";
            this.doubleInput1.ShowUpDown = true;
            this.doubleInput1.Size = new System.Drawing.Size(80, 21);
            this.doubleInput1.TabIndex = 1;
            this.doubleInput1.Value = 0D;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.radioButton1.Location = new System.Drawing.Point(5, 22);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(83, 16);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "固定列宽：";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // checkBoxX1
            // 
            this.checkBoxX1.Location = new System.Drawing.Point(7, 162);
            this.checkBoxX1.Name = "checkBoxX1";
            this.checkBoxX1.Size = new System.Drawing.Size(129, 23);
            this.checkBoxX1.TabIndex = 2;
            this.checkBoxX1.Text = "为新表格保存记忆";
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOK.Location = new System.Drawing.Point(97, 191);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "确定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.Location = new System.Drawing.Point(157, 191);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(54, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // TableInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(222, 224);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.checkBoxX1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TableInfo";
            this.ShowInTaskbar = false;
            this.Text = "插入表格";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private DevComponents.Editors.IntegerInput iniRow;
        private DevComponents.Editors.IntegerInput iniColumn;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevComponents.DotNetBar.Controls.CheckBoxX checkBoxX1;
        private DevComponents.DotNetBar.ButtonX btnOK;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private System.Windows.Forms.RadioButton radioButton2;
        private DevComponents.Editors.DoubleInput doubleInput1;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}