using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DTALib
{
    public class BinaryReaderBiEndian : IDisposable
    {
		private StreamReader textReader = null;
        private BinaryReader reader = null;
        public Stream BaseStream { get { return reader.BaseStream; } }
        /// <summary>
        /// Get/Set endianness. Default is LittleEndian, ie, same as Windows default.
        /// </summary>
        public bool IsBigEndian { get; set; }

		private string filename;
		public string Filename { get { return filename; } }

		public BinaryReaderBiEndian(string filename) : this(filename, false) { }
		public BinaryReaderBiEndian(string filename, bool isBigEndian)
			: this(new FileStream(filename, FileMode.Open), isBigEndian) { }
		
        public BinaryReaderBiEndian(Stream stream) : this(stream,false){}

		public BinaryReaderBiEndian(Stream stream, bool isBigEndian)
		{
			reader = new BinaryReader(stream);
			textReader = new StreamReader(stream);
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

        public short ReadInt16()
        {
            if (IsBigEndian)
            {
                byte[] b = reader.ReadBytes(2);
				if (b.Length < 2) throw new EndOfStreamException();
				Array.Reverse(b);
				return BitConverter.ToInt16(b, 0);
            }
            return reader.ReadInt16();
        }
        public ushort ReadUInt16()
        {
            if (IsBigEndian)
            {
            byte[] b = reader.ReadBytes(2);
			if (b.Length < 2) throw new EndOfStreamException();
			Array.Reverse(b);
			return BitConverter.ToUInt16(b, 0);
            }
            return reader.ReadUInt16();
        }

		public int ReadInt32()
		{
			if (IsBigEndian)
			{
				byte[] b = reader.ReadBytes(4);
				if (b.Length < 4) throw new EndOfStreamException();
				Array.Reverse(b);
				return BitConverter.ToInt32(b, 0);
			}
			return reader.ReadInt32();
		}

        public uint ReadUInt32()
        {
			if (IsBigEndian)
			{
				byte[] b = reader.ReadBytes(4);
				if (b.Length < 4) throw new EndOfStreamException();
				Array.Reverse(b);
				return BitConverter.ToUInt32(b, 0);
			}
            return reader.ReadUInt32();
        }
        public long ReadInt64()
        {
            if (IsBigEndian)
            {
				byte[] b = reader.ReadBytes(8);
				if (b.Length < 8) throw new EndOfStreamException();
				Array.Reverse(b);
				return BitConverter.ToInt64(b, 0);
            }
            return reader.ReadInt64();
        }
        public char ReadChar() { return reader.ReadChar(); }
        public char[] ReadChars(int count) { return reader.ReadChars(count); }

        public byte ReadByte() { return reader.ReadByte(); }
        public byte[] ReadBytes(int count) { return reader.ReadBytes(count); }

        public sbyte ReadSByte() { return reader.ReadSByte(); }

        public string ReadLine()
        {
			try
			{
				string s = textReader.ReadLine();
				return s;
			}
			catch (Exception e) { Console.Write("" + e); throw; }
			/*
			try
			{
				StringBuilder sb = new StringBuilder();
				char c = ReadChar();
				while (c != '\n')
				{
					sb.Append(c);
					c = ReadChar();
				}
				sb.Replace("\r", "");
				return "" + sb;
			}
			catch (ArgumentException) { }
			return "";
			 */
        }

        public float ReadSingle()
        {
            if (IsBigEndian)
            {
                byte[] b = reader.ReadBytes(4);
                if (b.Length < 4) throw new EndOfStreamException();
                Array.Reverse(b);
                return BitConverter.ToSingle(b, 0);
            }
            return reader.ReadSingle();
        }
        public double ReadDouble()
        {
            if (IsBigEndian)
            {
                byte[] b = reader.ReadBytes(8);
                if (b.Length < 8) throw new EndOfStreamException();
                Array.Reverse(b);
                return BitConverter.ToDouble(b, 0);
            }
            return reader.ReadDouble();
        }

        public void Dispose()
        {
            if (reader != null) reader.Dispose();
        }
    }
}
