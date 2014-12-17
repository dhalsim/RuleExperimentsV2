using System;
using System.Collections.Generic;
#if !SILVERLIGHT
using HaveBox.HaveBoxProxy;
#endif

namespace HaveBox.Configuration
{
    public class InjectionProperty : IInjectionProperty
    {
        protected readonly TypeDetails _typeDetails;

        internal InjectionProperty(TypeDetails typeDetails)
        {
            _typeDetails = typeDetails;
        }

        public ISingletonProperty AsSingleton()
        {
            _typeDetails.IsSingleton = true;
            _typeDetails.IsLazySingleton = false;
            return new SingletonProperty(_typeDetails);
        }

        public ISingletonProperty AsLazySingleton()
        {
            _typeDetails.IsSingleton = true;
            _typeDetails.IsLazySingleton = true;
            return new SingletonProperty(_typeDetails);
        }

        public IInjectionProperty AndInterceptInstantiationWith<TYPE>()
        {
            return AndInterceptInstantiationWith((IInstantiationInterceptor)typeof(TYPE).GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes));
        }

        public IInjectionProperty AndInterceptInstantiationWith(IInstantiationInterceptor interceptor)
        {
            _typeDetails.Interceptor = interceptor;
            return new InjectionProperty(_typeDetails);
        }

        public IInjectionProperty SetTypesToBeInjectedInEnumerable(IEnumerable<TypeDetails> typeDetailsEnumerable)
        {
            _typeDetails.TypeSiblings = typeDetailsEnumerable;
            return new InjectionProperty(_typeDetails);
        }

#if !SILVERLIGHT
        public IInjectionProperty AndInterceptMethodsWith<TYPE>()
        {
            return AndInterceptMethodsWith(typeof(TYPE));
        }

        public IInjectionProperty AndInterceptMethodsWith(Type interceptor)
        {
            _typeDetails.ProxyInterceptor = interceptor;
            _typeDetails.DependenciesTypeDetails = new TypeDetails[_typeDetails.DependenciesTypeDetails.Length + 1];
            new ClassProxyFactory().CreateProxyType(_typeDetails.ImplementType, out _typeDetails.ImplementType);
            return new InjectionProperty(_typeDetails);
        }
#endif
    }
}