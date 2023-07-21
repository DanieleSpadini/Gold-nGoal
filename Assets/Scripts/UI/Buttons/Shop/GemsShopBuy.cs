using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class GemsShopBuy : MonoBehaviour
{
	[Tooltip("Dollars for buying")]
	public float _dollarsRequired;

	[Tooltip("Gems that the player is buying")]
	public int _gemsAdded;

	#region Refs
	[HideInInspector]
	public TextMeshProUGUI _buyPanelText;

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

	public UnityAction _buyingAction;



	void Awake()
	{
		gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "$" + _dollarsRequired.ToString();
		_buyingAction += BuyWithDollars;
	}

	/// <summary>
	/// This method is assigned to a button to make a purchase
	/// </summary>
	public void Buy()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_swipe.enabled = false;
		_buyPanelText.text = $"Spend ${_dollarsRequired} to buy {_gemsAdded} Gems?";
		ShopPanels(true, false, true);
	}


	public void BuyWithDollars()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.WIND_CHIME);
		int totalGems = MenuUIManager.s_UIManagerInstance.GetGems();

		totalGems += _gemsAdded;

		MenuUIManager.s_UIManagerInstance.SetGems(totalGems, totalGems.ToString());
		PlayerPrefs.SetInt("Gems", totalGems);
		ShopPanels(false, false, false);
		_swipe.enabled = true;
	}

	public void ShopPanels(bool greyPanel, bool notBuyPanel, bool buyPanel)
	{
		_greyPanel.SetActive(greyPanel);
		_notBuyingPanel.SetActive(notBuyPanel);
		_buyingPanel.SetActive(buyPanel);
		if (_buyingPanel.activeSelf)
		{
			_yesButton.GetComponent<Button>().onClick.RemoveAllListeners();
			_yesButton.GetComponent<Button>().onClick.AddListener(_buyingAction);
		}
	}
}
