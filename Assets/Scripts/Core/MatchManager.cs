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
using Cinemachine;

namespace Core
{

    /// <summary>
    /// This class manages the match flow.
    /// It set-ups teams and players, keeps the adjacency matrix of players/balls distances updated and applies the rules of the game.
    /// 
    /// </summary>
    [DisallowMultipleComponent]
	public class MatchManager : MonoBehaviour
	{
		[Header("Configs")]
		// How many players per team. We can line-up an arbitrary number of players for maximum expandibility.
		// System is actually configured to work with 3 players per team.
		[SerializeField, Tooltip("How many players per team.")]
		private int _playersPerTeam;

		// How long the match last (in seconds).
		[SerializeField, Tooltip("How long the match last (in seconds).")]
		private float _matchDuration;

        // How long the teamA (player) has to wait before he can click the interaction button (in seconds).
        [SerializeField, Tooltip("How long the teamA (player) has to wait before he can click the interaction button (in seconds).")]
		private float _cooldownPeriodForPassingTeamA;

        // How long the teamB (cpu) has to wait before he can click the interaction button (in seconds).
        [SerializeField, Tooltip("How long the teamB (cpu) has to wait before he can click the interaction button (in seconds).")]
		private float _cooldownPeriodForPassingTeamB;

        // The spot to which the ball is kicked on interaction button press. It's an offset relative to the Playable Characters.
        //Positive offset means ball is targeted ahead player, negative behind.
        [SerializeField, Tooltip("The offset spot to which the ball is kicked on interaction button press.\nPositive offset means ball is targeted ahead player, negative behind.")]
		private float _passOffset;
		public float PassOffset { get => _passOffset; }

		// How much the Playable Characters will be moved away after intercept event occours (in meters).
		[SerializeField, Tooltip("How much the Playable Characters will be moved away after intercept event occours (in meters).")]
		private float _repositionDistance;
		public float RepositionDistance { get => _repositionDistance; }

		// How long is the area that affects the reposition routine (in meters).
		[SerializeField, Tooltip("How long is the area that affects the reposition routine (in meters).")]
		private float _areaRepositioning;
		public float AreaRepositioning { get => _areaRepositioning; }

        // How many raycasts every single PCs project for avoidance algorithm.
        // Please edit this value carefully. If you don't know how to set it, leave it as default (17).
        [SerializeField, Tooltip("How many raycasts every single PCs project for avoidance algorithm.\nPlease edit this value carefully. If you don't know how to set it, leave it as default (17).")]
		private int _rays;
		public int Rays { get => _rays; }

        // The angle that rays should cover.
        // Please edit this value carefully. If you don't know how to set it, leave it as default (75).
        [SerializeField, Tooltip("The angle that rays should cover.\nPlease edit this value carefully. If you don't know how to set it, leave it as default (75).")]
		private float _rayAngle;
		public float RayAngle { get =>  _rayAngle; }

        // How long raycast should be projected.
        // This may affects the character chance to steal the ball. Consider to change it in accordance with playable character 
        // 'Action Radius' property to avoid unexpected behavior.
        [SerializeField, Tooltip("How long raycast should be projected. This may affects the character chance to steal the ball.\nConsider to change it in accordance with playable character 'Action Radius' property to avoid unexpected behavior.")]
		private float _rayRange;
		public float RayRange { get => _rayRange; }

		// How long is the countdown at the start of the match.
		[SerializeField, Tooltip("How long is the countdown at the start of the match.")]
		private int _countdownToStartMatch;

        [Header("Refs")]
        // Team prefab reference (typically 'Team' file under prefabs folder)
        [SerializeField, Tooltip("Team prefab reference.")]
		private Team _teamPrefab;

        // Characters prefabs reference (typically under prefabs\characters folder).
		// If you don't want the chance to line-up a specific character, don't add it to this array.
		// In a standard configuration you need at least 3 different characters to successfully start the match.
        [SerializeField, Tooltip("Characters prefabs reference.")]
		private Player[] _playersPrefabs;

        // Ball prefab reference (typically 'Ball' file under prefabs folder).
        [SerializeField, Tooltip("Ball prefab reference.")]
		private Ball _ballPrefab;

		// Spawn spots transform for all lined-up characters.
		// This array length must be (_playersPerTeam * 2), so every character must have his spawn spot defined.
		[SerializeField, Tooltip("Spawn spots transform for all lined-up characters.")]
		private Transform[] _spawnTransforms;

        // Every character can has a name related to his line-up position.
        // This array length must be (_playersPerTeam * 2).
        // This name isn't shown to human player, so be creative... Could be also useful for debug purpose...
        [SerializeField, Tooltip("Names for all lined-up characters.")]
		private string[] _playerNames;

        // TeamA interaction button reference.
        [SerializeField, Tooltip("TeamA interaction button reference.")]
		private Button _interactTeamA = null;

        // TeamB interaction button reference.
        [SerializeField, Tooltip("TeamB interaction button reference.")]
		private Button _interactTeamB = null;

        // Ability buttons reference.
        // This array length must be (_playersPerTeam * 2).
        // TeamA reference buttons first then TeamB.
        [SerializeField, Tooltip("Ability buttons reference.")]
		private Button[] _interactActiveAbility = null;

		// Circle vfx reference.
		// First ref is for TeamA, the second for TeamB.
        [SerializeField, Tooltip("Circle vfx reference.")]
        private GameObject[] _teamCircle;

        // Audio references...
        // OST audioclip (in looping for the entire match).
        [SerializeField, Tooltip("OST looped for the entire match.")]
        private AudioClip _musicClip;

        // Match end effect audioclip reference.
        [SerializeField, Tooltip("Match end sound effect reference.")]
        private AudioClip _sfxClipMatchEnd;

        // Goal score effect audioclip reference.
        [SerializeField, Tooltip("Goal score effect reference.")]
        private AudioClip _sfxClipGoalScore;

        // New set effect audioclip reference.
		// Played after a goal score to notify the match will resume.
        [SerializeField, Tooltip("New set effect reference.")]
        private AudioClip _sfxClipStartNewSet;

		// Player loses effect audioclip reference.
		// Played at the end of the match.
        [SerializeField, Tooltip("Player loses effect reference.")]
        private AudioClip _sfxClipPlayerLoses;

        // Player wins effect audioclip reference.
        // Played at the end of the match.
        [SerializeField, Tooltip("Player wins effect reference.")]
        private AudioClip _sfxClipPlayerWins;

        // Match draw effect audioclip reference.
        // Played at the end of the match.
        [SerializeField, Tooltip("Match draw effect reference.")]
        private AudioClip _sfxClipMatchDraw;

		// Transform of the goal for TeamA.
		// This is the target TeamA characters look at to throw the ball and score.
        [SerializeField]
        private Transform _topGoal;

        // Transform of the goal for TeamB.
        // This is the target TeamB characters look at to throw the ball and score.
        [SerializeField]
        private Transform _bottomGoal;

		// Cinemachine references...
		// Two camera located near the goals area. Main camera blends to one of that during the goal.
        [SerializeField]
        private CinemachineVirtualCamera[] _vGoalCam;

		// Main camera.
        [SerializeField]
        private CinemachineVirtualCamera _mainCamera;
        public CinemachineVirtualCamera MainCamera { get => _mainCamera; }

		// Coutdown camera.
		// Game starts using this camera and blends to the main camera during the countdown.
        [SerializeField]
        private CinemachineVirtualCamera _CountdownCamera;
        public CinemachineVirtualCamera CountdownCamera { get => _CountdownCamera; }


        [Space(20)]
		[Header("Game data"), Tooltip("Changes to this section don't affect the game run.")]
		// TeamA ref.
		// This will cointains all TeamA (player) data.
		public Team TeamA = null;

        // TeamB ref.
        // This will cointains all TeamB (cpu) data.
        public Team TeamB = null;

        // Ball ref.
        // This will cointains all ball data.
        public Ball SoccerBall = null;

		// TeamA score.
		private int _teamAScore = 0;
		public int TeamAScore { get => _teamAScore; }

		// TeamB score.
		private int _teamBScore = 0;
		public int TeamBScore { get => _teamBScore; }

		// Timer of the match.
		private Timer _matchTimer = null;

        // Phase of the match.
		// This var can change the entire flow of the game. Please trigger a new status carefully.
        // See MatchPhase enum for all available match status.
        private MatchPhase _matchPhase = MatchPhase.WaitingToStartNewMatch;
		public MatchPhase Phase
		{
			get => _matchPhase;
			set
			{
				if (value != _matchPhase)
				{
					_matchPhase = value;
				}
			}
		}

		// Adjacency matrix of all lined-up characters.	
		private float[,] _playersDistances = null;

		// Distances between characters and ball.
		private float[] _ballDistances = null;

		// How many lined-up players for match.
		private int _playersNumber;

        private bool _isFinishedAnimGoal;
		private bool _isCountdownCorutineRunning = false;

		// Keeps track of the ability and interaction buttons status.
		private bool[] _buttonStatus;

        #region lifecycle method
        private void Start()
		{
			// We have 8 buttons state to save...
			_buttonStatus = new bool[8];

            // Scene starts in 'WaitingToStartNewMatch' status, so we can reposition characters
			// blend the camera, start the countdown, etc...
            Phase = MatchPhase.WaitingToStartNewMatch;

			// Blend camera to main camera (cinematic slow zoom-in effect).
			StartCoroutine(WaitForCameraBlend());

			// Starts looping OST.
            SoundManager.Instance.PlayMusic(_musicClip);

			// Setup the game.
            Init();

			// Start the game.
			KickOff();
		}

        private void Update()
        {
			// Make sure music is playing...
            SoundManager.Instance.PlayMusic(_musicClip);

			// If match is running...
            if (Phase == MatchPhase.InGame)
            {
				// Update characters adjacency matrix...
                CalculatePlayersDistances();

				// Update distances between characters and ball...
                CalculateBallDistances();

				// Map some keyboard keys to allow playing game from pc (easier to debug).
#if (UNITY_EDITOR)
                ProcessKeyDownForDebug();
#endif
            }
			// If game hasn't started yet (scene has just been loaded)...
            else if (Phase == MatchPhase.WaitingToStartNewMatch)
            {
				// Reset score...
                UIManager.Instance.ResetScore();

				// Start the countdown...
                if (!_isCountdownCorutineRunning)
                    StartCoroutine(CountdownToStartMatch(_countdownToStartMatch));
            }
			// If some player has just scored...
            else if (Phase == MatchPhase.WaitingToStartNewSet)
            {
				// Pause the stopwatch...
                PauseMatchTimer();

				// Stop processing inputs...
                SuspendAllInteractionButtonsLifecycle();

				// If characters have been relocated to starting positions...
                if (TeamA.Players.Find(x => x.IsRepositioning) == null &&
                     TeamB.Players.Find(x => x.IsRepositioning) == null)
                {
					// Play the new set effect sound...
                    SoundManager.Instance.PlayEffect(_sfxClipStartNewSet);

					// Give ball to the player who conceded goal...
                    SoccerBall.gameObject.SetActive(true);

					// Resume the match...
                    Phase = MatchPhase.InGame;

					// Resume the stopwatch...
                    ResumeMatchTimer();

					// Resume processing inputs...
                    ResumeAllInteractionButtonsLifecycle();
                }
            }

            if (_matchTimer != null)
            {
				// Show the stopwatch time...
                UIManager.Instance.UpdateTimer(_matchTimer.TimeRemaining);
            }
        }
        #endregion


        #region private methods
        /// <summary>
        /// Countdown to the start of the match.
        /// </summary>
        /// <param name="time">Countdown starts from this number.</param>
        /// <returns></returns>
        private IEnumerator CountdownToStartMatch(int time)
		{
			_isCountdownCorutineRunning = true;

			PauseMatchTimer();
			SuspendAllInteractionButtonsLifecycle();
			
            do
			{
				UIManager.Instance.UpdateCountdown(time);
				yield return new WaitForSeconds(1);
				time--;

			} while (time > 0);

			UIManager.Instance.HideCountdown();

            _matchTimer = Timer.CreateTimer($"{this} - Match timer", _matchDuration, true, false, TimeIsOver);

            SoundManager.Instance.PlayEffect(_sfxClipStartNewSet);

            SoccerBall.gameObject.SetActive(true);

            Phase = MatchPhase.InGame;

            ResumeMatchTimer();
			ResumeAllInteractionButtonsLifecycle() ;
			
            _isCountdownCorutineRunning = false;
        }

		/// <summary>
		/// Camera blend from 'countdown virtual camera' to the 'main virtual camera'.
		/// </summary>
		/// <returns></returns>
		private IEnumerator WaitForCameraBlend()
		{
			// We need to wait for a frame otherwise cinemachine skips the blend...
			yield return new WaitForEndOfFrame();
			_CountdownCamera.Priority = 5;
		}

		/// <summary>
		/// Pause the stopwatch of the match.
		/// </summary>
		private void PauseMatchTimer()
		{
			if (_matchTimer != null)
				_matchTimer.Pause();
        }

		/// <summary>
		/// Resume stopwatch of the match.
		/// </summary>
        private void ResumeMatchTimer()
        {
            if (_matchTimer != null)
                _matchTimer.Resume();
        }

        /// <summary>
        /// Stop processing inputs. Buttons will no longer trigger events.
        /// </summary>
        private void SuspendAllInteractionButtonsLifecycle()
		{
			if (TeamA.CountdownToDeployOnInteractPress != null)
				TeamA.CountdownToDeployOnInteractPress.Pause();

			if (TeamA.Interact != null)
			{
				_buttonStatus[0] = TeamA.Interact.enabled;
				TeamA.Interact.enabled = false;
			}

			if (TeamA.Players != null)
			{
				for (int i = 0; i < TeamA.Players.Count; i++)
				{
					Player p = TeamA.Players[i];
                    if (p.CountdownToDeployActiveAbility != null)
                        p.CountdownToDeployActiveAbility.Pause();
				}
			}

			if (TeamB.CountdownToDeployOnInteractPress != null)
	           TeamB.CountdownToDeployOnInteractPress.Pause();

            if (TeamB.Interact != null)
			{
                _buttonStatus[4] = TeamB.Interact.enabled;
                TeamB.Interact.enabled = false;
			}

            if (TeamB.Players != null)
			{
                for (int i = 0; i < TeamB.Players.Count; i++)
                {
                    Player p = TeamB.Players[i];
                    if (p.CountdownToDeployActiveAbility != null)
                        p.CountdownToDeployActiveAbility.Pause();
				}
			}
        }

		/// <summary>
		/// Resume processing inputs. Buttons will trigger events.
		/// </summary>
        private void ResumeAllInteractionButtonsLifecycle()
		{
            if (TeamA.Interact != null)
                TeamA.Interact.enabled = _buttonStatus[0];

            if (TeamA.CountdownToDeployOnInteractPress != null)
                TeamA.CountdownToDeployOnInteractPress.Resume();

            if (TeamA.Players != null)
			{
				for (int i = 0; i < TeamA.Players.Count; i++)
				{
					Player p = TeamA.Players[i];
					if (p.CountdownToDeployActiveAbility != null)
						p.CountdownToDeployActiveAbility.Resume();
				}
			}

            if (TeamB.Interact != null)
                TeamB.Interact.enabled = _buttonStatus[4];

            if (TeamB.CountdownToDeployOnInteractPress != null)
                TeamB.CountdownToDeployOnInteractPress.Resume();

            if (TeamB.Players != null)
			{
				for (int i = 0; i < TeamB.Players.Count; i++)
				{
					Player p = TeamB.Players[i];
					if (p.CountdownToDeployActiveAbility != null)
						p.CountdownToDeployActiveAbility.Resume();
				}
			}
        }

		/// <summary>
		/// Process keyboard inputs.
		/// This method doesn't work if you are running the game from Simulator or on a smartphone.
		/// Needed for internal testing and debug purpose.
		/// </summary>
		private void ProcessKeyDownForDebug()
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				if (TeamA.Interact.isActiveAndEnabled)
					TeamA.Interact.onClick?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (TeamA.Players[0].InteractAbility.isActiveAndEnabled)
					TeamA.Players[0].InteractAbility.onClick?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.W))
			{
				if (TeamA.Players[1].InteractAbility.isActiveAndEnabled)
					TeamA.Players[1].InteractAbility.onClick?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				if (TeamA.Players[2].InteractAbility.isActiveAndEnabled)
					TeamA.Players[2].InteractAbility.onClick?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.Keypad0))
			{
				if (TeamB.Interact.isActiveAndEnabled)
					TeamB.Interact.onClick?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				if (TeamB.Players[0].InteractAbility.isActiveAndEnabled)
					TeamB.Players[0].InteractAbility.onClick?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				if (TeamB.Players[1].InteractAbility.isActiveAndEnabled)
					TeamB.Players[1].InteractAbility.onClick?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.Keypad3))
			{
				if (TeamB.Players[2].InteractAbility.isActiveAndEnabled)
					TeamB.Players[2].InteractAbility.onClick?.Invoke();
			}

		}

		/// <summary>
		/// Setup the game.
		/// It instantiates ball, teams and characters.
		/// 
		/// </summary>
		private void Init()
		{
			SoccerBall = Instantiate<Ball>(_ballPrefab);
			SoccerBall.name = "Ball";

			TeamA = Instantiate<Team>(_teamPrefab);

			TeamB = Instantiate<Team>(_teamPrefab);

			TeamA.CreateTeam("Team A", TeamB, SoccerBall, GamePhase.Stall, TeamSideField.Bottom, _interactTeamA, _cooldownPeriodForPassingTeamA, _teamCircle[0]);
			TeamB.CreateTeam("Team B", TeamA, SoccerBall, GamePhase.Stall, TeamSideField.Top, _interactTeamB, _cooldownPeriodForPassingTeamB, _teamCircle[1]);

			// Init TeamA (human player) characters...
			for (int i = 0; i < _playersPerTeam; i++)
			{
				Player player = TeamManager.s_TeamManagerInstance?.Team[i];

				// Scene has been run by the menu and characters deck is populated.
				// This is the standard behavior expected in production.
				if (player != null)
				{
                    // Changing the material based on the team.				
                    SkinnedMeshRenderer renderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
                    Material[] mats = new Material[] { player.TeamAMaterial };
                    renderer.materials = mats;

					// Add character to the team...
					TeamA.AddPlayer(player, _spawnTransforms[i], _playerNames[i], _interactActiveAbility[i], _topGoal, _bottomGoal);
                }
                // Scene has been run by the editor.
                // This is the behavior expected in development.
                else
                {
					// We don't have a deck, so pick a random character from the list...
					player = PickUpRandomPlayerNotAlreadyChosen(_playersPrefabs, TeamA);

					// Changing the material based on the team.				
                    SkinnedMeshRenderer renderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
                    Material[] mats = new Material[] { player.TeamAMaterial };
                    renderer.materials = mats;

                    // Add character to the team...
                    if (player != null)
						TeamA.AddPlayer(player, _spawnTransforms[i], _playerNames[i], _interactActiveAbility[i], _topGoal, _bottomGoal);
				}
			}

            // Init TeamB (cpu) characters...
            for (int i = _playersPerTeam; i < _playersNumber; i++)
			{
                // Pick a random character from the list (duplicates not allowed)...
                Player player = PickUpRandomPlayerNotAlreadyChosen(_playersPrefabs, TeamB);

				// Changing the material based on the team.
                SkinnedMeshRenderer renderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
                Material[] mats = new Material[] { player.TeamBMaterial };
                renderer.materials = mats;

                // Add character to the team...
                if (player != null)
					TeamB.AddPlayer(player, _spawnTransforms[i], _playerNames[i], _interactActiveAbility[i], _topGoal, _bottomGoal);
			}

			// Init characters adjacency matrix and ball distances array...
			_playersDistances = new float[_playersNumber, _playersNumber];
			_ballDistances = new float[_playersNumber];
		}

		/// <summary>
		/// Choose a random character from the list.
		/// </summary>
		/// <param name="players">Character list</param>
		/// <param name="team">Team in which player will be linedup</param>
		/// <returns></returns>
		private Player PickUpRandomPlayerNotAlreadyChosen(Player[] players, Team team)
		{

			List<Player> playersAvailable = new List<Player>();

			foreach (Player pl in players)
			{
				bool found = false;

				foreach (Player plType in team.Players)
				{
					if (pl.GetType().Equals(plType.GetType()))
					{
						found = true;
						break;
					}
				}

				if (!found)
					playersAvailable.Add(pl);
			}

			if (playersAvailable != null && playersAvailable.Count > 0)
			{
				return playersAvailable[Random.Range(0, playersAvailable.Count)];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// KickOff method. Called to start the match.
		/// </summary>
		/// <param name="teamHasBall">Team that starts the kickoff</param>
		private void KickOff(Team teamHasBall = null)
		{
			if (teamHasBall == null)
			{
                if (Random.Range(0, 2) == 0)
                {
                    TeamA.SideOut(true);
                    TeamB.SideOut(false);
                }
                else
                {
                    TeamA.SideOut(false);
                    TeamB.SideOut(true);
                }
            }
            else
			{
				if (teamHasBall == TeamA)
				{
					TeamA.SideOut(true);
					TeamB.SideOut(false);
				}
				else
				{
					TeamA.SideOut(false);
					TeamB.SideOut(true);
				}
			}
		}

		/// <summary>
		/// Triggered when match time is over.
		/// Stop the match, stop the music and display the outcome.
		/// </summary>
        private void TimeIsOver()
        {
            UIManager instance = UIManager.Instance;

            Phase = MatchPhase.Ended;

            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlayEffect(_sfxClipMatchEnd);

            instance._endGamePanel.SetActive(true);
            instance._finalScorePlayer.text = _teamAScore.ToString();
            instance._finalScoreOpponent.text = _teamBScore.ToString();

            if (_teamAScore < _teamBScore)
            {
                instance._winLoseText.text = "DEFEAT";
                instance._endGameText.text = "You get 50 gold";

                SoundManager.Instance.PlayEffect(_sfxClipPlayerLoses);

            }
            else if (_teamAScore > _teamBScore)
            {
                instance._winLoseText.text = "VICTORY";
                instance._endGameText.text = "You get 300 gold";

                SoundManager.Instance.PlayEffect(_sfxClipPlayerWins);

            }
            else if (_teamAScore == _teamBScore)
            {
                instance._winLoseText.text = "DRAW";
                instance._endGameText.text = "You get 100 gold";

                SoundManager.Instance.PlayEffect(_sfxClipMatchDraw);
            }
        }

		/// <summary>
		/// Manages the goal transition.
		/// Stop the opponent characters, play the sound effect, blend to the right
		/// virtual camera and then reposition characters to initial spawn spot to start
		/// a new set.
		/// </summary>
		/// <param name="player">Character who scored goal</param>
		/// <returns></returns>
        private IEnumerator WaitGoal(Player player)
        {
            player.IsRepositioning = true;
            yield return new WaitForSeconds(1f);

            player.Animator.SetTrigger("Goal");

            List<Player> allplayers = new List<Player>();

            foreach (Player playerA in TeamA.Players)
            {
                allplayers.Add(playerA);
            }
            foreach (Player playerB in TeamB.Players)
            {
                allplayers.Add(playerB);
            }

            allplayers.Remove(player);

            foreach (Player players in allplayers)
            {
                players.IsMoving = false;
            }

            StartCoroutine(BallGoalMovement(2f, player, 15f));

            SoundManager.Instance.PlayEffect(_sfxClipGoalScore);

            yield return new WaitUntil(() => _isFinishedAnimGoal);

            if (TeamA.Players.Contains(player))
            {

                _vGoalCam[0].Priority = 1;
            }
            else
            {
                _vGoalCam[1].Priority = 1;
            }

            SoccerBall.gameObject.SetActive(false);
            player.IsRepositioning = false;

            foreach (Player pl in TeamA.Players)
            {
                pl.RemoveAllModifiers();
                pl.RepositionPlayer();
            }

            foreach (Player pl in TeamB.Players)
            {
                pl.RemoveAllModifiers();
                pl.RepositionPlayer();
            }

            if (TeamA.Players.Contains(player))
            {
                _teamAScore++;
                UIManager.Instance.ScoreGoalTeamA(_teamAScore);
                KickOff(TeamB);
            }
            else
            {
                _teamBScore++;
                UIManager.Instance.ScoreGoalTeamB(_teamBScore);
                KickOff(TeamA);
            }

            player.gameObject.GetComponent<Collider>().enabled = true;
        }

        private IEnumerator BallGoalMovement(float t, Player player, float speed)
        {
            _isFinishedAnimGoal = false;
            Vector3 moveDirection;

            if (TeamA.Players.Contains(player))
            {
                moveDirection = (player.GoalA.transform.position - player.transform.position).normalized;
            }
            else
            {
                moveDirection = (player.GoalB.transform.position - player.transform.position).normalized;
            }

            while (t > 0)
            {
                if (TeamA.Players.Contains(player))
                {
                    if (SoccerBall.gameObject.transform.position.z >= player.GoalA.transform.position.z)
                    {
                        _isFinishedAnimGoal = true;
                        yield break;
                    }
                }
                else
                {
                    if (SoccerBall.gameObject.transform.position.z <= player.GoalB.transform.position.z)
                    {
                        _isFinishedAnimGoal = true;
                        yield break;
                    }
                }

                SoccerBall.gameObject.transform.position += moveDirection * speed * Time.deltaTime;

                t -= Time.deltaTime;
                yield return null;
            }
        }
        #endregion

        #region public methods
		/// <summary>
		/// Trigger a ball change.
		/// </summary>
        public void TimeOut()
		{
			TeamA.TimeOut();
			TeamB.TimeOut();
		}

		/// <summary>
		/// Blend to the right virtual camera on character score.
		/// </summary>
		/// <param name="player"></param>
		public void PlayerScore(Player player)
		{
			_matchPhase = MatchPhase.WaitingToStartNewSet;		
			
			if (TeamA.Players.Contains(player))
			{
				_vGoalCam[0].Priority = 100;
			}
			else
			{
				_vGoalCam[1].Priority = 100;
			}
			
			StartCoroutine(WaitGoal(player));
		}
        #endregion

        #region Players distances calculator
		/// <summary>
		/// Update adjacency matrix frame by frame.
		/// </summary>
        private void CalculatePlayersDistances()
		{
			if (_playersDistances != null)
			{
				Player firstPlayer = null;
				Player secondPlayer = null;

				for (int i = 0; i < _playersNumber; i++)
				{
					if (i < _playersPerTeam)
						firstPlayer = TeamA.Players[i];
					else
						firstPlayer = TeamB.Players[i - _playersPerTeam];

					for (int k = 0; k < _playersNumber; k++)
					{
						if (i == k) break;

						if (k < _playersPerTeam)
							secondPlayer = TeamA.Players[k];
						else
							secondPlayer = TeamB.Players[k - _playersPerTeam];

						//Debug.Log($"Distance between {firstPlayer.name} - {secondPlayer.name}");
						Vector3 firstPlayerPosition = new Vector3(firstPlayer.transform.position.x, 0, firstPlayer.transform.position.z);
						Vector3 secondPlayerPosition = new Vector3(secondPlayer.transform.position.x, 0, secondPlayer.transform.position.z);

						_playersDistances[i, k] = _playersDistances[k, i] = Vector3.Distance(firstPlayerPosition, secondPlayerPosition);
					}
				}
			}
		}

        private float GetPlayersDistance(int p1, int p2) => _playersDistances[p1, p2];

        /// <summary>
        /// Get distance between character 1 and 2.
        /// </summary>
        /// <param name="p1">Character 1</param>
        /// <param name="p2">Character 2</param>
        /// <returns></returns>
        public float GetPlayersDistance(Player player1, Player player2)
		{
			int p1 = GetMatrixIndexByPlayerRef(player1);
			int p2 = GetMatrixIndexByPlayerRef(player2);

			Debug.Assert(p1 != -1 && p2 != -1, $"Issues in getting distance between players !!");

			return GetPlayersDistance(p1, p2);
		}

		/// <summary>
		/// Get nearest player in the other team.
		/// Used by some skill...
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public Player GetNearestPlayerInOpposingTeam(Player player)
		{
			int i = GetMatrixIndexByPlayerRef(player);

			int upperIndexBound;
			int lowerIndexBound;

			float distance = float.PositiveInfinity;
			int nearestPlayerIndex = -1;

			if (i < _playersPerTeam)
			{
				upperIndexBound = _playersNumber - 1;
				lowerIndexBound = _playersPerTeam;
			}
			else
			{
				upperIndexBound = _playersPerTeam - 1;
				lowerIndexBound = 0;
			}

			for (int j = lowerIndexBound; j <= upperIndexBound; j++)
			{
				//Debug.Log($"Checking distance between {i} - {j}");
				if (_playersDistances[i, j] < distance)
				{
					distance = _playersDistances[i, j];
					nearestPlayerIndex = j;
				}
			}

			if (nearestPlayerIndex != -1)
				return GetPlayerRefByMatrixIndex(nearestPlayerIndex);
			else
				return null;
		}

		/// <summary>
		/// Get characters list in opposing team within given radius.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public List<Player> GetPlayersWithinRadiusInOpposingTeam(Player player, float radius)
		{
			int i = GetMatrixIndexByPlayerRef(player);

			int upperIndexBound;
			int lowerIndexBound;

			List<Player> players = new List<Player>();

			if (i < _playersPerTeam)
			{
				upperIndexBound = _playersNumber - 1;
				lowerIndexBound = _playersPerTeam;
			}
			else
			{
				upperIndexBound = _playersPerTeam - 1;
				lowerIndexBound = 0;
			}

			for (int j = lowerIndexBound; j <= upperIndexBound; j++)
			{
				//Debug.Log($"Checking distance between {i} - {j}");
				if (_playersDistances[i, j] < radius)
				{
					players.Add(GetPlayerRefByMatrixIndex(j));
				}
			}

			if (players.Count >= 1)
				return players;
			else
				return null;
		}

		/// <summary>
		/// Get nearest character in same team.
		/// Used to pass ball to a mate.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public Player GetNearestPlayerInSameTeam(Player player)
		{
			int i = GetMatrixIndexByPlayerRef(player);

			int upperIndexBound;
			int lowerIndexBound;

			float distance = float.PositiveInfinity;
			int nearestPlayerIndex = -1;

			if (i < _playersPerTeam)
			{
				upperIndexBound = _playersPerTeam - 1;
				lowerIndexBound = 0;
			}
			else
			{
				upperIndexBound = _playersNumber - 1;
				lowerIndexBound = _playersPerTeam;
			}

			for (int j = lowerIndexBound; j <= upperIndexBound; j++)
			{
				if (i == j) continue;

				//Debug.Log($"Checking distance between {i} - {j}");
				if (_playersDistances[i, j] < distance)
				{
					distance = _playersDistances[i, j];
					nearestPlayerIndex = j;
				}
			}

			if (nearestPlayerIndex != -1)
				return GetPlayerRefByMatrixIndex(nearestPlayerIndex);
			else
				return null;
		}

        /// <summary>
        /// Get nearest character in same team ahead given player.
        /// Used to pass ball to a mate.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Player GetNearestPlayerAheadInSameTeam(Player player)
		{
			int i = GetMatrixIndexByPlayerRef(player);
			Team team = GetTeamByPlayerRef(player);

			int upperIndexBound;
			int lowerIndexBound;

			float distance = float.PositiveInfinity;
			int nearestPlayerIndex = -1;

			if (i < _playersPerTeam)
			{
				upperIndexBound = _playersPerTeam - 1;
				lowerIndexBound = 0;
			}
			else
			{
				upperIndexBound = _playersNumber - 1;
				lowerIndexBound = _playersPerTeam;
			}

			for (int j = lowerIndexBound; j <= upperIndexBound; j++)
			{
				if (i == j) continue;

				//Debug.Log($"Checking distance between {i} - {j}");
				if (_playersDistances[i, j] < distance)
				{
					if (team.SideField == TeamSideField.Top)
					{
						Player playerClose = GetPlayerRefByMatrixIndex(j);
						if (playerClose.transform.position.z < player.transform.position.z)
						{
							distance = _playersDistances[i, j];
							nearestPlayerIndex = j;
						}
					}
					else if (team.SideField == TeamSideField.Bottom)
					{
						Player playerClose = GetPlayerRefByMatrixIndex(j);
						if (playerClose.transform.position.z > player.transform.position.z)
						{
							distance = _playersDistances[i, j];
							nearestPlayerIndex = j;
						}

					}
				}
			}

			if (nearestPlayerIndex != -1)
				return GetPlayerRefByMatrixIndex(nearestPlayerIndex);
			else
				return null;
		}

		/// <summary>
		/// Internal method to query the matrix at low level.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		private int GetMatrixIndexByPlayerRef(Player player)
		{
			int index;

			if ((index = TeamA.Players.FindIndex(x => x == player)) != -1)
				return index;
			else if ((index = TeamB.Players.FindIndex(x => x == player)) != -1)
				return index + _playersPerTeam;
			else
				return -1;
		}

        /// <summary>
        /// Internal method to query the matrix at low level.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private Player GetPlayerRefByMatrixIndex(int i)
		{
			if (i >= 0 && i < _playersPerTeam)
				return TeamA.Players[i];
			else if (i >= _playersPerTeam && i < _playersNumber)
				return TeamB.Players[i - _playersPerTeam];
			else
				return null;
		}

		/// <summary>
		/// Return the team for a given player.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public Team GetTeamByPlayerRef(Player player)
		{
			if (TeamA.Players.Contains(player)) return TeamA;
			else if (TeamB.Players.Contains(player)) return TeamB;
			else return null;
		}

		/// <summary>
		/// Print the entire adjacency matrix.
		/// Use it for debugging purpose.
		/// </summary>
		private void PrintPlayersDistances()
		{
			Debug.Log("Players distances:");

			for (int i = 0; i < _playersNumber; i++)
			{
				for (int k = 0; k < _playersNumber; k++)
				{
					Debug.Log($"[{i},{k}]: {_playersDistances[i, k]}");
				}
			}
		}
		#endregion

		#region Ball distances calculator
		private void CalculateBallDistances()
		{
			if (_ballDistances != null)
			{
				for (int i = 0; i < _playersNumber; i++)
				{
					Player p = GetPlayerRefByMatrixIndex(i);

					if (p != null)
					{
						Vector3 playerPosition = new Vector3(p.transform.position.x, 0, p.transform.position.z);
						Vector3 ballPosition = new Vector3(SoccerBall.transform.position.x, 0, SoccerBall.transform.position.z);

						_ballDistances[i] = Vector3.Distance(playerPosition, ballPosition);
					}
				}
			}
		}

		/// <summary>
		/// Used to check if mate can get the ball (if he is in range).
		/// </summary>
		/// <param name="player"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public bool CanGetBall(Player player, float radius)
		{
			int i = GetMatrixIndexByPlayerRef(player);

			if (i != -1)
			{
				return (_ballDistances[i] < radius);
			}

			return false;
		}

        /// <summary>
        /// Print the ball distances array.
        /// Use it for debugging purpose.
        /// </summary>
        private void PrintBallDistances()
		{
			Debug.Log("Ball distances:");

			for (int i = 0; i < _playersNumber; i++)
			{
				Debug.Log($"[{i}]: {_ballDistances[i]}");
			}
		}
		#endregion

		#region Singleton
		// Boring singleton pattern stuff...
		public static MatchManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				DestroyImmediate(this.gameObject);
			}
			else
			{
				Instance = this;
				//DontDestroyOnLoad(gameObject);
			}

			_playersNumber = _playersPerTeam * 2;

			Debug.Assert(
					_playersNumber == _spawnTransforms.Count() &&
						_playersNumber == _spawnTransforms.Count(),
					"Number of players must be equal to: spawn positions array and player names array");
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
