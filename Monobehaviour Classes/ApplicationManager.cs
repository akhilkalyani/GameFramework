using UnityEngine;
namespace GF
{
    /// <summary>
    /// Responsible for creating & registering all the services used in the game. 
    /// </summary>
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        private GameServiceController _gameServiceController;
        protected override void Awake()
        {
            DontDestroyWhenLoad = true;
            ApplyHighlighter(Utils.GetColorByHashString("#8819CE"), Color.white);
            InitializeServices();
            base.Awake();
        }
        private void InitializeServices()
        {            
            _gameServiceController = new GameServiceController();
            _gameServiceController.Initialize();
            _gameServiceController.RegisterListener();
        }
        public void SpawnLoadingAndToastScreen(string loadingScreenpath)
        {
            if (!string.IsNullOrEmpty(loadingScreenpath))
            {
                var _loadingScreenGameobject = Instantiate(Resources.Load<DefaultLoadingUI>(loadingScreenpath),transform);
                _loadingScreenGameobject.name = $"DefaultLoadingUI";
                Utils.CallEventAsync(new LoadingScreenCreated(_loadingScreenGameobject));
            }
        }
        public void AddService<T>()
        {
            _gameServiceController.AddService(typeof(T));
        }
        public void RegisterAppEvent<T>(EventListener.EventHandler<T> eventHandler) where T : GameEvent
        {
            EventManager.Instance.AddListener(eventHandler);
        }
        public void RemoveAppEvent<T>(EventListener.EventHandler<T> eventHandler) where T : GameEvent
        {
            EventManager.Instance.RemoveListener(eventHandler);
        }
        private void Update()
        {
            _gameServiceController.Update();
            EventManager.Instance.Update();
        }
        protected override void OnApplicationQuit()
        {
            _gameServiceController.RemoveListener();
            EventManager.Instance.ReleaseEvents();
            base.OnApplicationQuit();
        }
    }
}
