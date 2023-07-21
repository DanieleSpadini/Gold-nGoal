using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameButton : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _name;

	[SerializeField]
	private TextMeshProUGUI _placeholder;

	[SerializeField]
	private GameObject _panel;

	/// <summary>
	/// Setting the name, checking the string if null or too long (max 10 characters)
	/// </summary>
	public void SetName()
	{
		if (string.IsNullOrEmpty(_name.text))
		{
			_placeholder.text = "Invalid name";
		}
		else
		{
			MenuUIManager.s_UIManagerInstance.SetName(_name.text);
			MenuSoundManager.s_SoundManagerInstance.PlayClipImmediatly(Clips.SWITCH_ON_METAL);
			PlayerPrefs.SetString("PlayerName", _name.text);
			_panel.SetActive(false);
		}
	}
}
