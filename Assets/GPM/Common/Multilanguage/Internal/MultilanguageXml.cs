using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Gpm.Common.Multilanguage.Internal
{
    [XmlRoot("multilanguage")]
    public class MultilanguageXml
    {
        public class DefaultData
        {
            [XmlElement("language")]
            public string language;

            [XmlElement("string")]
            public string text;
        }

        public class StringList
        {
            public class StringData
            {
                public class LanguageList
                {
                    [XmlAnyElement]
                    public XmlElement[] list;
                }

                [XmlElement("key")]
                public string key;

                [XmlElement("language")]
                public LanguageList language;
            }

            [XmlElement("string")]
            public List<StringData> list;
        }

        [XmlElement("default")]
        public DefaultData defaultData;

        [XmlElement("list")]
        public StringList stringList;
    }
}
