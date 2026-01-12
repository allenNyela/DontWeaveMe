using UnityEngine;
using Weave.Controller;

public static class WeaveInputSystem
{
	public static bool CanTakeInput()
	{
		return true;
	}

	public static readonly InputVector2 Move = new InputVector2("Move");

	public static readonly InputKey Weave = new InputKey("Weave");

	public static readonly InputKey Eat = new InputKey("Eat");

	public static readonly InputKey CancelWeave = new InputKey("CancelWeave");

	public static readonly InputKey DropNode = new InputKey("DropNode");

	public static readonly InputKey Undo = new InputKey("Undo");

	public static readonly InputKey Push = new InputKey("Push");
}
