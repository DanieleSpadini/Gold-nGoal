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
using TMPro;
using UnityEngine.SceneManagement;

namespace Core
{
	[DisallowMultipleComponent]
	public class UIManager : MonoBehaviour
	{

		[SerializeField]
		private TextMeshProUGUI _teamAScoreTxt = null;

		[SerializeField]
		private TextMeshProUGUI _teamBScoreTxt = null;

		[SerializeField]
		private TextMeshProUGUI _matchTimeTxt = null;

		[SerializeField]
		private Button sliderInteractTeamA = null;

		[SerializeField]
		private Button sliderInteractTeamB = null;

		[SerializeField]
		private Button[] _teamAButtons = null;

		[SerializeField]
		private Button[] _teamBButtons = null;

		[SerializeField]
		private TextMeshProUGUI _playerName = null;

		[SerializeField]
		private TextMeshProUGUI _playerNameEndGame;

        [SerializeField]
        private TextMeshProUGUI _countdownStart;

        [SerializeField]
        private TextMeshProUGUI _30secLeft;


        public GameObject _endGamePanel;

		public TextMeshProUGUI _finalScoreOpponent;

		public TextMeshProUGUI _finalScorePlayer;

		public TextMeshProUGUI _winLoseText;

		public TextMeshProUGUI _endGameText;



		void Start()
		{
			_playerName.text = MenuUIManager.s_UIManagerInstance?.GetName();
			_playerNameEndGame.text = MenuUIManager.s_UIManagerInstance?.GetName();

		}

		void Update()
		{

			float normalizedSliderInteractValue;

			normalizedSliderInteractValue = MatchManager.Instance.TeamA.CountdownToDeployOnInteractPress.TimeRemaining / MatchManager.Instance.TeamA.CountdownToDeployOnInteractPress.Duration;
			sliderInteractTeamA.image.material.SetFloat("_SliderAmount", 1 - normalizedSliderInteractValue);

			normalizedSliderInteractValue = MatchManager.Instance.TeamB.CountdownToDeployOnInteractPress.TimeRemaining / MatchManager.Instance.TeamB.CountdownToDeployOnInteractPress.Duration;
			sliderInteractTeamB.image.material.SetFloat("_SliderAmount", 1 - normalizedSliderInteractValue);

			for (int i = 0; i < MatchManager.Instance.TeamA.PlayerCount; i++)
			{

				Player player = MatchManager.Instance.TeamA.Players[i];

				if (player != null && player.CountdownToDeployActiveAbility != null)
				{
					float normalizedSliderValue = player.CountdownToDeployActiveAbility.TimeRemaining / player.CountdownToDeployActiveAbility.Duration;

					_teamAButtons[i].image.material.SetFloat("_SliderAmount", 1 - normalizedSliderValue);
					_teamAButtons[i].image.material.SetTexture("_Image", player._heroPanelAttributes.Art.texture);
				}
			}

			for (int i = 0; i < MatchManager.Instance.TeamB.PlayerCount; i++)
			{
				Player player = MatchManager.Instance.TeamB.Players[i];

				if (player != null && player.CountdownToDeployActiveAbility != null)
				{
					float normalizedSliderValue = player.CountdownToDeployActiveAbility.TimeRemaining / player.CountdownToDeployActiveAbility.Duration;

					_teamBButtons[i].image.material.SetFloat("_SliderAmount", 1 - normalizedSliderValue);
					_teamBButtons[i].image.material.SetTexture("_Image", player._heroPanelAttributes.Art.texture);
				}
			}
		}

		public void ScoreGoalTeamA(int score)
		{
			_teamAScoreTxt.text = score.ToString();

		}

		public void ScoreGoalTeamB(int score)
		{
			_teamBScoreTxt.text = score.ToString();

		}

		public void ResetScore()
		{
			_teamAScoreTxt.text = "0";
			_teamBScoreTxt.text = "0";
		}

		public void UpdateTimer(float time)
		{
			S.TimeSpan totalTime = S.TimeSpan.FromSeconds(time);
			_matchTimeTxt.text = totalTime.ToString("mm':'ss");

			if (time <= 30f && time >= 28f)
			{
				_30secLeft.gameObject.SetActive(true);
			}
			else
			{
				if (_30secLeft.gameObject.activeSelf)
                    _30secLeft.gameObject.SetActive(false);
            }
		}

		public void UpdateCountdown(int time)
		{
			if (int.TryParse(_countdownStart.text, out int resTime))
			{
                if (time != resTime)
				{
					if (!_countdownStart.gameObject.activeSelf)
						_countdownStart.gameObject.SetActive(true);
                    
					_countdownStart.text = time.ToString();
                }
            }
			else
			{
                _countdownStart.gameObject.SetActive(true);
                _countdownStart.text = time.ToString();
            }
        }

		public void HideCountdown()
		{
            _countdownStart.gameObject.SetActive(false);
        }

		public void ClaimChest()
		{
			MatchResult.s_MatchResult.SetGamesCounter();

			if (MatchManager.Instance.TeamBScore < MatchManager.Instance.TeamAScore)
			{
				MatchResult.s_MatchResult.result = MatchResults.WIN;
				SceneManager.LoadScene(0, LoadSceneMode.Single);
			}
			else if (MatchManager.Instance.TeamAScore < MatchManager.Instance.TeamBScore)
			{
				MatchResult.s_MatchResult.result = MatchResults.LOSE;
				SceneManager.LoadScene(0, LoadSceneMode.Single);
			}
			else if (MatchManager.Instance.TeamBScore == MatchManager.Instance.TeamAScore)
			{
				MatchResult.s_MatchResult.result = MatchResults.DRAW;
				SceneManager.LoadScene(0, LoadSceneMode.Single);
			}
			

			
		}

		#region Singleton
		public static UIManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				DestroyImmediate(this);
			}
			else
			{
				Instance = this;
				//DontDestroyOnLoad(gameObject);
			}
		}

		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}
		#endregion

	}

}
