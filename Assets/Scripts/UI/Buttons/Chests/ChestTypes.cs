using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "ChestTypes", menuName = "ChestTypes", order = 0)]
public class ChestTypes : ScriptableObject
{
	public Sprite Art;

	public string ChestName;

	public int UnlockTimeInSeconds;
	public int GoldEarned;
	public int GemsEarned;
}
