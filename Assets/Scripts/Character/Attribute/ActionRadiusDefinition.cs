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
    [CreateAssetMenu(fileName = "ActionRadius", menuName = "AttributeSystem/Action Radius", order = 0)]
    public class ActionRadiusDefinition : ScriptableObject
    {
        [SerializeField]
        public float Radius;
    }
}

