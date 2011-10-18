using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace DTALib.Forms
{
	[Designer(typeof(DragDropDesigner))]
	public class FormDialog : Component
	{
		SingleControlForm form = new SingleControlForm();

		public SingleControlForm.ButtonOptions Buttons { get { return form.Buttons; } set { form.Buttons = value; } }
		public Control Control { get { return form.Control; } set { form.Control = value; } }
		public string Text { get { return form.Text; } set { form.Text = value; } }
		public DialogResult DialogResult { get { return form.DialogResult; } }
		public FormStartPosition StartPosition { get { return form.StartPosition; } set { form.StartPosition = value; } }

		public SingleControlForm Form { get { return form; } }

		public virtual DialogResult ShowDialog() { return this.ShowDialog(null); }
		public virtual DialogResult ShowDialog(IWin32Window owner)
		{
			if (owner == null || owner.Handle == IntPtr.Zero)
				return form.ShowDialog();
			else
				return form.ShowDialog(owner);
		}

		public virtual void Show()
		{
			form.Show();
		}

		private void InitializeComponent()
		{
		}

	}

	public class WindowWrapper : IWin32Window
	{
		IntPtr handle = IntPtr.Zero;
		public IntPtr Handle { get { return handle; } }
		public bool IsNull { get { if (handle == IntPtr.Zero) return true; return false; } }
		public WindowWrapper(IntPtr handle) { this.handle = handle; }
	}

	public class DragDropDesigner : ComponentDesigner
	{
		//Adorner adorner;
		//IDesignerHost host;

		public DragDropDesigner() { 
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			/*
			adorner = new Adorner();
			BehaviorService service = (BehaviorService)GetService(typeof(BehaviorService));
			service.Adorners.Add(adorner);
			adorner.Glyphs.Add(new MyGlyph(component,service));
			service.EndDrag += new BehaviorDragDropEventHandler(service_EndDrag);
			service.BeginDrag += new BehaviorDragDropEventHandler(service_BeginDrag);

			host = (IDesignerHost)GetService(typeof(IDesignerHost));
			host.TransactionOpened += new EventHandler(host_TransactionOpened);
			 */
		}
		/*
		void host_TransactionOpened(object sender, EventArgs e)
		{
			MessageBox.Show("transaction opened: " + e+", "+host.TransactionDescription+", "+host+", "+this.ActionLists.Count);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && adorner != null)
			{
				BehaviorService service = (BehaviorService)GetService(typeof(BehaviorService));
				if (service != null)
				{
					service.Adorners.Remove(adorner);
				}
			}
		}

		void service_BeginDrag(object sender, BehaviorDragDropEventArgs e)
		{
			MessageBox.Show("Begin drag? " + e + ", " + e.DragComponents);
		}

		void service_EndDrag(object sender, BehaviorDragDropEventArgs e)
		{
			MessageBox.Show("End drag? " + e + ", " + e.DragComponents);
		}

		public override void DoDefaultAction()
		{
			MessageBox.Show("Default");
		}

		 */

	}
	/*
	public class MyGlyph : ComponentGlyph
	{
		BehaviorService service;

		public MyGlyph(IComponent relatedComponent, BehaviorService service)
			: base(relatedComponent, new DragDropBehavior())
		{
			this.service = service;
		}


		public override Cursor GetHitTest(System.Drawing.Point p)
		{
			return base.GetHitTest(p);
		}
	}

	public class DragDropBehavior : Behavior
	{
		public override bool OnMouseDown(Glyph g, MouseButtons button, System.Drawing.Point mouseLoc)
		{
			MessageBox.Show("Mouse down! " + g + ", " + button + ", " + mouseLoc);
			return base.OnMouseDown(g, button, mouseLoc);
		}
	}
	 */
}
