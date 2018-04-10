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

		public FollowTargetState( GameObject owner )
			: base( owner, AIStateType.FollowTarget )
		{
			AddTransition( AIStateType.Patrol );
            AddTransition( AIStateType.GoToLastKnownPosition );
        }

        void Start()
        {
            //Owner = GameObject.Find("Enemy");
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
        }

        public override void Update()
		{
			if ( !ChangeState() )
			{
                Vector3 tempPlayerPosition = new Vector3(Owner.GetComponent<EnemyUnit>().Target.position.x, 
                    0, Owner.GetComponent<EnemyUnit>().Target.position.z);

                if (Owner.GetComponent<EnemyUnit>().hasBeenNoticed) 
                {
                    Owner.GetComponent<EnemyUnit>().turnSpeed = 240;
                    Owner.GetComponent<EnemyUnit>().StartCoroutine(TurnToFace(Owner.GetComponent<EnemyUnit>().Target.position));
                    //Owner.GetComponent<EnemyUnit>().transform.LookAt(tempPlayerPosition);
                    Owner.GetComponent<EnemyUnit>().StartCoroutine(WaitShort());
                }

                //Owner.GetComponent<EnemyUnit>().transform.LookAt(tempPlayerPosition);
                //Owner.GetComponent<EnemyUnit>().StartCoroutine(TurnToFace(Owner.GetComponent<EnemyUnit>().Target.position));
                //Owner.GetComponent<EnemyUnit>().StartCoroutine(WaitShort());

                if (moveTowardsPlayer)
                {
                    //Owner.GetComponent<EnemyUnit>().turnSpeed = 60;
                    //Owner.GetComponent<EnemyUnit>().speed = 1.5f;
                    Owner.GetComponent<EnemyUnit>().transform.LookAt(tempPlayerPosition);
                    //Owner.GetComponent<EnemyUnit>().StartCoroutine(TurnToFace(Owner.GetComponent<EnemyUnit>().Target.position));
                    //Owner.GetComponent<EnemyUnit>().transform.position = Vector3.MoveTowards(Owner.GetComponent<EnemyUnit>().transform.position,
                    //    tempPlayerPosition,
                    //        Owner.GetComponent<EnemyUnit>().speed * Time.deltaTime);
                    Owner.GetComponent<EnemyUnit>().agent.speed = 1.5f;
                    Owner.GetComponent<EnemyUnit>().agent.angularSpeed = 60;
                    Owner.GetComponent<EnemyUnit>().agent.SetDestination(tempPlayerPosition);

                    if (Vector3.Distance(Owner.transform.position, Owner.GetComponent<EnemyUnit>().Target.position)
                        > Owner.GetComponent<EnemyUnit>().viewDistance)
                    {
                        gotAway = true;
                        //Owner.GetComponent<EnemyUnit>().speed = 1.5f;
                        moveTowardsPlayer = false;
                    }
                }
            }
		}

        private bool ChangeState()
		{
			// 1. Are we at the shooting range?
			// If yes, go to shoot state.
			//Vector3 toPlayerVector =
			//	Owner.transform.position - Owner.Target.transform.position;
			//float sqrDistanceToPlayer = toPlayerVector.sqrMagnitude;

			// 2. Did the player get away?
			// If yes, go to patrol state.
			//if ( moveAgain )
			//{
			//	//Owner.GetComponent<EnemyUnit>().Target = null;
   //             Owner.GetComponent<EnemyUnit>().StopAllCoroutines();

   //             return Owner.GetComponent<EnemyUnit>().PerformTransition( AIStateType.Patrol );
			//}
            if (gotAway)
            {
                //Owner.GetComponent<EnemyUnit>().StopAllCoroutines();
                Owner.GetComponent<EnemyUnit>().SetLastKnownPosition();
                gotAway = false;
                return Owner.GetComponent<EnemyUnit>().PerformTransition(AIStateType.GoToLastKnownPosition);
            }

			// Otherwise return false.
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

        IEnumerator WaitShort()
        {
            Owner.GetComponent<EnemyUnit>().hasBeenNoticed = false;
            yield return new WaitForSeconds(waitTime);
            yield return Owner.GetComponent<EnemyUnit>().StartCoroutine(MoveTowardsPlayer());
        }

        IEnumerator MoveTowardsPlayer()
        {
            Debug.Log("Moving Towards Player After Realization");
            moveTowardsPlayer = true;
            yield return null;
        }
    }
}
