using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class ChestShopGems : MonoBehaviour
{
	[Tooltip("Gems for buying a chest")]
	public int _gemsRequired;

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

	private enum ChestRarity
	{
		COMMON,
		RARE,
		EPIC
	}

	[SerializeField]
	private ChestRarity _rarity;

	public UnityAction BuyingAction;


	void Start()
	{
		BuyingAction += BuyWithGems;

		if (gameObject.name == "ChestShopReward1Match" ||
			gameObject.name == "ChestShopRewardAd" ||
			gameObject.name == "ChestShopRewardWeek")
		{
			// This is just to not set the text under the rewards buttons
		}
		else
		{
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _gemsRequired.ToString();
		}
	}

	/// <summary>
	/// This method is assigned to a button to make a purchase
	/// </summary>
	public void Buy()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		// Disabling the scroll script, enabling it when the buttons close the panel
		_swipe.enabled = false;

		int totalGems = MenuUIManager.s_UIManagerInstance.GetGems();

		// It opens a panel and show the text based on if the player has gems or not
		if (totalGems < _gemsRequired)
		{
			ShowShopPanels(true, true, false);
			_cantBuyPanelText.text = "Not enough Gems to buy this";
		}
		else
		{
			// It opens the panel to buy the chest
			_buyPanelText.text = $"Spend {_gemsRequired} Gems to buy this chest?";
			ShowShopPanels(true, false, true);
		}
	}

	/// <summary>
	/// It activates or deactivates the buying panels
	/// </summary>
	/// <param name="greyPanel"></param>
	/// <param name="notBuyPanel"></param>
	/// <param name="buyPanel"></param>
	public void ShowShopPanels(bool greyPanel, bool notBuyPanel, bool buyPanel)
	{
		_greyPanel.SetActive(greyPanel);
		_notBuyingPanel.SetActive(notBuyPanel);
		_buyingPanel.SetActive(buyPanel);

		if (_buyingPanel.activeSelf)
		{
			_yesButton.GetComponent<Button>().onClick.RemoveAllListeners();
			_yesButton.GetComponent<Button>().onClick.AddListener(BuyingAction);
		}
	}

	/// <summary>
	/// Removes the gems from the player and updating the text of it
	/// </summary>
	public void BuyWithGems()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.KNOCK);

		int totalGems = MenuUIManager.s_UIManagerInstance.GetGems();

		totalGems -= _gemsRequired;
		MenuUIManager.s_UIManagerInstance.SetGems(totalGems, totalGems.ToString());
		ShowShopPanels(false, false, false);
		OpenImmediatlyChest();
	}

	public void OpenImmediatlyChest()
	{
		switch (_rarity)
		{
			case ChestRarity.COMMON:
				InstanceChest(0);
				break;
			case ChestRarity.RARE:
				InstanceChest(1);
				break;
			case ChestRarity.EPIC:
				InstanceChest(2);
				break;
		}
	}

	/// <summary>
	/// Giving all the reference from the manager to the chest
	/// </summary>
	/// <param name="rarity"></param>
	private void InstanceChest(int rarity)
	{
		ChestsManager instance = ChestsManager.s_ChestsManagerInstance;

		ChestButton chest = Instantiate<ChestButton>(instance.Chests[rarity]);
		chest.ChestPanel = instance.ChestPanel;
		chest.Swipe = instance.MenuSwipe;
		chest.ChestName = instance.ChestName;
		chest.ChestImage = instance.ChestImage;
		chest.TimerTextInSeconds = instance.TimerTextInSeconds;
		chest.OpenChestButton = instance.OpenChestButton;
		chest.ContentParent = instance.ContentParent;
		chest.UnlockingPanel = instance.UnlockingPanel;
		chest.GetComponent<Button>().onClick.Invoke();
	}
}
