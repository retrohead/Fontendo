using Fontendo.UI;
using System.Diagnostics;
using System.Linq;
using static Fontendo.Extensions.FontBase;

namespace Fontendo.Extensions.BinaryTools
{
    public class BlockLinker
    {
        // Lookup tables
        private readonly Dictionary<FontPointerType, long> valueLookupTable = new Dictionary<FontPointerType, long>();
        private readonly Dictionary<FontPointerType, long> patchLookupTable = new Dictionary<FontPointerType, long>();
        private readonly Dictionary<FontPointerType, long> shortPatchLookupTable = new Dictionary<FontPointerType, long>();

        private readonly Dictionary<string, long> valueByNameLookupTable = new Dictionary<string, long>();
        private readonly Dictionary<string, long> patchByNameLookupTable = new Dictionary<string, long>();

        public BlockLinker()
        {
        }

        /// <summary>
        /// Apply all recorded patches to the binary writer.
        /// </summary>
        public void MakeBlockLink(BinaryWriterX bw)
        {
            foreach (var patch in patchLookupTable)
            {
                bw.SetPosition(patch.Value);
                UInt32 val = (UInt32)GetLookupValue(patch.Key);
                MainWindow.Log($"-> Patching uint {patch.Key} at 0x{bw.BaseStream.Position.ToString("X8")} with {val}");
                bw.WriteUInt32(val);
            }
            foreach (var patch in patchByNameLookupTable)
            {
                bw.SetPosition(patch.Value);
                UInt32 val = (UInt32)GetLookupValueByName(patch.Key);
                MainWindow.Log($"-> Patching uint {patch.Key} at 0x{bw.BaseStream.Position.ToString("X8")} with {val}");
                bw.WriteUInt32(val);
            }

            foreach (var patch in shortPatchLookupTable)
            {
                bw.SetPosition(patch.Value);
                var val = GetLookupValue(patch.Key);
                MainWindow.Log($"-> Patching ushort {patch.Key} at 0x{bw.BaseStream.Position.ToString("X8")} with {val}");
                bw.WriteUInt16((ushort)val);
            }
        }

        /// <summary>
        /// Add or overwrite a lookup value.
        /// </summary>
        public void AddLookupValue(FontPointerType lookup, long value)
        {
            valueLookupTable[lookup] = value;
        }

        /// <summary>
        /// Add or overwrite a lookup value.
        /// </summary>
        public void AddLookupValueByName(string name, long value)
        {
            valueByNameLookupTable[name] = value;
        }

        /// <summary>
        /// Increment a lookup value, or initialize if missing.
        /// </summary>
        public void IncLookupValue(FontPointerType lookup, long inc)
        {
            if (valueLookupTable.ContainsKey(lookup))
            {
                valueLookupTable[lookup] += inc;
            }
            else
            {
                valueLookupTable.Add(lookup, inc);
            }
        }

        /// <summary>
        /// Increment a lookup value, or initialize if missing.
        /// </summary>
        public void IncLookupValueByName(string lookup, long inc)
        {
            if (valueByNameLookupTable.ContainsKey(lookup))
            {
                valueByNameLookupTable[lookup] += inc;
            }
            else
            {
                valueByNameLookupTable.Add(lookup, inc);
            }
        }

        /// <summary>
        /// Retrieve a lookup value, or 0 if missing.
        /// </summary>
        public long GetLookupValue(FontPointerType lookup)
        {
            if (valueLookupTable.TryGetValue(lookup, out var result))
            {
                return result;
            }

            Debug.WriteLine($"Couldn't find value for key {lookup} in valueLookup!");
            return 0;
        }

        /// <summary>
        /// Retrieve a lookup value, or 0 if missing.
        /// </summary>
        public long GetLookupValueByName(string lookup)
        {
            if (valueByNameLookupTable.TryGetValue(lookup, out var result))
            {
                return result;
            }

            Debug.WriteLine($"Couldn't find value for key {lookup} in valueLookup!");
            return 0;
        }

        /// <summary>
        /// Add a patch address for a 32-bit value.
        /// </summary>
        public void AddPatchAddr(long address, FontPointerType lookup)
        {
            MainWindow.Log($"-> Adding patch address for {nameof(lookup)} at 0x{address.ToString("X8")}");
            patchLookupTable.Add(lookup, address);
        }

        /// <summary>
        /// Add a patch address for a 32-bit value.
        /// </summary>
        public void AddPatchAddrByName(long address, string name)
        {
            MainWindow.Log($"-> Adding patch address for {name} at 0x{address.ToString("X8")}");
            patchByNameLookupTable.Add(name, address);
        }

        /// <summary>
        /// Add a patch address for a 16-bit value.
        /// </summary>
        public void AddShortPatchAddr(long address, FontPointerType lookup)
        {
            MainWindow.Log($"-> Adding short patch address for {nameof(lookup)} at 0x{address.ToString("X8")}");
            shortPatchLookupTable.Add(lookup, address);
        }
    }
}
