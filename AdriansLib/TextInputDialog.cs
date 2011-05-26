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
            if(isCombo[0]) return ""+comboBox[0].SelectedItem;
                else return textBox[0].Text; } }
        public object ResultObject { get 
        {
            if (isCombo[0]) return comboBox[0].SelectedItem;
            else return textBox[0].Text;
        } }

        public object[] ResultObjects
        {
            get
            {
                object[] rObj = new object[isCombo.Length];
                for(int i = 0; i<rObj.Length; i++)
                    if (isCombo[0]) rObj[i] = comboBox[i].SelectedItem;
                    else rObj[i] = textBox[i].Text;
                return rObj;
            }
        }
    
        public string[] ResultStrings { get {
            string[] rObj = new string[isCombo.Length];
            for (int i = 0; i < rObj.Length; i++)
                if (isCombo[0]) rObj[i] = ""+comboBox[i].SelectedItem;
                else rObj[i] = textBox[i].Text;
            return rObj;
        } }

        private bool [] isCombo = new bool [] {false};

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
            isCombo = new bool[query.Length];
            for (int i = 0; i < query.Length; i++ )
            {
                if (initialItem != null && initialItem[i] != null)
                    textBox[i].Text = "" + initialItem[i];
                if (comboBoxItems == null || comboBoxItems[i] == null) isCombo[i] = false;
                else
                {
                    isCombo[i] = true;
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

    }
}
