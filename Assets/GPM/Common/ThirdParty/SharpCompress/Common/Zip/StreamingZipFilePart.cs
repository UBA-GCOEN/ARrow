#if CSHARP_7_3_OR_NEWER

using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common.Zip.Headers;
using Gpm.Common.ThirdParty.SharpCompress.Compressors.Deflate;
using Gpm.Common.ThirdParty.SharpCompress.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Common.Zip
{
    internal class StreamingZipFilePart : ZipFilePart
    {
        private Stream _decompressionStream;

        internal StreamingZipFilePart(ZipFileEntry header, Stream stream)
            : base(header, stream)
        {
        }

        protected override Stream CreateBaseStream()
        {
            return Header.PackedStream;
        }

        internal override Stream GetCompressedStream()
        {
            if (!Header.HasData)
            {
                return Stream.Null;
            }
            _decompressionStream = CreateDecompressionStream(GetCryptoStream(CreateBaseStream()), Header.CompressionMethod);
            if (LeaveStreamOpen)
            {
                return new NonDisposingStream(_decompressionStream);
            }
            return _decompressionStream;
        }

        internal BinaryReader FixStreamedFileLocation(ref RewindableStream rewindableStream)
        {
            if (Header.IsDirectory)
            {
                return new BinaryReader(rewindableStream);
            }
            if (Header.HasData && !Skipped)
            {
                if (_decompressionStream == null)
                {
                    _decompressionStream = GetCompressedStream();
                }
                _decompressionStream.Skip();

                DeflateStream deflateStream = _decompressionStream as DeflateStream;
                if (deflateStream != null)
                {
                    rewindableStream.Rewind(deflateStream.InputBuffer);
                }
                Skipped = true;
            }
            var reader = new BinaryReader(rewindableStream);
            _decompressionStream = null;
            return reader;
        }
    }
}

#endif