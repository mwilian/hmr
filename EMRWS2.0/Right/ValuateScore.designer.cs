namespace EMR
{
    partial class ValuateScore
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
            this.split = new System.Windows.Forms.SplitContainer();
            this.lbAbsence = new System.Windows.Forms.Label();
            this.btnReport = new System.Windows.Forms.Button();
            this.lbScore = new System.Windows.Forms.Label();
            this.lbLevel = new System.Windows.Forms.Label();
            this.lbValuate = new System.Windows.Forms.Label();
            this.lbKnock = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbPatient = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            //((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.Location = new System.Drawing.Point(2, 27);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.AutoScroll = true;
            this.split.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(235)))));
            this.split.Panel2.Controls.Add(this.lbAbsence);
            this.split.Panel2.Resize += new System.EventHandler(this.split_Panel2_Resize);
            this.split.Size = new System.Drawing.Size(634, 421);
            this.split.SplitterDistance = 330;
            this.split.TabIndex = 0;
            // 
            // lbAbsence
            // 
            this.lbAbsence.AutoSize = true;
            this.lbAbsence.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(235)))));
            this.lbAbsence.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbAbsence.ForeColor = System.Drawing.Color.Maroon;
            this.lbAbsence.Location = new System.Drawing.Point(3, 5);
            this.lbAbsence.Name = "lbAbsence";
            this.lbAbsence.Size = new System.Drawing.Size(91, 14);
            this.lbAbsence.TabIndex = 1;
            this.lbAbsence.Text = "空缺必要记录";
            this.lbAbsence.Visible = false;
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(402, 2);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(68, 23);
            this.btnReport.TabIndex = 5;
            this.btnReport.Text = "产生报表";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // lbScore
            // 
            this.lbScore.AutoSize = true;
            this.lbScore.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbScore.ForeColor = System.Drawing.Color.DimGray;
            this.lbScore.Location = new System.Drawing.Point(198, 20);
            this.lbScore.Name = "lbScore";
            this.lbScore.Size = new System.Drawing.Size(49, 14);
            this.lbScore.TabIndex = 3;
            this.lbScore.Text = "得分：";
            this.lbScore.Visible = false;
            // 
            // lbLevel
            // 
            this.lbLevel.AutoSize = true;
            this.lbLevel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbLevel.ForeColor = System.Drawing.Color.DimGray;
            this.lbLevel.Location = new System.Drawing.Point(10, 19);
            this.lbLevel.Name = "lbLevel";
            this.lbLevel.Size = new System.Drawing.Size(77, 14);
            this.lbLevel.TabIndex = 2;
            this.lbLevel.Text = "病历等级：";
            // 
            // lbValuate
            // 
            this.lbValuate.AutoSize = true;
            this.lbValuate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbValuate.ForeColor = System.Drawing.Color.Maroon;
            this.lbValuate.Location = new System.Drawing.Point(93, 19);
            this.lbValuate.Name = "lbValuate";
            this.lbValuate.Size = new System.Drawing.Size(55, 14);
            this.lbValuate.TabIndex = 0;
            this.lbValuate.Text = "label1";
            // 
            // lbKnock
            // 
            this.lbKnock.AutoSize = true;
            this.lbKnock.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbKnock.ForeColor = System.Drawing.Color.DimGray;
            this.lbKnock.Location = new System.Drawing.Point(198, 3);
            this.lbKnock.Name = "lbKnock";
            this.lbKnock.Size = new System.Drawing.Size(49, 14);
            this.lbKnock.TabIndex = 4;
            this.lbKnock.Text = "扣分：";
            this.lbKnock.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.Image = global::EMR.Properties.Resources.savesm;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(716, 1);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(57, 24);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "保存";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lbPatient
            // 
            this.lbPatient.AutoSize = true;
            this.lbPatient.ForeColor = System.Drawing.Color.Black;
            this.lbPatient.Location = new System.Drawing.Point(3, 7);
            this.lbPatient.Name = "lbPatient";
            this.lbPatient.Size = new System.Drawing.Size(41, 12);
            this.lbPatient.TabIndex = 4;
            this.lbPatient.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(235)))));
            this.panel1.Location = new System.Drawing.Point(337, 448);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(299, 39);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.panel2.Controls.Add(this.lbScore);
            this.panel2.Controls.Add(this.lbValuate);
            this.panel2.Controls.Add(this.lbLevel);
            this.panel2.Controls.Add(this.lbKnock);
            this.panel2.Location = new System.Drawing.Point(2, 448);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(329, 39);
            this.panel2.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(496, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "显示定级报表";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(580, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(54, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ValuateScore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(641, 490);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbPatient);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.split);
            this.Name = "ValuateScore";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "病历定级";
            this.Resize += new System.EventHandler(this.ValuateScore_Resize);
            this.split.Panel2.ResumeLayout(false);
            this.split.Panel2.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lbValuate;
        private System.Windows.Forms.Label lbAbsence;
        private System.Windows.Forms.Label lbLevel;
        private System.Windows.Forms.Label lbScore;
        private System.Windows.Forms.Label lbKnock;
        private System.Windows.Forms.Label lbPatient;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}