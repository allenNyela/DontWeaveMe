using System;

namespace Weave.Controller
{
	public class InputAxis : InputBase
	{
		public InputAxis(string inputName) : base(inputName)
		{
		}

		public float GetValue()
		{
			return this.Action.ReadValue<float>();
		}
	}
}
