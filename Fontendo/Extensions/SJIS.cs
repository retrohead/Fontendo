using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions
{

    public class SJISConv
    {
        private readonly Dictionary<ushort, ushort> nameLookUpTable;

        // Constructor
        public SJISConv(string sjisPath = "SHIFTJIS.TXT")
        {
            nameLookUpTable = new Dictionary<ushort, ushort>();

            if (File.Exists(sjisPath))
            {
                try
                {
                    foreach (var line in File.ReadLines(sjisPath))
                    {
                        // Expecting lines like: "8140 3000" (hex values)
                        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2 &&
                            ushort.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out ushort sjis) &&
                            ushort.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out ushort utf16))
                        {
                            nameLookUpTable[sjis] = utf16;
                        }
                    }
                } catch
                {
                    nameLookUpTable.Clear();
                    MessageBox.Show("SHIFTJIS.TXT file is corrupted and will not be used", "SHIFTJIS.TXT Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
        }

        // Methods
        public ushort CodeToUTF16(ushort code)
        {
            if (nameLookUpTable.TryGetValue(code, out ushort utf16))
                return utf16;

            return 0; // or throw, depending on your needs
        }

        public byte[] CodeToUTF8(ushort code)
        {
            ushort utf16 = CodeToUTF16(code);
            if (utf16 == 0)
                return Array.Empty<byte>();

            // Convert UTF-16 code unit to UTF-8 bytes
            string s = char.ConvertFromUtf32(utf16);
            return Encoding.UTF8.GetBytes(s);
        }
    }

}
