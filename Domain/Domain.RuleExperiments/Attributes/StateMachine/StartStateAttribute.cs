namespace Domain.RuleExperiments.Attributes.StateMachine
{
	public class StartStateAttribute : BaseStateAttribute
	{
		public StartStateAttribute (params string[] to) : base(to)
		{
			
		}
	}
}

