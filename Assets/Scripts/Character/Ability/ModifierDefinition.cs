using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using S = System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


using Core;

namespace Ability
{
    /// <summary>
    /// This scriptable object represents the form that a skill must have.
    /// The minimum requirements are a name, a duration, and a modifier routine to apply.
    /// </summary>
    public abstract class ModifierDefinition : ScriptableObject
    {
        // Ability name
        [SerializeField, Tooltip("Ability name")]
        protected string _name;

        // Ability is over ?
        protected bool _isOver;
        public bool IsOver { get => _isOver; }

        [SerializeField]
        protected float _timeElapsed;

        // Skill duration time
        [SerializeField, Tooltip("Skill duration time")]
        protected float _duration;

        // Skill VFx ref
        [SerializeField, Tooltip("Skill VFx ref")]
        private ParticleSystem _vfx;

        // Is the first frame of skill execution ?
        private bool _isFirstExec = true;
        public bool IsFirstExec { get => _isFirstExec; }

        /// <summary>
        /// This method will be called every single frame for the defined duration of the ability.
        /// </summary>
        /// <param name="player">Affected character</param>
        /// <param name="timeElapsed">Time passed from last call</param>
        public virtual void ExecuteModifier(Player player, float timeElapsed)
        {
            // Play VFx...
            PlayVFX(player);

            // Keep track of time...
            _timeElapsed += timeElapsed;

            // If the time for the skill is over...
            if (_timeElapsed >= _duration)
            {
                _isOver = true;
            }

            // Was it the first frame of skill ?
            if (_isFirstExec)
                _isFirstExec = false;
        }

        /// <summary>
        /// This method is called when time for the ability is over. 
        /// This is the ideal place to clean up and restore certain values ​​that have been changed.
        /// </summary>
        /// <param name="player"></param>
        public virtual void Dispose(Player player)
        {
            StopVFX(player);
        }

        #region VFX
        public void PlayTrail(Player player)
        {
            if (player.Trail != null)
                player.Trail.SetActive(true);
        }
        public void StopTrail(Player player)
        {
            if (player.Trail != null)
                player.Trail.SetActive(false);
        }
        public void PlayShockwave(Player player)
        {
            
            if (player.ShockWave != null && !player.ShockWave.activeSelf)
            {
                player.ShockWave.SetActive(true);
            }
            
        }
        public void StopShockwave(Player player)
        {
            if (player.ShockWave != null && player.ShockWave.activeSelf)
                player.ShockWave.SetActive(false);
        }
        public void PlayVFX(Player player)
        {
            if (_vfx != null && !_vfx.isPlaying)
            {
                _vfx = Instantiate(_vfx,player.transform.position,Quaternion.identity);
                
                
                _vfx.Play();
                Debug.Log(_vfx.isPlaying);
            }

        }
        public void StopVFX(Player player)
        {
            if (_vfx != null && !_vfx.isPlaying)
            {
                _vfx.Stop();
             
            }

        }
        #endregion
    }

}
