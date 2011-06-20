using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

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
                writer.WriteStartElement(obj.GetType().Name);
                writer.WriteElementString("Root", obj.GetType().FullName);
                WriteObjectToXml(writer, "", obj);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }


        public static void WriteObjectToXml(XmlWriter writer, string name, object obj)
        {
            if (obj == null) return;
            writer.WriteStartElement("Property");
            Type t = obj.GetType();
            PropertyInfo[] ps = t.GetProperties();
            writer.WriteElementString("Name", name);
            writer.WriteElementString("Type", obj.GetType().Name);
            writer.WriteElementString("Value", "" + obj);

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

        public static object ReadObjectFromXml(XmlReader reader, string name, object target)
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
                        ReadObjectFromXml(reader, name+"."+Name, target);
                    }
                    else prevName = reader.Name;
                    //Console.WriteLine(node+"Element: '" + reader.Name + "','" + reader.Value + "'");
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    //Console.WriteLine(node+"Text: '" + prevName + "','" + reader.Value + "'");
                    if (prevName.Equals("Name")) Name = reader.Value;
                    if (prevName.Equals("Type")) Typ = reader.Value;
                    if (prevName.Equals("Value"))
                    {
                        Value = reader.Value;
                    }
                    if (prevName.Equals("Length")) Length = reader.Value;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name.Equals("Property"))
                    {
                        AddDataToObject(target, name + "." + Name, Typ, Value);
                        break;
                    }

                }
                else
                {

                    //Console.WriteLine(node + "" + reader.NodeType + ": '" + reader.Name + "','" + reader.Value + "'");
                }
            }
            if (node.Length > 4) node = node.Substring(0, node.Length - 4);
            return target;
        }

        public static void AddDataToObject(object parentObj, string objPath, string type, string value)
        {
            try
            {
                // Exactly which object are we operating on here...
                string[] tree = objPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (tree == null || tree.Length < 1) return;
                string name = tree[tree.Length - 1];
                object[] objTree = new object[tree.Length + 1];
                Type[] typeTree = new Type[tree.Length + 1];
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

                if (setter == null) return;
                Type setType = null;
                setType = typeTree[objTree.Length - 2].GetProperty(name).PropertyType;
                if (setType == typeof(string))
                {
                    object par = value;
                    setter.Invoke(objTree[objTree.Length - 2], new object[] { value });
                    return;
                }

                if (setType.IsSubclassOf(typeof(Enum)))
                {
                    object par = Enum.Parse(setType, value);
                    setter.Invoke(objTree[objTree.Length - 2], new object[] { par });
                    return;
                }

                MethodInfo parseMethod = setType.GetMethod("Parse", new Type[] { typeof(string) });
                if (parseMethod != null)
                {
                    object par = parseMethod.Invoke(null, new object[] { value });
                    setter.Invoke(objTree[objTree.Length - 2], new object[] { par });
                    return;
                }

            }
            catch (Exception e) { Console.WriteLine("" + e); throw; }
        }


        public static bool ReadObjectFromXml(string filename, object target)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                bool success = false;
                try
                {
                    reader.ReadStartElement();
                    string s = reader.Name;
                    if (!s.Equals("Root"))
                        return false;
                    s = reader.ReadElementContentAsString();
                    if (!s.Equals(target.GetType().FullName)) return false;
                    ReadObjectFromXml(reader, "", target);
                    success = true;
                    object test = null;
                    test = AltReadObjectFromXml(reader, "", test);
                }
                catch (XmlException) { return success; }
            }
            return true;
        }




        public static object AltReadObjectFromXml(XmlReader reader, string name, object target)
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
                        AltReadObjectFromXml(reader, name + "." + Name, target);
                    }
                    else prevName = reader.Name;
                    //Console.WriteLine(node+"Element: '" + reader.Name + "','" + reader.Value + "'");
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    //Console.WriteLine(node+"Text: '" + prevName + "','" + reader.Value + "'");
                    if (prevName.Equals("Name")) Name = reader.Value;
                    if (prevName.Equals("Type")) Typ = reader.Value;
                    if (prevName.Equals("Value"))
                    {
                        Value = reader.Value;
                        Console.WriteLine("found value: " + Name + "," + Typ + "," + Value);
                    }
                    if (prevName.Equals("Length")) Length = reader.Value;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name.Equals("Property"))
                    {
                        AddDataToObject(target, name + "." + Name, Typ, Value);
                        break;
                    }

                }
                else
                {

                    //Console.WriteLine(node + "" + reader.NodeType + ": '" + reader.Name + "','" + reader.Value + "'");
                }
            }
            if (node.Length > 4) node = node.Substring(0, node.Length - 4);
            return target;
        }



    }

    public class Parseable : Attribute { }

}
