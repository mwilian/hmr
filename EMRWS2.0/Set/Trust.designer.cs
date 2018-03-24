namespace EMR
{
    partial class Trust
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
            this.lbPrompt = new System.Windows.Forms.Label();
            this.lbxDoctors = new System.Windows.Forms.ListBox();
            this.tbxDoctor = new System.Windows.Forms.TextBox();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbPrompt
            // 
            this.lbPrompt.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbPrompt.ForeColor = System.Drawing.Color.White;
            this.lbPrompt.Location = new System.Drawing.Point(2, 14);
            this.lbPrompt.Name = "lbPrompt";
            this.lbPrompt.Size = new System.Drawing.Size(105, 21);
            this.lbPrompt.TabIndex = 0;
            this.lbPrompt.Text = "label1";
            this.lbPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbxDoctors
            // 
            this.lbxDoctors.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbxDoctors.FormattingEnabled = true;
            this.lbxDoctors.ItemHeight = 14;
            this.lbxDoctors.Location = new System.Drawing.Point(109, 41);
            this.lbxDoctors.Name = "lbxDoctors";
            this.lbxDoctors.Size = new System.Drawing.Size(100, 270);
            this.lbxDoctors.TabIndex = 1;
            this.lbxDoctors.Click += new System.EventHandler(this.lbxDoctors_Click);
            this.lbxDoctors.SelectedIndexChanged += new System.EventHandler(this.lbxDoctors_SelectedIndexChanged);
            // 
            // tbxDoctor
            // 
            this.tbxDoctor.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxDoctor.Location = new System.Drawing.Point(109, 14);
            this.tbxDoctor.Name = "tbxDoctor";
            this.tbxDoctor.Size = new System.Drawing.Size(100, 23);
            this.tbxDoctor.TabIndex = 0;
            this.tbxDoctor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxDoctor_KeyPress);
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(324, 15);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(117, 21);
            this.dtpEnd.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(209, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "代理结束日期：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnOk.Image = global::EMR.Properties.Resources.ok11;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(372, 284);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(69, 27);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "确定";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // Trust
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(449, 316);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.tbxDoctor);
            this.Controls.Add(this.lbxDoctors);
            this.Controls.Add(this.lbPrompt);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Trust";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "委托代理";
            this.SizeChanged += new System.EventHandler(this.Trust_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbPrompt;
        private System.Windows.Forms.ListBox lbxDoctors;
        private System.Windows.Forms.TextBox tbxDoctor;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
    }
}