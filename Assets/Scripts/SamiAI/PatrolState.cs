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

        Vector3 currentWaypointWithoutY;

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
            AddTransition( AIStateType.GoToNoiseArea );
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
                if (enemy.isStandingStill)
                {
                    enemy.time += Time.deltaTime;
                }

                // 2. Are we close enough the current waypoint?
                //   2.1 If yes, get the next waypoint
                CurrentWaypoint = GetWaypoint();
                // 3. Move towards the current waypoint

                if (enemy.goToAlertMode)
                {
                    enemy.hearDistance = 10;
                    enemy.viewDistance = 12;
                } 
                else
                {
                    enemy.hearDistance = 8;
                    enemy.viewDistance = 10;
                }

                enemy.speed = 0.14f;
                enemy.agent.speed = 0.5f;

                // 3. Move and rotate towards the current waypoint
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
                if (enemy.isStandingStill)
                {
                    if (enemy.time >= enemy.waitTime)
                    {
                        result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
                        enemy.time = 0;
                    }
                }
                else
                {
                    result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
                }
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

            return false;
		}

        //IEnumerator TurnToFace(Vector3 lookTarget)
        //{
        //    Vector3 directionToLookTarget = (lookTarget - enemy.transform.position).normalized;
        //    float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z,
        //        directionToLookTarget.x) * Mathf.Rad2Deg;

        //    while (Mathf.Abs(Mathf.DeltaAngle(enemy.transform.eulerAngles.y, targetAngle)) > 0.09f)
        //    {
        //        float angle = Mathf.MoveTowardsAngle(enemy.transform.eulerAngles.y, targetAngle,
        //            enemy.turnSpeed * Time.deltaTime);

        //        enemy.transform.eulerAngles = Vector3.up * angle;
        //        yield return null;
        //    }
        //}
    }
}
