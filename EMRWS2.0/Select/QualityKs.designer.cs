namespace EMR
{
    partial class Qualityks
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Qualityks));
            this.dgvQuality = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.cboDoctor = new System.Windows.Forms.ComboBox();
            this.dd = new System.Windows.Forms.Label();
            this.cboEmrNote = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dpendTime = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.cboDepart = new System.Windows.Forms.ComboBox();
            this.patient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.noteName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.writenTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.limit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuality)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvQuality
            // 
            this.dgvQuality.AllowUserToDeleteRows = false;
            this.dgvQuality.AllowUserToResizeRows = false;
            this.dgvQuality.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvQuality.BackgroundColor = System.Drawing.Color.White;
            this.dgvQuality.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvQuality.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvQuality.ColumnHeadersHeight = 28;
            this.dgvQuality.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvQuality.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.patient,
            this.noteName,
            this.startTime,
            this.writenTime,
            this.limit,
            this.Rest,
            this.score});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.AntiqueWhite;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvQuality.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvQuality.Font = new System.Drawing.Font("宋体", 10.5F);
            this.dgvQuality.Location = new System.Drawing.Point(-9, 49);
            this.dgvQuality.MultiSelect = false;
            this.dgvQuality.Name = "dgvQuality";
            this.dgvQuality.ReadOnly = true;
            this.dgvQuality.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvQuality.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvQuality.RowHeadersWidth = 30;
            this.dgvQuality.RowTemplate.Height = 23;
            this.dgvQuality.RowTemplate.ReadOnly = true;
            this.dgvQuality.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvQuality.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvQuality.Size = new System.Drawing.Size(960, 451);
            this.dgvQuality.TabIndex = 26;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtpStart);
            this.groupBox1.Controls.Add(this.cboDoctor);
            this.groupBox1.Controls.Add(this.dd);
            this.groupBox1.Controls.Add(this.cboEmrNote);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dpendTime);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Controls.Add(this.cboDepart);
            this.groupBox1.Location = new System.Drawing.Point(10, -2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(910, 45);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "科室：";
            // 
            // dtpStart
            // 
            this.dtpStart.Location = new System.Drawing.Point(556, 14);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(102, 21);
            this.dtpStart.TabIndex = 21;
            // 
            // cboDoctor
            // 
            this.cboDoctor.FormattingEnabled = true;
            this.cboDoctor.Location = new System.Drawing.Point(207, 14);
            this.cboDoctor.Name = "cboDoctor";
            this.cboDoctor.Size = new System.Drawing.Size(91, 20);
            this.cboDoctor.TabIndex = 9;
            // 
            // dd
            // 
            this.dd.AutoSize = true;
            this.dd.Location = new System.Drawing.Point(491, 19);
            this.dd.Name = "dd";
            this.dd.Size = new System.Drawing.Size(59, 12);
            this.dd.TabIndex = 20;
            this.dd.Text = "入科时间:";
            // 
            // cboEmrNote
            // 
            this.cboEmrNote.FormattingEnabled = true;
            this.cboEmrNote.Location = new System.Drawing.Point(383, 14);
            this.cboEmrNote.Name = "cboEmrNote";
            this.cboEmrNote.Size = new System.Drawing.Size(93, 20);
            this.cboEmrNote.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(669, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "至";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "医师：";
            // 
            // dpendTime
            // 
            this.dpendTime.Location = new System.Drawing.Point(700, 14);
            this.dpendTime.Name = "dpendTime";
            this.dpendTime.Size = new System.Drawing.Size(102, 21);
            this.dpendTime.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(312, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "病历模板：";
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(813, 10);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(64, 23);
            this.btnSelect.TabIndex = 16;
            this.btnSelect.Text = "查询";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // cboDepart
            // 
            this.cboDepart.FormattingEnabled = true;
            this.cboDepart.Location = new System.Drawing.Point(53, 14);
            this.cboDepart.Name = "cboDepart";
            this.cboDepart.Size = new System.Drawing.Size(94, 20);
            this.cboDepart.TabIndex = 13;
            this.cboDepart.SelectedIndexChanged += new System.EventHandler(this.cboDepart_SelectedIndexChanged);
            // 
            // patient
            // 
            this.patient.HeaderText = "患者姓名";
            this.patient.Name = "patient";
            this.patient.ReadOnly = true;
            this.patient.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // noteName
            // 
            this.noteName.HeaderText = "记录名称";
            this.noteName.Name = "noteName";
            this.noteName.ReadOnly = true;
            this.noteName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // startTime
            // 
            this.startTime.HeaderText = "计时开始";
            this.startTime.Name = "startTime";
            this.startTime.ReadOnly = true;
            this.startTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // writenTime
            // 
            this.writenTime.HeaderText = "完成时间";
            this.writenTime.Name = "writenTime";
            this.writenTime.ReadOnly = true;
            // 
            // limit
            // 
            this.limit.HeaderText = "要求时限";
            this.limit.Name = "limit";
            this.limit.ReadOnly = true;
            this.limit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Rest
            // 
            this.Rest.HeaderText = "超时时间";
            this.Rest.Name = "Rest";
            this.Rest.ReadOnly = true;
            // 
            // score
            // 
            this.score.HeaderText = "评估";
            this.score.Name = "score";
            this.score.ReadOnly = true;
            this.score.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Qualityks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(954, 502);
            this.Controls.Add(this.dgvQuality);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Qualityks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "温馨提示--要及时书写病历";
            this.Load += new System.EventHandler(this.Qualityks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuality)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvQuality;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.ComboBox cboDoctor;
        private System.Windows.Forms.Label dd;
        private System.Windows.Forms.ComboBox cboEmrNote;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dpendTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.ComboBox cboDepart;
        private System.Windows.Forms.DataGridViewTextBoxColumn patient;
        private System.Windows.Forms.DataGridViewTextBoxColumn noteName;
        private System.Windows.Forms.DataGridViewTextBoxColumn startTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn writenTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn limit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rest;
        private System.Windows.Forms.DataGridViewTextBoxColumn score;

    }
}