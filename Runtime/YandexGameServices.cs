using UnityEngine;

namespace YandexGameSdk
{
    public class YandexGameServices : MonoBehaviour
    {
        
        public YandexGame Game { get; private set; }
        public YandexGamePlayer Player { get; private set; }
        public YandexGameGameAPI GameAPI { get; private set; }
        public YandexGamePlayerProgress Progress { get; private set; }
        public YandexGameAdvertisement Advertisement { get; private set; }
        public YandexGameLanguage Language { get; private set; }
        public YandexGamePayments Payments { get; private set; }

        public void CollectServices()
        {
            Game = GetComponent<YandexGame>();
            Player = GetComponent<YandexGamePlayer>();
            GameAPI = GetComponent<YandexGameGameAPI>();
            Progress = GetComponent<YandexGamePlayerProgress>();
            Advertisement = GetComponent<YandexGameAdvertisement>();
            Language = GetComponent<YandexGameLanguage>();
            Payments = GetComponent<YandexGamePayments>();
        }
    }

}