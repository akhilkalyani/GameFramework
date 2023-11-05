using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GF.SavingSystem
{
    public class SerializationManager
    {
        private static readonly string savePath = Application.persistentDataPath + "/save/";
        private static readonly string fileExtetion = ".sdf";
        public static void Save(string fileName,object saveData)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string filePath = savePath + fileName + fileExtetion;
            FileStream fileStream=File.Create(filePath);
            binaryFormatter.Serialize(fileStream, saveData);
            fileStream.Close();
        }
        private static object Load(string fileName)
        {
            string filePath = savePath + fileName + fileExtetion;
            if (!File.Exists(filePath))
            {
                return null;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(filePath,FileMode.Open);
            try
            {
                object save= binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                return save;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                fileStream.Close();
                return null;
            }
        }
        public static T Load<T>(string fileName)
        {
            return (T)Load(fileName);
        }
    }
}
