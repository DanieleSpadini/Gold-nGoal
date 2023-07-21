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
    /// This represents the ability performed by Human.
    /// </summary>
    [CreateAssetMenu(fileName = "DashThenStun", menuName = "AbilitySystem/DashThenStun", order = 0)]
    public class DashThenStun : ModifierDefinition
    {
        // Dash power. Represents the character's speed increase.
        [SerializeField, Tooltip("Represents the character's speed increase.")]
        protected int _power;
        public int Power { get => _power; }

        // Area range (meters).
        [SerializeField, Tooltip("Area range (meters).")]
        protected float _stunAreaRadius;

        // Stun skill ref.
        [SerializeField, Tooltip("Stun skill ref.")]
        protected ModifierDefinition _stunDefinition;

        // VFx ref..
        [SerializeField, Tooltip("VFx ref.")]
        private ParticleSystem _stunVFX;

        // Multiply the character's speed by the power factor and apply the
        // new speed for the defined time.
        public override void ExecuteModifier(Player player, float timeElapsed)
        {
            base.ExecuteModifier(player, timeElapsed);

            player.CurrentSpeed *= _power;

            PlayTrail(player);
            PlayShockwave(player);
        }

        // At the end of the dash ability set the stun debuff to the opponent within the defined distance.
        public override void Dispose(Player player)
        {
            StopTrail(player);
            StopShockwave(player);
            base.Dispose(player);
           
            List<Player> targets = MatchManager.Instance.GetPlayersWithinRadiusInOpposingTeam(player, _stunAreaRadius);
            if (targets != null)
            {
                foreach(Player target in targets)
                {
                    ParticleSystem stunvfx = Instantiate(_stunVFX, target.transform);
                    stunvfx.Emit(2);
                    stunvfx.Play();
                    target.AddModifier(Instantiate<ModifierDefinition>(_stunDefinition));
                }
            }
        }
    }
}