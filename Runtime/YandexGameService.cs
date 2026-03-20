using UnityEngine;

namespace YandexGameSdk
{
    [RequireComponent(typeof(YandexGame))]
    public abstract class YandexGameService : MonoBehaviour
    {
        protected YandexGame yandexGame;

        private void Awake()
        {
            yandexGame = GetComponent<YandexGame>();      
        }

        protected void DebugMessage(object message)
        {
            if (yandexGame.Config.Debug == true)
                Debug.Log("[Yandex Game SDK C#]: " + message); 
        }
    }

}