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
using Ability;

namespace Character
{
    /// <summary>
    /// Goblin character.
    /// </summary>
    public class Goblin : Player
    {
        [Header("Configs")]
        // Speed of character (m/s).
        [SerializeField, Tooltip("Speed of character (m/s).")]
        private SpeedDefinition _speedDefinition;

        // Ball kick power.
        [SerializeField, Tooltip("Ball kick power.")]
        private KickPowerDefinition _kickPowerDefinition;

        // Action range (in meters).
        [SerializeField, Tooltip("Action range (in meters).")]
        private ActionRadiusDefinition _actionRadiusDefinition;

        // Skill definition.
        [SerializeField, Tooltip("Skill definition")]
        private ModifierDefinition _activeAbilityDefinition;

        // Cooldown time ability (seconds).
        [SerializeField, Tooltip("Cooldown time ability (seconds).")]
        private float _cooldownTimeActiveAbility;

        // Speed of arrow (m/s).
        [SerializeField, Tooltip("Speed of arrow (m/s).")]
        private float _arrowSpeed;

        // Direction in which the arrow is fired.
        [SerializeField, Tooltip("Direction in which the arrow is fired.")]
        private Vector3 _arrowDirection;

        [Header("Refs")]
        // Arrow ref.
        [SerializeField, Tooltip("Arrow ref.")]
        private GameObject _arrow;

        // AudioClip reference of the skill.
        [SerializeField, Tooltip("AudioClip reference of the skill.")]
        private AudioClip _sfxAbilityClip;

        #region custom editor
        public SpeedDefinition SpeedDefinition { get { return _speedDefinition; } set { _speedDefinition = value; } }
        public KickPowerDefinition KickPowerDefinition { get { return _kickPowerDefinition; } set { _kickPowerDefinition = value; } }
        public ActionRadiusDefinition ActionRadiusDefinition { get { return _actionRadiusDefinition; } set { _actionRadiusDefinition = value; } }
        public ModifierDefinition ActiveAbilityDefinition { get { return _activeAbilityDefinition; } set { _activeAbilityDefinition = value; } }
        public float CooldownTimeActiveAbility { get { return _cooldownTimeActiveAbility; } set { _cooldownTimeActiveAbility = value; } }
        #endregion

        // Line up character...
        public override void Start()
        {
            InitCharacter(_speedDefinition.Speed,
                          _kickPowerDefinition.Power,
                          _actionRadiusDefinition.Radius,
                          _cooldownTimeActiveAbility,
                          DeployActiveAbility,
                          _team._teamCircle);

            // MIND OUT THERE: call the overridden base method AFTER the 'InitCharacter' method
            // otherwise some properties will not be set.
            base.Start();
        }

        /// <summary>
        /// Deploy the character's active ability...
        /// </summary>
        public void DeployActiveAbility()
        {
            //Debug.Log($"{this}: Deploy active ability!!");
            if (_interactAbility != null)
            {
                Core.SoundManager.Instance.PlayEffect(_sfxAbilityClip);

                ShotArrowToNearestOpposingPlayer();
            }
        }

        /// <summary>
        /// Shoot the arrow at the closest opponent.
        /// </summary>
        private void ShotArrowToNearestOpposingPlayer()
        {
            Player targetPlayer = MatchManager.Instance.GetNearestPlayerInOpposingTeam(this);

            _arrowDirection = targetPlayer.transform.position - this.transform.position;

            Vector3 spawnPoint = new Vector3(this.transform.position.x, 1, this.transform.position.z);

            GameObject _bullet = Instantiate(_arrow, spawnPoint, Quaternion.LookRotation(_arrowDirection));
            _bullet.GetComponent<Arrow>().Init(_arrowSpeed, _activeAbilityDefinition, this);
        }
    }
}
