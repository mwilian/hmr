namespace UserInterface
{
    partial class EmrNoteInfo
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.gpInfo = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.LastChecker = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.Checker = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.Writer = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.tbWriteTime = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.gpInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpInfo
            // 
            this.gpInfo.AutoScroll = true;
            this.gpInfo.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpInfo.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpInfo.Controls.Add(this.buttonX1);
            this.gpInfo.Controls.Add(this.LastChecker);
            this.gpInfo.Controls.Add(this.labelX4);
            this.gpInfo.Controls.Add(this.Checker);
            this.gpInfo.Controls.Add(this.labelX3);
            this.gpInfo.Controls.Add(this.Writer);
            this.gpInfo.Controls.Add(this.labelX2);
            this.gpInfo.Controls.Add(this.tbWriteTime);
            this.gpInfo.Controls.Add(this.labelX1);
            this.gpInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpInfo.Location = new System.Drawing.Point(0, 0);
            this.gpInfo.Name = "gpInfo";
            this.gpInfo.Size = new System.Drawing.Size(253, 297);
            // 
            // 
            // 
            this.gpInfo.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpInfo.Style.BackColorGradientAngle = 90;
            this.gpInfo.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpInfo.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpInfo.Style.BorderBottomWidth = 1;
            this.gpInfo.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpInfo.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpInfo.Style.BorderLeftWidth = 1;
            this.gpInfo.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpInfo.Style.BorderRightWidth = 1;
            this.gpInfo.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpInfo.Style.BorderTopWidth = 1;
            this.gpInfo.Style.CornerDiameter = 4;
            this.gpInfo.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpInfo.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpInfo.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.gpInfo.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpInfo.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.gpInfo.TabIndex = 2;
            this.gpInfo.Text = "Customer Data";
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.Location = new System.Drawing.Point(89, 230);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(75, 23);
            this.buttonX1.TabIndex = 8;
            this.buttonX1.Text = "确定";
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // LastChecker
            // 
            this.LastChecker.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.LastChecker.Border.Class = "TextBoxBorder";
            this.LastChecker.Location = new System.Drawing.Point(11, 184);
            this.LastChecker.Name = "LastChecker";
            this.LastChecker.ReadOnly = true;
            this.LastChecker.Size = new System.Drawing.Size(226, 21);
            this.LastChecker.TabIndex = 7;
            // 
            // labelX4
            // 
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            this.labelX4.Location = new System.Drawing.Point(12, 163);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(104, 23);
            this.labelX4.TabIndex = 6;
            this.labelX4.Text = "病历终审者：";
            // 
            // Checker
            // 
            this.Checker.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.Checker.Border.Class = "TextBoxBorder";
            this.Checker.Location = new System.Drawing.Point(10, 136);
            this.Checker.Name = "Checker";
            this.Checker.ReadOnly = true;
            this.Checker.Size = new System.Drawing.Size(226, 21);
            this.Checker.TabIndex = 5;
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            this.labelX3.Location = new System.Drawing.Point(11, 115);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(104, 23);
            this.labelX3.TabIndex = 4;
            this.labelX3.Text = "病历审核者：";
            // 
            // Writer
            // 
            this.Writer.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.Writer.Border.Class = "TextBoxBorder";
            this.Writer.Location = new System.Drawing.Point(11, 87);
            this.Writer.Name = "Writer";
            this.Writer.ReadOnly = true;
            this.Writer.Size = new System.Drawing.Size(226, 21);
            this.Writer.TabIndex = 3;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(12, 66);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(104, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "病历书写者：";
            // 
            // tbWriteTime
            // 
            this.tbWriteTime.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.tbWriteTime.Border.Class = "TextBoxBorder";
            this.tbWriteTime.Location = new System.Drawing.Point(11, 39);
            this.tbWriteTime.Name = "tbWriteTime";
            this.tbWriteTime.ReadOnly = true;
            this.tbWriteTime.Size = new System.Drawing.Size(226, 21);
            this.tbWriteTime.TabIndex = 1;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(12, 18);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(104, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "病历书写时间：";
            // 
            // EmrNoteInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 297);
            this.Controls.Add(this.gpInfo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmrNoteInfo";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "病历信息";
            this.gpInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.GroupPanel gpInfo;
        private DevComponents.DotNetBar.Controls.TextBoxX tbWriteTime;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private DevComponents.DotNetBar.Controls.TextBoxX LastChecker;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.TextBoxX Checker;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.TextBoxX Writer;
        private DevComponents.DotNetBar.LabelX labelX2;
    }
}

