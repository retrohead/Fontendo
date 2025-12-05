using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions
{

    public class SJISConv
    {
        private readonly Dictionary<UInt16, UInt16> nameLookupTable;

        // Constructor
        public SJISConv()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = nameof(Fontendo) + ".Resources.SHIFTJIS.TXT";
            nameLookupTable = new Dictionary<UInt16, UInt16>();
            StringBuilder sjis = new StringBuilder();
            try
            {
                using (Stream s = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (line.StartsWith("#"))
                                continue;
                            // Expecting lines like: "8140 3000" (hex values)
                            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 2 &&
                                UInt16.TryParse(parts[0].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out UInt16 sjisCode) &&
                                UInt16.TryParse(parts[1].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out UInt16 utf16))
                            {
                                nameLookupTable.Add(sjisCode, utf16);
                            }
                        }
                    }
                }
            }
            catch
            {
                nameLookupTable.Clear();
                MessageBox.Show("SHIFTJIS.TXT file is corrupted and will not be used", "SHIFTJIS.TXT Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // Methods
        public ushort CodeToUTF16(ushort code)
        {
            if (nameLookupTable.TryGetValue(code, out ushort utf16))
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
