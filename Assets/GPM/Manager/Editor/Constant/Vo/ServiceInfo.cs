using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Gpm.Manager.Constant
{
    public enum ServiceStatus
    {
        NONE,
        [XmlEnum("publish")]
        PUBLISH,
        [XmlEnum("prepare")]
        PREPARE
    }

    public enum ServiceInstall
    {
        [XmlEnum("auto")]
        AUTO,
        [XmlEnum("manual")]
        MANUAL
    }

    [XmlRoot("info")]
    public class ServiceInfo
    {
        [XmlType]
        public class Package
        {
            public string version;

            [XmlArrayItem("install")]
            public List<Install> installList;
        }

        public class Install
        {
            public string name;
            public string path;
        }

        public class Link
        {
            public string name;
            public string path;
        }

        public class Image
        {
            public string title;
            public string path;
        }

        public class DependencyInfo
        {
            public string version;
            public ServiceInstall install;
        }

        [XmlAttribute("version")]
        public string infoVersion;

        public string title;
        public ServiceStatus status;
        public string description;
        public string path;
        public string version;

        [XmlArrayItem("link")]
        public List<Link> linkList;

        [XmlArrayItem("image")]
        public List<Image> imageList;

        [XmlArrayItem("package")]
        public List<Package> packageList;

        [XmlIgnore]
        public Dictionary<string, DependencyInfo> dependencies;
        
        [XmlAnyElement("dependencies")]
        public XmlElement XmlDependencies
        {
            get
            {
                return null;
            }
            set
            {
                if (value == null)
                {
                    dependencies = null;
                }
                else
                {
                    var dependenciesElements = XElement.Parse(value.OuterXml);

                    dependencies = dependenciesElements.Elements().ToDictionary(
                        e => e.Name.LocalName,
                        e =>
                        {
                            var serializer = new XmlSerializer(typeof(DependencyInfo), new XmlRootAttribute(e.Name.LocalName));
                            var reader = e.CreateReader();

                            return (DependencyInfo)serializer.Deserialize(reader);
                        },
                        StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        public Package GetPackage(string version)
        {
            if (packageList != null)
            {
                foreach (var package in packageList)
                {
                    if (version.Equals(package.version) == true)
                    {
                        return package;
                    }
                }
            }
            return null;
        }
    }
}