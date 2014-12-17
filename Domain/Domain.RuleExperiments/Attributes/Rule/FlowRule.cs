using System;

namespace Domain.RuleExperiments.Attributes.Rule
{
    public class FlowRule : BaseRule
    {
        public Type ProviderType { get; set; }

        public FlowRule(string name, Type providerType) : base(name)
        {
        }
    }
}