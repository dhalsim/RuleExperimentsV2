using System;

namespace Domain.RuleExperiments.Interfaces
{
    public interface IIocContainer
    {
        T GetInstance<T>();

        T GetInstance<T>(Enum providerType);

        T GetInstance<T>(string providerType);
    }
}