using UnityEngine;
using System.Collections.Generic;
using GF;

namespace Netconfig
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "Create ServerConfig", order = 1)]
    public class ServerConfig : ScriptableObject
    {
        [System.Serializable]
        public class ServerEntry
        {
            public string configName;
            public string baseURL;
            public string socketURL;
        }
        // Singleton instance for easy access
        private static ServerConfig instance;
        public static ServerConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    // If the instance is null, try to load from resources
                    instance = Resources.Load<ServerConfig>("NetConfig/ServerConfig");

                    // If still null, create a new instance
                    if (instance == null)
                    {
                        instance = CreateInstance<ServerConfig>();
                    }
                }

                return instance;
            }
        }
        public List<ServerEntry> serverEntries = new List<ServerEntry>();
        public int selectedConfigIndex; // Index of the selected configuration
        
        // Current selected server URL
        public ServerEntry CurrentServerURL
        {
            get
            {
                // Add your logic here to determine which server URL to use based on the selected configuration
                int index = Mathf.Clamp(selectedConfigIndex, 0, serverEntries.Count - 1);
                return serverEntries.Count > 0 ? serverEntries[index] : null;
            }
        }
        [Serializable]
        public class ApiName
        {
            public RequestType requestType;
            public string api;
        }
        public List<ApiName> apiList;
        private Dictionary<RequestType, string> apiDictionary = new Dictionary<RequestType, string>();
        // Add a new server configuration
        public void AddServerEntry(string name, string baseUrl, string socketUrl)
        {
            ServerEntry newEntry = new ServerEntry { configName = name, baseURL = baseUrl, socketURL = socketUrl };
            serverEntries.Add(newEntry);
            selectedConfigIndex = serverEntries.Count - 1; // Select the newly added configuration
        }
         public void BuildApis()
        {
            foreach (var item in apiList)
            {
                apiDictionary.Add(item.requestType, item.api);
            }
        }
        // Remove a server configuration
        public void RemoveServerEntry(int index)
        {
            if (index >= 0 && index < serverEntries.Count)
            {
                serverEntries.RemoveAt(index);
                selectedConfigIndex = Mathf.Clamp(selectedConfigIndex, 0, serverEntries.Count - 1); // Adjust the selected index
            }
        }
        public string GetApiUrl(RequestType apiType)
        {
            if (apiDictionary.Count == 0) BuildApis();
            return $"{CurrentServerURL.baseURL}/{apiType.ToString().ToLower()}";
        }
    }
    public enum RequestType{}
}
