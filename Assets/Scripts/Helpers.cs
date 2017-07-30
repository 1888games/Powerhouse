using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Helpers {

	private static System.Random rng = new System.Random();  

	public static void Shuffle<T>(this IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	public static GameObject FindParentWithTag(GameObject childObject, string tag) {

		Transform t = childObject.transform;

		while (t.parent != null) {

			if (t.parent.tag == tag) {
				return t.parent.gameObject;
			}
			t = t.parent.transform;
		}
		return null;
	}



}
