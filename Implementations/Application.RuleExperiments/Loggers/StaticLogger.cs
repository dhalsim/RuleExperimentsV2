using Domain.RuleExperiments.Interfaces;
using Domain.RuleExperiments.Models.Log;

namespace Application.RuleExperiments.Loggers
{
    public class StaticLogger : ILogger
    {
        private Logs Logs { get; set; }

        public StaticLogger()
        {
            Logs = new Logs();
        }

        public void Log(Log log)
        {
            Logs.Add(log);
        }

        public void Clear()
        {
            Logs.Clear();
        }

        public Logs GetLogs()
        {
            return Logs;
        }
    }
}