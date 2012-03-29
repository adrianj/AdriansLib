using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DTALib
{
	/// <summary>
	/// A class that implements ICloneable by using Reflection to get and set all public Properties.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ReflectionClonedObject : ICloneable
	{
		public ReflectionClonedObject() { }


		public object Clone()
		{
			ReflectionClonedObject clone = Activator.CreateInstance(this.GetType()) as ReflectionClonedObject;
			foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				MethodInfo getter = pi.GetGetMethod();
				if (getter == null) continue;
				MethodInfo setter = pi.GetSetMethod();
				if (setter == null) continue;
				object value = getter.Invoke(this, new object[0]);
				if (value is ICloneable)
					value = (value as ICloneable).Clone();
				setter.Invoke(clone, new object[] { value });
			}

			return clone;
		}
	}
}
