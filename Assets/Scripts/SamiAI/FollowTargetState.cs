using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Invector.AI
{
	public class FollowTargetState : AIStateBase
	{
        public bool moveTowardsPlayer;

        public bool gotAway;

        public float waitTime = 0.5f;

        EnemyUnit enemy;

		public FollowTargetState( GameObject owner )
			: base( owner, AIStateType.FollowTarget )
		{
			AddTransition( AIStateType.Patrol );
            AddTransition( AIStateType.GoToLastKnownPosition );

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
                Vector3 tempPlayerPosition = new Vector3(enemy.Target.position.x, 
                    0, enemy.Target.position.z);

                if (enemy.hasBeenNoticed) 
                {
                    enemy.turnSpeed = 240;
                    enemy.StartCoroutine(TurnToFace(enemy.Target.position));
                    //enemy.transform.LookAt(tempPlayerPosition);
                    enemy.StartCoroutine(WaitShort());
                }

                //enemy.transform.LookAt(tempPlayerPosition);
                //enemy.StartCoroutine(TurnToFace(Owner.GetComponent<EnemyUnit>().Target.position));
                //enemy.StartCoroutine(WaitShort());

                if (moveTowardsPlayer)
                {
                    enemy.transform.LookAt(tempPlayerPosition);
                    //enemy.StartCoroutine(TurnToFace(enemy.Target.position));
                    //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position,
                    //    tempPlayerPosition, enemy.speed * Time.deltaTime);
                    //enemy.turnSpeed = 60;
                    enemy.agent.speed = 1.5f;
                    enemy.agent.angularSpeed = 60;
                    enemy.agent.SetDestination(tempPlayerPosition);

                    if (Vector3.Distance(Owner.transform.position, enemy.Target.position)
                        > enemy.viewDistance)
                    {
                        gotAway = true;
                        moveTowardsPlayer = false;
                    }
                }
            }
		}

        private bool ChangeState()
		{
			// 2. Did the player get away?
			// If yes, go to patrol state.
            if (gotAway)
            {
                enemy.StopAllCoroutines();
                enemy.SetLastKnownPosition();
                gotAway = false;
                return enemy.PerformTransition(AIStateType.GoToLastKnownPosition);
            }

			// Otherwise return false.
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

        IEnumerator WaitShort()
        {
            enemy.hasBeenNoticed = false;
            yield return new WaitForSeconds(waitTime);
            yield return enemy.StartCoroutine(MoveTowardsPlayer());
        }

        IEnumerator MoveTowardsPlayer()
        {
            Debug.Log("Moving Towards Player After Realization");
            moveTowardsPlayer = true;
            yield return null;
        }
    }
}
