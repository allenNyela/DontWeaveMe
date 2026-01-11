using System;
using UnityEngine;

namespace Weave.Controller
{
	public class RetrievableResourceSingleton<T> : MonoBehaviour where T : RetrievableResourceSingleton<T>
	{
		protected virtual void OnCreated()
		{
		}

		public static T GetInstanceWithoutCreating()
		{
			return RetrievableResourceSingleton<T>._instance;
		}


		public static T Instance
		{
			get
			{
				if (RetrievableResourceSingleton<T>.m_shuttingDown)
				{
					return RetrievableResourceSingleton<T>._instance;
				}
				if (RetrievableResourceSingleton<T>._instance == null)
				{
					RetrievableResourceSingleton<T>._instance = RetrievableResourceSingleton<T>.CreateInstance();
				}
				return RetrievableResourceSingleton<T>._instance;
			}
		}

		private static T CreateInstance()
		{
			if (!Application.isPlaying)
			{
				throw new Exception("Trying to access RetrievableResourceSingleton can only be used in play mode. Specified type: " + typeof(T).Name);
			}
			RetrievableResourceSingleton<T>._instance = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(typeof(T).Name)).GetComponent<T>();
			RetrievableResourceSingleton<T>._instance.OnCreated();
			return RetrievableResourceSingleton<T>._instance;
		}

		private void OnApplicationQuit()
		{
			RetrievableResourceSingleton<T>.m_shuttingDown = true;
		}

		private static bool m_shuttingDown;

		private static T _instance;
	}
}
