using System;

namespace Domain.RuleExperiments.Interfaces
{
    public interface IMethodDecorator
    {
        void OnEntry(string methodName, string className);

        void OnException(Exception exception);

        void OnExit();
    }
}