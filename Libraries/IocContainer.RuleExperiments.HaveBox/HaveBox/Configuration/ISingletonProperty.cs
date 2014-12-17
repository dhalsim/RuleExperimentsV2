
namespace HaveBox.Configuration
{
    public interface ISingletonProperty : IInjectionProperty
    {
        IInjectionProperty WithPerContainerLifeTime();
    }
}
