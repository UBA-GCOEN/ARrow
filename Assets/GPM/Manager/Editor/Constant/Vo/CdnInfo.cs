using System.Xml.Serialization;

namespace Gpm.Manager.Constant
{
    [XmlRoot("cdninfo")]
    public class CdnInfo
    {
        public string release = string.Empty;
        public string docs = string.Empty;
    }
}