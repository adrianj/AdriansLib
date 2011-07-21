using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DTALib;

namespace AdriansLibClient
{
    public partial class Test_TabHelper : Form, ITestClass
    {
        private TabControlHelper tabHelper;

        public string RunTests(ref int failCount, ref int testCount)
        {
            this.ShowDialog();
            return "";
        }

        public Test_TabHelper()
        {
            InitializeComponent();
            tabHelper = new TabControlHelper(tabControl1);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Clicked on Button: " + sender);
        }
    }
}
