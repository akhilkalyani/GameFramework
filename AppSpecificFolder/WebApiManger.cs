using System;
using Newtonsoft.Json;
using UnityEngine;

namespace GF
{
    public static class WebApiManger
    {
        public static void CallApi<T>(HttpRequestType httpRequestType,WebApiRequest apiRequest,Action<T,string> callback) where T : class
        {
            Utils.CallEventAsync(new RaiseWebApiEvent(httpRequestType, apiRequest, (req, res, error) =>
            {
                GF.Console.Log(LogType.HttpResponse, $"req : {res}");
                callback?.Invoke(JsonConvert.DeserializeObject<T>(res),error);
            }));
        }
        public static void DownloadImage(string url,Action<Texture2D> completeAction)
        {
            Utils.CallEventAsync(new DownloadImageEvent(url, completeAction));
        }
    }
}
