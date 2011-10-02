using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DTALib
{
	public partial class ExceptionRetryDialog : Form
	{
		public string ExceptionName { get { return this.Text; } set { this.Text = value; this.nameLabel.Text = value; } }



		public ExceptionRetryDialog()
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Abort;
			InitializeComponent();
		}

		public void PopulateWithException(Exception ex)
		{
			this.ExceptionName = ex.GetType().Name;
			this.descLabel.Text = ex.Message;
			this.stackTraceBox.Text = "" + ex;
		}

		public static DialogResult ShowDialog(Exception ex) { return ShowDialog(ex, "", DialogResult.Retry | DialogResult.Ignore | DialogResult.Abort); }
		public static DialogResult ShowDialog(Exception ex, string caption, DialogResult enabledButtons)
		{
			ExceptionRetryDialog dlg = new ExceptionRetryDialog();
			dlg.PopulateWithException(ex);
			if (!string.IsNullOrWhiteSpace(caption))
				dlg.ExceptionName = caption;
			if (!enabledButtons.HasFlag(DialogResult.Abort))
				dlg.abortButton.Enabled = false;
			if (!enabledButtons.HasFlag(DialogResult.Ignore))
				dlg.ignoreButton.Enabled = false;
			if (!enabledButtons.HasFlag(DialogResult.Retry))
				dlg.retryButton.Enabled = false;
			return dlg.ShowDialog();
		}

		private void Button_Click(object sender, EventArgs e)
		{
			if (sender == ignoreButton)
				this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			else if (sender == retryButton)
				this.DialogResult = System.Windows.Forms.DialogResult.Retry;

			this.Close();
		}
	}
}
