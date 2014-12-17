using HaveBox.Configuration;
using System;

namespace HaveBox
{
    public interface IContainer
    {
        void Configure(Action<IConfig> registry);        

        TYPE GetInstance<TYPE>();
        bool TryGetInstance<TYPE>(out TYPE instance);

        TYPE GetInstance<TYPE>(string implementationName);
        bool TryGetInstance<TYPE>(string implementationName, out TYPE instance);

        object GetInstance(Type type);
        bool TryGetInstance(Type type, out object instance);

        object GetInstance(Type type, string implementationName);
        bool TryGetInstance(Type type, string implementationName, out object instance);

        IDisposableContainer CreateChildContainer();
    }
}
