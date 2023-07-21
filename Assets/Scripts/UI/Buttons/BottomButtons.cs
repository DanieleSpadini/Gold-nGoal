using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BottomButtons : MonoBehaviour
{
	[SerializeField]
	private MenuSwipe _menu;

	[SerializeField, Range(0,2)]
	private int _pageIndex;

	// Method assigned to the buttons in the bottom bar to switch between pages
	public void SwitchPage()
	{
		TeamManager.s_TeamManagerInstance.LoadDeck();
		_menu._nextPage = _pageIndex;
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
	}
}
