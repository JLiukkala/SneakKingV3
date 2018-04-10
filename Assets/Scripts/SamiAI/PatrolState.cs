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

		public Waypoint CurrentWaypoint { get; private set; }

		public PatrolState( GameObject owner, Path path,
			Direction direction, float arriveDistance )
			: base()
		{
			State = AIStateType.Patrol;

            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            AddTransition( AIStateType.FollowTarget );
            AddTransition( AIStateType.GoToLastKnownPosition );
            _path = path;
			_direction = direction;
			_arriveDistance = arriveDistance;
		}

		public override void StateActivated()
		{
			base.StateActivated();
			CurrentWaypoint = _path.GetClosestWaypoint( Owner.transform.position );
		}

		public override void Update()
		{
			// 1. Should we change the state?
			//   1.1 If yes, change state and return.

			if ( !ChangeState() )
			{
                // 2. Are we close enough the current waypoint?
                //   2.1 If yes, get the next waypoint
                CurrentWaypoint = GetWaypoint();
                // 3. Move towards the current waypoint
                enemy.agent.speed = 0.2f;
                enemy.agent.angularSpeed = 10;
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position,
                        CurrentWaypoint.Position, enemy.speed * Time.deltaTime);
                enemy.agent.SetDestination(CurrentWaypoint.Position);
                // 4. Rotate towards the current waypoint
                enemy.StartCoroutine(TurnToFace(CurrentWaypoint.Position));
                //enemy.transform.LookAt(CurrentWaypoint.Position);
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
				result = _path.GetNextWaypoint( CurrentWaypoint, ref _direction );
			}

			return result;
		}

		private bool ChangeState()
		{
            if(enemy.playerVisibleTimer >= 0.5f && 
                enemy.playerVisibleTimer <= 0.99f ||
                    !enemy.Target.GetComponent<Invector.CharacterController.vThirdPersonController>().isCrouching &&
                        Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance)
            {
                //enemy.SetLastKnownPosition();
                enemy.StopAllCoroutines();
                enemy.hasBeenNoticed = true;
                return enemy.PerformTransition(AIStateType.FollowTarget);
            }

            return false;
		}

        IEnumerator TurnToFace(Vector3 lookTarget)
        {
            Vector3 directionToLookTarget = (lookTarget - enemy.transform.position).normalized;
            float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z,
                directionToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(enemy.transform.eulerAngles.y, targetAngle)) > 0.09f)
            {
                float angle = Mathf.MoveTowardsAngle(enemy.transform.eulerAngles.y, targetAngle,
                    enemy.turnSpeed * Time.deltaTime);

                enemy.transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }
    }
}
