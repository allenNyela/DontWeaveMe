using System;

namespace Weave.Controller
{
	public class InputKey : InputBase
	{
		public InputKey(string inputName) : base(inputName)
		{
		}

		public bool GetKeyUp()
		{
			return this.Action.WasCompletedThisFrame();
		}

		public bool GetKeyDown()
		{
			return this.Action.WasPerformedThisFrame();
		}

		public bool GetKey()
		{
			return this.Action.IsInProgress();
		}
	}
}
