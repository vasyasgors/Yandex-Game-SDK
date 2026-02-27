using UnityEngine;

namespace YandexGameSdk
{
    [RequireComponent(typeof(YandexGame))]
    public abstract class YandexGameService : MonoBehaviour
    {
        [SerializeField] protected YandexGameConfig config;

        protected YandexGame yandexGame;

        private void OnEnable()
        {
            yandexGame = GetComponent<YandexGame>();
        }

        protected void DebugMessage(object message)
        {
            if (config.Debug == true)
                Debug.Log("[Yandex Game SDK C#]: " + message); 
        }
    }

}