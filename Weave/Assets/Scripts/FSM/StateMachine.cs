
using System;
using System.Collections.Generic;
using UnityEngine;

namespace fsm
{
	public class StateMachine
	{
		public ITrigger CurrentTrigger;

		public Action OnStateChanged;

		private List<IState> m_states = new List<IState>(8);

		private Dictionary<Type, TransitionManager> m_triggerToTransitionManagers = new Dictionary<Type, TransitionManager>();

		public float CurrentStateTime { get; set; }

		public IState CurrentState { get; set; }

		public void ChangeState(IState state)
		{
			if (CurrentState != null)
			{
				CurrentState.OnExit();
			}
			CurrentState = state;
			CurrentStateTime = 0;
			if (CurrentState != null)
			{
				CurrentState.OnEnter();
			}
			if (OnStateChanged != null)
			{
				OnStateChanged();
			}
		}

		public void UpdateState(float dt)
		{
			if (CurrentState != null)
			{
				CurrentState.UpdateState(dt);
			}
			CurrentStateTime += dt;
		}

		public void Update()
        {
			if (CurrentState != null)
			{
				CurrentState.Update();
			}
		}

		public void LateUpdate()
        {
			if (CurrentState != null)
			{
				CurrentState.LateUpdate();
			}
		}

		public StateConfigurator Configure(IState state)
		{
			return new StateConfigurator(this, state);
		}

		public TransitionManager FindTransitionManager(Type trigger)
		{
			if (m_triggerToTransitionManagers.TryGetValue(trigger, out var value))
			{
				return value;
			}
			value = new TransitionManager();
			m_triggerToTransitionManagers.Add(trigger, value);
			return value;
		}

		public void Trigger(ITrigger trigger)
		{
			CurrentTrigger = trigger;
			if (m_triggerToTransitionManagers.TryGetValue(trigger.GetType(), out var value))
			{
				value.Process(this);
			}
		}

		public void Trigger<T>() where T : ITrigger
		{
			CurrentTrigger = null;
			if (m_triggerToTransitionManagers.TryGetValue(typeof(T), out var value))
			{
				value.Process(this);
			}
		}

		public TransitionManager GetTransistionManager<T>()
		{
			if (m_triggerToTransitionManagers.TryGetValue(typeof(T), out var value))
			{
				return value;
			}
			return null;
		}

		public void RegisterStates(params IState[] states)
		{
			foreach (IState item in states)
			{
				m_states.Add(item);
			}
		}

		private int StateToIndex(IState state)
		{
			return m_states.FindIndex((IState a) => a == state);
		}

		private IState IndexToState(int index)
		{
			return m_states[index];
		}
	}
}
