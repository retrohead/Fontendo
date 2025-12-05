
using System.Text;

namespace Fontendo.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    public class UnicodeNames
    {
        private readonly Dictionary<uint, string> nameLookupTable = new();

        public UnicodeNames()
        {

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = nameof(Fontendo) + ".Resources.DerivedName.txt";
            nameLookupTable = new Dictionary<uint, string>();
            StringBuilder unicode = new StringBuilder();
            try
            {
                using (Stream s = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {

                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Length == 0) continue;          // skip empty lines
                            if (line[0] == '#') continue;            // skip comments

                            if (!line.Contains(".."))
                            {
                                // Single code point
                                var parts = line.Split(';');
                                if (parts.Length < 2) continue;

                                var codePointStr = parts[0].Trim().Split(' ')[0];
                                var charNameStr = parts[1].Trim();

                                uint codePoint = uint.Parse(codePointStr, NumberStyles.HexNumber);
                                nameLookupTable.Add(codePoint, charNameStr);
                            }
                            else
                            {
                                // Range of code points
                                var parts = line.Split(';');
                                if (parts.Length < 2) continue;

                                var rangePart = parts[0].Trim();
                                var charNameStr = parts[1].Trim();

                                var rangePieces = rangePart.Split(new[] { ".." }, StringSplitOptions.None);
                                if (rangePieces.Length != 2) continue;

                                uint codePointStart = uint.Parse(rangePieces[0], NumberStyles.HexNumber);
                                uint codePointEnd = uint.Parse(rangePieces[1].Split(' ')[0], NumberStyles.HexNumber);

                                // Trim trailing '*' if present in name
                                int starIndex = charNameStr.IndexOf('*');
                                if (starIndex >= 0)
                                    charNameStr = charNameStr.Substring(0, starIndex).Trim();

                                for (uint cp = codePointStart; cp <= codePointEnd; cp++)
                                {
                                    nameLookupTable.Add(cp, $"{charNameStr}{cp:X4}");
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                nameLookupTable.Clear();
                MessageBox.Show("DerivedName.txt file is corrupted and will not be used", "DerivedName.txt Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public string GetCharNameFromUnicodeCodepoint(uint codePoint)
        {
            return nameLookupTable.TryGetValue(codePoint, out var result) ? result : string.Empty;
        }
    }

}
