namespace EMR
{
    partial class FilingUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbNote = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbNote
            // 
            this.lbNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(220)))));
            this.lbNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbNote.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lbNote.Location = new System.Drawing.Point(0, 0);
            this.lbNote.Name = "lbNote";
            this.lbNote.Size = new System.Drawing.Size(273, 21);
            this.lbNote.TabIndex = 0;
            this.lbNote.Text = "label1";
            this.lbNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbNote.MouseLeave += new System.EventHandler(this.FilingUC_MouseLeave);
            this.lbNote.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbNote_MouseClick);
            this.lbNote.MouseEnter += new System.EventHandler(this.FilingUC_MouseEnter);
            // 
            // FilingUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Controls.Add(this.lbNote);
            this.Name = "FilingUC";
            this.Size = new System.Drawing.Size(273, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbNote;
    }
}
