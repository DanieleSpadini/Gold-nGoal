using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class HeroShopGold : MonoBehaviour
{
	[Tooltip("Gold for buying a card")]
	public int GoldRequired;

	[Tooltip("Number of cards available in the shop")]
	public int NumberOfCards;

	public TextMeshProUGUI NumberCardsText;

	#region Panel Refs

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

	public UnityAction BuyingAction;

	[SerializeField]
	private HeroPanelAttributes _HeroAttributes;

	void Awake()
	{
		gameObject.GetComponentInChildren<TextMeshProUGUI>().text = GoldRequired.ToString();
		BuyingAction += BuyWithGold;
	}

	/// <summary>
	/// This method is assigned to a button to make a purchase
	/// </summary>
	public void Buy()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);

		// Disabling the scroll script, enabling it when the buttons close the panel
		Swipe.enabled = false;

		int totalGold = MenuUIManager.s_UIManagerInstance.GetGold();

		// It opens a panel and show the text based on whether there aren't cards or gold
		if (totalGold < GoldRequired || NumberOfCards <= 0)
		{
			ShowShopPanels(true, true, false);
			if (NumberOfCards <= 0)
			{
				CantBuyPanelText.text = "Cards are out";
			}
			else
			{
				CantBuyPanelText.text = "You don't have enough Gold to buy this";
			}
		}
		else
		{
			// It opens the panel to buy the cards
			ShowShopPanels(true, false, true);
			if (NumberOfCards > 0)
			{
				BuyPanelText.text = $"Spend {GoldRequired} gold to buy this hero?";
			}
		}
	}

	/// <summary>
	/// Removes the gold from the player and updating the text of it
	/// </summary>
	public void BuyWithGold()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.COMIC_METAL);

		int totalGold = MenuUIManager.s_UIManagerInstance.GetGold();

		totalGold -= GoldRequired;
		MenuUIManager.s_UIManagerInstance.SetGold(totalGold, totalGold.ToString());
		PlayerPrefs.SetInt("Gold", totalGold);
		NumberOfCards--;
		NumberCardsText.text = NumberOfCards.ToString();
		_HeroAttributes.CardsAvailable++;
		ShowShopPanels(false, false, false);
	}

	/// <summary>
	/// It activates or deactivates the buying panels
	/// </summary>
	public void ShowShopPanels(bool greyPanel, bool notBuyPanel, bool buyPanel)
	{
		GreyPanel.SetActive(greyPanel);
		NotBuyingPanel.SetActive(notBuyPanel);
		BuyingPanel.SetActive(buyPanel);
		
		if (BuyingPanel.activeSelf)
		{
			YesButton.GetComponent<Button>().onClick.RemoveAllListeners();
			YesButton.GetComponent<Button>().onClick.AddListener(BuyingAction);
		}
	}
}
