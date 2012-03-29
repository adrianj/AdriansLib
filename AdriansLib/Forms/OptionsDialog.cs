using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace DTALib.Forms
{
	public partial class OptionsDialog : Form
	{
		List<ICloneable> modifiedObjects;

		private OptionsDialog()
		{
			InitializeComponent();
		}

		public void PopulateOptions(List<ICloneable> cloneableOptionObjects)
		{
			modifiedObjects = new List<ICloneable>();
			foreach (ICloneable c in cloneableOptionObjects)
				modifiedObjects.Add(c.Clone() as ICloneable);
			foreach (ICloneable c in modifiedObjects)
			{
				TreeNode node = CreateNode(c);
				treeView1.Nodes.Add(node);
			}
			if (modifiedObjects.Count > 0)
			{
				treeView1.SelectedNode = treeView1.Nodes[0];
				treeView1.ExpandAll();
			}
		}

		private static TreeNode CreateNode(ICloneable c)
		{
			TreeNode node = new TreeNode("" + c);
			if (c == null) return node;
			node.Tag = c;
			foreach (PropertyInfo pi in c.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				MethodInfo getter = pi.GetGetMethod();
				if (getter == null) continue;
				if (pi.PropertyType.IsSubclassOf(typeof(OptionSet)))
				{
					OptionSet ic = getter.Invoke(c, new object[0]) as OptionSet;
					TreeNode subNode = CreateNode(ic);
					if (subNode.Tag != null)
					{
						Console.WriteLine("Tag: (" + subNode.Tag + ")");
						node.Nodes.Add(CreateNode(ic));
					}
				}
			}
			return node;
		}

		public List<ICloneable> GetModifiedOptions()
		{
			return modifiedObjects;
		}

		public static DialogResult Show(List<OptionSet> optionObjects)
		{
			List<ICloneable> list = optionObjects.Cast<ICloneable>().ToList(); 
			OptionsDialog od = new OptionsDialog();
			od.PopulateOptions(list);
			DialogResult r = od.ShowDialog();
			if (r == DialogResult.OK)
			{
				optionObjects.Clear();
				foreach (ICloneable c in od.GetModifiedOptions())
					optionObjects.Add(c as OptionSet);
			}
			return r;
		}

		public static DialogResult Show(List<ICloneable> cloneableOptionObjects)
		{
			OptionsDialog od = new OptionsDialog();
			od.PopulateOptions(cloneableOptionObjects);
			DialogResult r = od.ShowDialog();
			if (r == DialogResult.OK)
			{
				cloneableOptionObjects.Clear();
				foreach (ICloneable c in od.GetModifiedOptions())
					cloneableOptionObjects.Add(c);
			}
			return r;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			object o = e.Node.Tag;
			propertyGrid1.SelectedObject = o;
			groupBox1.Text = "" + o;
		}
	}

	[Browsable(false)]
	public class OptionSet : ReflectionClonedObject
	{

		public class ModifiedDataPair
		{
			public PropertyInfo Info { get; set; }
			public object OldValue { get; set; }
			public object NewValue { get; set; }

			public override string ToString()
			{
				return "" + Info + " changed from " + OldValue + " to " + NewValue;
			}
		}

		[Browsable(false)]
		public virtual string OptionsLabel { get; set; }

		public OptionSet()
		{
			OptionsLabel = this.GetType().Name;
		}

		public override string ToString()
		{
			return OptionsLabel;
		}


		public IEnumerable<ModifiedDataPair> GetModifiedValues(OptionSet oldValues)
		{
			if (oldValues != null && oldValues.GetType() != this.GetType())
				yield break;
			foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				MethodInfo getter = pi.GetGetMethod();
				if (getter == null) continue;
				object oldValue = null;
				if (oldValues != null) oldValue = getter.Invoke(oldValues, new object[0]);
				object newValue = getter.Invoke(this, new object[0]);

				if (oldValue == null && newValue == null)
					continue;

				if (pi.PropertyType.IsSubclassOf(typeof(OptionSet)))
				{
					foreach (ModifiedDataPair m in (newValue as OptionSet).GetModifiedValues(oldValue as OptionSet))
						yield return m;
				}
				else
				{
					if ((oldValue == null && newValue != null) || !oldValue.Equals(newValue))
						yield return new ModifiedDataPair() { Info = pi, NewValue = newValue, OldValue = oldValue };
				}
			}
		}
	}
}
