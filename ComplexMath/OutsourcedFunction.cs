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
			if (Callback == null)
				return null;
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

		public static void SetCallbackFunction(ICFunction function, NodeLabelCallback callback)
		{
			if (function is OutsourcedFunction)
				(function as OutsourcedFunction).Callback = callback;
			foreach (ICFunction child in function.Children)
				SetCallbackFunction(child, callback);
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
                if (value.Count >= 1 && value[0] is OutsourcedFunction)
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

    }
}
