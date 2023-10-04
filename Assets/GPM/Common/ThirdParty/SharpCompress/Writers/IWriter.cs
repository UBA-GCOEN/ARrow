#if CSHARP_7_3_OR_NEWER

using System;
using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common;

namespace Gpm.Common.ThirdParty.SharpCompress.Writers
{
    public interface IWriter : IDisposable
    {
        ArchiveType WriterType { get; }
        void Write(string filename, Stream source, DateTime? modificationTime);
    }
}

#endif