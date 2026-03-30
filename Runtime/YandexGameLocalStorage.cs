using UnityEngine;
using System.Runtime.InteropServices;
using YandexGameSdk;


namespace YandexGameSdk
{

    public class YandexGameLocalStorage : YandexGameService
    {
        [DllImport("__Internal")] public static extern void SetKey_LocalStorage_js(string key, string value);
        [DllImport("__Internal")] public static extern void DeleteKey_LocalStorage_js(string key);
        [DllImport("__Internal")] public static extern void ClearAllKeys_LocalStorage_js();
        [DllImport("__Internal")] public static extern string GetKey_LocalStorage_js(string key);
        [DllImport("__Internal")] private static extern int HasKey_LocalStorage_js(string key);

        public void SetKey(string key, string value)
        {
            if (yandexGame.UseRealAPI == true)
            {
                SetKey_LocalStorage_js(key, value);
            }
            else
            {
                PlayerPrefs.SetString(key, value);
                PlayerPrefs.Save();
            }
        }

        public string GetKey(string key, string defaultValue = "")
        {
            if (!HasKey(key))
                return defaultValue;

            if (yandexGame.UseRealAPI == true)
            {
                return GetKey_LocalStorage_js(key);
            }
            else
            {
                return PlayerPrefs.GetString(key);
            }
        }

        public bool HasKey(string key)
        {
            if (yandexGame.UseRealAPI == true)
            {
                try
                {
                    return HasKey_LocalStorage_js(key) == 1;
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                    return false;
                }
            }
            else
            {
                return PlayerPrefs.HasKey(key);
            }
        }


        public void DeleteKey(string key)
        {
            if (yandexGame.UseRealAPI == true)
            {
                DeleteKey_LocalStorage_js(key);
            }
            else
            {
                PlayerPrefs.DeleteKey(key);
            }
        }


        public void DeleteAll()
        {
            if (yandexGame.UseRealAPI == true)
            {
                ClearAllKeys_LocalStorage_js();
            }
            else
            {
                PlayerPrefs.DeleteAll();
            }
        }
    }
}

