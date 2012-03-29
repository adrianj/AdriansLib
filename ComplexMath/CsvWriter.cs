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
		public static void ExportToCsv(CurveList curves, string filename)
		{
			using (StreamWriter writer = new StreamWriter(filename, false, ASCIIEncoding.ASCII))
			{
				WriteHeaders(curves, writer);
				WriteData(curves, writer);
			}
		}

		static void WriteHeaders(CurveList curves, StreamWriter writer)
		{

		}

		static void WriteData(CurveList curves, StreamWriter writer)
		{

		}
	}
}
