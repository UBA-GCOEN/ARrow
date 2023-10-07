#if CSHARP_7_3_OR_NEWER

using System;
using System.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Common.Zip.Headers
{
    internal class SplitHeader : ZipHeader
    {
        public SplitHeader()
            : base(ZipHeaderType.Split)
        {
        }

        internal override void Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}

#endif