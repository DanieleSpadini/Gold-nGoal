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
    /// <summary>
    /// This class manages the players that make up the team.
    /// Setups the team, Manages the interaction button and its cooldown period, changes team behavior based on phase.
    /// </summary>
    [DisallowMultipleComponent]
    public class Team : MonoBehaviour
    {
        // Team name.
        // Not shown to the user, maybe in the future?
        // Useful for debugging.
        private string _name;
        public string Name
        {
            get => _name;
            private set { }
        }

        // List of characters that make up the team.
        private List<Player> _players = null;
        public List<Player> Players
        {
            get => _players;
            private set => _players = value;
        }

        // Opposing team ref.
        private Team _opposingTeam = null;

        // Ball ref.
        private Ball _soccerBall;

        // Interaction button ref.
        private Button _interact;
        public Button Interact { get => _interact; }

        // Cooldown for interaction.
        private Timer _countdownToDeployOnInteractPress;
        public Timer CountdownToDeployOnInteractPress { get => _countdownToDeployOnInteractPress; }
        private float _cooldownPeriodOnInteractPress;

        // The current phase of the team.
        // Check the enum in Globals for the list of possible values...
        private GamePhase _phase;
	    public GamePhase Phase
        {
            get => _phase;
            private set => _phase = value;
        }

        // The previous phase of the team.
        private GamePhase _previousPhase;
        public GamePhase PreviousPhase
        {
            get => _previousPhase;
            private set => _previousPhase = value;
        }

        // Which field the team occupies (basically top or bottom).
        public TeamSideField SideField;

        // Keep track of the nearest mate without ball.
        // Becomes the possible player to pass the ball to...
        private Player nearestAheadWithoutBallInPreviousFrame;

        // Indexer to iterate the characters from the Team class.
        public Player this[int index] { get => _players[index]; }

        // The number of characters in the team.
        public int PlayerCount { get => _players.Count(); }

        // Particle ref for player highlighting...
        public GameObject _teamCircle;

        #region lifecycle method
        private void Update()
        {
            // If game is running...
            if (MatchManager.Instance.Phase == MatchPhase.InGame)
            {
                // If team is in attack phase...
                if (Phase == GamePhase.Attack)
                {
                    // Find who has the ball...
                    Player whoHasBall = _players.Find(x => x.HasBall == true);

                    if (whoHasBall != null)
                    {
                        // Check who I can pass the ball to...
                        Player nearestAhead = MatchManager.Instance.GetNearestPlayerAheadInSameTeam(whoHasBall);
                        Player nearestWithoutBall = nearestAhead != null ? nearestAhead : MatchManager.Instance.GetNearestPlayerInSameTeam(whoHasBall);

                        if (nearestAheadWithoutBallInPreviousFrame != null)
                        {
                            // If mate has changed, turn off his flag and turn on the new one...
                            if (nearestWithoutBall != nearestAheadWithoutBallInPreviousFrame)
                            {
                                nearestAheadWithoutBallInPreviousFrame?.CanReceiveBall(false);
                                nearestWithoutBall?.CanReceiveBall(true);

                                nearestAheadWithoutBallInPreviousFrame = nearestWithoutBall;
                            }
                        }
                        else
                        {
                            nearestWithoutBall?.CanReceiveBall(true);
                            nearestAheadWithoutBallInPreviousFrame = nearestWithoutBall;
                        }
                    }
                }

                // If the ball is lost, save the current phase (in order to be able to resume it if necessary) and
                // trigger the scramble phase...
                if (_soccerBall.IsFreeToRolls && _soccerBall.CurrentSpeed <= 0)
                {
                    if (Phase != GamePhase.Scramble)
                    {
                        PreviousPhase = Phase;
                    }
                    Phase = GamePhase.Scramble;
                }
            }
        }
        #endregion

        /// <summary>
        /// Setup a team.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="opposingTeam"></param>
        /// <param name="soccerBall"></param>
        /// <param name="phase"></param>
        /// <param name="sideField"></param>
        /// <param name="interact"></param>
        /// <param name="cooldownTimeOnInteractPress"></param>
        /// <param name="teamCircle"></param>
        public void CreateTeam(string name, Team opposingTeam, Ball soccerBall, GamePhase phase, TeamSideField sideField, Button interact, float cooldownTimeOnInteractPress, GameObject teamCircle)
        {
            gameObject.name = name;
            _name = name;

            _opposingTeam = opposingTeam;
            _soccerBall = soccerBall;
            Phase = phase;
            SideField = sideField;

            _players = new List<Player>();

            _interact = interact;
            _cooldownPeriodOnInteractPress = cooldownTimeOnInteractPress;

            _teamCircle = teamCircle;

            _interact.onClick.AddListener(InteractButtonLifecycle);

            _interact.enabled = false;

            _countdownToDeployOnInteractPress = Timer.CreateTimer($"{this} - Interact timer", _cooldownPeriodOnInteractPress, true, false, EnableInteractButton);

        }

        /// <summary>
        /// Add character to the team.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="spawnSpot"></param>
        /// <param name="name"></param>
        /// <param name="interactAbility"></param>
        /// <param name="topGoal"></param>
        /// <param name="bottomGoal"></param>
        public void AddPlayer(Player player, Transform spawnSpot, string name, Button interactAbility, Transform topGoal, Transform bottomGoal)
        {
            Player newPlayer = Instantiate<Player>(player);
            newPlayer.LineUpPlayer(name, this, spawnSpot, _opposingTeam, _soccerBall, _interact, interactAbility, topGoal, bottomGoal);

            _players.Add(newPlayer);
        }


        #region interaction button lifecycle
        /// <summary>
        /// Manages the lifecycle of the interaction button.
        /// Configure the cooldown after which the button is enabled...
        /// </summary>
        public void InteractButtonLifecycle()
        {
            _interact.enabled = false;
            _countdownToDeployOnInteractPress = Timer.CreateTimer($"{this} - Cooldown interact timer", _cooldownPeriodOnInteractPress, true, false, EnableInteractButton);
        }

        public void EnableInteractButton()
        {
            _interact.enabled = true;
        }
        #endregion

        /// <summary>
        /// Manages the ball change of the match.
        /// </summary>
        /// <param name="active"></param>
        /// <param name="whoHasBall"></param>
        public void SideOut(bool active, int whoHasBall = -1)
        {
            int playerIndexHasBall = (active && whoHasBall == -1) ? Random.Range(0, PlayerCount) : whoHasBall;

            for (int i = 0; i < _players.Count; i++)
            {
                if (active && playerIndexHasBall == i)
                    _players[i].HasBall = true;
                else
                    _players[i].HasBall = false;

                _players[i].CanReceiveBall(false);
            }

            if (active)
            {
                Phase = GamePhase.Attack;
            }                
            else
            {
                Phase = GamePhase.Defense;
            }

            nearestAheadWithoutBallInPreviousFrame = null;
        }
    
        /// <summary>
        /// Suspend the match.
        /// </summary>
        public void TimeOut()
        {
            Phase = GamePhase.Stall;
        }

        /// <summary>
        /// Routine for repositioning players after losing the ball.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="area"></param>
        /// <param name="repositionDistance"></param>
        public void TeamLostBall(Player player, float area,float repositionDistance)
        {
            List<Player> affectedplayers = new List<Player> ();
            affectedplayers = MatchManager.Instance.GetPlayersWithinRadiusInOpposingTeam(player, area);
            
            if (affectedplayers == null)
                return;

            foreach(Player movePlayers in affectedplayers)
            {                
                if(movePlayers._team == MatchManager.Instance.TeamA)
                {
                    Vector3 movedVectors = new Vector3(movePlayers.SpawnSpot.transform.position.x,
                                                        movePlayers.transform.position.y,
                                                        Mathf.Clamp((movePlayers.transform.position.z - repositionDistance),
                                                        movePlayers.SpawnSpot.transform.position.z, -movePlayers.SpawnSpot.transform.position.z));
                    
                    StartCoroutine(movePlayers.MovePlayerToPosition(0.5f, movedVectors));
                }
                else
                {
                    Vector3 movedVectors = new Vector3(movePlayers.SpawnSpot.transform.position.x,
                                                        movePlayers.transform.position.y,
                                                        Mathf.Clamp((movePlayers.transform.position.z + repositionDistance), 
                                                        -movePlayers.SpawnSpot.transform.position.z, movePlayers.SpawnSpot.transform.position.z));

                    StartCoroutine(movePlayers.MovePlayerToPosition(0.5f, movedVectors));
                }
            }
        }
    }
}
