namespace EMR
{
    partial class SetIndividualOptions
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
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.individualOptions1 = new EMR.IndividualOptions();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Location = new System.Drawing.Point(322, 204);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(65, 23);
            this.btnExit.TabIndex = 66;
            this.btnExit.Text = "关 闭";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // individualOptions1
            // 
            this.individualOptions1.BackColor = System.Drawing.Color.Transparent;
            this.individualOptions1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.individualOptions1.Location = new System.Drawing.Point(1, 1);
            this.individualOptions1.Name = "individualOptions1";
            this.individualOptions1.Size = new System.Drawing.Size(397, 197);
            this.individualOptions1.TabIndex = 1;
            // 
            // SetIndividualOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(399, 252);
            this.ControlBox = false;
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.individualOptions1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetIndividualOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "设置局部选项";
            this.ResumeLayout(false);

        }

        #endregion

        private IndividualOptions individualOptions1;
        private DevComponents.DotNetBar.ButtonX btnExit;

    }
}