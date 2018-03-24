namespace EMR
{
    partial class frmEquation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEquation));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.mathMLControl = new MathMLControl.MathMLControl();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(466, 337);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "确定(&O)";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(547, 337);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // mathMLControl
            // 
            this.mathMLControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mathMLControl.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.mathMLControl.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.mathMLControl.BackColor = System.Drawing.SystemColors.Control;
            this.mathMLControl.Location = new System.Drawing.Point(0, 0);
            this.mathMLControl.MC_BackgroundColor = System.Drawing.SystemColors.Control;
            this.mathMLControl.MC_Configuration = "";
            this.mathMLControl.MC_Enable_BuiltIn_SaveChangesDialog = true;
            this.mathMLControl.MC_EnableDefaultKeyboardShortcuts = true;
            this.mathMLControl.MC_EnableGreekModeKeyboardShortcut = true;
            this.mathMLControl.MC_EnableInsertClosingBrackets = true;
            this.mathMLControl.MC_EnableStretchyBrackets = true;
            this.mathMLControl.MC_EntityHandling_Clipboard = MathMLControl.enum_EntityHandling.EntityNames;
            this.mathMLControl.MC_EntityHandling_File = MathMLControl.enum_EntityHandling.EntityNames;
            this.mathMLControl.MC_FontSize = 16F;
            this.mathMLControl.MC_ShowInfoBox = false;
            this.mathMLControl.MC_Toolbar = MathMLControl.enum_Toolbar.custom;
            this.mathMLControl.MC_ToolbarLayout_Designtime = resources.GetString("mathMLControl.MC_ToolbarLayout_Designtime");
            this.mathMLControl.MC_UnsavedChanges = false;
            this.mathMLControl.MC_UseDefaultContextMenu = true;
            this.mathMLControl.Name = "mathMLControl";
            this.mathMLControl.Size = new System.Drawing.Size(634, 322);
            this.mathMLControl.TabIndex = 3;
            // 
            // frmEquation
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(624, 362);
            this.Controls.Add(this.mathMLControl);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "frmEquation";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加公式";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private MathMLControl.MathMLControl mathMLControl;


    }
}