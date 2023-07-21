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
    /// This represents the teleport performed by Dwarf.
    /// </summary>
    [CreateAssetMenu(fileName = "Teleport", menuName = "AbilitySystem/Teleport", order = 0)]
    public class Teleport : ModifierDefinition
    {
        // Simply reset his position to the spawn point...
        public override void ExecuteModifier(Player player, float timeElapsed)
        {
            base.ExecuteModifier(player, timeElapsed);

            player.transform.position = player.SpawnSpot.position;
            player.IsMoving = true;
        }
    }
}