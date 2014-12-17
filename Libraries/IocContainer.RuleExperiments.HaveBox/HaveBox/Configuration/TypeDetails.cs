using System;
using System.Collections.Generic;

namespace HaveBox.Configuration
{
    public class TypeDetails
    {
        public DelegateFactory.CreateInstance CreateInstanceDelegate;
        public TypeDetails[] DependenciesTypeDetails;
        public Type ImplementType;
        public bool IsSingleton;
        public bool IsLazySingleton;
        public object SingletonObject;
        public bool PerContainerLifeTime;
        public Func<object> LamdaFunction;
        public Guid SingletonOwner;
        public IInstantiationInterceptor Interceptor;
        public Type ProxyInterceptor;
        public IEnumerable<TypeDetails> TypeSiblings;

        public object Clone()
        {
            return new TypeDetails
            {
                CreateInstanceDelegate = CreateInstanceDelegate,
                DependenciesTypeDetails = DependenciesTypeDetails,
                ImplementType = ImplementType,
                IsSingleton = IsSingleton,
                IsLazySingleton = IsLazySingleton,
                SingletonObject = SingletonObject,
                PerContainerLifeTime = PerContainerLifeTime,
                LamdaFunction = LamdaFunction,
                SingletonOwner = SingletonOwner,
                Interceptor = Interceptor,
                ProxyInterceptor = ProxyInterceptor,
                TypeSiblings = TypeSiblings,
            };
        }
    }
}
