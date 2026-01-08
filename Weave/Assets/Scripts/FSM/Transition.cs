namespace fsm
{
	public class Transition
	{
		public IState SourceState;

		public IState TargetState;

		public ICondition Condition;

		public IAction Action;

		public Transition(IState sourceState, IState targetState, ICondition condition, IAction action)
		{
			SourceState = sourceState;
			TargetState = targetState;
			Condition = condition;
			Action = action;
		}
	}
}
