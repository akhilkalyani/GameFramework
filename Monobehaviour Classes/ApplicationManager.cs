using System;
using UnityEngine;
namespace GF
{
    /// <summary>
    /// Responsible for creating & registering all the services used in the game. 
    /// </summary>
    public class ApplicationManager : Singleton<ApplicationManager>
    {
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
        public void SpawnLoadingAndToastScreen(string loadingScreenpath,string toastScreenPath)
        {
            _audioManager = AudioManager.Instance;
            if (string.IsNullOrEmpty(loadingScreenpath)) return; 
            var _loadingScreenGameobject = Instantiate(Resources.Load<DefaultLoadingUI>(loadingScreenpath),transform);
            _loadingScreenGameobject.name = $"DefaultLoadingUI";
            Utils.CallEventAsync(new LoadingScreenCreated(_loadingScreenGameobject));
            if(string.IsNullOrEmpty(toastScreenPath)) return;
            var toastScreen = Instantiate(Resources.Load<ToastMessgeUI>(toastScreenPath),transform);
            toastScreen.name = $"DefaultToastUI";
            Utils.CallEventAsync(new ToastScreenCreated(toastScreen));
        }
        public void AddService<T>()
        {
            Utils.CallEventAsync(new AddServiceEvent(typeof(T)));
        }
        protected override void OnApplicationQuit()
        {
            _gameServiceController.RemoveListener();
            EventManager.Instance.ReleaseEvents();
            base.OnApplicationQuit();
        }
    }
}
