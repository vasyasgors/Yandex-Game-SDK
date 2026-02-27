
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace YandexGameSdk
{

    [DisallowMultipleComponent]
    public class YandexGameAdvertisement : YandexGameService
    {
        private const float FullscreenAdInterval = 60;

  

        [DllImport("__Internal")] private static extern void YSDKShowFullscreenAd();
        [DllImport("__Internal")] private static extern void YSDKShowRewardedAd();

        [DllImport("__Internal")] private static extern void YSDKShowStickyBanner();
        [DllImport("__Internal")] private static extern void YSDKHideStickyBanner();

        private event UnityAction cachedFullscreenClosed;
        private event UnityAction cachedRewardedClosed;
        private event UnityAction cachedRewardedRewarded;

        private DateTime lastTimeFullscreenAd;

        public bool IsShowFullscreenAd = true;
        public bool IsShowRewardedAd = true;
        public bool IsShowStickydBanner = true;

        public bool IsRewardedShowed { get; private set; }

        public void OnRewardedAdOpen()
        {
            IsRewardedShowed = true;
        }

        public void OnRewardedAdRewarded()
        {
            yandexGame.ContinueGame();

            cachedRewardedRewarded?.Invoke();

            cachedRewardedRewarded = null;
            IsRewardedShowed = false;
        }

        public void OnRewardedAdClose()
        {
            yandexGame.ContinueGame();

            cachedRewardedClosed = null;
            IsRewardedShowed = false;
        }

        public void OnRewardedAdError()
        {
            yandexGame.ContinueGame();

            cachedRewardedClosed = null;
            cachedRewardedRewarded = null;
        }

        public void OnFullscreenAdOpen()
        {
            lastTimeFullscreenAd = DateTime.Now;

            DebugMessage("On Fullscreen Ad Open");
        }

        public void OnFullscreenAdClosed()
        {
            yandexGame.ContinueGame();

            cachedFullscreenClosed?.Invoke();
            cachedFullscreenClosed = null;

            DebugMessage("On Fullscreen Ad Closed");
        }

        public void OnFullscreenAdError()
        {
            yandexGame.ContinueGame();

            cachedFullscreenClosed?.Invoke();

            cachedFullscreenClosed = null;

            DebugMessage("On Fullscreen Ad Error");
        }

        // Переписать аккуратнее 

        public void ShowStickyBanner()
        {
            if (IsShowStickydBanner == false) return;

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKShowStickyBanner();
            }
            else
            {
                DebugMessage("Show sticky banner");
            }

        }

        public void HideStickyBanner()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKHideStickyBanner();
            }
            else
            {
                DebugMessage("Hide sticky banner");
            }

            
        }

        public void ShowFullscreenAd(UnityAction onClosed = null)
        {
            if(IsShowFullscreenAd == false)
            {
                onClosed?.Invoke();
                return;
            }

            yandexGame.StopGame();


            if (DateTime.Now <= lastTimeFullscreenAd.AddSeconds(FullscreenAdInterval))
            {
                onClosed?.Invoke();
                DebugMessage("Advertising frequency exceeded");

                yandexGame.ContinueGame();
                return;
            }

            lastTimeFullscreenAd = DateTime.Now;
            cachedFullscreenClosed = onClosed;



            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKShowFullscreenAd();
            }
            else
            {

                cachedFullscreenClosed?.Invoke();
                yandexGame.ContinueGame();

                DebugMessage("Show Fullscreen Ad");

            }
        }

        public void ShowRewardedAd(UnityAction onRewarded = null, UnityAction onClosed = null)
        {
            if (IsShowRewardedAd == false)
            {
                onRewarded?.Invoke();
                return;
            }

            cachedRewardedRewarded = onRewarded;
            cachedRewardedClosed = onClosed;

            yandexGame.StopGame();

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKShowRewardedAd();
            }
            else
            {
                yandexGame.ContinueGame();

                cachedRewardedClosed?.Invoke();
                cachedRewardedRewarded?.Invoke();
                DebugMessage("Show Rewarded Ad");
            }
        }



    }
}