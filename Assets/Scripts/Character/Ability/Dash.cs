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
    /// This represents the dash performed by Boar.
    /// </summary>
    [CreateAssetMenu(fileName = "Dash", menuName = "AbilitySystem/Dash", order = 0)]
    public class Dash : ModifierDefinition
    {
        // Dash power. Represents the character's speed increase.
        [SerializeField, Tooltip("Represents the character's speed increase.")]
        protected int _power;
        public int Power { get => _power; }

        // Multiply the character's speed by the power factor and apply the
        // new speed for the defined time.
        public override void ExecuteModifier(Player player, float timeElapsed)
        {
            base.ExecuteModifier(player, timeElapsed);

            player.CurrentSpeed *= _power;

            PlayTrail(player);
        }

        // Nothing fancy. Just stop particle...
        public override void Dispose(Player player)
        {
            base.Dispose(player);
            StopTrail(player);
        }
    }
}