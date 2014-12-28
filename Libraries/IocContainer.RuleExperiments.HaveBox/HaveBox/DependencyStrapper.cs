using HaveBox.Collections;
using HaveBox.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IocContainer.RuleExperiments.HaveBox;

namespace HaveBox
{
    internal class DependencyStrapper : IDependencyStrapper
    {
        private readonly IDelegateFactory _delegateFactory;

        internal DependencyStrapper(IDelegateFactory delegateFactory)
        {
            _delegateFactory = delegateFactory;
        }

        public void Strap(IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            dependencyMap.Each(x => x.Value.Where(typeDetailsList => typeDetailsList.CreateInstanceDelegate == null)
                .Each(z => z.CreateInstanceDelegate = GetCreateInstance(z, dependencyMap)));
        }

        private DelegateFactory.CreateInstance GetCreateInstance(TypeDetails typeDetails, IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            if (typeDetails.IsSingleton && !typeDetails.IsLazySingleton)
            {
                if (typeDetails.CreateInstanceDelegate == null)
                {
                    GetCreateInstanceDelegate(typeDetails, dependencyMap).Invoke(typeDetails, out typeDetails.SingletonObject);
                }
                return _delegateFactory.GetCreateInstanceSingleton(typeDetails);
            }

            if (typeDetails.LamdaFunction != null)
            {
                return _delegateFactory.GetCreateInstanceFromLambdaFunction(typeDetails);
            }

            return GetCreateInstanceDelegate(typeDetails, dependencyMap);
        }

        private DelegateFactory.CreateInstance GetCreateInstanceDelegate(TypeDetails typeDetails, IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            DelegateFactory.CreateInstance createInstance;

            if (typeDetails.ImplementType.GetInterfaces().Contains(typeof(ITypeEnumerable)))
            {
                createInstance = _delegateFactory.GetCreateInstanceIEnumerable(typeDetails);
            }
            else
            {
                DependencyCreateInstanceResolver(typeDetails, dependencyMap);

                createInstance = _delegateFactory.GetCreateInstanceDelegate(typeDetails);
            }

            if(typeDetails.IsLazySingleton)
            {
                createInstance = _delegateFactory.GetCreateInstanceLazySingleton(typeDetails, createInstance);
            }

            if (typeDetails.Interceptor != null)
            {
                createInstance = _delegateFactory.GetCreateInstanceWithInterception(typeDetails, createInstance);
            }

            return createInstance;
        }

        private void DependencyCreateInstanceResolver(TypeDetails typeDetails, IDictionary<Type, IList<TypeDetails>> dependencyMap)
        {
            var constructorParameters = typeDetails.ImplementType.GetAllConstructors().First().GetParameters();

            int index = 0;
            constructorParameters.Each(x =>
            {
                var parameterTypeDetails = GetParameterTypeDetails(dependencyMap, x);

                if (parameterTypeDetails.CreateInstanceDelegate == null)
                {
                    parameterTypeDetails.CreateInstanceDelegate = GetCreateInstance(parameterTypeDetails, dependencyMap);
                }

                typeDetails.DependenciesTypeDetails[index] = parameterTypeDetails;
                index++;
            });
        }

        private static TypeDetails GetParameterTypeDetails(IDictionary<Type, IList<TypeDetails>> dependencyMap, ParameterInfo parameterInfo)
        {
            var typeDetailsList = ResolveTypeDetailsList(dependencyMap, parameterInfo.ParameterType);

            if (typeDetailsList.Count() < 2)
            {
                return typeDetailsList.First();
            }

            var typeDetail = typeDetailsList.SingleOrDefault(x => x.ImplementType.Name == parameterInfo.Name);
            if (typeDetail == null)
            {
                throw new Exception("Unknown type: " + parameterInfo.Name + " for " + parameterInfo.ParameterType.Name);
            }

            return typeDetail;
        }

        private static IList<TypeDetails> ResolveTypeDetailsList(IDictionary<Type, IList<TypeDetails>> dependencyMap, Type type)
        {
            if (type.IsIEnumerable() && type.GetGenericArguments().First().FullName == null)
            {
                return dependencyMap.Where(keyValue => keyValue.Key.Name == type.Name).Select(keyValue => keyValue.Value).FirstOrDefault();
            }

            type = type.IsGenericType && type.FullName == null ? type.GetGenericTypeDefinition() : type;

            IList<TypeDetails> typeDetailsList;
            if (!dependencyMap.TryGetValue(type, out typeDetailsList))
            {
                throw new Exception("Unknown type: " + type.Name);
            }

            return typeDetailsList;
        }
    }
}
