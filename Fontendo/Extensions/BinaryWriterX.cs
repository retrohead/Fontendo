using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions
{

    public class BinaryWriterX : IDisposable
    {
        private readonly BinaryWriter bw;
        public bool FlipBytes { get; set; } = false;
        public Stream BaseStream 
        { 
            get 
            { 
                return bw.BaseStream; 
            }
        }

        public BinaryWriterX(string path, bool isLittleEndian)
        {
            bw = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None));
            SetEndianness(isLittleEndian);
        }

        public void SetEndianness(bool isLE)
        {
            FlipBytes = (Endianness.IsSystemLittleEndian() != isLE);
        }
        public Endianness.Endian Endian()
        {
            if (Endianness.IsSystemLittleEndian())
            {
                if (!FlipBytes) return Endianness.Endian.Little;
                else return Endianness.Endian.Big;
            }
            else
            {
                if (!FlipBytes) return Endianness.Endian.Big;
                else return Endianness.Endian.Little;
            }
        }

        public long GetPosition() => bw.BaseStream.Position;

        public void SetPosition(long pos) => bw.BaseStream.Seek(pos, SeekOrigin.Begin);

        public void Write(byte value) => bw.Write(value);

        public void Write(sbyte value) => bw.Write(value);

        public void Write(byte[] buffer, int index, int count) => bw.Write(buffer, index, count);

        public void Write(float value) => Write(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0));

        public void Write(double value) => Write(BitConverter.ToUInt64(BitConverter.GetBytes(value), 0));

        public void Write(short value) => Write((ushort)value);

        public void Write(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (FlipBytes) Array.Reverse(bytes);
            bw.Write(bytes);
        }

        public void Write(int value) => Write((uint)value);

        public void Write(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (FlipBytes) Array.Reverse(bytes);
            bw.Write(bytes);
        }

        public void Write(long value) => Write((ulong)value);

        public void Write(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (FlipBytes) Array.Reverse(bytes);
            bw.Write(bytes);
        }

        public void Write(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            bw.Write(bytes);
        }

        public void Dispose()
        {
            bw?.Dispose();
        }
    }
}
