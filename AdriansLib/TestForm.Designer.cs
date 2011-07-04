namespace DTALib
{
    partial class TestForm
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
            this.runButton = new System.Windows.Forms.Button();
            this.testCombo = new System.Windows.Forms.ComboBox();
            this.failCountBox = new System.Windows.Forms.TextBox();
            this.failMsgBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(12, 12);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(109, 23);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run Test";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // testCombo
            // 
            this.testCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testCombo.FormattingEnabled = true;
            this.testCombo.Location = new System.Drawing.Point(127, 14);
            this.testCombo.Name = "testCombo";
            this.testCombo.Size = new System.Drawing.Size(387, 21);
            this.testCombo.TabIndex = 1;
            // 
            // failCountBox
            // 
            this.failCountBox.Location = new System.Drawing.Point(12, 41);
            this.failCountBox.Name = "failCountBox";
            this.failCountBox.Size = new System.Drawing.Size(109, 20);
            this.failCountBox.TabIndex = 2;
            // 
            // failMsgBox
            // 
            this.failMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.failMsgBox.Location = new System.Drawing.Point(127, 41);
            this.failMsgBox.Multiline = true;
            this.failMsgBox.Name = "failMsgBox";
            this.failMsgBox.Size = new System.Drawing.Size(387, 67);
            this.failMsgBox.TabIndex = 3;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 120);
            this.Controls.Add(this.failMsgBox);
            this.Controls.Add(this.failCountBox);
            this.Controls.Add(this.testCombo);
            this.Controls.Add(this.runButton);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.ComboBox testCombo;
        private System.Windows.Forms.TextBox failCountBox;
        private System.Windows.Forms.TextBox failMsgBox;
    }
}