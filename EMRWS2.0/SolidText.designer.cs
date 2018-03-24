namespace EMR
{
    partial class SolidTextChange
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
            this.cboInfo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cboInfo
            // 
            this.cboInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInfo.FormattingEnabled = true;
            this.cboInfo.Items.AddRange(new object[] {
            "入院时间",
            "死亡时间",
            "病情陈述者",
            "入院时情况",
            "入院时诊断",
            "诊疗经过及抢救经过",
            "死亡原因",
            "死亡诊断"});
            this.cboInfo.Location = new System.Drawing.Point(15, 14);
            this.cboInfo.Name = "cboInfo";
            this.cboInfo.Size = new System.Drawing.Size(121, 20);
            this.cboInfo.TabIndex = 0;
            this.cboInfo.SelectedIndexChanged += new System.EventHandler(this.cboInfo_SelectedIndexChanged);
            // 
            // SolidTextChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(151, 49);
            this.Controls.Add(this.cboInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SolidTextChange";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "固化信息";
            this.Deactivate += new System.EventHandler(this.SolidTextChange_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboInfo;
    }
}