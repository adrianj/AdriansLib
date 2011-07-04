using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexMath
{

    public class ConstantFunction : BaseFunction
    {
        public CDoubleArray Data { get; set; }
        private string mName;
        public override string Name { get { return mName; } }
        public ConstantFunction(CDoubleArray data, string name)
        {
            Data = data;
            mName = name;
        }
        protected override CDoubleArray Eval(List<CDoubleArray> a, List<string> functionNames)
        {
            return Data;
        }

        public override string ToString()
        {
            return mName;
        }


        public static ConstantFunction CreateConstantFunction(string label, string function)
        {
            if (label.Length == 0 || function.Length == 0) return null;
            double d = 0;
            if (!double.TryParse(function, out d))
                return null;

            ConstantFunction cf = new ConstantFunction(new CDoubleArray(d), label);
            return cf;
            /*
            double start = 0;
            double step = 1;
            string[] split = function.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (!double.TryParse(split[0], out start))
            {
                return null;
            }
            double p1 = 0;
            double p2 = 0;
            double end = start;
            if (split.Length >= 2)
            {
                if (!double.TryParse(split[1], out p1))
                    return null;
            }
            if (split.Length >= 3)
            {
                if (!double.TryParse(split[2], out p2))
                   return null;
            }

            if (split.Length == 3)
            {
                step = p1;
                end = p2;
            }
            else if (split.Length == 2)
            {
                end = p1;
            }
            List<double> ret = new List<double>();
            ret.Add(start);
            while (step != 0)
            {
                start = start + step;
                if (step < 0 && start < end) break;
                if (step > 0 && start > end) break;
                ret.Add(start);
            }

            ConstantFunction cf = new ConstantFunction(new CDoubleArray(ret.ToArray()), label);
            return cf;
             */
        }
    }
    public class OutsourcedFunction : BaseFunction
    {
        private string mName;
        public override string Name { get { return mName; } }
        public NodeLabelCallback Callback { get; set; }
        public OutsourcedFunction(string name, NodeLabelCallback callback)
        {
            mName = name;
            Callback = callback;
        }

        protected override CDoubleArray Eval(List<CDoubleArray> a, List<string> functionNames)
        {
            if (AlreadyEvaluated) return mEvalResult;
            mEvalResult = Callback(Name);
            AlreadyEvaluated = true;
            return mEvalResult;
        }

        public override string ToString()
        {
            return mName;
        }

        public static OutsourcedFunction CreateOutsourcedFunction(string name, NodeLabelCallback callBack)
        {
            // Check if a Var type function has been desclared with this name
            foreach (ICFunction icf in BaseFunction.AllNodes.Values)
            {
                if (icf.GetType().Equals(typeof(Var)) && name.Equals(icf.Name))
                {
                    Var v = icf as Var;
                    OutsourcedFunction ret = new OutsourcedFunction(name, v.Callback);
                    return ret;
                }
            }
            OutsourcedFunction r = new OutsourcedFunction(name, callBack);
            return r;
        }
    }
    /// <summary>
    /// This function is a variable.
    /// It simply acts as a function with one child node, and it is this child that gets evaluated.
    /// </summary>
    public class Var : BaseFunction
    {
        public Var() : base() { }
        public override string ToString()
        {
            if (Children.Count >= 2)
                return "" + Children[0].Name + " = " + Children[1];
            return base.ToString();
        }
        public override List<ICFunction> Children
        {
            get
            {
                return base.Children;
            }
            set
            {
                if (value.Count >= 1 && value[0].GetType().Equals(typeof(OutsourcedFunction)))
                {
                    OutsourcedFunction osf = value[0] as OutsourcedFunction;
                    osf.Callback = Callback;
                }
                base.Children = value;
            }
        }
        public CDoubleArray Callback(string label)
        {
            return Children[1].Eval();
        }
        protected override CDoubleArray Eval(List<CDoubleArray> a, List<string> functionNames)
        {
            if (Children.Count < 2) return null;
            return Children[1].Eval();
        }

        /*
        protected override CDoubleArray Eval(CDoubleArray a, CDoubleArray b)
        {
            if (Children.Count < 2) return null;
            return Children[1].Eval();
        }
         */
    }
}
