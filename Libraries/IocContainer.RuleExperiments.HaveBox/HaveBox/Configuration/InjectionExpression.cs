using System;
using System.Collections.Generic;
using System.Linq;

namespace HaveBox.Configuration
{
    internal class InjectionExpression : IInjectionExpression
    {
        private readonly IList<TypeDetails> _types;

        public InjectionExpression(IList<TypeDetails> types)
        {
            _types = types;
        }

        public IInjectionProperty Use<TYPE>()
        {
            return Use(typeof(TYPE));
        }

        public IInjectionProperty Use(Type type)
        {
            CheckForMoreThanOneConstructorOnType(type);
            var typeDetails = _types.SingleOrDefault(x => x.ImplementType == type);

            if (typeDetails == null)
            {
                typeDetails = CreateTypeDetails();
                typeDetails.ImplementType = type;
                typeDetails.DependenciesTypeDetails = new TypeDetails[typeDetails.ImplementType.GetConstructors().First().GetParameters().Count()];

                _types.Add(typeDetails);
            }

            return new InjectionProperty(typeDetails);
        }

        private static TypeDetails CreateTypeDetails()
        {
            return new TypeDetails
            {
                IsSingleton = false,
                SingletonObject = null,
            };
        }

        public IInjectionProperty Use(Func<object> function)
        {
            var typeDetails = CreateTypeDetails();
            typeDetails.LamdaFunction = function;

            _types.Add(typeDetails);

            return new InjectionProperty(typeDetails);
        }

        private static void CheckForMoreThanOneConstructorOnType(Type type)
        {
            if (type.GetConstructors().Count() != 1)
            {
                throw new NotSupportedException(
                    string.Format("Resolving for multiple {0} constructors, is not supported",
                        type.Name));
            }
        }
    }
}