using System;
using System.Collections.Generic;
using UnityEngine;
namespace GF
{
    public abstract class SceneHandler<T> : MonoBehaviour where T:Enum
    {
        [Header("UI canvas prefab")]
        public string UIprefabPath;
        [Header("LoadingScreen prefab path")]
        public string LoadingScreenPath;
        [Header("ToastScreen prefab path")]
        public string ToastScreenPath;
        protected GameObject GUI = null;
        private readonly Dictionary<T, BaseScreen<T>> _screensDictionary = new Dictionary<T, BaseScreen<T>>();
        public T StartScreen;
        protected BaseScreen<T> currentActiveScreen;
        private static SceneHandler<T> instance;
        public static SceneHandler<T> Instance=>instance;
        protected virtual void Awake()
        {
            instance = this;
            ApplicationManager.Instance.SpawnLoadingAndToastScreen(LoadingScreenPath,ToastScreenPath);
        }
        protected virtual void Start()
        {
            RegisterListener();
            if (IsScreensPrepared())
            {
                ApplyHighlighter(Utils.GetColorByHashString("#E20D6B"), Color.white," GUI");
                var screens = GUI.GetComponentsInChildren<BaseScreen<T>>(true);
                for (int i = 0; i < screens.Length; i++)
                {
                    _screensDictionary.Add(screens[i].Screen, screens[i]);
                    screens[i].gameObject.SetActive(false);
                }
                currentActiveScreen = GetScreen(StartScreen);
                currentActiveScreen.OpenScreen();
            }
            RegisterServices();
        }
        private void RegisterListener()
        {
            
        }

        public void ChangeScreen(T screen)
        {
            var nextScreen = GetScreen(screen);
            if (nextScreen != null)
            {
                nextScreen.OpenScreen();
                currentActiveScreen.CloseScreen();
                currentActiveScreen = nextScreen;
            }
        }
        private bool IsScreensPrepared()
        {
            if(string.IsNullOrEmpty(UIprefabPath))
            {
                Debug.LogWarning("Specify Main UI Canvas parent prefab path in the Inspector.");
                return false;
            }
            else
            {
                GUI = Instantiate(Resources.Load<GameObject>(UIprefabPath));
                GUI.AddComponent<HierarchyHighlighter>();
                Canvas can = GUI.GetComponent<Canvas>();
                can.renderMode = RenderMode.ScreenSpaceCamera;
                can.worldCamera = Camera.main;
                return true;
            }    
        }
        protected void ApplyHighlighter(Color bg, Color text, string name)
        {
            GUI.name =$"{GUI.name}-->{name}";
            HierarchyHighlighter hr = GUI.GetComponent<HierarchyHighlighter>();
            hr.Background_Color = bg;
            hr.Text_Color = text;
            hr.TextStyle = FontStyle.BoldAndItalic;
        }
        protected BaseScreen<T> GetScreen(T screen)
        {
            if (_screensDictionary.TryGetValue(screen, out BaseScreen<T> val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }
        private void RemoveListener()
        {
            
        }
        private void OnDisable()
        {
            RemoveListener();
        }
        protected abstract void RegisterServices();
    }
}
