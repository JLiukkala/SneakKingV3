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

        public float hearDistance;

        private float viewAngle;
        [HideInInspector]
        public float playerVisibleTimer;

        [HideInInspector]
        public Vector3 lastPositionOfPlayer;

        [HideInInspector]
        public Vector3 lastPositionOfEnemy;

        Color originalVisionConeColor;

        //bool almostSpotted;

        [HideInInspector]
        public bool hasBeenNoticed;

        [HideInInspector]
        public bool goToAlertMode = false;

        [HideInInspector]
        public bool heardNoise = false;

        public bool isStandingStill;

        public bool hasNoiseArea;

        [HideInInspector]
        public Transform noiseArea;
        
        [HideInInspector]
        public float time;

        [HideInInspector]
        public float waitTime = 6;

        public NavMeshAgent agent;

        private IList<AIStateBase> _states = new List< AIStateBase >();

		public AIStateBase CurrentState { get; private set; }
		// The player unit this enemy is trying to shoot at.
		public Transform Target { get; set; }

        private Animator _docAnimator;
		
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

		public void Start()
		{
            if ( _docAnimator == null)
            {
                _docAnimator = GetComponent<Animator>();
            }
            
            Target = GameObject.FindGameObjectWithTag("Player").transform;

            viewAngle = visionCone.spotAngle;
            originalVisionConeColor = visionCone.color;

            if (hasNoiseArea)
            {
                noiseArea = GameObject.Find("NoiseArea").transform;
            }

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

        private void InitStates()
		{
            PatrolState patrol = new PatrolState(GameObject.FindGameObjectWithTag("Enemy"), _path, _direction, _waypointArriveDistance);
            _states.Add(patrol);

            FollowTargetState followTarget = new FollowTargetState(GameObject.FindGameObjectWithTag("Enemy"));
            _states.Add(followTarget);

            GoToLastKnownPositionState goToLastKnownPosition = new GoToLastKnownPositionState(GameObject.FindGameObjectWithTag("Enemy"));
            _states.Add(goToLastKnownPosition);

            GoToNoiseArea goToNoiseArea = new GoToNoiseArea(GameObject.FindGameObjectWithTag("Enemy"));
            _states.Add(goToNoiseArea);

            StopState stopState = new StopState(GameObject.FindGameObjectWithTag("Enemy"));
            _states.Add(stopState);


            CurrentState = patrol;
			CurrentState.StateActivated();
		}

		protected void Update()
		{
			gameObject.GetComponent<EnemyUnit>().CurrentState.Update();
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

		private AIStateBase GetStateByType( AIStateType stateType )
		{
			// Returns the first object from the list _states which State property's value
			// equals to stateType. If no object is found, returns null.
			return _states.FirstOrDefault( state => state.State == stateType );
		}
	}
}
