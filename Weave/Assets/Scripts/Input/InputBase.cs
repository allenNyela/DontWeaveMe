using UnityEngine;
using UnityEngine.InputSystem;


namespace Weave.Controller
{
	public abstract class InputBase
	{
		protected InputBase(string inputName)
		{
			this.Action = InputSystem.actions[inputName];
		}

		public readonly InputAction Action;
	}
}

