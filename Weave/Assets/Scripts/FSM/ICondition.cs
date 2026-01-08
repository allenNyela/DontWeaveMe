namespace fsm
{
	public interface ICondition
	{
		bool Validate(IContext context);
	}
}
