using System;
using System.Collections;
using UnityEngine;

namespace GF {

    public class LoadingScreenService : IService
    {
        private GameObject _loadingState;
        private GameObject _idleState;
        private GameObject _unloadingState;
        public void Initialize()
        {
            //assign all states here.
            Console.Log(LogType.Log, "LoadingScreenService created");
        }

        public void RegisterListener()
        {
            EventManager.Instance.AddListener<LoadingScreenCreated>(InitializeLoadingScreen);
        }

        private void InitializeLoadingScreen(LoadingScreenCreated e)
        {
            EventManager.Instance.AddListener<LoadingEvent>(OpenLoadingScreen);
            EventManager.Instance.AddListener<UnloadingEvent>(CloseLoadingScreen);
        }

        private void OpenLoadingScreen(LoadingEvent e)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(ShowLoadingScreenCoroutine()));
        }

        private IEnumerator ShowLoadingScreenCoroutine()
        {
            yield break;
        }

        private void CloseLoadingScreen(UnloadingEvent e)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(CloseLoadingScreenCoroutine()));
        }

        private IEnumerator CloseLoadingScreenCoroutine()
        {
            yield break;
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<LoadingScreenCreated>(InitializeLoadingScreen);
        }
    }
}
