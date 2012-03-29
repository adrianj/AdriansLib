using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.IO;
using ZedGraph;

namespace ComplexMath
{
	public class DataExporter
	{
		public static Dictionary<string, Action<CurveList, string>> ExportMethods = new Dictionary<string, Action<CurveList, string>>()
		{
			{".csv",CsvWriter.ExportToCsv}

		};

		public static string[] SupportedFileExtensions { get { return ExportMethods.Keys.ToArray(); } }

		static string CreateFilter()
		{
			if(SupportedFileExtensions == null || SupportedFileExtensions.Length < 1)
				return "*.*|*.*";
			string ret = "*"+SupportedFileExtensions[0] + "|*"+SupportedFileExtensions[0];
			for (int i = 1; i < SupportedFileExtensions.Length; i++)
				ret += "|*" + SupportedFileExtensions[i] + "|*" + SupportedFileExtensions[i];
			return ret;
		}

		public static void QueryUserAndSaveAs(CurveList curves)
		{
			if (curves.Count < 1)
			{
				MessageBox.Show("No data to save!");
				return;
			}
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = CreateFilter();
			if (sfd.ShowDialog() == DialogResult.OK)
				SaveDataAs(curves, sfd.FileName);
		}

		public static void SaveDataAs(CurveList curves, string filename)
		{
			string ext = Path.GetExtension(filename).ToLower();
			if (!ExportMethods.ContainsKey(ext))
			{
				MessageBox.Show("Cannot write to file format: " + ext);
				return;
			}
			ExportMethods[ext](curves, filename);
		}
	}
}
