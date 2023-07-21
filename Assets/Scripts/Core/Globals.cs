using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Core
{
    /// <summary>
    /// Globals vars/enums...
    /// </summary>
    public enum GamePhase
    {
	    Stall, Attack, Defense, Scramble
    }

    public enum TeamSideField
    {
	    Top, Bottom
    }

    public enum MatchPhase
    {
        WaitingToStartNewSet, WaitingToStartNewMatch, InGame, Paused, Ended,Goal
    }

    public class Globals
    {
	    public const string LEFT_BARRIER = "Left Barrier";
        public const string RIGHT_BARRIER = "Right Barrier";

        public const string TOP_GOAL_AREA = "Top Goal Area";
        public const string BOTTOM_GOAL_AREA = "Bottom Goal Area";

        public const string TOP_DRAG_AREA = "Top Drag Area";
        public const string BOTTOM_DRAG_AREA = "Bottom Drag Area";
    }
}
