#if CSHARP_7_3_OR_NEWER

using Gpm.Common.ThirdParty.SharpCompress.Common;

namespace Gpm.Common.ThirdParty.SharpCompress.Readers
{
    public class ReaderOptions : OptionsBase
    {
        /// <summary>
        /// Look for RarArchive (Check for self-extracting archives or cases where RarArchive isn't at the start of the file)
        /// </summary>
        public bool LookForHeader { get; set; }

        public string Password { get; set; }
    }
}

#endif