using System;
using System.Collections.Generic;
using System.Reflection;

namespace HaveBox.Configuration
{
    public interface IConfig
    {
        IInjectionExpression For<TYPE>();
        IInjectionExpression For(Type type);

        void MergeConfig<CONFIG>();
        void MergeConfig(IConfig config);
    }
}
