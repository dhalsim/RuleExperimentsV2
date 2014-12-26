using NUnit.Framework;
using StateMachine;

namespace UnitTests.RuleExperiments.StateMachineTests
{
	[TestFixture]
	public class StateMachineFixture
	{
		[Test]
		public void Should_create_a_state_machine()
		{
            StartState start = new StartState("Start");
		    State search = new State("search", start);

		    var stateMachine = SimpleStateMachine.Create()
                .AddStartState(start)
		        .AddState(search)
		        .Start()
		        .NextState("search");
            
			Assert.IsNotNull(stateMachine);
            Assert.IsTrue(start.To.Contains(search));
			Assert.IsTrue(search.From.Contains(start));
            Assert.AreEqual(stateMachine.CurrentState, search);
		}
	}
}

