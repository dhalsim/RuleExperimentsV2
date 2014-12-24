using System;
using Domain.RuleExperiments.Interfaces;
using Domain.RuleExperiments.Models.Log;

namespace Domain.RuleExperiments.Attributes
{
    public class LoggerDecoratorAttribute : BaseAttribute, IMethodDecorator
    {
        private ILogger Logger { get; set; }
        
        public LoggerDecoratorAttribute() { }

        public LoggerDecoratorAttribute(ILogger logger)
        {
            Logger = logger;
        }

        public void OnEntry(string methodName, string className)
        {
            Logger.Log(new Log(LogLevel.Trace, string.Format("{0} of type {1} entered", methodName, className)));
        }

        public void OnException(Exception exception)
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}