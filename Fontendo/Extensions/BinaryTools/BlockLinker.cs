using System.Diagnostics;

namespace Fontendo.Extensions.BinaryTools
{
    public class BlockLinker
    {
        // Lookup tables
        private readonly Dictionary<string, long> valueLookupTable = new Dictionary<string, long>();
        private readonly Dictionary<string, long> patchLookupTable = new Dictionary<string, long>();
        private readonly Dictionary<string, long> shortPatchLookupTable = new Dictionary<string, long>();

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
                Debug.WriteLine($"Patching uint {patch.Key} at 0x{bw.BaseStream.Position:X}");
                bw.WriteUInt32((UInt32)GetLookupValue(patch.Key));
            }

            foreach (var patch in shortPatchLookupTable)
            {
                bw.SetPosition(patch.Value);
                Debug.WriteLine($"Patching ushort {patch.Key} at 0x{bw.BaseStream.Position:X}");
                var val = GetLookupValue(patch.Key);
                bw.WriteUInt16((ushort)val);
            }
        }

        /// <summary>
        /// Add or overwrite a lookup value.
        /// </summary>
        public void AddLookupValue(string lookup, long value)
        {
            valueLookupTable[lookup] = value;
        }

        /// <summary>
        /// Increment a lookup value, or initialize if missing.
        /// </summary>
        public void IncLookupValue(string lookup, long inc)
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
        /// Retrieve a lookup value, or 0 if missing.
        /// </summary>
        public long GetLookupValue(string lookup)
        {
            if (valueLookupTable.TryGetValue(lookup, out var result))
            {
                return result;
            }

            Debug.WriteLine($"Couldn't find value for key {lookup} in valueLookup!");
            return 0;
        }

        /// <summary>
        /// Add a patch address for a 32-bit value.
        /// </summary>
        public void AddPatchAddr(long address, string lookup)
        {
            patchLookupTable.Add(lookup, address);
        }

        /// <summary>
        /// Add a patch address for a 16-bit value.
        /// </summary>
        public void AddShortPatchAddr(long address, string lookup)
        {
            shortPatchLookupTable.Add(lookup, address);
        }
    }
}
