using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTALib;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Threading;

namespace DTALibClient
{
    public class Testing
    {
        private static TestForm form;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Application.DoEvents();
            Application.EnableVisualStyles();
            form = new TestForm();
            form.Load += new EventHandler(form_Load);
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            Application.Run(form);
        }


        static void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (form.SelectedTest != null && form.SelectedTest.Length > 0)
            {
                AdriansLibClient.Properties.Settings.Default.PreviousTest = form.SelectedTest;
                AdriansLibClient.Properties.Settings.Default.Save();
            }
        }

        static void form_Load(object sender, EventArgs e)
        {
            form.SelectedTest = AdriansLibClient.Properties.Settings.Default.PreviousTest;
        }
    }

    public class Test_AboutBox : ITestClass
    {
        public string RunTests(ref int failCount, ref int testCount)
        {
            About.CheckDependencies();
            About.ShowAboutDialog();
            testCount++;
            return "";
        }
    }


    public class Test_ProgressBarForm : ITestClass
    {
        private int eventCount = 0;

        public string RunTests(ref int failCount, ref int testCount)
        {
            int expectedResult = 20;
            eventCount = 0;
            DialogResult expectedDialogResult = DialogResult.OK;
            using (ProgressBarForm prog = new ProgressBarForm("Testing...", (int)expectedResult, true))
            {
                prog.RunWorkerCompleted += new RunWorkerCompletedEventHandler(prog_RunWorkerCompleted);
                MessageBox.Show("Will start progress bar with text box. Wait 2 seconds. Do not click Cancel");
                prog.ButtonWidth += 30;
                prog.StartWorker(Operation, expectedResult);
                if (expectedDialogResult != prog.DialogResult) Console.WriteLine("Failure: " + (++failCount)); testCount++;
                if (eventCount != 1) Console.WriteLine("Failure: " + (++failCount)); testCount++;
                if (prog.DialogResult == DialogResult.OK && expectedResult != (int)prog.Result) Console.WriteLine("Failure: " + (++failCount)); testCount++;
            }
            expectedDialogResult = DialogResult.Cancel; 
            expectedResult = 50;
            using (ProgressBarForm prog = new ProgressBarForm("Testing...", (int)expectedResult, false))
            {
                prog.RunWorkerCompleted += new RunWorkerCompletedEventHandler(prog_RunWorkerCompleted);
                MessageBox.Show("Will start progress bar without text box. Click Cancel");
                prog.StartWorker(Operation, expectedResult);
                if (expectedDialogResult != prog.DialogResult) Console.WriteLine("Failure: " + (++failCount)); testCount++;
                if (eventCount != 2) Console.WriteLine("Failure: " + (++failCount)); testCount++;
                if (prog.DialogResult == DialogResult.OK && expectedResult != (int)prog.Result) Console.WriteLine("Failure: " + (++failCount)); testCount++;
            }

            return "";
        }

        void prog_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            eventCount++;
        }

        public object Operation(object p, BackgroundWorker w, DoWorkEventArgs e)
        {
            int c = 0;
            int l = (int)p;
            while (c < l)
            {
                if (w.CancellationPending) { e.Cancel = true; return null; }
                w.ReportProgress(c, "(" + c + " / " + l+")");
                c++;
                Thread.Sleep(100);
            }
            return l;
        }
    }
}
