using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ComplexMath
{
    public class Plot : BaseFunction
    {
        public static Color DefaultColor = Color.BlanchedAlmond;
        public static string DefaultSymbol = "None";
        public Plot()
            : base()
        {
            SymbolString = DefaultSymbol;
            LineColor = DefaultColor;
        }
        public string SymbolString { get; set; }
        public Color LineColor { get; set; }
        private string mLegend = null;
        public string Legend { get { return getLegend(); } set { mLegend = value; } }


        /// <summary>
        /// Evaluate.
        /// 
        /// The CDoubleArray list should have 1 or 2 non-null elements.
        /// If 1 element then Eval will make sure the real and imaginary parts are of
        /// equal length, and these become X and Y for the plot.
        /// If 2 elements, then the real parts are used as X and Y.
        /// 
        /// functionNames are used to determine extra pieces of information, eg, LineColour
        /// and SymbolString in that order.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="functionNames"></param>
        /// <returns></returns>
        protected override CDoubleArray Eval(List<CDoubleArray> a, List<string> functionNames)
        {
            if (a.Count < 1 || a[0] == null) return null;
            
            if (a.Count == 1 || a[1] == null)
            {
                applyFunctionNames(functionNames, 1);
                if (a[0].Imag == null || a[0].Imag.Length < 1)
                {
                    double[] index = new double[a[0].Length];
                    for (int i = 0; i < index.Length; i++) index[i] = (double)i;
                    return new CDoubleArray(index,a[0].Real);
                }
                a[0].EqualiseLengths();
                return a[0];
            }
            applyFunctionNames(functionNames, 2);
            CDoubleArray cd = new CDoubleArray(a[0].Real, a[1].Real);
            cd.EqualiseLengths();
            return cd;
        }

        private void applyFunctionNames(List<string> functionNames, int nonNullElements)
        {
            if (functionNames.Count >= 1 + nonNullElements
                && functionNames[nonNullElements] != null)
            {
                LineColor = Color.FromName(functionNames[nonNullElements]);
                if (!LineColor.Name.Equals("Black", StringComparison.InvariantCultureIgnoreCase)
                    && LineColor.A == 0 && LineColor.R == 0 && LineColor.G == 0 && LineColor.B == 0)
                    LineColor = DefaultColor;
            }
            else LineColor = DefaultColor;

            if (functionNames.Count >= 2 + nonNullElements
                && functionNames[1 + nonNullElements] != null)
                SymbolString = functionNames[1 + nonNullElements];
            else SymbolString = DefaultSymbol;
        }

        private string getLegend()
        {
            if (mLegend != null) return mLegend;
            if (Children.Count == 0 || Children[0] == null) return "";
            if (Children.Count == 1 || Children[1] == null) return "" + Children[0];
            if (Children[1].Eval() == null) return "" + Children[0];
            return ""+Children[1];
        }
    }
}
