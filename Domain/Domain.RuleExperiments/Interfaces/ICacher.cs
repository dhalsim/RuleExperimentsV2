namespace Domain.RuleExperiments.Interfaces
{
    public interface ICacher
    {
        void AddOrUpdate(string key, object @object);

        T Get<T>(string key);
    }
}