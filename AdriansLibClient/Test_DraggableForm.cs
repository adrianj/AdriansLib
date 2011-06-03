using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdriansLibClient
{
    public partial class Test_DraggableForm : UserControl, AdriansLib.ITestClass
    {

        public int RunTests()
        {
            AdriansLib.DraggableForm.ShowForm(textBox);
            return 0;
        }

        public Test_DraggableForm()
        {
            InitializeComponent();
        }
    }
}
