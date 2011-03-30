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
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
        public DialogResult DialogResult {get;set;}
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
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            if (ButtonClick != null)
                ButtonClick(sender, e);
        }

        // pretty much, if busy then button is cancel, otherwise it's ok, which can be used however.
        public delegate void setBusyCallback(bool busy);
        private void setBusy(bool busy)
        {
            if(cancelButton.InvokeRequired)
            {
                setBusyCallback d = new setBusyCallback(setBusy);
                this.Invoke(d, new object[] { busy });
            }
            else
            {
                if (!busy)
                {
                    progressBar.Cursor = Cursors.Default;
                    cancelButton.Text = "Ok";
                }
                else
                {
                    progressBar.Cursor = Cursors.WaitCursor;
                    cancelButton.Text = "Cancel";
                }
            }
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
            setBusy(true);
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
            if(ProgressChanged != null)
                ProgressChanged(sender, e);
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setBusy(false);
            if (e.Cancelled)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Result = null;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Result = e.Result;
                //this.progressBar.Value = this.Maximum;
            }
            if(RunWorkerCompleted != null)
                RunWorkerCompleted(sender, e);
        }

        public event EventHandler ButtonClick;
    }

    /*
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
    
     */

}
