using Fontendo.Extensions.BinaryTools;
using System;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using static Fontendo.Extensions.FontBase;

namespace Fontendo.Formats
{
    public class CGLP
    {
        public UInt32 BitmapPtr; //Here just for keeping track of where the image data is located, not actually present in the file
        public UInt32 Magic; //Section magic, should always be 0x43474C50, CGLP in ASCII
        public UInt32 Length; //Length of the section including bitmap data itself in bytes
        public byte CellWidth; //Glyph cell width, in pixels
        public byte CellHeight; //Glyph cell height, in pixels
        public UInt16 CellSize; //IDK even, maybe cell size in bytes?
        public sbyte BaselinePos; //Baseline position
        public byte MaxCharWidth; //Maximum character width
        public byte BytesPerPixel; //Bitmap Bits Per Pixel
        public byte Flags; //Flags, see the enum below. Called "reserved" since RVL SDK
        private readonly BinaryReaderX? br;


        //Enums
        public enum fontGlyphFlags : byte
        {
            verticalWriting = (1 << 0),
            rot0Deg = (0 << 1),
            rot90Deg = (1 << 1),
            rot180Deg = (2 << 1),
            rot270Deg = (3 << 1),
            rotMask = (3 << 1)
        };

        public CGLP(BinaryReaderX br, bool isNewVersion)
        {
            this.br = br;
        }
        public CGLP(byte CellWidth, byte CellHeight, UInt16 CellSize, sbyte BaselinePos, byte MaxCharWidth, byte BytesPerPixel, byte Flags)
        {
            this.Magic = 0x43474C50U;
            this.CellWidth = CellWidth;
            this.CellHeight = CellHeight;
            this.CellSize = CellSize;
            this.BaselinePos = BaselinePos;
            this.MaxCharWidth = MaxCharWidth;
            this.BytesPerPixel = BytesPerPixel;
            this.Flags = Flags;
        }

        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to CGLP");
            try
            {
                Magic = br.ReadUInt32();
                Length = br.ReadUInt32();
                CellWidth = br.ReadByte();
                CellHeight = br.ReadByte();
                CellSize = br.ReadUInt16();
                BaselinePos = br.ReadSByte();
                MaxCharWidth = br.ReadByte();
                BytesPerPixel = br.ReadByte();
                Flags = br.ReadByte();
                BitmapPtr = (UInt32)br.BaseStream.Position;
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CGLP exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid CGLP signature!!!");
            return new ActionResult(true, "OK");
        }

        private bool ValidateSignature()
        {
            if (Magic != 0x43474C50U && Magic != 0x504C4743U)
                return false;
            return true;
        }

        /// <summary>
        /// Serialize CGLP block into the binary stream, recording patch addresses in the linker.
        /// </summary>
        public void Serialize(BinaryWriterX bw, BlockLinker linker, List<byte[]> images)
        {
            // Increment block count
            linker.IncLookupValue(FontPointerType.blockCount, 1);

            // Write header
            bw.WriteUInt32(Magic);

            // Patch glyph length
            linker.AddPatchAddr((uint)bw.BaseStream.Position, FontPointerType.glyphLength);
            bw.WriteUInt32(Length);

            // Record glyph pointer
            uint ptr = (uint)bw.BaseStream.Position;
            linker.AddLookupValue(FontPointerType.ptrGlyph, ptr);

            bw.WriteByte(CellWidth);
            bw.WriteByte(CellHeight);
            bw.WriteUInt16(CellSize);
            bw.WriteSbyte(BaselinePos);
            bw.WriteByte(MaxCharWidth);
            bw.WriteByte(BytesPerPixel);
            bw.WriteByte(Flags);

            // Image data
            foreach (var img in images)
            {
                for (uint i = 0; i < CellSize; i++)
                {
                    bw.WriteByte(img[i]);
                }
            }

            // Padding to 4-byte boundary
            uint padBytes = 4U - ((uint)bw.BaseStream.Position % 4U);
            if (padBytes != 4U)
            {
                for (uint i = 0; i < padBytes; i++)
                    bw.WriteByte((byte)0x0);
            }

            // Update glyph length
            linker.AddLookupValue(FontPointerType.glyphLength, (uint)bw.BaseStream.Position - ptr + 0x8U);
        }
    }
}