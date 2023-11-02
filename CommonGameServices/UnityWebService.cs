using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace GF
{
    public class UnityWebService : IService
    {
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
        private string BearerToken;
        private string BearerTokenKey = "BearerToken";
        private ApiConfiguration apiConfiguration;
        public void Initialize()
        {
            Console.Log(LogType.Log, "UnityWebService created");
            if (!PlayerPrefs.HasKey(BearerTokenKey))
            {
                PlayerPrefs.SetString(BearerTokenKey, "");
            }
            cashedImageUrl = Application.persistentDataPath + "/CashedImages/";
            if (!Directory.Exists(cashedImageUrl))
            {
                Directory.CreateDirectory(cashedImageUrl);
            }
            apiConfiguration = Resources.Load<ApiConfiguration>("ApiConfiguration");
        }

        
        public void RegisterListener()
        {
            EventManager.Instance.AddListener<DownloadImageEvent>(DoanloadImage);
            EventManager.Instance.AddListener<RaiseWebApiEvent>(OnApiRequest);
        }

        
        private void OnApiRequest(RaiseWebApiEvent e)
        {
            HTTPPost(apiConfiguration.GetApiUrl(e.ApiRequest.requestType),
                e.ApiRequest.requestType.ToString(),
                JsonConvert.SerializeObject(e.ApiRequest),
                e._responseCallback);
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<DownloadImageEvent>(DoanloadImage);
            EventManager.Instance.RemoveListener<RaiseWebApiEvent>(OnApiRequest);
        }
        /// <summary>
        /// Use this function for downloading images
        /// </summary>
        /// <param name="url"></param>
        /// <param name="image"></param>
        protected void DoanloadImage(DownloadImageEvent downloadImageEvent)
        {
            string url=downloadImageEvent.Url;
            Action<Texture2D> image=downloadImageEvent.Action;
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
            Utils.RaiseEventAsync(new CoroutineEvent(DownloadImageRoutine(url, hascahed, image)));
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

                Debug.Log("wait time->" + waitTime);
                yield return new WaitUntil(() => !isWait);
            }
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                Debug.Log("<color=magenta>Request Arrive->" + imageRequestCount + "</color>");
                yield return request.SendWebRequest();
                if (request.result!=UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                    image?.Invoke(null);
                    var finishedRequest = imageRequestQueue.Dequeue();
                    Debug.Log("<color=blue>Request is Completed with failed status->" + finishedRequest.RequestID + "</color>");
                }
                else
                {
                    if (!islocal)
                    {
                        string[] fileName = url.Split('/');
                        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName[fileName.Length - 1]), request.downloadHandler.data);
                    }
                    image?.Invoke(DownloadHandlerTexture.GetContent(request));
                    var finishedRequest = imageRequestQueue.Dequeue();
                    Debug.Log("<color=blue>Request is Completed->" + finishedRequest.RequestID + "</color>");
                }
            }
            if (imageRequestQueue.Count == 0)
            {
                waitTime = 0;
                imageRequestCount = 0;
            }
        }
        /// <summary>
        /// It used for downloading any binary file from server using desire url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="data"></param>
        /// <param name="OnDownloadCompleteCallback"></param>
        /// <param name="OnProgressCallback"></param>
        protected void DownloadFile(string url, string requestType, string data, Action<string, byte[], string> OnDownloadCompleteCallback, Action<float, string> OnProgressCallback)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(DownloadFileRequest(url, requestType, data, OnDownloadCompleteCallback, OnProgressCallback)));
        }

        private IEnumerator DownloadFileRequest(string url, string requestType, string data, Action<string, byte[], string> onDownloadCompleteCallback, Action<float, string> onProgressCallback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                if (onProgressCallback != null)
                    onProgressCallback(webRequest.downloadProgress, requestType);
                webRequest.SendWebRequest();
                //Request and wait for the desire page
                while (webRequest.downloadProgress < 1)
                {
                    onProgressCallback(webRequest.downloadProgress, requestType);
                    yield return new WaitForSeconds(0.03f);
                }
                onProgressCallback(1f, requestType);
                if (onDownloadCompleteCallback != null)
                    onDownloadCompleteCallback(requestType, (webRequest.isDone ? webRequest.downloadHandler.data : null), webRequest.error);
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
        
        protected void HTTPPost(string url, string requestType, string data, Action<string, WebApiResponse, string> OnPostCompleteCalback)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(PostRequest(url, requestType, data, OnPostCompleteCalback)));
        }

        
        private IEnumerator PostRequest(string url, string requestType, string data, Action<string, WebApiResponse, string> onPostCompleteCalback)
        {
            using (var request = UnityWebRequest.PostWwwForm(url, data))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + BearerToken);
                yield return request.SendWebRequest();

                if (request.result!=UnityWebRequest.Result.Success)
                    Debug.Log("Network error has occured: " + request.GetResponseHeader(""));
                else
                {
                    byte[] results = request.downloadHandler.data;
                    Debug.Log("Success " + request.downloadHandler.text);
                    if (onPostCompleteCalback != null)
                    {
                        WebApiResponse response = null;
                        try
                        {
                            response = JsonConvert.DeserializeObject<WebApiResponse>(request.isDone ? request.downloadHandler.text : null);
                        }
                        catch (Exception e) {
                            Console.Log(LogType.Error, e.StackTrace);
                        }
                        onPostCompleteCalback(requestType,response, request.error);
                    }
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
        protected void HTTPGet(string url, string requestType, Action<string, string, string> OnGetCompleteCallback, Action<float> progressCallback)
        {
            Utils.RaiseEventAsync(new CoroutineEvent(GetRequest(url, requestType, OnGetCompleteCallback, progressCallback)));
        }

        private IEnumerator GetRequest(string url, string requestType, Action<string, string, string> onGetCompleteCallback, Action<float> progressCallback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (progressCallback != null)
                    progressCallback(webRequest.downloadProgress);
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    if (onGetCompleteCallback != null)
                        onGetCompleteCallback(requestType, (webRequest.isDone ? webRequest.downloadHandler.text : null), webRequest.error);
                }
                else
                {
                    Debug.LogError($"Error occured {webRequest.result}");
                }
            }
        }
    }
}
