using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Netconfig;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using ToolCityGame.Assets.Scripts.API;
using toolcity.DataManager;

namespace GF
{
    public class UnityWebService : IService
    {
        private bool isUpdateRequired = false;
        public class Request
        {
            public int RequestID;
            public Request(int id)
            {
                RequestID = id;
            }
        }
        private string cashedImageUrl;
        private int imageRequestCount = 0;
        private int imageMaxRequest = 3;
        private float maxWaitDuration = 10f;
        private Queue<Request> imageRequestQueue = new Queue<Request>();
        private string BearerTokenKey = "BearerToken";
        public bool IsUpdateRequired => isUpdateRequired;
        public void Initialize()
        {
            Logger.Log(LogType.Log, "UnityWebService created");
            if (!PlayerPrefs.HasKey(BearerTokenKey))
            {
                PlayerPrefs.SetString(BearerTokenKey, "");
            }
            cashedImageUrl = Application.persistentDataPath + "/CashedImages/";
            if (!Directory.Exists(cashedImageUrl))
            {
                Directory.CreateDirectory(cashedImageUrl);
            }
        }


        public void RegisterListener()
        {
            EventManager.Instance.AddListener<DownloadImageEvent>(DownloadImage);
            EventManager.Instance.AddListener<ApiEvent>(OnApiRequest);
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<DownloadImageEvent>(DownloadImage);
            EventManager.Instance.RemoveListener<ApiEvent>(OnApiRequest);
        }

        private void OnApiRequest(ApiEvent e)
        {
            switch (e.HttpRequestType)
            {
                case HttpRequestType.POST:
                    HTTPPost(e.url,
                        e.Request.requestType,
                        e.Request.requestData,
                        e.ResponseCallback);
                    break;
                case HttpRequestType.GET:
                    HTTPGet(e.url,
                        e.Request.requestType,
                        e.ResponseCallback);
                    break;
                case HttpRequestType.PUT:
                    HTTPPut(e.url,
                    e.Request.requestType,
                    e.Request.requestData,
                    e.ResponseCallback);
                    break;
                case HttpRequestType.DELETE:
                    HTTPDelete(e.url,
                        e.Request.requestType,
                        e.Request.requestData,
                        e.ResponseCallback);
                    break;
            }
        }
        /// <summary>
        /// Use this function for downloading images
        /// </summary>
        /// <param name="url"></param>
        /// <param name="image"></param>
        protected void DownloadImage(DownloadImageEvent downloadImageEvent)
        {
            string url = downloadImageEvent.Url;

            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("Image url is empty or null");
                return;
            }
            bool hascahed = false;
            string[] str = url.Split('/');

            if (File.Exists(Path.Combine(cashedImageUrl, str[str.Length - 1])))
            {
                url = "file://" + Path.Combine(cashedImageUrl, str[str.Length - 1]);
                hascahed = true;
            }
            Utils.CallEventAsync(new CoroutineEvent(DownloadImageRoutine(url, hascahed, downloadImageEvent.Action)));
            // Task task = DownloadImageAsync(url, hascahed, downloadImageEvent.Action);
            // await task;
        }
        private IEnumerator DownloadImageRoutine(string url, bool islocal, Action<Texture2D> image)
        {
            float waitTime = 0;
            bool isWait = false;
            imageRequestQueue.Enqueue(new Request(++imageRequestCount));

            if (imageRequestQueue.Count > imageMaxRequest)
            {
                isWait = true;
                Debug.Log("Request is waiting-->" + imageRequestCount);
                while (isWait)
                {
                    waitTime += Time.deltaTime;
                    if (imageRequestQueue.Count <= imageMaxRequest || waitTime >= maxWaitDuration)
                        isWait = false;
                    yield return null;
                }
                yield return new WaitUntil(() => !isWait);
            }
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                Debug.Log("<color=magenta>Request Arrive->" + imageRequestCount + "</color>");
                yield return request.SendWebRequest();
                var finishedRequest = imageRequestQueue.Dequeue();
                Debug.Log("<color=blue>Request is Completed with failed status->" + finishedRequest.RequestID + "</color>");
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                    image?.Invoke(null);
                }
                else
                {
                    if (!islocal)
                    {
                        string[] fileName = url.Split('/');
                        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName[fileName.Length - 1]), request.downloadHandler.data);
                    }
                    image?.Invoke(DownloadHandlerTexture.GetContent(request));
                }
                if (imageRequestQueue.Count == 0)
                {
                    waitTime = 0;
                    imageRequestCount = 0;
                }
            }

        }

        /// <summary>
        /// It Post data to server using url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="data"></param>
        /// <param name="OnPostCompleteCalback"></param>
        /// <param name="OnProgressCallback"></param>

        protected void HTTPPost(string url, RequestType requestType, string data, Action<IResponse> OnPostCompleteCalback)
        {
            Utils.CallEventAsync(new CoroutineEvent(PostRequest(url, requestType, data, OnPostCompleteCalback)));
        }

        private IEnumerator PostRequest(string url, RequestType requestType, string data, Action<IResponse> onPostCompleteCalback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-POST]-{requestType}-data-{data}");
            // Convert JSON string to bytes
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                    // Attach upload handler with JSON body
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                // Attach download handler
                request.downloadHandler = new DownloadHandlerBuffer();

                // Headers
                request.SetRequestHeader("Content-Type", "application/json");

                if (PlayerPrefs.HasKey("BearerToken") && !string.IsNullOrEmpty(PlayerPrefs.GetString("BearerToken")))
                    request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    IResponse response = (IResponse)JsonConvert.DeserializeObject(request.downloadHandler.text, GetTypeFromRequestType(requestType));
                    onPostCompleteCalback?.Invoke(response);
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onPostCompleteCalback?.Invoke(new Response(request.responseCode, request.error));
                }
            }

        }

        /// <summary>
        /// It Get data from  server using url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="OnGetCompleteCallback"></param>
        /// <param name="progressCallback"></param>
        protected void HTTPGet(string url, RequestType requestType, Action<IResponse> OnGetCompleteCallback)
        {
            Utils.CallEventAsync(new CoroutineEvent(GetRequest(url, requestType, OnGetCompleteCallback)));
        }

        private IEnumerator GetRequest(string url, RequestType requestType, Action<IResponse> onGetCompleteCallback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-GET]-{requestType}");
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Headers
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    IResponse response = (IResponse)JsonConvert.DeserializeObject(request.downloadHandler.text, GetTypeFromRequestType(requestType));
                    onGetCompleteCallback?.Invoke(response);
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onGetCompleteCallback?.Invoke(new Response(request.responseCode, request.error));
                }
            }
        }

        /// <summary>
        /// PUT REQUEST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="data"></param>
        /// <param name="OnPutCompleteCallback"></param>
        protected void HTTPPut(string url, RequestType requestType, string data, Action<IResponse> OnPutCompleteCallback)
        {
            Utils.CallEventAsync(new CoroutineEvent(PutRequest(url, requestType, data, OnPutCompleteCallback)));
        }
        private IEnumerator PutRequest(string url, RequestType requestType, string data, Action<IResponse> onPostCompleteCalback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-POST]-{requestType}-data-{data}");
            // Convert JSON string to bytes
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                    // Attach upload handler with JSON body
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                // Attach download handler
                request.downloadHandler = new DownloadHandlerBuffer();

                // Headers
                request.SetRequestHeader("Content-Type", "application/json");

                if (PlayerPrefs.HasKey("BearerToken") && !string.IsNullOrEmpty(PlayerPrefs.GetString("BearerToken")))
                    request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    IResponse response = (IResponse)JsonConvert.DeserializeObject(request.downloadHandler.text, GetTypeFromRequestType(requestType));
                    onPostCompleteCalback?.Invoke(response);
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onPostCompleteCalback?.Invoke(new Response(request.responseCode, request.error));
                }
            }
        }
        /// <summary>
        /// DELETE REQUEST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="data"></param>
        /// <param name="OnPutCompleteCallback"></param>
        protected void HTTPDelete(string url, RequestType requestType, string data, Action<IResponse> OnPutCompleteCallback)
        {
            Utils.CallEventAsync(new CoroutineEvent(DeleteRequest(url, requestType, data, OnPutCompleteCallback)));
        }
        private IEnumerator DeleteRequest(string url, RequestType requestType, string data, Action<IResponse> onPostCompleteCalback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-POST]-{requestType}-data-{data}");
            // Convert JSON string to bytes
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbDELETE))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                    // Attach upload handler with JSON body
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                // Attach download handler
                request.downloadHandler = new DownloadHandlerBuffer();

                // Headers
                request.SetRequestHeader("Content-Type", "application/json");

                if (PlayerPrefs.HasKey("BearerToken") && !string.IsNullOrEmpty(PlayerPrefs.GetString("BearerToken")))
                    request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    IResponse response = (IResponse)JsonConvert.DeserializeObject(request.downloadHandler.text, GetTypeFromRequestType(requestType));
                    onPostCompleteCalback?.Invoke(response);
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onPostCompleteCalback?.Invoke(new Response(request.responseCode, request.error));
                }
            }
        }
        private Type GetTypeFromRequestType(RequestType requestType)
        {
            switch (requestType)
            {
                case RequestType.Login:
                case RequestType.SilentLogin:
                    return typeof(LoginResponse);
                case RequestType.GetFriends:
                case RequestType.GetWeekLeaderboard:
                case RequestType.GetGlobalLeaderboard:
                    return typeof(LeaderboardResponse);
                case RequestType.UpdateScore:
                case RequestType.UpdateScoreWeek:
                case RequestType.UpdateCoin:
                case RequestType.UpdateStar:
                case RequestType.DeleteAccount:
                case RequestType.Logout:
                    return typeof(Response);
                default:
                    return typeof(Response);
            }
        }
        public void Update()
        {

        }
    }
}
