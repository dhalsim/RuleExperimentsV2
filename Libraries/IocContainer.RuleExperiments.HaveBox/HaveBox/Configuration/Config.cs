using HaveBox.Collections;
using HaveBox.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if !SILVERLIGHT
using HaveBox.HaveBoxProxy;
#endif

namespace HaveBox.Configuration
{
    public class Config : IConfig 
    {
        private IDictionary<Type, IList<TypeDetails>> _dependencyMap;

        public Config()
        {
        }

        internal Config(IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            _dependencyMap = dependencyMap;
        }

        public IInjectionExpression For<TYPE>()
        {
            return For(typeof(TYPE));
        }

        public IInjectionExpression For(Type type)
        {
            CreatedependencyMapIfNeeded();

            IList<TypeDetails> dependencies;

            if (!_dependencyMap.TryGetValue(type, out dependencies))
            {
                dependencies = new List<TypeDetails>();
                _dependencyMap[type] = dependencies;
            }

            return new InjectionExpression(dependencies);
        }

        public void MergeConfig<CONFIG>()
        {
            MergeConfig((IConfig)typeof(CONFIG).GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes));
        }

        public void MergeConfig(IConfig config)
        {
            CreatedependencyMapIfNeeded();
            ((Config)config).GetdependencyMap().Each(x => _dependencyMap[x.Key] = x.Value);
        }

        private void CreatedependencyMapIfNeeded()
        {
            if (_dependencyMap == default(Dictionary<Type, IList<TypeDetails>>))
            {
                _dependencyMap = new HashTable<Type, IList<TypeDetails>>();
            }
        }

        internal IDictionary<Type, IList<TypeDetails>> GetdependencyMap()
        {
            CreatedependencyMapIfNeeded();
            return _dependencyMap;
        }

        internal void PostRegisterIEnumerables()
        {
            var subConfig = new Config();
            _dependencyMap.Each(keyValue => {
                if (keyValue.Value.First().ImplementType == null || (keyValue.Value.First().ImplementType != null && !keyValue.Value.First().ImplementType.GetInterfaces().Contains(typeof(ITypeEnumerable))))
                {
                    ((InjectionProperty)subConfig.For(typeof(IEnumerable<>).MakeGenericType(keyValue.Key))
                                                 .Use(typeof(TypeEnumerable<>).MakeGenericType(keyValue.Key)))
                                                 .SetTypesToBeInjectedInEnumerable(keyValue.Value);
                }
            });
           MergeConfig(subConfig);
        }

        internal void PostRegisterClosedGenerics()
        {
            var subConfig = new Config();
            _dependencyMap.Each(keyValue =>
            {
                keyValue.Value.Where(typeDetails => typeDetails.ImplementType != null).Each(typeDetails =>
                {
                    RegistreGenericsFromConstructor(subConfig, typeDetails);
                });
            });
            MergeConfig(subConfig);
        }

        private void RegistreGenericsFromConstructor(Config subConfig, TypeDetails typeDetails)
        {
            var constructor = typeDetails.ImplementType.GetConstructors();

            if (constructor.Any())
            {
                var constructorParameters = constructor.First().GetParameters();
                constructorParameters.Each(parameter =>
                {
                    RegistreGenericParameter(subConfig, parameter);
                });
            }
        }

        private void RegistreGenericParameter(Config subConfig, ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;

            if (!parameterType.IsGenericType || parameterType.IsGenericTypeDefinition || parameterType.FullName == null)
            {
                return;
            }

            if (_dependencyMap.ContainsKey(parameterType))
            {
                return;
            }

            var type = ResolveGenericParameterType(parameter);
            var openGenericType = type.GetGenericTypeDefinition();

            IList<TypeDetails> typeDetailsList;
            if (_dependencyMap.TryGetValue(openGenericType, out typeDetailsList))
            {
                typeDetailsList.Each(typeDetailz =>
                {
                    var closedGeneric = typeDetailz.ImplementType.MakeGenericType(type.GetGenericArguments());
                    subConfig.For(type).Use(closedGeneric);
                });
            }
        }

        private static Type ResolveGenericParameterType(ParameterInfo parameter)
        {
            if (parameter.ParameterType.IsIEnumerable() && parameter.ParameterType.GetGenericArguments().First().IsGenericType)
            {
                return parameter.ParameterType.GetGenericArguments().First();
            }

            return parameter.ParameterType;
        }

#if !SILVERLIGHT
        internal void PostRegisterProxyInterceptors()
        {
            var subConfig = new Config();
            var proxyInterceptors = _dependencyMap.Values.SelectMany(typeDetail => typeDetail.Where(x => x.ProxyInterceptor != null).Select(x => x.ProxyInterceptor));
            proxyInterceptors.Each(x => subConfig.For<IInterceptor>().Use(x));
            MergeConfig(subConfig);
        }
#endif
    }
}
