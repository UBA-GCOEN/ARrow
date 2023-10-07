#if CSHARP_7_3_OR_NEWER

namespace Gpm.Common.ThirdParty.SharpCompress.Common
{
    public class IncompleteArchiveException : ArchiveException
    {
        public IncompleteArchiveException(string message)
            : base(message)
        {
        }
    }
}

#endif