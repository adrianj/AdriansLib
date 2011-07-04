using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DTALibClient
{
    public partial class Test_DraggableForm : UserControl, DTALib.ITestClass
    {

        public string RunTests(ref int testCount, ref int failCount)
        {
            DTALib.DraggableForm.ShowForm(textBox);
            testCount++;
            return "";
        }

        public Test_DraggableForm()
        {
            InitializeComponent();
        }
    }
}
