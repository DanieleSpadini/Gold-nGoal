using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Refs")]
        // Audio Source ref for OST
        [SerializeField, Tooltip("AudioSource ref for OST")] 
        private AudioSource _musicAudioSource;

        // Audio Source ref for SFX
        [SerializeField, Tooltip("AudioSource ref for SFX")]
        private AudioSource _sfxAudioSource;

        // Audio Mixer ref used in match
        [SerializeField, Tooltip("Audio Mixer ref used in match")]
        private AudioMixer _matchAudioMixer;

        private void Start()
        {
            // Load volume setting from saved preferences...
            // This will set the same volume for all the scenes.
            float volume = PlayerPrefs.GetFloat("Volume", 1f);
            _matchAudioMixer.SetFloat("MatchVolume", Mathf.Log10(volume) * 20);
        }

        /// <summary>
        /// Play audioclip.
        /// </summary>
        /// <param name="clip">sfx to play</param>
        public void PlayEffect(AudioClip clip)
        {
            _sfxAudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Play OST.
        /// </summary>
        /// <param name="clip">Background music to play</param>
        public void PlayMusic(AudioClip clip)
        {
            if (_musicAudioSource == null) 
                return;
            
            // Play the clip only if nothing is playing...
            if (!_musicAudioSource.isPlaying)
            {
                _musicAudioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Suspend OST.
        /// </summary>
        public void StopMusic()
        {
            if (_musicAudioSource == null)
                return;

            if (_musicAudioSource.isPlaying)
            {
                _musicAudioSource.Stop();
            }
        }

        #region Singleton
        // Boring singleton pattern stuff...
        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(this.gameObject);
            }
            else
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        #endregion
    }
}
