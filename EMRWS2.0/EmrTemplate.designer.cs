namespace EMR
{
    partial class EmrTemplate
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmrTemplate));
            this.menuTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.open = new System.Windows.Forms.ToolStripMenuItem();
            this.use = new System.Windows.Forms.ToolStripMenuItem();
            this.delete = new System.Windows.Forms.ToolStripMenuItem();
            this.qToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.close = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tvTemplate = new System.Windows.Forms.TreeView();
            this.rename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTemplate.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuTemplate
            // 
            this.menuTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.open,
            this.use,
            this.delete,
            this.rename,
            this.qToolStripMenuItem,
            this.close});
            this.menuTemplate.Name = "menuTemplate";
            this.menuTemplate.Size = new System.Drawing.Size(153, 142);
            this.menuTemplate.Opening += new System.ComponentModel.CancelEventHandler(this.menuTemplate_Opening);
            // 
            // open
            // 
            this.open.Name = "open";
            this.open.Size = new System.Drawing.Size(152, 22);
            this.open.Text = "查看";
            this.open.Click += new System.EventHandler(this.open_Click);
            // 
            // use
            // 
            this.use.Name = "use";
            this.use.Size = new System.Drawing.Size(152, 22);
            this.use.Text = "使用";
            this.use.Click += new System.EventHandler(this.use_Click);
            // 
            // delete
            // 
            this.delete.Name = "delete";
            this.delete.Size = new System.Drawing.Size(152, 22);
            this.delete.Text = "删除";
            this.delete.Click += new System.EventHandler(this.delete_Click);
            // 
            // qToolStripMenuItem
            // 
            this.qToolStripMenuItem.Name = "qToolStripMenuItem";
            this.qToolStripMenuItem.Size = new System.Drawing.Size(149, 6);
            // 
            // close
            // 
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(152, 22);
            this.close.Text = "退出";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "eye.ico");
            this.imageList1.Images.SetKeyName(1, "man.ico");
            this.imageList1.Images.SetKeyName(2, "woman.ico");
            this.imageList1.Images.SetKeyName(3, "Users.ico");
            this.imageList1.Images.SetKeyName(4, "User.ico");
            this.imageList1.Images.SetKeyName(5, "Book2.ico");
            this.imageList1.Images.SetKeyName(6, "sign.ico");
            // 
            // tvTemplate
            // 
            this.tvTemplate.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tvTemplate.ContextMenuStrip = this.menuTemplate;
            this.tvTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTemplate.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tvTemplate.HotTracking = true;
            this.tvTemplate.ImageIndex = 0;
            this.tvTemplate.ImageList = this.imageList1;
            this.tvTemplate.Location = new System.Drawing.Point(0, 0);
            this.tvTemplate.Margin = new System.Windows.Forms.Padding(0);
            this.tvTemplate.Name = "tvTemplate";
            this.tvTemplate.SelectedImageIndex = 0;
            this.tvTemplate.Size = new System.Drawing.Size(209, 433);
            this.tvTemplate.TabIndex = 2;
            this.tvTemplate.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvTemplate_ItemDrag);
            this.tvTemplate.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvTemplate_NodeMouseClick);
            this.tvTemplate.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvTemplate_NodeMouseDoubleClick);
            this.tvTemplate.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvTemplate_DragDrop);
            this.tvTemplate.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvTemplate_DragEnter);
            // 
            // rename
            // 
            this.rename.Name = "rename";
            this.rename.Size = new System.Drawing.Size(152, 22);
            this.rename.Text = "更名";
            this.rename.Click += new System.EventHandler(this.rename_Click);
            // 
            // EmrTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tvTemplate);
            this.Name = "EmrTemplate";
            this.Size = new System.Drawing.Size(209, 433);
            this.Load += new System.EventHandler(this.EmrTemplate_Load);
            this.menuTemplate.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip menuTemplate;
        private System.Windows.Forms.ToolStripMenuItem open;
        private System.Windows.Forms.ToolStripMenuItem use;
        private System.Windows.Forms.ToolStripMenuItem delete;
        private System.Windows.Forms.ImageList imageList1;
        public System.Windows.Forms.TreeView tvTemplate; //2012-03-27 LiuQi模板列表显示功能
        private System.Windows.Forms.ToolStripSeparator qToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem close;
        private System.Windows.Forms.ToolStripMenuItem rename;
    }
}
