using System;

namespace fsm
{
	public class StateConfigurator
	{
		private readonly StateMachine m_stateMachine;

		private readonly IState m_state;

		public StateConfigurator(StateMachine stateMachine, IState state)
		{
			m_stateMachine = stateMachine;
			m_state = state;
		}

		public StateConfigurator AddTransition<T>(IState to, ICondition condition = null, IAction action = null) where T : ITrigger
		{
			m_stateMachine.FindTransitionManager(typeof(T)).AddTransition(m_state, to, condition, action);
			return this;
		}

		public StateConfigurator AddTransition<T>(IState to, Func<bool> condition, Action action = null) where T : ITrigger
		{
			m_stateMachine.FindTransitionManager(typeof(T)).AddTransition(m_state, to, condition, action);
			return this;
		}
	}
}
