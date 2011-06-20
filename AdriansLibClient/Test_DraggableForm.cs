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

        public int RunTests()
        {
            DTALib.DraggableForm.ShowForm(textBox);
            return 0;
        }

        public Test_DraggableForm()
        {
            InitializeComponent();
        }
    }
}
