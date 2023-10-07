using Gpm.Common.Log;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gpm.Common.Multilanguage.Internal
{
    public class MultilanguageImplementation
    {
        private static MultilanguageImplementation instance;

        public static MultilanguageImplementation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MultilanguageImplementation();
                }

                return instance;
            }
        }
        
        private Dictionary<string, MultilanguageServiceData> services = new Dictionary<string, MultilanguageServiceData>();
        private IMultilanguageLoader defaultLoader = new MultilanguageLoader();

        public void SetDafaultLoader(IMultilanguageLoader loader)
        {
            this.defaultLoader = loader;
        }

        public void Load(IMultilanguageLoader loader, string serviceName, string languagePath, MultilanguageCallback callback)
        {
            if (services.ContainsKey(serviceName) == true)
            {
                callback(MultilanguageResultCode.ALREADY_LOADED, null);
                return;
            }

            loader.Load(
                languagePath,
                (resultCode, xmlData, messageInfo) =>
                {
                    callback(InitializeService(serviceName, resultCode, xmlData), messageInfo);
                });
        }

        public void Load(string serviceName, string languagePath, MultilanguageCallback callback)
        {
            Load(defaultLoader, serviceName, languagePath, callback);
        }

        public void Unload(string serviceName, MultilanguageCallback callback)
        {
            if (services.ContainsKey(serviceName) == true)
            {
                services.Remove(serviceName);
                callback(MultilanguageResultCode.SUCCESS, null);
            }
            else
            {
                callback(MultilanguageResultCode.NOT_LOADED, null);
            }
        }

        public void SelectLangaugeByCode(string serviceName, string languageCode, MultilanguageCallback callback)
        {
            MultilanguageServiceData service;
            if (services.TryGetValue(serviceName, out service) == true)
            {
                MultilanguageResultCode resultCode = service.SelectLanguage(languageCode);
                callback(resultCode, null);
            }
            else
            {
                callback(MultilanguageResultCode.SERVICE_NOT_FOUND, null);
            }
        }

        public void SelectLanguageByNativeName(string serviceName, string nativeName, MultilanguageCallback callback)
        {
            string languageCode = GetLanguageCodeByNativeName(nativeName);

            if (string.IsNullOrEmpty(languageCode) == true)
            {
                languageCode = nativeName;
            }

            SelectLangaugeByCode(serviceName, languageCode, callback);
        }

        public string GetString(string serviceName, string stringKey)
        {
            MultilanguageServiceData languageInfo;
            if (services.TryGetValue(serviceName, out languageInfo) == false)
            {
                return string.Empty;
            }

            return languageInfo.GetString(stringKey);
        }

        public string GetSelectLanguage(string serviceName, bool isNativeName)
        {
            MultilanguageServiceData service;
            if (services.TryGetValue(serviceName, out service) == true)
            {
                string code = service.GetSelectLanguage();
                if (isNativeName == false)
                {
                    return code;
                }
                
                try
                {
                    return CultureInfo.GetCultureInfo(code).NativeName;
                }
                catch (Exception e)
                {
                    GpmLogger.Error(
                        string.Format("Language information not found. (language code: {0})(message: {1})", code, e.Message),
                        GpmMultilanguage.SERVICE_NAME, GetType());
                    return code;
                }
            }

            return string.Empty;
        }

        public bool IsLoadService(string serviceName)
        {
            return services.ContainsKey(serviceName);
        }

        public string[] GetSupportLanguages(string serviceName, bool isNativeName)
        {
            MultilanguageServiceData languageInfo;
            if (services.TryGetValue(serviceName, out languageInfo) == true)
            {
                List<string> result = new List<string>();

                var supportCodes = languageInfo.GetSupportLanguages();
                if (isNativeName == false)
                {
                    foreach (var code in supportCodes)
                    {
                        result.Add(code);
                    }

                    return result.ToArray();
                }

                foreach (var code in supportCodes)
                {
                    try
                    {
                        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(code);
                        result.Add(cultureInfo.NativeName);
                    }
                    catch (Exception e)
                    {
                        GpmLogger.Error(
                            string.Format("Language information not found. (language code: {0})(message: {1})", code, e.Message),
                            GpmMultilanguage.SERVICE_NAME, GetType());
                        
                        result.Add(code);
                    }
                }

                return result.ToArray();
            }

            GpmLogger.Debug("Not load language file.", serviceName, GetType());
            return null;
        }

        private string GetLanguageCodeByNativeName(string nativeName)
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            CultureInfo findCulture = null;
            for (int i = 0; i < cultures.Length; i++)
            {
                if (cultures[i].NativeName.Equals(nativeName, StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    findCulture = cultures[i];
                    break;
                }
            }

            if (findCulture == null)
            {
                return string.Empty;
            }

            return findCulture.Name;
        }

        private MultilanguageResultCode InitializeService(string serviceName, MultilanguageResultCode resultCode, MultilanguageXml xmlData)
        {
            if (resultCode != MultilanguageResultCode.SUCCESS)
            {
                return resultCode;
            }

            MultilanguageServiceData languageInfo = new MultilanguageServiceData(serviceName);

            resultCode = languageInfo.Initialize(xmlData);
            if (resultCode == MultilanguageResultCode.SUCCESS)
            {
                services.Add(serviceName, languageInfo);
            }

            return resultCode;
        }
    }
}