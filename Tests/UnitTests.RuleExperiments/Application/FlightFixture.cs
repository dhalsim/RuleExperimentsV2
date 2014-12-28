using System;
using NUnit.Framework;
using HaveBox;
using StateMachine;
using Application.RuleExperiments.Application.Flight;
using Domain.RuleExperiments;
using IocContainer.RuleExperiments.HaveBox;

namespace UnitTests.RuleExperiments.Application
{
	[TestFixture]
	public class FlightFixture
	{
		IContainer _container;
		Flight _flight;

		// states
		StartState _startState;
		State _searchState;

		[TestFixtureSetUp]
		public void SetUp()
		{
			// instantiate states
			_startState = new StartState("Started");
			_searchState = new State("Search", _startState);

			_container = new Container();
			_container.Configure(cfg => cfg.For<BaseStateMachine>().Use<SimpleStateMachine>().AsSingleton());

			IocContainerFactory.Current = new IocContainerImplementation(_container);

			var stateMachine = _container.GetInstance<IStateMachine>();
			stateMachine.AddStartState(_startState)
				.AddState(_searchState);

			_flight = new Flight(IocContainerFactory.Current.GetInstance<IStateMachine>());
		}

		[Test]
		public void Should_search_state_machine()
		{
			Assert.AreEqual(_flight.StateMachine.CurrentState, _startState);
			Assert.AreEqual(_flight.StateMachine.History.Count, 0);

			_flight.Search();

			Assert.AreEqual(_flight.StateMachine.CurrentState, _searchState);
			Assert.AreEqual(_flight.StateMachine.History.Count, 1);
			Assert.AreEqual(_flight.StateMachine.History[0].State, _startState);
		}
	}
}

