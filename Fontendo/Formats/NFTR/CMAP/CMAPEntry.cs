public class CMAPEntry
{
    public UInt16 code; //Character code
    public UInt16 index; //Glyph index

    public CMAPEntry()
    {

    }
    public CMAPEntry(UInt16 code, UInt16 index)
    {
        this.code = code;
        this.index = index;
    }

    public ActionResult Parse(BinaryReader br)
    {
        try
        {
            code = br.ReadUInt16();
            index = br.ReadUInt16();
        }
        catch (Exception e)
        {
            return new ActionResult(false, $"CMAPEntry exception {e.Message}");
        }
        return new ActionResult(true, "OK");
    }
}