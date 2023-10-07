using Gpm.Manager.Ad.Internal;
using UnityEditor;
using UnityEngine;

namespace Gpm.Manager.Ad
{
    internal static class Advertisement
    {
        public static void Initialize(EditorWindow window, Rect drawRect, AdvertisementConfigurations advertisementInfo, string languageCode)
        {
            AdvertisementImplementation.Instance.Initialize(window, drawRect, advertisementInfo, languageCode);
        }

        public static void Draw()
        {
            AdvertisementImplementation.Instance.Draw();
        }
        
        public static void SetSelectAdvertisementInfoCallback(System.Action<string, string> selectAdvertisementInfoCallback)
        {
            AdvertisementImplementation.Instance.SetSelectAdvertisementInfoCallback(selectAdvertisementInfoCallback);
        }

        public static void SetLanguageCode(string languageCode)
        {
            AdvertisementImplementation.Instance.SetLanguageCode(languageCode);
        }

        public static void OnDestroy()
        {
            AdvertisementImplementation.Instance.OnDestroy();
        }
    }
}
