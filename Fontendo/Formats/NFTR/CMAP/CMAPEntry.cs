namespace Fontendo.Formats.CTR
{
    public class CMAPEntry
    {
        public UInt16 Code; //Character code
        public UInt16 Index; //Glyph index

        public CMAPEntry()
        {

        }
        public CMAPEntry(UInt16 Code, UInt16 Index)
        {
            this.Code = Code;
            this.Index = Index;
        }

        public ActionResult Parse(BinaryReader br)
        {
            try
            {
                Code = br.ReadUInt16();
                Index = br.ReadUInt16();
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CMAPEntry exception {e.Message}");
            }
            return new ActionResult(true, "OK");
        }
    }
}