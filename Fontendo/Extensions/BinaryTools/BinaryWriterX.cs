using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions.BinaryTools
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
            FlipBytes = Endianness.IsSystemLittleEndian() != isLE;
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

        public void WriteByte(byte value) => bw.Write(value);

        public void WriteSbyte(sbyte value) => bw.Write(value);

        public void WriteBytes(byte[] buffer, int index, int count) => bw.Write(buffer, index, count);

        public void WriteFloat(float value) => bw.Write(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0));

        public void WriteDouble(double value) => bw.Write(BitConverter.ToUInt64(BitConverter.GetBytes(value), 0));

        public void WriteShort(short value) => bw.Write((ushort)value);

        public void WriteUshort(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (FlipBytes) Array.Reverse(bytes);
            bw.Write(bytes);
        }

        public void WriteUInt16(UInt16 value) => bw.Write((UInt16)value);

        public void WriteUInt32(UInt32 value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (FlipBytes) Array.Reverse(bytes);
            bw.Write(bytes);
        }

        public void WriteLong(long value) => WriteULong((ulong)value);

        public void WriteULong(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (FlipBytes) Array.Reverse(bytes);
            bw.Write(bytes);
        }

        public void WriteString(string value)
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
