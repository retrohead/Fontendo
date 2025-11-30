namespace Fontendo.Formats.NFTR
{
    public class CMAP
    {
        private UInt32 entriesOffset;

        public UInt32 magic; //Should always be 0x434D4150, CMAP in ASCII
        public UInt32 length; //CMAP section length in bytes
        public UInt16 codeBegin; //First character code
        public UInt16 codeEnd; //Last character code
        public UInt16 mappingMethod; //Mapping method, 0x0 - Direct, 0x1 - Table, 0x2 - Scan
        public UInt16 reserved; //Reserved (this is referred to it as so even in the official SDK software)
        public UInt32 ptrNext; //Pointer to the next CMAP(next CMAP offset + 0x8), 0x0 if last
        public List<CMAPEntry> entries = new List<CMAPEntry>();

        public ActionResult Parse(BinaryReader br)
        {
            try
            {
                magic = br.ReadUInt32();
                length = br.ReadUInt32();
                codeBegin = br.ReadUInt16();
                codeEnd = br.ReadUInt16();
                mappingMethod = br.ReadUInt16();
                reserved = br.ReadUInt16();
                ptrNext = br.ReadUInt32();
                entriesOffset = (UInt32)br.BaseStream.Position;

                // read entries
                ActionResult result = ReadEntries(br);
                if (!result.Success)
                    return result;
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CMAP exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid CMAP signature!!!");

            return new ActionResult(true, "OK");
        }

        private bool ValidateSignature()
        {
            if (magic != 0x434D4150U && magic != 0x50414D43)
                return false;
            return true;
        }

        private ActionResult ReadEntries(BinaryReader br)
        {
            entries = new List<CMAPEntry>();
            if (entriesOffset == 0)
                return new ActionResult(false, "CMAP Entry Offset is zero");
            br.BaseStream.Position = entriesOffset;
            UInt16 temp = 0;
            switch (mappingMethod)
            {
                case 0: //Direct
                    temp = br.ReadUInt16(); //indexOffset
                    for (UInt16 i = codeBegin; i <= codeEnd; i++)
                    {
                        entries.Add(new CMAPEntry(i, temp));
                        temp++;
                    }
                    break;
                case 1: //Table
                    for (UInt16 i = codeBegin; i <= codeEnd; i++)
                    {
                        temp = br.ReadUInt16(); //index
                        if (temp != 0xFFFFU) entries.Add(new CMAPEntry(i, temp));
                    }
                    break;
                case 2: //Scan
                    temp = br.ReadUInt16(); //count
                    for (UInt16 i = 0; i < temp; i++)
                    {
                        CMAPEntry entry = new CMAPEntry();
                        ActionResult result = entry.Parse(br);
                        if (!result.Success)
                            return new ActionResult(false, $"Failed reading CMAP Entry at index {i}\n\n{result.Message}");
                        entries.Add(entry);
                    }
                    break;
            }
            return new ActionResult(true, "OK");
        }
    }
}