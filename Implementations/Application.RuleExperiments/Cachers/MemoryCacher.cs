using System.Runtime.Caching;
using Domain.RuleExperiments.Interfaces;

namespace Application.RuleExperiments.Cachers
{
    public class MemoryCacher : ICacher
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        public void AddOrUpdate(string key, object @object)
        {
            if (Cache.Contains(key))
            {
                Cache.Set(key, @object, new CacheItemPolicy());
            }
            else
            {
                Cache.Add(key, @object, new CacheItemPolicy());
            }
        }

        public T Get<T>(string key)
        {
            return (T) Cache.Get(key);
        }
    }
}