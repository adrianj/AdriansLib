using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DTALib
{
    public class BinaryReaderBiEndian : IDisposable
    {
        private BinaryReader reader = null;
        public Stream BaseStream { get { return reader.BaseStream; } }
        /// <summary>
        /// Get/Set endianness. Default is LittleEndian, ie, same as Windows default.
        /// </summary>
        public bool IsBigEndian { get; set; }

        public BinaryReaderBiEndian(Stream stream)
        {
            reader = new BinaryReader(stream);
        }

        public BinaryReaderBiEndian(Stream stream, bool isBigEndian)
            : this(stream)
        { IsBigEndian = isBigEndian; }

        public short ReadInt16()
        {
            if (IsBigEndian)
            {
                byte[] b = reader.ReadBytes(2);
                if (b.Length < 2) throw new EndOfStreamException();
                return (short)(b[1] + (b[0] << 8));
            }
            return reader.ReadInt16();
        }
        public ushort ReadUInt16()
        {
            if (IsBigEndian)
            {
            byte[] b = reader.ReadBytes(2);
            if (b.Length < 2) throw new EndOfStreamException();
            return (ushort)(b[1] + (b[0] << 8));
            }
            return reader.ReadUInt16();
        }
        public uint ReadUInt32()
        {
            if (IsBigEndian)
            {
            byte[] b = reader.ReadBytes(4);
            if (b.Length < 4) throw new EndOfStreamException();
            return (uint)(b[3] + (b[2] << 8) + (b[1] << 16) + (b[0] << 24));
            }
            return reader.ReadUInt32();
        }
        public long ReadInt64()
        {
            if (IsBigEndian)
            {
                byte[] b = reader.ReadBytes(4);
                if (b.Length < 4) throw new EndOfStreamException();
                return (uint)(b[7] + (b[6] << 8) + (b[5] << 16) + (b[4] << 24)
                    +(b[3] << 32) + (b[2] << 40) + (b[1] << 48) + (b[0] << 56));
            }
            return reader.ReadInt64();
        }
        public char ReadChar() { return reader.ReadChar(); }
        public char[] ReadChars(uint count) { return reader.ReadChars((int)count); }

        public byte ReadByte() { return reader.ReadByte(); }
        public byte[] ReadBytes(uint count) { return reader.ReadBytes((int)count); }

        public sbyte ReadSByte() { return reader.ReadSByte(); }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            char c = ReadChar();
            while (c != '\n')
            {
                sb.Append(c);
                c = ReadChar();
            }
            sb.Replace("\r", "");
            return ""+sb;
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
