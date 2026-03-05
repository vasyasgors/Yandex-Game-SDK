using UnityEngine;

namespace YandexGameSdk
{
    [CreateAssetMenu(fileName = "YandexGameConfig", menuName = "Configs/YandexGameConfig")]
    public class YandexGameConfig : ScriptableObject
    {
        public bool BuildForYandexGame = true;
        public bool Debug = true;
        public bool UseInAppPurchases = false;

    }
}

