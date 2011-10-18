using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DTALib;

namespace AdriansLibClient
{
	public class TestCompressionList : ITestClass
	{
		public string RunTests(ref int failCount, ref int testCount)
		{
			string ret = "";

			List<double> list = new List<double>();
			list.AddRange(new double[6]);
			list.Add(4);
			list.AddRange(new double[5]);

			CompressionList<double> cList = new ConstantCompressionList<double>();
			cList.Compress(list);
			List<double> uList = cList.Decompress();
			if (!ListsEqual<double>(list,uList)) failCount++; testCount++;

			cList = new NoCompressionList<double>();
			cList.Compress(list);
			uList = cList.Decompress();
			if (!ListsEqual<double>(list, uList)) failCount++; testCount++;

			cList = new LinearCompressionList_double();
			cList.Compress(list);
			uList = cList.Decompress();
			if (!ListsEqual<double>(list, uList)) failCount++; testCount++;

			cList = new LinearCompressionList_double();
			list = new List<double>(new double[] { 0, 1, 2, 3, 4, 5, 6, 4, 3, 2, 6, 9, 12, 15, 18, 21, 24, 27 });
			cList.Compress(list);
			uList = cList.Decompress();
			if (!ListsEqual<double>(list, uList)) failCount++; testCount++;

			cList = CompressionFactory.GetBestCompressionOf<double>(list, 8);
			ret += "Best compression method is " + cList.GetType().Name;

			return ret;
		}

		bool ListsEqual<T>(List<T> a, List<T> b)
		{
			if (a.Count != b.Count)
				return false;
			for (int i = 0; i < a.Count; i++)
				if (!a[i].Equals(b[i]))
					return false;
			return true;
		}
	}
}
