namespace DTALib
{
    partial class FileBrowserBox
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
			this.textBox = new System.Windows.Forms.TextBox();
			this.button = new System.Windows.Forms.Button();
			this.comboBox = new System.Windows.Forms.ComboBox();
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(32, 0);
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.Size = new System.Drawing.Size(194, 20);
			this.textBox.TabIndex = 0;
			this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			this.textBox.Leave += new System.EventHandler(this.textBox_Leave_1);
			// 
			// button
			// 
			this.button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.button.Location = new System.Drawing.Point(226, 0);
			this.button.Name = "button";
			this.button.Size = new System.Drawing.Size(30, 21);
			this.button.TabIndex = 1;
			this.button.Text = "...";
			this.button.UseVisualStyleBackColor = true;
			this.button.Click += new System.EventHandler(this.button_Click);
			// 
			// comboBox
			// 
			this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox.FormattingEnabled = true;
			this.comboBox.Location = new System.Drawing.Point(32, 0);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new System.Drawing.Size(194, 21);
			this.comboBox.TabIndex = 2;
			this.comboBox.Visible = false;
			this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
			this.comboBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			this.comboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox_KeyDown);
			this.comboBox.Leave += new System.EventHandler(this.textBox_Leave_1);
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(3, 3);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(0, 13);
			this.label.TabIndex = 3;
			// 
			// FileBrowserBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label);
			this.Controls.Add(this.comboBox);
			this.Controls.Add(this.button);
			this.Controls.Add(this.textBox);
			this.Name = "FileBrowserBox";
			this.Size = new System.Drawing.Size(256, 21);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button button;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Label label;
    }
}
