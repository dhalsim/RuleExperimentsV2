using System;
using System.Collections.Generic;
using StateMachine;
using System.Security.Cryptography;

namespace StateMachine
{
	public class States : List<State>
	{

	}

	public class State
	{
		public States To { get; set; }

		public States From { get; set; }

		public String Name { get; set; }

		protected State (string name)
		{
			Name = name;
			To = new States();
			From = new States();
		}

		public State (string name, State from) : this(name)
		{
			from.To.Add(this);
			From.Add(from);
		}
	}

	public class StartState : State
	{
		public StartState (string name) : base(name)
		{
		}
	}

	public interface IStateMachine : IStartState
	{

	}

	public interface IStartState
	{
		IBuildState AddState(StartState state);
	}

	public interface IBuildState
	{
		void AddState(State state);
	}

	public abstract class BaseStateMachine : IStateMachine, IStartState, IBuildState
	{
		private States States { get; set; }

		private StartState StartState { get; set; }

		public State CurrentState { get; private set; }

		protected BaseStateMachine ()
		{
			States = new States();
		}

		public void AddState(State state)
		{
			States.Add(state);
		}

		public IBuildState AddState(StartState state)
		{
			States.Add(state);
			StartState = state;
			return this;
		}
	}

	public class SimpleStateMachine : BaseStateMachine
	{
		protected SimpleStateMachine ()
		{

		}

		public static IStateMachine Create()
		{
			return new SimpleStateMachine();
		}
	}
}

