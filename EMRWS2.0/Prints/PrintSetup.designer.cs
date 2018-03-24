namespace UserInterface
{
    partial class PrintSetup
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.gbxprint = new System.Windows.Forms.GroupBox();
            this.FindPrinter = new DevComponents.DotNetBar.ButtonX();
            this.BtnAttribute = new DevComponents.DotNetBar.ButtonX();
            this.DefaultPrint = new System.Windows.Forms.CheckBox();
            this.cbxHand = new System.Windows.Forms.CheckBox();
            this.cbxPrintToFile = new System.Windows.Forms.CheckBox();
            this.PortName = new System.Windows.Forms.Label();
            this.DriverName = new System.Windows.Forms.Label();
            this.PrinterStatus = new System.Windows.Forms.Label();
            this.LQPrinter = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btCopyExam = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.label14 = new System.Windows.Forms.Label();
            this.tbPage = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.rbArea = new System.Windows.Forms.RadioButton();
            this.rbNow = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nudCopy = new System.Windows.Forms.NumericUpDown();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.printContent = new System.Windows.Forms.ComboBox();
            this.print = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxZoom = new System.Windows.Forms.ComboBox();
            this.cbxFormat = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnOn = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.gbxprint.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCopy)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxprint
            // 
            this.gbxprint.Controls.Add(this.FindPrinter);
            this.gbxprint.Controls.Add(this.BtnAttribute);
            this.gbxprint.Controls.Add(this.DefaultPrint);
            this.gbxprint.Controls.Add(this.cbxHand);
            this.gbxprint.Controls.Add(this.cbxPrintToFile);
            this.gbxprint.Controls.Add(this.PortName);
            this.gbxprint.Controls.Add(this.DriverName);
            this.gbxprint.Controls.Add(this.PrinterStatus);
            this.gbxprint.Controls.Add(this.LQPrinter);
            this.gbxprint.Controls.Add(this.label5);
            this.gbxprint.Controls.Add(this.label4);
            this.gbxprint.Controls.Add(this.label3);
            this.gbxprint.Controls.Add(this.label2);
            this.gbxprint.Controls.Add(this.label1);
            this.gbxprint.Controls.Add(this.btCopyExam);
            this.gbxprint.Location = new System.Drawing.Point(6, 6);
            this.gbxprint.Name = "gbxprint";
            this.gbxprint.Size = new System.Drawing.Size(573, 111);
            this.gbxprint.TabIndex = 13;
            this.gbxprint.TabStop = false;
            this.gbxprint.Text = "打印机";
            // 
            // FindPrinter
            // 
            this.FindPrinter.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.FindPrinter.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.FindPrinter.Font = new System.Drawing.Font("宋体", 9F);
            this.FindPrinter.Location = new System.Drawing.Point(450, 46);
            this.FindPrinter.Name = "FindPrinter";
            this.FindPrinter.Size = new System.Drawing.Size(117, 23);
            this.FindPrinter.TabIndex = 63;
            this.FindPrinter.Text = "查找打印机(D)...";
            // 
            // BtnAttribute
            // 
            this.BtnAttribute.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnAttribute.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.BtnAttribute.Font = new System.Drawing.Font("宋体", 9F);
            this.BtnAttribute.Location = new System.Drawing.Point(450, 20);
            this.BtnAttribute.Name = "BtnAttribute";
            this.BtnAttribute.Size = new System.Drawing.Size(117, 23);
            this.BtnAttribute.TabIndex = 62;
            this.BtnAttribute.Text = " 属性(P)";
            // 
            // DefaultPrint
            // 
            this.DefaultPrint.AutoSize = true;
            this.DefaultPrint.Checked = true;
            this.DefaultPrint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DefaultPrint.Location = new System.Drawing.Point(65, 9);
            this.DefaultPrint.Name = "DefaultPrint";
            this.DefaultPrint.Size = new System.Drawing.Size(108, 16);
            this.DefaultPrint.TabIndex = 60;
            this.DefaultPrint.Text = "使用默认打印机";
            this.DefaultPrint.UseVisualStyleBackColor = true;
            this.DefaultPrint.CheckedChanged += new System.EventHandler(this.DefaultPrint_CheckedChanged);
            // 
            // cbxHand
            // 
            this.cbxHand.AutoSize = true;
            this.cbxHand.Location = new System.Drawing.Point(447, 90);
            this.cbxHand.Name = "cbxHand";
            this.cbxHand.Size = new System.Drawing.Size(114, 16);
            this.cbxHand.TabIndex = 20;
            this.cbxHand.Text = "手动双面打印(X)";
            this.cbxHand.UseVisualStyleBackColor = true;
            this.cbxHand.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // cbxPrintToFile
            // 
            this.cbxPrintToFile.AutoSize = true;
            this.cbxPrintToFile.Location = new System.Drawing.Point(447, 72);
            this.cbxPrintToFile.Name = "cbxPrintToFile";
            this.cbxPrintToFile.Size = new System.Drawing.Size(102, 16);
            this.cbxPrintToFile.TabIndex = 19;
            this.cbxPrintToFile.Text = "打印到文件(L)";
            this.cbxPrintToFile.UseVisualStyleBackColor = true;
            this.cbxPrintToFile.CheckedChanged += new System.EventHandler(this.cbxPrintToFile_CheckedChanged);
            // 
            // PortName
            // 
            this.PortName.AutoSize = true;
            this.PortName.Location = new System.Drawing.Point(63, 76);
            this.PortName.Name = "PortName";
            this.PortName.Size = new System.Drawing.Size(0, 12);
            this.PortName.TabIndex = 18;
            // 
            // DriverName
            // 
            this.DriverName.AutoSize = true;
            this.DriverName.Location = new System.Drawing.Point(63, 61);
            this.DriverName.Name = "DriverName";
            this.DriverName.Size = new System.Drawing.Size(0, 12);
            this.DriverName.TabIndex = 17;
            // 
            // PrinterStatus
            // 
            this.PrinterStatus.AutoSize = true;
            this.PrinterStatus.Location = new System.Drawing.Point(63, 46);
            this.PrinterStatus.Name = "PrinterStatus";
            this.PrinterStatus.Size = new System.Drawing.Size(0, 12);
            this.PrinterStatus.TabIndex = 14;
            // 
            // LQPrinter
            // 
            this.LQPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LQPrinter.Enabled = false;
            this.LQPrinter.FormattingEnabled = true;
            this.LQPrinter.Location = new System.Drawing.Point(65, 25);
            this.LQPrinter.Name = "LQPrinter";
            this.LQPrinter.Size = new System.Drawing.Size(360, 20);
            this.LQPrinter.TabIndex = 13;
            this.LQPrinter.SelectedIndexChanged += new System.EventHandler(this.printName_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "备注：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "位置：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "类型：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "状态：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "名称(N):";
            // 
            // btCopyExam
            // 
            this.btCopyExam.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btCopyExam.Location = new System.Drawing.Point(941, 19);
            this.btCopyExam.Name = "btCopyExam";
            this.btCopyExam.Size = new System.Drawing.Size(48, 30);
            this.btCopyExam.TabIndex = 7;
            this.btCopyExam.Text = "使用";
            this.btCopyExam.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.tbPage);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.rbArea);
            this.groupBox1.Controls.Add(this.rbNow);
            this.groupBox1.Controls.Add(this.rbAll);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(5, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 127);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "页面范围";
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Enabled = false;
            this.radioButton4.Location = new System.Drawing.Point(141, 34);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(89, 16);
            this.radioButton4.TabIndex = 19;
            this.radioButton4.Text = "所选内容(S)";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 92);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(251, 12);
            this.label14.TabIndex = 16;
            this.label14.Text = "从文档或节的开头算起），例如: 1 , 3 , 3-5";
            // 
            // tbPage
            // 
            this.tbPage.Location = new System.Drawing.Point(110, 52);
            this.tbPage.Name = "tbPage";
            this.tbPage.Size = new System.Drawing.Size(170, 21);
            this.tbPage.TabIndex = 18;
            this.tbPage.TextChanged += new System.EventHandler(this.tbPage_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 76);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(227, 12);
            this.label12.TabIndex = 14;
            this.label12.Text = "请键入页码和/或页码范围（用逗号分隔，";
            // 
            // rbArea
            // 
            this.rbArea.AutoSize = true;
            this.rbArea.Location = new System.Drawing.Point(9, 54);
            this.rbArea.Name = "rbArea";
            this.rbArea.Size = new System.Drawing.Size(95, 16);
            this.rbArea.TabIndex = 17;
            this.rbArea.Text = "页码范围(G):";
            this.rbArea.UseVisualStyleBackColor = true;
            this.rbArea.CheckedChanged += new System.EventHandler(this.rbArea_CheckedChanged);
            // 
            // rbNow
            // 
            this.rbNow.AutoSize = true;
            this.rbNow.Location = new System.Drawing.Point(9, 34);
            this.rbNow.Name = "rbNow";
            this.rbNow.Size = new System.Drawing.Size(77, 16);
            this.rbNow.TabIndex = 16;
            this.rbNow.Text = "当前页(E)";
            this.rbNow.UseVisualStyleBackColor = true;
            this.rbNow.CheckedChanged += new System.EventHandler(this.rbNow_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(9, 15);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(65, 16);
            this.rbAll.TabIndex = 15;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "全部(A)";
            this.rbAll.UseVisualStyleBackColor = true;
            this.rbAll.CheckedChanged += new System.EventHandler(this.rbAll_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(396, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "label7";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(445, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "label6";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.nudCopy);
            this.groupBox2.Controls.Add(this.checkBox3);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Location = new System.Drawing.Point(299, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(280, 127);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "副本";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EMR.Properties.Resources._1;
            this.pictureBox1.Location = new System.Drawing.Point(32, 51);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(109, 66);
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // nudCopy
            // 
            this.nudCopy.Location = new System.Drawing.Point(66, 18);
            this.nudCopy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCopy.Name = "nudCopy";
            this.nudCopy.Size = new System.Drawing.Size(120, 21);
            this.nudCopy.TabIndex = 23;
            this.nudCopy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(147, 67);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(90, 16);
            this.checkBox3.TabIndex = 22;
            this.checkBox3.Text = "逐份打印(T)";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 13;
            this.label11.Text = "分数(C)：";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 252);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 12);
            this.label15.TabIndex = 16;
            this.label15.Text = "打印内容(W):";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 275);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 12);
            this.label16.TabIndex = 17;
            this.label16.Text = "打印(R):";
            // 
            // printContent
            // 
            this.printContent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.printContent.FormattingEnabled = true;
            this.printContent.Items.AddRange(new object[] {
            "文档"});
            this.printContent.Location = new System.Drawing.Point(105, 248);
            this.printContent.Name = "printContent";
            this.printContent.Size = new System.Drawing.Size(188, 20);
            this.printContent.TabIndex = 20;
            // 
            // print
            // 
            this.print.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.print.FormattingEnabled = true;
            this.print.Items.AddRange(new object[] {
            "范围内的所有页",
            "奇数页",
            "偶数页"});
            this.print.Location = new System.Drawing.Point(105, 271);
            this.print.Name = "print";
            this.print.Size = new System.Drawing.Size(188, 20);
            this.print.TabIndex = 21;
            this.print.SelectedIndexChanged += new System.EventHandler(this.print_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxZoom);
            this.groupBox3.Controls.Add(this.cbxFormat);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Location = new System.Drawing.Point(299, 248);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(280, 70);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "缩放";
            // 
            // cbxZoom
            // 
            this.cbxZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxZoom.FormattingEnabled = true;
            this.cbxZoom.Items.AddRange(new object[] {
            "无缩放",
            "信纸",
            "Tabloid",
            "法律专用纸",
            "Statement",
            "A3"});
            this.cbxZoom.Location = new System.Drawing.Point(116, 44);
            this.cbxZoom.Name = "cbxZoom";
            this.cbxZoom.Size = new System.Drawing.Size(147, 20);
            this.cbxZoom.TabIndex = 22;
            // 
            // cbxFormat
            // 
            this.cbxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFormat.FormattingEnabled = true;
            this.cbxFormat.Items.AddRange(new object[] {
            "1版",
            "2版",
            "4版",
            "6版",
            "8版",
            "16版"});
            this.cbxFormat.Location = new System.Drawing.Point(116, 18);
            this.cbxFormat.Name = "cbxFormat";
            this.cbxFormat.Size = new System.Drawing.Size(147, 20);
            this.cbxFormat.TabIndex = 21;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 45);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(113, 12);
            this.label18.TabIndex = 1;
            this.label18.Text = "按纸张大小缩放(Z):";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 21);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(89, 12);
            this.label17.TabIndex = 0;
            this.label17.Text = "每页的版数(H):";
            // 
            // btnOn
            // 
            this.btnOn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOn.Font = new System.Drawing.Font("宋体", 9F);
            this.btnOn.Location = new System.Drawing.Point(410, 322);
            this.btnOn.Name = "btnOn";
            this.btnOn.Size = new System.Drawing.Size(75, 23);
            this.btnOn.TabIndex = 61;
            this.btnOn.Text = "确定";
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.Font = new System.Drawing.Font("宋体", 9F);
            this.btnCancel.Location = new System.Drawing.Point(504, 322);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 62;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PrintSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 351);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOn);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.print);
            this.Controls.Add(this.printContent);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbxprint);
            this.MaximizeBox = false;
            this.Name = "PrintSetup";
            this.ShowIcon = false;
            this.Text = "打印";
            this.Load += new System.EventHandler(this.LQContiPrint_Load);
            this.gbxprint.ResumeLayout(false);
            this.gbxprint.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCopy)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxprint;
        private System.Windows.Forms.Button btCopyExam;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox LQPrinter;
        private System.Windows.Forms.CheckBox cbxHand;
        private System.Windows.Forms.CheckBox cbxPrintToFile;
        private System.Windows.Forms.Label PortName;
        private System.Windows.Forms.Label DriverName;
        private System.Windows.Forms.Label PrinterStatus;
        private System.Windows.Forms.RadioButton rbArea;
        private System.Windows.Forms.RadioButton rbNow;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.TextBox tbPage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox print;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbxZoom;
        private System.Windows.Forms.ComboBox cbxFormat;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown nudCopy;
        private System.Windows.Forms.ComboBox printContent;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox DefaultPrint;
        private DevComponents.DotNetBar.ButtonX btnOn;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.ButtonX FindPrinter;
        private DevComponents.DotNetBar.ButtonX BtnAttribute;
    }
}