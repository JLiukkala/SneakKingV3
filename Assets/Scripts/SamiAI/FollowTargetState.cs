using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Invector.AI
{
	public class FollowTargetState : AIStateBase
	{
        private float time = 0;
        public float waitTime = 0.3f;

        EnemyUnit enemy;

		public FollowTargetState( GameObject owner )
			: base( owner, AIStateType.FollowTarget )
		{
            State = AIStateType.FollowTarget;

            AddTransition( AIStateType.Patrol );
            AddTransition( AIStateType.GoToLastKnownPosition );
            AddTransition( AIStateType.GoToNoiseArea );

            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

            enemy = Owner.GetComponent<EnemyUnit>();
        }

        public override void Update()
		{
			if ( !ChangeState() )
			{
                if (time < waitTime) 
                {
                    time += Time.deltaTime;
                }

                if (time >= waitTime)
                {
                    time = 0;
                    enemy.hasBeenNoticed = false;
                    enemy.speed = 0.15f;

                    enemy.agent.SetDestination(enemy.Target.position);

                    //if (Vector3.Distance(Owner.transform.position, enemy.Target.position)
                    //    > enemy.viewDistance / 8)
                    //{
                    //    enemy.speed = 0;
                    //    enemy.agent.speed = 0;
                    //}
                    //else
                    //{
                    //    enemy.speed = 0.15f;
                    //    enemy.agent.speed = 0.5f;
                    //    enemy.agent.SetDestination(enemy.Target.position);
                    //}
                }
            }
		}

        private bool ChangeState()
		{
			// 2. Did the player get away?
			// If yes, go to last known position state.
            if (Vector3.Distance(Owner.transform.position, enemy.Target.position)
                        > enemy.viewDistance)
            {
                enemy.SetLastKnownPosition();
                Debug.Log("Going to last known position!");
                return enemy.PerformTransition(AIStateType.GoToLastKnownPosition);
            }

            // Otherwise return false.
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
