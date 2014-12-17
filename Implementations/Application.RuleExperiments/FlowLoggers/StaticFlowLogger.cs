using Domain.RuleExperiments.Interfaces;
using Domain.RuleExperiments.Models.Flow;

namespace Application.RuleExperiments.FlowLoggers
{
    public class StaticFlowLogger : IFlowLogger
    {
        private FlowLogs FlowLogs { get; set; }

        public StaticFlowLogger()
        {
            FlowLogs = new FlowLogs();
        }

        public void AddMethod(FlowLog flow)
        {
            FlowLogs.Add(flow);
        }

        public FlowLogs GetFlowLogs()
        {
            return FlowLogs;
        }
    }
}