using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdriansLib
{
    /// <summary>
    /// A class that wraps around the progressBarControl.
    /// </summary>
    public partial class ProgressBarForm : Form
    {
        public new DialogResult DialogResult { get { return progBar.DialogResult; } }
        public object Result { get { return progBar.Result; } }
        public bool CanCancel { get { return progBar.CanCancel; } set { progBar.CanCancel = value; } }
        public int ButtonWidth { get { return progBar.ButtonWidth; } set { progBar.ButtonWidth = value; } }
        public string InfoText { get { return progBar.Text; } set { progBar.Text = value; } }
        public bool EnableTextBox
        {
            get { return progBar.EnableTextBox; }
            set
            {
                progBar.EnableTextBox = value;
                if (value)
                {
                    this.Height = 180;
                }
                else
                {
                    this.Height = 80;
                }
            }
        }
        public ProgressBarForm(string title) : this(title,false) { }
        public ProgressBarForm(string title, bool enableTextBox) : this(title, 100,enableTextBox) { }
        public ProgressBarForm(string title, int maximum) : this(title, maximum,false) { }
        public ProgressBarForm(string title, int maximum, bool enableTextbox)
            : this()
        {
            this.Text = title;
            this.EnableTextBox = enableTextbox;
            progBar.Maximum = maximum;
        }
        public ProgressBarForm()
        {
            InitializeComponent();
            
        }
        public void Increment(int i) { progBar.Increment(i);}
        public void StartWorker(WorkerFunction function) { StartWorker(function, null); }
        public void StartWorker(WorkerFunction function, object parameter) { 
            progBar.StartWorker(function, parameter);
            //Application.Run(this);
            this.ShowDialog();
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted;
        private void progBar_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(RunWorkerCompleted != null)
                RunWorkerCompleted(this, e);
            this.Dispose();
        }

    }
}
