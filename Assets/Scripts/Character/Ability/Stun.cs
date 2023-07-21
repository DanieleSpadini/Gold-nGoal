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
    /// This represents the stun as side effect of the arrow or performed by Human.
    /// </summary>
    [CreateAssetMenu(fileName = "Stun", menuName = "AbilitySystem/Stun", order = 0)]
    public class Stun : ModifierDefinition
    {
        [Header("Refs")]
        // AudioClip reference of the skill.
        [SerializeField, Tooltip("AudioClip reference of the skill.")]
        private AudioClip _sfxStunClip;

        // Stop the character where he is for the duration of the ability...
        // He can't even catch the ball...
        public override void ExecuteModifier(Player player, float timeElapsed)
        {
            if (IsFirstExec)
            {
                Core.SoundManager.Instance.PlayEffect(_sfxStunClip);
            }

            base.ExecuteModifier(player, timeElapsed);

            player.CurrentSpeed *= 0;

            player.CanGetBall = false;
        }

        // Nothing fancy. Trigger the animator transition and 
        // restores the possibility of catching the ball...
        public override void Dispose(Player player)
        {
            base.Dispose(player);
            
            player.CanGetBall = true;
            player.Animator.SetBool("WasHit", false);
        }
    }
}