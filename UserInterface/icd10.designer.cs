namespace UserInterface
{
    partial class icd10
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
            this.tbxCode = new System.Windows.Forms.TextBox();
            this.dgvIcd10 = new System.Windows.Forms.DataGridView();
            //((System.ComponentModel.ISupportInitialize)(this.dgvIcd10)).BeginInit();
            this.SuspendLayout();
            // 
            // tbxCode
            // 
            this.tbxCode.Location = new System.Drawing.Point(2, 5);
            this.tbxCode.Name = "tbxCode";
            this.tbxCode.Size = new System.Drawing.Size(384, 21);
            this.tbxCode.TabIndex = 0;
            this.tbxCode.Enter += new System.EventHandler(this.tbxCode_Enter);
            this.tbxCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbxCode_KeyUp);
            // 
            // dgvIcd10
            // 
            this.dgvIcd10.AllowUserToAddRows = false;
            this.dgvIcd10.AllowUserToDeleteRows = false;
            this.dgvIcd10.BackgroundColor = System.Drawing.Color.White;
            this.dgvIcd10.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvIcd10.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIcd10.ColumnHeadersVisible = false;
            this.dgvIcd10.Location = new System.Drawing.Point(2, 30);
            this.dgvIcd10.Name = "dgvIcd10";
            this.dgvIcd10.RowHeadersVisible = false;
            this.dgvIcd10.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.dgvIcd10.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(255)))), ((int)(((byte)(220)))));
            this.dgvIcd10.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.dgvIcd10.RowTemplate.Height = 23;
            this.dgvIcd10.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvIcd10.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIcd10.Size = new System.Drawing.Size(518, 235);
            this.dgvIcd10.TabIndex = 1;
            this.dgvIcd10.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIcd10_CellContentDoubleClick);
            // 
            // icd10
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(522, 266);
            this.Controls.Add(this.dgvIcd10);
            this.Controls.Add(this.tbxCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "icd10";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "icd10";
            this.Resize += new System.EventHandler(this.icd10_Resize);
            //((System.ComponentModel.ISupportInitialize)(this.dgvIcd10)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxCode;
        private System.Windows.Forms.DataGridView dgvIcd10;
    }
}