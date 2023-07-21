using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class ChestButton : MonoBehaviour
{

	#region Chest Panel Refs
	[HideInInspector]
	public GameObject ChestPanel;

	[HideInInspector]
	public Button OpenChestButton;

	[HideInInspector]
	public MenuSwipe Swipe;

	[HideInInspector]
	public TextMeshProUGUI TimerTextInSeconds;

	[HideInInspector]
	public Image ChestImage;

	[HideInInspector]
	public TextMeshProUGUI ChestName;

	[HideInInspector]
	public GameObject UnlockingPanel;

	[HideInInspector]
	public Transform ContentParent;

	public VideoClip OpeningAnimation;

	#endregion

	private enum Rarity
	{
		COMMON,
		RARE,
		EPIC
	}

	[SerializeField]
	private Rarity rarity;

	[SerializeField]
	private ChestTypes _chestType;

	[SerializeField]
	private HeroPanelAttributes[] _heroAttributes;

	private TextMeshProUGUI _timerInMenu;

	private List<HeroPanelAttributes> _commonHeroes = new List<HeroPanelAttributes>();
	private List<HeroPanelAttributes> _rareHeroes = new List<HeroPanelAttributes>();
	private List<HeroPanelAttributes> _epicHeroes = new List<HeroPanelAttributes>();

	private int _rndCommon = 0;
	private int _rndRare = 0;
	private int _rndEpic = 0;
	private int _rndTotal = 0;

	private UnityAction OpenChestAction;



	void OnEnable()
	{
		OpenChestAction += UnlockChest;
		_timerInMenu = GetComponentInChildren<TextMeshProUGUI>();
		int timer = _chestType.UnlockTimeInSeconds;
		_timerInMenu.text = $"{timer / 3600}h {timer % 3600 / 60}min";
	}

	public void OpenChestPanel()
	{
		if (gameObject.transform.parent != null)
		{
			MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
			ChestPanel.SetActive(true);

			// This just makes sure that you cannot scroll when the chest panel is active.
			Swipe.enabled = false;

			int timer = _chestType.UnlockTimeInSeconds;

			// Setting up the chest panel with all informations
			ChestImage.sprite = _chestType.Art;
			ChestName.text = _chestType.ChestName;
			TimerTextInSeconds.text = $"{timer / 3600}h {timer % 3600 / 60}min";
			OpenChestButton.onClick.RemoveAllListeners();
			OpenChestButton.onClick.AddListener(OpenChestAction);
		}
		else
		{
			// This is when a chest is unlocked via shop, so the panel doesn't have to pop up
			UnlockChest();
		}
	}

	/// <summary>
	/// This method is assigned to the "Open chest" buttons with the onclick event
	/// </summary>
	public void UnlockChest()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		int gold = _chestType.GoldEarned;

		switch (rarity)
		{
			case Rarity.COMMON:
				OpeningPanelAnimation();
				AddGoldAndGems(gold);
				AddGoldAndGemsToUnlockPanel();
				OpenCommonChest();
				
				break;
			case Rarity.RARE:
				OpeningPanelAnimation();
				AddGoldAndGems(gold);
				AddGoldAndGemsToUnlockPanel();
				OpenRareChest();
				break;
			case Rarity.EPIC:
				OpeningPanelAnimation();
				AddGoldAndGems(gold);
				AddGoldAndGemsToUnlockPanel();
				OpenEpicChest();
				break;
		}
	}

	/// <summary>
	/// Method that updates the amount of gems and gold, even in the UI
	/// </summary>
	private void AddGoldAndGems(int goldEarned)
	{
		int totalGold = MenuUIManager.s_UIManagerInstance.GetGold();

		totalGold += goldEarned;
		MenuUIManager.s_UIManagerInstance.SetGold(totalGold, totalGold.ToString());
		PlayerPrefs.SetInt("Gold", totalGold);
		if (rarity != Rarity.COMMON)
		{
			int totalGems = MenuUIManager.s_UIManagerInstance.GetGems();
			totalGems += _chestType.GemsEarned;
			MenuUIManager.s_UIManagerInstance.SetGems(totalGems, totalGems.ToString());
			PlayerPrefs.SetInt("Gems", totalGems);
		}

		CloseChestPanel();

		// This is to make sure that once the chest is opened, it's destroyed
		Destroy(gameObject);
	}

	public void CloseChestPanel()
	{
		ChestPanel.SetActive(false);
		Swipe.enabled = true;
	}

	private void OpenCommonChest()
	{
		UnlockingPanel.SetActive(true);
		
		// Taking a random heroes from each list

		_rndCommon = Random.Range(0, _commonHeroes.Count);
		_rndRare = Random.Range(0, _rareHeroes.Count);
		_rndTotal = Random.Range(0, 100);

		foreach (HeroPanelAttributes heroes in _heroAttributes)
		{
			// Adding each hero type to the list
			if (heroes.Rarity == "Common")
			{
				_commonHeroes.Add(heroes);
			}
		}
		// Adding a card to the random hero selected 
		_commonHeroes[_rndCommon].CardsAvailable++;
		
		// Updating the panel with the image of the hero that gets a card
		AddImageToUnlockPanel(_rndCommon, "Common");


		foreach (HeroPanelAttributes heroes in _heroAttributes)
		{
			if (heroes.Rarity == "Rare")
			{
				_rareHeroes.Add(heroes);
			}
		}

		// Random choosing if the chest gives a common or rare hero (70%/30%)
		if (_rndTotal < 30)
		{
			_rareHeroes[_rndRare].CardsAvailable++;
			AddImageToUnlockPanel(_rndRare, "Rare");
		}
		else
		{
			_rndCommon = Random.Range(0, _commonHeroes.Count);
			_commonHeroes[_rndCommon].CardsAvailable++;
			AddImageToUnlockPanel(_rndCommon, "Common");
		}
	}

	private void OpenRareChest()
	{
		OpenCommonChest();

		foreach (HeroPanelAttributes heroes in _heroAttributes)
		{
			if (heroes.Rarity == "Epic")
			{
				_epicHeroes.Add(heroes);
			}
		}

		// This has to be redone to take each time a different random percentage
		_rndTotal = Random.Range(0, 100);
		_rndRare = Random.Range(0, _rareHeroes.Count);

		// Random choosing if the chest gives a rare or epic hero (70%/30%)
		if (_rndTotal < 30)
		{
			_rndEpic = Random.Range(0, _epicHeroes.Count);
			_epicHeroes[_rndEpic].CardsAvailable++;
			AddImageToUnlockPanel(_rndEpic, "Epic");
		}
		else
		{
			_rareHeroes[_rndRare].CardsAvailable++;
			AddImageToUnlockPanel(_rndRare, "Rare");
		}
	}
	private void OpenEpicChest()
	{
		OpenRareChest();

		_rndEpic = Random.Range(0, _epicHeroes.Count);
		_epicHeroes[_rndEpic].CardsAvailable++;
		AddImageToUnlockPanel(_rndEpic, "Epic");
	}

	/// <summary>
	/// Creating a gameobject with a text that will have a content with grid layout group as a parent
	/// </summary>
	private void AddImageToUnlockPanel(int heroNumber, string rarity)
	{
		Swipe.enabled = false;

		// Creating a gameobject with an image that will have a content with grid layout group as a parent
		GameObject image = new GameObject();
		image.name = "HeroSprite";
		image.AddComponent<Image>();

		if (rarity == "Common")
		{
			// Those steps are to assing the hero that gets the card image to the unlocking panel image
			image.GetComponent<Image>().sprite = _commonHeroes[heroNumber].Art;
			image.transform.SetParent(ContentParent);
		}
		else if (rarity == "Rare")
		{
			image.GetComponent<Image>().sprite = _rareHeroes[heroNumber].Art;
			image.transform.SetParent(ContentParent);
		}
		else if (rarity == "Epic")
		{
			image.GetComponent<Image>().sprite = _epicHeroes[heroNumber].Art;
			image.transform.SetParent(ContentParent);
		}


	}

	/// <summary>
	/// Creating a gameobject with a text that will have a content with grid layout group as a parent
	/// </summary>
	private void AddGoldAndGemsToUnlockPanel()
	{
		
		GameObject goldPanel = new GameObject();
		goldPanel.name = "GoldQuantity";
		goldPanel.AddComponent<TextMeshProUGUI>();

		if (_chestType.name == "Common")
		{
			SetUpChestGoldLootPanel(goldPanel);
		}

		else if (_chestType.name == "Rare")
		{
			SetUpChestGoldLootPanel(goldPanel);

			GameObject gemsPanel = new GameObject();

			SetUpChestGemsLootPanel(gemsPanel);
		}

		else if (_chestType.name == "Epic")
		{
			SetUpChestGoldLootPanel(goldPanel);

			GameObject gemsPanel = new GameObject();

			SetUpChestGemsLootPanel(gemsPanel);
		}
	}

	/// <summary>
	/// Creating a little panel when a chest is unlocked with the gold amount
	/// </summary>
	private void SetUpChestGoldLootPanel(GameObject goldPanel)
	{
		TextMeshProUGUI goldText = goldPanel.GetComponent<TextMeshProUGUI>();
		goldText.enableAutoSizing = true;
		goldText.alignment = TextAlignmentOptions.Center;
		goldText.text = $"{_chestType.GoldEarned} gold";
		goldPanel.transform.SetParent(ContentParent);
	}

	/// <summary>
	/// Creating a little panel when a chest is unlocked with the gems amount
	/// </summary>
	/// <param name="gemsPanel"></param>
	private void SetUpChestGemsLootPanel(GameObject gemsPanel)
	{
		gemsPanel.name = "GemsQuantity";
		gemsPanel.AddComponent<TextMeshProUGUI>();
		TextMeshProUGUI gemsText = gemsPanel.GetComponent<TextMeshProUGUI>();
		gemsText.text = $"{_chestType.GemsEarned} gems";
		gemsText.enableAutoSizing = true;
		gemsText.alignment = TextAlignmentOptions.Center;
		gemsPanel.transform.SetParent(ContentParent);
	}

	/// <summary>
	/// Shows the animation clip
	/// </summary>
	private void OpeningPanelAnimation()
	{
		ChestsManager instance = ChestsManager.s_ChestsManagerInstance;
		instance.AnimationPanel.SetActive(true);
		VideoPlayer player = instance.Player;
		player.clip = OpeningAnimation;
		player.SetDirectAudioVolume(0, MenuSoundManager.s_SoundManagerInstance._volumeSlider.value);
		player.Play();
		player.loopPointReached += ClosingPanelAnimation;
	}

	/// <summary>
	/// This method is called when the opening animation is over, it just closes the panel
	/// </summary>
	/// <param name="vp"></param>
	private void ClosingPanelAnimation(VideoPlayer vp)
	{
		ChestsManager instance = ChestsManager.s_ChestsManagerInstance;
		vp = instance.Player;
		vp.Stop();
		instance.AnimationPanel.SetActive(false);
	}
}