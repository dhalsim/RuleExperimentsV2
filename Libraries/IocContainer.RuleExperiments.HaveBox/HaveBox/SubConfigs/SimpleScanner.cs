using HaveBox;
using HaveBox.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace HaveBox.SubConfigs
{
    public class SimpleScanner : Config
    {
        public SimpleScanner(Assembly assembly)
        {
            assembly.GetTypes()
            .Where(type => type.BaseType != typeof(System.Enum))
            .Where(type => type.BaseType != typeof(System.MulticastDelegate))
            .Where(type => type.BaseType != typeof(System.Delegate))
            .Where(type => type.BaseType != typeof(HaveBox.Configuration.Config))
            .Where(type => !HasValueTypesInConstructorParameters(type))
            .Where(type => !HasObjectTypesInConstructorParameters(type))
            .Each(type =>
            {
                type.GetInterfaces().Each(interfaze =>
                {
                    For(interfaze).Use(type);
                });
            });
        }

        private bool HasValueTypesInConstructorParameters(Type type)
        {
            if (!type.GetConstructors().Any())
            {
                return false;
            }

            var constructorParameters = type.GetConstructors().First().GetParameters();
            return constructorParameters.Any(parameter => parameter.ParameterType.IsValueType);
        }

        private bool HasObjectTypesInConstructorParameters(Type type)
        {
            if (!type.GetConstructors().Any())
            {
                return false;
            }

            var constructorParameters = type.GetConstructors().First().GetParameters();
            return constructorParameters.Any(parameter => parameter.ParameterType == typeof(System.Object));
        }
    }
}
