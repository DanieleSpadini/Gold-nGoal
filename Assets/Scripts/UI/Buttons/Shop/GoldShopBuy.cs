using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class GoldShopBuy : MonoBehaviour
{
	[Tooltip("Gems for buying")]
	public int _gemsRequired;

	[Tooltip("Gold that the player is buying")]
	public int _goldAdded;

	#region
	[HideInInspector]
	public TextMeshProUGUI BuyPanelText;

	[HideInInspector]
	public TextMeshProUGUI CantBuyPanelText;

	[HideInInspector]
	public GameObject GreyPanel;

	[HideInInspector]
	public GameObject NotBuyingPanel;

	[HideInInspector]
	public GameObject BuyingPanel;

	[HideInInspector]
	public GameObject YesButton;

	[HideInInspector]
	public MenuSwipe Swipe;
	#endregion


	public UnityAction _buyingAction;



	void Awake()
	{
		gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _gemsRequired.ToString();
		_buyingAction += BuyWithGems;
	}

	/// <summary>
	/// This method is assigned to a button to make a purchase
	/// </summary>
	public void Buy()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		Swipe.enabled = false;
		int totalGems = MenuUIManager.s_UIManagerInstance.GetGems();

		if (totalGems < _gemsRequired)
		{
			ShopPanels(true, true, false);
			CantBuyPanelText.text = "Not enough Gems to buy this";
		}
		else
		{
			BuyPanelText.text = $"Spend {_gemsRequired} Gems to buy {_goldAdded} Gold?";
			ShopPanels(true, false, true);
		}
	}


	public void BuyWithGems()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.COINS_MOVEMENT);

		int totalGems = MenuUIManager.s_UIManagerInstance.GetGems();
		int totalGold = MenuUIManager.s_UIManagerInstance.GetGold();

		totalGems -= _gemsRequired;
		totalGold += _goldAdded;

		MenuUIManager.s_UIManagerInstance.SetGems(totalGems, totalGems.ToString());
		MenuUIManager.s_UIManagerInstance.SetGold(totalGold, totalGold.ToString());
		PlayerPrefs.SetInt("Gold", totalGold);
		PlayerPrefs.SetInt("Gems", totalGems);

		ShopPanels(false, false, false);
		Swipe.enabled = true;
	}

	public void ShopPanels(bool greyPanel, bool notBuyPanel, bool buyPanel)
	{
		GreyPanel.SetActive(greyPanel);
		NotBuyingPanel.SetActive(notBuyPanel);
		BuyingPanel.SetActive(buyPanel);

		if (BuyingPanel.activeSelf)
		{
			YesButton.GetComponent<Button>().onClick.RemoveAllListeners();
			YesButton.GetComponent<Button>().onClick.AddListener(_buyingAction);
		}
	}
}
