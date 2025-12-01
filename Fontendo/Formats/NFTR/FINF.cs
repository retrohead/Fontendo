using Fontendo.Extensions.BinaryTools;
using System;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Forms;

namespace Fontendo.Formats.CTR
{
    public class FINF
    {
        public UInt32 Magic; //Should always be 0x46494E46, FINF in ASCII
        public UInt32 Length; //FINF section length in ushorts
        public byte FontType; //Font type, 0x0 - Bitmap, 0x1 - TGLP
        public byte LineFeed; //Line feed, == leading
        public UInt16 DefaultCharIndex; //Index of default character, used for when a program requests a character that doesn't exist in the font
        public CharWidths? DefaultWidths; //Default character widths, ? fallback for characters that don't have width information
        public byte Encoding; //Font encoding, 0x0 - UTF-8, 0x1 - UTF-16, 0x2 - ShiftJIS, 0x3 - CP1252
        public UInt32 PtrGlyph; //TGLP section data pointer (TGLP + 0x8)
        public UInt32 PtrWidth; //CWDH section data pionter (CWDH + 0x8)
        public UInt32 PtrMap; //CMAP section data pointer (CMAP + 0x8)
        public byte Height; //Font height, v1.2+ and all later revisions only
        public byte Width; //Font width, v1.2+ and all later revisions only
        public byte Ascent; //Ascent, v1.2+ and all later revisions only
        public byte Padding; //Padding ushort, v1.2+ and all later revisions only

        private readonly BinaryReaderX? br;
        public FINF(BinaryReaderX br)
        {
            this.br = br;
        }
        public FINF(UInt32 Length, byte FontType, byte LineFeed, UInt16 DefaultCharIndex, CharWidths DefaultWidths,
             byte Encoding, byte Height = 0xFF, byte Width = 0xFF, byte Ascent = 0xFF, UInt32 Magic = 0x46494E46U)
        {

            this.Magic = Magic;
            this.Length = Length;
            this.FontType = FontType;
            this.LineFeed = LineFeed;
            this.DefaultCharIndex = DefaultCharIndex;
            this.DefaultWidths = DefaultWidths;
            this.Encoding = Encoding;
            this.PtrGlyph = 0x0U;
            this.PtrWidth = 0x0U;
            this.PtrMap = 0x0U;
            this.Height = Height;
            this.Width = Width;
            this.Ascent = Ascent;
            this.Padding = 0;
        }


        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to FINF");
            try
            {
                Magic = br.ReadUInt32();
                Length = br.ReadUInt32();
                FontType = br.ReadByte();
                LineFeed = br.ReadByte();
                DefaultCharIndex = br.ReadUInt16();
                DefaultWidths = new CharWidths(br);
                ActionResult cwhdresult = DefaultWidths.Parse();
                if (!cwhdresult.Success)
                    return new ActionResult(false, $"Invalid FINF charwidths {cwhdresult.Message}");

                Encoding = br.ReadByte();
                PtrGlyph = br.ReadUInt32();
                PtrWidth = br.ReadUInt32();
                PtrMap = br.ReadUInt32();
                if (Length == 0x20U)
                {
                    Height = br.ReadByte();
                    Width = br.ReadByte();
                    Ascent = br.ReadByte();
                    Padding = br.ReadByte();
                }
                else if (Length == 0x1CU)  //Old version has a size of 0x1C, doesn't have the four bytes below
                {
                    Height = 0xFF;
                    Width = 0xFF;
                    Ascent = 0xFF;
                    Padding = 0xFF;
                }
                else
                {
                    return new ActionResult(false, $"Invalid FINF section length 0x{Length.ToString("X8")}");
                }
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"FINF exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid FINF signature!!!");
            return new ActionResult(true, "OK");
        }

        private bool ValidateSignature()
        {
            if (Magic != 0x46494E46U && Magic != 0x464E4946U)
                return false;
            return true;
        }

        /// <summary>
        /// Serialize FINF header into the binary stream, recording patch addresses in the linker.
        /// </summary>
        public void Serialize(BinaryWriterX bw, BlockLinker linker)
        {
            // Increment lookup values
            linker.IncLookupValue("ptrInfo", bw.BaseStream.Position);
            linker.IncLookupValue("blockCount", 1);

            // Write header fields
            bw.WriteUInt32(Magic);
            bw.WriteUInt32(Length);
            bw.WriteByte(FontType);
            bw.WriteByte(LineFeed);
            bw.WriteUInt16(DefaultCharIndex);

            // Serialize default widths
            DefaultWidths?.Serialize(bw);

            bw.WriteByte(Encoding);

            // Patch pointers
            linker.AddPatchAddr((uint)bw.BaseStream.Position, "ptrGlyph");
            bw.WriteUInt32(PtrGlyph);

            linker.AddPatchAddr((uint)bw.BaseStream.Position, "ptrWidth");
            bw.WriteUInt32(PtrWidth);

            linker.AddPatchAddr((uint)bw.BaseStream.Position, "ptrMap");
            bw.WriteUInt32(PtrMap);

            // Extra fields if length == 0x20
            if (Length == 0x20U)
            {
                bw.WriteByte(Height);
                bw.WriteByte(Width);
                bw.WriteByte(Ascent);
                bw.WriteByte((byte)0x0);
            }
        }

    }
}