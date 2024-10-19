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
        public ToastEvent(string message, float duration)
        {
            SetIsDone(false);
            _message = message;
            _duration = duration;
        }
    }
    public class UnloadingEvent : GameEvent
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
    public class LoadingScreenCreated : GameEvent
    {
        private DefaultLoadingUI _defaultLoadingUI;
        public DefaultLoadingUI DefaultLoadingUI { get { return _defaultLoadingUI; } }
        public LoadingScreenCreated(DefaultLoadingUI defaultLoadingUI)
        {
            _defaultLoadingUI = defaultLoadingUI;
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
    public class DownlaodAudioEvent : GameEvent
    {
        private string _url;
        private Action<AudioClip> _callback;
        public string Url { get => _url; }
        private Action<float> _progressCallback;
        public Action<AudioClip> Callback { get => _callback; }
        public Action<float> ProgressCallback { get => _progressCallback; }
        public DownlaodAudioEvent(string url, Action<AudioClip> callback, Action<float> progressCallback)
        {
            _url = url;
            _callback = callback;
            _progressCallback = progressCallback;
        }
    }
    public class DownloadImageEvent : GameEvent
    {
        private string _url;
        private Action<Texture2D> _action;
        public string Url { get { return _url; } }
        public Action<Texture2D> Action { get { return _action; } }
        public DownloadImageEvent(string url, Action<Texture2D> action)
        {
            _url = url;
            _action = action;
        }
    }
    public class CoroutineEvent : GameEvent
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
        private Audio_type audio;
        private AudioClip _audioClip;
        public AudioClip AudioClip { get { return _audioClip; } }
        public Audio_type Audio_Type { get { return audio; } }
        public PlayAudioEvent(Audio_type SoundType, AudioClip audioClip)
        {
            audio = SoundType;
            _audioClip = audioClip;
        }
    }
    public class RaiseWebApiEvent : GameEvent
    {
        private HttpRequestType _httpRequestType;
        private WebApiRequest _apiRequest;
        public WebApiRequest ApiRequest { get { return _apiRequest; } }
        public HttpRequestType HttpRequestType { get { return _httpRequestType; } }
        public Action<string, string, string> _responseCallback;
        public RaiseWebApiEvent(HttpRequestType httpRequestType, WebApiRequest apiRequest, Action<string, string, string> responseCallback)
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
    public class CreateToastEvent : GameEvent
    {
        private ITaost taostInstance;
        public ITaost TaostInstance { get { return taostInstance; } }
        public CreateToastEvent(ITaost taostInstance)
        {
            this.taostInstance = taostInstance;
        }
    }
    public class ToastShowEvent : GameEvent
    {
        private string message;
        private float duration;
        public string Message { get { return message; } }

        public float Duration { get { return duration; } }


        public ToastShowEvent(string message)
        {
            this.message = message;
        }
    }
}