using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.AI
{
    public class StopState : AIStateBase
    {
        private float time = 0;
        public float waitTime = 2f;

        EnemyUnit enemy;

        [HideInInspector]
        public GameObject questionMark;

        public StopState(GameObject owner)
            : base() //owner, AIStateType.Stop)
        {
            State = AIStateType.Stop;

            AddTransition( AIStateType.Patrol );
            AddTransition( AIStateType.FollowTarget );

            if (Owner == null)
            {
                Owner = owner;
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            questionMark = GameObject.Find("QuestionMark");
        }

        public override void Update()
        {
            if (!ChangeState())
            {
                if (time < waitTime)
                {
                    time += Time.deltaTime;
                }
            }
        }

        private bool ChangeState()
        {
            // 2. Did the player get away?
            // If yes, go to patrol state.
            if (time >= waitTime)
            {
                time = 0;
                enemy.agent.speed = 0;
                enemy.speed = 0;
                enemy.transform.rotation = Quaternion.identity;
                Debug.Log("Going back to patrolling!");
                HideQuestionMark();
                return enemy.PerformTransition(AIStateType.Patrol);
            }

            if (enemy.playerVisibleTimer >= 0.5f &&
                enemy.playerVisibleTimer <= 0.99f ||
                    !enemy.Target.GetComponent<Invector.CharacterController.vThirdPersonController>().isCrouching &&
                        Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance
                            || Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance / 1.5f)
            {
                //enemy.SetOwnLastKnownPosition();
                enemy.hasBeenNoticed = true;
                enemy.time = 0;
                Debug.Log("Noticed player!");
                HideQuestionMark();
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

        public void ShowQuestionMark()
        {
            for (int i = 0; i < questionMark.transform.childCount; i++)
            {
                questionMark.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        public void HideQuestionMark()
        {
            for (int i = 0; i < questionMark.transform.childCount; i++)
            {
                questionMark.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
