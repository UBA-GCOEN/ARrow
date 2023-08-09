using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace UnityEditor.XR.ARKit
{
    class Plist
    {
        public class Element
        {
            XmlNode m_Node;

            public Element(XmlNode node) => m_Node = node ?? throw new ArgumentNullException(nameof(node));

            public Element this[string key]
            {
                get
                {
                    if (m_Node.Name != "dict")
                        throw new InvalidDataException($"Node '{m_Node.Name}' is not a dictionary.");

                    var value = EnumerateKeys(m_Node.ChildNodes)
                        .Where(k => k.InnerText == key)
                        .Select(k => new Element(k.NextSibling))
                        .FirstOrDefault();

                    if (value == null)
                        throw new KeyNotFoundException($"Key '{key}' not found.");

                    return value;
                }
            }

            public Dictionary<string, Element> AsDictionary()
            {
                if (m_Node.Name != "dict")
                    return null;

                var dict = new Dictionary<string, Element>();
                foreach (var key in EnumerateKeys(m_Node.ChildNodes))
                {
                    var value = key.NextSibling;
                    dict[key.InnerText] = new Element(value);
                }

                return dict;
            }

            public Element[] AsArray() => m_Node.Name == "array"
                ? EnumerateNodes(m_Node.ChildNodes).Select(node => new Element(node)).ToArray()
                : null;

            public string AsString() => m_Node.Name == "string"
                ? m_Node.InnerText
                : null;

            public int? AsInt32() => m_Node.Name == "integer"
                ? new int?(int.Parse(m_Node.InnerText))
                : null;

            public float? AsFloat() => m_Node.Name == "real"
                ? new float?(float.Parse(m_Node.InnerText, CultureInfo.InvariantCulture))
                : null;

            public Vector3? AsVector3()
            {
                var array = AsArray()?.Select(element => element.AsFloat()).ToArray();
                return array?.Length == 3 && array.All(v => v.HasValue)
                    ? new Vector3?(new Vector3(array[0].Value, array[1].Value, array[2].Value))
                    : null;
            }

            public Quaternion? AsQuaternion()
            {
                var array = AsArray()?.Select(element => element.AsFloat()).ToArray();
                return array?.Length == 4 && array.All(v => v.HasValue)
                    ? new Quaternion?(new Quaternion(array[0].Value, array[1].Value, array[2].Value, array[3].Value))
                    : null;
            }
        }

        XmlDocument m_XmlDocument;

        public Plist(XmlDocument xmlDocument) =>
            m_XmlDocument = xmlDocument ?? throw new ArgumentNullException(nameof(xmlDocument));

        public static Plist Load(StreamReader reader)
        {
            var xml = new XmlDocument();
            xml.Load(reader);
            return new Plist(xml);
        }

        public static Plist ReadFromString(string contents)
        {
            var xml = new XmlDocument();
            xml.LoadXml(contents);
            return new Plist(xml);
        }

        public Element root
        {
            get
            {
                var child = m_XmlDocument.SelectSingleNode("child::plist/dict");
                return child != null ? new Element(child) : null;
            }
        }

        static IEnumerable<XmlNode> EnumerateNodes(XmlNodeList nodeList)
        {
            var enumerator = nodeList?.GetEnumerator();
            if (enumerator == null)
                yield break;

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current as XmlNode;
            }
        }

        static IEnumerable<XmlNode> EnumerateKeys(XmlNodeList nodeList) =>
            EnumerateNodes(nodeList).Where(node => node.Name == "key");
    }
}
