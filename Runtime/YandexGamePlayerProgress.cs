using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace YandexGameSdk
{
    [DisallowMultipleComponent]
    public class YandexGamePlayerProgress : YandexGameService
    {
        [DllImport("__Internal")] private static extern void YSDKSavePlayerProgress(string data);
        [DllImport("__Internal")] private static extern void YSDKLoadPlayerProgress();


        public UnityAction<string> cachedProgressLoaded;

        private TaskCompletionSource<string> progressLoadTask;

        public void OnPlayerProgressLoaded(string progress)
        {
            DebugMessage("Progress loaded from yandex " + progress);

            //  возвращает "{}" если прогресса у игрока нет. Нужно придумать, что можно сделать
            cachedProgressLoaded?.Invoke(progress);
            progressLoadTask.SetResult(progress);

            cachedProgressLoaded = null;
        }

        public void Save(string progress)
        {
            YSDKSavePlayerProgress(progress);
        }

        public void Load(UnityAction<string> onLoaded = null)
        {
            cachedProgressLoaded = onLoaded;
            YSDKLoadPlayerProgress();
        }

        public async Task<string> LoadAsync()
        {
            YSDKLoadPlayerProgress();

            progressLoadTask = new TaskCompletionSource<string>();

            return await progressLoadTask.Task;
        }

    }
}