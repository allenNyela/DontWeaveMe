using System;
using System.Collections.Generic;

namespace fsm
{
	public class AllConditions : ICondition
	{
		private readonly IEnumerable<ICondition> m_conditions;

		public AllConditions(params ICondition[] conditions)
		{
			m_conditions = conditions;
		}

		public AllConditions(params Func<bool>[] conditions)
		{
			ICondition[] array = new FuncCondition[conditions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new FuncCondition(conditions[i]);
			}
			m_conditions = array;
		}

		public bool Validate(IContext context)
		{
			foreach (ICondition condition in m_conditions)
			{
				if (!condition.Validate(context))
				{
					return false;
				}
			}
			return true;
		}
	}
}
