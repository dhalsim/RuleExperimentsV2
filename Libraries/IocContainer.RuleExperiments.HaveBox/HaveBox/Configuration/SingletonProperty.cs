
namespace HaveBox.Configuration
{
    public class SingletonProperty : InjectionProperty, ISingletonProperty
    {
        internal SingletonProperty(TypeDetails typeDetails)
            : base(typeDetails)
        {}

        public IInjectionProperty WithPerContainerLifeTime()
        {
            _typeDetails.PerContainerLifeTime = true;
            return new InjectionProperty(_typeDetails);
        }
    }
}
