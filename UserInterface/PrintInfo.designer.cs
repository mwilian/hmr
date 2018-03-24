namespace UserInterface
{
    partial class PrintInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintInfo));
            this.dgvPrintInfo = new System.Windows.Forms.DataGridView();
            this.department = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.doctor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //((System.ComponentModel.ISupportInitialize)(this.dgvPrintInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPrintInfo
            // 
            this.dgvPrintInfo.AllowUserToDeleteRows = false;
            this.dgvPrintInfo.AllowUserToResizeRows = false;
            this.dgvPrintInfo.BackgroundColor = System.Drawing.Color.White;
            this.dgvPrintInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPrintInfo.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPrintInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPrintInfo.ColumnHeadersHeight = 22;
            this.dgvPrintInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPrintInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.department,
            this.doctor,
            this.patient,
            this.note,
            this.count});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.AntiqueWhite;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPrintInfo.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPrintInfo.Font = new System.Drawing.Font("宋体", 10.5F);
            this.dgvPrintInfo.Location = new System.Drawing.Point(1, 0);
            this.dgvPrintInfo.MultiSelect = false;
            this.dgvPrintInfo.Name = "dgvPrintInfo";
            this.dgvPrintInfo.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.PeachPuff;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPrintInfo.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPrintInfo.RowHeadersWidth = 16;
            this.dgvPrintInfo.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPrintInfo.RowTemplate.Height = 23;
            this.dgvPrintInfo.RowTemplate.ReadOnly = true;
            this.dgvPrintInfo.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPrintInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvPrintInfo.Size = new System.Drawing.Size(751, 493);
            this.dgvPrintInfo.TabIndex = 0;
            // 
            // department
            // 
            this.department.HeaderText = "科室";
            this.department.Name = "department";
            this.department.ReadOnly = true;
            this.department.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.department.Width = 230;
            // 
            // doctor
            // 
            this.doctor.HeaderText = "操作";
            this.doctor.Name = "doctor";
            this.doctor.ReadOnly = true;
            this.doctor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // patient
            // 
            this.patient.HeaderText = "患者";
            this.patient.Name = "patient";
            this.patient.ReadOnly = true;
            this.patient.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // note
            // 
            this.note.HeaderText = "记录";
            this.note.Name = "note";
            this.note.ReadOnly = true;
            this.note.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.note.Width = 230;
            // 
            // count
            // 
            this.count.HeaderText = "次数";
            this.count.Name = "count";
            this.count.ReadOnly = true;
            this.count.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.count.Width = 60;
            // 
            // PrintInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(752, 498);
            this.Controls.Add(this.dgvPrintInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "病历打印情况";
            this.Load += new System.EventHandler(this.PrintInfo_Load);
            //((System.ComponentModel.ISupportInitialize)(this.dgvPrintInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPrintInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn department;
        private System.Windows.Forms.DataGridViewTextBoxColumn doctor;
        private System.Windows.Forms.DataGridViewTextBoxColumn patient;
        private System.Windows.Forms.DataGridViewTextBoxColumn note;
        private System.Windows.Forms.DataGridViewTextBoxColumn count;
    }
}