using System.Collections.Generic;

namespace fsm
{
	public class AnyConditions : ICondition
	{
		private readonly IEnumerable<ICondition> m_conditions;

		public AnyConditions(params ICondition[] conditions)
		{
			m_conditions = conditions;
		}

		public bool Validate(IContext context)
		{
			foreach (ICondition condition in m_conditions)
			{
				if (condition.Validate(context))
				{
					return true;
				}
			}
			return false;
		}
	}
}
