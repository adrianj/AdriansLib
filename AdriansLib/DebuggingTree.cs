using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace AdriansLib
{
    /// <summary>
    /// A control for showing an object and all it's public properties.
    /// Child properties can then be expanded to view their properties, and so on.
    /// Also includes a PropertyGrid where certain properties can be edited during runtime.
    /// </summary>
    public partial class DebuggingTree : UserControl
    {
        private bool mShowPropertyGrid = true;
        private object mDebugObject = null;
        /// <summary>
        /// The object that is the root node of the tree.
        /// </summary>
        public object RootObject
        {
            get
            {
                return mDebugObject;
            }
            set
            {
                if (value != null)
                {
                    mDebugObject = value;
                    if (debugTreeView.Nodes.Count > 0)
                        debugTreeView.Nodes.RemoveAt(0);
                    //debugTreeView.Nodes.Clear();
                    debugTreeView.Nodes.Add(AddPropertyNodes(value, "" + value));
                    debugTreeView.Nodes[0].Expand();
                }
            }
        }
        /// <summary>
        /// Selects whether to display and enable the PropertyGrid pane
        /// </summary>
        public bool ShowPropertyGrid
        {
            get { return mShowPropertyGrid; }
            set
            {
                mShowPropertyGrid = value;
                SetPropertyGridVisibility();
            }
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DebuggingTree()
        {
            InitializeComponent();
        }

        private void SetPropertyGridVisibility()
        {
            propertyGrid.Enabled = mShowPropertyGrid;
            propertyGrid.Visible = mShowPropertyGrid;
            if (!mShowPropertyGrid)
            {
                splitContainer.SplitterDistance = splitContainer.Width;
            }
            else
                splitContainer.SplitterDistance = splitContainer.Width / 2;
        }


        private TreeNode AddPropertyNodes(object value, string name)
        {
            if (value != null)
            {
                string sVal = "" + value;
                if (sVal.Length > 30) sVal = sVal.Substring(0, 30);
                string text = String.Format("{0} {1} {2}", name.PadRight(30), sVal.PadRight(40), value.GetType().ToString().PadRight(50));
                TreeNode tree = new TreeNode(text);
                tree.Tag = value;
                if (value is IDictionary)
                {
                    IDictionary dic = (IDictionary)value;
                    foreach (object o in dic.Keys)
                    {
                        tree.Nodes.Add(AddPropertyNodes(dic[o], "[" + o + "]"));
                    }
                }
                else if (value is ICollection)
                {
                    ICollection coll = (ICollection)value;
                    int i = 0;
                    foreach (object o in coll)
                    {
                        tree.Nodes.Add(AddPropertyNodes(o, "[" + i + "]"));
                        i++;
                    }
                }
                else if (!(value is String))
                {
                    PropertyInfo[] props = value.GetType().GetProperties();
                    foreach (PropertyInfo p in props)
                    {

                        try
                        {
                            object o = p.GetValue(value, null);
                            tree.Nodes.Add(AddPropertyNodes(o, p.Name));
                        }
                        catch { }

                    }
                }
                return tree;
            }
            return null;
        }


        private void debugTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            propertyGrid.SelectedObject = e.Node.Tag;
        }


    }

    /// <summary>
    /// Modified TreeView that doesn't automatically show big tooltips for long strings.
    /// </summary>
    public class TreeViewWithoutTooltips : TreeView
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.Style |= 0x80; // Turn on TVS_NOTOOLTIPS
                return parms;
            }
        }
    } 
}
