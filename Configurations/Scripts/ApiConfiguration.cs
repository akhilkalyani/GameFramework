using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace GF
{
    [CreateAssetMenu(fileName = "ApiConfiguration", menuName = "ScriptableObjects/ApiConfiguration", order = 1)]
    public class ApiConfiguration : ScriptableObject
    {
        [Serializable]
        public class Api
        {
            public RequestType requestType;
            public string api;
        }
        public string BaseUrl;
        public List<Api> apis;
        public string GetApiUrl(RequestType requestType)
        {
            string url = null;
            url = Path.Combine(BaseUrl, apis.Find(apiItem => apiItem.requestType.Equals(requestType)).api);
            return url;
        }
    }
}
