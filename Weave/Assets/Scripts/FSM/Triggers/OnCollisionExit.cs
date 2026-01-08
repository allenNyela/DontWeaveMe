using UnityEngine;

namespace fsm.triggers
{
	public class OnCollisionExit : ITrigger
	{
		public readonly Collision Collision;

		public OnCollisionExit(Collision collision)
		{
			Collision = collision;
		}
	}
}
