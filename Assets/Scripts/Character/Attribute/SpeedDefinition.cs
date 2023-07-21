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
    [CreateAssetMenu(fileName = "Speed", menuName = "AttributeSystem/Speed", order = 0)]
    public class SpeedDefinition : ScriptableObject
    {
        [SerializeField]
        public float Speed;
    }
}

