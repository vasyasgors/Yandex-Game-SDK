using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace YandexGameSdk
{
    [DisallowMultipleComponent]
    public class YandexGamePlayerProgress : YandexGameService
    {
        private const string ProgressPlayerPrefsKey = "YandexGamePlayerProgress";

        [DllImport("__Internal")] private static extern void YSDKSavePlayerProgress(string data);
        [DllImport("__Internal")] private static extern void YSDKLoadPlayerProgress();


        public UnityAction<string> cachedProgressLoaded;

        private TaskCompletionSource<string> progressLoadTask;

        public void OnPlayerProgressLoaded(string progress)
        {

            DebugMessage("Progress loaded from yandex " + progress);


            cachedProgressLoaded?.Invoke(progress);
            progressLoadTask.SetResult(progress);

            cachedProgressLoaded = null;
        }

        public void Save(string progress)
        {
            if(yandexGame.UseRealAPI == true)
            {
                YSDKSavePlayerProgress(progress);
            }
            else
            {
                PlayerPrefs.SetString(ProgressPlayerPrefsKey, progress);
            }
        }

        public void Load(UnityAction<string> onLoaded = null)
        {
            if (yandexGame.UseRealAPI == true)
            {
                cachedProgressLoaded = onLoaded;
                YSDKLoadPlayerProgress();
            }
            else
            {
                cachedProgressLoaded?.Invoke(PlayerPrefs.GetString(ProgressPlayerPrefsKey));
            }

         
        }

        public async Task<string> LoadAsync()
        {
            if (yandexGame.UseRealAPI == true)
            {
                YSDKLoadPlayerProgress();

                progressLoadTask = new TaskCompletionSource<string>();

                return await progressLoadTask.Task;
            }
            else
            {
                return PlayerPrefs.GetString(ProgressPlayerPrefsKey);
            }
        }

    }
}