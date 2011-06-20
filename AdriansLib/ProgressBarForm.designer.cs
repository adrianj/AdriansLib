namespace DTALib
{
    partial class ProgressBarForm
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
            this.progBar = new DTALib.ProgressBarControl();
            this.SuspendLayout();
            // 
            // progBar
            // 
            this.progBar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.progBar.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.progBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progBar.EnableTextBox = true;
            this.progBar.Location = new System.Drawing.Point(0, 0);
            this.progBar.Maximum = 100;
            this.progBar.Minimum = 0;
            this.progBar.Name = "progBar";
            this.progBar.Result = null;
            this.progBar.Size = new System.Drawing.Size(707, 53);
            this.progBar.TabIndex = 0;
            this.progBar.Value = 0;
            this.progBar.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.progBar_RunWorkerCompleted);
            // 
            // ProgressBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 53);
            this.ControlBox = false;
            this.Controls.Add(this.progBar);
            this.Name = "ProgressBarForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProgressBarForm";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private ProgressBarControl progBar;
    }
}