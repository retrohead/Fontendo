using System;

public class BitReader
{
    private long bitPosition;
    private long bytePosition;
    private byte buffer;
    private BinaryReader br;

    public BitReader(BinaryReader br)
    {
        this.br = br;
        bytePosition = br.BaseStream.Position;
        bitPosition = bytePosition * 8U;
        buffer = br.ReadByte();
    }

    public bool IsByteAligned()
    {
        return bitPosition % 8 == 0;
    }

    public byte ReadBits(int count)
    {
        byte result = 0;
        for (int i = count - 1; i >= 0U && i < count; i--)
        {
            result |= (byte)(ReadBit() << i);
        }
        return result;
    }
    public byte ReadBitsBackwards(int count)
    {
        byte result = 0;
        for (int i = 0; i < count; i++)
        {
            result |= (byte)(ReadBit() << i);
        }
        return result;
    }
    public byte ReadBitsNormalized(int count)
    {
        return (byte)Math.Round(((double)ReadBits(count) / (double)((1 << count) - 1)) * 255.0);
    }
    public byte ReadBitsNormalizedBackwards(int count)
    {
        return (byte)Math.Round(((double)ReadBitsBackwards(count) / (double)((1 << count) - 1)) * 255.0);
    }


    public byte ReadBit()
    {
        long bytepos = bitPosition / 8; //Calculate which byte we should be reading
        if (bytepos != bytePosition) //If it's not the same as already read, then update buffer and positions
        {
            bytePosition = bytepos; //Update byte position
            br.BaseStream.Position = bytepos; //Set position
            buffer = br.ReadByte();
        }
        int shift = 7 - (int)(bitPosition % 8);
        byte value = (byte)((buffer >> shift) & 0x01);
        bitPosition++; //Increment the bit postion for the next read
        return value;
    }
    public void Update()
    {
        bytePosition = br.BaseStream.Position;
        bitPosition = bytePosition * 8U;
        buffer = br.ReadByte();
    }
    public void UpdatePosition(long pos)
    {
        br.BaseStream.Position = pos;
        bytePosition = pos;
        bitPosition = bytePosition * 8U;
        buffer = br.ReadByte();
    }

}