using System;

public class Endianness
{
    private static readonly Int32 testLE = 0x03020100;
    private static readonly Int32 testBE = 0x00010203;
    private static readonly byte[] test = { 0, 1, 2, 3 };
    public enum Endian
    {
        Little,
        Big
    }
    public static bool IsSystemLittleEndian()
    {
        return GetSystemEndianness() == Endian.Little;
    }

    public static Endian GetSystemEndianness()
    {
        uint value = BitConverter.ToUInt32(test, 0);

        if (value == testLE)
            return Endian.Little;
        else if (value == testBE)
            return Endian.Big;
        else
            throw new InvalidOperationException("Unknown CPU endianness!");
    }
}
