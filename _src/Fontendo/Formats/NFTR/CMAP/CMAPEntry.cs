using Fontendo.Extensions.BinaryTools;
using System;
using System.IO;

namespace Fontendo.Formats.CTR
{
    public class CMAPEntry
    {
        public UInt16 Code; //Character code
        public UInt16 Index; //Glyph index

        private readonly BinaryReaderX? br;
        public CMAPEntry(BinaryReaderX br)
        {
            this.br = br;
        }
        public CMAPEntry(UInt16 Code, UInt16 Index)
        {
            this.Code = Code;
            this.Index = Index;
        }

        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to CMAPEntry");
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
        public void Serialize(BinaryWriterX bw)
        {
            bw.WriteUInt16(Code);
            bw.WriteUInt16(Index);
        }
    }
}