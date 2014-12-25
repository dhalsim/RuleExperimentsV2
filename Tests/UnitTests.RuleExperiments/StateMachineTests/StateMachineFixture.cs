﻿using NUnit.Framework;
using StateMachine;

namespace UnitTests.RuleExperiments.StateMachineTests
{
	[TestFixture]
	public class StateMachineFixture
	{
		[Test]
		public void Should_create_a_state_machine()
		{
			IStateMachine stateMachine = SimpleStateMachine.Create();
			StartState start = new StartState("Start");
			State search = new State("Search", start);

			stateMachine.AddState(start)
				.AddState(search);

			Assert.IsNotNull(stateMachine);
			Assert.IsTrue(start.To.Contains(search));
			Assert.IsTrue(search.From.Contains(start));
		}
	}
}
