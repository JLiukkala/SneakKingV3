using UnityEngine;
using System.Collections;

namespace Invector.AI
{
	public class GoToLastKnownPositionState : AIStateBase
	{
        private float time = 0;
        public float waitTime = 2f;

        EnemyUnit enemy;

		public GoToLastKnownPositionState( GameObject owner )
			: base( owner, AIStateType.GoToLastKnownPosition )
		{
            State = AIStateType.GoToLastKnownPosition;

            AddTransition( AIStateType.Stop );
            AddTransition( AIStateType.FollowTarget );

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
                enemy.speed = 0.14f;

                if (time < waitTime)
                {
                    time += Time.deltaTime;
                    enemy.agent.SetDestination(enemy.lastPositionOfPlayer);
                }
            }
		}

        private bool ChangeState()
		{
            // 2. Did the player get away?
            // If yes, go to stop state.
            if (time >= waitTime)
            {
                enemy.goToAlertMode = true;
                time = 0;
                return enemy.PerformTransition(AIStateType.Stop);
            }

            if (enemy.playerVisibleTimer >= 0.5f &&
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
