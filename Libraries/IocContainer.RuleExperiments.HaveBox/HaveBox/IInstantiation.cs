using System;
using System.Collections.Generic;

namespace HaveBox
{
    public interface IInstantiation
    {
        IEnumerable<Type> Arguments { get; }
        object[] InstancesToBeInjected { get; set; }
        void Proceed();
    }
}
