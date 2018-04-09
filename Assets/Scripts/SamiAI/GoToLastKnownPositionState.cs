using UnityEngine;
using System.Collections;

namespace Invector.AI
{
	public class GoToLastKnownPositionState : AIStateBase
	{
        //public float SqrShootingDistance
        //{
        //	get { return Owner.ShootingDistance * Owner.ShootingDistance; }
        //}

        //public float SqrDetectEnemyDistance
        //{
        //	get { return Owner.DetectEnemyDistance * Owner.DetectEnemyDistance; }
        //}

        public bool moveAgain;

        public float waitTime = 3;

        //public Invector.CharacterController.EnemyUnit enemyUnit;

		public GoToLastKnownPositionState( GameObject owner )
			: base( owner, AIStateType.GoToLastKnownPosition )
		{
            AddTransition( AIStateType.FollowTarget );
            AddTransition( AIStateType.Patrol );
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
                //Owner.Mover.Move( Owner.transform.forward );
                //Owner.Mover.Turn( Owner.Target.transform.position );
                Vector3 tempPlayerPosition = new Vector3(Owner.GetComponent<EnemyUnit>().lastPositionOfPlayer.x, 
                    0, Owner.GetComponent<EnemyUnit>().lastPositionOfPlayer.z);

                Owner.GetComponent<EnemyUnit>().transform.position = Vector3.MoveTowards(Owner.GetComponent<EnemyUnit>().transform.position,
                    tempPlayerPosition, 
                        Owner.GetComponent<EnemyUnit>().speed * Time.deltaTime);
                //Owner.GetComponent<EnemyUnit>().transform.position = new Vector3(Owner.GetComponent<EnemyUnit>().transform.position.x,
                //     1.1f, Owner.GetComponent<EnemyUnit>().transform.position.z);
                //Owner.GetComponent<EnemyUnit>().transform.LookAt(Owner.GetComponent<EnemyUnit>().lastPositionOfPlayer);

                Owner.GetComponent<EnemyUnit>().StartCoroutine(TurnToFace(Owner.GetComponent<EnemyUnit>().lastPositionOfPlayer));
                Owner.GetComponent<EnemyUnit>().StartCoroutine(Wait());
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
			if ( moveAgain )
			{
				//Owner.GetComponent<EnemyUnit>().Target = null;
                Owner.GetComponent<EnemyUnit>().StopAllCoroutines();
                moveAgain = false;
                return Owner.GetComponent<EnemyUnit>().PerformTransition( AIStateType.Patrol );
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

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(waitTime);
            yield return Owner.GetComponent<EnemyUnit>().StartCoroutine(MoveAgain());
        }

        IEnumerator MoveAgain()
        {
            Debug.Log("Going Back To Patrolling");
            moveAgain = true;
            yield return null;
        }
    }
}
