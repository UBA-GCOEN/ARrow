#if CSHARP_7_3_OR_NEWER

using System;

namespace Gpm.Common.ThirdParty.SharpCompress.Common
{
    public class ExtractionException : Exception
    {
        public ExtractionException(string message)
            : base(message)
        {
        }

        public ExtractionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

#endif