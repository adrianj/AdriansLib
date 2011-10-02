using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexMath
{

    public class Add : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a, CDouble b)
        {
            return new CDouble(a.R + b.R, a.I + b.I);
        }
    }
    public class Sub : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a, CDouble b)
        {
            return new CDouble(a.R - b.R, a.I - b.I);
        }
    }
    public class Mul : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a, CDouble b)
        {
            if (double.IsNaN(a.I) || double.IsNaN(b.I))
                return new CDouble(a.R * b.R);
            return new CDouble(a.R * b.R - a.I * b.I, a.R * b.I - a.I * b.R);
        }
    }
    public class Div : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a, CDouble b)
        {
            return new CDouble(a.R / b.R, a.I / b.I);
        }
    }
    public class Sqrt : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a)
        {
            return new CDouble(Math.Sqrt(a.R));
        }
    }
    public class Sum : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList == null || inList.Count < 1 || inList[0] == null) return null;
            double sum = 0;
            for (int i = 0; i < inList[0].Length; i++)
                sum += inList[0].Real[i];
            return new CDoubleArray(sum);
        }
    }
    /// <summary>
    /// An Edge detection routine.
    /// Returns a list of indices identifying the samples immediately after a transition from 
    /// negative to positive.
    /// Is only interested in the Real component of the first argument.
    /// </summary>
    public class RisingEdge : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList == null || inList.Count < 1 || inList[0] == null || inList[0].Length < 2) return null;
            List<double> crossings = new List<double>();
            double current = inList[0].Real[0];
            for (int i = 1; i < inList[0].Length; i++)
            {
                if (current < 0 && inList[0].Real[i] >= 0)
                    crossings.Add((double)i);
                current = inList[0].Real[i];
            }
            if (crossings.Count < 1) return null;
            return new CDoubleArray(crossings.ToArray());
        }
    }
    /// <summary>
    /// An Edge detection routine.
    /// Returns a list of indices identifying the samples immediately after a transition from 
    /// negative to positive.
    /// Is only interested in the Real component of the first argument.
    /// </summary>
    public class FallingEdge : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList == null || inList.Count < 1 || inList[0] == null || inList[0].Length < 2) return null;
            List<double> crossings = new List<double>();
            double current = inList[0].Real[0];
            for (int i = 1; i < inList[0].Length; i++)
            {
                if (current >= 0 && inList[0].Real[i] < 0)
                    crossings.Add((double)i);
                current = inList[0].Real[i];
            }
            if (crossings.Count < 1) return null;
            return new CDoubleArray(crossings.ToArray());
        }
    }
    public class Mag : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a)
        {
            return new CDouble(Math.Sqrt(a.R * a.R + a.I * a.I));
        }
    }
    public class Abs : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a)
        {
            return new CDouble(Math.Abs(a.R));
        }
    }
    public class Atan : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a)
        {
            return new CDouble(Math.Atan2(a.R, a.I));
        }
    }
    public class FFT : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> a, List<string> functionNames)
        {
            if (a.Count < 1 || a[0] == null) return null;
            double[] real = a[0].Real;
            double[] imag = a[0].Imag;
            if (imag == null || imag.Length != real.Length) imag = new double[real.Length];
            if (Children.Count > 1)
                imag = Children[1].Eval().Real;
            // Find 2^x just less than data length
            long sample_rate = 1;
            while (sample_rate <= real.Length) sample_rate <<= 1;
            sample_rate >>= 1;
            //variables for the fft
            long n, mmax, m, j, istep, i;
            double wtemp, wr, wpr, wpi, wi, theta, tempr, tempi;
            //long number_of_samples = d.Length;

            double[] vector = new double[2 * sample_rate];
            for (i = 0; i < sample_rate; i++)
            {
                vector[i * 2] = real[i];
                vector[i * 2 + 1] = imag[i];
            }
            //binary inversion (note that the indices 
            //start from 0 which means that the
            //real part of the complex is on the even-indices 
            //and the complex part is on the odd-indices)
            n = sample_rate << 1;
            j = 0;
            for (i = 0; i < n / 2; i += 2)
            {
                if (j > i)
                {
                    swap(ref vector[j], ref vector[i]);
                    swap(ref vector[j + 1], ref vector[i + 1]);
                    if ((j / 2) < (n / 4))
                    {
                        swap(ref vector[(n - (i + 2))], ref vector[(n - (j + 2))]);
                        swap(ref vector[(n - (i + 2)) + 1], ref vector[(n - (j + 2)) + 1]);
                    }
                }
                m = n >> 1;
                while (m >= 2 && j >= m)
                {
                    j -= m;
                    m >>= 1;
                }
                j += m;
            }
            //end of the bit-reversed order algorithm

            //Danielson-Lanzcos routine
            mmax = 2;
            while (n > mmax)
            {
                istep = mmax << 1;
                theta = 2 * Math.PI / mmax;
                wtemp = Math.Sin(0.5 * theta);
                wpr = -2.0 * wtemp * wtemp;
                wpi = Math.Sin(theta);
                wr = 1.0;
                wi = 0.0;
                for (m = 1; m < mmax; m += 2)
                {
                    for (i = m; i <= n; i += istep)
                    {
                        j = i + mmax;
                        tempr = wr * vector[j - 1] - wi * vector[j];
                        tempi = wr * vector[j] + wi * vector[j - 1];
                        vector[j - 1] = vector[i - 1] - tempr;
                        vector[j] = vector[i] - tempi;
                        vector[i - 1] += tempr;
                        vector[i] += tempi;
                    }
                    wr = (wtemp = wr) * wpr - wi * wpi + wr;
                    wi = wi * wpr + wtemp * wpi + wi;
                }
                mmax = istep;
            }

            CDoubleArray ret = new CDoubleArray(vector.Length / 4);
            real = new double[sample_rate / 2];
            imag = new double[sample_rate / 2];
            for (i = 0; i < real.Length; i++)
            {
                real[i] = vector[i * 2];
                imag[i] = vector[i * 2 + 1];
            }

            return new CDoubleArray(real,imag);
        }
        private static void swap(ref double a, ref double b) { double t = a; a = b; b = t; }
    }
    public class Real : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> a, List<string> functionNames)
        {
            if (a == null || a.Count < 1 || a[0] == null) return null;
            return new CDoubleArray(a[0].Real);
        }
    }
    public class Imag : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> a, List<string> functionNames)
        {
            if (a == null || a.Count < 1 || a[0] == null) return null;
            return new CDoubleArray(a[0].Imag);
        }
    }
    /// <summary>
    /// Function to index into an array. Parameter a is the array to be indexed into, Parameter b has the list
    /// of indices to return.
    /// Indices are rounded to Nearest integer as opposed to Floor, to account for anomolies in double
    /// representations of ints.
    /// </summary>
    public class Index : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList.Count < 2 || inList[0] == null || inList[1] == null) return null;
            CDoubleArray a = inList[0];
            CDoubleArray b = inList[1];
            List<int> iList = new List<int>();
            List<int> imagList = new List<int>();
            for (int i = 0; i < b.Length; i++)
            {
                int index = (int)Math.Round(b[i].R);
                if (index >= 0 && index < a.Length)
                    iList.Add(index);
                if (a.Imag != null && index >= 0 && index < a.Imag.Length)
                    imagList.Add(index);
            }
            double[] rr = new double[iList.Count];
            double[] ri = null;
            for (int i = 0; i < rr.Length; i++)
            {
                rr[i] = a.Real[iList[i]];
            }
            if (imagList.Count > 0)
            {
                ri = new double[imagList.Count];
                for (int i = 0; i < ri.Length; i++)
                {
                    ri[i] = a.Imag[imagList[i]];
                }
            }
            return new CDoubleArray(rr,ri);
        }
    }
    public class Max : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {

            if (inList.Count < 1 || inList[0] == null) return null;
            CDoubleArray a = inList[0];
            int maxIndex = 0;
            double max = a[0].R;
            for (int i = 1; i < a.Length; i++)
            {
                if (a[i].R > max)
                {
                    maxIndex = i;
                    max = a[i].R;
                }
            }
            return new CDoubleArray(new double[] { max }, new double[] { maxIndex });
        }
    }
    /// <summary>
    /// Converts from Polar form into Rectangular form.
    /// Assumes that Child[0] contains magnitude, Child[1] contains phase
    /// </summary>
    public class Rect : BaseFunction
    {
        public override CDouble EvalDouble(CDouble a, CDouble b)
        {
            return new CDouble(a.R * Math.Cos(b.R),a.R * Math.Sin(b.R));
        }
    }
    /// <summary>
    /// A function for providing an evenly spaced vector
    /// </summary>
    public class Range : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList.Count < 1 || inList[0] == null) return null;
            double start = 0, step = 0, end = 0;
            start = inList[0][0].R;
            if ((inList.Count == 2 || inList[2] == null) && inList[1] != null)
            {
                end = inList[1][0].R;
                if (start < end) step = 1;
                else step = -1;
            }
            if (inList.Count == 3 && inList[2] != null)
            {
                end = inList[2][0].R;
                step = inList[1][0].R;
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
            return new CDoubleArray(ret.ToArray());
        }
    }
    public class Log : BaseFunction
    {
        private List<CDouble> log = new List<CDouble>();

        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList == null || inList.Count < 1 || inList[0] == null) return null;
            int count = 0;
            if (inList.Count >= 2 && inList[1] != null)
                count = (int)inList[1][0].R;

            for(int i = 0; i < inList[0].Length; i++)
                log.Add(inList[0][i]);
            if (count > 0 && count < log.Count)
                log = log.GetRange(log.Count-count, count);

            CDoubleArray ret = new CDoubleArray(log.ToArray());
            return ret;
        }
    }

	public class Counter : BaseFunction
	{
		int currentValue = 0;
		int terminalCount = int.MaxValue;

		protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
		{
			if (inList != null && inList.Count > 0 && inList[0] != null && inList[0].Length > 0)
				terminalCount = RestrictDoubleToIntRange(inList[0][0].R);
			double ret = (double)currentValue;
			currentValue = (currentValue + 1) % terminalCount;
			return new CDoubleArray(ret);
		}

		int RestrictDoubleToIntRange(double d)
		{
			if (d > int.MaxValue)
				return int.MaxValue;
			if (d < 1)
				return 1;
			return (int)d;
		}
	}

    public class Concat : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList == null) return null;
            List<double> real = new List<double>();
            List<double> imag = new List<double>();
            for (int i = 0; i < inList.Count; i++)
            {
                if (inList[i] == null) continue;
                if (inList[i].Real != null) real.AddRange(inList[i].Real);
                if (inList[i].Imag != null) real.AddRange(inList[i].Imag);
            }
            return new CDoubleArray(real.ToArray(), imag.ToArray());
        }
    }
    public class Length : BaseFunction
    {
        protected override CDoubleArray Eval(List<CDoubleArray> inList, List<string> functionNames)
        {
            if (inList == null || inList.Count < 1 || inList[0] == null) return null;
            return new CDoubleArray((double)inList[0].Length);
        }
    }
}
