using System;
using UnityEngine;
namespace GF
{
    /// <summary>
    /// Responsible for creating & registering all the services used in the game. 
    /// </summary>
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        private GameObject _loadingScreenGameobject;
        public GameObject LoadingScreenGameObject{get{return _loadingScreenGameobject;}}
        private GameServiceController _gameServiceController;
        private AudioManager _audioManager;
        protected override void Awake()
        {
            DontDestroyWhenLoad = true;
            ApplyHighlighter(Utils.GetColorByHashString("#8819CE"), Color.white);
            InitializeServices();
            base.Awake();
        }
        private void InitializeServices()
        {
            EventManager.Instance.Start();
            _gameServiceController = new GameServiceController();
            _gameServiceController.Initialize();
            _gameServiceController.RegisterListener();
        }
        public void SpawnLoadingScreen(string path)
        {
            _audioManager = AudioManager.Instance;
            if (string.IsNullOrEmpty(path)) return; 
            _loadingScreenGameobject = Instantiate(Resources.Load<GameObject>(path),transform);
            _loadingScreenGameobject.name = $"DefaultLoadingUI";
            Utils.RaiseEventAsync(new LoadingScreenCreated(_loadingScreenGameobject.GetComponent<DefaultLoadingUI>()));
        }
        public void AddService<T>()
        {
            Utils.RaiseEventAsync(new AddServiceEvent(typeof(T)));
        }
        public void RemoveService<T>()
        {
            Utils.RaiseEventAsync(new RemoveServiceEvent(typeof(T)));
        }
        protected override void OnApplicationQuit()
        {
            EventManager.Instance.ReleaseEvents();
            _gameServiceController.RemoveListener();
            base.OnApplicationQuit();
        }
    }
}
