using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace DTALib
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
            set {
                mTestableTypes = value; 
                testCombo.Items.Clear();
                testCombo.Items.AddRange(mTestableTypes);
            }
        }

        public string SelectedTest
        {
            get
            {
                if (testCombo == null || testCombo.SelectedItem == null) 
                    return "";
                return ""+testCombo.SelectedItem;
            }
            set {
                foreach (object t in testCombo.Items)
                {
                    if (t.ToString().Equals(value))
                        testCombo.SelectedItem = t;
                }
            }
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            Type [] ts = Assembly.GetEntryAssembly().GetTypes();
            List<Type> test = new List<Type>();
            foreach (Type t in ts)
            {
                if (t.GetInterfaces().Contains(typeof(ITestClass)))
                    test.Add(t);
            }
            TestableTypes = test.ToArray();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            if(this.testCombo.SelectedItem == null) return;
            Type t = testCombo.SelectedItem as Type;
            ITestClass itc = Activator.CreateInstance(t) as ITestClass;
            int failCount = 0;
            int testCount = 0; 
            failMsgBox.Text = itc.RunTests(ref failCount, ref testCount);
            failCountBox.Text = "Failed " + failCount + "/" + testCount;
        }

    }
}
