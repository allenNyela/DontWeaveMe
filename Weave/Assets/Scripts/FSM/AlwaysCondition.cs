namespace fsm
{
	public class AlwaysCondition : ICondition
	{
		public bool Validate(IContext context)
		{
			return true;
		}
	}
}
