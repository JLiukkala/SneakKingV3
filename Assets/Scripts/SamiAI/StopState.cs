using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Invector.AI
{
    public class StopState : AIStateBase
    {
        #region Variables
        private float time = 0;
        public float waitTime = 1.5f;

        EnemyUnit enemy;

        [HideInInspector]
        public GameObject questionMark;

        Transform position1;

        Invector.CharacterController.vThirdPersonController cc;
        #endregion

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

                // The enemy looks at this position in the
                // second room before beginning to patrol.
                if (enemy.isRoomTwo && !enemy.turningDone)
                {
                    enemy.transform.LookAt(position1);
                }

                // The enemy appears to not be moving during this state.
                enemy.speed = 0.001f;
                enemy.agent.speed = 0.01f;
            }
        }

        private bool ChangeState()
        {
            if (time >= waitTime)
            {
                time = 0;
                enemy.agent.speed = 0;
                enemy.speed = 0;

                if (enemy.isRoomTwo)
                {
                    enemy.turningDone = true;
                    enemy.gotUp = true;
                }

                // The rotation is not reset for the enemy during the second room because that would
                // make it glance at the player awkwardly while getting up from the sofa, when it shouldn't.
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
                // If the enemy has gotten up in the second room, 
                // they are able to begin chasing the player.
                if (enemy.gotUp)
                {
                    if (enemy.playerVisibleTimer >= 0.5f && enemy.playerVisibleTimer <= 0.99f ||
                    !cc.isCrouching && Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance
                            || Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance / 1.5f)
                    {
                        enemy.hasBeenNoticed = true;
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

            return false;
        }

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
