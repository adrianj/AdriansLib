using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace AdriansLib
{
    public partial class About : Form
    {
        public static void ShowAboutDialog()
        {
            Application.EnableVisualStyles();
            About ab = new About();
            ab.ShowDialog();
        }

        public static bool CheckDependencies()
        {
            try
            {
                List<string> missingPaths = new List<string>();
                BuildDependentAssemblyList(Assembly.GetEntryAssembly().FullName, null, null,missingPaths);
                if (missingPaths.Count > 0)
                {
                    string s = "";
                    foreach (string path in missingPaths) s += Environment.NewLine + path;
                    MessageBox.Show("Dependency Check Failed to find:\n" + s);
                    return false;
                }
            }
            catch (Exception e) {
                MessageBox.Show("Unexpected exception with dependency check.\n\n"+e); return false; 
            }
            return true;
        }

        public About()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription;
            populateDependencies();
        }

        private Assembly getAss()
        {
            return Assembly.GetEntryAssembly();
        }

        public static Assembly[] BuildDependentAssemblyList(string path,
                                                List<string> assemblies, List<Assembly> asses, List<string> missingPaths)
        {
            // Maintain a list of assemblies the original one needs.
            if (assemblies == null)
                assemblies = new List<string>();
            if (asses == null)
                asses = new List<Assembly>();
            if (missingPaths == null)
                missingPaths = new List<string>();

            // Have we already seen this one?
            if (assemblies.Contains(path) == true)
                return (new Assembly[0]);
            Assembly asm = null;
            // Look for common path delimiters in the string
            // to see if it is a name or a path.
            if ((path.IndexOf(Path.DirectorySeparatorChar, 0, path.Length) != -1) ||
                (path.IndexOf(Path.AltDirectorySeparatorChar, 0, path.Length) != -1))
            {
                // Load the assembly from a path.
                asm = Assembly.ReflectionOnlyLoadFrom(path);
            }
            else
            {
                // Try as assembly name.
                try
                {
                    asm = Assembly.ReflectionOnlyLoad(path);
                }
                catch (FileNotFoundException)
                {
                    missingPaths.Add(path);
                    return null;
                }
            }

            // Add the assembly to the list.
            if (asm != null)
            {
                assemblies.Add(path);
                asses.Add(asm);
            }
            else
            {
                missingPaths.Add(path);
                return null;
            }
            // Get the referenced assemblies.
            AssemblyName[] imports = asm.GetReferencedAssemblies();

            // Iterate.
            foreach (AssemblyName asmName in imports)
            {
                // Now recursively call this assembly to get the new modules
                // it references.
                BuildDependentAssemblyList(asmName.FullName, assemblies,asses,missingPaths);
            }

            Assembly[] temp = new Assembly[asses.Count];
            asses.CopyTo(temp, 0);
            return (temp);
        }

        private void populateDependencies()
        {
            string [] SystemPrefixes = new string[]{"C:\\Windows"};

            Assembly[] deps = BuildDependentAssemblyList(getAss().FullName, null,null,null);
            foreach (Assembly a in deps)
            {
                //Console.WriteLine("" + a);
                string filename = Path.GetFileName(a.Location);
                string path = Path.GetDirectoryName(a.Location);
                AssemblyName an = new AssemblyName(a.FullName);
                if (an.FullName.Equals(getAss().FullName)) continue;
                ListViewItem lvi = new ListViewItem(new string[] { filename, ""+an.Version,path });
                dependencyList.Items.Add(lvi);
                bool system = false;
                foreach (string s in SystemPrefixes)
                    if (path.Length >= s.Length && path.Substring(0, s.Length).Equals(s, StringComparison.InvariantCultureIgnoreCase))
                        system = true;
                if(!system)
                    dependencyList.Groups[0].Items.Add(lvi);
                else
                    dependencyList.Groups[1].Items.Add(lvi);
            }
        }

        #region Assembly Attribute Accessors


        public string AssemblyTitle
        {
            get
            {
                object[] attributes = getAss().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(getAss().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return getAss().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = getAss().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = getAss().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = getAss().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = getAss().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
