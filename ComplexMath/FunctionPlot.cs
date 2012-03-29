using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Text;
using ZedGraph;

namespace ComplexMath
{
	public class FunctionPlot : SimplePlot
	{

		public ICFunction[] ComplexFunctions { get; set; }

		public string GetComplexFunctionsString(string seperator)
		{
			return BaseFunction.FunctionsToString(ComplexFunctions, seperator);
		}


		public void AutoGenerateScaleAndOffsets()
		{
			EnableScaling = false;
			PlotWaveform();
			PointPairList[] data = PlotData;
			double[] yScale = new double[data.Length];
			double[] yOffset = new double[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				// find max and min of data
				double max = double.MinValue;
				double min = double.MaxValue;
				for (int k = 0; k < data[i].Count; k++)
				{
					if (data[i][k].Y > max) max = data[i][k].Y;
					if (data[i][k].Y < min) min = data[i][k].Y;
				}
				if (max == min) max = min + 1;
				double diff = max - min;
				diff = getNearestPowerOfTwo(1 / diff);
				yScale[i] = diff;
				yOffset[i] = i;
			}
			YScale = yScale;
			YOffset = yOffset;
			EnableScaling = true;
			PlotWaveform();
		}

		private double getNearestPowerOfTwo(double d)
		{
			if (double.IsInfinity(d)) return d;
			if (double.IsNaN(d)) return d;
			if (d == double.MaxValue) return d;
			if (d == double.MinValue) return d;
			double s = 5;
			double n = 1;
			if (Math.Abs(d) < 1)
			{
				while (Math.Abs(n) > Math.Abs(d))
				{
					if (s == 2) s = 5; else s = 2;
					n /= s;
				}
				return n;
			}
			if (Math.Abs(d) > 1)
			{
				while (Math.Abs(n) <= Math.Abs(d))
				{
					if (s == 2) s = 5; else s = 2;
					n *= s;
				}
				return n / s;
			}
			return 1;
		}

		public void SetComplexFunctions(string functionString, NodeLabelCallback FunctionCallback, string seperator)
		{
			string[] ss = functionString.Split(new string[] { seperator }, StringSplitOptions.None);
			ICFunction[] cft = new ICFunction[ss.Length];
			for (int i = 0; i < ss.Length; i++)
			{
				cft[i] = BaseFunction.CreateNode(ss[i], FunctionCallback);
			}
			ComplexFunctions = cft;
		}
		public void PlotWaveform()
		{
			ICFunction[] cft = ComplexFunctions;
			if (cft == null) return;
			int nPlots = 0;
			foreach (ICFunction icf in cft) if (icf.Name.Equals("Plot")) nPlots++;
			double[][] x = new double[nPlots][];
			double[][] y = new double[nPlots][];
			Color[] colors = new Color[nPlots];
			ZedGraph.Symbol[] symbols = new ZedGraph.Symbol[nPlots];
			string[] labels = new string[nPlots];
			nPlots = 0;
			for (int i = 0; i < cft.Length; i++)
			{
				CDoubleArray cd = cft[i].Eval();
				if (cft[i].GetType().Name.Equals("Plot"))
				{
					Plot plot = cft[i] as Plot;
					if (cd != null)
					{
						x[nPlots] = cd.Real;
						y[nPlots] = cd.Imag;
						labels[nPlots] = plot.Legend;
						symbols[nPlots] = GraphSymbol.GetSymbol(plot.SymbolString, plot.LineColor);
						colors[nPlots] = plot.LineColor;
					}
					nPlots++;
				}
			}
			base.PlotWaveform(x, y, labels.ToArray(), colors, symbols);
		}
	}
}
