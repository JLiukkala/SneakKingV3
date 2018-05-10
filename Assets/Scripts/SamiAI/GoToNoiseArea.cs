using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.AI
{
    public class GoToNoiseArea : AIStateBase
    {
        private float time = 0;
        public float waitTime = 4f;

        EnemyUnit enemy;

        [HideInInspector]
        public GameObject questionMark;

        Invector.CharacterController.vThirdPersonController cc;

        public GoToNoiseArea(GameObject owner)
            : base() //owner, AIStateType.GoToNoiseArea)
        {
            State = AIStateType.GoToNoiseArea;

            AddTransition( AIStateType.FollowTarget );
            AddTransition( AIStateType.Stop );

            if (Owner == null)
            {
                Owner = owner;
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            cc = enemy.Target.GetComponent<Invector.CharacterController.vThirdPersonController>();

            questionMark = GameObject.Find("QuestionMark");
        }

        public override void Update()
        {
            if (!ChangeState())
            {
                enemy.speed = 0.14f;
                enemy.agent.speed = 0.5f;
                ShowQuestionMark();

                if (enemy.isRoomTwo)
                {
                    if (!enemy.gotUp)
                    {
                        waitTime = 0.01f;
                    }
                    else if (enemy.gotUp)
                    {
                        waitTime = 3f;
                    }
                }

                if (time < waitTime)
                {
                    enemy.heardNoise = false;
                    time += Time.deltaTime;
                    enemy.agent.SetDestination(enemy.noiseArea.position);
                }
            }
        }

        private bool ChangeState()
        {
            // 2. Did the player get away?
            // If yes, go to stop state.
            if (time >= waitTime || Vector3.Distance(Owner.transform.position, enemy.noiseArea.position) < enemy.stopDistance)
            {
                if (!enemy.isRoomTwo)
                {
                    enemy.goToAlertMode = true;
                }
                time = 0;
                return enemy.PerformTransition(AIStateType.Stop);
            }

            if (enemy.playerVisibleTimer >= 0.5f && enemy.playerVisibleTimer <= 0.99f ||
                    !cc.isCrouching && Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance
                            || Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance / 1.5f)
            {
                enemy.SetOwnLastKnownPosition();
                enemy.hasBeenNoticed = true;
                enemy.time = 0;
                Debug.Log("Noticed player!");
                HideQuestionMark();
                return enemy.PerformTransition(AIStateType.FollowTarget);
            }

            if (enemy.inCameraView)
            {
                enemy.time = 0;
                Debug.Log("Seen by camera!");
                return enemy.PerformTransition(AIStateType.GoToLastKnownPosition);
            }

            // Otherwise return false.
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
