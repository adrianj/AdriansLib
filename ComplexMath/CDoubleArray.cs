using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexMath
{
    public class CDoubleArray
    {
        private double [] real;
        private double[] imag;

        public CDouble this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }
        //public double R { get { return r[0]; } set { r[0] = value; } }
        //public double I { get { return i[0]; } set { i[0] = value; } }
        public double[] Real
        {
            get
            {
                return real;
            }
        }
        public double[] Imag
        {
            get
            {
                return imag;
            }
        }
        public int Length
        {
            get
            {
                if (imag == null) return real.Length;
                return Math.Max(real.Length, imag.Length);
            }
        }

        public CDoubleArray(int length)
        {
            real = new  double[length];
            imag = new double[length];
        }

        public CDoubleArray(IEnumerable<CDouble> cd)
        {
            real = new double[cd.Count()];
			imag = new double[cd.Count()];
			int i = 0;
			foreach(CDouble cdub in cd)
            {
				real[i] = cdub.R;
				imag[i] = cdub.I;
            }
        }
        /*
        public CDoubleArray(CDoubleArray copy)
        {
            copy.Real.CopyTo(this.Real, 0);
            copy.Imag.CopyTo(this.Imag, 0);
        }
         */

        /*
        public CDoubleArray(int length, double data)
            : this(length)
        {
            for (int i = 0; i < length; i++) Real[i] = data;
        }
         */

        public CDoubleArray(double r) : this(new double[]{r}, null) { }
        public CDoubleArray(double r, double i)
            : this(1)
        {
            real[0] = r;
            imag[0] = i;
        }
        public CDoubleArray(IEnumerable<double> r) : this(r, null) { }

		public CDoubleArray(IEnumerable<double> r, IEnumerable<double> i)
        {
			if(r != null)
            real = r.ToArray();
			if(i != null)
            imag = i.ToArray();
        }
         
        /*
        public CDoubleArray GetRange(int start, int end)
        {
            CDoubleArray c = new CDoubleArray(end - start);
            return c;
        }
         */
        public void EqualiseLengths()
        {
            if (real.Length > imag.Length)
            {
                double[] d = new double[imag.Length];
                Array.Copy(real, d, imag.Length);
                real = d;
            }
            if (real.Length < imag.Length)
            {
                double[] d = new double[real.Length];
                Array.Copy(imag, d, real.Length);
                imag = d;
            }
        }

        public CDouble Get(int index)
        {
            double r = 0;
            if (real != null && real.Length > 0)
            {
                r = real[0];
                if (index < real.Length) r = real[index];
            }

            double i = 0;
            if (imag != null && imag.Length > 0)
            {
                i = imag[0];
                if (index < imag.Length) i = imag[index];
            }
            return new CDouble(r, i);
        }

        public void Set(int index, CDouble d)
        {
            if (index >= 0 && index < real.Length) real[index] = d.R;
            if (index >= 0 && index < imag.Length) imag[index] = d.I;
        }


        public string Print()
        {
            return Print(", ", 10,true);
        }
        public string Print(string gapToken, int digits, bool forceSign)
        {
            string ret = "";
            if(this.Length <3  && this[0].I != 0 &&  !double.IsNaN(this[0].I))
                return printDouble(this[0].R,digits,forceSign) + " + "+printDouble(this[0].I,digits,forceSign)+" j";
            bool first = true;
            for (int i = 0; i < Math.Min(3,this.Length); i++)
            {
                if (!first) ret += gapToken; first = false;
                string s = printDouble(this[0].R, digits, forceSign);
                ret += s;
            }
            return ret;
        }
        private string printDouble(double val, int digits, bool forceSign)
        {
            string s = "" + Math.Round(val,digits);
            
            if (s.Length > digits && digits > 5)
            {
                if (val < 0) digits--;
                s = val.ToString("e"+(digits-7));
            }
             
            return s;
        }

    }
}
