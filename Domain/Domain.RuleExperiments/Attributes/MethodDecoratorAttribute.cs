using Domain.RuleExperiments.Interfaces;

namespace Domain.RuleExperiments.Attributes
{
    public class MethodDecoratorAttribute : BaseAttribute, IMethodDecorator
    {
        public void OnEntry()
        {
            throw new System.NotImplementedException();
        }

        public void OnException()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}