using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace YandexGameSdk
{
    [DisallowMultipleComponent]
    public class YandexGame : YandexGameService
    {
        [SerializeField] private YandexGameAdvertisement yandexGameAdvertisement;
        [SerializeField] private YandexGamePayments yandexGamePayments;

        [DllImport("__Internal")] private static extern bool YSDKIsMobile();
        [DllImport("__Internal")] private static extern bool YSDKInit();
        [DllImport("__Internal")] private static extern long YSDKGetServerTimeMillisecond();

        public event UnityAction PageVisibilityOn;
        public event UnityAction PageVisibilityOff;

        public bool IsMobile => CheckForMobile();

        private TaskCompletionSource<bool> yandexInitTask;
        private TaskCompletionSource<bool> unityInstanceInit;

        private bool CheckForMobile()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                return YSDKIsMobile();
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> InitAsync()
        {
            yandexInitTask = new TaskCompletionSource<bool>();
            unityInstanceInit = new TaskCompletionSource<bool>(); 

           
            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                await unityInstanceInit.Task;

                YSDKInit();
            }
            else
            {
                DebugMessage("Yandex sdk inited!");
                yandexInitTask.SetResult(true);
                unityInstanceInit.SetResult(true);
                return await yandexInitTask.Task;
            }

            return await yandexInitTask.Task;

        }
        
        public long GetServerTimeMillisecond()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                return YSDKGetServerTimeMillisecond();
            }
            else
            {
                return System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }

        public void StopGame()
        {
            AudioListener.pause = true;
            Time.timeScale = 0;
        }

        public void ContinueGame()
        {
            AudioListener.pause = false;
            Time.timeScale = 1;
        }

        public void OnGameInstanceInit()
        {
            unityInstanceInit.SetResult(true);
        }

        public void OnYandexSDKInit()
        {
            yandexInitTask.SetResult(true);
        }

        public void OnPageVisibilityOff()
        {
            StopGame();
            PageVisibilityOff?.Invoke();
        }

        public void OnPageVisibilityOn()
        {
            if (yandexGameAdvertisement.IsRewardedShowed == true) return;

            ContinueGame();
            PageVisibilityOn?.Invoke();
        }



    }
}