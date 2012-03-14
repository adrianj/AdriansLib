using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DTALib;

namespace AdriansLibClient
{
	public partial class Test_TextInputDialog : Form, ITestClass
	{
		public Test_TextInputDialog()
		{
			InitializeComponent();
		}

		public string RunTests(ref int failCount, ref int testCount)
		{
			this.ShowDialog();
			return "";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string[] labels = new string[] { "Is", "Testing", "Good?" };
			object[] initial = new object[] { "Testing", "Is", "Good" };
			TextInputDialog tid = new TextInputDialog(labels, initial);
			ShowAndPresentResults(tid);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string[] labels = new string[] { "Is", "Testing", "Good?" };
			object[] initial = new object[] { "Testing", "Is", "Good" };
			object[] comboItems = new object[] { true, false, 1, 1.6f, "bad" };
			TextInputDialog tid = new TextInputDialog(labels, initial, comboItems);
			ShowAndPresentResults(tid);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			string[] labels = new string[] { "Is", "Testing", "Good?", "Yeah!" };
			List<Control> controls = new List<Control>();
			Control c = new NumericUpDown();
			(c as NumericUpDown).Value = 15;
			controls.Add(c);
			c = new CheckBox();
			(c as CheckBox).Checked = true;
			controls.Add(c);
			c = new RichTextBox();
			controls.Add(c);
			c = new ComboBox();
			(c as ComboBox).DataSource = Enum.GetValues(typeof(DialogResult));
			controls.Add(c);
			TextInputDialog tid = new TextInputDialog(labels, controls.ToArray());
			ShowAndPresentResults(tid);
		}

		private void ShowAndPresentResults(TextInputDialog tid)
		{
			DialogResult res = tid.ShowDialog();
			resultBox.Text = "" + res + Environment.NewLine;
			foreach (object o in tid.ResultObjects)
				resultBox.Text += "" + o + "("+o.GetType()+"), ";
			resultBox.Text += Environment.NewLine;
			foreach (string o in tid.ResultStrings)
				resultBox.Text += o + ",";
		}
	}
}
