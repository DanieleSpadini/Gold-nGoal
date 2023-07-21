using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Core;
using Character;
using System;

public class TeamManager : MonoBehaviour
{
	public RectTransform[] _selectedHeroesSlots;

	[SerializeField]
	private DraggableHero[] _heroButtons;

	[SerializeField]
	private Player[] _heroPrefabs;

	[SerializeField]
	private MenuSwipe _swipe;

	[SerializeField]
	private RectTransform _parent;

	#region Hero Panel
	[Space(20), Header("Hero panel reference")]

	[SerializeField]
	private GameObject _heroPanel;

	[SerializeField]
	private Image _heroArt;

	[SerializeField]
	private TextMeshProUGUI _heroActiveName;

	[SerializeField]
	private TextMeshProUGUI _heroActiveEffect;

	[SerializeField]
	private TextMeshProUGUI _heroName;

	[SerializeField]
	private TextMeshProUGUI _heroSpeed;

	[SerializeField]
	private TextMeshProUGUI _heroRarity;

	[SerializeField]
	private TextMeshProUGUI _levelProgression;

	[SerializeField]
	private Button _upgradeButton;

	[SerializeField]
	private GameObject[] _heroModel3D;

	[SerializeField]
	private Transform _spawnPoint;
	#endregion

	#region Upgrade Panel
	[Space(20), Header("Upgrade panel reference")]

	[SerializeField]
	private GameObject _upgradePanel;

	[SerializeField]
	private Button _gotItNO;

	[SerializeField]
	private Button _shopYES;

	[SerializeField]
	private TextMeshProUGUI _textPanel;
	#endregion


	[Space(20), Header("Debug")]
	public Player[] Team = new Player[2];

	public GameObject[] _savedHeros = new GameObject[6];

	public static TeamManager s_TeamManagerInstance { get; private set; }


	void Awake()
	{
		if (s_TeamManagerInstance != null && s_TeamManagerInstance != this)
		{
			DestroyImmediate(this.gameObject);
		}
		else
		{
			s_TeamManagerInstance = this;
		}


		// Instantiating hero buttons and giving them the reference to the team slots

		for (int i = 0; i < _heroButtons.Length; i++)
		{
			DraggableHero drag = Instantiate<DraggableHero>(_heroButtons[i], _parent);
			drag._swipe = _swipe;
			drag._parent = _parent;

			drag._3DHeroModel = _heroModel3D[i];
			drag._spawnPoint = _spawnPoint;
			drag._art = _heroArt;
			drag._name = _heroName;
			drag._abilityEffect = _heroActiveEffect;
			drag._abilityName = _heroActiveName;
			drag._speed = _heroSpeed;
			drag._rarity = _heroRarity;
			drag._heroPanel = _heroPanel;
			drag._levelProgression = _levelProgression;
			drag._upgradeButton = _upgradeButton;

			drag._upgradePanel = _upgradePanel;
			drag._gotItNO = _gotItNO;
			drag._shopYES = _shopYES;
			drag._textPanel = _textPanel;


			for (int j = 0; j < _selectedHeroesSlots.Length; j++)
			{
				drag._slot[j] = _selectedHeroesSlots[j];
			}
		}


	}


	public void CloseHeroPanel()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_swipe.enabled = true;

		// Check to destroy the 3D model once the panel is closed
		if (_spawnPoint.childCount > 0)
		{
			Destroy(_spawnPoint.GetChild(0).gameObject);
		}
		_heroPanel.SetActive(false);
	}

	/// <summary>
	/// This is called to load the players in the deck
	/// </summary>
	public void LoadPlayers(string slot, int index)
	{
		if (slot != "Empty")
		{
			for (int i = 0; i < _parent.transform.childCount; i++)
			{
				_savedHeros[i] = _parent.transform.GetChild(i).gameObject;

			}
			for (int i = 0; i < _savedHeros.Length; i++)
			{
				DraggableHero hero = _savedHeros[i].GetComponent<DraggableHero>();
				if (hero._character.GetType() == Type.GetType("Character." + slot))
				{
					hero.CheckForDeckChanges(hero.GetComponent<RectTransform>(), true, index);
					break;
				}
			}
		}
	}

	public void LoadDeck()
	{
		LoadPlayers(PlayerPrefs.GetString("0"), 0);
		LoadPlayers(PlayerPrefs.GetString("1"), 1);
		LoadPlayers(PlayerPrefs.GetString("2"), 2);
	}

	public void SaveDeck()
	{
		for (int i = 0; i < _selectedHeroesSlots.Length; i++)
		{
			if (_selectedHeroesSlots[i].childCount != 0)
			{

				PlayerPrefs.SetString($"{i}", _selectedHeroesSlots[i].transform.GetChild(0).gameObject.GetComponent<DraggableHero>()._character.GetType().Name);
			}
			else
			{
				PlayerPrefs.SetString($"{i}", "Empty");
			}
		}

	}
}
