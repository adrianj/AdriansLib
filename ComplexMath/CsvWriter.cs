using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace ComplexMath
{
	public class CsvWriter
	{
		static bool firstElementInLine = true;
		static List<bool[]> validDimensions;

		public static void ExportToCsv(CurveList curves, string filename)
		{
			validDimensions = CalculateValidDimensions(curves);
			using (StreamWriter writer = new StreamWriter(filename, false, ASCIIEncoding.ASCII))
			{
				WriteHeaders(curves, writer);
				WriteData(curves, writer);
			}
		}

		static void WriteHeaders(CurveList curves, StreamWriter writer)
		{
			firstElementInLine = true;
			for(int i = 0; i < curves.Count; i++)
			{
				WriteHeaders(curves[i], validDimensions[i], writer);
			}
			writer.WriteLine();
		}

		static void WriteHeaders(CurveItem curve, bool [] validDims, StreamWriter writer)
		{
			if (validDims[0]) WriteElement(curve.Label.Text + "_X", writer);
			if (validDims[1]) WriteElement(curve.Label.Text + "_Y", writer);
			if (validDims[2]) WriteElement(curve.Label.Text + "_Z", writer);
		}

		static void WriteData(CurveList curves, StreamWriter writer)
		{
			int maxPoints = CalculateMaxPoints(curves);
			Console.WriteLine(maxPoints);
			for (int i = 0; i < maxPoints; i++)
			{
				Console.WriteLine("line: " + i);
				firstElementInLine = true;
				WriteDataRow(curves, i, writer);
				writer.WriteLine();
			}
		}

		static void WriteDataRow(CurveList curves, int row, StreamWriter writer)
		{
			for(int i = 0; i < curves.Count; i++)
				WriteDataRow(curves[i], validDimensions[i], row, writer);
		}

		static void WriteDataRow(CurveItem curve, bool[] validDims, int row, StreamWriter writer)
		{
			if (row < curve.NPts)
			{
				if (validDims[0]) WriteDouble(curve.Points[row].X, writer);
				if (validDims[1]) WriteDouble(curve.Points[row].Y, writer);
				if (validDims[2]) WriteDouble(curve.Points[row].Z, writer);
			}
			else
			{
				if (validDims[0]) WriteElement("", writer);
				if (validDims[1]) WriteElement("", writer);
				if (validDims[2]) WriteElement("", writer);
			}
		}

		static void WriteDouble(double d, StreamWriter writer)
		{
			string s = d.ToString();
			if (double.IsNaN(d))
				s = "";
			WriteElement(s, writer);
		}

		static void WriteElement(string s, StreamWriter writer)
		{
			if (!firstElementInLine) writer.Write(','); 
			firstElementInLine = false;
			writer.Write(s);
		}

		static List<bool[]> CalculateValidDimensions(CurveList curves)
		{
			List<bool[]> ret = new List<bool[]>();
			foreach (CurveItem curve in curves)
				ret.Add(DimensionContainsNonZeroData(curve));
			return ret;
		}

		static bool[] DimensionContainsNonZeroData(CurveItem curve)
		{
			bool[] ret = new bool[3];
			for(int i = 0; i < curve.NPts; i++)
			{
				PointPair pp = curve.Points[i];
				if (pp.X != 0) ret[0] = true;
				if (pp.Y != 0) ret[1] = true;
				if (pp.Z != 0) ret[2] = true;
			}
			return ret;
		}

		static int CalculateMaxPoints(CurveList curves)
		{
			int max = 0;
			foreach (CurveItem curve in curves)
				max = Math.Max(max, curve.NPts);
			return max;
		}
	}
}
