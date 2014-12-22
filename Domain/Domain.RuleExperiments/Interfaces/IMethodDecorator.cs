namespace Domain.RuleExperiments.Interfaces
{
    public interface IMethodDecorator
    {
        void OnEntry();

        void OnException();

        void OnExit();
    }
}