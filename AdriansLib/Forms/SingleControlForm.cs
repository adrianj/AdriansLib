using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DTALib.Forms
{
	[ToolboxItem(true)]
	public partial class SingleControlForm : Form
	{
		public enum ButtonOptions { None, OK, OKCancel };

		ButtonOptions buttons = ButtonOptions.OKCancel;
		public ButtonOptions Buttons { get { return buttons; } set { buttons = value; UpdateButtons(); } }

		private Control control;
		public Control Control
		{
			get
			{
				return control;
			}
			set
			{
				if (control != null)
					this.Controls.Remove(control);
				control = value;
				if (control == null) return;
				control.Visible = true;
				control.Dock = DockStyle.Fill;
				SetToPreferredSize();
				this.tableLayoutPanel1.Controls.Add(control, 0, 0);
				this.tableLayoutPanel1.SetColumnSpan(control, 3);
			}
		}

		private void SetToPreferredSize()
		{
			this.Size = new System.Drawing.Size(control.Width + 12, control.Height + 30);
			
			Size pref = control.PreferredSize;

			if (pref.Height == 0 && pref.Width == 0)
				pref = control.MinimumSize;
			if (pref.Height == 0 && pref.Width == 0)
				return;
			int width = pref.Width;
			if (width < MinimumSize.Width)
				width = MinimumSize.Width;
			int height = pref.Height;
			if (buttons != ButtonOptions.None)
				height += MinimumSize.Height;
			this.MinimumSize = new Size(width+12, height+30);
			//this.Size = MinimumSize;
		}

		public SingleControlForm()
		{
			InitializeComponent();
			this.StartPosition = FormStartPosition.CenterScreen;
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		void UpdateButtons()
		{
			this.tableLayoutPanel1.Controls.Remove(okButton);
			this.tableLayoutPanel1.Controls.Remove(cancelButton);

			if (buttons == ButtonOptions.OKCancel)
			{
				this.tableLayoutPanel1.Controls.Add(okButton, 0, 1);
				this.tableLayoutPanel1.Controls.Add(cancelButton, 2, 1);
			}
			else if (buttons == ButtonOptions.OK)
			{
				this.tableLayoutPanel1.Controls.Add(okButton, 1, 1);
			}
		}

		private void buttonClick(object sender, EventArgs e)
		{
			if (sender == okButton)
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			if (sender == cancelButton)
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}

		private void SingleControlForm_SizeChanged(object sender, EventArgs e)
		{
		}

	}
}
