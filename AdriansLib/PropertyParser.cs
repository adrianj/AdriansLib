using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace DTALib
{
    public class PropertyParser
    {
        public const string FILE_FILTER = "Settings files (*.xml)|*.xml|All Files (*.*)|*.*";

        public static void WriteObjectToXml(string filename, object obj)
        {
                using(XmlWriter writer = XmlWriter.Create(filename))
                {
                writer.WriteStartDocument();
                writer.WriteComment("XML Generated based on code structure. Do not manually edit structure as this will likely cause a parsing error.");
                writer.WriteStartElement(obj.GetType().FullName);
                //writer.WriteElementString("Root", obj.GetType().FullName);
                //WriteObjectToXml(writer, "", obj);
                AltWriteObjectToXml(writer, "Root", obj,obj.GetType());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }


        private static void AltWriteObjectToXml(XmlWriter writer, string name, object obj, Type typ)
        {
            if (obj == null || typ == null)
            {
                //writer.WriteElementString("Type", "" + "null");
                writer.WriteElementString("Value", "");
                return;
            }
            if (typ.IsArray)
            {
                Array a = obj as Array;
                //writer.WriteElementString("ElementType", a.GetType().GetElementType().FullName);
                writer.WriteStartElement("Array");
                writer.WriteElementString("Length", "" + a.Length);
                for (int i = 0; i < a.Length; i++)
                {
                    object o = a.GetValue(i);
                    AltWriteObjectToXml(writer, "" + i, o, a.GetType().GetElementType());
                }
                writer.WriteEndElement();
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
                    if (o == null) AltWriteObjectToXml(writer, "" + i, null, null);
                    else AltWriteObjectToXml(writer, "" + i, o, o.GetType());
                }
                writer.WriteEndElement();
                return;
            }
            else
            {
                List<PropertyInfo> props = getParseableProperties(typ);
                if (props.Count > 0)
                {
                    writer.WriteStartElement("Class");
                    foreach (PropertyInfo p in props)
                    {
                        MethodInfo getter = p.GetGetMethod();
                        object val = getter.Invoke(obj, null);
                        writer.WriteStartElement("Property");
                        writer.WriteElementString("Name", p.Name);
                        AltWriteObjectToXml(writer, p.Name, val, p.PropertyType);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    return;
                }
                writer.WriteElementString("Value", "" + obj);
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

        private static void WriteObjectToXml(XmlWriter writer, string name, object obj)
        {
            if (obj == null) return;
            
            

            writer.WriteStartElement("Property");
            Type t = obj.GetType();
            PropertyInfo[] ps = t.GetProperties();
            writer.WriteElementString("Name", name);
            writer.WriteElementString("Type", obj.GetType().Name);

            if (obj.GetType().IsArray)
            {
                Console.WriteLine("PropertyParser: object is an array: " + obj);
                Array a = obj as Array;
                writer.WriteElementString("Length", "" + a.Length);
                for (int i = 0; i < a.Length; i++)
                {
                    object o = a.GetValue(i);
                    WriteObjectToXml(writer, "" + i, o);
                }
            }
            writer.WriteElementString("Value", "" + obj);
            
            foreach (PropertyInfo p in ps)
            {
                object[] attr = p.GetCustomAttributes(typeof(Parseable), true);
                MethodInfo getter = p.GetGetMethod();

                if (attr.Length > 0 && getter != null)
                {
                    object val = getter.Invoke(obj, null);
                    if(val != null)
                        WriteObjectToXml(writer, p.Name, val);
                }
            }
            writer.WriteEndElement();
        }

        private static string node = "";

        private static bool ReadObjectFromXml(XmlReader reader, string name, object target)
        {
            node += "----";
            string prevName = "";
            string Name = "";
            string Value = "";
            string Typ = "";
            string Length = "0";

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals("Property"))
                    {
                        if (!ReadObjectFromXml(reader, name + "." + Name, target)) return false;
                    }
                    else prevName = reader.Name;
                    //Console.WriteLine(node+"Element: '" + reader.Name + "','" + reader.Value + "'");
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    //Console.WriteLine(node+"Text: '" + prevName + "','" + reader.Value + "'");
                    if (prevName.Equals("Name")) Name = reader.Value;
                    if (prevName.Equals("Type")) Typ = reader.Value;
                    if (prevName.Equals("Value")) Value = reader.Value;                 
                    if (prevName.Equals("Length")) Length = reader.Value;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name.Equals("Property"))
                    {
                        if (!AddDataToObject(target, name + "." + Name, Typ, Value)) return false;
                        break;
                    }

                }
                else
                {

                    //Console.WriteLine(node + "" + reader.NodeType + ": '" + reader.Name + "','" + reader.Value + "'");
                }
            }
            if (node.Length > 4) node = node.Substring(0, node.Length - 4);
            //return target;
            return true;
        }

        public static bool TryParseObject(string value, Type type, out object par)
        {
            par = null;
            try
            {
                if (type == null || value == null) return false;
                MethodInfo parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
                if (type == typeof(string))
                {
                    par = value;
                    return true;
                }
                else if (type.IsSubclassOf(typeof(Enum)))
                {
                    par = Enum.Parse(type, value);
                    return true;
                }
                else if (parseMethod != null)
                {
                    par = parseMethod.Invoke(null, new object[] { value });
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is ArithmeticException || ex is FormatException)
                    return false;
                else
                    throw;
            }
            
            return false;
        }

        private static bool AddDataToObject(object parentObj, string objPath, string type, string value)
        {
            string[] tree = null;
            object[] objTree = null;
            Type[] typeTree = null;
            try
            {
                // Exactly which object are we operating on here...
                tree = objPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (tree == null || tree.Length < 1) return true;
                string name = tree[tree.Length - 1];
                objTree = new object[tree.Length + 1];
                typeTree = new Type[tree.Length + 1];
                PropertyInfo[] propTree = new PropertyInfo[tree.Length];
                objTree[0] = parentObj;
                for (int i = 0; i < tree.Length; i++)
                {
                    typeTree[i] = objTree[i].GetType();
                    propTree[i] = typeTree[i].GetProperty(tree[i], BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    MethodInfo getter = null;
                    if (propTree[i] != null)
                        getter = propTree[i].GetGetMethod();
                    if (getter != null)
                        objTree[i + 1] = getter.Invoke(objTree[i], null);

                }
                MethodInfo setter = null;
                typeTree[typeTree.Length - 1] = objTree[objTree.Length - 1].GetType();
                setter = propTree[propTree.Length - 1].GetSetMethod();

                if (setter == null) return true;
                Type setType = null;
                setType = typeTree[objTree.Length - 2].GetProperty(name).PropertyType;
                object par = null;
                TryParseObject(value,setType,out par);
                if(par != null)
                    setter.Invoke(objTree[objTree.Length - 2], new object[] { par });
                

            }
            catch (Exception e) { 
                Console.WriteLine("" + e +"\n"
                    +"\nParent: "+parentObj
                    + "\nObjPath: " + objPath
                    + "\nType: " + type
                    +"\nValue: "+value
                    +"\nTree: "+printArray(tree)
                    + "\nObjTree: " + printArray(objTree)
                    + "\nTypeTree: " + printArray(typeTree)
                    ); 
                return false; }
            return true;
        }

        private static string printArray(Array a)
        {
            if (a == null || a.Length < 1) return "";
            string ret = ""+a.GetValue(0);
            for (int i = 1; i < a.Length; i++)
                ret += ", " + a.GetValue(i);
            return ret;
        }

        
        public static void ReadObjectFromXml(string filename, object target)
        {
            PropertyReader reader = null;
            try
            {
                reader = new PropertyReader(filename);

                if (reader.FileType == null || !reader.FileType.Equals(target.GetType().FullName))
                    throw new ArgumentException("Target object type does not match file type");
                reader.ReadObjectFromXml(target, target.GetType());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null) reader.Dispose();
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
            PropertyParser.TryParseObject(value, type,out par);
            return par;
        }

        public object InvokeGetOrCreate(object parent, PropertyInfo pInfo)
        {
            if (pInfo == null || parent == null) return null;
            object target = pInfo.GetGetMethod().Invoke(parent, null);
            if (target != null) return target;
            try
            {
                target = Activator.CreateInstance(pInfo.PropertyType);
                return target;
            }
            catch (Exception) { return null; }
        }

        public object ReadArrayFromXml(Array target, Type arrayType)
        {
            Array ret = target;
            string lens = reader.ReadElementString();
            int len = 0;
            int.TryParse(lens, out len);
            if (target.Length != len)
            {
                ret = Array.CreateInstance(arrayType.GetElementType(), len);
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

        public object ReadListFromXml(IList target, Type listType)
        {
            IList ret = target;
            string lens = reader.ReadElementString();
            int len = 0;
            int.TryParse(lens, out len);
            if (ret == null)
                ret = (IList)Activator.CreateInstance(listType);
            for (int i = 0; i < len; i++)
            {
                Type elementType = listType.GetElementType(); 
                if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    elementType = listType.GetGenericArguments()[0]; // use this...
                }
                object previous = null;
                if(i < ret.Count)
                    previous = ret[i];
                object newValue = ReadObjectFromXml(previous, elementType);
                if (i < ret.Count)
                    ret[i] = newValue;
                else
                    ret.Add(newValue);
            }
            // Skip over the end List element.
            Read();
            return ret;
        }

        public object ReadClassFromXml(object parent, Type parentType)
        {
            object ret = parent;
            // Keep reading through the Start Property elements.
            while (Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("Property"))
                {
                    PropertyInfo pInfo = null;
                    object property = ReadPropertyFromXml(parent, parentType, out pInfo);
                    if (property != null && ret != null)
                        pInfo.GetSetMethod().Invoke(parent, new object[] { property });
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("Class"))
                {
                    Read();
                    break;
                }
            }
            return ret;
        }

        public object ReadPropertyFromXml(object parent, Type parentType, out PropertyInfo pInfo)
        {
            object ret = null;
            string name = reader.ReadElementString();
            pInfo = PropertyParser.getParseableProperty(parentType, name);
            ret = InvokeGetOrCreate(parent, pInfo);
            ret = ReadObjectFromXml(ret, pInfo.PropertyType);
            return ret;
        }

        string d = "";

        public object ReadObjectFromXml(object parent, Type parentType)
        {
            try
            {
                d += "---+";
                //Console.WriteLine(d + "Entering ReadObject: parent: " + parent);
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
                            ret = ReadClassFromXml(parent, parentType);
                            break;
                        }
                        else if (reader.Name.Equals("Property"))
                        {
                            PropertyInfo pInfo;
                            ret = ReadPropertyFromXml(parent, parentType,out pInfo);
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
            catch (Exception) { throw; }
        }


        private Array ReadArrayFromXml(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Text) return null;
            Type t = Type.GetType(reader.Value);
            if (!t.IsArray) return null;
            reader.ReadElementString();
            //Array a = Array.CreateInstance(
            return null;
        }

    }


    public class Parseable : Attribute { }

}
