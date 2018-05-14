using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Invector.AI
{
    public class StopState : AIStateBase
    {
        private float time = 0;
        public float waitTime = 1.5f;

        EnemyUnit enemy;

        [HideInInspector]
        public GameObject questionMark;

        Transform position1;
        //Transform position2;

        Invector.CharacterController.vThirdPersonController cc;

        public StopState(GameObject owner)
            : base()
        {
            State = AIStateType.Stop;

            AddTransition( AIStateType.Patrol );
            AddTransition( AIStateType.FollowTarget );

            if (Owner == null)
            {
                Owner = owner;
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            cc = enemy.Target.GetComponent<Invector.CharacterController.vThirdPersonController>();

            questionMark = GameObject.Find("QuestionMark");

            if (enemy.isRoomTwo)
            {
                position1 = GameObject.Find("Position1").transform;
                //position2 = GameObject.Find("Position2").transform;
            }
        }

        public override void Update()
        {
            if (!ChangeState())
            {
                if (time < waitTime)
                {
                    time += Time.deltaTime;
                }

                if (enemy.isRoomTwo)
                {
                    enemy.speed = 0.001f;
                    enemy.agent.speed = 0.01f;
                }

                if (enemy.isRoomTwo && !enemy.turningDone)
                {
                    //enemy.StartCoroutine(TurnToFace(position1.position));
                    waitTime = 1.5f;
                    enemy.transform.LookAt(position1);
                    //LookInPlace(position1.position);
                }
                else
                {
                    waitTime = 1.5f;
                }

                enemy.speed = 0.001f;
                enemy.agent.speed = 0.01f;

                //if (enemy.isRoomTwo && !enemy.turningDone && time >= 1.5f)
                //{
                //    //LookInPlace(position1.position);
                //    //enemy.transform.LookAt(position2);
                //    enemy.StartCoroutine(TurnToFace(position2.position));
                //    //nmm.ignoreFromBuild = false;
                //}
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
                if (enemy.isRoomTwo)
                {
                    //enemy.StopAllCoroutines();
                    //enemy.isStandingStill = false;
                    //enemy.hasNoiseArea = false;
                    enemy.turningDone = true;
                    enemy.gotUp = true;
                }

                if (!enemy.isRoomTwo)
                {
                    enemy.transform.rotation = Quaternion.identity;
                }
                Debug.Log("Going back to patrolling!");
                HideQuestionMark();
                return enemy.PerformTransition(AIStateType.Patrol);
            }

            if (enemy.isRoomTwo)
            {
                if (enemy.gotUp)
                {
                    if (enemy.playerVisibleTimer >= 0.5f && enemy.playerVisibleTimer <= 0.99f ||
                    !cc.isCrouching && Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance
                            || Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance / 1.5f)
                    {
                        enemy.hasBeenNoticed = true;
                        //enemy.time = 0;
                        Debug.Log("Noticed player!");
                        HideQuestionMark();
                        enemy.inCameraView = false;
                        return enemy.PerformTransition(AIStateType.FollowTarget);
                    }
                }
            }
            else if (!enemy.isRoomTwo)
            {
                if (enemy.playerVisibleTimer >= 0.5f && enemy.playerVisibleTimer <= 0.99f ||
                    !cc.isCrouching && Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance
                            || Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance / 1.5f)
                {
                    enemy.hasBeenNoticed = true;
                    enemy.time = 0;
                    Debug.Log("Noticed player!");
                    HideQuestionMark();
                    enemy.inCameraView = false;
                    return enemy.PerformTransition(AIStateType.FollowTarget);
                }
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

        //public void LookInPlace(Vector3 position)
        //{
        //    this.enemy.agent.updatePosition = false;
        //    this.enemy.agent.updateRotation = true;
        //    this.enemy.agent.SetDestination(position);
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
