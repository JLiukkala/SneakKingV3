using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Invector.AI
{
	public class FollowTargetState : AIStateBase
	{
        private float time = 0;
        public float waitTime = 0.3f;

        [HideInInspector]
        public GameObject exclamationMark;

        EnemyUnit enemy;

		public FollowTargetState( GameObject owner )
			: base()
		{
            State = AIStateType.FollowTarget;

            AddTransition( AIStateType.Patrol );
            AddTransition( AIStateType.GoToLastKnownPosition );
            AddTransition( AIStateType.GoToNoiseArea );

            if (Owner == null)
            {
                Owner = owner;
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            exclamationMark = GameObject.Find("ExclamationMark");
        }

        public override void Update()
		{
			if ( !ChangeState() )
			{
                if (time < waitTime) 
                {
                    time += Time.deltaTime;
                }

                if (time >= waitTime)
                {
                    time = 0;
                    enemy.hasBeenNoticed = false;
                    enemy.speed = 0.15f;

                    Vector3 playerPosWithoutY = new Vector3(enemy.Target.position.x, enemy.transform.position.y, enemy.Target.position.z);

                    enemy.agent.SetDestination(enemy.Target.position);

                    ShowExclamationMark();

                    if (Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance)
                    {
                        enemy.transform.LookAt(playerPosWithoutY);
                        enemy.speed = 0.01f;
                        enemy.agent.speed = 0.001f;
                    }
                    else if (Vector3.Distance(Owner.transform.position, enemy.Target.position) > enemy.stopDistance)
                    {
                        if (enemy.isRoomEight)
                        {
                            enemy.speed = 0.15f;
                            enemy.agent.speed = 1f;
                        }
                        else if (enemy.isRoomTwo)
                        {
                            enemy.speed = 0.15f;
                            enemy.agent.speed = 0.3f;
                            enemy.viewDistance = 14;
                            enemy.hearDistance = 10;
                        }
                        else
                        {
                            enemy.speed = 0.15f;
                            enemy.agent.speed = 0.3f;
                        }

                        enemy.agent.SetDestination(enemy.Target.position);
                    }
                }
            }
		}

        private bool ChangeState()
		{
			// 2. Did the player get away?
			// If yes, go to last known position state.
            if (Vector3.Distance(Owner.transform.position, enemy.Target.position)
                        > enemy.viewDistance)
            {
                enemy.SetLastKnownPosition();
                Debug.Log("Going to last known position!");
                HideExclamationMark();
                return enemy.PerformTransition(AIStateType.GoToLastKnownPosition);
            }

            // Otherwise return false.
            return false;
		}

        public void ShowExclamationMark()
        {
            for (int i = 0; i < exclamationMark.transform.childCount; i++)
            {
                exclamationMark.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        public void HideExclamationMark()
        {
            for (int i = 0; i < exclamationMark.transform.childCount; i++)
            {
                exclamationMark.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
