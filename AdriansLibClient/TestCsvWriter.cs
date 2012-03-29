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

			CsvWriter.ExportToCsv(cl, folder + labels[0]);
			
		}

	}
}
