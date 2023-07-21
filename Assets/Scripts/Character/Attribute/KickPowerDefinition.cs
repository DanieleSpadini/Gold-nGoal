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

namespace Core
{
    [CreateAssetMenu(fileName = "KickPower", menuName = "AttributeSystem/Kick Power", order = 0)]
    public class KickPowerDefinition : ScriptableObject
    {
        [SerializeField]
        public float Power;
    }
}

