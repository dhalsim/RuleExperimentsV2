using Domain.RuleExperiments.Models.Flow;

namespace Domain.RuleExperiments.Interfaces
{
    public interface IFlowLogger
    {
        void AddMethod(FlowLog flow);
        FlowLogs GetFlowLogs();
    }
}