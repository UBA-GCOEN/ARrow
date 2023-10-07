#if CSHARP_7_3_OR_NEWER

using System;
using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common;
using Gpm.Common.ThirdParty.SharpCompress.Compressors;
using Gpm.Common.ThirdParty.SharpCompress.Compressors.Deflate;
using Gpm.Common.ThirdParty.SharpCompress.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Writers.GZip
{
    public class GZipWriter : AbstractWriter
    {
        private bool _wroteToStream;

        public GZipWriter(Stream destination, GZipWriterOptions options = null)
            : base(ArchiveType.GZip, options ?? new GZipWriterOptions())
        {
            if (WriterOptions.LeaveStreamOpen)
            {
                destination = new NonDisposingStream(destination);
            }
            InitalizeStream(new GZipStream(destination, CompressionMode.Compress, 
                                           options?.CompressionLevel ?? CompressionLevel.Default,
                                           WriterOptions.ArchiveEncoding.GetEncoding()));
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                //dispose here to finish the GZip, GZip won't close the underlying stream
                OutputStream.Dispose();
            }
            base.Dispose(isDisposing);
        }

        public override void Write(string filename, Stream source, DateTime? modificationTime)
        {
            if (_wroteToStream)
            {
                throw new ArgumentException("Can only write a single stream to a GZip file.");
            }
            GZipStream stream = OutputStream as GZipStream;
            stream.FileName = filename;
            stream.LastModified = modificationTime;
            source.TransferTo(stream);
            _wroteToStream = true;
        }
    }
}

#endif