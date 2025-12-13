using Fontendo;
using Fontendo.Extensions.BinaryTools;
using System;
using System.Runtime.CompilerServices;
using static Fontendo.Extensions.FontBase;

public class TGLP
{
    public UInt32 Magic; //Should always be 0x54474C50, TGLP in ASCII
    public UInt32 Length; //TGLP section length in bytes
    public byte CellWidth; //Glyph cell width, in pixels
    public byte CellHeight; //Glyph cell height, in pixels
    public byte BaselinePos; //Baseline position
    public byte MaxCharWidth; //Maximum character width
    public UInt32 SheetSize; //Single sheet size in bytes
    public UInt16 SheetCount; //Sheet count
    public UInt16 SheetFormat; //Sheet image format
    public UInt16 CellsPerRow; //Glyph cells per row
    public UInt16 CellsPerColumn; //Glyph cells per column
    public UInt16 SheetWidth; //Sheet width, in pixels
    public UInt16 SheetHeight; //Sheet height, in pixels
    public UInt32 SheetPtr; //Sheet data pointer

    private readonly BinaryReaderX? br;

    public TGLP(BinaryReaderX br)
    {
        this.br = br;
    }

    public TGLP(byte CellWidth, byte CellHeight, byte BaselinePos,
        byte MaxCharWidth, UInt32 SheetSize, UInt16 SheetCount, UInt16 SheetFormat,
        UInt16 CellsPerRow, UInt16 CellsPerColumn, UInt16 SheetWidth, UInt16 SheetHeight, UInt32 Magic = 0x54474C50U, UInt32 Length = 0x0U,
        UInt32 SheetPtr = 0x0U)
    {
        this.Magic = Magic;
        this.Length = Length;
        this.CellWidth = CellWidth;
        this.CellHeight = CellHeight;
        this.BaselinePos = BaselinePos;
        this.MaxCharWidth = MaxCharWidth;
        this.SheetSize = SheetSize;
        this.SheetCount = SheetCount;
        this.SheetFormat = SheetFormat;
        this.CellsPerRow = CellsPerRow;
        this.CellsPerColumn = CellsPerColumn;
        this.SheetWidth = SheetWidth;
        this.SheetHeight = SheetHeight;
        this.SheetPtr = SheetPtr;
    }

    public ActionResult Parse()
    {
        if (br == null)
            return new ActionResult(false, "Binary reader not attached to TGLP");
        try
        {
            Magic = br.ReadUInt32();
            Length = br.ReadUInt32();
            CellWidth = br.ReadByte();
            CellHeight = br.ReadByte();
            BaselinePos = br.ReadByte();
            MaxCharWidth = br.ReadByte();
            SheetSize = br.ReadUInt32();
            SheetCount = br.ReadUInt16();
            SheetFormat = br.ReadUInt16();
            CellsPerRow = br.ReadUInt16();
            CellsPerColumn = br.ReadUInt16();
            SheetWidth = br.ReadUInt16();
            SheetHeight = br.ReadUInt16();
            SheetPtr = br.ReadUInt32();
        }
        catch (Exception e)
        {
            return new ActionResult(false, $"TGLP exception {e.Message}");
        }
        if (!ValidateSignature())
            return new ActionResult(false, "Invalid TGLP signature!!!");
        return new ActionResult(true, "OK");
    }

    private bool ValidateSignature()
    {
        if (Magic != 0x54474C50U && Magic != 0x504C4754U)
            return false;
        return true;
    }
    /// <summary>
    /// Serialize TGLP block into the binary stream, recording patch addresses in the linker.
    /// </summary>
    public void Serialize(BinaryWriterX bw, BlockLinker linker, List<byte[]> sheets, uint align)
    {
        // Increment block count
        linker.IncLookupValue(FontPointerType.blockCount, 1);

        // Write header
        bw.WriteUInt32(Magic);

        // Patch glyph length
        linker.AddPatchAddr(bw.BaseStream.Position, FontPointerType.glyphLength);
        bw.WriteUInt32(Length);

        // Record glyph pointer
        long ptr = bw.BaseStream.Position;
        linker.AddLookupValue(FontPointerType.ptrGlyph, ptr);

        bw.WriteByte(CellWidth);
        bw.WriteByte(CellHeight);
        bw.WriteByte(BaselinePos);
        bw.WriteByte(MaxCharWidth);
        bw.WriteUInt32(SheetSize);
        bw.WriteUInt16(SheetCount);
        bw.WriteUInt16(SheetFormat);
        bw.WriteUInt16(CellsPerRow);
        bw.WriteUInt16(CellsPerColumn);
        bw.WriteUInt16(SheetWidth);
        bw.WriteUInt16(SheetHeight);

        // Patch sheet pointer
        linker.AddPatchAddr(bw.BaseStream.Position, FontPointerType.sheetPtr);
        bw.WriteUInt32(SheetPtr);
        MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} TGLP End");

        // Pad to next align boundary (e.g. 0x10)
        long padBytes = align - (bw.BaseStream.Position % align);
        if (padBytes != align)
        {
            for (uint i = 0; i < padBytes; i++)
                bw.WriteByte((byte)0x0);
        }

        MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} Sheets Start");
        // Image data
        linker.AddLookupValue(FontPointerType.sheetPtr, bw.BaseStream.Position);
        foreach (var sheet in sheets)
        {
            bw.WriteBytes(sheet, 0, sheet.Length);
        }

        // Padding to 4-byte boundary
        padBytes = 0x4 - (bw.BaseStream.Position % 0x4);
        if (padBytes != 0x4)
        {
            for (uint i = 0; i < padBytes; i++)
                bw.WriteByte((byte)0x0);
        }
        MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} Sheets End");

        // Update glyph length
        linker.AddLookupValue(FontPointerType.glyphLength, bw.BaseStream.Position - ptr + 0x8U);
    }
}