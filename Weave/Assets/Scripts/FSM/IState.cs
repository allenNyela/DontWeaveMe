

using UnityEngine;

namespace fsm
{
	public interface IState
	{
		void UpdateState(float deltaTime);

		void Update();

		void LateUpdate();

		void OnEnter();

		void OnExit();

	}
}
