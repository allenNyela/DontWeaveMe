using System;
using UnityEngine;

namespace fsm
{
	public class CompoundState : IState
	{
		private readonly IState[] m_states;

		public Action OnUpdateEvent = delegate
		{
		};

		public Action OnEnterEvent = delegate
		{
		};

		public Action OnExitEvent = delegate
		{
		};

		public CompoundState(params IState[] states)
		{
			m_states = states;
		}

        public void LateUpdate()
        {
			IState[] states = m_states;
			foreach (IState state in states)
			{
				state.LateUpdate();
			}
		}

        public void OnEnter()
		{
			IState[] states = m_states;
			foreach (IState state in states)
			{
				state.OnEnter();
			}
			OnEnterEvent();
		}

		public void OnExit()
		{
			IState[] states = m_states;
			foreach (IState state in states)
			{
				state.OnExit();
			}
			OnExitEvent();
		}

        public void Update()
        {
			IState[] states = m_states;
			foreach (IState state in states)
			{
				state.Update();
			}
		}

		public void UpdateState(float deltaTime)
		{
			IState[] states = m_states;
			foreach (IState state in states)
			{
				state.UpdateState(deltaTime);
			}
			OnUpdateEvent();
		}
	}
}
