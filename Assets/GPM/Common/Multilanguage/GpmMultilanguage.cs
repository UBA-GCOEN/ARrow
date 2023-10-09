using Gpm.Common.Multilanguage.Internal;

namespace Gpm.Common.Multilanguage
{
    public static class GpmMultilanguage
    {
        public const string SERVICE_NAME = "Multilanguage";
        
        public static void SetDafaultLoader(IMultilanguageLoader loader)
        {
            MultilanguageImplementation.Instance.SetDafaultLoader(loader);
        }
        
        public static void Load(IMultilanguageLoader loader, string serviceName, string filepath, MultilanguageCallback callback)
        {
            MultilanguageImplementation.Instance.Load(loader, serviceName, filepath, callback);
        }

        public static void Load(string serviceName, string filepath, MultilanguageCallback callback)
        {
            MultilanguageImplementation.Instance.Load(serviceName, filepath, callback);
        }

        public static void Unload(string serviceName, MultilanguageCallback callback)
        {
            MultilanguageImplementation.Instance.Unload(serviceName, callback);
        }

        public static void SelectLanguageByCode(string serviceName, string languageCode, MultilanguageCallback callback)
        {
            MultilanguageImplementation.Instance.SelectLangaugeByCode(serviceName, languageCode, callback);
        }

        public static void SelectLanguageByNativeName(string serviceName, string natvieName, MultilanguageCallback callback)
        {
            MultilanguageImplementation.Instance.SelectLanguageByNativeName(serviceName, natvieName, callback);
        }

        public static string GetString(string serviceName, string stringKey)
        {
            return MultilanguageImplementation.Instance.GetString(serviceName, stringKey);
        }

        public static string GetSelectLanguage(string serviceName, bool isNativeName)
        {
            return MultilanguageImplementation.Instance.GetSelectLanguage(serviceName, isNativeName);
        }

        public static string[] GetSupportLanguages(string serviceName, bool isNativeName)
        {
            return MultilanguageImplementation.Instance.GetSupportLanguages(serviceName, isNativeName);
        }

        public static bool IsLoadService(string serviceName)
        {
            return MultilanguageImplementation.Instance.IsLoadService(serviceName);
        }

        public static string GetSystemLanguage(bool isNativeName)
        {
            return MultilanguageUtil.GetSystemLanguage(isNativeName);
        }
    }
}
