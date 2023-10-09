#if CSHARP_7_3_OR_NEWER

using System;
using System.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Common.Zip.Headers
{
    internal class IgnoreHeader : ZipHeader
    {
        public IgnoreHeader(ZipHeaderType type)
            : base(type)
        {
        }

        internal override void Read(BinaryReader reader)
        {
        }
    }
}

#endif