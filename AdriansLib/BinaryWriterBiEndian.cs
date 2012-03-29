using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DTALib
{
    public class BinaryWriterBiEndian : IDisposable
    {
		public enum LineEndingType {Dos, Posix};
		public LineEndingType LineEnding { get; set; }

        private BinaryWriter writer = null;
        public Stream BaseStream { get { if(writer == null) return null; return writer.BaseStream; } }

		private string filename;
		public string Filename { get { return filename; } }

        /// <summary>
        /// Get/Set endianness. Default is LittleEndian, ie, same as Windows default.
        /// </summary>
        public bool IsBigEndian { get; set; }

		public BinaryWriterBiEndian(string filename) : this(filename, false) { }
		public BinaryWriterBiEndian(string filename, bool isBigEndian)
			: this(new FileStream(filename, FileMode.Open), isBigEndian) { }
		
        public BinaryWriterBiEndian(Stream stream) : this(stream,false){}

        public BinaryWriterBiEndian(Stream stream, bool isBigEndian)
        {
			writer = new BinaryWriter(stream);
			if (stream is FileStream)
			{
				filename = (stream as FileStream).Name;
			}
			else
			{
				filename = this.ToString();
			}
			IsBigEndian = isBigEndian;
        }

        public void Write(short val)
        {
            if (IsBigEndian)
            {
                byte[] b = BitConverter.GetBytes(val);
                Array.Reverse(b);
                writer.Write(b);
            }
            else
                writer.Write(val);
        }
        public void Write(ushort val)
        {
            if (IsBigEndian)
            {
                byte[] b = BitConverter.GetBytes(val);
                Array.Reverse(b);
                writer.Write(b);
            }
            else
                writer.Write(val);
        }
        public void Write(uint val)
        {
            if (IsBigEndian)
            {
                byte[] b = BitConverter.GetBytes(val);
                Array.Reverse(b);
                writer.Write(b);
            }
            else
                writer.Write(val);
        }
        public void Write(int val)
        {
            if (IsBigEndian)
            {
                byte[] b = BitConverter.GetBytes(val);
                Array.Reverse(b);
                writer.Write(b);
            }
            else
                writer.Write(val);
        }
        public void Write(long val)
        {
            if (IsBigEndian)
            {
                byte[] b = BitConverter.GetBytes(val);
                Array.Reverse(b);
                writer.Write(b);
            }
            else
                writer.Write(val);
        }

        public void Write(double val)
        {
            if (IsBigEndian)
            {
                byte[] b = BitConverter.GetBytes(val);
                Array.Reverse(b);
                writer.Write(b);
            }
            else
                writer.Write(val);
        }
        
        public void Write(float val)
        {
            if (IsBigEndian)
            {
                byte[] b = BitConverter.GetBytes(val);
                Array.Reverse(b);
                writer.Write(b);
            }
            else
                writer.Write(val);
        }

        public void Write(byte b) { writer.Write(b); }
        public void Write(sbyte b) { writer.Write(b); }
		[Obsolete("Use WriteRange(byte) instead.")]
        public void Write(byte[] b) { writer.Write(b); }
        public void Write(char c) { writer.Write(c); }
		[Obsolete("Use WriteRange(char) instead.")]
        public void Write(char[] c) { writer.Write(c); }

		public void WriteRange(byte[] b) { writer.Write(b); }
		public void WriteRange(char[] b) { writer.Write(b); }
		public void WriteRange(int[] ir) { foreach (int i in ir) Write(i); }
		public void WriteRange(uint[] ir) { foreach (uint i in ir) Write(i); }
		public void WriteRange(short[] ir) { foreach (short i in ir) Write(i); }
		public void WriteRange(ushort[] ir) { foreach (ushort i in ir) Write(i); }
		public void WriteRange(long[] ir) { foreach (long i in ir) Write(i); }
		public void WriteRange(ulong[] ir) { foreach (ulong i in ir) Write(i); }
		public void WriteRange(float[] ir) { foreach (float i in ir) Write(i); }
		public void WriteRange(double[] ir) { foreach (double i in ir) Write(i); }

        public void Write(string s) { WriteRange(s.ToCharArray()); }
		public void WriteLine(string s)
		{
			if (LineEnding == LineEndingType.Posix) Write(s + "\n");
			else Write(s + "\r\n");
		}

        public void Dispose()
        {
            if (writer != null) writer.Dispose();
        }
    }
}
