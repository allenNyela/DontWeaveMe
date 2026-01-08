using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GameObjectExtensions
{
	public static T AddComponentIfNotExist<T>(this GameObject go) where T : Component
	{

		if (go == null)
		{
			Debug.Log("AddComponentIfNotExist param 'go' is null.");
			return null;
		}

		var t = go.GetComponent<T>();
		if (t == null) t = go.AddComponent<T>();
		return t;
	}
}

