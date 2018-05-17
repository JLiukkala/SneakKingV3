using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Invector.AI;
using Invector.WaypointSystem;

namespace Invector
{
	public class EnemyUnit : MonoBehaviour
	{
        #region Variables
        [SerializeField]
		private Path _path;

		[SerializeField]
		private float _waypointArriveDistance;

		[SerializeField]
		private Direction _direction;

        public float speed = 0.14f;

        public float turnSpeed = 90;

        public float timeToSpotPlayer = 1;

        public Light visionCone;
        public float viewDistance;
        public LayerMask viewMask;

        [HideInInspector]
        public float stopDistance;

        public float hearDistance;

        private float viewAngle;
        [HideInInspector]
        public float playerVisibleTimer;

        [HideInInspector]
        public Vector3 lastPositionOfPlayer;

        [HideInInspector]
        public Vector3 lastPositionOfEnemy;

        Color originalVisionConeColor;

        [HideInInspector]
        public bool hasBeenNoticed;

        [HideInInspector]
        public bool goToAlertMode = false;

        //[HideInInspector]
        //public bool heardNoise = false;

        public bool isStandingStill;

        public bool roomHasNoiseArea;

        [HideInInspector]
        public bool inCameraView;

        public bool isRoomTwo;

        public bool isRoomThree;

        public bool isRoomEight;

        [HideInInspector]
        public Transform noiseArea;
        
        [HideInInspector]
        public float time = 0;

        [HideInInspector]
        public float waitTime = 2f;

        public NavMeshAgent agent;

        [HideInInspector]
        public bool turningDone = false;

        [HideInInspector]
        public bool gotUp = false;

        [HideInInspector]
        public EnemyUnit enemyScript;

        private IList<AIStateBase> _states = new List< AIStateBase >();

		public AIStateBase CurrentState { get; private set; }

		public Transform Target { get; set; }

        [HideInInspector]
        public Animator _docAnimator;
		
		public Vector3? ToTargetVector
		{
			get
			{
				if ( Target != null )
				{
					return Target.transform.position - transform.position;
				}
				return null;
			}
		}
        #endregion

        public void Start()
		{
            if ( _docAnimator == null)
            {
                _docAnimator = GetComponent<Animator>();
            }
            
            Target = GameObject.FindGameObjectWithTag("Player").transform;

            viewAngle = visionCone.spotAngle;
            originalVisionConeColor = visionCone.color;

            if (roomHasNoiseArea)
            {
                noiseArea = GameObject.Find("NoiseArea").transform;
            }

            stopDistance = viewDistance / 3.2f;

            enemyScript = GetComponent<EnemyUnit>();

            // Initializes the state system.
            InitStates();
		}

        bool CanSeePlayer()
        {
            // If the distance between the enemy and the player is less than
            // the view distance that has been set, the angle between them is
            // less than the view angle divided by 2 and the player's collider
            // is recognized by the enemy, the enemy is able to see the player.
            if (Vector3.Distance(visionCone.transform.position, Target.position) < viewDistance)
            {
                Vector3 dirToPlayer = (Target.position - visionCone.transform.position).normalized;
                float angleBetweenEnemyAndPlayer = Vector3.Angle(visionCone.transform.forward, dirToPlayer);

                if (angleBetweenEnemyAndPlayer < viewAngle / 2)
                {
                    if (!Physics.Linecast(visionCone.transform.position, Target.position, viewMask))
                    {
                        //Debug.Log("CanSeePlayer");
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetLastKnownPosition()
        {
            lastPositionOfPlayer = Target.position;
        }

        public void SetOwnLastKnownPosition()
        {
            lastPositionOfEnemy = transform.position;
        }

        /// <summary>
		/// Initiates all of the states and also sets the current state.
        /// A part of Sami's AI system for Game Programming 2.
		/// </summary>
        private void InitStates()
		{
            PatrolState patrol = new PatrolState(this.gameObject, _path, _direction, _waypointArriveDistance);
            _states.Add(patrol);

            FollowTargetState followTarget = new FollowTargetState(this.gameObject);
            _states.Add(followTarget);

            GoToLastKnownPositionState goToLastKnownPosition = new GoToLastKnownPositionState(this.gameObject);
            _states.Add(goToLastKnownPosition);

            GoToNoiseArea goToNoiseArea = new GoToNoiseArea(this.gameObject);
            _states.Add(goToNoiseArea);

            StopState stopState = new StopState(this.gameObject);
            _states.Add(stopState);

            StandingStillState standingStill = new StandingStillState(this.gameObject, _path, _direction, _waypointArriveDistance);
            _states.Add(standingStill);

            if (isStandingStill)
            {
                CurrentState = standingStill;
            }
            else
            {
                CurrentState = patrol;
            }

            CurrentState.StateActivated();
		}

		protected void Update()
		{
			enemyScript.CurrentState.Update();
            if (_docAnimator!=null)
            {
                _docAnimator.SetFloat("Speed", speed);
            }
            // If the enemy sees the player while the player
            // visible timer is less than the time to spot the player,
            // playerVisibleTimer is increased by Time.deltaTime.
            if (CanSeePlayer() && playerVisibleTimer < timeToSpotPlayer)
            {
                playerVisibleTimer += Time.deltaTime;
            }
            // Otherwise, playerVisibleTimer is decresed by Time.deltaTime.
            else if (!CanSeePlayer() && playerVisibleTimer > 0)
            {
                playerVisibleTimer -= Time.deltaTime;
            }

            // Clamps the value of the playerVisibleTimer variable 
            // to be between zero and the timeToSpotPlayer variable.
            playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

            // The enemy's vision cone lerps towards red 
            // if the player is visible to the enemy.
            visionCone.color = Color.Lerp(originalVisionConeColor, Color.red,
                playerVisibleTimer / timeToSpotPlayer);

            // If the playerVisibleTimer reaches the value of the timeToSpotPlayer
            // variable, the event OnEnemyHasSpottedPlayer is called.
            if (playerVisibleTimer >= timeToSpotPlayer)
            {
                GameUI._losingStatement = true;
            }
        }

        /// <summary>
		/// Performs the transition from the current state to the target state.
        /// A part of Sami's AI system for Game Programming 2.
		/// </summary>
		/// <param name="targetState">The target state to go to.</param>
		/// <returns>True, if the transition is completed, false otherwise.</returns>
		public bool PerformTransition( AIStateType targetState )
		{
			if ( !CurrentState.CheckTransition( targetState ) )
			{
				return false;
			}

			bool result = false;

			AIStateBase state = GetStateByType( targetState );
			if ( state != null )
			{
				CurrentState.StateDeactivating();
				CurrentState = state;
				CurrentState.StateActivated();
				result = true;
			}

			return result;
		}

        /// <summary>
		/// Gets the correct state needed to perform the transition.
        /// A part of Sami's AI system for Game Programming 2.
		/// </summary>
		/// <param name="targetState">The target state to match the State property with.</param>
		/// <returns>Returns the state type.</returns>
		private AIStateBase GetStateByType( AIStateType stateType )
		{
			// Returns the first object from the list _states which State property's value
			// equals to stateType. If no object is found, returns null.
			return _states.FirstOrDefault( state => state.State == stateType );
		}
    }
}
