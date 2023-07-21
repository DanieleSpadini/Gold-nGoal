using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "HeroAttributes", menuName = "HeroAttributes", order = 0)]
public class HeroPanelAttributes : ScriptableObject
{
	public Sprite Art;

	public string HeroName;
	public string AbilityName;
	public string AbilityEffect;
	public string Speed;
	public string Rarity;
	
	public int Level;
	public int GoldForNextLevel;
	public int CardsForNextLevel;
	public int CardsAvailable;
}

