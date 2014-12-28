using System;
using StateMachine;

namespace Application.RuleExperiments.Application.Flight
{
	public class Flight
	{
		public IStateMachine StateMachine { get; set; }

		public Flight (IStateMachine stateMachine)
		{
			StateMachine = stateMachine;
		}

		public void Search()
		{

		}

		public void Reset()
		{

		}

		public void Result()
		{

		}

		public void Basket()
		{

		}

		public void Passenger()
		{

		}

		public void Book()
		{

		}

		public void Summary()
		{

		}
	}
}

