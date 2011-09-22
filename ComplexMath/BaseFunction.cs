using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ComplexMath
{

    public interface ICFunction
    {
        string Name { get; }
        CDoubleArray Eval();
        List<ICFunction> Children { get; set; }
        bool AlreadyEvaluated {get;set;}
    }

    /// <summary>
    /// Delegate function which is called if an unknown function label is encountered.
    /// </summary>
    /// <param name="functionDescription"></param>
    /// <returns></returns>
    public delegate CDoubleArray NodeLabelCallback(string label);

    public abstract class BaseFunction : ICFunction
    {
        public static string FunctionsToString(ICFunction[] ComplexFunctions, string seperator)
        {
            string s = "";
            if (ComplexFunctions == null || ComplexFunctions.Length < 1) return s;
            s += "" + ComplexFunctions[0];
            for (int i = 1; i < ComplexFunctions.Length; i++)
                s += seperator + ComplexFunctions[i];
            return s;
        }


        public static Dictionary<ICFunction, ICFunction> AllNodes = new Dictionary<ICFunction, ICFunction>();

        public static char[] DisallowedLabelCharacters = new char[] { ' ', '\t', '\n', '\r', '+', '/', ')', '(', '-', ';', ',', '.','=',':' };

        public virtual string Name { get { return "" + this.GetType().Name; } }
        private List<ICFunction> mChildren = new List<ICFunction>();
        public virtual List<ICFunction> Children
        {
            get { return mChildren; }
            set { mChildren = value; }
        }

        public BaseFunction() { }

        private bool mEvaluated = false;
        /// <summary>
        /// A flag indicating whether function has been evaluated.
        /// If it already has been, then further calls to Eval() will be faster
        /// as it can use the previous value.
        /// To restart a calculation, first set this flag to false.
        /// </summary>
        public virtual bool AlreadyEvaluated
        {
            get
            { return mEvaluated; }
            set
            {
                mEvaluated = value;
                if (!value)
                {
                    foreach (ICFunction icf in Children) icf.AlreadyEvaluated = value;
                }
            }
        }
        protected CDoubleArray mEvalResult = null;


        public override bool Equals(object obj)
        {
            // if actually same address, return true.
            if (base.Equals(obj)) return true;
            // Must be same type, same Name.
            if (obj.GetType() != this.GetType()) return false;
            bool ret = obj.ToString().Equals(this.ToString());
            return ret;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            string s = this.Name + "( ";
            if (Children != null && Children.Count > 0)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    s += "" + Children[i];
                    if (i < Children.Count - 1) s += ", ";
                }
            }
            return s + " )";
        }

        /// <summary>
        /// Inheritors should implement this function to evaluate the function.
        /// Alternatively, if it is a simple function that performs position-wise methods,
        /// (eg, "+" in Matlab) it is also ok to implement the Eval(a,b) method.
        /// </summary>
        /// <returns>Evaluated array of Complex Doubles</returns>
        public  CDoubleArray Eval()
        {
            if (mEvaluated) return mEvalResult;
            // Check if no children. Call Eval(null) anyway, as some functions have no children but
            // still return a result (eg, constants, outsourced)
            if (Children == null || Children.Count == 0)
                return returnResult(Eval(null,null));
            List<CDoubleArray> eval = new List<CDoubleArray>();
            List<string> stringList = new List<string>();
            for (int i = 0; i < Children.Count; i++)
            {
                eval.Add(Children[i].Eval());
                stringList.Add(""+Children[i]);
            }

            return returnResult(Eval(eval,stringList));
        }

        private CDoubleArray returnResult(CDoubleArray ret)
        {
            AlreadyEvaluated = true;
            mEvalResult = ret;
            return mEvalResult;
        }

        protected virtual CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList == null || inList.Count < 1 || inList[0] == null) return null;
            CDoubleArray r = null;
            if (inList.Count == 1)
            {
                r = new CDoubleArray(inList[0].Length);
                if (inList[0] == null) return null;
                for (int i = 0; i < inList[0].Length; i++)
                {
                    CDouble rr = EvalDouble(inList[0][i]);
                    r[i] = rr;
                }
                return r;
            }
            // Any function expecting more than 2 children should implement Eval(List<> directly)
            else
            {
                if (inList[1] == null) return inList[0];
                // Equalise lengths.
                int length = Math.Min(inList[0].Length, inList[1].Length);
                // If one of the inputs is a single digit, then apply it to entire other array.
                if (length == 1) length = Math.Max(inList[0].Length, inList[1].Length);
                r = new CDoubleArray(length);
                for (int i = 0; i < length; i++)
                {
                    CDouble rr = EvalDouble(inList[0][i], inList[1][i]);
                    r[i] = rr;
                }
                return r;
            }
            //return null;
        }



        public virtual CDouble EvalDouble(CDouble a)
        {
            return a;
        }

        public virtual CDouble EvalDouble(CDouble a, CDouble b)
        {
            return EvalDouble(a);
        }


        public static string removeWhitespace(string s)
        {
            string ret = "";
            for (int i = 0; i < s.Length; i++)
                if (!Char.IsWhiteSpace(s, i)) ret += s[i];
            return ret;
        }

        public static List<Type> GetFunctions()
        {
            Type[] types = Assembly.GetCallingAssembly().GetTypes();
            List<Type> ret = new List<Type>();
            foreach (Type t in types)
                if (t.GetInterfaces().Contains(typeof(ICFunction))) ret.Add(t);
            return ret;
        }

        public static ICFunction GetFunctionFromLabel(string label)
        {
            List<Type> typeList = GetFunctions();
            foreach (Type t in typeList)
                if (t.Name.Equals(label))
                    return Activator.CreateInstance(t) as ICFunction;
            return null;
        }

        public static ICFunction AddNodeToList(ICFunction ret)
        {
            // If this node already exists use the existing one and ignore this one!
            if (BaseFunction.AllNodes.ContainsKey(ret)) return BaseFunction.AllNodes[ret];
            BaseFunction.AllNodes[ret] = ret;
            return ret;
        }


        public static void Reset()
        {
            foreach (ICFunction icf in AllNodes.Values)
                icf.AlreadyEvaluated = false;
        }

        public static ICFunction CreateNode(string label, NodeLabelCallback callBack)
        {
            label = removeWhitespace(label);
            ICFunction ret;

            // if it has an '=' then it's a variable.
            // Rearrange X = Y into Var(X,Y) format.
            if (label.Contains("="))
            {
                string[] pars = label.Split(new char[] { '=' });
                label = "Var(" + pars[0] + "," + pars[1] + ")";
            }

            // Split on first '('
            string[] split = label.Split(new char[] { '(' }, 2);
            ret = GetFunctionFromLabel(split[0]);
            // If null, then it's either a constant or node.
            if (split.Length < 2 || ret == null)
            {
                ret = ConstantFunction.CreateConstantFunction(label, split[0]);
                if (ret != null)
                    return AddNodeToList(ret);
                ret = OutsourcedFunction.CreateOutsourcedFunction(split[0], callBack);
                if (ret != null)
                    return AddNodeToList(ret);

            }
            // Now need to find children represented by remainder of split: split[1]
            // eg "4,5,Sub(3,4))"
            List<ICFunction> children = new List<ICFunction>();
            string child = "";
            int relevance = 0;
            foreach (char c in split[1])
            {
                if (c == '(') relevance++;
                if (c == ')') relevance--;
                child += c;
                if (relevance == 0)
                {
                    if (c == ')')
                    {
                        child = child.TrimEnd(new char[] { ')' });
                        if (child.Length > 0)
                        {
                            ICFunction icf = CreateNode(child, callBack);
                            children.Add(icf);
                        }
                        child = "";
                    }
                    if (c == ',')
                    {
                        child = child.TrimEnd(new char[] { ',' });
                        if (child.Length > 0)
                        {
                            ICFunction icf = CreateNode(child, callBack);
                            children.Add(icf);
                        }
                        child = "";
                    }
                }
            }
            child = child.TrimEnd(new char[] { ',', ')' });
            if (child.Length > 0)
            {
                ICFunction icff = CreateNode(child, callBack);
                children.Add(icff);
            }
            ret.Children = children;

            return AddNodeToList(ret);
        }

    }
}
