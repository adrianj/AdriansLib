using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace ComplexMath
{
    public class GraphSymbol
    {
        
        public static Dictionary<string, GraphicsPath> UserSymbols = new Dictionary<string,GraphicsPath>();

        public static Symbol GetSymbol(string s, Color color)
        {
            if (UserSymbols.ContainsKey(s))
            {
                Symbol ret = new Symbol(SymbolType.UserDefined, color);
                ret.UserSymbol = UserSymbols[s];
                return ret;
            }
            if (s.Equals("Square")) return new Symbol(SymbolType.Square, color);
            if (s.Equals("Circle")) return new Symbol(SymbolType.Circle, color);
            if (s.Equals("Diamond")) return new Symbol(SymbolType.Diamond, color);
            if (s.Equals("HDash")) return new Symbol(SymbolType.HDash, color);
            if (s.Equals("None")) return new Symbol(SymbolType.None, color);
            if (s.Equals("Plus")) return new Symbol(SymbolType.Plus, color);
            if (s.Equals("Star")) return new Symbol(SymbolType.Star, color);
            if (s.Equals("Triangle")) return new Symbol(SymbolType.Triangle, color);
            if (s.Equals("TriangleDown")) return new Symbol(SymbolType.TriangleDown, color);
            if (s.Equals("UserDefined")) return new Symbol(SymbolType.UserDefined, color);
            if (s.Equals("VDash")) return new Symbol(SymbolType.VDash, color);
            if (s.Equals("XCross")) return new Symbol(SymbolType.XCross, color);
            return new Symbol(SymbolType.Default, color);
        }

		static GraphSymbol()
		{
			GraphicsPath graph = new GraphicsPath();
			graph.AddLine(new Point(100, 0), new Point(-100, 0));
			graph.AddLine(new Point(0, 100), new Point(0, -100));
			UserSymbols["Highlight"] = graph;
		}
         
    }
}
