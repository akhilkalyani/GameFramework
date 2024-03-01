using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class JsonDataService : IDataService
{
    string folderName = "/User/";
   
    public T LoadData<T>(string fileName)
    {
        T data=default(T);

        string folderPath = $"{Application.persistentDataPath}{folderName}";

        string filepath = $"{folderPath}{fileName}";

        if (!File.Exists(filepath)) return data;
       
        string jsonContent = File.ReadAllText(filepath);
        
        data=JsonConvert.DeserializeObject<T>(jsonContent);
        
        return data;
    }
    public bool SaveData(string fileName, string json)
    {
        string folderPath = $"{Application.persistentDataPath}{folderName}";

        string filepath=$"{folderPath}{fileName}";

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        
        File.WriteAllText(filepath,json);

        Debug.Log($"File saved path->{filepath}");

        return true;
    }
}
