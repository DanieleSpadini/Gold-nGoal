using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchResults
{
	WIN,
	LOSE,
	DRAW,
	NOMATCH
}

[DisallowMultipleComponent]
public class MatchResult : MonoBehaviour
{
	public MatchResults result = MatchResults.NOMATCH;
	private int _gamesCounter;
	public int GamesCounter
	{
		get { return _gamesCounter; }
	}

	#region Singleton
	public static MatchResult s_MatchResult{ get; private set; }

	private void Awake()
	{
		if (s_MatchResult != null && s_MatchResult != this)
		{
			DestroyImmediate(this.gameObject);
		}
		else
		{
			s_MatchResult = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	#endregion

	/// <summary>
	/// This is to make sure that the button in the shop "Play one game" is interactable only after a game is played
	/// </summary>
	public void SetGamesCounter()
	{
		_gamesCounter++;
	}
}
