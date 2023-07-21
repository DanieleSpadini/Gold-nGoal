using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using S = System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Ability;
using UnityEngine.Events;

namespace Core
{

    /// <summary>
    /// This class manages the behaviours of the player during the match.
    /// It holds all the references of the scriptableobjects that define: the speed, the actionradius and the modifier definition
    /// of the ability of the player prefab.
    /// It also manages the animator transitions accordingly with the state of the player.
    ///  
    /// </summary>


    [DisallowMultipleComponent]
    public class Player : MonoBehaviour
    {
        [Header("Refs")]
        //The spot to which the ball is snapped when a player has the ball.
        [SerializeField,Tooltip("The spot where the ball is snapped when a player has the ball")]
        private Transform _ballSpot;

        //The immage that is show according to the closest player who is avaible for the pass.
        [SerializeField,Tooltip("The immagine displayed for the closest player avaible for a pass")]
        private Image _highlight;

        //The reference to the animator controlling the player animation.
        //The name of the Animator should always match the name of the prefab.
        [SerializeField,Tooltip("The reference to the animator, always fill with the animator that matches the name of the player prefab")]
        private Animator _animator;
        public Animator Animator { get => _animator; }

        //The reference to the UI button of ths player that will be assigned at runtime.
        //This button handles the pass and intercept of the player.
        [SerializeField, Tooltip("The reference to the UI button for the pass and intercept of the player")]
        private Button _interaction;

        //The VFX reference that is assigned at runtime of the teamcircle.
        [SerializeField,Tooltip("This reference is assigned at runtime, it holds the vfx teamcircle and it changes accordingly with the team the player is in")]
        private GameObject _teamCircle;
        public GameObject TeamCircle
        {
            get => _teamCircle;
            set => _teamCircle = value;
        }

        //The reference that is passed from the team selection in the Menu Scene.
        [Tooltip("Always fill this reference with the scriptableobject of heropannel with the same name as this player prefab name")]
        public HeroPanelAttributes _heroPanelAttributes;

        //The 2 materials references of the 2 team shirts.
        [Tooltip("Fill this with the shirts material of team A in Materials/Characters/ and the selected player prefab")]
        public Material TeamAMaterial;
        [Tooltip("Fill this with the shirts material of team B in Materials/Characters/ and the selected player prefab")]
        public Material TeamBMaterial;

        //The reference of the Trail object inside the player prefab.
        [SerializeField,Tooltip("Always assign this reference with the child gameobject named Trail if there is one. It should alwyas start not active")]
        private GameObject _trail;
        public GameObject Trail { get => _trail; }

        //The reference of the ShockWave object inside the player prefab.
        [SerializeField,Tooltip("Always assign this reference with the child gameobject named ShockWave if there is one. It should alwyas start not active")]
        private GameObject _shockWave;
        public GameObject ShockWave { get => _shockWave; }

        //The reference of the transform of the immage of the pass.
        //This transform change the way you see the immage from in game.
        [SerializeField,Tooltip("The posistion where you see the immage of the pass in game. To change it open the prefab and move manually the gameobejet in the canvas TeamA ")]
        private Transform _teamAPassUI;

        //The reference of the transform of the immage of the pass.
        //This transform change the way you see the immage from in game.
        [SerializeField, Tooltip("The posistion where you see the immage of the pass in game. To change it open the prefab and move manually the gameobejet in the canvas TeamB ")]
        private Transform _teamBPassUI;

        //Audio reference to the player passing the ball.
        [SerializeField, Tooltip("Ball pass audio effect reference")]
        private AudioClip _sfxClipPassBall;

        [Space(20)]
        [Header("Debug")]
        /// <summary>
        /// This section is used for Debug purposes it displays in numeric form: the speed, the speed with the ball,
        /// the action radius and the current actionradius.
        /// It also shows which modifiers definition are applied to the player during the game, the position of the player in the current frame and his direction.
        /// <summary>

        [SerializeField]
        protected float _speed;
        [SerializeField]
        protected float _kickPower;

        [SerializeField]
        protected float _actionRadius;

        public float ActionRadius { get => _actionRadius; }

        [SerializeField]
        private float _currentActionRadius;
        //The set property of the current actionradius is used to manage the VFX attached to the player of the teamCircle.
        public float CurrentActionRadius
        {
            get => _currentActionRadius;
            set
            {
                if (_currentActionRadius != value)
                {
                    TeamCircle tc = GetComponentInChildren<TeamCircle>();
                    tc.StopParticle();

                    _currentActionRadius = value;

                    tc.CircleRadius = _currentActionRadius;

                    tc.StartParticle();
                }
            }
        }


        [SerializeField]
        protected float _speedWithBall;
        
        //This is the current speed of the player factoring the possible changes due to the player receiving debuffs on the speed from abilities
        [SerializeField,Tooltip("This shows the current speed of the player factoring the debuffs thta the player can receive")]
        public float CurrentSpeed = 0f;

        [SerializeField,Tooltip("This list shows which modifiers are applied to the playe during the game")]
        private List<ModifierDefinition> _modifiers = null;

        //Flag that is used to start the animation of the goal.
        private bool isGoal;
        public bool IsGoal { get => isGoal; }

        //Timer that is created at runtime that handles the countdown of the ability.
        //The player can use his active ability only when this timer reached 0.
        private Timer _countdownToDeployActiveAbility;
        public Timer CountdownToDeployActiveAbility { get => _countdownToDeployActiveAbility; }

        //Variable that is used to populate the creation of the timer with the desired one.
        private float _cooldownPeriodActiveAbility;

        //Unity Action that is connected to the UI button of the ability that handles the deployment of the baility
        [SerializeField]
        private UnityAction _actionAtActiveButtonPressed;

        
        private string _name;
        public string Name
        {
            get => _name;
            private set { }
        }

        //This transform is assigned at runtime to every player, it determines where the player is moved at the start of the match.
        private Transform _spawnSpot = null;
        public Transform SpawnSpot { get => _spawnSpot; } 

        //The team to which this player is assigned at the start of the game.
        public Team _team = null;

        //The opposing team of this player.
        private Team _opposingTeam = null;

        //The object Ball in the game.
        private Ball _soccerBall = null;

        //The Vector3 that displays in inspector the current position of the player every frame.
        [SerializeField,Tooltip("This shows the position of the player in the current frame")]
        private Vector3 _deltaPos;

        //The Vector3 that displays in inspector the current direction of the player every frame.
        [SerializeField,Tooltip("This shows the direction of the player in the current frame")]
        private Vector3 _direction;
        public Vector3 Direction { get => _direction; }


        //This flag shows if the player has the ball.
        [SerializeField]
        public bool HasBall = false;

        //This flag shows if the player can receive the ball.
        //It is by default true but it can become false if the player is hit by an ability that stun or displace him.
        [SerializeField]
        public bool CanGetBall = true;

        //Flag that shows if the the interact button is in cooldown.
        [SerializeField]
        private bool _isInCoolDown = false;

        //This flag shows if the player is allowed to move.
        //This changes due to the player reaching the goal line or by being hit by an ability that can stop him.
        [SerializeField]
        private bool _isMoving = false;
        public bool IsMoving 
        { 
            get => _isMoving;  
            set => _isMoving = value;
        }


        //This flag shows if the player is repositiong.
        //This changes due to the player team score or by the player receiving an ability thta displace him.
        private bool _isRepositioning = false;
        public bool IsRepositioning
        {
            get => _isRepositioning;
            set => _isRepositioning = value;
        }

        //This flag shows if the direction can be updated.
        //This changes due to the player receiving a debuff that stun him or displace him.
        [SerializeField]
        public bool CanUpdateDirection = true;


        /// <summary>
        /// In this section the references are transcribed from the match manager according to which team the player is on
        /// </summary>

        //This button is filled at runtime with the assigned Ui button for the intercat action.
        private Button _interact;

        //This button is filled at runtime with the assigned Ui button for the ability action.
        protected Button _interactAbility;
        public Button InteractAbility { get => _interactAbility; }


        //This reference is transcribed from the matchManager information for the transform of the goal of team A.
        private Transform _goalA;
        public Transform GoalA { get => _goalA; }
        
        //This reference is transcribed from the matchManager information for the transform of the goal of team B.
        private Transform _goalB;
        public Transform GoalB { get => _goalB; }

        //This reference is transcribed from the matchManager information for the transform of left barrier.
        private Transform _leftBarrierRef;

        //This reference is transcribed from the matchManager information for the transform of right barrier.
        private Transform _rightBarrierRef;

        //This reference is transcribed from the matchManager information for the transform of top barrier.
        private Transform _topBarrierRef;

        //This reference is transcribed from the matchManager information for the transform of bottom barrier.
        private Transform _bottomBarrierRef;

        //This reference is transcribed from the matchManager information for the Vector3 of left barrier.
        private Vector3 _leftBarrier;

        //This reference is transcribed from the matchManager information for the Vector3 of right barrier.
        private Vector3 _rightBarrier;
        
        //This reference is transcribed from the matchManager information for the Vector3 of right barrier.
        private Vector3 _topBarrier;

        //This reference is transcribed from the matchManager information for the Vector3 of bottom barrier.
        private Vector3 _bottomBarrier;

        /// <summary>
        /// These variables are used to display the rays of the steering behaviour in the OnDrawGizmos function
        /// </summary>
        private int _rays;
        private float _rayAngle;
        private float _rayRange;

        private void Awake()
        {
            // Create the list that will be populated by the modifiers applaied to this player
            _modifiers = new List<ModifierDefinition>();

            // Populate the barriers references
            _leftBarrierRef = GameObject.FindGameObjectWithTag(Globals.LEFT_BARRIER).transform;
            _rightBarrierRef = GameObject.FindGameObjectWithTag(Globals.RIGHT_BARRIER).transform;
            _topBarrierRef = GameObject.FindGameObjectWithTag(Globals.TOP_DRAG_AREA).transform;
            _bottomBarrierRef = GameObject.FindGameObjectWithTag(Globals.BOTTOM_DRAG_AREA).transform;

            // Populate the barriers that check if the players is allowed to move
            _leftBarrier = new Vector3(_leftBarrierRef.transform.position.x + _leftBarrierRef.GetComponent<BoxCollider>().bounds.size.x,
                                        _leftBarrierRef.transform.position.y,
                                        _leftBarrierRef.transform.position.z);

            _rightBarrier = new Vector3(_rightBarrierRef.transform.position.x - _rightBarrierRef.GetComponent<BoxCollider>().bounds.size.x,
                                        _rightBarrierRef.transform.position.y,
                                        _rightBarrierRef.transform.position.z);

            _topBarrier = new Vector3(_topBarrierRef.transform.position.x,
                                      _topBarrierRef.transform.position.y,
                                      _topBarrierRef.transform.position.z);

            _bottomBarrier = new Vector3(_bottomBarrierRef.transform.position.x,
                                         _bottomBarrierRef.transform.position.y,
                                         _bottomBarrierRef.transform.position.z);                      
        }
        private void OnEnable()
        {
            // Moves the player to the correct spawspot at the start of the game
            if (this._spawnSpot == null)
                return;
            else
                this.transform.position = _spawnSpot.position;
        }

        /// <summary>
        /// Virtual base start that is shared acros all players. 
        /// </summary>
        public virtual void Start()
        {
            //Calculates the speed with the ball.
            _speedWithBall = _speed / 2f;
            //Sets the current action radius .
            CurrentActionRadius = _actionRadius;

            //Sets the rays for the OndrawGizmos function for the steering Behaviour.
            _rays = MatchManager.Instance.Rays;
            _rayAngle = MatchManager.Instance.RayAngle;
            _rayRange = MatchManager.Instance.RayRange;   
        }

        private void Update()
        {
            //Checks for the change of the phase.
            if(MatchManager.Instance.Phase == MatchPhase.Paused)
            {
                return;
            }

            if (MatchManager.Instance.Phase == MatchPhase.Ended)
            {
                return;
            }

            //If the phase is WaitingToStartNewSet play the Goal Animation.
            if(MatchManager.Instance.Phase == MatchPhase.WaitingToStartNewSet)
            {
                AnimationMove(CurrentSpeed, _isMoving);
                return;
            }

            //If the phase is InGame and the players are not repositioning unlock the logic for the InGame behaviours.
            if (MatchManager.Instance.Phase == MatchPhase.InGame && !IsRepositioning)
            {
                //If the player has the Ball snap the ball to the correct transform and reduce the speed of the player, 
                //otherwise set the speed to the normal one.
                if (HasBall)
                {
                    _soccerBall.transform.parent = _ballSpot;
                    _soccerBall.transform.localPosition = Vector3.zero;
         
                    CurrentSpeed = _speedWithBall;                    
                }
                else
                {
                    CurrentSpeed = _speed;
                }

                //If the Team is in the attack phase changes the behaviour of the player accordingly.
                if (_team.Phase == GamePhase.Attack)
                {
                    AttackBehaviour();
                }
                //If the Team is in the defense phase changes the behaviour of the player accordingly.
                else if (_team.Phase == GamePhase.Defense)
                {
                    DefenseBehaviour();
                }
                //Special phase of the game that it is activated if none recieved the ball after a pass.
                else if (_team.Phase == GamePhase.Scramble)
                {
                    ScrambleBehaviour();
                }

                //Clear all the modifiers affecting this players if the inner timer of those ran out.
                ClearExpiredModifiers();

                //Execute the modifiers affecting this player if the inner time of those didn't run out.
                foreach (ModifierDefinition modifiers in _modifiers)
                {
                    modifiers.ExecuteModifier(this, Time.deltaTime);
                }

                //If the player is moving and doesn't have the ball ,check if it will collide with the barriers.
                if (_isMoving)
                {
                    if (!HasBall)
                    {
                        if (CanUpdateDirection)
                        {
                            _deltaPos = Vector3.zero;

                            for (int i = 0; i < _rays; i++)
                            {
                                Quaternion rotation = this.transform.rotation;
                                Quaternion rotationMod = Quaternion.AngleAxis(i / (((float)_rays) - 1) * _rayAngle * 2 - _rayAngle, transform.up);
                                Vector3 myDirection = rotation * rotationMod * Vector3.forward;

                                Vector3 rayStartingPoint = new Vector3(transform.position.x,
                                                                        transform.position.y + 1,
                                                                        transform.position.z);

                                Ray ray = new Ray(rayStartingPoint, myDirection);
                                RaycastHit hitInfo;

                                if (Physics.Raycast(ray, out hitInfo, _rayRange))
                                {
                                    _deltaPos -= (1f / _rays) * myDirection;
                                }
                                else
                                {
                                    _deltaPos += (1f / _rays) * myDirection;
                                }
                            }
                        }

                        if (_leftBarrier.x < (transform.position + (_deltaPos * CurrentSpeed * Time.deltaTime)).x &&
                            _rightBarrier.x > (transform.position + (_deltaPos * CurrentSpeed * Time.deltaTime)).x &&
                            _topBarrier.z > (transform.position + (_deltaPos * CurrentSpeed * Time.deltaTime)).z &&
                            _bottomBarrier.z < (transform.position + (_deltaPos * CurrentSpeed * Time.deltaTime)).z)
                        {
                            transform.position += _deltaPos * CurrentSpeed * Time.deltaTime;
                        }
                    }
                    else
                    {
                        if (_leftBarrier.x < (transform.position + (_direction * CurrentSpeed * Time.deltaTime)).x &&
                            _rightBarrier.x > (transform.position + (_direction * CurrentSpeed * Time.deltaTime)).x &&
                            _topBarrier.z > (transform.position + (_direction * CurrentSpeed * Time.deltaTime)).z &&
                            _bottomBarrier.z < (transform.position + (_direction * CurrentSpeed * Time.deltaTime)).z)
                        {
                            transform.position += _direction * CurrentSpeed * Time.deltaTime;
                        }
                    }
                }
                //Sends to the animator the state of the player such has his speed and if it is moving
                AnimationMove(CurrentSpeed, _isMoving);
            }
        }
        private void LateUpdate()
        {
            //Moves the immage of the passing according to where the player is oriented
            _highlight.transform.LookAt(transform.position + MatchManager.Instance.MainCamera.transform.rotation * Vector3.forward,
                                        MatchManager.Instance.MainCamera.transform.rotation * Vector3.up);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (int i = 0; i < _rays; i++)
            {
                Quaternion rotation = this.transform.rotation;
                Quaternion rotationMod = Quaternion.AngleAxis(i / (((float) _rays) - 1) * _rayAngle * 2 - _rayAngle, transform.up);
                Vector3 direction = rotation * rotationMod * Vector3.forward;

                Vector3 rayStartingPoint = new Vector3(transform.position.x,
                                                       transform.position.y + 1, 
                                                       transform.position.z);

                Gizmos.DrawRay(rayStartingPoint, direction);
            }
        }
#endif

        /// <summary>
        /// This OnTriggerEneter checks where the player entered.
        /// If the player has the ball this will start the Animation goal and it will award the Team that scored a point,
        /// if not it will stop the player
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (_team.SideField == TeamSideField.Bottom &&
                _team.Phase == GamePhase.Attack &&
                other.CompareTag(Globals.TOP_GOAL_AREA) &&
                HasBall)
            {
                // Team A Goal
                _isMoving = false;
                gameObject.GetComponent<Collider>().enabled = false;
                MatchManager.Instance.PlayerScore(this);
            }
            else if (_team.SideField == TeamSideField.Top &&
                     _team.Phase == GamePhase.Attack &&
                     other.CompareTag(Globals.BOTTOM_GOAL_AREA) &&
                     HasBall)
            {
                // Team B Goal
                _isMoving = false;
                gameObject.GetComponent<Collider>().enabled = false;
                MatchManager.Instance.PlayerScore(this);
            }
            else if (_team.SideField == TeamSideField.Bottom &&
                     _team.Phase == GamePhase.Attack &&
                     other.CompareTag(Globals.TOP_GOAL_AREA))
            {
                // Goal area reached. Wait.
                _isMoving = false;
            }
            else if (_team.SideField == TeamSideField.Top &&
                     _team.Phase == GamePhase.Attack &&
                     other.CompareTag(Globals.BOTTOM_GOAL_AREA))
            {
                // Goal area reached. Wait.
                _isMoving = false;
            }
            else if (_team.SideField == TeamSideField.Bottom &&
                     _team.Phase == GamePhase.Defense &&
                     other.CompareTag(Globals.BOTTOM_GOAL_AREA))
            {
                // Goal area reached. Wait.
                _isMoving = false;
            }
            else if (_team.SideField == TeamSideField.Top &&
                     _team.Phase == GamePhase.Defense &&
                     other.CompareTag(Globals.TOP_GOAL_AREA))
            {
                // Goal area reached. Wait.
                _isMoving = false;
            }
        }

        /// <summary>
        /// This OntriggerStay check if the player that is already in the goal area receive the ball,
        /// if it does it will start the goal animation and it will award the Team that scored a point
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
        {
            if (_team.SideField == TeamSideField.Bottom &&
                _team.Phase == GamePhase.Defense &&
                other.CompareTag(Globals.TOP_DRAG_AREA))
            {
                _isMoving = true;
            }
            else if (_team.SideField == TeamSideField.Top &&
                     _team.Phase == GamePhase.Defense &&
                     other.CompareTag(Globals.BOTTOM_DRAG_AREA))
            {
                _isMoving = true;
            }


            if (_team.SideField == TeamSideField.Bottom &&
                _team.Phase == GamePhase.Attack &&
                other.CompareTag(Globals.TOP_GOAL_AREA) &&
                HasBall)
            {
                // Team A Goal
                _isMoving = false;
                gameObject.GetComponent<Collider>().enabled = false;
                MatchManager.Instance.PlayerScore(this);
            }
            else if (_team.SideField == TeamSideField.Top &&
                     _team.Phase == GamePhase.Attack &&
                     other.CompareTag(Globals.BOTTOM_GOAL_AREA) &&
                     HasBall)
            {
                // Team B Goal
                _isMoving = false;
                gameObject.GetComponent<Collider>().enabled = false;
                MatchManager.Instance.PlayerScore(this);
            }

            if (_team.Phase == GamePhase.Scramble)
            {
                _isMoving = true;
            }
        }

        private void OnDisable()
        {
            //Remove the listneres for the interact button
            _interact.onClick.RemoveListener(OnInteractPress);
        }

        #region protected initialise character
        /// <summary>
        /// This initialise the player at the start of the game, it is used as a constructor for the player
        /// </summary>
        /// <param name="charSpeed"></param>
        /// <param name="charKickPower"></param>
        /// <param name="charActionRadius"></param>
        /// <param name="cooldownTimeActiveAbility"></param>
        /// <param name="actionAtActiveButtonPressed"></param>
        /// <param name="teamCircle"></param>
        protected void InitCharacter(float charSpeed,
                                     float charKickPower,
                                     float charActionRadius,
                                     float cooldownTimeActiveAbility,
                                     UnityAction actionAtActiveButtonPressed,
                                     GameObject teamCircle)
        {
            _speed = charSpeed;
            _speedWithBall = _speed / 2;

            _kickPower = charKickPower;
            _actionRadius = charActionRadius;

            _actionAtActiveButtonPressed = actionAtActiveButtonPressed;

            _cooldownPeriodActiveAbility = cooldownTimeActiveAbility;

            _teamCircle = teamCircle;

            if (_interactAbility != null)
            {
                _interactAbility.onClick.AddListener(ActiveAbilityButtonLifecycle);
                _interactAbility.onClick.AddListener(_actionAtActiveButtonPressed);

                _interactAbility.enabled = false;
            }

            _countdownToDeployActiveAbility = Timer.CreateTimer($"{this} - Cooldown ability timer", _cooldownPeriodActiveAbility, true, false, EnableActiveAbilityButton);

            Instantiate(_teamCircle, new Vector3(this.transform.position.x,0.1f, this.transform.position.z), Quaternion.identity, this.transform);

            if (_team.SideField == TeamSideField.Top)
                _highlight.rectTransform.SetParent(_teamBPassUI,false);
            else
                _highlight.rectTransform.SetParent(_teamAPassUI,false);

        }
        #endregion
       
        #region public methods
        /// <summary>
        /// This is used as a contructor for the player, assigning: the team, the spawnposition,
        /// the interact Ui button and Ability UI button, both references to
        /// goals which are used during the Goal animation and the reference to the ball
        /// </summary>
        /// <param name="name"></param>
        /// <param name="team"></param>
        /// <param name="spawnSpot"></param>
        /// <param name="opposingteam"></param>
        /// <param name="soccerBall"></param>
        /// <param name="interact"></param>
        /// <param name="interactAbility"></param>
        /// <param name="topGoal"></param>
        /// <param name="bottomGoal"></param>
        public void LineUpPlayer(string name, Team team, Transform spawnSpot, Team opposingteam, Ball soccerBall, Button interact, Button interactAbility, Transform topGoal, Transform bottomGoal)
        {
            gameObject.name = name;
            _name = name;
            _team = team;
            gameObject.transform.position = spawnSpot.position;
            gameObject.transform.parent = team.transform;
            _goalA = topGoal;
            _goalB = bottomGoal;

            _spawnSpot = spawnSpot;

            _opposingTeam = opposingteam;
            _soccerBall = soccerBall;

            _interact = interact;
            _interact.onClick.AddListener(OnInteractPress);

            _interactAbility = interactAbility;

            IsRepositioning = false;
        }

        /// <summary>
        /// Lifecycle of the Active Ability Button
        /// </summary>
        public void ActiveAbilityButtonLifecycle()
        {
            if (_interactAbility != null)
            {
                _interactAbility.enabled = false;
                _countdownToDeployActiveAbility = Timer.CreateTimer($"{this} - Cooldown ability timer", _cooldownPeriodActiveAbility, true, false, EnableActiveAbilityButton);
            }
        }

        /// <summary>
        /// Unlock the Active Ability Button 
        /// </summary>
        public void EnableActiveAbilityButton()
        {
            if (_interactAbility != null)
            {
                _interactAbility.enabled = true;
            }
        }

        /// <summary>
        /// Wrapper for the coroutine MovePlayerToPosition
        /// </summary>
        public void RepositionPlayer()
        {
            StartCoroutine(MovePlayerToPosition(2f, _spawnSpot.position));
        }

        /// <summary>
        /// Coroutine used to move the player after a goal or after loosing the ball.
        /// During this the player won't be able to interact with the UI buttons.
        /// This also manages the state of the animator.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerator MovePlayerToPosition(float duration, Vector3 position)
        {
            AnimationReposition(CurrentSpeed, true);

            bool buttonAbilityPreviousStatus = true;

            if (_interactAbility != null)
            {
                buttonAbilityPreviousStatus = _interactAbility.enabled;
                _interactAbility.enabled = false;
                _countdownToDeployActiveAbility.Pause();
            }

            _isRepositioning = true;

            float timeElapsed = 0;
            Vector3 startPosition = transform.position;

            while (timeElapsed < duration)
            {
                if (_interact != null)
                {
                    _interact.enabled = false;
                }

                transform.position = Vector3.Lerp(startPosition, position, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = position;

            _isRepositioning = false;

            _isMoving = true;

            if (_interactAbility != null)
            {
                _interactAbility.enabled = buttonAbilityPreviousStatus;
                _countdownToDeployActiveAbility.Resume();
            }

            if (_interact != null)
            {
                _interact.enabled = true;
            }

            AnimationReposition(CurrentSpeed, false);

        }

        /// <summary>
        /// Function that is used to add a modifier to the player.
        /// First it calls for the dispose of the previous modifier and it clears the list,
        /// after that it adds the new modfier to the player.
        /// </summary>
        /// <param name="modifier"></param>
        public void AddModifier(ModifierDefinition modifier)
        {
            foreach (ModifierDefinition mod in _modifiers)
            {
                mod.Dispose(this);
            }

            _modifiers.Clear();
            _modifiers.Add(modifier);
        }

        /// <summary>
        /// This function is called from the matchManager at it clears all the modifiers of 
        /// the player when the set is restarted. 
        /// </summary>
        public void RemoveAllModifiers()
        {
            foreach(ModifierDefinition modifier in _modifiers)
            {
                modifier.Dispose(this);
            }

            _modifiers.Clear();
        }

        /// <summary>
        /// Function that is called when the UI Button for the interact is pressed.
        /// This checks for the player who has the highlight immage on and select him as the target. 
        /// </summary>
        public void OnInteractPress()
        {

            if (MatchManager.Instance.Phase == MatchPhase.InGame)
            {
                if (CanGetBall)
                {
                    if (_team.Phase == GamePhase.Attack)
                    {
                        if (HasBall)
                        {
                            Player receiver = _team.Players.Find(x => x._highlight.IsActive());

                            if (receiver != null)
                            {
                                SoundManager.Instance.PlayEffect(_sfxClipPassBall);

                                _isInCoolDown = true;

                                HasBall = false;
                                _soccerBall.transform.parent = null;

                                StartCoroutine(CheckingForCooldown());

                                Vector3 targetSpot = receiver._ballSpot.transform.position;

                                if (Vector3.Dot(transform.forward, Vector3.forward) >= 0)
                                {
                                    targetSpot = new Vector3(targetSpot.x,
                                                             targetSpot.y,
                                                             targetSpot.z + MatchManager.Instance.PassOffset);
                                }
                                else
                                {
                                    targetSpot = new Vector3(targetSpot.x,
                                                             targetSpot.y,
                                                             targetSpot.z - MatchManager.Instance.PassOffset);

                                }

                                _soccerBall.ShootTowards(targetSpot, _kickPower);
                                _animator.SetTrigger("Pass");

                            }
                        }
                    }
                    else if (_team.Phase == GamePhase.Defense || _team.Phase == GamePhase.Scramble)
                    {
                        if (MatchManager.Instance.CanGetBall(this, CurrentActionRadius))
                        {
                            _team.SideOut(true, _team.Players.IndexOf(this));
                            _opposingTeam.SideOut(false);

                            _soccerBall.BallStop(this._ballSpot);

                            _opposingTeam.TeamLostBall(this, MatchManager.Instance.AreaRepositioning,MatchManager.Instance.RepositionDistance);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function activates the highlight immage
        /// </summary>
        /// <param name="active"></param>
        public void CanReceiveBall(bool active)
        {
            _highlight.gameObject.SetActive(active);
        }
        #endregion

        #region private methods

        /// <summary>
        /// Function that defines the Behaviour when the player is in the Attack phase. 
        /// </summary>
        private void AttackBehaviour()
        {
            if (CanUpdateDirection)
            {
                //Rotate the player towards  the correct direction.
                if (_team.SideField == TeamSideField.Top)
                {                
                    transform.LookAt(transform.position + (-Vector3.forward));
                    _direction = transform.forward;             
                }
                else
                {
                    transform.LookAt(transform.position + Vector3.forward);
                    _direction = transform.forward;   
                }
            }

            if (CanGetBall)
            {
                if (_soccerBall.IsFreeToRolls && !_isInCoolDown)
                {
                    if (MatchManager.Instance.CanGetBall(this, CurrentActionRadius))
                    {
                        HasBall = true;
                        _soccerBall.BallStop(this._ballSpot);

                    }
                }
            }
        }

        /// <summary>
        /// Function that defines the Behaviour when the player is in the Defense phase. 
        /// </summary>
        private void DefenseBehaviour()
        {
            if (CanUpdateDirection)
            {
                Vector3 ballPos = new Vector3(_soccerBall.transform.position.x,
                                              0,
                                              _soccerBall.transform.position.z);

                Vector3 playerPos = new Vector3(transform.position.x,
                                                0,
                                                transform.position.z);

                //Rotate the player towards the ball every frame
                transform.LookAt(ballPos);
                _direction = transform.forward;
            }
        }


        /// <summary>
        /// Function that defines the Behaviour when the player is in the Scramble phase. 
        /// After a missed pass if the ball stopped moving this funciotn will be called.
        /// If the player is in attack as soon as he is in range of the ball he will get it back.
        /// If the player is in defense he will need to press the interact button to get the ball back.
        /// </summary>
        private void ScrambleBehaviour()
        {
            if (CanUpdateDirection)
            {
                Vector3 ballPos = new Vector3(_soccerBall.transform.position.x,
                                  0,
                                  _soccerBall.transform.position.z);

                Vector3 playerPos = new Vector3(transform.position.x,
                                                0,
                                                transform.position.z);

                transform.LookAt(ballPos);
                _direction = transform.forward;

            }


            if (CanGetBall && _team.PreviousPhase == GamePhase.Attack)
            {
                if (_soccerBall.IsFreeToRolls && !_isInCoolDown)
                {
                    if (MatchManager.Instance.CanGetBall(this, CurrentActionRadius))
                    {
                        HasBall = true;
                        _soccerBall.BallStop(this._ballSpot);

                        _team.SideOut(true, _team.Players.IndexOf(this));
                        _opposingTeam.SideOut(false);
                    }
                }
            }
        }

        /// <summary>
        /// This function searches every frame if the modifier isOver, if it is this will remove the modifier from the player
        /// </summary>
        private void ClearExpiredModifiers()
        {
            foreach (ModifierDefinition modifier in _modifiers)
            {
                if (modifier.IsOver)
                {
                    modifier.Dispose(this);
                }
            }
            
            _modifiers = _modifiers.Except<ModifierDefinition>(_modifiers.Where<ModifierDefinition>(x => x.IsOver)).ToList<ModifierDefinition>();
        }

        /// <summary>
        /// Coroutine that checks if the interact button is in cooldown, if it isn't it will unlock the interact button.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckingForCooldown()
        {
            while (MatchManager.Instance.CanGetBall(this, CurrentActionRadius))
            {
                yield return null;
            }

            _isInCoolDown = false;
        }
        #endregion

        #region animator functions

        /// <summary>
        /// Function that manages the states of the move animations
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="isMoving"></param>
        private void AnimationMove(float speed, bool isMoving)
        {
            _animator.SetBool("IsMoving", isMoving);
            _animator.SetFloat("Speed", speed);
        }

        /// <summary>
        /// Function that manages the states of the reposition animations
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="isRepositioning"></param>
        private void AnimationReposition(float speed, bool isRepositioning)
        {
            _animator.SetBool("IsRepositioning", isRepositioning);
            _animator.SetFloat("Speed", speed);
        }
        #endregion
    }
}
