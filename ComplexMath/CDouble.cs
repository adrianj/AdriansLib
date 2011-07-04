using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexMath
{
    public class CDouble
    {
        public double R { get; set; }
        public double I { get; set; }
        public CDouble(double r, double i)
        {
            R = r;
            I = i;
        }
        public CDouble(double r) : this(r, 0) { }
        public CDouble() { }
    }
}
