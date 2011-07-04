using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DTALib
{
    public class TabControlHelper
    {
        private TabControl mTabs = null;
        private List<Panel> mPanels = new List<Panel>();
        public List<Panel> AllPanels { get { return mPanels; } }

        private bool mTabLock = false;
        /// <summary>
        /// Set the TabLock to true to prevent any tab movements.
        /// </summary>
        public bool TabLock { get { return mTabLock; } set { mTabLock = value; } }

        private Timer ClickTimer = new Timer();

        public TabControlHelper(TabControl tc)
        { 
            mTabs = tc;
            foreach (TabPage tp in mTabs.TabPages) mPanels.Add(tp as Panel);
            mTabs.ControlAdded += new ControlEventHandler(mTabs_ControlAdded);
            mTabs.ControlRemoved += new ControlEventHandler(mTabs_ControlRemoved);
            //mTabs.DragDrop +=new DragEventHandler(mTabs_DragDrop);
            //mTabs.DragEnter +=new DragEventHandler(mTabs_DragEnter);
            mTabs.MouseUp +=new MouseEventHandler(mTabs_MouseUp);
            mTabs.MouseMove +=new MouseEventHandler(mTabs_MouseMove);
            mTabs.MouseDown += new MouseEventHandler(mTabs_MouseDown);
            ClickTimer.Tick += new EventHandler(ClickTimer_Tick);
        }


        void mTabs_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (!(e.Control is Panel)) return;
            mPanels.Remove(e.Control as Panel);
        }

        void mTabs_ControlAdded(object sender, ControlEventArgs e)
        {
            if (!(e.Control is Panel)) return;
            mPanels.Add(e.Control as Panel);
        }


        #region Tab dragging and dropping
        private bool mouseListening = false;
        private int mouseSelected = 0;
        private Cursor tabMoveCursor = Cursors.VSplit;

        private void mTabs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ClickTimer.Interval = 300;
                ClickTimer.Start();
            }
        }

        void ClickTimer_Tick(object sender, EventArgs e)
        {
            mouseListening = true;
            mTabs.Cursor = tabMoveCursor;
            ClickTimer.Stop();
        }

        void mTabs_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && mTabs.SelectedTab != null)
            {
                if (mouseListening)
                {
                    if (e.Y >= 0 && e.Y <= mTabs.ItemSize.Height && e.X >= 0 && e.X <= mTabs.Width)
                    {
                        // Work out tab the mouse is actually pointing at.
                        Graphics g = mTabs.CreateGraphics();
                        int start = 0;
                        mouseSelected = 0;
                        for (int i = 0; i < mTabs.TabPages.Count; i++)
                        {
                            SizeF sf = g.MeasureString(mTabs.TabPages[i].Text, mTabs.TabPages[i].Font);
                            int width = Math.Max((int)sf.Width + 4, 45);
                            start += width;
                            mouseSelected = i;
                            if (start >= e.X) break;
                        }
                        g.Dispose();
                    }

                    else
                    {
                        // If we leave the form, then pop out the tab into its own window.
                        Point screen = mTabs.PointToScreen(e.Location);
                        Point form = mTabs.TopLevelControl.PointToScreen(e.Location);
                        Point outside = new Point(screen.X - form.X + e.X, screen.Y - form.Y + e.Y);
                        if (outside.X < -5 || outside.Y < -20 || outside.X > mTabs.TopLevelControl.Width || outside.Y > mTabs.TopLevelControl.Height)
                        {
                            mouseListening = false;
                            mTabs.Cursor = Cursors.Default;
                            popOutTab();
                        }
                    }
                }
            }
        }

        private void mTabs_MouseUp(object sender, MouseEventArgs e)
        {
            ClickTimer.Stop();
            if (!mTabLock && mouseListening && mTabs.SelectedIndex != mouseSelected && mouseSelected < mTabs.TabPages.Count)
            {
                //this code needs work
                // swap the tabs around.
                TabPage targetTab = mTabs.TabPages[mouseSelected];
                TabPage tab = mTabs.TabPages[mTabs.SelectedIndex];
                int oldIndex = mTabs.SelectedIndex;
                int newIndex = mouseSelected;
                mTabs.TabPages.RemoveAt(oldIndex);
                //if (newIndex > oldIndex) newIndex--;
                mTabs.TabPages.Insert(newIndex, tab);

                // swap the panels around also.
                //oldIndex = mPanels.FindIndex(delegate(Panel p) { return p == targetTab; });
                //newIndex = mPanels.FindIndex(delegate(Panel p) { return p == tab; });
                //mTabs.TabPages[mouseSelected] = tab2;
                //mTabs.TabPages[mTabs.SelectedIndex] = tab1;
                //Panel panel1 = mPanels[oldIndex];
                //mPanels[oldIndex] = mPanels[newIndex];
                //mPanels[newIndex] = panel1;
                //mTabs.TabPages.Insert(mouseSelected, tab1);
                
                mTabs.SelectedTab = tab;
            }
            mouseListening = false;
            mTabs.Cursor = Cursors.Default;
        }

        void df_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mTabLock) { e.Cancel = true; return; }
            DTALib.DraggableForm df = sender as DTALib.DraggableForm;
            mergeDraggableForm(df);
        }

        private void popOutTab()
        {
            if (mTabLock) return;
            TabPage selectedTab = mTabs.TabPages[mTabs.SelectedIndex];
            Panel newPanel = convertTabToPanel(selectedTab);
            DTALib.DraggableForm df = DTALib.DraggableForm.ShowForm(newPanel);
            df.Text = mTabs.TabPages[mTabs.SelectedIndex].Text;
            df.FormClosing += new FormClosingEventHandler(df_FormClosing);
            mTabs.TabPages.Remove(selectedTab);
            df.Focus();
        }

        private Panel convertTabToPanel(TabPage tab)
        {
            Panel pan = new Panel();
            pan.Size = tab.Size;
            int count = tab.Controls.Count;
            for (int i = 0; i < count; i++)
                pan.Controls.Add(tab.Controls[0]);
            return pan;
        }

        private void mergeDraggableForm(DTALib.DraggableForm df)
        {
            if (mTabLock) return;
            Panel panel = df.Control as Panel;
            TabPage newTab = new TabPage();
            mTabs.TabPages.Add(newTab);
            newTab.Controls.Add(panel);
            newTab.Text = df.Text;

        }

        //private Type[] supportedDropTypes = new Type[] { typeof(DTALib.DraggableForm), typeof(TabPage) };
        //private object droppedObj = null;

        /*
        private void mTabs_DragEnter(object sender, DragEventArgs e)
        {
            foreach (Type t in supportedDropTypes)
            {
                object o = e.Data.GetData(t);
                if (o != null)
                {
                    e.Effect = DragDropEffects.Move;
                    droppedObj = o;
                    return;
                }
            }
            droppedObj = null;
        }

        private void mTabs_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect != DragDropEffects.Move || mTabLock) return;
            if (droppedObj.GetType().Equals(typeof(DTALib.DraggableForm)))
            {
                DTALib.DraggableForm df = droppedObj as DTALib.DraggableForm;
                df.Close();
            }
        }
         */
        #endregion
    }
}
