using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParentCheck : MonoBehaviour
{
	[SerializeField]
	private MenuSwipe _swipe;

	public void ParentChecker()
	{
		if (gameObject.activeInHierarchy)
		{
			_swipe.enabled = false;
		}
		else
		{
			_swipe.enabled = true;
		}
	}
}
