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
        public bool Cancelled { get { return progBar.Cancelled; } }
        public object Result { get { return progBar.Result; } }

        public ProgressBarForm(string title, int maximum)
            : this()
        {
            this.Text = title;
            progBar.Maximum = maximum;
        }
        public ProgressBarForm(string title) : this(title, 100) { }
        public ProgressBarForm(int maximum) : this("Progress", maximum) { }
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

        private void progBar_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispose();
        }

    }
}
