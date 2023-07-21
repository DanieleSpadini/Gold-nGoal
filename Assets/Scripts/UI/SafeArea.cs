using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Menu
{
	// Script to make some UI elements fit in the safe area
	public class SafeArea : MonoBehaviour
	{
		private RectTransform _safeAreaPanel;

		private void Start()
		{
			_safeAreaPanel = GetComponent<RectTransform>();
			SetSafeArea(Screen.safeArea);
		}

		private void SetSafeArea(Rect r)
		{
			Vector2 anchorMin = r.position;
			Vector2 anchorMax = r.position + r.size;

			anchorMin.x /= Screen.width;
			anchorMin.y /= Screen.height;
			anchorMax.x /= Screen.width;
			anchorMax.y /= Screen.height;

			_safeAreaPanel.anchorMin = anchorMin;
			_safeAreaPanel.anchorMax = anchorMax;
		}
	}
}

