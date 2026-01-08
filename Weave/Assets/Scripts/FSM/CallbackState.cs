using System;
using UnityEngine;

namespace fsm
{
	public class CallbackState : IState
	{
		private readonly IState m_state;

		public Action UpdateStateEvent = delegate
		{
		};

		public Action OnEnterEvent = delegate
		{
		};

		public Action OnExitEvent = delegate
		{
		};

		public CallbackState(IState state)
		{
			m_state = state;
		}

		public CallbackState AddUpdateStateAction(Action updateStateAction)
		{
			UpdateStateEvent = (Action)Delegate.Combine(UpdateStateEvent, updateStateAction);
			return this;
		}

		public void UpdateState(float deltaTime)
		{
			m_state.UpdateState(deltaTime);
			UpdateStateEvent();
		}

		public void OnEnter()
		{
			m_state.OnEnter();
			OnEnterEvent();
		}

		public void OnExit()
		{
			m_state.OnExit();
			OnExitEvent();
		}

        public void Update()
        {
			m_state.Update();
		}

        public void LateUpdate()
        {
			m_state.LateUpdate();
		}

    }
}
