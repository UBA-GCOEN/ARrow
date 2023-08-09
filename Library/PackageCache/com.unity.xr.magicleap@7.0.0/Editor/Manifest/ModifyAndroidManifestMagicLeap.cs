using System;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

// from com.unity.xr.openxr
internal class AndroidXmlDocument : XmlDocument
{
    private string m_Path;
    protected XmlNamespaceManager nsMgr;
    public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

    public AndroidXmlDocument(string path)
    {
        m_Path = path;
        using (var reader = new XmlTextReader(m_Path))
        {
            reader.Read();
            Load(reader);
        }

        nsMgr = new XmlNamespaceManager(NameTable);
        nsMgr.AddNamespace("android", AndroidXmlNamespace);
    }

    public string Save()
    {
        return SaveAs(m_Path);
    }

    public string SaveAs(string path)
    {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
        {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }

        return path;
    }
}

internal class ModifyAndroidManifestMagicLeap : AndroidXmlDocument
{
    private readonly XmlElement ActivityElement;

    public ModifyAndroidManifestMagicLeap(string path) : base(path)
    {
        ActivityElement = SelectSingleNode("/manifest/application/activity") as XmlElement;
    }

    public void IncludeMagicLeapMetaData(bool shouldIncludeMetadata)
    {
        string attrName = "com.magicleap.augmented_reality_only";
        var existing = ActivityElement.SelectNodes($"meta-data[@android:name='{attrName}']", nsMgr);
        if (existing.Count == 0)
        {
            if (shouldIncludeMetadata)
            {
                var metadataTag = ActivityElement.AppendChild(CreateElement("meta-data"));
                metadataTag.Attributes.Append(CreateAndroidAttribute("name", $"{attrName}"));
                metadataTag.Attributes.Append(CreateAndroidAttribute("value", "true"));
            }
        }
        else if(!shouldIncludeMetadata)
        {
            for(int i = existing.Count - 1; i >= 0; i--)
            {
                ActivityElement.RemoveChild(existing[i]);
            }
        }
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value)
    {
        XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }
}
