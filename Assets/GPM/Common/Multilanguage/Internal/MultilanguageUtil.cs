using System.Globalization;
using UnityEngine;

namespace Gpm.Common.Multilanguage.Internal
{
    internal static class MultilanguageUtil
    {
        public static string GetSystemLanguage(bool isNativeName)
        {
            SystemLanguage language = Application.systemLanguage;

            CultureInfo findCulture = null;

            // UnityEngine.SystemLanguage와 System.Globalization.CultureInfo와 매칭이 필요한데 매칭이 되지 않는 예외 사항 처리
            switch (language)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    {
                        findCulture = CultureInfo.GetCultureInfo("zh-CN");
                        break;
                    }
                case SystemLanguage.ChineseTraditional:
                    {
                        findCulture = CultureInfo.GetCultureInfo("zh-TW");
                        break;
                    }
                case SystemLanguage.SerboCroatian:
                    {
                        // Serbo-Croatian에는 4개의 표준언어 Serbian, Croatian, Bosnian, Montenegrin가 있음
                        // .NET에서는 표준 언어를 선택해야 하기에 대표적으로 Serbian(sr)을 선택하여 진행
                        findCulture = CultureInfo.GetCultureInfo("sr");
                        break;
                    }
                case SystemLanguage.Unknown:
                    {
                        break;
                    }
                default:
                    {
                        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

                        foreach (var culture in cultures)
                        {
                            if (culture.EnglishName.Equals(language.ToString()) == true)
                            {
                                findCulture = culture;
                                break;
                            }
                        }

                        break;
                    }
            }

            if (findCulture == null)
            {
                return string.Empty;
            }

            return isNativeName == true ? findCulture.NativeName : findCulture.Name;
        }
    }
}
