namespace Fontendo.Formats.CTR
{
    public class CMAP
    {
        private UInt32 EntriesOffset;

        public UInt32 Magic; //Should always be 0x434D4150, CMAP in ASCII
        public UInt32 Length; //CMAP section length in bytes
        public UInt16 CodeBegin; //First character code
        public UInt16 CodeEnd; //Last character code
        public UInt16 MappingMethod; //Mapping method, 0x0 - Direct, 0x1 - Table, 0x2 - Scan
        public UInt16 Reserved; //Reserved (this is referred to it as so even in the official SDK software)
        public UInt32 PtrNext; //Pointer to the next CMAP(next CMAP offset + 0x8), 0x0 if last
        public List<CMAPEntry> entries = new List<CMAPEntry>();

        public ActionResult Parse(BinaryReader br)
        {
            try
            {
                Magic = br.ReadUInt32();
                Length = br.ReadUInt32();
                CodeBegin = br.ReadUInt16();
                CodeEnd = br.ReadUInt16();
                MappingMethod = br.ReadUInt16();
                Reserved = br.ReadUInt16();
                PtrNext = br.ReadUInt32();
                EntriesOffset = (UInt32)br.BaseStream.Position;

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
            if (Magic != 0x434D4150U && Magic != 0x50414D43)
                return false;
            return true;
        }

        private ActionResult ReadEntries(BinaryReader br)
        {
            entries = new List<CMAPEntry>();
            if (EntriesOffset == 0)
                return new ActionResult(false, "CMAP Entry Offset is zero");
            br.BaseStream.Position = EntriesOffset;
            UInt16 temp = 0;
            switch (MappingMethod)
            {
                case 0: //Direct
                    temp = br.ReadUInt16(); //indexOffset
                    for (UInt16 i = CodeBegin; i <= CodeEnd; i++)
                    {
                        entries.Add(new CMAPEntry(i, temp));
                        temp++;
                    }
                    break;
                case 1: //Table
                    for (UInt16 i = CodeBegin; i <= CodeEnd; i++)
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