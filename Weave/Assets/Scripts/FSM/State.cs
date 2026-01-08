using System;
using UnityEngine;

namespace fsm
{
	public class State : IState
	{
		public Action UpdateStateEvent = delegate
		{
		};

		public Action OnEnterEvent = delegate
		{
		};

		public Action OnExitEvent = delegate
		{
		};

		public void UpdateState(float deltaTime)
		{
			UpdateStateEvent();
		}

		public void OnEnter()
		{
			OnEnterEvent();
		}

		public void OnExit()
		{
			OnExitEvent();
		}

        public void Update()
        {

        }

		public void OnCollisionStay(Collision collision)
        {

        }

        public void LateUpdate()
        {

        }
    }
}
