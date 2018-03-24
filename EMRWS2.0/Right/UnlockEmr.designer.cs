namespace EMR
{
    partial class UnlockEmr
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
            this.btnOK = new System.Windows.Forms.Button();
            this.rtbReasion = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nupExpire = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.chxPublic = new System.Windows.Forms.CheckBox();
            //((System.ComponentModel.ISupportInitialize)(this.nupExpire)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOK.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnOK.Image = global::EMR.Properties.Resources.savesm;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(501, 17);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(59, 24);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确认";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rtbReasion
            // 
            this.rtbReasion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbReasion.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbReasion.Location = new System.Drawing.Point(3, 45);
            this.rtbReasion.Name = "rtbReasion";
            this.rtbReasion.Size = new System.Drawing.Size(558, 223);
            this.rtbReasion.TabIndex = 1;
            this.rtbReasion.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(241, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(4, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "有效期：";
            // 
            // nupExpire
            // 
            this.nupExpire.Location = new System.Drawing.Point(66, 21);
            this.nupExpire.Name = "nupExpire";
            this.nupExpire.Size = new System.Drawing.Size(50, 21);
            this.nupExpire.TabIndex = 6;
            this.nupExpire.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(120, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 14);
            this.label3.TabIndex = 7;
            this.label3.Text = "天";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chxPublic
            // 
            this.chxPublic.AutoSize = true;
            this.chxPublic.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chxPublic.ForeColor = System.Drawing.Color.Black;
            this.chxPublic.Location = new System.Drawing.Point(206, 23);
            this.chxPublic.Name = "chxPublic";
            this.chxPublic.Size = new System.Drawing.Size(208, 18);
            this.chxPublic.TabIndex = 8;
            this.chxPublic.Text = "开封后允许其他医师编辑病历";
            this.chxPublic.UseVisualStyleBackColor = true;
            // 
            // UnlockEmr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(564, 271);
            this.Controls.Add(this.chxPublic);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nupExpire);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbReasion);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnlockEmr";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "填写病历开封原因";
            //((System.ComponentModel.ISupportInitialize)(this.nupExpire)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RichTextBox rtbReasion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nupExpire;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chxPublic;

    }
}