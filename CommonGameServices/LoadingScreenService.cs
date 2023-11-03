using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GF {

    public class LoadingScreenService : IService
    {
        private DefaultLoadingUI _defaultLoadingUI;
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
            _defaultLoadingUI = e.DefaultLoadingUI;
            EventManager.Instance.AddListener<LoadingEvent>(OpenLoadingScreen);
            EventManager.Instance.AddListener<UnloadingEvent>(CloseLoadingScreen);
            EventManager.Instance.AddListener<SceneLoadingEvent>(LoadScene);
        }

        private void LoadScene(SceneLoadingEvent e)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(LoadSceneAsync((int)e.SceneType)));
        }

        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            yield return _defaultLoadingUI.ShowLoadingScreenCoroutine();
            AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(sceneIndex);
            while (!sceneOperation.isDone)
            {
                _defaultLoadingUI.UpdateProgress(sceneOperation.progress);
                yield return null;
            }
            yield break;
        }

        private void OpenLoadingScreen(LoadingEvent e)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(_defaultLoadingUI.ShowLoadingScreenCoroutine()));
        }

        private void CloseLoadingScreen(UnloadingEvent e)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(_defaultLoadingUI.CloseLoadingScreenCoroutine()));
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<LoadingScreenCreated>(InitializeLoadingScreen);
            EventManager.Instance.RemoveListener<LoadingEvent>(OpenLoadingScreen);
            EventManager.Instance.RemoveListener<UnloadingEvent>(CloseLoadingScreen);
            EventManager.Instance.RemoveListener<SceneLoadingEvent>(LoadScene);
        }
    }
}
