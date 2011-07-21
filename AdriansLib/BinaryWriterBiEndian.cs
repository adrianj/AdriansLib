using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DTALib
{
    public class BinaryWriterBiEndian : IDisposable
    {
        private BinaryWriter writer = null;
        public Stream BaseStream { get { if(writer == null) return null; return writer.BaseStream; } }

        /// <summary>
        /// Get/Set endianness. Default is LittleEndian, ie, same as Windows default.
        /// </summary>
        public bool IsBigEndian { get; set; }
        
        public BinaryWriterBiEndian(Stream stream)
        {
            writer = new BinaryWriter(stream);
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
        public void Write(byte[] b) { writer.Write(b); }
        public void Write(char c) { writer.Write(c); }
        public void Write(char[] c) { writer.Write(c); }

        public void Write(string s) { Write(s.ToCharArray()); }
        public void WriteLine(string s) { Write(s+"\r\n"); }

        public void Dispose()
        {
            if (writer != null) writer.Dispose();
        }
    }
}
