using Invector.WaypointSystem;
using UnityEngine;
using System.Collections;

namespace Invector.AI
{
	public class PatrolState : AIStateBase
	{
		private Path _path;
		private Direction _direction;
		private float _arriveDistance;

        EnemyUnit enemy;

        Invector.CharacterController.vThirdPersonController cc;

		public Waypoint CurrentWaypoint { get; private set; }

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

            //if (enemy.isRoomTwo)
            //{
            //    enemy.hearDistance = 1;
            //}

            CurrentWaypoint = _path.GetClosestWaypoint(Owner.transform.position);
		}

		public override void Update()
		{
			// If ChangeState returns true, the state changes.
			if ( !ChangeState() )
			{
                if (enemy.isRoomTwo)
                {
                    enemy.time += Time.deltaTime;
                }

                if (enemy.isRoomTwo && enemy.time >= enemy.waitTime / 2)
                {
                    enemy.hearDistance = 8;
                }
                else
                {
                    enemy.hearDistance = 1;
                }

                // If close enough to the current waypoint, get the next waypoint.
                CurrentWaypoint = GetWaypoint();

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

            if (enemy.heardNoise)
            {
                enemy.time = 0;
                Debug.Log("Heard something!");
                return enemy.PerformTransition(AIStateType.GoToNoiseArea);
            }

            if (enemy.inCameraView)
            {
                enemy.time = 0;
                Debug.Log("Seen by camera!");
                return enemy.PerformTransition(AIStateType.GoToLastKnownPosition);
            }

            return false;
		}
    }
}
