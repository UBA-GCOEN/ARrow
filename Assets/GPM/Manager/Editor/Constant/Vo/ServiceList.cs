using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gpm.Manager.Constant
{
    [XmlRoot("serviceList")]
    public class ServiceList
    {
        [XmlRoot("service")]
        public class Service
        {
            public string name;
            public string version;
        }

        [XmlElement("service")]
        public List<Service> list;
    }
}