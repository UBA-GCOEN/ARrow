#if CSHARP_7_3_OR_NEWER

using System;

namespace Gpm.Common.ThirdParty.SharpCompress.Common
{
    public class InvalidFormatException : ExtractionException
    {
        public InvalidFormatException(string message)
            : base(message)
        {
        }

        public InvalidFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

#endif