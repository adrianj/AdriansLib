using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DTALib;
using System.Reflection;

namespace AdriansLibClient
{
    public partial class Test_PropertyParser : Form, ITestClass
    {
        public Test_PropertyParser()
        {
            ComplexProp = new ComplexClass1();
            InitializeComponent();
            List<PropertyInfo> props = PropertyParser.getParseableProperties(this.GetType());
            debugCombo.Items.AddRange(props.ToArray());
            debugCombo.SelectedIndex = 0;
        }
        public string RunTests(ref int failCount, ref int testCount)
        {
            this.ShowDialog();
            testCount++;
            return "PropertyParser Test Complete";
        }


        private List<bool> mBools = new List<bool>();
        [Parseable]
        public List<bool> BoolList { get { return mBools; } set { mBools = value; } }

        private Color mColor = Color.Aquamarine;
        [Parseable]
        public Color TestColor { get { return mColor; } set { mColor = value; } }
        private List<ComplexClass2> mComps = new List<ComplexClass2>();
        [Parseable]
        public List<ComplexClass2> CompList { get { return mComps; } set { mComps = value; } }

        [Parseable]
        public ComplexClass1 ComplexProp { get; set; }
        [Parseable]
        public string StringProp { get { return textBox.Text; } set { textBox.Text = value; } }

        private int[] mArray = new int[10];
        [Parseable]
        public int[] IntArrayProp { get { return mArray; } set { mArray = value; } }


        [Parseable]
        public bool CheckBox { get { return checkBox.Checked; } set { checkBox.Checked = value; } }

        [Parseable]
        public string[] Lines { get { return textBoxMulti.Lines; } set { textBoxMulti.Lines = value; } }

        private void Test_PropertyParser_Load(object sender, EventArgs e)
        {
            fileBrowserBox.Filename = AdriansLibClient.Properties.Settings.Default.PropertyParserFile;
            LoadSettings();
        }

        private void LoadSettings()
        {
            try { PropertyParser.ReadObjectFromXml(fileBrowserBox.Filename, this); }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to parse property file\n"+ex+"\nInner: \n"+ex.InnerException);
            }
            SetDebugDisplays();
        }

        private void SaveSettings()
        {
            PropertyParser.WriteObjectToXml(fileBrowserBox.Filename, this);
        }

        private void ClearFields()
        {
            IntArrayProp = new int[2];
            ComplexProp = new ComplexClass1();
            CheckBox = false;
            Lines = new string[0];
            StringProp = "";
            CompList.Clear(); CompList.Add(new ComplexClass2(4, 5)); CompList.Add(null); CompList.Add(new ComplexClass2(6, 7));
            BoolList.Clear(); BoolList.Add(true); BoolList.Add(false); BoolList.Add(true);
            SetDebugDisplays();
        }

        private void SetDebugDisplays()
        {
            if (debugCombo.SelectedItem != null)
            {
                PropertyInfo pInfo = (PropertyInfo)debugCombo.SelectedItem;
                object o = pInfo.GetGetMethod().Invoke(this, null);
                propertyGrid.SelectedObject = IntArrayProp;
                debuggingTree1.RootObject = o;
            }
        }

        private void Test_PropertyParser_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (fileBrowserBox.Filename != null && fileBrowserBox.Filename.Length > 0)
            {
                AdriansLibClient.Properties.Settings.Default.PropertyParserFile = fileBrowserBox.Filename;
                AdriansLibClient.Properties.Settings.Default.Save();
            }
        }

        private void buttonPressed(object sender, EventArgs e)
        {
            if (sender == saveButton)
                SaveSettings();
            else if (sender == loadButton)
                LoadSettings();
            else if (sender == clearButton)
                ClearFields();
        }

        private void debugCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            SetDebugDisplays();
        }
        
    }

    public class ComplexClass1
    {
        [Parseable]
        public ComplexClass2[] ComplexArray { get { return mArray; } set { mArray = value; } }
        private string mName = "FunkyClass";
        [Parseable]
        public string Name { get { return mName; } set { mName = value; } }
        [Parseable]
        public string NullProp { get { return null; } set {}}

        private ComplexClass2[] mArray = new ComplexClass2[] { new ComplexClass2(), new ComplexClass2() };
    }

    public class ComplexClass2
    {
        [Parseable]
        public double DoubleVal { get; set; }
        [Parseable]
        public long LongVal { get; set; }

        public ComplexClass2() { }

        public ComplexClass2(double d, long l)
        {
            LongVal = l;
            DoubleVal = d;
        }

    }
}
