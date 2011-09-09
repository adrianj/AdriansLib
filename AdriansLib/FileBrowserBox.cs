using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;

namespace DTALib
{
    public partial class FileBrowserBox : UserControl
    {
        public bool Directory { get; set; }
        private bool mSaveFile = false;
        public bool SaveFile { get {  return mSaveFile; }
            set {  mSaveFile = value; }
        }
        public string Filename
        {
            get {
                if (mMaxPreviousFiles == 0)
                    return textBox.Text;
                else
                    return (string)comboBox.Text;
            }
            set
            {
                textBox.Text = value;
                updateComboBox(value);
                FireFilenameChangedEvent();
            }
        }
        
        public string InitialDirectory { get; set; }
        public bool TextBoxReadOnly { get { return textBox.ReadOnly; } set { textBox.ReadOnly = value; } }
        public string Filter { get; set; }
        public override string Text
        {
            get
            {
                return Filename;
            }
            set
            {
                Filename = value;
            }
        }

        public string Label
        {
            get { return label.Text; }
            set
            {
                label.Text = value;
                int adj = textBox.Left - label.Width - 3;
                if (label.Text.Length == 0)
                    adj = textBox.Left;
                textBox.Left -= adj;
                comboBox.Left -= adj;
                textBox.Width += adj;
                comboBox.Width += adj;
            }
        }

        private int mMaxPreviousFiles;
        public int MaxPreviousFiles
        {
            get
            {
                return mMaxPreviousFiles;
            }
            set
            {
                mMaxPreviousFiles = value;
                if (mMaxPreviousFiles > 0)
                {
                    comboBox.Visible = true;
                    textBox.Visible = false;
                }
                else
                {
                    comboBox.Visible = false;
                    textBox.Visible = true;
                }
            }
        }

        public event EventHandler FilenameChanged;
        private string previousFilename = null;
        private void FireFilenameChangedEvent()
        {
            string newFile = Filename;
            if (newFile != null)
            {
                if (previousFilename == null || !newFile.Equals(previousFilename))
                {
                    if (FilenameChanged != null) FilenameChanged(this, new EventArgs());
                    previousFilename = Filename;
                }
            }
        }

        public void updateComboBox(string value)
        {
            if(!comboBox.Items.Contains(value)) comboBox.Items.Add(value);
            comboBox.SelectedItem = value;
            comboBox.Select(0, 0);
        }

        public FileBrowserBox(int maxFiles, bool saveFile) : this("", maxFiles, saveFile) { }
        public FileBrowserBox(int maxFiles) : this("", maxFiles,false) { }
        public FileBrowserBox(string filename) : this(filename, 5,false) { }
        public FileBrowserBox() : this("",5,false) {  }
        public FileBrowserBox(string filename, int maxFiles, bool saveFile)
        {
            InitializeComponent();
            RestoreDefaults();
            AttachToolTipToComboBox(comboBox);
            Filename = filename;
            MaxPreviousFiles = maxFiles;
            SaveFile = saveFile;
        }

        public void RestoreDefaults()
        {
            InitialDirectory = "";
            Filter = "All Files (*.*)|*.*";
            Filename = "";
            Label = "";
            MaxPreviousFiles = 5;
            SaveFile = false;
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (Directory)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.SelectedPath = this.Filename;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Filename = fbd.SelectedPath;
                }
            }
            else
            {
                if (SaveFile) // Use a saveFileDialog, that may or may not overwrite
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    if (Filename == null || Filename.Length < 1)
                        sfd.InitialDirectory = InitialDirectory;
                    else
                        sfd.InitialDirectory = Path.GetDirectoryName(Filename);
                    sfd.AddExtension = true;
                    sfd.Filter = Filter;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                            Filename = sfd.FileName;
                    }
                }
                else // Use an openFileDialog
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    if (Filename == null || Filename.Length < 1)
                        ofd.InitialDirectory = InitialDirectory;
                    else
                        ofd.InitialDirectory = Path.GetDirectoryName(Filename);
                    ofd.AddExtension = true;
                    ofd.Filter = Filter;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                            Filename = ofd.FileName;
                    }
                }
            }
        }

        public void AttachToolTipToComboBox(ComboBox newC)
        {
            ToolTip tt = new ToolTip();
            newC.Tag = tt;
            newC.DrawMode = DrawMode.OwnerDrawFixed;
            newC.DrawItem += comboBox1_DrawItem;
            newC.DropDownClosed += comboBox1_DropDownClosed;
        }

        private static void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(ComboBox)) return;
            ComboBox newC = (ComboBox)sender;
            if (newC.Tag.GetType() != typeof(ToolTip)) return;
            ToolTip tt = (ToolTip)newC.Tag;
            tt.Hide(newC);
        }

        private static void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (sender.GetType() != typeof(ComboBox)) return;
            ComboBox newC = (ComboBox)sender;
            if (newC.Tag.GetType() != typeof(ToolTip)) return;
            ToolTip tt = (ToolTip)newC.Tag;
            string text = newC.GetItemText(newC.Items[e.Index]);
            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            { e.Graphics.DrawString(text, e.Font, br, e.Bounds); }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            { tt.Show(text, newC, e.Bounds.Right, e.Bounds.Bottom); }
            e.DrawFocusRectangle();
        }

        public new event EventHandler TextChanged;

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (sender == comboBox)
            {
                if (TextChanged != null) TextChanged(this, e);
                //FireFilenameChangedEvent();
            }
        }

        private void textBox_Validating(object sender, CancelEventArgs e)
        {
            Filename = textBox.Text;
            
        }


        private void textBox_Leave_1(object sender, EventArgs e)
        {
            if(!(sender is Control)) return;
            Control c = sender as Control;
            Filename = c.Text;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedItem != null)
                Filename = comboBox.SelectedItem as string;
        }

		private void comboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
				Filename = comboBox.Text;
		}
    }

}
