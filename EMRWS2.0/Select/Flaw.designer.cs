namespace EMR
{
    partial class Flaw
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbNoteName = new System.Windows.Forms.Label();
            this.dgvFlaws = new System.Windows.Forms.DataGridView();
            this.FlawText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.knockoff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbScore = new System.Windows.Forms.Label();
            //((System.ComponentModel.ISupportInitialize)(this.dgvFlaws)).BeginInit();
            this.SuspendLayout();
            // 
            // lbNoteName
            // 
            this.lbNoteName.AutoSize = true;
            this.lbNoteName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbNoteName.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbNoteName.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbNoteName.Location = new System.Drawing.Point(5, 4);
            this.lbNoteName.Name = "lbNoteName";
            this.lbNoteName.Size = new System.Drawing.Size(49, 14);
            this.lbNoteName.TabIndex = 0;
            this.lbNoteName.Text = "label1";
            // 
            // dgvFlaws
            // 
            this.dgvFlaws.AllowUserToAddRows = false;
            this.dgvFlaws.AllowUserToDeleteRows = false;
            this.dgvFlaws.AllowUserToResizeColumns = false;
            this.dgvFlaws.AllowUserToResizeRows = false;
            this.dgvFlaws.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvFlaws.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFlaws.ColumnHeadersVisible = false;
            this.dgvFlaws.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FlawText,
            this.knockoff});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFlaws.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFlaws.Enabled = false;
            this.dgvFlaws.Location = new System.Drawing.Point(0, 22);
            this.dgvFlaws.Name = "dgvFlaws";
            this.dgvFlaws.RowHeadersVisible = false;
            this.dgvFlaws.RowTemplate.Height = 23;
            this.dgvFlaws.Size = new System.Drawing.Size(336, 125);
            this.dgvFlaws.TabIndex = 1;
            // 
            // FlawText
            // 
            this.FlawText.HeaderText = "х╠ощ";
            this.FlawText.Name = "FlawText";
            this.FlawText.Width = 200;
            // 
            // knockoff
            // 
            this.knockoff.HeaderText = "©ш╥ж";
            this.knockoff.Name = "knockoff";
            this.knockoff.Width = 36;
            // 
            // lbScore
            // 
            this.lbScore.AutoSize = true;
            this.lbScore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbScore.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbScore.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbScore.Location = new System.Drawing.Point(290, 4);
            this.lbScore.Name = "lbScore";
            this.lbScore.Size = new System.Drawing.Size(49, 14);
            this.lbScore.TabIndex = 2;
            this.lbScore.Text = "label1";
            // 
            // Flaw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.dgvFlaws);
            this.Controls.Add(this.lbNoteName);
            this.Controls.Add(this.lbScore);
            this.Name = "Flaw";
            this.Size = new System.Drawing.Size(343, 150);
            this.Resize += new System.EventHandler(this.Flaw_Resize);
            //((System.ComponentModel.ISupportInitialize)(this.dgvFlaws)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbNoteName;
        private System.Windows.Forms.DataGridView dgvFlaws;
        private System.Windows.Forms.DataGridViewTextBoxColumn FlawText;
        private System.Windows.Forms.DataGridViewTextBoxColumn knockoff;
        private System.Windows.Forms.Label lbScore;
    }
}
