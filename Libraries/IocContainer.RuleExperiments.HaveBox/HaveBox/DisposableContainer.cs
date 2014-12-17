using HaveBox.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HaveBox
{
    internal class DisposableContainer : Container, IDisposableContainer
    {
        IList<object> _disposeAbleInstances;

        public DisposableContainer(IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            _dependencyMap = dependencyMap;
            _disposeAbleInstances = new List<object>();
            _containerId = Guid.NewGuid();
            MarkSingletionsWithContainerId(_containerId, _dependencyMap);
            RegisterDisposeableSingletons(dependencyMap);
            _dependencyMap.Each(x => _dependencyFastMap[x.Key] = x.Value.First());
        }

        public override void Configure(Action<IConfig> registry)
        {
            base.Configure(registry);
            RegisterDisposeableSingletons(_dependencyMap);
        }

        private void RegisterDisposeableSingletons(IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            dependencyMap.Each(type =>
                    type.Value.Each(typeDetails =>
                        {
                            if (typeDetails.IsSingleton && typeDetails.SingletonOwner == _containerId && typeDetails.SingletonObject is IDisposable)
                            {
                                _disposeAbleInstances.Add(typeDetails.SingletonObject);
                            }
                        }));
        }

        public override object GetInstance(Type type)
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
            return GetInstanceAndRegisterDiposable(typeDetails);
        }

        public override object GetInstance(Type type, string implementationName)
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
            return GetInstanceAndRegisterDiposable(typeDetails);
        }

        public override bool TryGetInstance(Type type, out object instance)
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
            instance = GetInstanceAndRegisterDiposable(typeDetails);
            return true;
        }

        public override bool TryGetInstance(Type type, string implementationName, out object instance)
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
            instance = GetInstanceAndRegisterDiposable(typeDetails);
            return true;
        }

        private object GetInstanceAndRegisterDiposable(TypeDetails typeDetails)
        {
            object instance;
            typeDetails.CreateInstanceDelegate(typeDetails, out instance);

            if (instance is IDisposable && (!typeDetails.IsSingleton || typeDetails.SingletonOwner == _containerId))
            {
                _disposeAbleInstances.Add(instance);
            }
            return instance;
        }

        public void Dispose()
        {
            _disposeAbleInstances.Distinct()
                .Cast<IDisposable>()
                .Each(instance => instance.Dispose());

            GC.SuppressFinalize(this);
        }

        public void DisposeInstance(IDisposable instance)
        {
            _disposeAbleInstances = _disposeAbleInstances.Distinct().ToList();
            if (_disposeAbleInstances.Remove(instance))
            {
                instance.Dispose();
            }
        }
    }
}
