using System;
using System.Reflection;

namespace IocContainer.RuleExperiments.HaveBox
{
    public static class TypeExtensions
    {
        public static ConstructorInfo[] GetAllConstructors(this Type type)
        {
            return type.GetConstructors(BindingFlags.Public
                                 | BindingFlags.NonPublic
                                 | BindingFlags.Instance);
        }
    }
}