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

		public Waypoint CurrentWaypoint { get; private set; }

		public PatrolState( GameObject owner, Path path,
			Direction direction, float arriveDistance )
			: base()
		{
			State = AIStateType.Patrol;

            //Owner = GameObject.Find("EnemyMoving");
            //if (GameObject.Find("EnemyMoving"))
            //{
            //    Owner = GameObject.Find("EnemyMoving");
            //}
            //if (GameObject.Find("EnemyNotMoving"))
            //{
            //    Owner = GameObject.Find("EnemyNotMoving");
            //}
            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

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
                Owner.GetComponent<EnemyUnit>().agent.speed = 0.2f;
                Owner.GetComponent<EnemyUnit>().agent.angularSpeed = 10;
                Owner.GetComponent<EnemyUnit>().transform.position = 
                    Vector3.MoveTowards(Owner.GetComponent<EnemyUnit>().transform.position, 
                        CurrentWaypoint.Position,
                            Owner.GetComponent<EnemyUnit>().speed * Time.deltaTime);
                Owner.GetComponent<EnemyUnit>().agent.SetDestination(CurrentWaypoint.Position);
                // 4. Rotate towards the current waypoint
                Owner.GetComponent<EnemyUnit>().StartCoroutine(TurnToFace(CurrentWaypoint.Position));
                //Owner.GetComponent<EnemyUnit>().transform.LookAt(CurrentWaypoint.Position);
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
            if(Owner.GetComponent<EnemyUnit>().playerVisibleTimer >= 0.5f && 
                Owner.GetComponent<EnemyUnit>().playerVisibleTimer <= 0.99f ||
                    !Owner.GetComponent<EnemyUnit>().Target.GetComponent<Invector.CharacterController.vThirdPersonController>().isCrouching &&
                        Vector3.Distance(Owner.transform.position, Owner.GetComponent<EnemyUnit>().Target.position) < Owner.GetComponent<EnemyUnit>().hearDistance)
            {
                Owner.GetComponent<EnemyUnit>().SetLastKnownPosition();
                Owner.GetComponent<EnemyUnit>().StopAllCoroutines();
                Owner.GetComponent<EnemyUnit>().hasBeenNoticed = true;
                return Owner.GetComponent<EnemyUnit>().PerformTransition(AIStateType.FollowTarget);
            }

            return false;
		}

        IEnumerator TurnToFace(Vector3 lookTarget)
        {
            Vector3 directionToLookTarget = (lookTarget - Owner.GetComponent<EnemyUnit>().transform.position).normalized;
            float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z,
                directionToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(Owner.GetComponent<EnemyUnit>().transform.eulerAngles.y, targetAngle)) > 0.05f)
            {
                float angle = Mathf.MoveTowardsAngle(Owner.GetComponent<EnemyUnit>().transform.eulerAngles.y, targetAngle,
                    Owner.GetComponent<EnemyUnit>().turnSpeed * Time.deltaTime);

                Owner.GetComponent<EnemyUnit>().transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }
    }
}
