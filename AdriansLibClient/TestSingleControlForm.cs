using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DTALib;
using DTALib.Forms;
using System.Windows.Forms;

namespace AdriansLibClient
{
	[TestFixture]
	public class TestSingleControlForm : UserControl, ITestClass
	{
		private FormDialog form;
	
		public TestSingleControlForm()
		{
			this.InitializeComponent();
		}
	
		public string RunTests(ref int failCount, ref int testCount)
		{
			//RichTextBox control = new RichTextBox();
			//form.Control = control;
			form.Text = "Test single form";
			DialogResult res = form.ShowDialog();
			Console.WriteLine("" + res);
			form.Show();
			res = form.DialogResult;
			Console.WriteLine("" + res);
			return "done";
		}

		private void InitializeComponent()
		{
			this.form = new DTALib.Forms.FormDialog();
			this.SuspendLayout();
			// 
			// form
			// 
			this.form.Buttons = DTALib.Forms.SingleControlForm.ButtonOptions.OKCancel;
			this.form.Control = null;
			this.form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.form.Text = "SingleControlForm";
			// 
			// TestSingleControlForm
			// 
			this.Name = "TestSingleControlForm";
			this.Size = new System.Drawing.Size(331, 298);
			this.ResumeLayout(false);

		}
	}
}
