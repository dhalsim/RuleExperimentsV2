namespace Domain.RuleExperiments.Attributes.Rule
{
    public class BaseRule : BaseAttribute
    {
        public string Name { get; set; }

        public BaseRule(string name)
        {
            Name = name;
        }
    }
}