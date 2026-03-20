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
            if (yandexGame.UseRealAPI == true)
            {
                return YSDKGetLanguage();
            }

            return "ru";
        }
    
    }
}