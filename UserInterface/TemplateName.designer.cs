namespace UserInterface
{
    partial class TemplateName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateName));
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.lbNames = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // bnCancel
            // 
            this.bnCancel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bnCancel.Image = ((System.Drawing.Image)(resources.GetObject("bnCancel.Image")));
            this.bnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bnCancel.Location = new System.Drawing.Point(309, 467);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 36);
            this.bnCancel.TabIndex = 11;
            this.bnCancel.Text = "取消";
            this.bnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnOK
            // 
            this.bnOK.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bnOK.Image = ((System.Drawing.Image)(resources.GetObject("bnOK.Image")));
            this.bnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bnOK.Location = new System.Drawing.Point(309, 7);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 33);
            this.bnOK.TabIndex = 10;
            this.bnOK.Text = "确定";
            this.bnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // lbNames
            // 
            this.lbNames.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbNames.ForeColor = System.Drawing.Color.DimGray;
            this.lbNames.ItemHeight = 14;
            this.lbNames.Location = new System.Drawing.Point(89, 71);
            this.lbNames.Name = "lbNames";
            this.lbNames.Size = new System.Drawing.Size(296, 396);
            this.lbNames.Sorted = true;
            this.lbNames.TabIndex = 9;
            this.lbNames.SelectedIndexChanged += new System.EventHandler(this.lbNames_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 10F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 14);
            this.label1.TabIndex = 8;
            this.label1.Text = "模版名称：";
            // 
            // tbName
            // 
            this.tbName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbName.Font = new System.Drawing.Font("宋体", 10F);
            this.tbName.Location = new System.Drawing.Point(89, 46);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(296, 23);
            this.tbName.TabIndex = 7;
            // 
            // TemplateName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(402, 515);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.lbNames);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TemplateName";
            this.Text = "TemplateName";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.ListBox lbNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
    }
}