using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class ChestsManager : MonoBehaviour
{
	public GameObject ChestPanel;
	public GameObject[] ChestSlots;
	public GameObject AnimationPanel;

	public MenuSwipe MenuSwipe;

	public ChestButton[] Chests;

	public VideoClip[] OpeningAnimations;

	public VideoPlayer Player;

	#region Chest Panel Refs
	[Space(20), Header("Chest Panel Refs")]

	public TextMeshProUGUI TimerTextInSeconds;
	public TextMeshProUGUI ChestName;
	public Image ChestImage;
	public Button OpenChestButton;
	public GameObject UnlockingPanel;
	public Transform ContentParent;

	#endregion 
	public static ChestsManager s_ChestsManagerInstance { get; private set; }


	void Awake()
	{
		if (s_ChestsManagerInstance != null && s_ChestsManagerInstance != this)
		{
			DestroyImmediate(this.gameObject);
		}
		else
		{
			s_ChestsManagerInstance = this;
		}
	}

	void Start()
	{
		// After a won match there is a percentage to get common, rare and epic chest (80%/15%/5%)

		if (MatchResult.s_MatchResult.result == MatchResults.WIN)
		{
			int rnd = Random.Range(0, 100);

			if (rnd >= 0 && rnd <= 79)
			{
				InstanceChest(0);
			}
			else if (rnd >= 80 && rnd <= 94)
			{
				InstanceChest(1);
			}
			else if (rnd >= 95 && rnd < 100)
			{
				InstanceChest(2);
			}
		}
	}

	private void InstanceChest(int rarity)
	{
		for (int i = 0; i < ChestSlots.Length; i++)
		{
			if (ChestSlots[i].transform.childCount == 0)
			{
				ChestButton chest = Instantiate<ChestButton>(Chests[rarity], ChestSlots[i].transform);
				chest.ChestPanel = ChestPanel;
				chest.Swipe = MenuSwipe;
				chest.ChestName = ChestName;
				chest.ChestImage = ChestImage;
				chest.TimerTextInSeconds = TimerTextInSeconds;
				chest.OpenChestButton = OpenChestButton;
				chest.ContentParent = ContentParent;
				chest.UnlockingPanel = UnlockingPanel;
				break;
			}
		}
	}
}