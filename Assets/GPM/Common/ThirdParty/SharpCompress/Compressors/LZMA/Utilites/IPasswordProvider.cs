#if CSHARP_7_3_OR_NEWER

namespace Gpm.Common.ThirdParty.SharpCompress.Compressors.LZMA.Utilites
{
    internal interface IPasswordProvider
    {
        string CryptoGetTextPassword();
    }
}

#endif