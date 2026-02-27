using System.Runtime.InteropServices;
using UnityEngine;

namespace YandexGameSdk
{
    [DisallowMultipleComponent]
    public class YandexGameGameAPI : YandexGameService
    {

        [DllImport("__Internal")] private static extern void YSDKGameplayAPIReady();
        [DllImport("__Internal")] private static extern void YSDKGameplayAPIStart();
        [DllImport("__Internal")] private static extern void YSDKGameplayAPIStop();


        private bool isGameReady = false;
        private bool isGameplayStarted = false; // продумать название


        private void Start()
        {
            yandexGame.PageVisibilityOn += OnPageVisibilityOn;
            yandexGame.PageVisibilityOff += OnPageVisibilityOff;
        }

        private void OnDestroy()
        {
            yandexGame.PageVisibilityOn -= OnPageVisibilityOn;
            yandexGame.PageVisibilityOff -= OnPageVisibilityOff;
        }

        public void OnPageVisibilityOff()
        {
            if (isGameplayStarted == false)
            {
                GameplayStop();
            }
        }

        public void OnPageVisibilityOn()
        {
            if (isGameplayStarted == true)
            {
                GameplayStart();
            }
        }

        public void GameplayReady()
        {
            if (isGameReady == true) return;

            isGameReady = true;

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
                YSDKGameplayAPIReady();
            else
                DebugMessage("Gameplay ready!");
        }

        public void GameplayStart()
        {
            if (isGameplayStarted == true) return;

            isGameplayStarted = true;

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
                YSDKGameplayAPIStart();
            else
                DebugMessage("Gameplay start!");
        }

        public void GameplayStop()
        {
            if (isGameplayStarted == false) return;

            isGameplayStarted = false;

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
                YSDKGameplayAPIStop();
            else
                DebugMessage("Gameplay stop!");
        }
    }
}