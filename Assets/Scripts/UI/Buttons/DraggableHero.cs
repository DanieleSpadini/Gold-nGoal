using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;
using TMPro;
using UnityEngine.Events;

using Menu;
using Core;
using Ability;

public class DraggableHero : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	public HeroPanelAttributes _heroPanelAttributes;

	public Player _character;

	private bool _isDragging = false;

	[SerializeField]
	private RectTransform _thisHeroRect;

	#region Refs
	[HideInInspector]
	public GameObject _heroPanel;

	[HideInInspector]
	public Image _art;

	[HideInInspector]
	public TextMeshProUGUI _name;

	[HideInInspector]
	public int _level;

	[HideInInspector]
	public TextMeshProUGUI _abilityName;

	[HideInInspector]
	public TextMeshProUGUI _abilityEffect;

	[HideInInspector]
	public TextMeshProUGUI _speed;

	[HideInInspector]
	public TextMeshProUGUI _rarity;

	[HideInInspector]
	public TextMeshProUGUI _levelProgression;

	[HideInInspector]
	public Button _upgradeButton;

	[HideInInspector]
	public RectTransform[] _slot;

	[HideInInspector]
	public RectTransform _parent;

	[HideInInspector]
	public GameObject _3DHeroModel;

	[HideInInspector]
	public MenuSwipe _swipe;

	[HideInInspector]
	public Transform _spawnPoint;
	#endregion

	#region Upgrade Panel
	[Space(20), Header("Upgrade panel reference")]

	[HideInInspector]
	public GameObject _upgradePanel;

	[HideInInspector]
	public Button _gotItNO;

	[HideInInspector]
	public Button _shopYES;

	[HideInInspector]
	public TextMeshProUGUI _textPanel;
	#endregion


	private UnityAction _upgrade;
	private UnityAction _openUpgradePanel;
	private UnityAction _closeUpgradePanel;
	private UnityAction _goToShop;

	private void Awake()
	{
		_thisHeroRect = GetComponent<RectTransform>();

		_openUpgradePanel += OpenUpgradePanel;
		_closeUpgradePanel += CloseUpgradePanel;
		_upgrade += LevelUp;
		_goToShop += GoToShopPage;
	}

	public void OnPointerDown(PointerEventData eventData)
	{

		StartCoroutine(DragCoroutine());

	}

	/// <summary>
	/// This coroutine is the pickup of the hero
	/// </summary>
	/// <returns></returns>
	private IEnumerator DragCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.BOOT_PICK_UP);
		GetComponent<Button>().interactable = false;
		_isDragging = true;
		transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
		_swipe.enabled = false;

		// This part is for make the button visible if the player passes it over the others
		transform.SetParent(transform.root);
		transform.SetAsLastSibling();
	}


	public void OnDrag(PointerEventData eventData)
	{
		// This is to make the button follow the touch input point
		if (_isDragging)
		{
			transform.position = Touchscreen.current.position.ReadValue();
		}
	}


	public void OnPointerUp(PointerEventData eventData)
	{
		if (_isDragging)
		{
			MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.IMPACT_LOG_DROP);
		}

		StopAllCoroutines();
		_isDragging = false;
		transform.SetParent(_parent);
		transform.localScale = Vector3.one;
		_swipe.enabled = true;
		CheckForDeckChanges(_thisHeroRect);

		StartCoroutine(InteractableCoroutine());
	}

	// This coroutine is to make sure that when you release the button it doesn't count as pressed
	private IEnumerator InteractableCoroutine()
	{
		yield return new WaitForEndOfFrame();
		GetComponent<Button>().interactable = true;
	}

	/// <summary>
	/// Changes the deck when the hero is dropped on a deck slot
	/// </summary>
	public void CheckForDeckChanges(RectTransform hero, bool skip = false, int index = -1)
	{
		TeamManager instance = TeamManager.s_TeamManagerInstance;
		if (!skip)
		{
			// The array "slot" is checked if in its components there are any child
			for (int i = 0; i < _slot.Length; i++)
			{
				// If the hero rect is over a slot rect (at least in part of it) it can be part of the team
				if (RectOverlap(_slot[i], hero))
				{
					if (_slot[i].gameObject.transform.childCount != 0)
					{
						_slot[i].gameObject.transform.GetChild(0).SetParent(_parent);
						instance.Team[i] = _character;

					}

					hero.SetParent(_slot[i]);
					hero.pivot = new Vector2(0.5f, 0.5f);
					hero.localPosition = Vector2.zero;

					instance.Team[i] = _character;
				}

				// If the slot is empty, removes the character from the team list
				if (_slot[i].childCount == 0)
				{
					instance.Team[i] = null;
				}
			}

			instance.SaveDeck();
		}
		else
		{
			if (index != -1)
			{
				hero.SetParent(_slot[index]);
				hero.pivot = new Vector2(.5f, .5f);
				hero.localPosition = Vector2.zero;
				instance.Team[index] = _character;
			}
		}
	}

	// Check if the hero rect overlaps the slot rect
	private bool RectOverlap(RectTransform firstRect, RectTransform secondRect)
	{

		if (firstRect.position.x + firstRect.rect.width * 0.5f < secondRect.position.x - secondRect.rect.width * 0.5f)
		{
			return false;
		}
		if (secondRect.position.x + secondRect.rect.width * 0.5f < firstRect.position.x - firstRect.rect.width * 0.5f)
		{
			return false;
		}
		if (firstRect.position.y + firstRect.rect.height * 0.5f < secondRect.position.y - secondRect.rect.height * 0.5f)
		{
			return false;
		}
		if (secondRect.position.y + secondRect.rect.height * 0.5f < firstRect.position.y - firstRect.rect.height * 0.5f)
		{
			return false;
		}
		return true;
	}

	public void OpenHeroPanel()
	{
		SettingThePanel();
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_upgradeButton.onClick.RemoveAllListeners();
		_upgradeButton.onClick.AddListener(_openUpgradePanel);
	}

	private void SettingThePanel()
	{
		// Hero Scriptable Object
		HeroPanelAttributes heroSO = _heroPanelAttributes;

		_heroPanel.SetActive(true);

		// Instance of the 3D model of the hero
		if (_spawnPoint.childCount == 0)
		{
			Instantiate(_3DHeroModel, _spawnPoint.position, Quaternion.Euler(0, 180, 0), _spawnPoint);
		}

		// Setting the panel based on the scriptable object of the hero
		_swipe.enabled = false;
		_art.sprite = heroSO.Art;
		_level = heroSO.Level;
		_name.text = $"{heroSO.HeroName}    Level: {heroSO.Level}";
		_abilityName.text = $"Active: {heroSO.AbilityName}";
		_abilityEffect.text = heroSO.AbilityEffect;
		_speed.text = heroSO.Speed;
		_rarity.text = heroSO.Rarity;
		_levelProgression.text = $"{heroSO.CardsAvailable} / {heroSO.CardsForNextLevel}";

		if (heroSO.Level == 5)
		{
			_upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Max level";
			_upgradeButton.interactable = false;
		}
		else
		{
			_upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade with {heroSO.GoldForNextLevel} gold";
			_upgradeButton.interactable = true;
		}
	}

	private void OpenUpgradePanel()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_upgradePanel.SetActive(true);

		// Hero Scriptable Object
		HeroPanelAttributes heroSO = _heroPanelAttributes;

		int currentCards = heroSO.CardsAvailable;
		int cardsNeeded = heroSO.CardsForNextLevel;
		int goldNeeded = heroSO.GoldForNextLevel;

		int currentGold = MenuUIManager.s_UIManagerInstance.GetGold();

		// Checks for gold and cards available for the level up, changing the behavious of the buttons
		if (currentCards >= cardsNeeded && currentGold >= goldNeeded)
		{
			_textPanel.text = $"Spend {goldNeeded} Gold to upgrade this hero?";
			_gotItNO.GetComponentInChildren<TextMeshProUGUI>().text = "No";
			_gotItNO.onClick.AddListener(_closeUpgradePanel);
			_shopYES.GetComponentInChildren<TextMeshProUGUI>().text = "Yes";
			_shopYES.onClick.RemoveAllListeners();
			_shopYES.onClick.AddListener(_upgrade);
		}
		else
		{
			_textPanel.text = "You don't have enough Gold or Copies to upgrade this hero. You can get more in chests or at the shop.";
			_gotItNO.GetComponentInChildren<TextMeshProUGUI>().text = "Got it";
			_gotItNO.onClick.RemoveAllListeners();
			_gotItNO.onClick.AddListener(_closeUpgradePanel);
			_shopYES.GetComponentInChildren<TextMeshProUGUI>().text = "Shop";
			_shopYES.onClick.RemoveAllListeners();
			_shopYES.onClick.AddListener(_goToShop);
		}
	}

	private void LevelUp()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SUCCESS_FANFARE);
		// Hero Scriptable Object
		HeroPanelAttributes heroSO = _heroPanelAttributes;

		int currentLevel = heroSO.Level;
		int currentGold = MenuUIManager.s_UIManagerInstance.GetGold();

		// Level up requirements
		switch (currentLevel)
		{
			case 1:
				CloseUpgradePanel();
				PanelAfterLevelUp(heroSO, currentGold, 1000, 4);
				SettingThePanel();
				break;
			case 2:
				CloseUpgradePanel();
				PanelAfterLevelUp(heroSO, currentGold, 3000, 8);
				SettingThePanel();
				break;
			case 3:
				CloseUpgradePanel();
				PanelAfterLevelUp(heroSO, currentGold, 6000, 8);
				SettingThePanel();
				break;
			case 4:
				CloseUpgradePanel();
				PanelAfterLevelUp(heroSO, currentGold, 9999, 9999);
				SettingThePanel();
				break;
		}
	}

	/// <summary>
	/// Updating the hero panel with new requirements
	/// </summary>
	private void PanelAfterLevelUp(HeroPanelAttributes heroSO, int currentGold, int goldNextLevel, int cardsNextLevel)
	{
		MenuUIManager.s_UIManagerInstance.SetGold(currentGold - heroSO.GoldForNextLevel, (currentGold - heroSO.GoldForNextLevel).ToString());
		heroSO.Level++;
		heroSO.GoldForNextLevel = goldNextLevel;
		heroSO.CardsAvailable -= heroSO.CardsForNextLevel;
		heroSO.CardsForNextLevel = cardsNextLevel;
	}

	private void CloseUpgradePanel()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_upgradePanel.SetActive(false);
	}

	/// <summary>
	/// Method given to a button at runtime to go to the shop in the upgrade panel
	/// </summary>
	private void GoToShopPage()
	{
		_heroPanel.SetActive(false);
		_upgradePanel.SetActive(false);
		_upgradePanel.SetActive(false);
		_swipe.enabled = true;
		_swipe._buttons[0].onClick.Invoke();
	}
}
