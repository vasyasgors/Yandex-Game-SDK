using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace YandexGameSdk
{

    [DisallowMultipleComponent]
    public class YandexGamePlayer : YandexGameService
    {
        [DllImport("__Internal")] private static extern void YSDKInitPlayer();


        public event UnityAction<bool> Initialized;

        public UnityAction<bool> initialized;

        public bool IsAuth { get; private set; }

        private TaskCompletionSource<bool> playerInitTask;

        public void InitPlayer(UnityAction<bool> onInitialized = null)
        {
            initialized = onInitialized;

            if (yandexGame.UseRealAPI == true)
            {
                YSDKInitPlayer();
            }
            else
            {
                DebugMessage("Init player!");
                initialized?.Invoke(false);
                Initialized?.Invoke(false);
            }

        }

        public async Task<bool> InitPlayerAsync()
        {
            playerInitTask = new TaskCompletionSource<bool>();

            if (yandexGame.UseRealAPI == true)
            {
                YSDKInitPlayer();
            }
            else
            {
                DebugMessage("Init player!");
                playerInitTask.SetResult(true);
                return await playerInitTask.Task;
            }

            return await playerInitTask.Task;
        }

        public void OnPlayerNotAuth()
        {
            IsAuth = false;

            initialized?.Invoke(IsAuth);
            Initialized?.Invoke(IsAuth);
            playerInitTask.SetResult(IsAuth);
        }

        public void OnPlayerAuth()
        {
            IsAuth = true;
            initialized?.Invoke(IsAuth);
            Initialized?.Invoke(IsAuth);
            playerInitTask.SetResult(IsAuth);
        }

       


    }
}