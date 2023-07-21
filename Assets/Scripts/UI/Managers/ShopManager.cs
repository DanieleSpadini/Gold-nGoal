using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _buyPanelText;

	[SerializeField]
	private TextMeshProUGUI _cantBuyPanelText;


	[SerializeField]
	private GameObject _greyPanel;

	[SerializeField]
	private GameObject _notBuyingPanel;

	[SerializeField]
	private GameObject _buyingPanel;

	[SerializeField]
	private GameObject _yesButton;



	[SerializeField]
	private MenuSwipe _swipe;


	#region Shop buttons
	[SerializeField]
	private HeroShopGold[] _prefabHeroShopGold;

	[SerializeField]
	private ChestShopReward[] _prefabChestShopReward;

	[SerializeField]
	private ChestShopGems[] _prefabChestShopGems;

	[SerializeField]
	private GoldShopBuy[] _prefabGoldShopBuy;

	[SerializeField]
	private GemsShopBuy[] _prefabGemsShopBuy;
	#endregion

	[SerializeField]
	private Transform[] _grids;



	public static ShopManager s_ShopManagerInstance { get; private set; }

	private void Awake()
	{
		if (s_ShopManagerInstance != null && s_ShopManagerInstance != this)
		{
			DestroyImmediate(this.gameObject);
		}
		else
		{
			s_ShopManagerInstance = this;
		}
	}

	private void Start()
	{
		// Spawing the first row of the shop, with all the references
		for (int i = 0; i < _prefabHeroShopGold.Length; i++)
		{
			HeroShopGold button = Instantiate<HeroShopGold>(_prefabHeroShopGold[i], _grids[0]);
			button.name = _prefabHeroShopGold[i].name;
			button.BuyPanelText = _buyPanelText;
			button.CantBuyPanelText = _cantBuyPanelText;
			button.GreyPanel = _greyPanel;
			button.NotBuyingPanel = _notBuyingPanel;
			button.BuyingPanel = _buyingPanel;
			button.YesButton = _yesButton;
			button.Swipe = _swipe;
			button.NumberCardsText.text = button.NumberOfCards.ToString();
		}

		// Spawing the second row of the shop, with all the references
		for (int i = 0; i < _prefabChestShopReward.Length; i++)
		{
			ChestShopReward button = Instantiate<ChestShopReward>(_prefabChestShopReward[i], _grids[1]);
			button.name = _prefabChestShopReward[i].name;
			button._buyPanelText = _buyPanelText;
			button._cantBuyPanelText = _cantBuyPanelText;
			button._greyPanel = _greyPanel;
			button._notBuyingPanel = _notBuyingPanel;
			button._buyingPanel = _buyingPanel;
			button._yesButton = _yesButton;
			button._swipe = _swipe;
		}

		// Spawing the third row of the shop, with all the references
		for (int i = 0; i < _prefabChestShopGems.Length; i++)
		{
			ChestShopGems button = Instantiate<ChestShopGems>(_prefabChestShopGems[i], _grids[2]);
			button.name = _prefabChestShopGems[i].name;
			button._buyPanelText = _buyPanelText;
			button._cantBuyPanelText = _cantBuyPanelText;
			button._greyPanel = _greyPanel;
			button._notBuyingPanel = _notBuyingPanel;
			button._buyingPanel = _buyingPanel;
			button._yesButton = _yesButton;
			button._swipe = _swipe;
		}

		// Spawing the fourth row of the shop, with all the references
		for (int i = 0; i < _prefabGoldShopBuy.Length; i++)
		{
			GoldShopBuy button = Instantiate<GoldShopBuy>(_prefabGoldShopBuy[i], _grids[3]);
			button.name = _prefabGoldShopBuy[i].name;
			button.BuyPanelText = _buyPanelText;
			button.CantBuyPanelText = _cantBuyPanelText;
			button.GreyPanel = _greyPanel;
			button.NotBuyingPanel = _notBuyingPanel;
			button.BuyingPanel = _buyingPanel;
			button.YesButton = _yesButton;
			button.Swipe = _swipe;
		}

		// Spawing the fifth row of the shop, with all the references
		for (int i = 0; i < _prefabGemsShopBuy.Length; i++)
		{
			GemsShopBuy button = Instantiate<GemsShopBuy>(_prefabGemsShopBuy[i], _grids[4]);
			button.name = _prefabGemsShopBuy[i].name;
			button._buyPanelText = _buyPanelText;
			button._greyPanel = _greyPanel;
			button._notBuyingPanel = _notBuyingPanel;
			button._buyingPanel = _buyingPanel;
			button._yesButton = _yesButton;
			button._swipe = _swipe;
		}
	}

	public void ClosePanel()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_greyPanel.SetActive(false);
	}
}
