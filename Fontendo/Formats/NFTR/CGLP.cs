namespace Fontendo.Formats.CTR
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

        public ActionResult Parse(BinaryReader br)
        {
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
    }
}