using Domain.RuleExperiments.Models.Log;

namespace Domain.RuleExperiments.Interfaces
{
    public interface ILogger
    {
        void Log(Log log);
        void Clear();
        Logs GetLogs();
    }
}