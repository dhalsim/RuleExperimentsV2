using System;
using System.Collections.Generic;

namespace Domain.RuleExperiments.Attributes.StateMachine
{
	public class BaseStateAttribute : BaseAttribute
	{
		public List<string> To { get; set; }

		public BaseStateAttribute (params string[] to)
		{
			To = new List<string>();
			To.AddRange(to);
		}
	}
}

