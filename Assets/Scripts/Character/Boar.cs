using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Core;
using Ability;

namespace Character
{
    /// <summary>
    /// Boar character.
    /// </summary>
    public class Boar : Player
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
        public float CooldownTimeActiveAbility 
        { 
            get => _cooldownTimeActiveAbility; 
            set => _cooldownTimeActiveAbility = value; 
        }

        [Header("Refs")]
        // AudioClip reference of the skill.
        [SerializeField, Tooltip("AudioClip reference of the skill.")]
        private AudioClip _sfxAbilityClip;

        #region custom editor
        public SpeedDefinition SpeedDefinition { get { return _speedDefinition; } set { _speedDefinition = value; } }
        public KickPowerDefinition KickPowerDefinition { get { return _kickPowerDefinition; } set { _kickPowerDefinition = value; } }
        public ActionRadiusDefinition ActionRadiusDefinition { get { return _actionRadiusDefinition; } set { _actionRadiusDefinition = value; } }
        public ModifierDefinition ActiveAbilityDefinition { get { return _activeAbilityDefinition; } set { _activeAbilityDefinition = value; } }
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

                this.AddModifier(Instantiate<ModifierDefinition>(_activeAbilityDefinition));
            }
        }
    }
}
