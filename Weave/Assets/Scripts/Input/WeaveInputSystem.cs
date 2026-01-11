using UnityEngine;
using Weave.Controller;

public static class WeaveInputSystem
{
	public static bool CanTakeInput()
	{
		return true;
	}

	public static readonly InputVector2 Move = new InputVector2("Move");

	public static readonly InputKey Dive = new InputKey("Dive");

	public static readonly InputKey MainAbility = new InputKey("Ability");

	public static readonly InputKey Interact = new InputKey("Interact");

	public static readonly InputKey FastRun = new InputKey("FastRun");


}
