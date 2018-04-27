﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.AI
{
    public class GoToNoiseArea : AIStateBase
    {
        private float time = 0;
        public float waitTime = 5f;

        EnemyUnit enemy;

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
        }

        public override void Update()
        {
            if (!ChangeState())
            {
                enemy.speed = 0.14f;
                enemy.ShowQuestionMark();

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
                enemy.goToAlertMode = true;
                time = 0;
                //Debug.Log("Do we stay here?");
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
                enemy.HideQuestionMark();
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
