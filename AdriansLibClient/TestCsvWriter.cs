using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Text;
using NUnit.Framework;
using ComplexMath;
using ZedGraph;

namespace AdriansLibClient
{
	[TestFixture]
	public class TestCsvWriter
	{
		string folder = @"C:\Milds\testing\";

		[Test]
		public void TestEasyData()
		{
			
			string [] labels = {"basic1","basic2"};
			double[] x1 = { 4, 5, 6 };
			double[] y1 = { 12, 15, 18 };
			double[] x2 = { -2, -3, -4 };
			double[] y2 = { 0.01, 0.1, 1 };
			CurveList cl = new CurveList();
			cl.Add(new LineItem(labels[0], x1, y1, Color.Black, SymbolType.Circle));
			cl.Add(new LineItem(labels[1], x2, y2, Color.Black, SymbolType.Circle));
			string filename = folder + labels[0]+".csv";
			Console.WriteLine("Writing for file: " + filename);
			CsvWriter.ExportToCsv(cl, filename);
			
		}


		[Test]
		public void TestComplexData()
		{

			string[] labels = { "complex1", "complex2","complex3" };
			double[] x1 = { 4e12, 5e13, 6e14 };
			double[] y1 = { 12e-10, 15e-10, 18e-10 };
			double[] x2 = { -2, -3, -4, -5, -6, -7, -8, -9, -10, -11 };
			double[] y2 = { 0.001, 0.01, 0.1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
			double[] z2 = { 1.23453456345345345, 2.6450985347635, 3.45237895638925, 4.42378962387534, 5.4532498734563, 
							  6.429874343, 7.5349857345, 8.5347637634, 9.3764684593453, 10.53948756876345 };
			double[] x3 = { 1, 2, 3, 4, 5, 6, 7, 8 };
			double[] y3 = new double[x3.Length];
			CurveList cl = new CurveList();
			cl.Add(new LineItem(labels[0], x1, y1, Color.Black, SymbolType.Circle));
			PointPairList ppl = new PointPairList();
			for(int i = 0; i < x2.Length; i++)
				ppl.Add(x2[i],y2[i],z2[i]);
			cl.Add(new LineItem(labels[1], ppl, Color.Black, SymbolType.Circle));
			cl.Add(new LineItem(labels[2], x3, y3, Color.Black, SymbolType.Circle));
			string filename = folder + labels[0] + ".csv";
			Console.WriteLine("Writing for file: " + filename);
			CsvWriter.ExportToCsv(cl, filename);

		}
	}
}
