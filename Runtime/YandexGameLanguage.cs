using System.Runtime.InteropServices;
using UnityEngine;

namespace YandexGameSdk
{
    [DisallowMultipleComponent]
    public class YandexGameLanguage : YandexGameService
    {

        [DllImport("__Internal")] private static extern string YSDKGetLanguage();


        public string GetLanguageCode()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                return YSDKGetLanguage();
            }

            return "ru";
        }
    
    }
}