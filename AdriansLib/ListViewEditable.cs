using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DTALib
{

    public partial class ListViewEditable : ListView
    {
        private Control[][] mControlList;
        /// <summary>
        /// Array of controls that are used for editing.
        /// If list is null, empty, or a column is edited that exceeds the length of the list
        /// then a default TextBox is used.
        /// All Controls added are set invisible, so don't use controls normally used in other forms.
        /// </summary>
        public Control[] ControlList
        {
            get { if (mControlList == null) return null; else return mControlList[0]; }
            set { if (value == null) ControlListIndependent = null; else ControlListIndependent = new Control[][] { value }; }
        }

        /// <summary>
        /// Use this control list if independent rows are to have independent editing controls.
        /// </summary>
        public Control[][] ControlListIndependent
        {
            get { return mControlList; }
            set
            {
                // First remove existing controls
                if (mControlList != null)
                foreach(Control [] mc in mControlList)
                    foreach (Control c in mc)
                        Controls.Remove(c);
                mControlList = value;
                if (mControlList == null || mControlList.Length == 0) mControlList = new Control[][]{new Control[] { new TextBox() }};
                foreach (Control [] mc in mControlList)
                    foreach(Control c in mc)
                    {
                        if (c != null)
                        {
                            c.Visible = false;
                            c.Validating += new CancelEventHandler(control_Validating);
                            c.LostFocus += new EventHandler(textEditor_LostFocus);
                            Controls.Add(c);
                        }
                    }
            }
        }

        private Control mEditor;
        private ListViewItem.ListViewSubItem editingSubItem;
        private ListViewItem.ListViewSubItem previousItem;
        private ListViewItem editingItem;
        public event ListViewSubItemChangeHandler ListViewSubItemChanging;
        private int mSubItemColumn;

        // Mask out AllowColumnReorder, as it should never be allowed.
        public new bool AllowColumnReorder { get { return base.AllowColumnReorder; } }

        public ListViewEditable()
        {
            this.View = System.Windows.Forms.View.Details;
            base.AllowColumnReorder = false;
            InitializeComponent();
            // null ControlList - this automatically populates a default textbox control.
            this.ControlList = null;
            MouseUp += new MouseEventHandler(ListViewEditable_MouseUp);
        }

        void textEditor_LostFocus(object sender, EventArgs e)
        {
            mEditor.Visible = false;
        }

        void control_Validating(object sender, CancelEventArgs e)
        {
            ListViewItem.ListViewSubItem prev = new ListViewItem.ListViewSubItem(this.SelectedItems[0], editingSubItem.Text, editingSubItem.ForeColor,
                    editingSubItem.BackColor, editingSubItem.Font);
            editingSubItem.Text = mEditor.Text;
            if (FireListViewSubItemChangeEvent(new ListViewSubItemChangeEventArgs(editingItem,editingSubItem,mSubItemColumn)))
            {
                editingSubItem = prev;
                editingItem.SubItems[mSubItemColumn] = editingSubItem;
            }
            mEditor.Visible = false;
        }

        /// <summary>
        /// Listeners have ability to cancel. Therefore, iterate through listeners
        /// and execute, with option to break early if listeners choose to cancel.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool FireListViewSubItemChangeEvent(ListViewSubItemChangeEventArgs e)
        {
            bool cancel = false;
            if (ListViewSubItemChanging != null)
            {
                foreach (ListViewSubItemChangeHandler de in ListViewSubItemChanging.GetInvocationList())
                {
                    de(this, e);
                    cancel = e.Cancel;
                    if (cancel) break;
                }
            }
            Console.WriteLine("Cancel " + (cancel));
            return cancel;
        }

        void ListViewEditable_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ListViewItem lvi = this.GetItemAt(1, e.Y);
                if (lvi == null) return;
                // Find which column.
                mSubItemColumn = 0;
                int sum = 0;
                if (this.Columns.Count > 1)
                {
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        sum += Columns[i].Width;
                        if (e.X < sum) { mSubItemColumn = i; break; }
                    }
                }
                editingSubItem = lvi.SubItems[mSubItemColumn];
                editingItem = lvi;
                bool change = (editingSubItem == previousItem);
                previousItem = editingSubItem;
                lvi.Selected = true;
                if (change)
                {

                    mEditor = GetControlAtIndex(mSubItemColumn,editingItem.Index);
                    if (mEditor != null)
                    {
                        Rectangle bounds = editingSubItem.Bounds;
                        //if (this.CheckBoxes && mSubItemColumn == 0)
                        //    bounds.Offset(new Point(22, 0));
                        mEditor.Bounds = bounds;
                        mEditor.Text = editingSubItem.Text;

                        mEditor.Visible = true;
                        mEditor.BringToFront();
                        //mEditor.Focus();
                        mEditor.Select();
                    }
                }
            }
        }

        public Control GetControlAtIndex(int column, int row)
        {
            if (row < 0 || row >= mControlList.Length) return GetControlAtIndex(column, 0);
            if (column < 0 || column >= mControlList[0].Length) return GetControlAtIndex(0,row);
            return mControlList[row][column];
        }
    }

    public delegate void ListViewSubItemChangeHandler(object sender, ListViewSubItemChangeEventArgs e);

    public class ListViewSubItemChangeEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public ListViewItem Item { get; set; }
        public ListViewItem.ListViewSubItem SubItem { get; set; }
        public int SubItemColumn { get; set; }
        public ListViewSubItemChangeEventArgs(ListViewItem item, ListViewItem.ListViewSubItem subItem, int column)
        {
            Cancel = false;
            SubItem = subItem;
            Item = item;
            SubItemColumn = column;
        }
    }
}
