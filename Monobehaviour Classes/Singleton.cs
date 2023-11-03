using JetBrains.Annotations;
using UnityEngine;
namespace GF
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [CanBeNull] private static T _instance;
        [NotNull] private static readonly object Lock = new object();

        protected bool dontDestroyedOnLoad;
        public static bool Quitting { get; private set; }

        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    Debug.LogWarning($"({nameof(Singleton<T>)}<{typeof(T)}>) Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }
                lock (Lock)
                {
                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    if (count> 0)
                    {
                        if (count == 1)
                        {
                            return _instance = instances[0];
                        }
                        else
                        {
                            Debug.LogWarning($"({nameof(Singleton<T>)}<{typeof(T)}>) There should never be more than one {nameof(Singleton<T>)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                            for (var i = 1; i < instances.Length; i++)
                                Destroy(instances[i]);
                            return _instance = instances[0];
                        }
                    }
                    Console.Log(LogType.Log,$"[{nameof(Singleton<T>)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return _instance = new GameObject($"(Singleton){typeof(T)}").AddComponent<T>();
                }
            }
        }

        protected virtual void Awake()
        {
            if (dontDestroyedOnLoad)
                DontDestroyOnLoad(this);
        }

        protected void ApplyHighlighter(Color bg, Color text)
        {
            gameObject.AddComponent<HierarchyHighlighter>();
            HierarchyHighlighter hr = GetComponent<HierarchyHighlighter>();
            hr.Background_Color = bg;
            hr.Text_Color = text;
            hr.TextStyle = FontStyle.BoldAndItalic;
        }
        protected virtual void OnApplicationQuit()
        {
            Quitting = true;
        }

    }
}