using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions.BinaryTools
{
    public class BinaryReaderX : BinaryReader
    {
        public bool FlipBytes { get; set; } = false;

        // Pass-through constructors
        public BinaryReaderX(Stream input) : base(input) { }
        public BinaryReaderX(Stream input, Encoding encoding) : base(input, encoding) { }
        public BinaryReaderX(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

        public void SetEndianness(int offset = 0x4)
        {
            bool isLE = ReadEndiannessByte(offset);
            FlipBytes = Endianness.IsSystemLittleEndian() != isLE;
        }
        public Endianness.Endian GetEndianness()
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
        public bool ReadEndiannessByte(int offset)
        {
            BaseStream.Position = offset;
            ushort bom = ReadByte();
            bool isLE;
            if (bom == 0xFE) isLE = false;
            else if (bom == 0xFF) isLE = true;
            else throw new Exception($"Error: Unknown endianness byte at 0x{offset}!");
            BaseStream.Position = 0;
            return isLE;
        }

        public override ushort ReadUInt16()
        {
            {
                // Read two bytes
                byte[] temp = base.ReadBytes(2);
                if (temp.Length < 2)
                    throw new EndOfStreamException("Unexpected end of stream while reading UInt16");

                if (!FlipBytes)
                {
                    // Default little-endian interpretation
                    return BitConverter.ToUInt16(temp, 0);
                }
                else
                {
                    // Manual big-endian flip
                    return (ushort)(temp[0] << 8 | temp[1]);
                }
            }
        }


        public override uint ReadUInt32()
        {
            // Read four bytes
            byte[] temp = base.ReadBytes(4);
            if (temp.Length < 4)
                throw new EndOfStreamException("Unexpected end of stream while reading UInt32");

            if (!FlipBytes)
            {
                // Default little-endian interpretation
                return BitConverter.ToUInt32(temp, 0);
            }
            else
            {
                // Manual big-endian flip
                return (uint)(temp[0] << 24 | temp[1] << 16 | temp[2] << 8 | temp[3]);
            }
        }
    }
}
