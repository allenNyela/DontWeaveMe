using System;
using UnityEngine;

namespace Weave.Controller
{
	public class InputVector2 : InputBase
	{
		public InputVector2(string inputName) : base(inputName)
		{
		}

		public Vector2 GetValue()
		{
			return this.Action.ReadValue<Vector2>();
		}
	}
}
