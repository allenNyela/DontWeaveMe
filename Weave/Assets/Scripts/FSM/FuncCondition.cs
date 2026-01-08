using System;

namespace fsm
{
	public class FuncCondition : ICondition
	{
		private readonly Func<bool> m_function;

		public FuncCondition(Func<bool> function)
		{
			m_function = function;
		}

		public bool Validate(IContext context)
		{
			if (m_function != null)
			{
				return m_function();
			}
			return true;
		}
	}
}
