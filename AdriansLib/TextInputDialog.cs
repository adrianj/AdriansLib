using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AdriansLib
{
    public partial class TextInputDialog : Form
    {
        public static int PixelsPerField = 42;

        public string ResultString { get {
            if (isCombo[0] == 1) return "" + comboBox[0].SelectedItem;
                else return textBox[0].Text; } }
        public object ResultObject { get 
        {
            if (isCombo[0] == 1) return comboBox[0].SelectedItem;
            else return textBox[0].Text;
        } }

        public object[] ResultObjects
        {
            get
            {
                object[] rObj = new object[isCombo.Length];
                for(int i = 0; i<rObj.Length; i++)
                    if (isCombo[0] == 1) rObj[i] = comboBox[i].SelectedItem;
                    else rObj[i] = textBox[i].Text;
                return rObj;
            }
        }
    
        public string[] ResultStrings { get {
            string[] rObj = new string[isCombo.Length];
            for (int i = 0; i < rObj.Length; i++)
                if (isCombo[0]==1) rObj[i] = ""+comboBox[i].SelectedItem;
                else rObj[i] = textBox[i].Text;
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
            InitializeComponent(query.Length);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Height = PixelsPerField * query.Length + 70;
            okButton.Top = this.Bottom - 55;
            cancelButton.Top = this.Bottom - 55;
            isCombo = new int[query.Length];
            for (int i = 0; i < query.Length; i++ )
            {
                if (initialItem != null && initialItem[i] != null)
                    textBox[i].Text = "" + initialItem[i];
                if (comboBoxItems == null || comboBoxItems[i] == null) isCombo[i] = 0;
                else
                {
                    isCombo[i] = 1;
                    comboBox[i].Visible = true;
                    comboBox[i].Enabled = true;
                    comboBox[i].DataSource = comboBoxItems[i];
                    comboBox[i].SelectedItem = initialItem[i];
                    textBox[i].Visible = false;
                    textBox[i].Enabled = false;
                }
                queryLabel[i].Text = query[i];
                comboBox[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                textBox[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
            textBox[query.Length - 1].Multiline = true;
            textBox[query.Length - 1].Anchor = textBox[query.Length - 1].Anchor | AnchorStyles.Bottom;
            cancelButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            okButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
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
            InitializeComponent(query, givenControls);
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(string [] query,Control [] givenControls)
        {
            this.Text = "";
            int nFields = givenControls.Length;
            string[] queries = query;
            if (query.Length != nFields)
            {
                queries = new string[nFields];
                Array.Copy(query,queries,Math.Min(nFields,query.Length));
            }

            this.StartPosition = FormStartPosition.CenterParent;
            this.Height = PixelsPerField * nFields + 70;

            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            queryLabel = new Label[nFields];
            for (int i = 0; i < nFields; i++)
            {
                this.queryLabel[i] = new System.Windows.Forms.Label();
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

            cancelButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            okButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            int top = 5;

            for (int i = 0; i < nFields; i++)
            {
                // 
                // queryLabel
                // 
                this.queryLabel[i].AutoSize = true;
                this.queryLabel[i].Location = new System.Drawing.Point(12, top);
                this.queryLabel[i].Name = "query"+i;
                this.queryLabel[i].Size = new System.Drawing.Size(35, 13);
                this.queryLabel[i].TabIndex = 3;
                this.queryLabel[i].Text = query[i];
                top += queryLabel[i].Height + 5;
                // 
                // generalControl
                // 
                givenControls[i].Location = new System.Drawing.Point(12, top);
                givenControls[i].Name = "given"+i;
                givenControls[i].Width = this.Width-40;
                givenControls[i].TabIndex = 4;
                givenControls[i].Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                if(i == nFields-1)
                    givenControls[i].Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
                top += givenControls[i].Height + 5;
            }
            okButton.Top = top;
            cancelButton.Top = top;
            // 
            // TextInputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(200, 97);

            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            for (int i = 0; i < nFields; i++)
            {
                this.Controls.Add(this.queryLabel[i]);
                this.Controls.Add(givenControls[i]);
            }
            this.Name = "TextInputDialog";
            this.Text = "TextInputDialog";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
    }
}
