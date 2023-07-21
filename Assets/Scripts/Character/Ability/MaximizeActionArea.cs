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
    /// This represents the increased action range area performed by Elf.
    /// </summary>
    [CreateAssetMenu(fileName = "MaximizeActionArea", menuName = "AbilitySystem/MaximizeActionArea", order = 0)]
    public class MaximizeActionArea : ModifierDefinition
    {
        // New area range (meters).
        [SerializeField, Tooltip("New area range (meters).")]
        protected float _radiusChange;

        // Stop the character and increase the action range area as defined...
        public override void ExecuteModifier(Player player, float timeElapsed)
        {
            base.ExecuteModifier(player, timeElapsed);
            
            player.CurrentSpeed *= 0;

            player.CurrentActionRadius = _radiusChange;
        }

        // Nothing fancy. Restore previous area range...
        public override void Dispose(Player player)
        {
            player.CurrentActionRadius = player.ActionRadius;

            base.Dispose(player);
        }
    }
}