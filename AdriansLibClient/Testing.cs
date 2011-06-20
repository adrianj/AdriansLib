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
    class Testing
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Application.DoEvents();
            Application.EnableVisualStyles();
            Application.Run(new TestForm());
            /*
            int tests = Tester.RunTests(args);
            if (tests >= 0)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
             */
        }
    }

    public class Test_AboutBox : ITestClass
    {
        public int RunTests()
        {
            About.CheckDependencies();
            About.ShowAboutDialog();
            return 0;
        }
    }


    public class Test_ProgressBarForm : ITestClass
    {
        public int RunTests()
        {
            if (MessageBox.Show("Do you wish to test?", "test?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return -1;

            int failures = 0;
            //failures += TestBackground();

            return failures;
        }

        private int eventCount = 0;


        public int TestBackground()
        {
            int failures = 0;
            int tests = 0;
            int expectedResult = 20;
            eventCount = 0;
            DialogResult expectedDialogResult = DialogResult.OK;
            using (ProgressBarForm prog = new ProgressBarForm("Testing...", (int)expectedResult, true))
            {
                prog.RunWorkerCompleted += new RunWorkerCompletedEventHandler(prog_RunWorkerCompleted);
                MessageBox.Show("Will start progress bar with text box. Wait 2 seconds. Do not click Cancel");
                prog.ButtonWidth += 30;
                prog.StartWorker(Operation, expectedResult);
                if (expectedDialogResult != prog.DialogResult) Console.WriteLine("Failure: " + (++failures)); tests++;
                if (eventCount != 1) Console.WriteLine("Failure: " + (++failures)); tests++;
                if (prog.DialogResult == DialogResult.OK && expectedResult != (int)prog.Result) Console.WriteLine("Failure: " + (++failures)); tests++;
            }
            expectedDialogResult = DialogResult.Cancel; 
            expectedResult = 50;
            using (ProgressBarForm prog = new ProgressBarForm("Testing...", (int)expectedResult, false))
            {
                prog.RunWorkerCompleted += new RunWorkerCompletedEventHandler(prog_RunWorkerCompleted);
                MessageBox.Show("Will start progress bar without text box. Click Cancel");
                prog.StartWorker(Operation, expectedResult);
                if (expectedDialogResult != prog.DialogResult) Console.WriteLine("Failure: " + (++failures)); tests++;
                if (eventCount != 2) Console.WriteLine("Failure: " + (++failures)); tests++;
                if (prog.DialogResult == DialogResult.OK && expectedResult != (int)prog.Result) Console.WriteLine("Failure: " + (++failures)); tests++;
            }

            Console.WriteLine("TestBackground() Failed " + failures + " / " + tests);
            return failures;
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
