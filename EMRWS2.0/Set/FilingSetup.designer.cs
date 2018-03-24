namespace EMR
{
    partial class FilingSetup
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
            this.pnlNote = new System.Windows.Forms.Panel();
            this.tvNotes = new System.Windows.Forms.TreeView();
            this.lbNoteName = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbDepartment = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            //((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.BackColor = System.Drawing.Color.White;
            this.split.Location = new System.Drawing.Point(3, 23);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.Color.White;
            this.split.Panel1.Controls.Add(this.pnlNote);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.tvNotes);
            this.split.Size = new System.Drawing.Size(848, 458);
            this.split.SplitterDistance = 557;
            this.split.TabIndex = 0;
            // 
            // pnlNote
            // 
            this.pnlNote.AllowDrop = true;
            this.pnlNote.AutoScroll = true;
            this.pnlNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(220)))));
            this.pnlNote.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNote.Location = new System.Drawing.Point(0, 3);
            this.pnlNote.Name = "pnlNote";
            this.pnlNote.Size = new System.Drawing.Size(557, 455);
            this.pnlNote.TabIndex = 1;
            this.pnlNote.DragDrop += new System.Windows.Forms.DragEventHandler(this.pnlNote_DragDrop);
            this.pnlNote.DragEnter += new System.Windows.Forms.DragEventHandler(this.pnlNote_DragEnter);
            // 
            // tvNotes
            // 
            this.tvNotes.BackColor = System.Drawing.Color.AliceBlue;
            this.tvNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvNotes.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvNotes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tvNotes.Location = new System.Drawing.Point(0, 0);
            this.tvNotes.Name = "tvNotes";
            this.tvNotes.Size = new System.Drawing.Size(287, 458);
            this.tvNotes.TabIndex = 0;
            this.tvNotes.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvNotes_ItemDrag);
            // 
            // lbNoteName
            // 
            this.lbNoteName.AutoSize = true;
            this.lbNoteName.BackColor = System.Drawing.Color.Transparent;
            this.lbNoteName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbNoteName.ForeColor = System.Drawing.Color.White;
            this.lbNoteName.Location = new System.Drawing.Point(226, 6);
            this.lbNoteName.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.lbNoteName.Name = "lbNoteName";
            this.lbNoteName.Size = new System.Drawing.Size(41, 12);
            this.lbNoteName.TabIndex = 11;
            this.lbNoteName.Text = "label1";
            this.lbNoteName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = global::EMR.Properties.Resources.savesm;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.Location = new System.Drawing.Point(3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(20, 20);
            this.btnSave.TabIndex = 9;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbDepartment
            // 
            this.cbDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDepartment.FormattingEnabled = true;
            this.cbDepartment.Location = new System.Drawing.Point(48, 2);
            this.cbDepartment.Name = "cbDepartment";
            this.cbDepartment.Size = new System.Drawing.Size(121, 20);
            this.cbDepartment.TabIndex = 12;
            this.cbDepartment.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(172, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(46, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FilingSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(851, 483);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbDepartment);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.split);
            this.Controls.Add(this.lbNoteName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FilingSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "科室归档项目设置";
            this.Load += new System.EventHandler(this.FilingSetup_Load);
            this.Resize += new System.EventHandler(this.FilingSetup_Resize);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.TreeView tvNotes;
        private System.Windows.Forms.Panel pnlNote;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lbNoteName;
        private System.Windows.Forms.ComboBox cbDepartment;
        private System.Windows.Forms.Button button1;
    }
}