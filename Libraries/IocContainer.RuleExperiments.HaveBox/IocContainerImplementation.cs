using System;
using Domain.RuleExperiments.Interfaces;
using HaveBox;

namespace IocContainer.RuleExperiments.HaveBox
{
    public class IocContainerImplementation : IIocContainer
    {
        private readonly IContainer _container;

        public IocContainerImplementation(IContainer container)
        {
            _container = container;
        }

        public T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }

        public T GetInstance<T>(Enum providerType)
        {
            return _container.GetInstance<T>(providerType.ToString());
        }

        public T GetInstance<T>(string providerType)
        {
            return _container.GetInstance<T>(providerType);
        }
    }
}