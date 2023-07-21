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
    /// This represents the knockback as a Debuff by Human.
    /// </summary>
	[CreateAssetMenu(fileName = "Knockback", menuName = "AbilitySystem/Knockback", order = 0)]
	public class Knockback : ModifierDefinition
    {
        // Knockback power. Represents the player's backward speed.
        [SerializeField, Tooltip("Represents the player's backward speed.")]
        protected int _power;

        [Header("Refs")]
        // AudioClip reference of the skill.
        [SerializeField, Tooltip("AudioClip reference of the skill.")]
        private AudioClip _sfxStunClip;

        // Multiply the character's speed by the power factor and apply the
        // new opposite speed for the defined time.
        public override void ExecuteModifier(Player player, float timeElapsed)
        {            
            if (IsFirstExec)
            {
                Core.SoundManager.Instance.PlayEffect(_sfxStunClip);
            }

            base.ExecuteModifier(player, timeElapsed);

            player.CanUpdateDirection = false;

            player.CurrentSpeed *= -_power;
        }

        // Nothing fancy. Character can resume movement and stop particle...
        public override void Dispose(Player player)
        {
            base.Dispose(player);

            player.CanUpdateDirection = true;
            StopShockwave(player);
        }
    }
}
