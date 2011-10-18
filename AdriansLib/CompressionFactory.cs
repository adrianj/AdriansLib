using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DTALib
{
	public enum CompressionType { NoCompression, ConstantCompression, LinearCompression };

	public class CompressionFactory
	{
		public static CompressionList<T> GetCompressionClass<T>(CompressionType type)
		{
			if (type == CompressionType.NoCompression)
				return new NoCompressionList<T>();
			if (type == CompressionType.ConstantCompression)
				return new ConstantCompressionList<T>();
			if (type == CompressionType.LinearCompression)
			{
				if(typeof(T) == typeof(int)) return (CompressionList<T>)new LinearCompressionList_int();
				if (typeof(T) == typeof(double)) return (CompressionList<T>)new LinearCompressionList_double();
				if (typeof(T) == typeof(long)) return (CompressionList<T>)new LinearCompressionList_long();
				if (typeof(T).GetInterfaces().Contains(typeof(AdditiveValue))) return (CompressionList<T>)new LinearCompressionList_value();
			}
			return new NoCompressionList<T>();
		}

		public static CompressionList<T> GetBestCompressionOf<T>(List<T> listToCompress, int sizeOfTStructure)
		{
			CompressionList<T> best = GetCompressionClass<T>(CompressionType.NoCompression);
			best.Compress(listToCompress);

			CompressionList<T> contender1 = GetCompressionClass<T>(CompressionType.ConstantCompression);
			contender1.Compress(listToCompress);

			if (contender1.GetApproxSizeInBytes(sizeOfTStructure) < best.GetApproxSizeInBytes(sizeOfTStructure))
				best = contender1;

			CompressionList<T> contender2 = GetCompressionClass<T>(CompressionType.LinearCompression);
			contender2.Compress(listToCompress);

			if (contender2.GetApproxSizeInBytes(sizeOfTStructure) < best.GetApproxSizeInBytes(sizeOfTStructure))
				best = contender2;

			//Console.WriteLine("Best compression method: " + best.GetType().Name + ", "
			//	+ (sizeOfTStructure * listToCompress.Count) + " to " + best.GetApproxSizeInBytes(sizeOfTStructure));

			return best;

		}
	}
}
