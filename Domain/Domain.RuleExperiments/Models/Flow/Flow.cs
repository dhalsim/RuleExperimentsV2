using System;
using Domain.RuleExperiments.Attributes.Rule;

namespace Domain.RuleExperiments.Models.Flow
{
    public class Flow
    {
        public BaseRule Rule { get; set; }

        public DateTime Time { get; set; }

        public Enum ProviderType { get; set; }

        public FlowLogs FlowLogs { get; set; }

        public Flow(BaseRule rule, Enum providerType)
        {
            Time = DateTime.Now;
            FlowLogs = new FlowLogs();

            Rule = rule;
            ProviderType = providerType;
        }
    }
}