using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyChildrens : MonoBehaviour
{
	// Script assigned to the content of Unlocking Panel to destroy it children each time is disabled,
	// so the panel is empty when a chest is unlocked

	[SerializeField]
	private Scrollbar _scroll;

	private void OnDisable()
	{
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		// This is to reset the position of the scrollbar each time the panel is closed
		_scroll.value = 1;
	}
}
