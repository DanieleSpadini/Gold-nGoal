using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _pausePanel;

	[SerializeField]
	private GameObject _checkPanel;

	[SerializeField, Tooltip("Button click effect reference")]
	private AudioClip _sfxButtonClick;


	public void Resume()
	{
		if (_checkPanel.activeSelf)
		{
			_checkPanel.SetActive(false);
		}
		else
		{
			_pausePanel.SetActive(false);
		}

		SoundManager.Instance.PlayEffect(_sfxButtonClick);
	}

	public void BackToMenu()
	{
		SceneManager.LoadScene(0);
		SoundManager.Instance.PlayEffect(_sfxButtonClick);
	}

	public void OpenPausePanel()
	{
		_pausePanel.SetActive(true);
		SoundManager.Instance.PlayEffect(_sfxButtonClick);
	}

	public void OpenCheck()
	{
		_checkPanel.SetActive(true);
		SoundManager.Instance.PlayEffect(_sfxButtonClick);
	}
}
