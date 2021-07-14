namespace FastMobile.FXamarin.Core
{
    public interface IFCache
    {
        void SetCache<T>(string key, T obj);

        T GetCache<T>(string key);

        void RemoveCache(string key);
    }
}