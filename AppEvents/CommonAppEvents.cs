using System;
using System.Collections;
using System.Reflection;
using Netconfig;
using toolcity.DataManager;
using ToolCityGame.Assets.Scripts.API;
using Unity.VisualScripting;
using UnityEngine;

namespace GF
{
    public class YesNoPopupEvent : GameEvent
    {
        public Action yesAction;
        public Action noAction;
        public string title;
        public string info;
        public YesNoPopupEvent(string title, string info, Action yes, Action no)
        {
            this.yesAction = yes;
            this.noAction = no;
            this.title = title;
            this.info = info;
        }
    }

    public class OkPopupEvent : GameEvent
    {
        public Action okAction;
        public string title;
        public string info;
        public OkPopupEvent(string title, string info, Action ok)
        {
            this.title = title;
            this.info = info;
            this.okAction = ok;
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
    public class ApiEvent : GameEvent
    {
        public Request Request;
        public HttpRequestType HttpRequestType;
        public string url;
        public Action<IResponse> ResponseCallback;
        public ApiEvent(Request apiRequest, Action<IResponse> responseCallback)
        {
            Request = apiRequest;
            ResponseCallback = responseCallback;
            url = BuildUrl(apiRequest.requestType);
        }
        private string BuildUrl(RequestType e)
        {
            var result = ServerConfig.Instance.GetApiUrl(e);
            string url = result.Item1;
            HttpRequestType = result.Item2;
            switch (e)
            {
                case RequestType.GetFriends:
                case RequestType.GetGlobalLeaderboard:
                case RequestType.GetWeekLeaderboard:
                case RequestType.Logout:
                case RequestType.DeleteAccount:
                    return url + UserDataManager.Instance.UserID;
            }
            return url;
        }
    }

    public class RegisterCustomServiceEvent : GameEvent
    {
        public string NameSpaceType;
        private Assembly assembly;
        public Assembly Assembly => string.IsNullOrEmpty(assemblyName) ? Assembly.GetExecutingAssembly() : Assembly.Load(assemblyName);
        public string assemblyName;
        public RegisterCustomServiceEvent(string nameSpaceType, string assembly)
        {
            this.NameSpaceType = nameSpaceType;
            this.assemblyName = assembly;
        }
    }
}