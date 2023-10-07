#if CSHARP_7_3_OR_NEWER

using Gpm.Common.ThirdParty.SharpCompress.Common;

namespace Gpm.Common.ThirdParty.SharpCompress.Writers
{
    public class WriterOptions : OptionsBase
    {
        public WriterOptions(CompressionType compressionType)
        {
            CompressionType = compressionType;
        }
        public CompressionType CompressionType { get; set; }

        public static implicit operator WriterOptions(CompressionType compressionType)
        {
            return new WriterOptions(compressionType);
        }
    }
}

#endif