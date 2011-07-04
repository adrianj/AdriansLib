using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace DTALib
{
    /// <summary>
    /// A control for showing an object and all it's public properties.
    /// Child properties can then be expanded to view their properties, and so on.
    /// Also includes a PropertyGrid where certain properties can be edited during runtime.
    /// </summary>
    public partial class DebuggingTree : UserControl
    {
        private int mMaxDepth = 4;
        public int MaximumDepth { get { return mMaxDepth; } set { mMaxDepth = value; } }

        private int mMaxProperties = 20;
        public int MaximumProperties { get { return mMaxProperties; } set { mMaxProperties = value; } }

        private int [] mPadding = new int[] { 30, 30, 30 };
        public int [] CharacterPadding
        {
            get { return mPadding; }
            set
            {
                if (value == null) return;
                for (int i = 0; i < Math.Min(value.Length, mPadding.Length); i++)
                    mPadding[i] = value[i];
                Refresh();
            }
        }

        private int mTimeout = 2000;
        /// <summary>
        /// Time, in milliseconds, before giving up with the RootObject assignment operation.
        /// This is essential for some objects that have recursive properties, eg, contain a Property within 
        /// them that has the same class as the parent.
        /// </summary>
        public int RootAssignmentTimeout { get { return mTimeout; } set { mTimeout = value; } }
        private System.Timers.Timer mTimer = new System.Timers.Timer();
        private bool mCancelUpdate = false;

        private int mCurrentDepth = 0;

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
                    mTimer.Stop();
                    mTimer.Interval = RootAssignmentTimeout;
                    mCancelUpdate = false;
                    mCurrentDepth = 0;
                    mDebugObject = value;
                    if (debugTreeView.Nodes.Count > 0)
                        debugTreeView.Nodes.RemoveAt(0);
                    //debugTreeView.Nodes.Clear();
                    mTimer.Start();
                    debugTreeView.Nodes.Add(AddPropertyNodes(value, "" + value));
                    mTimer.Stop();
                    debugTreeView.Nodes[0].Expand();
                    propertyGrid.SelectedObject = value;
                    //debugTreeView.Nodes[0].se
                }
            }
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            mCancelUpdate = true;
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
            RootObject = this;
            mTimer.Elapsed += mTimer_Tick;
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
            TreeNode tree = null;
            mCurrentDepth++;
            if (mCancelUpdate)
            {
                tree = new TreeNode("... Timed out. Truncated");
            }
            else if (value != null && mCurrentDepth <= MaximumDepth)
            {
                string sVal = "" + value;
                if (sVal.Length > mPadding[0]) sVal = sVal.Substring(0, mPadding[0]);
                string typeName = value.GetType().Name;
                string[] s = typeName.Split(new char[] { '.' });
                typeName = s[s.Length - 1];
                string text = String.Format("{0} {1} {2}", name.PadRight(mPadding[0]), sVal.PadRight(mPadding[1]),
                    typeName.PadRight(mPadding[2]));
                tree = new TreeNode(text);
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
                        if (o != null)
                            tree.Nodes.Add(AddPropertyNodes(o, "[" + i + "]"));
                        else
                        {
                            TreeNode tn = new TreeNode("[" + i + "]");
                            tree.Nodes.Add(tn);
                        }
                        i++;
                    }
                }
                else if (value is Array)
                {
                    Array a = value as Array;
                    for (int i = 0; i < a.Length; i++)
                        tree.Nodes.Add(AddPropertyNodes(a.GetValue(i), "[" + i + "]"));
                }
                else
                {
                    PropertyInfo[] props = value.GetType().GetProperties();
                    foreach (PropertyInfo p in props)
                    {
                        int i = 0;
                        try
                        {
                            MethodInfo getter = p.GetGetMethod();
                            //object o = p.GetValue(value, null);
                            object o = getter.Invoke(value, null);
                            tree.Nodes.Add(AddPropertyNodes(o, p.Name));
                            i++;
                            if (i >= MaximumProperties)
                                break;
                        }
                        catch (ArgumentException) { break; }
                        catch (TargetParameterCountException) { break; }
                        catch (Exception) { throw; }
                    }
                }
            }
            mCurrentDepth--;
            return tree;
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
