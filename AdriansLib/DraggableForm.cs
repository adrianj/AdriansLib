using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdriansLib
{
    public partial class DraggableForm : Form
    {
        public static DraggableForm ShowForm(Control controlToInclude)
        {
            DraggableForm df = new DraggableForm();
            df.Control = controlToInclude;
            df.Show();
            return df;
        }

        public DraggableForm()
        {
            InitializeComponent();
        }
        public Control Control
        {
            get { if (Controls.Count >= 1) return Controls[0]; return null; }
            set
            {
                Controls.Clear();
                if (value != null)
                {
                    this.ClientSize = value.Size;
                    value.Dock = DockStyle.Fill;
                    Controls.Add(value);
                }
            }
        }

        // The code below is for implementing a form that can be DragAndDropped by right-click dragging the title bar

        protected void OnTitlebarClick()
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0xA4)
            {
                OnTitlebarClick();
                return;
            }
            base.WndProc(ref m);
        }
    }
}
