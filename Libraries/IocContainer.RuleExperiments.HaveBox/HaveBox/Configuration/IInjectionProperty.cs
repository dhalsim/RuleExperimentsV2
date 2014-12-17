using System;

namespace HaveBox.Configuration
{
    public interface IInjectionProperty
    {
        ISingletonProperty AsSingleton();
        ISingletonProperty AsLazySingleton();
        IInjectionProperty AndInterceptInstantiationWith<TYPE>();
        IInjectionProperty AndInterceptInstantiationWith(IInstantiationInterceptor interceptor);
#if !SILVERLIGHT
        IInjectionProperty AndInterceptMethodsWith<TYPE>();
        IInjectionProperty AndInterceptMethodsWith(Type interceptor);
#endif
    }
}
