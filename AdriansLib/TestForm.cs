using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace AdriansLib
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private Type[] mTestableTypes = null;
        public Type[] TestableTypes
        {
            get { return mTestableTypes; }
            set { mTestableTypes = value; testCombo.DataSource = mTestableTypes; }
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            Type [] ts = Assembly.GetEntryAssembly().GetTypes();
            List<Type> test = new List<Type>();
            foreach (Type t in ts)
            {
                Console.WriteLine("" + t+","+t.IsSubclassOf(typeof(ITestClass)));
                if (t.GetInterfaces().Contains(typeof(ITestClass)))
                    test.Add(t);
            }
            TestableTypes = test.ToArray();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            if(testCombo.SelectedValue == null) return;
            Type t = testCombo.SelectedValue as Type;
            ITestClass itc = Activator.CreateInstance(t) as ITestClass;
            itc.RunTests();
        }
    }
}
