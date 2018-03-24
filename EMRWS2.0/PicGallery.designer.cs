namespace EMR
{
    partial class PicGallery
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PicGallery));
            this.PicMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.delPicture = new System.Windows.Forms.ToolStripMenuItem();
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.panelEx2 = new DevComponents.DotNetBar.PanelEx();
            this.itemPanel2 = new DevComponents.DotNetBar.ItemPanel();
            this.plContain = new System.Windows.Forms.Panel();
            this.labelItem2 = new DevComponents.DotNetBar.LabelItem();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.btnOK = new DevComponents.DotNetBar.ButtonX();
            this.chartTypes = new DevComponents.DotNetBar.ItemPanel();
            this.buttonItem1 = new DevComponents.DotNetBar.ButtonItem();
            this.dtSelector = new DevComponents.DotNetBar.ButtonItem();
            this.pnSelector = new DevComponents.DotNetBar.ButtonItem();
            this.labelItem1 = new DevComponents.DotNetBar.LabelItem();
            this.PicMenu2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPicture = new System.Windows.Forms.ToolStripMenuItem();
            this.exit2 = new System.Windows.Forms.ToolStripMenuItem();
            this.PicMenu.SuspendLayout();
            this.panelEx2.SuspendLayout();
            this.itemPanel2.SuspendLayout();
            this.PicMenu2.SuspendLayout();
            this.SuspendLayout();
            // 
            // PicMenu
            // 
            this.PicMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.delPicture,
            this.exit});
            this.PicMenu.Name = "PicMenu";
            this.PicMenu.Size = new System.Drawing.Size(95, 48);
            this.PicMenu.Opening += new System.ComponentModel.CancelEventHandler(this.PicMenu_Opening);
            // 
            // delPicture
            // 
            this.delPicture.Name = "delPicture";
            this.delPicture.Size = new System.Drawing.Size(94, 22);
            this.delPicture.Text = "删除";
            this.delPicture.Click += new System.EventHandler(this.deletePicture_Click);
            // 
            // exit
            // 
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(94, 22);
            this.exit.Text = "退出";
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEx1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(761, 34);
            this.panelEx1.Style.BackColor1.Color = System.Drawing.Color.AliceBlue;
            this.panelEx1.Style.BackColor2.Color = System.Drawing.Color.LightSteelBlue;
            this.panelEx1.Style.ForeColor.Color = System.Drawing.Color.SteelBlue;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.Style.MarginLeft = 8;
            this.panelEx1.TabIndex = 1;
            this.panelEx1.Text = "图谱";
            // 
            // panelEx2
            // 
            this.panelEx2.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx2.Controls.Add(this.itemPanel2);
            this.panelEx2.Controls.Add(this.btnClose);
            this.panelEx2.Controls.Add(this.btnOK);
            this.panelEx2.Controls.Add(this.chartTypes);
            this.panelEx2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx2.Location = new System.Drawing.Point(0, 34);
            this.panelEx2.Name = "panelEx2";
            this.panelEx2.Size = new System.Drawing.Size(761, 405);
            this.panelEx2.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx2.Style.BackColor1.Color = System.Drawing.Color.LightCyan;
            this.panelEx2.Style.BackColor2.Color = System.Drawing.Color.AliceBlue;
            this.panelEx2.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx2.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx2.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx2.Style.GradientAngle = 90;
            this.panelEx2.TabIndex = 2;
            // 
            // itemPanel2
            // 
            this.itemPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.itemPanel2.BackgroundStyle.BackColor = System.Drawing.Color.White;
            this.itemPanel2.BackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.itemPanel2.BackgroundStyle.BackColorGradientAngle = 90;
            this.itemPanel2.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel2.BackgroundStyle.BorderBottomWidth = 1;
            this.itemPanel2.BackgroundStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.itemPanel2.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel2.BackgroundStyle.BorderLeftWidth = 1;
            this.itemPanel2.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel2.BackgroundStyle.BorderRightWidth = 1;
            this.itemPanel2.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel2.BackgroundStyle.BorderTopWidth = 1;
            this.itemPanel2.BackgroundStyle.PaddingBottom = 1;
            this.itemPanel2.BackgroundStyle.PaddingLeft = 1;
            this.itemPanel2.BackgroundStyle.PaddingRight = 1;
            this.itemPanel2.BackgroundStyle.PaddingTop = 1;
            this.itemPanel2.Controls.Add(this.plContain);
            this.itemPanel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemPanel2.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.labelItem2});
            this.itemPanel2.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.itemPanel2.Location = new System.Drawing.Point(172, 6);
            this.itemPanel2.Name = "itemPanel2";
            this.itemPanel2.Size = new System.Drawing.Size(582, 364);
            this.itemPanel2.TabIndex = 8;
            this.itemPanel2.Text = "itemPanel2";
            // 
            // plContain
            // 
            this.plContain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.plContain.AutoScroll = true;
            this.plContain.ContextMenuStrip = this.PicMenu2;
            this.plContain.Location = new System.Drawing.Point(0, 25);
            this.plContain.Name = "plContain";
            this.plContain.Size = new System.Drawing.Size(575, 332);
            this.plContain.TabIndex = 0;
            this.plContain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plContain_MouseDown);
            // 
            // labelItem2
            // 
            this.labelItem2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.labelItem2.BorderSide = DevComponents.DotNetBar.eBorderSide.Bottom;
            this.labelItem2.BorderType = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.labelItem2.Name = "labelItem2";
            this.labelItem2.PaddingBottom = 1;
            this.labelItem2.PaddingLeft = 1;
            this.labelItem2.PaddingRight = 1;
            this.labelItem2.PaddingTop = 1;
            this.labelItem2.SingleLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.labelItem2.Text = "<b>图谱预览</b>";
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(682, 376);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(67, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "退出";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(609, 376);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(67, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "确定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chartTypes
            // 
            this.chartTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            // 
            // 
            // 
            this.chartTypes.BackgroundStyle.BackColor = System.Drawing.Color.White;
            this.chartTypes.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.chartTypes.BackgroundStyle.BorderBottomWidth = 1;
            this.chartTypes.BackgroundStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.chartTypes.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.chartTypes.BackgroundStyle.BorderLeftWidth = 1;
            this.chartTypes.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.chartTypes.BackgroundStyle.BorderRightWidth = 1;
            this.chartTypes.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.chartTypes.BackgroundStyle.BorderTopWidth = 1;
            this.chartTypes.BackgroundStyle.PaddingBottom = 1;
            this.chartTypes.BackgroundStyle.PaddingLeft = 1;
            this.chartTypes.BackgroundStyle.PaddingRight = 1;
            this.chartTypes.BackgroundStyle.PaddingTop = 1;
            this.chartTypes.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chartTypes.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem1,
            this.dtSelector,
            this.pnSelector});
            this.chartTypes.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.chartTypes.Location = new System.Drawing.Point(3, 6);
            this.chartTypes.Name = "chartTypes";
            this.chartTypes.Size = new System.Drawing.Size(161, 364);
            this.chartTypes.TabIndex = 2;
            this.chartTypes.Text = "itemPanel1";
            // 
            // buttonItem1
            // 
            this.buttonItem1.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem1.Image = ((System.Drawing.Image)(resources.GetObject("buttonItem1.Image")));
            this.buttonItem1.ImagePaddingHorizontal = 8;
            this.buttonItem1.Name = "buttonItem1";
            this.buttonItem1.OptionGroup = "charts";
            this.buttonItem1.Text = "分类列表";
            // 
            // dtSelector
            // 
            this.dtSelector.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.dtSelector.Checked = true;
            this.dtSelector.Image = ((System.Drawing.Image)(resources.GetObject("dtSelector.Image")));
            this.dtSelector.ImagePaddingHorizontal = 8;
            this.dtSelector.Name = "dtSelector";
            this.dtSelector.OptionGroup = "charts";
            this.dtSelector.Text = "科室";
            this.dtSelector.Click += new System.EventHandler(this.dtSelector_Click);
            // 
            // pnSelector
            // 
            this.pnSelector.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.pnSelector.Image = ((System.Drawing.Image)(resources.GetObject("pnSelector.Image")));
            this.pnSelector.ImagePaddingHorizontal = 8;
            this.pnSelector.Name = "pnSelector";
            this.pnSelector.OptionGroup = "charts";
            this.pnSelector.Text = "个人";
            this.pnSelector.Click += new System.EventHandler(this.pnSelector_Click);
            // 
            // labelItem1
            // 
            this.labelItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.labelItem1.BorderSide = DevComponents.DotNetBar.eBorderSide.Bottom;
            this.labelItem1.BorderType = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.labelItem1.Name = "labelItem1";
            this.labelItem1.PaddingBottom = 1;
            this.labelItem1.PaddingLeft = 1;
            this.labelItem1.PaddingRight = 1;
            this.labelItem1.PaddingTop = 1;
            this.labelItem1.SingleLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.labelItem1.Text = "<b>Column</b>";
            // 
            // PicMenu2
            // 
            this.PicMenu2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPicture,
            this.exit2});
            this.PicMenu2.Name = "PicMenu";
            this.PicMenu2.Size = new System.Drawing.Size(153, 70);
            // 
            // addPicture
            // 
            this.addPicture.Name = "addPicture";
            this.addPicture.Size = new System.Drawing.Size(152, 22);
            this.addPicture.Text = "添加";
            this.addPicture.Click += new System.EventHandler(this.addPicture_Click);
            // 
            // exit2
            // 
            this.exit2.Name = "exit2";
            this.exit2.Size = new System.Drawing.Size(94, 22);
            this.exit2.Text = "退出";
            this.exit.Click += new System.EventHandler(this.exit_Click);
        
            // 
            // PicGallery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 439);
            this.Controls.Add(this.panelEx2);
            this.Controls.Add(this.panelEx1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PicGallery";
            this.ShowIcon = false;
            this.PicMenu.ResumeLayout(false);
            this.panelEx2.ResumeLayout(false);
            this.itemPanel2.ResumeLayout(false);
            this.PicMenu2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip PicMenu;
        private System.Windows.Forms.ToolStripMenuItem delPicture;
        private System.Windows.Forms.ToolStripMenuItem exit;
        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.PanelEx panelEx2;
        private DevComponents.DotNetBar.ItemPanel chartTypes;
        private DevComponents.DotNetBar.ButtonItem buttonItem1;
        private DevComponents.DotNetBar.ButtonItem dtSelector;
        private DevComponents.DotNetBar.ButtonItem pnSelector;
        private DevComponents.DotNetBar.LabelItem labelItem1;
        private DevComponents.DotNetBar.ItemPanel itemPanel2;
        private DevComponents.DotNetBar.LabelItem labelItem2;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.ButtonX btnOK;
        private System.Windows.Forms.Panel plContain;
        private System.Windows.Forms.ContextMenuStrip PicMenu2;
        private System.Windows.Forms.ToolStripMenuItem addPicture;
        private System.Windows.Forms.ToolStripMenuItem exit2;

    }
}