using System;

public class FuncAction : IAction
{
	public Action Action;

	public FuncAction(Action action)
	{
		Action = action;
	}

	public void Perform(IContext context)
	{
		if (Action != null)
		{
			Action();
		}
	}
}
