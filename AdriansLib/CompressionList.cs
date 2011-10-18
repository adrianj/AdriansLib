using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace DTALib
{
	public interface CompressionList<T> : ISerializable
	{
		void Compress(List<T> listToCompress);
		List<T> Decompress();
		long GetApproxSizeInBytes(int sizeOfTStructure);
	}

	public interface AdditiveValue : ISerializable
	{
		AdditiveValue Add(AdditiveValue other);
		AdditiveValue Sub(AdditiveValue other);
	}

	[Serializable]
	public class NoCompressionList<T> : CompressionList<T>
	{
		protected List<T> storage = new List<T>();

		public NoCompressionList() { }
		public NoCompressionList(SerializationInfo info, StreamingContext ctxt)
		{
			storage = (List<T>)info.GetValue("storage", typeof(List<T>));
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("storage", storage);
		}

		public virtual void Compress(List<T> listToCompress)
		{
			storage = listToCompress;
		}

		public virtual List<T> Decompress()
		{
			return storage;
		}

		public virtual long GetApproxSizeInBytes(int sizeOfTStructure)
		{
			return sizeOfTStructure * storage.Count;
		}
	}

	[Serializable]
	public class ConstantCompressionList<T> : NoCompressionList<T>
	{
		protected List<int> duplicateCounts = new List<int>();

		public ConstantCompressionList() : base() { }
		public ConstantCompressionList(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{
			duplicateCounts = (List<int>)info.GetValue("duplicateCounts", typeof(List<int>));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			base.GetObjectData(info,ctxt);
			info.AddValue("duplicateCounts", duplicateCounts);
		}

		public override void Compress(List<T> listToCompress) 
		{
			if (listToCompress == null || listToCompress.Count() < 1)
				return;
			T current = listToCompress[0];
			int count = 1;
			storage.Add(current);
			for(int i = 1; i < listToCompress.Count; i++)
			{
				T newT = listToCompress[i];
				if (newT.Equals(current))
					count++;
				else
				{
					current = newT;
					duplicateCounts.Add(count);
					count = 1;
					storage.Add(current);
				}
			}
			duplicateCounts.Add(count);
		}

		public override List<T> Decompress()
		{
			if (duplicateCounts.Count != storage.Count)
				return base.Decompress();
			List<T> ret = new List<T>();
			for (int i = 0; i < storage.Count; i++)
			{
				T[] a = GetArrayOfGivenLength(storage[i], duplicateCounts[i]);
				ret.AddRange(a);
			}
			return ret;
		}

		T[] GetArrayOfGivenLength(T value, int count)
		{
			T[] ret = new T[count];
			for (int i = 0; i < count; i++)
				ret[i] = value;
			return ret;
		}

		public override long GetApproxSizeInBytes(int sizeOfTStructure)
		{
			long extra = Marshal.SizeOf(typeof(int)) * duplicateCounts.Count;
			return extra + base.GetApproxSizeInBytes(sizeOfTStructure);
		}
	}


	[Serializable]
	public abstract class LinearCompressionList<T> : ConstantCompressionList<T>
	{
		List<T> gradients = new List<T>();
		
		public LinearCompressionList() : base() { }
		public LinearCompressionList(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{
			gradients = (List<T>)info.GetValue("gradients", typeof(List<T>));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			base.GetObjectData(info, ctxt);
			info.AddValue("gradients", gradients);
		}

		public override void Compress(List<T> listToCompress)
		{
			if (listToCompress.Count < 2)
			{
				base.Compress(listToCompress);
				return;
			}
			T previous = listToCompress[0];
			storage.Add(previous);
			T current = listToCompress[1];
			T grad = Sub(current, previous);
			previous = current;
			gradients.Add(grad);
			int count = 2;
			for (int i = 2; i < listToCompress.Count; i++)
			{
				current = listToCompress[i];
				T expected = Add(previous, grad);
				if (current.Equals(expected))
				{
					count++;
				}
				else
				{
					duplicateCounts.Add(count);
					grad = Sub(current, previous);
					gradients.Add(grad);
					count = 1;
					storage.Add(current);
				}
				previous = current;
			}
			duplicateCounts.Add(count);
			if (gradients.Count < duplicateCounts.Count)
				gradients.Add(grad);

		}

		public override List<T> Decompress()
		{
			if(gradients.Count != duplicateCounts.Count)
				return base.Decompress(); 
			List<T> ret = new List<T>();
			for (int i = 0; i < storage.Count; i++)
			{
				T[] a = GetArraySegment(storage[i], duplicateCounts[i], gradients[i]);
				ret.AddRange(a);
			}
			return ret;
		}

		T[] GetArraySegment(T startValue, int count, T grad)
		{
			T[] ret = new T[count];
			ret[0] = startValue;
			for (int i = 1; i < count; i++)
				ret[i] = Add(ret[i-1], grad);
			return ret;
		}

		public override long GetApproxSizeInBytes(int sizeOfTStructure)
		{
			long extra = sizeOfTStructure * gradients.Count;
			return extra + base.GetApproxSizeInBytes(sizeOfTStructure);
		}

		protected abstract T Add(T a, T b);
		protected abstract T Sub(T a, T b);
	}

	[Serializable]
	public class LinearCompressionList_double : LinearCompressionList<double>
	{
		public LinearCompressionList_double() { }
		public LinearCompressionList_double(SerializationInfo info, StreamingContext ctxt) :base(info,ctxt) { }
		protected override double Add(double a, double b) { return a + b; }
		protected override double Sub(double a, double b) { return a - b; } 
	}
	[Serializable]
	public class LinearCompressionList_int : LinearCompressionList<int>
	{
		public LinearCompressionList_int() { }
		public LinearCompressionList_int(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
		protected override int Add(int a, int b) { return a + b; }
		protected override int Sub(int a, int b) { return a - b; }
	}
	[Serializable]
	public class LinearCompressionList_long : LinearCompressionList<long>
	{
		public LinearCompressionList_long() { }
		public LinearCompressionList_long(SerializationInfo info, StreamingContext ctxt) :base(info,ctxt) { }
		protected override long Add(long a, long b) { return a + b; }
		protected override long Sub(long a, long b) { return a - b; }
	}
	[Serializable]
	public class LinearCompressionList_value : LinearCompressionList<AdditiveValue>
	{
		public LinearCompressionList_value() { }
		public LinearCompressionList_value(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
		protected override AdditiveValue Add(AdditiveValue a, AdditiveValue b) { return a.Add(b); }
		protected override AdditiveValue Sub(AdditiveValue a, AdditiveValue b) { return a.Sub(b); }
	}
}
