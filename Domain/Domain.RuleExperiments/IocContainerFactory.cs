using Domain.RuleExperiments.Interfaces;

namespace Domain.RuleExperiments
{
    public static class IocContainerFactory
    {
        public static IIocContainer Current { get; set; }
    }
}
