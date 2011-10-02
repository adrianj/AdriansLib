namespace DTALib
{
	partial class ExceptionRetryDialog
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.abortButton = new System.Windows.Forms.Button();
			this.ignoreButton = new System.Windows.Forms.Button();
			this.retryButton = new System.Windows.Forms.Button();
			this.nameLabel = new System.Windows.Forms.Label();
			this.stackTraceBox = new System.Windows.Forms.TextBox();
			this.descLabel = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.Controls.Add(this.abortButton, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this.ignoreButton, 4, 7);
			this.tableLayoutPanel1.Controls.Add(this.retryButton, 2, 7);
			this.tableLayoutPanel1.Controls.Add(this.nameLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.stackTraceBox, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.descLabel, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 8;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(473, 404);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// abortButton
			// 
			this.abortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.abortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.abortButton.Location = new System.Drawing.Point(3, 355);
			this.abortButton.Name = "abortButton";
			this.abortButton.Size = new System.Drawing.Size(88, 23);
			this.abortButton.TabIndex = 0;
			this.abortButton.Text = "Abort";
			this.abortButton.UseVisualStyleBackColor = true;
			this.abortButton.Click += new System.EventHandler(this.Button_Click);
			// 
			// ignoreButton
			// 
			this.ignoreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ignoreButton.Location = new System.Drawing.Point(379, 355);
			this.ignoreButton.Name = "ignoreButton";
			this.ignoreButton.Size = new System.Drawing.Size(91, 23);
			this.ignoreButton.TabIndex = 1;
			this.ignoreButton.Text = "Ignore";
			this.ignoreButton.UseVisualStyleBackColor = true;
			this.ignoreButton.Click += new System.EventHandler(this.Button_Click);
			// 
			// retryButton
			// 
			this.retryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.retryButton.Location = new System.Drawing.Point(191, 355);
			this.retryButton.Name = "retryButton";
			this.retryButton.Size = new System.Drawing.Size(88, 23);
			this.retryButton.TabIndex = 2;
			this.retryButton.Text = "Retry";
			this.retryButton.UseVisualStyleBackColor = true;
			this.retryButton.Click += new System.EventHandler(this.Button_Click);
			// 
			// nameLabel
			// 
			this.nameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.nameLabel.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.nameLabel, 5);
			this.nameLabel.Location = new System.Drawing.Point(3, 3);
			this.nameLabel.Margin = new System.Windows.Forms.Padding(3);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(35, 13);
			this.nameLabel.TabIndex = 3;
			this.nameLabel.Text = "label1";
			// 
			// stackTraceBox
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.stackTraceBox, 5);
			this.stackTraceBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.stackTraceBox.Location = new System.Drawing.Point(3, 41);
			this.stackTraceBox.Multiline = true;
			this.stackTraceBox.Name = "stackTraceBox";
			this.stackTraceBox.ReadOnly = true;
			this.tableLayoutPanel1.SetRowSpan(this.stackTraceBox, 4);
			this.stackTraceBox.Size = new System.Drawing.Size(467, 286);
			this.stackTraceBox.TabIndex = 4;
			// 
			// descLabel
			// 
			this.descLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.descLabel.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.descLabel, 5);
			this.descLabel.Location = new System.Drawing.Point(3, 22);
			this.descLabel.Margin = new System.Windows.Forms.Padding(3);
			this.descLabel.Name = "descLabel";
			this.tableLayoutPanel1.SetRowSpan(this.descLabel, 2);
			this.descLabel.Size = new System.Drawing.Size(35, 13);
			this.descLabel.TabIndex = 5;
			this.descLabel.Text = "label1";
			// 
			// ExceptionRetryDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(473, 404);
			this.ControlBox = false;
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ExceptionRetryDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ExceptionRetryDialog";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button abortButton;
		private System.Windows.Forms.Button ignoreButton;
		private System.Windows.Forms.Button retryButton;
		private System.Windows.Forms.Label nameLabel;
		private System.Windows.Forms.TextBox stackTraceBox;
		private System.Windows.Forms.Label descLabel;
	}
}