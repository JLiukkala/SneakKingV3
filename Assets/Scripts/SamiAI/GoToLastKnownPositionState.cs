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
                // If the player is seen by a camera, the enemy 
                // runs to the last known position of the player.
                if (enemy.inCameraView)
                {
                    // At 0.15f the enemy starts running.
                    enemy.speed = 0.15f;
                }
                else
                {
                    // At 0.14f the enemy walks.
                    enemy.speed = 0.14f;
                }

                // The enemy walks or runs towards the last known position 
                // of the player until the waitTime value is reached.
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
            // If enough time has passed or the enemy is close enough to
            // the last known position of the player, the state is changed.
            if (time >= waitTime || Vector3.Distance(Owner.transform.position, enemy.lastPositionOfPlayer) < enemy.stopDistance)
            {
                enemy.goToAlertMode = true;
                time = 0;
                enemy.inCameraView = false;
                return enemy.PerformTransition(AIStateType.Stop);
            }

            // If the enemy has gotten up in the second room, they are 
            // able to notice the player and begin following them.
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
