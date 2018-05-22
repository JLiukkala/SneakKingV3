using Invector.WaypointSystem;
using UnityEngine;
using System.Collections;

namespace Invector.AI
{
	public class PatrolState : AIStateBase
	{
        #region Variables
        private Path _path;
		private Direction _direction;
		private float _arriveDistance;

        EnemyUnit enemy;

        Invector.CharacterController.vThirdPersonController cc;

		public Waypoint CurrentWaypoint { get; private set; }
        #endregion

        public PatrolState( GameObject owner, Path path,
			Direction direction, float arriveDistance )
			: base()
		{
			State = AIStateType.Patrol;

            if (Owner == null)
            {
                Owner = owner;
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            cc = enemy.Target.GetComponent<Invector.CharacterController.vThirdPersonController>();

            AddTransition( AIStateType.FollowTarget );
            AddTransition( AIStateType.GoToLastKnownPosition );
            AddTransition( AIStateType.GoToNoiseArea );

            _path = path;
			_direction = direction;
			_arriveDistance = arriveDistance;
		}

		public override void StateActivated()
		{
			base.StateActivated();
            // Sets the closest waypoint for the enemy to be the current waypoint.
            CurrentWaypoint = _path.GetClosestWaypoint(Owner.transform.position);
		}

		public override void Update()
		{
			// If ChangeState returns true, the state changes.
			if ( !ChangeState() )
			{
                // These are related to the second room in the game. If the waitTime reaches 
                // a certain value, the hear distance for the enemy is brought back up.
                if (enemy.isRoomTwo)
                {
                    enemy.time += Time.deltaTime;
                }

                if (enemy.isRoomTwo && enemy.time >= enemy.waitTime / 4)
                {
                    enemy.hearDistance = 8;
                }
                else
                {
                    enemy.hearDistance = 1;
                }

                // If close enough to the current waypoint, get the next waypoint.
                CurrentWaypoint = GetWaypoint();

                // After going back to the patrol state, the enemy is 
                // more easily alerted to the movement of the player.
                if (enemy.goToAlertMode)
                {
                    enemy.hearDistance = 10;
                    enemy.viewDistance = 12;
                }
                else if (enemy.isRoomTwo)
                {
                    enemy.viewDistance = 10;
                }
                else
                {
                    enemy.hearDistance = 8;
                    enemy.viewDistance = 10;
                }

                enemy.speed = 0.14f;
                enemy.agent.speed = 0.4f;

                // Move and rotate towards the current waypoint.
                enemy.agent.SetDestination(CurrentWaypoint.Position);
            }
		}

        /// <summary>
		/// Gets the next waypoint on the enemy's path.
        /// A part of Sami's waypoint system for Game Programming 2.
		/// </summary>
        /// <returns>A waypoint position.</returns>
		private Waypoint GetWaypoint()
		{
			Waypoint result = CurrentWaypoint;
			Vector3 toWaypointVector = CurrentWaypoint.Position - Owner.transform.position;
			float toWaypointSqr = toWaypointVector.sqrMagnitude;
			float sqrArriveDistance = _arriveDistance * _arriveDistance;

            if ( toWaypointSqr <= sqrArriveDistance )
			{
                result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
			}

			return result;
		}

		private bool ChangeState()
		{
            // If the player is seen by the enemy for enough time, or the player is not crouching
            // while being within the distance of the enemy's hear distance, or the player is too 
            // close to the enemy's position, the state is changed to FollowTarget.
            if(enemy.playerVisibleTimer >= 0.5f && enemy.playerVisibleTimer <= 0.99f ||
                    !cc.isCrouching && Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance
                            || Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance / 1.5f)
            {
                enemy.SetOwnLastKnownPosition();
                enemy.hasBeenNoticed = true;
                enemy.time = 0;
                Debug.Log("Noticed player!");
                return enemy.PerformTransition(AIStateType.FollowTarget);
            }

            // If the enemy hears noise provided by a noise area in the scene,
            // the state is changed to the GoToNoiseArea state.
            if (NoiseArea.heardNoise)
            {
                enemy.time = 0;
                Debug.Log("Heard something!");
                return enemy.PerformTransition(AIStateType.GoToNoiseArea);
            }

            // If the player is seen by a camera, the enemy goes to the 
            // last known position of the player (GoToLastKnownPosition state).
            if (enemy.inCameraView)
            {
                enemy.time = 0;
                Debug.Log("Seen by camera!");
                return enemy.PerformTransition(AIStateType.GoToLastKnownPosition);
            }

            // Otherwise returns false.
            return false;
		}
    }
}
