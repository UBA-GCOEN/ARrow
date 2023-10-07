using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gpm.Manager.Constant
{
    [XmlRoot("supportInfo")]
    public class SupportInfo
    {
        [XmlRoot("menuList")]
        public class MenuList
        {
            [XmlRoot("menu")]
            public class Menu
            {
                public string text;
                public string url;
            }

            [XmlElement("menu")]
            public List<Menu> list;
        }
        
        public string version;
        public MenuList menuList;
        public string mail = string.Empty;
    }
}