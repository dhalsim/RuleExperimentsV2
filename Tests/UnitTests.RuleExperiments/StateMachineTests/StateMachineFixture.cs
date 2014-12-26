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
			IStateMachine stateMachine = SimpleStateMachine.Create();
			stateMachine.AddStartState(new StartState("Start"));

			Assert.IsNotNull(stateMachine);
			Assert.AreEqual(stateMachine.CurrentState, null);
			Assert.AreEqual(0, stateMachine.History.Count);
		}

		[Test]
		public void Should_create_a_state_machine_and_start()
		{
		    StartState start = new StartState("Start");
			State search = new State("Search", start);

			IStateMachine stateMachine = SimpleStateMachine.Create()
			    .AddStartState(start)
			    .AddState(search)
			    .Start();

			Assert.IsNotNull(stateMachine);
			Assert.IsTrue(start.To.Contains(search));
			Assert.IsTrue(search.From.Contains(start));
			Assert.AreEqual(stateMachine.CurrentState, start);
			Assert.AreEqual(1, stateMachine.History.Count);
			Assert.AreEqual("Start", stateMachine.History[0].State.Name);
		}

		[Test]
		public void Should_create_a_state_machine_and_search()
		{
		    StartState start = new StartState("Start");
			State search = new State("Search", start);

			IStateMachine stateMachine = SimpleStateMachine.Create()
			    .AddStartState(start)
			    .AddState(search)
			    .Start()
			    .NextState("Search");

			Assert.IsNotNull(stateMachine);
            Assert.IsTrue(start.To.Contains(search));
			Assert.IsTrue(search.From.Contains(start));
			Assert.AreEqual(stateMachine.CurrentState, search);

			Assert.AreEqual(2, stateMachine.History.Count);
			Assert.AreEqual("Start", stateMachine.History[0].State.Name);
			Assert.AreEqual("Search", stateMachine.History[1].State.Name);
		}
	}
}

