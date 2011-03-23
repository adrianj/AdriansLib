using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AdriansLib
{
    public partial class ProgressBarForm : Form
    {
        public ProgressBarForm()
        {
            InitializeComponent();
            Show();
        }
        public ProgressBarForm(string title) : this()
        {
            this.Text = title;
        }

        public ProgressBarForm(string title, int maximum)
            : this(title)
        {
            Maximum = maximum;
        }

        public int Minimum { get { return progressBar.Minimum; } set { progressBar.Minimum = value; } }
        public int Maximum { get { return progressBar.Maximum; } set { progressBar.Maximum = value; } }
        public int Value { get { return progressBar.Value; } set { progressBar.Value = value; } }

        public void Increment(int value)
        {
            progressBar.Increment(value);
            this.Refresh();
        }
    }

    /// <summary>
    /// A Progress Bar that operates independently in its own thread.
    /// </summary>
    public class ProgressBarThread
    {
        public string Title { get; set;}
        public long ExpectedTimeToComplete { get; set;}
        private bool mEndThread = false;
        public bool EndThread { get { return mEndThread; } set { mEndThread = value; } }
        public Thread ActualThread { get; set; }

        public ProgressBarThread(string title, long expectedTimeToComplete)
        {
            EndThread = false;
            Title = title;
            if (expectedTimeToComplete <= 0) ExpectedTimeToComplete = 1;
            else ExpectedTimeToComplete = expectedTimeToComplete;
            ActualThread = new Thread(this.ThreadMain);
            ActualThread.Start();
        }

        public void ThreadMain()
        {
            EndThread = false;
            long time = 0;
            int interval = 1;
            ProgressBarForm progForm = new ProgressBarForm(Title);
            progForm.Maximum = 1000;
            if (ExpectedTimeToComplete <= 1000)
            {
                progForm.Maximum = (int)ExpectedTimeToComplete;
            }
            else
            {
                interval = (int)ExpectedTimeToComplete / 1000;
            }
            progForm.Show();
            while (time < ExpectedTimeToComplete && !EndThread)
            {
                progForm.Increment(1);
                Thread.Sleep(interval);
                time += interval;
            }
            progForm.Close();
            this.Close();
        }

        public void Close()
        {
            EndThread = true;
        }
        
    }

}
