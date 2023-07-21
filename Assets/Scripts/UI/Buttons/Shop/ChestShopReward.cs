using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class ChestShopReward : MonoBehaviour
{
	#region Refs
	[HideInInspector]
	public TextMeshProUGUI _buyPanelText;

	[HideInInspector]
	public TextMeshProUGUI _cantBuyPanelText;

	[HideInInspector]
	public GameObject _greyPanel;

	[HideInInspector]
	public GameObject _notBuyingPanel;

	[HideInInspector]
	public GameObject _buyingPanel;

	[HideInInspector]
	public GameObject _yesButton;

	[HideInInspector]
	public MenuSwipe _swipe;
	#endregion

	private int _counterAds = 0;
	private int _counterMatch = 0;
	private int _counterWeekly = 0;


	private enum ButtonType
	{
		MATCH,
		ADS,
		WEEKLY
	}

	[SerializeField]
	private ButtonType button;

	void Start()
	{
		if (button == ButtonType.MATCH)
		{
			if (MatchResult.s_MatchResult.GamesCounter == 1)
			{
				gameObject.GetComponent<Button>().interactable = true;
			}
			else
			{
				gameObject.GetComponent<Button>().interactable = false;
			}
		}
	}
	/// <summary>
	/// This method is assigned to daily buttons
	/// </summary>
	public void Open()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.KNOCK);
		// Disabling the scroll script, enabling it when the buttons close the panel
		_swipe.enabled = false;

		switch (button)
		{
			case ButtonType.WEEKLY:
				if (_counterWeekly == 0)
				{
					_cantBuyPanelText.text = "Watching weekly ad...";
					ShowShopPanels(true, true, false);
					_counterWeekly++;
					gameObject.GetComponent<Button>().interactable = false;
				}
				break;
			case ButtonType.MATCH:
				gameObject.GetComponent<Button>().interactable = true;
				if (_counterMatch == 0)
				{
					_cantBuyPanelText.text = "Chest unlocked";
					ShowShopPanels(true, true, false);
					_counterMatch++;
					gameObject.GetComponent<Button>().interactable = false;
				}
				break;
			case ButtonType.ADS:
				if (_counterAds == 0)
				{
					_cantBuyPanelText.text = "Watching ad...";
					ShowShopPanels(true, true, false);
					_counterAds++;
					gameObject.GetComponent<Button>().interactable = false;
				}
				break;
		}
	}

	/// <summary>
	/// It activates or deactivates the buying panels
	/// </summary>
	public void ShowShopPanels(bool greyPanel, bool notBuyPanel, bool buyPanel)
	{
		_greyPanel.SetActive(greyPanel);
		_notBuyingPanel.SetActive(notBuyPanel);
		_buyingPanel.SetActive(buyPanel);
	}
}


