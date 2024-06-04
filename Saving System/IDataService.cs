public interface IDataService
{
    bool SaveData(string RelativePath,string data);

    T LoadData<T>(string RelativePath);
}
