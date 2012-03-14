using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DTALib
{
    public partial class TextInputDialog : Form
    {
		Control[] controls;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label[] queryLabel;

		public string ResultString { get { return ResultStrings[0]; } }
		public object ResultObject { get { return ResultObjects[0]; } }
  

        public object[] ResultObjects
        {
            get
            {
                object[] rObj = new object[isCombo.Length];
                for(int i = 0; i<rObj.Length; i++)
					rObj[i] = GetControlValue(controls[i]);
                return rObj;
            }
        }

		private object GetControlValue(Control control)
		{
			if (control is ComboBox) return (control as ComboBox).SelectedItem;
			if (control is CheckBox) return (control as CheckBox).Checked;
			if (control is NumericUpDown) return (control as NumericUpDown).Value;
			if (control is ScrollBar) return (control as ScrollBar).Value;
			return control.Text;
		}
    
        public string[] ResultStrings { get {
            string[] rObj = new string[isCombo.Length];
			for (int i = 0; i < rObj.Length; i++)
				rObj[i] = "" + GetControlValue(controls[i]);
            return rObj;
        } }

        // 0 for TextBox, 1 for ComboBox, 2 for other control.
        private int [] isCombo = new int [] {0};

        public TextInputDialog(string query) :this(query, "",null) {  }
        public TextInputDialog(string [] query) : this(query, null, null) { }
        public TextInputDialog(string[] query, object[] initialItems) : this(query, initialItems, null) { }
        public TextInputDialog(string query, object initialItem) : this(query, initialItem, null) { }

        public TextInputDialog(string query,object initialItem, object comboBoxItems)
            : this(new string [] {query},new object [] {initialItem},new object [] {comboBoxItems}){}

        public TextInputDialog(string []query,object []initialItem, object []comboBoxItems)
		{
			InitializeComponent();

			Control[] controls = new Control[query.Length];


            for (int i = 0; i < query.Length; i++ )
            {
				if (comboBoxItems == null || comboBoxItems.Length <= i || comboBoxItems[i] == null)
				{
					controls[i] = new TextBox(); 
					if (initialItem != null && initialItem[i] != null)
						controls[i].Text = "" + initialItem[i];
					if (i == query.Length - 1)
						(controls[i] as TextBox).Multiline = true;
				}
				else
				{
					controls[i] = new ComboBox();
					(controls[i] as ComboBox).DataSource = comboBoxItems.Clone();
					if (initialItem != null && initialItem[i] != null)
						(controls[i] as ComboBox).SelectedItem = initialItem[i];
				}
            }


			AddAllControls(query, controls);
        }

       

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        #region General Purpose Controls

        public TextInputDialog(string[] query, Control[] givenControls)
        {
			InitializeComponent();
            AddAllControls(query, givenControls);
        }

        private void AddAllControls(string [] query,Control [] givenControls)
        {
			this.SuspendLayout();

			AddButtons();

			int topOfNextRow = AddControls(query, givenControls);

            okButton.Top = topOfNextRow;
            cancelButton.Top = topOfNextRow;

			this.StartPosition = FormStartPosition.CenterParent;
			this.Height = topOfNextRow + okButton.Height + 30;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

		private int AddControls(string[] query, Control[] givenControls)
		{
			int nFields = givenControls.Length;
			controls = givenControls;
			isCombo = new int[nFields];
			string[] queries = query;
			if (query.Length != nFields)
			{
				queries = new string[nFields];
				Array.Copy(query, queries, Math.Min(nFields, query.Length));
			}

			queryLabel = new Label[nFields];
			for (int i = 0; i < nFields; i++)
			{
				this.queryLabel[i] = new System.Windows.Forms.Label();
			}

			int top = 5;

			for (int i = 0; i < nFields; i++)
			{
				// 
				// queryLabel
				// 
				this.queryLabel[i].AutoSize = true;
				this.queryLabel[i].Location = new System.Drawing.Point(12, top);
				this.queryLabel[i].Name = "query" + i;
				this.queryLabel[i].Size = new System.Drawing.Size(this.Width - 40, 13);
				this.queryLabel[i].TabIndex = 3;
				this.queryLabel[i].Text = query[i];
				if(!string.IsNullOrWhiteSpace(query[i]))
					top += queryLabel[i].Height + 5;
				// 
				// generalControl
				// 
				controls[i].Location = new System.Drawing.Point(12, top);
				controls[i].Name = "given" + i;
				controls[i].Width = this.Width - 40;
				controls[i].TabIndex = 4;
				controls[i].Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
				controls[i].Visible = true;
				controls[i].Enabled = true;
				if (i == nFields - 1)
					controls[i].Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;

				if (controls[i] is ComboBox)
					isCombo[i] = 1;
				else if (controls[i] is TextBox)
					isCombo[i] = 0;
				else
					isCombo[i] = 2;

				this.Controls.Add(this.queryLabel[i]);
				this.Controls.Add(controls[i]);

				top += controls[i].Height + 5;
			}
			return top;
		}

		private void AddButtons()
		{
			okButton = new Button();
			cancelButton = new Button();
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
			this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
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
			this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

			cancelButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			okButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			
			this.CancelButton = cancelButton;
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
		}
        #endregion

		private void TextInputDialog_Shown(object sender, EventArgs e)
		{
			controls[0].Focus();
		}
    }
}
