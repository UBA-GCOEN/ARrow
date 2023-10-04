#if CSHARP_7_3_OR_NEWER

using System;
using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common;
using Gpm.Common.ThirdParty.SharpCompress.Writers.GZip;
using Gpm.Common.ThirdParty.SharpCompress.Writers.Tar;
using Gpm.Common.ThirdParty.SharpCompress.Writers.Zip;

namespace Gpm.Common.ThirdParty.SharpCompress.Writers
{
    public static class WriterFactory
    {
        public static IWriter Open(Stream stream, ArchiveType archiveType, WriterOptions writerOptions)
        {
            switch (archiveType)
            {
                case ArchiveType.GZip:
                {
                    if (writerOptions.CompressionType != CompressionType.GZip)
                    {
                        throw new InvalidFormatException("GZip archives only support GZip compression type.");
                    }
                    return new GZipWriter(stream, new GZipWriterOptions(writerOptions));
                }
                case ArchiveType.Zip:
                {
                    return new ZipWriter(stream, new ZipWriterOptions(writerOptions));
                }
                case ArchiveType.Tar:
                {
                    return new TarWriter(stream, new TarWriterOptions(writerOptions));
                }
                default:
                {
                    throw new NotSupportedException("Archive Type does not have a Writer: " + archiveType);
                }
            }
        }
    }
}

#endif