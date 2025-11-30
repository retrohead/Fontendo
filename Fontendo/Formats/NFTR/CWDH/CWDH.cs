namespace Fontendo.Formats.CTR
{
    public class CWDH
    {
        private UInt32 EntriesOffset; //Used to keep track of where to read the entries from

        public UInt32 Magic; //Should always be 0x43574448, CWDH in ASCII
        public UInt32 Length; //CWDH section length in bytes
        public UInt16 IndexBegin; //First glyph index
        public UInt16 IndexEnd; //Last glyph index
        public UInt32 PtrNext; //Pointer to the next (next CWDH + 0x8), 0x0 if last
        public List<CharWidths> Entries = new List<CharWidths>(); //CharWidths entries are stored after the header

        public ActionResult Parse(BinaryReader br)
        {
            try
            {
                Magic = br.ReadUInt32();
                Length = br.ReadUInt32();
                IndexBegin = br.ReadUInt16();
                IndexEnd = br.ReadUInt16();
                PtrNext = br.ReadUInt32();
                EntriesOffset = (UInt32)br.BaseStream.Position;
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CWDH exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid CWDH signature!!!");

            // read entries
            ActionResult result = ReadEntries(br);
            if (!result.Success)
                return result;

            return new ActionResult(true, "OK");
        }

        private bool ValidateSignature()
        {
            if (Magic != 0x43574448U && Magic != 0x48445743U)
                return false;
            return true;
        }

        private ActionResult ReadEntries(BinaryReader br)
        {
            Entries = new List<CharWidths>();
            if (EntriesOffset == 0U)
            {
                // TODO: 
                // not really sure what the original author meant by this next line
                // presuming this is an error or meant for new file fornow
                //return; //The case if the first contructor was chosen
                return new ActionResult(false, "Entry offset is zero");
            }
            br.BaseStream.Position = EntriesOffset;
            //Read all the entries
            for (int i = IndexBegin; i <= IndexEnd; i++)
            {
                CharWidths charwidth = new CharWidths();
                ActionResult result = charwidth.Parse(br);
                if (!result.Success)
                    return new ActionResult(false, $"Failed parsing CWDH charwidth at index {i}\n\n{result.Message}");
                Entries.Add(charwidth);
            }
            return new ActionResult(true, "OK");
        }
    }
}