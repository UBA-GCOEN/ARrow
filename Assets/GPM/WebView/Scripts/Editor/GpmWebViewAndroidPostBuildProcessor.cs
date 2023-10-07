#if UNITY_EDITOR
namespace Gpm.WebView.Editor
{
    using Gpm.Common.Util;
    using System.Text;
    using System.Xml;
    using UnityEditor.Android;

    public class GpmWebViewAndroidPostBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            var changed = false;
            var androidManifest = new AndroidManifest(GetManifestPath(basePath));
            changed = (androidManifest.SetHardwareAccelerated(true) || changed);
            if (changed == true)
            {
                androidManifest.Save();
            }
        }

        public int callbackOrder { get { return 1; } }

        private string GetManifestPath(string basePath)
        {
            return GpmPathUtil.Combine(basePath, "src", "main", "AndroidManifest.xml");
        }
    }

    internal class AndroidXmlDocument : XmlDocument
    {
        public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        public readonly string ToolsXmlNamespace = "http://schemas.android.com/tools";

        private string filePath;
        protected XmlNamespaceManager xmlNamespaceManager;

        public AndroidXmlDocument(string path)
        {
            filePath = path;
            using (var reader = new XmlTextReader(filePath))
            {
                reader.Read();
                Load(reader);
            }
            xmlNamespaceManager = new XmlNamespaceManager(NameTable);
            xmlNamespaceManager.AddNamespace("android", AndroidXmlNamespace);
        }

        public string Save()
        {
            return SaveAs(filePath);
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

    internal class AndroidManifest : AndroidXmlDocument
    {
        private readonly XmlElement applicationElement;

        public AndroidManifest(string path) : base(path)
        {
            applicationElement = SelectSingleNode("/manifest/application") as XmlElement;
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }

        internal XmlNode GetActivityWithLaunchIntent()
        {
            return SelectSingleNode(
                "/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and "
                + "intent-filter/category/@android:name='android.intent.category.LAUNCHER']",
                xmlNamespaceManager);
        }

        internal bool SetHardwareAccelerated(bool enabled)
        {
            XmlElement activity = GetActivityWithLaunchIntent() as XmlElement;

            bool changed = false;
            string enabledString = GetBoolString(enabled);
            if (activity.GetAttribute("hardwareAccelerated", AndroidXmlNamespace) != enabledString)
            {
                activity.SetAttribute("hardwareAccelerated", AndroidXmlNamespace, enabledString);
                changed = true;
            }

            return changed;
        }

        private string GetBoolString(bool value)
        {
            return value ? "true" : "false";
        }
    }
}
#endif