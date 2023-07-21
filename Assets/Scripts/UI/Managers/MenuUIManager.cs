using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuUIManager : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _goldText;

	[SerializeField]
	private TextMeshProUGUI _gemsText;

	[SerializeField]
	private TextMeshProUGUI _playerName;

	[SerializeField]
	private TextMeshProUGUI _timeLeftToFill;

	[SerializeField]
	private TextMeshProUGUI _goldSliderAmountText;

	[SerializeField]
	private TextMeshProUGUI _nameInSettings;

	[SerializeField]
	private Slider _goldSlider;

	[SerializeField]
	private int _goldSliderAmount;

	[SerializeField]
	private MenuSwipe _swipe;

	private int _gold;
	private int _gems;
	private int totalCoinsPile = 3000;
	private int _timeLeft;

	private float _timer;

	private bool _canMatchStart = false;

	#region Panels
	[Space(20), Header("Panels")]

	[SerializeField]
	private GameObject _teamCheckPanel;

	[SerializeField]
	private GameObject _settingsPanel;

	[SerializeField]
	private GameObject _namePanel;

	[SerializeField]
	private GameObject _tutorialPanel;

	[SerializeField]
	private GameObject _creditsPanel;

	[SerializeField]
	private GameObject _resetProgressPanel;
	#endregion

	void Start()
	{
		_playerName.text = PlayerPrefs.GetString("PlayerName");
		_nameInSettings.text = PlayerPrefs.GetString("PlayerName");
		_gold = PlayerPrefs.GetInt("Gold");
		_gems = PlayerPrefs.GetInt("Gems");

		_goldText.text = _gold.ToString();
		_gemsText.text = _gems.ToString();
	}

	void Update()
	{
		// Adding gold to the gold pile each 12 seconds
		_timer += Time.deltaTime;
		if (_timer >= 12)
		{
			if (_goldSliderAmount < totalCoinsPile)
			{
				AddGoldToPile();
			}
			_timer = 0f;
		}

		_timeLeft = (totalCoinsPile - _goldSliderAmount) * 12;
		_timeLeftToFill.text = $"{_timeLeft / 3600}h: {(_timeLeft % 3600) / 60}m";
	}


	public void SetGold(int gold, string goldText)
	{
		this._gold = gold;
		this._goldText.text = goldText;
	}

	public int GetGold()
	{
		return this._gold;
	}

	public void SetGems(int gems, string gemsText)
	{
		this._gems = gems;
		this._gemsText.text = gemsText;
	}

	public int GetGems()
	{
		return this._gems;
	}

	public void SetName(string playerName)
	{
		this._playerName.text = playerName;
	}

	public string GetName()
	{
		return this._playerName.text;
	}

	/// <summary>
	/// It checks if the team is complete, if not opens a panel and directs the aplayer to the team screen
	/// </summary>
	public void ChangeSceneToMatch()
	{
		TeamManager.s_TeamManagerInstance.LoadDeck();
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		foreach (RectTransform child in TeamManager.s_TeamManagerInstance._selectedHeroesSlots)
		{
			if (child.childCount == 0)
			{
				OpenTeamPanel();
				_canMatchStart = false;
				break;
			}
			else
			{
				_canMatchStart = true;
			}
		}

		if (_canMatchStart)
		{
			SceneManager.LoadScene(1, LoadSceneMode.Single);
		}
	}

	
	private void AddGoldToPile()
	{
		_goldSliderAmount++;
		_goldSlider.value = _goldSliderAmount;
		_goldSliderAmountText.text = $"{_goldSliderAmount} / 3000";
	}

	/// <summary>
	/// Method assigned to a button that put the player on the team screen when the team is not complete
	/// </summary>
	public void GoToTeam()
	{
		_swipe._buttons[2].onClick.Invoke();
		_teamCheckPanel.SetActive(false);
	}

	public void OpenTeamPanel()
	{
		_teamCheckPanel.SetActive(true);
	}

	public void OpenSettingsPanel()
	{
		MenuSoundManager.s_SoundManagerInstance._volumeSlider.value = PlayerPrefs.GetFloat("Volume",70f);
		_settingsPanel.SetActive(true);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_swipe.enabled = false;
		_nameInSettings.text = _playerName.text;
	}

	public void CloseSettingsPanel()
	{
		_settingsPanel.SetActive(false);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_swipe.enabled = true;
	}

	public void OpenTutorialPanel()
	{
		_tutorialPanel.SetActive(true);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
	}

	public void OpenCreditsPanel()
	{
		_creditsPanel.SetActive(true);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
	}

	public void CloseCreditsPanel()
	{
		_creditsPanel.SetActive(false);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
	}

	public void ChangeName()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		_namePanel.SetActive(true);
		_settingsPanel.SetActive(false);
	}

	public void OpenResetPanel()
	{
		_resetProgressPanel.SetActive(true);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
	}

	public void CloseResetPanel()
	{
		_resetProgressPanel.SetActive(false);
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
	}

	public void ResetPreferences()
	{
		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene(0);
	}

	#region Singleton
	public static MenuUIManager s_UIManagerInstance { get; private set; }

	private void Awake()
	{
		if (s_UIManagerInstance != null && s_UIManagerInstance != this)
		{
			DestroyImmediate(this.gameObject);
		}
		else
		{
			s_UIManagerInstance = this;
		}
	}
	#endregion

}
