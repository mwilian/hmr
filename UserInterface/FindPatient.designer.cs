namespace UserInterface
{
    partial class FindPatient
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cbxIn = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.dtpTime2 = new System.Windows.Forms.DateTimePicker();
            this.label15 = new System.Windows.Forms.Label();
            this.dtpTime1 = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFind2 = new DevComponents.DotNetBar.ButtonX();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cbxDepart = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.rbChief = new System.Windows.Forms.RadioButton();
            this.cbxDoctor = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.rbAttending = new System.Windows.Forms.RadioButton();
            this.rbHouse = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbPName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.rbBoth = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.rbFemale = new System.Windows.Forms.RadioButton();
            this.rbMale = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.dtBegin = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAllPatientName = new System.Windows.Forms.TextBox();
            this.btnFind1 = new DevComponents.DotNetBar.ButtonX();
            this.tbPatientName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbOutRegistryID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbArchiveNum = new System.Windows.Forms.TextBox();
            this.lbRegistryID = new System.Windows.Forms.Label();
            this.tbRegistryID = new System.Windows.Forms.TextBox();
            this.panelEx1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.Controls.Add(this.groupBox8);
            this.panelEx1.Controls.Add(this.groupBox1);
            this.panelEx1.Controls.Add(this.groupBox3);
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(534, 359);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.Color = System.Drawing.Color.AliceBlue;
            this.panelEx1.Style.BackColor2.Color = System.Drawing.Color.LightSteelBlue;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 0;
            this.panelEx1.VisibleChanged += new System.EventHandler(this.panelEx1_VisibleChanged);
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox8.Controls.Add(this.cbxIn);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.dtpTime2);
            this.groupBox8.Controls.Add(this.label15);
            this.groupBox8.Controls.Add(this.dtpTime1);
            this.groupBox8.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox8.ForeColor = System.Drawing.Color.Gray;
            this.groupBox8.Location = new System.Drawing.Point(3, 25);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(255, 99);
            this.groupBox8.TabIndex = 62;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "时间范围";
            this.groupBox8.Visible = false;
            // 
            // cbxIn
            // 
            this.cbxIn.AutoSize = true;
            this.cbxIn.Location = new System.Drawing.Point(18, 19);
            this.cbxIn.Name = "cbxIn";
            this.cbxIn.Size = new System.Drawing.Size(82, 18);
            this.cbxIn.TabIndex = 57;
            this.cbxIn.Text = "入院时间";
            this.cbxIn.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label14.Location = new System.Drawing.Point(14, 71);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 14);
            this.label14.TabIndex = 56;
            this.label14.Text = "截至日期：";
            // 
            // dtpTime2
            // 
            this.dtpTime2.Font = new System.Drawing.Font("宋体", 10.5F);
            this.dtpTime2.Location = new System.Drawing.Point(98, 67);
            this.dtpTime2.Name = "dtpTime2";
            this.dtpTime2.Size = new System.Drawing.Size(150, 23);
            this.dtpTime2.TabIndex = 55;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label15.Location = new System.Drawing.Point(14, 43);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 14);
            this.label15.TabIndex = 54;
            this.label15.Text = "开始日期：";
            // 
            // dtpTime1
            // 
            this.dtpTime1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.dtpTime1.Location = new System.Drawing.Point(98, 39);
            this.dtpTime1.Name = "dtpTime1";
            this.dtpTime1.Size = new System.Drawing.Size(150, 23);
            this.dtpTime1.TabIndex = 53;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnFind2);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox1.Location = new System.Drawing.Point(264, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 350);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "复合检索(出院)";
            // 
            // btnFind2
            // 
            this.btnFind2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnFind2.BackColor = System.Drawing.Color.Transparent;
            this.btnFind2.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.btnFind2.Location = new System.Drawing.Point(183, 284);
            this.btnFind2.Name = "btnFind2";
            this.btnFind2.Size = new System.Drawing.Size(74, 23);
            this.btnFind2.TabIndex = 65;
            this.btnFind2.Text = "检索";
            this.btnFind2.Click += new System.EventHandler(this.btnFind2_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.cbxDepart);
            this.groupBox6.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox6.ForeColor = System.Drawing.Color.Gray;
            this.groupBox6.Location = new System.Drawing.Point(5, 232);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(255, 43);
            this.groupBox6.TabIndex = 64;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "科室";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label12.Location = new System.Drawing.Point(6, 17);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 14);
            this.label12.TabIndex = 65;
            this.label12.Text = "住院科室：";
            // 
            // cbxDepart
            // 
            this.cbxDepart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDepart.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cbxDepart.FormattingEnabled = true;
            this.cbxDepart.ItemHeight = 14;
            this.cbxDepart.Location = new System.Drawing.Point(98, 13);
            this.cbxDepart.MaxDropDownItems = 20;
            this.cbxDepart.Name = "cbxDepart";
            this.cbxDepart.Size = new System.Drawing.Size(149, 22);
            this.cbxDepart.TabIndex = 64;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.rbChief);
            this.groupBox5.Controls.Add(this.cbxDoctor);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.rbAttending);
            this.groupBox5.Controls.Add(this.rbHouse);
            this.groupBox5.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox5.ForeColor = System.Drawing.Color.Gray;
            this.groupBox5.Location = new System.Drawing.Point(6, 159);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(255, 66);
            this.groupBox5.TabIndex = 58;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "医师姓名";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Location = new System.Drawing.Point(37, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 14);
            this.label9.TabIndex = 65;
            this.label9.Text = "姓名：";
            // 
            // rbChief
            // 
            this.rbChief.AutoSize = true;
            this.rbChief.Font = new System.Drawing.Font("宋体", 10.5F);
            this.rbChief.ForeColor = System.Drawing.Color.DimGray;
            this.rbChief.Location = new System.Drawing.Point(199, 39);
            this.rbChief.Name = "rbChief";
            this.rbChief.Size = new System.Drawing.Size(53, 18);
            this.rbChief.TabIndex = 62;
            this.rbChief.TabStop = true;
            this.rbChief.Text = "主任";
            this.rbChief.UseVisualStyleBackColor = true;
            // 
            // cbxDoctor
            // 
            this.cbxDoctor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDoctor.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cbxDoctor.FormattingEnabled = true;
            this.cbxDoctor.ItemHeight = 14;
            this.cbxDoctor.Location = new System.Drawing.Point(98, 13);
            this.cbxDoctor.MaxDropDownItems = 20;
            this.cbxDoctor.Name = "cbxDoctor";
            this.cbxDoctor.Size = new System.Drawing.Size(149, 22);
            this.cbxDoctor.TabIndex = 64;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Location = new System.Drawing.Point(37, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(49, 14);
            this.label11.TabIndex = 59;
            this.label11.Text = "级别：";
            // 
            // rbAttending
            // 
            this.rbAttending.AutoSize = true;
            this.rbAttending.Font = new System.Drawing.Font("宋体", 10.5F);
            this.rbAttending.ForeColor = System.Drawing.Color.DimGray;
            this.rbAttending.Location = new System.Drawing.Point(150, 39);
            this.rbAttending.Name = "rbAttending";
            this.rbAttending.Size = new System.Drawing.Size(53, 18);
            this.rbAttending.TabIndex = 61;
            this.rbAttending.TabStop = true;
            this.rbAttending.Text = "主治";
            this.rbAttending.UseVisualStyleBackColor = true;
            // 
            // rbHouse
            // 
            this.rbHouse.AutoSize = true;
            this.rbHouse.Font = new System.Drawing.Font("宋体", 10.5F);
            this.rbHouse.ForeColor = System.Drawing.Color.DimGray;
            this.rbHouse.Location = new System.Drawing.Point(98, 39);
            this.rbHouse.Name = "rbHouse";
            this.rbHouse.Size = new System.Drawing.Size(53, 18);
            this.rbHouse.TabIndex = 60;
            this.rbHouse.TabStop = true;
            this.rbHouse.Text = "住院";
            this.rbHouse.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbPName);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.rbBoth);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.rbFemale);
            this.groupBox4.Controls.Add(this.rbMale);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox4.ForeColor = System.Drawing.Color.Gray;
            this.groupBox4.Location = new System.Drawing.Point(5, 88);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(255, 67);
            this.groupBox4.TabIndex = 57;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "患者姓名";
            // 
            // tbPName
            // 
            this.tbPName.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tbPName.Location = new System.Drawing.Point(96, 13);
            this.tbPName.MaxLength = 20;
            this.tbPName.Name = "tbPName";
            this.tbPName.Size = new System.Drawing.Size(153, 23);
            this.tbPName.TabIndex = 64;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(-1, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 14);
            this.label10.TabIndex = 63;
            this.label10.Text = "姓名拼音字头：";
            // 
            // rbBoth
            // 
            this.rbBoth.AutoSize = true;
            this.rbBoth.Font = new System.Drawing.Font("宋体", 10.5F);
            this.rbBoth.ForeColor = System.Drawing.Color.DimGray;
            this.rbBoth.Location = new System.Drawing.Point(199, 41);
            this.rbBoth.Name = "rbBoth";
            this.rbBoth.Size = new System.Drawing.Size(53, 18);
            this.rbBoth.TabIndex = 62;
            this.rbBoth.TabStop = true;
            this.rbBoth.Text = "不详";
            this.rbBoth.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(37, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 14);
            this.label8.TabIndex = 59;
            this.label8.Text = "性别：";
            // 
            // rbFemale
            // 
            this.rbFemale.AutoSize = true;
            this.rbFemale.Font = new System.Drawing.Font("宋体", 10.5F);
            this.rbFemale.ForeColor = System.Drawing.Color.DimGray;
            this.rbFemale.Location = new System.Drawing.Point(153, 41);
            this.rbFemale.Name = "rbFemale";
            this.rbFemale.Size = new System.Drawing.Size(39, 18);
            this.rbFemale.TabIndex = 61;
            this.rbFemale.TabStop = true;
            this.rbFemale.Text = "女";
            this.rbFemale.UseVisualStyleBackColor = true;
            // 
            // rbMale
            // 
            this.rbMale.AutoSize = true;
            this.rbMale.Font = new System.Drawing.Font("宋体", 10.5F);
            this.rbMale.ForeColor = System.Drawing.Color.DimGray;
            this.rbMale.Location = new System.Drawing.Point(101, 41);
            this.rbMale.Name = "rbMale";
            this.rbMale.Size = new System.Drawing.Size(39, 18);
            this.rbMale.TabIndex = 60;
            this.rbMale.TabStop = true;
            this.rbMale.Text = "男";
            this.rbMale.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.dtEnd);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.dtBegin);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox2.ForeColor = System.Drawing.Color.Gray;
            this.groupBox2.Location = new System.Drawing.Point(5, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 70);
            this.groupBox2.TabIndex = 54;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "出院时间";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(7, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 14);
            this.label7.TabIndex = 1;
            this.label7.Text = "截至日期：";
            // 
            // dtEnd
            // 
            this.dtEnd.Font = new System.Drawing.Font("宋体", 10.5F);
            this.dtEnd.Location = new System.Drawing.Point(98, 41);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(150, 23);
            this.dtEnd.TabIndex = 55;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(7, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 54;
            this.label6.Text = "开始日期：";
            // 
            // dtBegin
            // 
            this.dtBegin.Font = new System.Drawing.Font("宋体", 10.5F);
            this.dtBegin.Location = new System.Drawing.Point(98, 15);
            this.dtBegin.Name = "dtBegin";
            this.dtBegin.Size = new System.Drawing.Size(150, 23);
            this.dtBegin.TabIndex = 53;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtAllPatientName);
            this.groupBox3.Controls.Add(this.btnFind1);
            this.groupBox3.Controls.Add(this.tbPatientName);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.tbOutRegistryID);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tbArchiveNum);
            this.groupBox3.Controls.Add(this.lbRegistryID);
            this.groupBox3.Controls.Add(this.tbRegistryID);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox3.Location = new System.Drawing.Point(3, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(255, 350);
            this.groupBox3.TabIndex = 56;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "简单检索(全院)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(6, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 14);
            this.label3.TabIndex = 61;
            this.label3.Text = "患者姓名:";
            // 
            // txtAllPatientName
            // 
            this.txtAllPatientName.Font = new System.Drawing.Font("宋体", 10.5F);
            this.txtAllPatientName.Location = new System.Drawing.Point(78, 278);
            this.txtAllPatientName.MaxLength = 20;
            this.txtAllPatientName.Name = "txtAllPatientName";
            this.txtAllPatientName.Size = new System.Drawing.Size(153, 23);
            this.txtAllPatientName.TabIndex = 60;
            // 
            // btnFind1
            // 
            this.btnFind1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnFind1.BackColor = System.Drawing.SystemColors.Control;
            this.btnFind1.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.btnFind1.Location = new System.Drawing.Point(169, 307);
            this.btnFind1.Name = "btnFind1";
            this.btnFind1.Size = new System.Drawing.Size(78, 23);
            this.btnFind1.TabIndex = 59;
            this.btnFind1.Text = "检索";
            this.btnFind1.Click += new System.EventHandler(this.btnFind1_Click);
            this.btnFind1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.btnFind1_KeyPress);
            // 
            // tbPatientName
            // 
            this.tbPatientName.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tbPatientName.Location = new System.Drawing.Point(79, 249);
            this.tbPatientName.MaxLength = 20;
            this.tbPatientName.Name = "tbPatientName";
            this.tbPatientName.Size = new System.Drawing.Size(153, 23);
            this.tbPatientName.TabIndex = 58;
            this.tbPatientName.TextChanged += new System.EventHandler(this.tbPatientName_TextChanged);
            this.tbPatientName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPatientName_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(13, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 14);
            this.label2.TabIndex = 57;
            this.label2.Text = "患者姓名：(拼音字头)";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // tbOutRegistryID
            // 
            this.tbOutRegistryID.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tbOutRegistryID.Location = new System.Drawing.Point(79, 202);
            this.tbOutRegistryID.Name = "tbOutRegistryID";
            this.tbOutRegistryID.Size = new System.Drawing.Size(154, 23);
            this.tbOutRegistryID.TabIndex = 47;
            this.tbOutRegistryID.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(13, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 14);
            this.label5.TabIndex = 48;
            this.label5.Text = "门诊号：";
            this.label5.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(12, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 37;
            this.label1.Text = "病案号：";
            // 
            // tbArchiveNum
            // 
            this.tbArchiveNum.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tbArchiveNum.Location = new System.Drawing.Point(78, 136);
            this.tbArchiveNum.Name = "tbArchiveNum";
            this.tbArchiveNum.Size = new System.Drawing.Size(154, 23);
            this.tbArchiveNum.TabIndex = 38;
            this.tbArchiveNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbArchiveNum_KeyPress);
            // 
            // lbRegistryID
            // 
            this.lbRegistryID.AutoSize = true;
            this.lbRegistryID.Font = new System.Drawing.Font("宋体", 10.5F);
            this.lbRegistryID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbRegistryID.Location = new System.Drawing.Point(13, 174);
            this.lbRegistryID.Name = "lbRegistryID";
            this.lbRegistryID.Size = new System.Drawing.Size(63, 14);
            this.lbRegistryID.TabIndex = 42;
            this.lbRegistryID.Text = "住院号：";
            // 
            // tbRegistryID
            // 
            this.tbRegistryID.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tbRegistryID.Location = new System.Drawing.Point(78, 169);
            this.tbRegistryID.Name = "tbRegistryID";
            this.tbRegistryID.Size = new System.Drawing.Size(154, 23);
            this.tbRegistryID.TabIndex = 39;
            this.tbRegistryID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbRegistryID_KeyPress);
            // 
            // FindPatient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelEx1);
            this.Name = "FindPatient";
            this.Size = new System.Drawing.Size(534, 359);
            this.panelEx1.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.PanelEx panelEx1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbPatientName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbOutRegistryID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbArchiveNum;
        private System.Windows.Forms.Label lbRegistryID;
        private System.Windows.Forms.TextBox tbRegistryID;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbxDepart;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rbChief;
        private System.Windows.Forms.ComboBox cbxDoctor;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton rbAttending;
        private System.Windows.Forms.RadioButton rbHouse;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbPName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton rbBoth;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rbFemale;
        private System.Windows.Forms.RadioButton rbMale;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtEnd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtBegin;
        private DevComponents.DotNetBar.ButtonX btnFind1;
        private DevComponents.DotNetBar.ButtonX btnFind2;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox cbxIn;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DateTimePicker dtpTime2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DateTimePicker dtpTime1;
        private System.Windows.Forms.TextBox txtAllPatientName;
        private System.Windows.Forms.Label label3;
    }
}
