namespace Gpm.Common.ThirdParty.LitJson
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonSkipAttribute : Attribute
    {
    }
}