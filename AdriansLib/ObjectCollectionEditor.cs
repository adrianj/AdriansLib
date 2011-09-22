using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Reflection;

namespace DTALib
{
	public class NoEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			return value;
		}
	}


	public class ObjectCollectionEditor : CollectionEditor
	{
		public ObjectCollectionEditor(Type t) : base(t) { }
		/// <summary>
		/// Override this method to specify which class types can be instantiated within this collection editor.
		/// </summary>
		/// <returns>An array of Types that can be instantiated by the editor</returns>
		protected virtual IEnumerable<Type> GetTypes()
		{
			return new Type[] { typeof(object) };
		}


		protected override Type[] CreateNewItemTypes()
		{
			return GetTypes().ToArray();
		}

		protected override object CreateInstance(Type itemType)
		{
			object obj = Activator.CreateInstance(itemType);

			if (obj is IComponent)
			{
				IContainer cont = GetParentComponent(this.Context);
				if (cont != null)
				{
					IComponent comp = obj as IComponent;
					cont.Add(comp);
				}
			}

			return obj;
		}
		private IContainer GetParentComponent(ITypeDescriptorContext context)
		{
			try
			{
				IContainer cont = GetDesignerContainer();
				if (cont != null)
					return cont;
			}
			catch (NullReferenceException) { }
			catch (InvalidCastException) { }
			catch (Exception) { throw; }
			if (this.Context.Instance.GetType().IsSubclassOf(typeof(Component)))
			{
				Component comp = this.Context.Instance as Component;
				return comp.Container;
			}
			return null;
		}
		private IContainer GetDesignerContainer()
		{
			IDesignerHost designer = GetHost();
			if (designer == null || designer.RootComponent == null) return null;
			return designer.Container;
		}
		private IDesignerHost GetHost()
		{
			IDesignerHost designer = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			return designer;
		}

		protected override void DestroyInstance(object instance)
		{
			ICollection collection = GetCollection();
			if (instance is IDisposable)
			{
				IDisposable s = instance as IDisposable;
				s.Dispose();
			}

			base.DestroyInstance(instance);			
		}

		private ICollection GetCollection()
		{
			return Context.PropertyDescriptor.GetValue(Context.Instance) as ICollection;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			System.Collections.ICollection begin = new ArrayList(value as ICollection);
			ICollection end = base.EditValue(context, provider, value) as ICollection;

			if (!CollectionsEqual(begin, end))
				FireCollectionChangedEvent(new PropertyChangedEventArgs(context.PropertyDescriptor.Name));
				
			return end;
		}

		private bool CollectionsEqual(ICollection collA, ICollection collB)
		{
			if (collA == null || collB == null) return false;
			if (collA.Count != collB.Count) return false;
			IEnumerator eA = collA.GetEnumerator();
			IEnumerator eB = collB.GetEnumerator();
			for(int i = 0; i < collA.Count; i++)
			{
				eA.MoveNext();
				eB.MoveNext();
				if (eA.Current == null && eB.Current != null) return false;
				if (eA.Current == null && eB.Current == null) continue;
				if(!eA.Current.Equals(eB.Current)) return false;
			}
			return true;
		}


		public static IEnumerable<Type> GetImplementingTypes(Type typeToImplement)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			List<Type> ret = new List<Type>();
			ret = ret.Union(GetImplementingTypesInAssembly(typeToImplement, asm)).ToList();
			asm = Assembly.GetCallingAssembly();
			if (asm != null)
				ret = ret.Union(GetImplementingTypesInAssembly(typeToImplement, asm)).ToList();

			return ret;
		}

		private static System.Collections.Generic.List<Type> GetImplementingTypesInAssembly(Type typeToImplement, Assembly asm)
		{
			List<Type> ret = new List<Type>();
			foreach (Type t in asm.GetTypes())
			{
				if (TypeIsSubClassOf(t, typeToImplement) || TypeImplementsInterface(t, typeToImplement))
					if (HasSimpleConstructor(t))
						ret.Add(t);
			}
			return ret;
		}

		public static bool TypeImplementsInterface(Type type, Type typeToImplement)
		{
			Type[] interfaces = type.FindInterfaces(
					(typ, obj) => typ == typeToImplement,
					type);
			if (interfaces != null && interfaces.Length > 0)
				return true;
			return false;
		}

		public static bool TypeIsSubClassOf(Type type, Type typeToImplement)
		{
			if (type.IsInterface) return false;
			if (type.IsAbstract) return false;
			if (type == typeToImplement) return true;
			if (type.IsSubclassOf(typeToImplement)) return true;
			return false;
		}

		private static bool HasSimpleConstructor(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null) return false;
			return true;
		}

		#region Static Property Changed Events
		// Define a static event to expose the inner PropertyGrid's
		// PropertyValueChanged event args...
		public static event PropertyValueChangedEventHandler CollectionValueChanged;
		public static event PropertyChangedEventHandler CollectionChanged;
		private void FireCollectionChangedEvent(PropertyChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(Context.Instance, e);
		}

		// Override this method in order to access the containing user controls
		// from the default Collection Editor form or to add new ones...
		protected override CollectionForm CreateCollectionForm()
		{
			// Getting the default layout of the Collection Editor...
			CollectionForm collectionForm = base.CreateCollectionForm();

			Form frmCollectionEditorForm = collectionForm as Form;
			TableLayoutPanel tlpLayout = frmCollectionEditorForm.Controls[0] as TableLayoutPanel;

			if (tlpLayout != null)
			{
				// Get a reference to the inner PropertyGrid and hook
				// an event handler to it.
				if (tlpLayout.Controls[5] is PropertyGrid)
				{
					PropertyGrid propertyGrid = tlpLayout.Controls[5] as PropertyGrid;
					propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
				}
			}

			return collectionForm;
		}

		void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			// Fire our customized collection event...
			if (ObjectCollectionEditor.CollectionValueChanged != null)
			{
				ObjectCollectionEditor.CollectionValueChanged(Context.Instance,e);
			}
		}
		#endregion
	}
}
