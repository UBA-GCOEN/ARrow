using Gpm.Common.Log;
using System.Collections.Generic;

namespace Gpm.Common.Multilanguage.Internal
{
    using StringDictionary = Dictionary<string, string>;

    public class MultilanguageServiceData
    {
        private const string DEFAULT_LANGUAGE_CODE = "en";

        private string serviceName;
        private string selectLanguage;
        private StringDictionary selectLanguageStrings;
        private Dictionary<string, StringDictionary> languagePacks = new Dictionary<string, StringDictionary>();

        public MultilanguageServiceData(string serviceName)
        {
            this.serviceName = serviceName;
        }

        public MultilanguageResultCode Initialize(MultilanguageXml languageXml)
        {
            if (languageXml.stringList == null)
            {
                return MultilanguageResultCode.FILE_PARSING_ERROR;
            }

            foreach (var data in languageXml.stringList.list)
            {
                string key = data.key;
                var languages = data.language;

                foreach (var language in languages.list)
                {
                    string languageCode = language.LocalName;

                    StringDictionary languageStringMap;
                    if (languagePacks.TryGetValue(languageCode, out languageStringMap) == false)
                    {
                        languageStringMap = new StringDictionary();
                        languagePacks.Add(languageCode, languageStringMap);
                    }

                    string value = language.InnerText.Replace("\\n", "\n");
                    if (languageStringMap.ContainsKey(key) == true)
                    {
                        GpmLogger.Warn(string.Format("Already have a string key. (key: {0}, language: {1})", key, languageCode), serviceName, GetType());
                    }
                    else
                    {
                        languageStringMap.Add(key, value);
                    }
                }
            }

            return InitializeLanguageCode(languageXml.defaultData);
        }

        public MultilanguageResultCode SelectLanguage(string languageCode)
        {
            if (languagePacks.ContainsKey(languageCode) == false)
            {
                GpmLogger.Error("Language code not found.", serviceName, GetType());
                return MultilanguageResultCode.LANGUAGE_CODE_NOT_FOUND;
            }

            selectLanguage = languageCode;
            selectLanguageStrings = languagePacks[selectLanguage];

            return MultilanguageResultCode.SUCCESS;
        }

        public IEnumerable<string> GetSupportLanguages()
        {
            return languagePacks.Keys;
        }

        public string GetString(string stringKey)
        {
            if (selectLanguageStrings == null)
            {
                GpmLogger.Error("Language file is not loaded.", serviceName, GetType());
                return stringKey;
            }

            if (selectLanguageStrings.ContainsKey(stringKey) == false)
            {
                GpmLogger.Error(string.Format("String key not found. (key= {0})", stringKey), serviceName, GetType());
                return stringKey;
            }

            string value = selectLanguageStrings[stringKey];
            if (string.IsNullOrEmpty(value) == true)
            {
                GpmLogger.Error(string.Format("String value is null or empty. (key= {0})", stringKey), serviceName, GetType());
                return stringKey;
            }

            return value;
        }
        
        public string GetSelectLanguage()
        {
            return selectLanguage;
        }

        private MultilanguageResultCode InitializeLanguageCode(MultilanguageXml.DefaultData defaultData)
        {
            string systemLanguageCode = MultilanguageUtil.GetSystemLanguage(false);

            if (IsSupportLanguage(systemLanguageCode) == true)
            {
                selectLanguage = systemLanguageCode;
            }
            else if (defaultData != null && IsSupportLanguage(defaultData.language) == true)
            {
                selectLanguage = defaultData.language;
            }
            else if (IsSupportLanguage(DEFAULT_LANGUAGE_CODE) == true)
            {
                selectLanguage = DEFAULT_LANGUAGE_CODE;
            }
            else if (languagePacks.Count > 0)
            {
                foreach (var key in languagePacks.Keys)
                {
                    selectLanguage = key;
                    break;
                }
            }
            else
            {
                return MultilanguageResultCode.LANGUAGE_LIST_EMPTY;
            }

            SelectLanguage(selectLanguage);

            return MultilanguageResultCode.SUCCESS;
        }

        private bool IsSupportLanguage(string languageCode)
        {
            return string.IsNullOrEmpty(languageCode) == false && languagePacks.ContainsKey(languageCode) == true;
        }
    }
}