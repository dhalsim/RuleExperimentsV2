using HaveBox.Configuration;
using System;
using System.Collections.Generic;

namespace HaveBox
{
    internal interface IDependencyStrapper
    {
        void Strap(IDictionary<Type, IList<TypeDetails>> dependencyMap);
    }
}
