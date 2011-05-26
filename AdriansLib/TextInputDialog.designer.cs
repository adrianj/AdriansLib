using System.Windows.Forms;

namespace AdriansLib
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
        private void InitializeComponent(int nFields)
        {

            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            textBox = new TextBox [nFields];
            queryLabel = new Label[nFields];
            comboBox = new ComboBox[nFields];
            for (int i = 0; i < nFields; i++)
            {
                this.textBox[i] = new System.Windows.Forms.TextBox();
                this.queryLabel[i] = new System.Windows.Forms.Label();
                this.comboBox[i] = new System.Windows.Forms.ComboBox();
            }
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(12, 63);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(113, 63);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click); 
            for (int i = 0; i < nFields; i++)
            {
                // 
                // textBox
                // 
                this.textBox[i].Location = new System.Drawing.Point(12, 26+PixelsPerField*i);
                this.textBox[i].Name = "textBox";
                this.textBox[i].Size = new System.Drawing.Size(176, 20);
                this.textBox[i].TabIndex = 0;
                // 
                // queryLabel
                // 
                this.queryLabel[i].AutoSize = true;
                this.queryLabel[i].Location = new System.Drawing.Point(12, 9 + PixelsPerField * i);
                this.queryLabel[i].Name = "queryLabel";
                this.queryLabel[i].Size = new System.Drawing.Size(35, 13);
                this.queryLabel[i].TabIndex = 3;
                this.queryLabel[i].Text = "label1";
                // 
                // comboBox
                // 
                this.comboBox[i].Enabled = false;
                this.comboBox[i].FormattingEnabled = true;
                this.comboBox[i].Location = new System.Drawing.Point(12, 26 + PixelsPerField * i);
                this.comboBox[i].Name = "comboBox";
                this.comboBox[i].Size = new System.Drawing.Size(176, 21);
                this.comboBox[i].TabIndex = 4;
                this.comboBox[i].Visible = false;
            }
            // 
            // TextInputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 97);
            
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            for (int i = 0; i < nFields; i++)
            {
                this.Controls.Add(this.comboBox[i]);
                this.Controls.Add(this.queryLabel[i]);
                this.Controls.Add(this.textBox[i]);
            }
            this.Name = "TextInputDialog";
            this.Text = "TextInputDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox [] textBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label [] queryLabel;
        private System.Windows.Forms.ComboBox [] comboBox;
    }
}