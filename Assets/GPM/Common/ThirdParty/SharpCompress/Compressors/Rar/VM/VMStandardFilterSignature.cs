#if CSHARP_7_3_OR_NEWER

namespace Gpm.Common.ThirdParty.SharpCompress.Compressors.Rar.VM
{
    internal class VMStandardFilterSignature
    {
        internal VMStandardFilterSignature(int length, uint crc, VMStandardFilters type)
        {
            Length = length;
            CRC = crc;
            Type = type;
        }

        internal int Length { get; }

        internal uint CRC { get; }

        internal VMStandardFilters Type { get; }
    }
}

#endif