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

        protected State(string name)
        {
            Name = name;
            To = new States();
            From = new States();
        }

        public State(string name, State from)
            : this(name)
        {
            from.To.Add(this);
            From.Add(from);
        }
    }

    public class StartState : State
    {
        public StartState(string name)
            : base(name)
        {
        }
    }

    public interface IStateMachine : IStartStateAddable
    {

    }

    public interface IGetCurrentState
    {
        State CurrentState { get; }
    }

    public interface IStartStateAddable
    {
        IBuildableState AddStartState(StartState state);
    }

    public interface IBuildableState
    {
        IInitializableState AddState(State state);
    }

    public interface IInitializableState : IBuildableState
    {
        IUsableState Start();
    }

    public interface IUsableState : IGetCurrentState
    {
        IUsableState NextState(string stateName);
    }

    public class StateLog
    {
        public State State { get; set; }

        public DateTime Time { get; set; }

        public StateLog(State state)
        {
            State = state;
            Time = DateTime.Now;
        }
    }

    public class StateHistory : List<StateLog>
    {
        
    }

    public abstract class BaseStateMachine : IStateMachine, IInitializableState, IUsableState
    {
        private States States { get; set; }

        private StartState StartState { get; set; }

        public State CurrentState { get; private set; }

        public StateHistory StateHistory { get; set; }

        protected BaseStateMachine()
        {
            States = new States();
            StateHistory = new StateHistory();
        }

        public IInitializableState AddState(State state)
        {
            States.Add(state);
            return this;
        }

        public IUsableState NextState(string stateName)
        {
            var state = States.Find(s => s.Name == stateName);
            StateHistory.Add(new StateLog(state));
            CurrentState = state;
            return this;
        }

        public IUsableState Start()
        {
            StateHistory.Add(new StateLog(StartState));
            CurrentState = StartState;
            return this;
        }

        public IBuildableState AddStartState(StartState state)
        {
            States.Add(state);
            StartState = state;
            return this;
        }
    }

    public class SimpleStateMachine : BaseStateMachine
    {
        protected SimpleStateMachine()
        {

        }

        public static IStateMachine Create()
        {
            return new SimpleStateMachine();
        }
    }
}

