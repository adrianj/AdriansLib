using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace DTALib
{
    public class PropertyParser
    {
		public const string FILE_FILTER = "Settings files (*.xml)|*.xml|All Files (*.*)|*.*";

		public delegate string ToStringMethod<T>(T item);

		public static string[] ConvertListToStringArray<T>(IEnumerable<T> list, ToStringMethod<T> method)
		{
			string[] ret = new string[list.Count()];
			int i = 0;
			foreach (T obj in list)
				ret[i++] = method.Invoke(obj);
			return ret;
		}

		public static string ConvertListToString<T>(IEnumerable<T> list, ToStringMethod<T> method, string seperator)
		{
			string[] ret = ConvertListToStringArray(list, method);
			return ConvertListToString(ret, seperator);
		}

		public static string ConvertListToString(string[] list, string seperator)
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (string s in list)
			{
				if (!first)
					sb.Append(seperator);
				first = false;
				sb.Append(s);
			}
			return "" + sb;
		}


		public static void WriteObjectToXml(string filename, object obj)
		{
			using (XmlWriter writer = XmlWriter.Create(filename))
			{
				writer.WriteStartDocument();
				writer.WriteComment("XML Generated based on code structure. Do not manually edit structure as this will likely cause a parsing error.\n");
				writer.WriteStartElement(obj.GetType().FullName);
				WriteObjectToXml(writer, "Root", obj, obj.GetType());
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}
		

        private static void WriteObjectToXml(XmlWriter writer, string name, object obj, Type typ)
        {
            if (obj == null || typ == null)
            {
                writer.WriteElementString("Value", "");
                return;
            }
            if (typ.IsArray)
            {
                Array a = obj as Array;
                writer.WriteStartElement("Array");
                writer.WriteElementString("Length", "" + a.Length);
                for (int i = 0; i < a.Length; i++)
                {
                    object o = a.GetValue(i);
                    WriteObjectToXml(writer, "" + i, o, a.GetType().GetElementType());
                }
				writer.WriteEndElement();
				writer.WriteWhitespace("\n");
                return;
            }
            else if (typ.GetInterface("System.Collections.IList") != null)
            {
                IList l = obj as IList;
                writer.WriteStartElement("List");
                writer.WriteElementString("Length", "" + l.Count);
                for (int i = 0; i < l.Count; i++)
                {
                    object o = l[i];
                    if (o == null) WriteObjectToXml(writer, "" + i, null, null);
                    else WriteObjectToXml(writer, "" + i, o, o.GetType());
                }
				writer.WriteEndElement();
				writer.WriteWhitespace("\n");
                return;
            }
            else
            {
                List<PropertyInfo> props = getParseableProperties(typ);
                if (props.Count > 0)
                {
					writer.WriteStartElement("Class");
					writer.WriteElementString("Type", typ.AssemblyQualifiedName);
					writer.WriteWhitespace("\n");
                    foreach (PropertyInfo p in props)
                    {
                        MethodInfo getter = p.GetGetMethod();
                        object val = getter.Invoke(obj, null);
                        writer.WriteStartElement("Property");
                        writer.WriteElementString("Name", p.Name);
						//writer.WriteElementString("Type", p.PropertyType.AssemblyQualifiedName);
                        WriteObjectToXml(writer, p.Name, val, p.PropertyType);
                        writer.WriteEndElement();
						writer.WriteWhitespace("\n");
                    }
					writer.WriteEndElement();
					writer.WriteWhitespace("\n");
                    return;
				}
				TypeConverter converter = TypeDescriptor.GetConverter(obj);
				if (converter != null && converter.CanConvertTo(typeof(string)))
				{
					string s = converter.ConvertTo(obj, typeof(string)) as string;
					writer.WriteElementString("Value", s);
				}
				else
					writer.WriteElementString("Value", ""+obj);
            }
        }

        public static PropertyInfo getParseableProperty(Type targetType, string propertyName)
        {
            List<PropertyInfo> ps = getParseableProperties(targetType);
            foreach(PropertyInfo p in ps)
                if(p.Name.Equals(propertyName)) return p;
            return null;
        }
        public static List<PropertyInfo> getParseableProperties(Type targetType)
        {           
            List<PropertyInfo> ret = new List<PropertyInfo>();
            foreach (PropertyInfo p in targetType.GetProperties())
            {
                object[] attr = p.GetCustomAttributes(typeof(Parseable), true);
                MethodInfo getter = p.GetGetMethod();
                MethodInfo setter = p.GetSetMethod();
                if (attr.Length > 0 && getter != null && setter != null)
                    ret.Add(p);
            }
            return ret;
        }

        public static bool TryParseObject(string value, Type type, ref object par)
        {
			try
			{
				if (type == typeof(string))
				{
					par = value;
					return true;
				}
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if (converter.CanConvertFrom(typeof(string)))
				{
									
					par = converter.ConvertFrom(value);
					return true;
				}
				else
				{
					if (type == null || string.IsNullOrWhiteSpace(value)) return false;
					par = InvokeParseMethod(type, value);
					return true;
				}
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (ArithmeticException)
			{
				return false;
			}
			catch (FormatException)
			{
				return false;
			}
        }


		public static object InvokeParseMethod(Type type, string value)
		{
			MethodInfo parseMethod = type.GetMethod("Parse");
			if (parseMethod != null)
			{
				return parseMethod.Invoke(null, new object[] { value });
			}
			if (type.IsSubclassOf(typeof(Enum)))
			{
				return Enum.Parse(type, value);
			}
			if (type == typeof(Type))
			{
				return Type.GetType(value);
			}
			throw new ArgumentException("No Parse Method available for Type '" + type + "'");
		}


		public static void ReadObjectFromXml(string filename, object target)
		{
			using (PropertyReader reader = new PropertyReader(filename))
			{
				
				if (reader.FileType == null || !reader.FileType.Equals(target.GetType().FullName))
					throw new ArgumentException("Target object type does not match file type");
				reader.ReadObjectFromXml(target, target.GetType());
			}
		}


    }

    public class PropertyReader : IDisposable
    {
        private XmlReader reader = null;
        private string mFileType = null;
        public string FileType { get { return mFileType; } }

        public bool Read()
        {
            return reader.Read();
        }

        public bool EOF { get { return reader.EOF; } }

        public PropertyReader(string filename)
        {
            reader = XmlReader.Create(filename);
            // Skip comment header
            Read();
            // Skip comment text
            Read();
            // Read main file type.
            Read();
            mFileType = reader.Name;

        }

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
            }
        }

        public object ReadValueFromXml(Type type)
        {
            string value = reader.ReadElementString();
            object par = null;
            PropertyParser.TryParseObject(value, type,ref par);
            return par;
        }

        public object InvokeGetOrCreate(object parent, PropertyInfo pInfo)
        {
            if (pInfo == null || parent == null) return null;
            object target = pInfo.GetGetMethod().Invoke(parent, null);
            if (target != null) return target;
			if (pInfo.PropertyType != null)
			{
				target = CreateInstance(pInfo.PropertyType);
				return target;
			}
			return null;
        }

		private object CreateInstance(Type type) { return CreateInstance(type, new object[0]); }
		private object CreateInstance(Type type, object [] args)
		{
			try
			{
				if (type == null) return null;
				if (type == typeof(Array))
					return Array.CreateInstance(type.GetElementType(), args.Cast<int>().ToArray());
				if (type == typeof(string))
					return "";
				return Activator.CreateInstance(type, args);
			}
			catch (TargetInvocationException tie) { Console.WriteLine("" + tie.Message + Environment.NewLine + tie.StackTrace); return null; }
		}

        public object ReadArrayFromXml(Array target, Type arrayType)
        {
            Array ret = target;
            string lens = reader.ReadElementString();
            int len = 0;
            int.TryParse(lens, out len);
            if (target.Length != len)
            {
                ret = CreateInstance(arrayType, new object[]{len}) as Array;
				if (ret == null) return target;
                Array.Copy(target, ret, Math.Min(ret.Length,target.Length));
            }
            for (int i = 0; i < len; i++)
            {
                object o = ret.GetValue(i);
                object newValue = ReadObjectFromXml(o, arrayType.GetElementType());
                ret.SetValue(newValue,i);
            }
            // Skip over the end Array element.
            Read();
            return ret;
        }

        public object ReadListFromXml(IList parent, Type listType)
        {
            string lens = reader.ReadElementString();
            int len = 0;
            int.TryParse(lens, out len);
			if (parent == null)
				parent = (IList)CreateInstance(listType);
			if (parent == null)
				return parent;
			while (parent.Count > len)
				parent.RemoveAt(parent.Count - 1);
            for (int i = 0; i < len; i++)
            {
                Type elementType = listType.GetElementType(); 
                if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    elementType = listType.GetGenericArguments()[0];
                }
                object previous = null;
				if (i < parent.Count)
					previous = parent[i];
                object newValue = ReadObjectFromXml(previous, elementType);
                if (i < parent.Count)
					parent[i] = newValue;
                else
					parent.Add(newValue);
            }
            // Skip over the end List element.
			Read();
			return parent;
        }

        public object ReadClassFromXml(object parent)
        {
			string typeString = reader.ReadElementString();
			Type parentType = Type.GetType(typeString);
			if(parentType == null && parent != null)
				parentType = parent.GetType();
			if (parent == null || parent.GetType() != parentType)
				parent = CreateInstance(parentType);
            // Keep reading through the Start Property elements.
            while (true)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("Property"))
                {
					if (parentType != null)
					{
						PropertyInfo pInfo = null;
						object property = ReadPropertyFromXml(parent, parentType, out pInfo);
						if (property != null && parent != null)
							pInfo.GetSetMethod().Invoke(parent, new object[] { property });
					}
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("Class"))
                {
                    Read();
                    break;
                }
				if (!Read()) break;
            }
			return parent;
        }

        public object ReadPropertyFromXml(object parent, Type parentType, out PropertyInfo pInfo)
        {
            object ret = null;
            string name = reader.ReadElementString();
            pInfo = PropertyParser.getParseableProperty(parentType, name);
			if (pInfo == null) return null;
            ret = InvokeGetOrCreate(parent, pInfo);
            ret = ReadObjectFromXml(ret, pInfo.PropertyType);
            return ret;
        }

        string d = "";

		public object ReadObjectFromXml(object parent, Type parentType)
		{
			d += "---+";
			object ret = null;
			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name.Equals("Value"))
					{
						ret = ReadValueFromXml(parentType);
						break;
					}
					else if (reader.Name.Equals("Class"))
					{
						ret = ReadClassFromXml(parent);
						break;
					}
					else if (reader.Name.Equals("Property"))
					{
						PropertyInfo pInfo;
						ret = ReadPropertyFromXml(parent, parentType, out pInfo);
						break;
					}
					else if (reader.Name.Equals("Array"))
					{
						ret = ReadArrayFromXml((Array)parent, parentType);
						break;
					}
					else if (reader.Name.Equals("List"))
					{
						ret = ReadListFromXml((IList)parent, parentType);
						break;
					}
				}
				if (!Read()) break;
			}
			if (d.Length > 4) d = d.Substring(4);
			return ret;
		}


        private Array ReadArrayFromXml(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Text) return null;
            Type t = Type.GetType(reader.Value);
            if (!t.IsArray) return null;
            reader.ReadElementString();
            return null;
        }

    }


    public class Parseable : Attribute {	}

}
