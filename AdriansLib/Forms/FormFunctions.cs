using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace DTALib.Forms
{
	public class FormFunctions
	{
		public static void SetPropertyGridLabelWidth(PropertyGrid grid, int width)
		{
			Control gridView = GetControlOfNamedType(grid, "PropertyGridView");
			Control docComment = GetControlOfNamedType(grid, "DocComment");
			FieldInfo widthField = gridView.GetType().GetField("labelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
			widthField.SetValue(gridView, width);
			grid.Refresh();
			width = (int)widthField.GetValue(gridView);
			FieldInfo heightField = docComment.GetType().GetField("height", BindingFlags.Instance | BindingFlags.NonPublic);
			if (heightField != null)
			{
				int height = (int)heightField.GetValue(docComment);
				Console.WriteLine("" + heightField);
			}
		}

		public static Control GetControlOfNamedType(Control parentControl, string name)
		{
			foreach (Control control in parentControl.Controls)
			{
				if (control.GetType().Name.Equals(name))
					return control;
			}
			return null;
		}
	}
}
