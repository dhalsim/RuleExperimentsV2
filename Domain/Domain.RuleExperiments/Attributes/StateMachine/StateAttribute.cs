using System;

namespace Domain.RuleExperiments.Attributes.StateMachine
{
	public class StateAttribute : BaseStateAttribute
	{
		public StateAttribute (params string[] to) : base(to)
		{
		}
	}
}

