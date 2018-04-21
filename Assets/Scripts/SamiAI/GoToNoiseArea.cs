using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.AI
{
    public class GoToNoiseArea : AIStateBase
    {
        public bool moveAgain;
        public float waitTime = 8f;
        EnemyUnit enemy;

        public GoToNoiseArea(GameObject owner)
            : base(owner, AIStateType.GoToNoiseArea)
        {
            AddTransition(AIStateType.FollowTarget);
            AddTransition(AIStateType.Patrol);
            AddTransition(AIStateType.GoToLastKnownPosition);

            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

            enemy = Owner.GetComponent<EnemyUnit>();
        }

        public override void Update()
        {
            if (!ChangeState())
            {
                Vector3 tempNoiseAreaPosition = new Vector3(enemy.noiseArea.position.x,
                    enemy.transform.position.y, enemy.noiseArea.position.z);

                //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position,
                //    tempLastPlayerPosition, 
                //        enemy.speed * Time.deltaTime);
                enemy.agent.SetDestination(tempNoiseAreaPosition);
                //enemy.transform.position = new Vector3(enemy.transform.position.x,
                //     1.1f, enemy.transform.position.z);
                //enemy.transform.LookAt(enemy.lastPositionOfPlayer);

                enemy.StartCoroutine(TurnToFace(enemy.noiseArea.position));
                enemy.StartCoroutine(Wait());
            }
        }

        private bool ChangeState()
        {
            // 2. Did the player get away?
            // If yes, go to patrol state.
            if (moveAgain)
            {
                enemy.StopAllCoroutines();
                moveAgain = false;
                enemy.goToAlertMode = true;
                enemy.heardNoise = false;
                enemy.agent.speed = 2f;
                //enemy.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                return enemy.PerformTransition(AIStateType.Patrol);
            }
            // Otherwise return false.
            return false;
        }

        IEnumerator TurnToFace(Vector3 lookTarget)
        {
            Vector3 directionToLookTarget = (lookTarget - enemy.transform.position).normalized;
            float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z,
                directionToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(enemy.transform.eulerAngles.y, targetAngle)) > 0.09f)
            {
                float angle = Mathf.MoveTowardsAngle(enemy.transform.eulerAngles.y, targetAngle,
                    enemy.turnSpeed * Time.deltaTime);

                enemy.transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(waitTime);
            //hasWaited = true;
            yield return enemy.StartCoroutine(MoveAgain());
        }

        IEnumerator MoveAgain()
        {
            Debug.Log("Going Back To Patrolling");
            moveAgain = true;
            //hasWaited = false;
            yield return null;
        }
    }
}
