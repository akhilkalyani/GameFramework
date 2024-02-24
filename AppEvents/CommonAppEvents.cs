using System;
using System.Collections;
using UnityEngine;

namespace GF
{
    public class ToastEvent : GameEvent
    {
        private string _message;
        public string Message { get { return _message; } }
        private float _duration;
        public float Duration { get { return _duration; } }
        public ToastEvent(string message,float duration)
        {
            _message = message;
            _duration = duration;
        }
    }
    public class UnloadingEvent:GameEvent
    {
        private Action _oncomplete;
        public Action OnComplete { get { return _oncomplete; } }
        public UnloadingEvent(Action action)
        {
            _oncomplete = action;
        }
    }
    public class LoadingEvent : GameEvent
    {
        private Action _oncomplete;
        public Action OnComplete { get { return _oncomplete; } }
        public LoadingEvent(Action action)
        {
            _oncomplete = action;
        }
    }
    public class AddServiceEvent : GameEvent
    {
        private Type _serviceType;
        public Type ServiceType { get { return _serviceType; } }
        public AddServiceEvent(Type serviceType)
        {
            _serviceType = serviceType;
        }
    }
    public class LoadingScreenCreated:GameEvent
    {
        private DefaultLoadingUI _defaultLoadingUI;
        public DefaultLoadingUI DefaultLoadingUI { get { return _defaultLoadingUI; } }
        public LoadingScreenCreated(DefaultLoadingUI defaultLoadingUI)
        {
            _defaultLoadingUI = defaultLoadingUI;
        }
    }
    public class ToastScreenCreated : GameEvent
    {
        private ToastMessgeUI _toastMessgeUI;
        public ToastMessgeUI ToastMessgeUI { get { return _toastMessgeUI; } }
        public ToastScreenCreated(ToastMessgeUI toastMessgeUI)
        {
            _toastMessgeUI = toastMessgeUI;
        }
    }
    public class SceneLoadingEvent : GameEvent
    {
        private int _sceneType;
        public int SceneType { get { return _sceneType; } }
        public SceneLoadingEvent(int scene)
        {
            _sceneType = scene;
        }
    }
    public class DownloadImageEvent : GameEvent
    {
        private string _url;
        private Action<Texture2D> _action;
        public string Url { get { return _url; } }
        public Action<Texture2D> Action { get{ return _action; } }
        public DownloadImageEvent(string url,Action<Texture2D> action)
        {
            _url = url;
            _action = action;
        }
    }
    public class CoroutineEvent:GameEvent
    {
        private IEnumerator _enumerator;
        public IEnumerator Enumerator { get { return _enumerator; } }
        public CoroutineEvent(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }
    }
    public class PlayAudioEvent : GameEvent
    {
        private AudioType _audioType;
        public AudioType AudioType { get { return _audioType; } }
        public PlayAudioEvent(AudioType audioType)
        {
            _audioType = audioType;
        }
    }
    public class RaiseWebApiEvent : GameEvent
    {
        private HttpRequestType _httpRequestType;
        private WebApiRequest _apiRequest;
        public WebApiRequest ApiRequest { get { return _apiRequest; } }
        public HttpRequestType HttpRequestType { get { return _httpRequestType; } }
        public Action<string, string, string> _responseCallback;
        public RaiseWebApiEvent(HttpRequestType httpRequestType,WebApiRequest apiRequest, Action<string, string, string> responseCallback)
        {
            _apiRequest = apiRequest;
            _responseCallback = responseCallback;
            _httpRequestType = httpRequestType;
        }
    }
    [System.Serializable]
    public class WebApiRequest
    {
        public RequestType requestType;
    }
}