using HaveBox.Collections.Generic;
using HaveBox.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HaveBox
{
    public class Container : IContainer
    {
        internal IDictionary<Type, IList<TypeDetails>> _dependencyMap;
        internal IDictionary<Type, TypeDetails> _dependencyFastMap;
        private readonly IDependencyStrapper _dependencyStrapper;
        protected Guid _containerId;

        public Container()
        {
            _dependencyMap = new HashTable<Type, IList<TypeDetails>>();
            _dependencyFastMap = new HashTable<Type, TypeDetails>();
            _dependencyStrapper = new DependencyStrapper(new DelegateFactory());
            _containerId = Guid.NewGuid();
        }

        public virtual void Configure(Action<IConfig> registry)
        {
            var config = new Config(_dependencyMap);
            registry.Invoke(config);
#if !SILVERLIGHT
            config.PostRegisterProxyInterceptors();
#endif
            config.PostRegisterClosedGenerics();
            config.PostRegisterIEnumerables();

            StrapAndMarkSingletons(_containerId, _dependencyMap);
        }

        private void StrapAndMarkSingletons(Guid containerId, IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            _dependencyStrapper.Strap(dependencyMap);
            _dependencyMap.Each(x => _dependencyFastMap[x.Key] = x.Value.First());

            MarkSingletionsWithContainerId(containerId, dependencyMap);
        }

        public virtual TYPE GetInstance<TYPE>()
        {
            return (TYPE) GetInstance(typeof(TYPE));
        }

        public virtual bool TryGetInstance<TYPE>(out TYPE instance)
        {
            object _instance;
            var hasInstance = TryGetInstance(typeof(TYPE), out _instance);
            instance = (TYPE)_instance;

            return hasInstance;
        }

        public virtual object GetInstance(Type type)
        {
            object instance;
            TypeDetails typeDetails;

            if (!_dependencyFastMap.TryGetValue(type, out typeDetails))
            {
                if (!TryResolveGenericType(type))
                {
                    throw new Exception("Unknown type: " + type.Name);
                }

                typeDetails = _dependencyFastMap[type];
            }

            typeDetails.CreateInstanceDelegate(typeDetails, out instance);
            return instance;
        }

        public virtual bool TryGetInstance(Type type, out object instance)
        {
            TypeDetails typeDetails;

            if (!_dependencyFastMap.TryGetValue(type, out typeDetails))
            {
                if (!TryResolveGenericType(type))
                {
                    instance = null;
                    return false;
                }

                typeDetails = _dependencyFastMap[type];
            }

            typeDetails.CreateInstanceDelegate(typeDetails, out instance);
            return true;
        }

        public virtual bool TryGetInstance<TYPE>(string implementationName, out TYPE instance)
        {
            object _instance;
            var hasInstance = TryGetInstance(typeof(TYPE), implementationName, out _instance);
            instance = (TYPE)_instance;

            return hasInstance;
        }

        public virtual bool TryGetInstance(Type type, string implementationName, out object instance)
        {
            IList<TypeDetails> typeDetailsList;

            if (!_dependencyMap.TryGetValue(type, out typeDetailsList))
            {
                if (!TryResolveGenericType(type))
                {
                    instance = null;
                    return false;
                }

                typeDetailsList = _dependencyMap[type];
            }

            var typeDetails = typeDetailsList.SingleOrDefault(x => x.ImplementType.Name == implementationName);
            if (typeDetails == null)
            {
                instance = null;
                return false;
            }

            typeDetails.CreateInstanceDelegate(typeDetails, out instance);
            return true;
        }

        public virtual TYPE GetInstance<TYPE>(string implementationName)
        {
            return (TYPE) GetInstance(typeof(TYPE), implementationName);
        }

        public virtual object GetInstance(Type type, string implementationName)
        {
            object instance;
            IList<TypeDetails> typeDetailsList;

            if (!_dependencyMap.TryGetValue(type, out typeDetailsList))
            {
                if (!TryResolveGenericType(type))
                {
                    throw new Exception("Unknown type: " + type.Name);
                }

                typeDetailsList = _dependencyMap[type];
            }

            var typeDetails = typeDetailsList.SingleOrDefault(x => x.ImplementType.Name == implementationName);
            if (typeDetails == null)
            {
                throw new Exception("Unknown type: " + implementationName + " for " + type.Name);
            }

            typeDetails.CreateInstanceDelegate(typeDetails, out instance);
            return instance;
        }

        protected bool TryResolveGenericType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            if (type.IsIEnumerable())
            {
                var genericArgument = type.GetGenericArguments().First();

                if (genericArgument.IsGenericType)
                {
                    type = genericArgument;
                }
            }

            var config = new Config(_dependencyMap);
            TryResolveGenericTypeTree(config, type);
            config.PostRegisterIEnumerables();
            StrapAndMarkSingletons(_containerId, _dependencyMap);
            return true;
        }

        private void TryResolveGenericTypeTree(Config config, Type type)
        {
            if (_dependencyMap.ContainsKey(type))
            {
                return;
            }

            if (type.IsIEnumerable())
            {
                type = type.GetGenericArguments().First();
            }

            var openGenericTypes = _dependencyMap[type.GetGenericTypeDefinition()];

            openGenericTypes.Each(openGenericType =>
            {
                CreateClosedGenericTypeAndRegister(config, type, openGenericType);
            });
        }

        private void CreateClosedGenericTypeAndRegister(Config config, Type type, TypeDetails openGenericType)
        {
            var closedGenericType = openGenericType.ImplementType.MakeGenericType(type.GetGenericArguments());

            var constructorParamters = GetGenericConstructorParameters(closedGenericType);

            constructorParamters.Each(parameter => TryResolveGenericTypeTree(config, parameter.ParameterType));

            config.For(type).Use(closedGenericType);
        }

        private static IEnumerable<System.Reflection.ParameterInfo> GetGenericConstructorParameters(Type closedGenericType)
        {
            return closedGenericType
                        .GetConstructors()
                        .First()
                        .GetParameters()
                        .Where(parameter => parameter.ParameterType.IsGenericType);
        }

        internal void MarkSingletionsWithContainerId(Guid containerId, IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            dependencyMap.Each(typeKey =>
            {
                typeKey.Value.Where(typeDetail => typeDetail.SingletonOwner == default(Guid)).Each(typeDetail =>
                {
                    typeDetail.SingletonOwner = containerId;
                });
            });
        }

        private IDictionary<Type, IList<TypeDetails>> CreateMapWithNewSingletons(IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            var mapWithNewSingletons = new Dictionary<Type, IList<TypeDetails>>();

            dependencyMap.Each(typeKey =>
                {
                    mapWithNewSingletons[typeKey.Key] = new List<TypeDetails>();
                    typeKey.Value.Each(typeDetail =>
                    {
                        if (!(typeDetail.IsSingleton && typeDetail.PerContainerLifeTime))
                        {
                            mapWithNewSingletons[typeKey.Key].Add(typeDetail);
                        }
                        else
                        {
                            var implementationClone = (TypeDetails)typeDetail.Clone();
                            implementationClone.CreateInstanceDelegate = null;
                            implementationClone.SingletonObject = null;
                            implementationClone.SingletonOwner = default(Guid);
                            mapWithNewSingletons[typeKey.Key].Add(implementationClone);
                        }
                    });
                });

            _dependencyStrapper.Strap(mapWithNewSingletons);
            return mapWithNewSingletons;
        }

        public IDisposableContainer CreateChildContainer()
        {
            return new DisposableContainer(CreateMapWithNewSingletons(_dependencyMap));
        }
    }
}
