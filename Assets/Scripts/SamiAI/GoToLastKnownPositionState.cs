using UnityEngine;
using System.Collections;

namespace Invector.AI
{
	public class GoToLastKnownPositionState : AIStateBase
	{
        #region Variables
        private float time = 0;
        public float waitTime = 5f;

        EnemyUnit enemy;

        [HideInInspector]
        public GameObject questionMark;

        Invector.CharacterController.vThirdPersonController cc;
        #endregion

        public GoToLastKnownPositionState( GameObject owner )
			: base()
		{
            State = AIStateType.GoToLastKnownPosition;

            AddTransition( AIStateType.Stop );
            AddTransition( AIStateType.FollowTarget );

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
			if ( !ChangeState() )
			{
                if (enemy.inCameraView)
                {
                    enemy.speed = 0.15f;
                }
                else
                {
                    enemy.speed = 0.14f;
                }

                if (time < waitTime)
                {
                    time += Time.deltaTime;
                    enemy.agent.SetDestination(enemy.lastPositionOfPlayer);
                }
                ShowQuestionMark();

                if (enemy.isRoomTwo)
                {
                    waitTime = 1;
                }
            }
		}

        private bool ChangeState()
		{
            if (time >= waitTime || Vector3.Distance(Owner.transform.position, enemy.lastPositionOfPlayer) < enemy.stopDistance)
            {
                enemy.goToAlertMode = true;
                time = 0;
                enemy.inCameraView = false;
                return enemy.PerformTransition(AIStateType.Stop);
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
