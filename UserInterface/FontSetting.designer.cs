namespace UserInterface
{
    partial class FontSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontSetting));
            this.comboBoxItem1 = new DevComponents.DotNetBar.ComboBoxItem();
            this.comboBoxItem2 = new DevComponents.DotNetBar.ComboBoxItem();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.btnOK = new DevComponents.DotNetBar.ButtonX();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.p3 = new System.Windows.Forms.Panel();
            this.p2 = new System.Windows.Forms.Panel();
            this.lbView = new System.Windows.Forms.Label();
            this.p1 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.colorPickerButton1 = new DevComponents.DotNetBar.ColorPickerButton();
            this.cbxUnderline = new System.Windows.Forms.CheckBox();
            this.cbxStrikethrough = new System.Windows.Forms.CheckBox();
            this.labelX8 = new DevComponents.DotNetBar.LabelX();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbxFont = new System.Windows.Forms.TextBox();
            this.lbFont = new System.Windows.Forms.ListBox();
            this.lbxSize = new System.Windows.Forms.ListBox();
            this.tbxSize = new System.Windows.Forms.TextBox();
            this.tbxShape = new System.Windows.Forms.TextBox();
            this.lbxShape = new System.Windows.Forms.ListBox();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.panelEx1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxItem1
            // 
            this.comboBoxItem1.ComboWidth = 75;
            this.comboBoxItem1.DropDownHeight = 106;
            this.comboBoxItem1.ItemHeight = 16;
            this.comboBoxItem1.Name = "comboBoxItem1";
            // 
            // comboBoxItem2
            // 
            this.comboBoxItem2.ComboWidth = 75;
            this.comboBoxItem2.DropDownHeight = 106;
            this.comboBoxItem2.ItemHeight = 16;
            this.comboBoxItem2.Name = "comboBoxItem2";
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.panelEx1.Controls.Add(this.btnCancel);
            this.panelEx1.Controls.Add(this.btnOK);
            this.panelEx1.Controls.Add(this.groupBox6);
            this.panelEx1.Controls.Add(this.groupBox5);
            this.panelEx1.Controls.Add(this.groupBox4);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(451, 325);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.Location = new System.Drawing.Point(361, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOK.Location = new System.Drawing.Point(265, 290);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "确实";
            this.btnOK.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.panel1);
            this.groupBox6.Controls.Add(this.p3);
            this.groupBox6.Controls.Add(this.p2);
            this.groupBox6.Controls.Add(this.lbView);
            this.groupBox6.Controls.Add(this.p1);
            this.groupBox6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox6.Font = new System.Drawing.Font("宋体", 9F);
            this.groupBox6.Location = new System.Drawing.Point(174, 127);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(270, 148);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "文字预览";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(220, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 64);
            this.panel1.TabIndex = 5;
            // 
            // p3
            // 
            this.p3.BackColor = System.Drawing.Color.Black;
            this.p3.Location = new System.Drawing.Point(33, 40);
            this.p3.Name = "p3";
            this.p3.Size = new System.Drawing.Size(1, 64);
            this.p3.TabIndex = 4;
            // 
            // p2
            // 
            this.p2.BackColor = System.Drawing.Color.Black;
            this.p2.Location = new System.Drawing.Point(33, 103);
            this.p2.Name = "p2";
            this.p2.Size = new System.Drawing.Size(202, 1);
            this.p2.TabIndex = 3;
            // 
            // lbView
            // 
            this.lbView.AutoSize = true;
            this.lbView.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbView.ForeColor = System.Drawing.Color.Black;
            this.lbView.Location = new System.Drawing.Point(88, 64);
            this.lbView.Name = "lbView";
            this.lbView.Size = new System.Drawing.Size(91, 14);
            this.lbView.TabIndex = 1;
            this.lbView.Text = "众心医疗科技";
            // 
            // p1
            // 
            this.p1.BackColor = System.Drawing.Color.Black;
            this.p1.Location = new System.Drawing.Point(33, 40);
            this.p1.Name = "p1";
            this.p1.Size = new System.Drawing.Size(202, 1);
            this.p1.TabIndex = 2;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.colorPickerButton1);
            this.groupBox5.Controls.Add(this.cbxUnderline);
            this.groupBox5.Controls.Add(this.cbxStrikethrough);
            this.groupBox5.Controls.Add(this.labelX8);
            this.groupBox5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox5.Font = new System.Drawing.Font("宋体", 9F);
            this.groupBox5.Location = new System.Drawing.Point(5, 126);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(150, 149);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "文字效果";
            // 
            // colorPickerButton1
            // 
            this.colorPickerButton1.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.colorPickerButton1.Image = ((System.Drawing.Image)(resources.GetObject("colorPickerButton1.Image")));
            this.colorPickerButton1.Location = new System.Drawing.Point(10, 44);
            this.colorPickerButton1.Name = "colorPickerButton1";
            this.colorPickerButton1.SelectedColorImageRectangle = new System.Drawing.Rectangle(2, 2, 12, 12);
            this.colorPickerButton1.Size = new System.Drawing.Size(117, 23);
            this.colorPickerButton1.TabIndex = 11;
            this.colorPickerButton1.Text = "自动";
            this.colorPickerButton1.SelectedColorChanged += new System.EventHandler(this.colorPickerButton1_SelectedColorChanged);
            // 
            // cbxUnderline
            // 
            this.cbxUnderline.AutoSize = true;
            this.cbxUnderline.Location = new System.Drawing.Point(12, 93);
            this.cbxUnderline.Name = "cbxUnderline";
            this.cbxUnderline.Size = new System.Drawing.Size(60, 16);
            this.cbxUnderline.TabIndex = 6;
            this.cbxUnderline.Text = "下划线";
            this.cbxUnderline.UseVisualStyleBackColor = true;
            this.cbxUnderline.CheckedChanged += new System.EventHandler(this.cbUnderline_CheckedChanged);
            // 
            // cbxStrikethrough
            // 
            this.cbxStrikethrough.AutoSize = true;
            this.cbxStrikethrough.Location = new System.Drawing.Point(12, 73);
            this.cbxStrikethrough.Name = "cbxStrikethrough";
            this.cbxStrikethrough.Size = new System.Drawing.Size(60, 16);
            this.cbxStrikethrough.TabIndex = 5;
            this.cbxStrikethrough.Text = "删除线";
            this.cbxStrikethrough.UseVisualStyleBackColor = true;
            this.cbxStrikethrough.CheckedChanged += new System.EventHandler(this.cbStrikethrough_CheckedChanged);
            // 
            // labelX8
            // 
            this.labelX8.Font = new System.Drawing.Font("宋体", 10F);
            this.labelX8.Location = new System.Drawing.Point(12, 19);
            this.labelX8.Name = "labelX8";
            this.labelX8.Size = new System.Drawing.Size(119, 23);
            this.labelX8.TabIndex = 0;
            this.labelX8.Text = "字体颜色（C）：";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbxFont);
            this.groupBox4.Controls.Add(this.lbFont);
            this.groupBox4.Controls.Add(this.lbxSize);
            this.groupBox4.Controls.Add(this.tbxSize);
            this.groupBox4.Controls.Add(this.tbxShape);
            this.groupBox4.Controls.Add(this.lbxShape);
            this.groupBox4.Controls.Add(this.labelX4);
            this.groupBox4.Controls.Add(this.labelX5);
            this.groupBox4.Controls.Add(this.labelX7);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 9F);
            this.groupBox4.Location = new System.Drawing.Point(5, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(439, 114);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "字体";
            // 
            // tbxFont
            // 
            this.tbxFont.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbxFont.Location = new System.Drawing.Point(12, 37);
            this.tbxFont.Name = "tbxFont";
            this.tbxFont.ReadOnly = true;
            this.tbxFont.Size = new System.Drawing.Size(86, 21);
            this.tbxFont.TabIndex = 10;
            this.tbxFont.Text = "宋体";
            // 
            // lbFont
            // 
            this.lbFont.FormattingEnabled = true;
            this.lbFont.ItemHeight = 12;
            this.lbFont.Location = new System.Drawing.Point(12, 59);
            this.lbFont.Name = "lbFont";
            this.lbFont.Size = new System.Drawing.Size(106, 52);
            this.lbFont.TabIndex = 9;
            this.lbFont.SelectedIndexChanged += new System.EventHandler(this.lbFont_SelectedIndexChanged);
            // 
            // lbxSize
            // 
            this.lbxSize.FormattingEnabled = true;
            this.lbxSize.ItemHeight = 12;
            this.lbxSize.Items.AddRange(new object[] {
            "小四",
            "五号"});
            this.lbxSize.Location = new System.Drawing.Point(335, 68);
            this.lbxSize.Name = "lbxSize";
            this.lbxSize.Size = new System.Drawing.Size(86, 40);
            this.lbxSize.TabIndex = 8;
            this.lbxSize.SelectedIndexChanged += new System.EventHandler(this.lbxSize_SelectedIndexChanged);
            // 
            // tbxSize
            // 
            this.tbxSize.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbxSize.Location = new System.Drawing.Point(335, 43);
            this.tbxSize.Name = "tbxSize";
            this.tbxSize.ReadOnly = true;
            this.tbxSize.Size = new System.Drawing.Size(86, 21);
            this.tbxSize.TabIndex = 7;
            this.tbxSize.Text = "五号";
            // 
            // tbxShape
            // 
            this.tbxShape.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbxShape.Location = new System.Drawing.Point(178, 43);
            this.tbxShape.Name = "tbxShape";
            this.tbxShape.ReadOnly = true;
            this.tbxShape.Size = new System.Drawing.Size(86, 21);
            this.tbxShape.TabIndex = 6;
            this.tbxShape.Text = "常规";
            // 
            // lbxShape
            // 
            this.lbxShape.FormattingEnabled = true;
            this.lbxShape.ItemHeight = 12;
            this.lbxShape.Items.AddRange(new object[] {
            "常规",
            "倾斜",
            "加粗",
            "倾斜 加粗"});
            this.lbxShape.Location = new System.Drawing.Point(178, 68);
            this.lbxShape.Name = "lbxShape";
            this.lbxShape.Size = new System.Drawing.Size(86, 40);
            this.lbxShape.TabIndex = 5;
            this.lbxShape.SelectedIndexChanged += new System.EventHandler(this.lbShape_SelectedIndexChanged);
            // 
            // labelX4
            // 
            this.labelX4.Font = new System.Drawing.Font("宋体", 10F);
            this.labelX4.Location = new System.Drawing.Point(335, 23);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(82, 23);
            this.labelX4.TabIndex = 3;
            this.labelX4.Text = "字号：";
            // 
            // labelX5
            // 
            this.labelX5.Font = new System.Drawing.Font("宋体", 10F);
            this.labelX5.Location = new System.Drawing.Point(178, 23);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(80, 23);
            this.labelX5.TabIndex = 2;
            this.labelX5.Text = "字形（Y）：";
            // 
            // labelX7
            // 
            this.labelX7.Font = new System.Drawing.Font("宋体", 10F);
            this.labelX7.Location = new System.Drawing.Point(12, 17);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(119, 23);
            this.labelX7.TabIndex = 0;
            this.labelX7.Text = "选择字体（T）：";
            // 
            // FontSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 325);
            this.Controls.Add(this.panelEx1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FontSetting";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "字体";
            this.panelEx1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ComboBoxItem comboBoxItem1;
        private DevComponents.DotNetBar.ComboBoxItem comboBoxItem2;
        //private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        //private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        //private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        //private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.ButtonX btnOK;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lbView;
        //private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        //private Microsoft.VisualBasic.PowerPacks.LineShape lineShape3;
        //private Microsoft.VisualBasic.PowerPacks.LineShape lineShape4;
        //private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private System.Windows.Forms.GroupBox groupBox5;
        private DevComponents.DotNetBar.ColorPickerButton colorPickerButton1;
        private System.Windows.Forms.CheckBox cbxUnderline;
        private System.Windows.Forms.CheckBox cbxStrikethrough;
        private DevComponents.DotNetBar.LabelX labelX8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbxFont;
        private System.Windows.Forms.ListBox lbFont;
        private System.Windows.Forms.ListBox lbxSize;
        private System.Windows.Forms.TextBox tbxSize;
        private System.Windows.Forms.TextBox tbxShape;
        private System.Windows.Forms.ListBox lbxShape;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.LabelX labelX7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel p3;
        private System.Windows.Forms.Panel p2;
        private System.Windows.Forms.Panel p1;
    }
}