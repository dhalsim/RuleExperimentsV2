using System;

namespace HaveBox.Configuration
{
    public interface IInjectionExpression
    {
        IInjectionProperty Use<TYPE>();
        IInjectionProperty Use(Type type);
        IInjectionProperty Use(Func<object> function);
    }
}