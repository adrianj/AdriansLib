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
            Application.Run(this);
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted;
        private void progBar_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RunWorkerCompleted(sender, e);
            this.Dispose();
        }

    }
}
