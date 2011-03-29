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
    /// <summary>
    /// This is the pattern for any functions to be rnu by the background worker of ProgressBarForm.
    /// Typical pattern is: 
    /// 1. Create instance of ProgressBarForm
    /// 2. Call form.StartWorker([function matching delegate],[parameter is optional])
    /// 3. Function does its thing, then eventually form.Cancelled and form.Result are available.
    /// Meanwhile, the function can report progress using e.ReportProgress and should be polling worker.CancellationPending
    /// </summary>
    /// <param name="parameter">Function input. Can be anything.</param>
    /// <param name="worker">The worker doing the work. Be sure to check CancellationPending and then set e.Cancel = true if it is.</param>
    /// <param name="e">Worker event args. Gives mechanism to report or cancel.</param>
    /// <returns>An object. Cast as appropriate.</returns>
    public delegate object WorkerFunction(object parameter, BackgroundWorker worker, DoWorkEventArgs e);

    public partial class ProgressBarControl : UserControl
    {
        public ProgressBarControl()
        {
            this.Cancelled = false;
            this.Result = null;
            InitializeComponent();
        }

        public ProgressBarControl(bool enableTextBox) : this(100,enableTextBox){}
        public ProgressBarControl(int maximum) : this(maximum, false) { }
        public ProgressBarControl(int maximum, bool enableTextBox) : this()
        {
            EnableTextBox = enableTextBox;
            Maximum = maximum;
        }

        public int Minimum { get { return progressBar.Minimum; } set { progressBar.Minimum = value; } }
        public int Maximum { get { return progressBar.Maximum; } set { progressBar.Maximum = value; } }
        public int Value { get { return progressBar.Value; } set { progressBar.Value = value; } }
        public bool Cancelled { get; set; }
        public object Result { get; set; }

        public bool EnableTextBox
        {
            get
            {
                return infoBox.Visible;
            }
            set
            {
                infoBox.Visible = value;
                if (value)
                {
                    this.Height = 150;
                }
                else
                {
                    this.Height = 50;
                }
            }
        }

        public void Increment(int value)
        {
                progressBar.Increment(value);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }


        private WorkerFunction mFunction = null;
        public void StartWorker(WorkerFunction function) { StartWorker(function, null); }
        public void StartWorker(WorkerFunction function, object parameter)
        {
            if (function == null) return;
            mFunction = function;
            if(!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync(parameter);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            e.Result = mFunction(e.Argument, worker, e);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is string && EnableTextBox)
            {
                infoBox.Text = e.UserState as string;
            }
            if (e.ProgressPercentage <= this.Maximum)
                this.progressBar.Value = e.ProgressPercentage;
            else
                this.progressBar.Value = this.Maximum;
            ProgressChanged(sender, e);
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            if (e.Cancelled)
            {
                this.Cancelled = true;
            }
            else
            {
                this.Result = e.Result;
                this.progressBar.Value = this.Maximum;
            }
            RunWorkerCompleted(sender, e);
            this.Dispose();
        }
    }


    /*
    /// <summary>
    /// A Progress Bar that operates independently in its own thread.
    /// </summary>
    public class ProgressBarThread
    {
        public string Title { get; set;}
        public long ExpectedTimeToComplete { get; set;}
        //private bool mEndThread = false;
        //public bool EndThread { get { return mEndThread; } set { mEndThread = value; } }
        public Thread ActualThread { get; set; }
        private ProgressBarForm progForm;

        public ProgressBarThread(string title, long expectedTimeToComplete)
        {
            Title = title;
            ExpectedTimeToComplete = expectedTimeToComplete;
            ActualThread = new Thread(this.ThreadMain);
            ActualThread.Start();
        }

        public void ProgressEventHandler(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("Thread Event! '"+e.Code+","+this.ExpectedTimeToComplete+","+progForm.InvokeRequired);
            if (progForm.InvokeRequired)
            {
                Console.WriteLine("1");
                AdriansLib.ProgressEventHandler d = new AdriansLib.ProgressEventHandler(this.ProgressEventHandler);
                Console.WriteLine("2");
               
                Console.WriteLine("3");
            }
            else progForm.ProgressEventHandler(sender, e);
        }


        public void ThreadMain()
        {
            long time = 0;
            int interval = 1;
            progForm = new ProgressBarForm(Title);
            Thread.Sleep(0);

            progForm.Maximum = 1000;
            if (ExpectedTimeToComplete > 0)
            {
                if (ExpectedTimeToComplete <= 1000)
                {
                    progForm.Maximum = (int)ExpectedTimeToComplete;
                }
                else
                {
                    interval = (int)ExpectedTimeToComplete / 1000;
                }
            }
            // else : Event driven. intervals are meaningless.
            progForm.Show();

            if (ExpectedTimeToComplete > 0)
            {
                while (time < ExpectedTimeToComplete && !progForm.Complete)
                {
                    progForm.Increment(1);
                    progForm.Activate();
                    Thread.Sleep(interval);
                    time += interval;
                }
            }
            else
            {
                while (!progForm.Complete) { 
                    Thread.Sleep(1000);
                    Console.WriteLine("sleeping...");
                } // Relying on a ProgressEvent to set EndThread = true.
            }

            progForm.Close();
            this.Close();
        }

        public void Close()
        {
            if(progForm != null)
                progForm.Complete = true;
        }
        
    }
     */

    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

    /// <summary>
    /// ProgressEventArgs.
    /// There are three layers of Message - think of it as a document outline, where L1 is like a header moving down to L3 is the body.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        public string MessageL1 { get; set; }
        public string MessageL2 { get; set; }
        public string MessageL3 { get; set; }
        public enum ProgressCode { InProgress,Done, Error, Cancelled };
        public int ProgressIncrement { get; set; }
        public int ProgressTotal { get; set; }
        public ProgressCode Code { get; set; }

        public ProgressEventArgs() : this("", "", "", ProgressCode.InProgress) { }
        public ProgressEventArgs(ProgressCode code) : this("", "", "", code) { }
        public ProgressEventArgs(string m1) : this(m1, "", "", ProgressCode.InProgress) { }
        public ProgressEventArgs(string m1, ProgressCode code) : this(m1, "", "", code) { }
        public ProgressEventArgs(string m1, string m2, string m3, ProgressCode code)
        {
            ProgressIncrement = 1;
            ProgressTotal = 100;
            MessageL1 = m1;
            MessageL2 = m2;
            MessageL3 = m3;
            Code = code;
        }

        public string PrintOutlinedMessage()
        {
            string s = MessageL3;
            if (MessageL2.Length > 0) s = MessageL2 + Environment.NewLine + Environment.NewLine+s;
            if (MessageL1.Length > 0) s = MessageL1 + Environment.NewLine + Environment.NewLine+s;
            return s;
        }

    }
     

}
