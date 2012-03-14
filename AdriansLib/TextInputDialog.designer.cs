using System.Windows.Forms;

namespace DTALib
{
    partial class TextInputDialog
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
			this.SuspendLayout();
			// 
			// TextInputDialog
			// 
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Name = "TextInputDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Shown += new System.EventHandler(this.TextInputDialog_Shown);
			this.ResumeLayout(false);

        }

        #endregion

    }
}