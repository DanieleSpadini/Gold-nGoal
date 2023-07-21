using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialPages : MonoBehaviour
{
	[SerializeField]
	private Sprite[] _images;

	[SerializeField]
	private Image _imagePanel;

	[SerializeField]
	private string[] _texts;

	[SerializeField]
	private TextMeshProUGUI _textField;

	[SerializeField]
	private TextMeshProUGUI _nextButtonText;

	[SerializeField]
	private GameObject _namePanel;

	[SerializeField]
	private Button _nextButton;

	[SerializeField]
	private Button _previousButton;

	[SerializeField]
	private MenuSwipe _swipe;

	private int _counter = 0;

	[SerializeField, Space(20), Header("Debug"), Tooltip("If true it saves the 'first time' tutorial")]
	private bool _savingTutorialStatus = true;

	private int _tutorialPanelCounter = 0;

	private UnityAction _firstOpening;

	private void Start()
	{
		_tutorialPanelCounter = PlayerPrefs.GetInt("Counter");

		if (_tutorialPanelCounter > 0)
		{
			gameObject.SetActive(false);
		}
		else
		{
			_swipe.enabled = false;
			gameObject.SetActive(true);
		}

	}

	/// <summary>
	/// This slide forward the tutorial images and texts
	/// </summary>
	public void NextPage()
	{
		
		if (!_previousButton.gameObject.activeSelf)
		{
			_previousButton.gameObject.SetActive(true);
		}

		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		if (_counter >= 0 && _counter < _images.Length - 1)
		{
			_counter++;
			_textField.text = _texts[_counter];
			_imagePanel.sprite = _images[_counter];
		}

		// If it's the last page, the next time the button is clicked the tutorial panel is closed
		else if (_counter == _images.Length - 1)
		{
			ClosePanel();
		}


		if (_counter == _images.Length - 1)
		{
			// If it's the first time opening the application, after all the pages opens the name panel to set it
			if (_tutorialPanelCounter == 0)
			{
				_firstOpening += OpenNamePanel;
				_nextButton.onClick.AddListener(_firstOpening);
			}
			_nextButtonText.text = "Let's play!";
		}
	}

	/// <summary>
	/// This slide backwards the tutorial images and texts
	/// </summary>
	public void PreviousPage()
	{
		MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
		if (_counter > 0 && _counter <= _images.Length)
		{
			_counter--;
			_textField.text = _texts[_counter];
			_imagePanel.sprite = _images[_counter];
			if (_counter == 0)
			{
				_previousButton.gameObject.SetActive(false);
			}

			_nextButtonText.text = "Next";
			
			
			if (_counter == _images.Length -2)
			{
                if (_firstOpening != null)
				{
					_nextButton.onClick.RemoveListener(_firstOpening);
					_firstOpening -= OpenNamePanel;
				}
			}
		}
	}
	/// <summary>
	/// Closes the panel resetting it
	/// </summary>
	public void ClosePanel()
	{
		_nextButton.onClick.RemoveAllListeners();
		_counter = 0;
		_imagePanel.sprite = _images[_counter];
		_textField.text = _texts[_counter];
		_nextButtonText.text = "Next";
		_tutorialPanelCounter++;
		_previousButton.gameObject.SetActive(false);
		PlayerPrefs.SetInt("Counter", _tutorialPanelCounter);
		gameObject.SetActive(false);
	}

	private void OpenNamePanel()
	{
		_namePanel.SetActive(true);
		gameObject.SetActive(false);
	}

#if UNITY_EDITOR
	private void OnEnable()
	{
		if (_savingTutorialStatus == false)
		{
			PlayerPrefs.DeleteKey("Counter");
		}
	}
#endif
}
