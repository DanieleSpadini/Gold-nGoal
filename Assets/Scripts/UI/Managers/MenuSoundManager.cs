using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum Clips
{
	SWITCH_ON_METAL = 0,
	MAGIC_SPELL_WHOOSH = 1,
	BOOT_PICK_UP = 2,
	IMPACT_LOG_DROP = 3,
	WIND_CHIME = 4,
	COINS_MOVEMENT = 5,
	COMIC_METAL = 6,
	KNOCK = 7,
	BELT_CLOTH_SLIDE = 8,
	DOOR_CREAK_DOOR = 9,
	MAGICAL_BURST = 10,
	SUCCESS_FANFARE = 11
}

public class MenuSoundManager : MonoBehaviour
{
	public Slider _volumeSlider;

	[SerializeField]
	private AudioMixer _menuAudioMixer;
	
	[SerializeField]
	private AudioClip[] sound;

	private AudioSource source;



	void Start()
	{
		source = GetComponent<AudioSource>();

		_volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
		_menuAudioMixer.SetFloat("MasterVolume", Mathf.Log10(_volumeSlider.value) * 20);
		
	}

	public void SetVolume()
	{
		float volume = _volumeSlider.value;
		_menuAudioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
		PlayerPrefs.SetFloat("Volume", volume);
	}

	public void PlayClipImmediatly(Clips clip)
	{
		source.PlayOneShot(sound[(int)clip]);
	}

	public void PlayClipOneAfterAnother(Clips clip)
	{
		StartCoroutine(WaitUntilSoundFinish(clip));
	}

	// This coroutine is for scheduling one sound after another
	public IEnumerator WaitUntilSoundFinish(Clips clip)
	{
		while (source.isPlaying)
		{
			yield return null;
		}
		source.PlayOneShot(sound[(int)clip]);
	}


	#region Singleton
	public static MenuSoundManager s_SoundManagerInstance { get; private set; }

	private void Awake()
	{
		if (s_SoundManagerInstance != null && s_SoundManagerInstance != this)
		{
			DestroyImmediate(this.gameObject);
		}
		else
		{
			s_SoundManagerInstance = this;
		}
	}

	private void OnDestroy()
	{
		if (s_SoundManagerInstance == this)
		{
			s_SoundManagerInstance = null;
		}
	}
	#endregion
}

