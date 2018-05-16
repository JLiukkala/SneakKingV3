using System.Collections;
using System.Collections.Generic;
using Invector.WaypointSystem;
using UnityEngine;

namespace Invector.AI
{
    public class StandingStillState : AIStateBase
    {
        #region Variables
        private Path _path;
        private Direction _direction;
        private float _arriveDistance;

        EnemyUnit enemy;

        private float roomTwoTime = 0f;

        Invector.CharacterController.vThirdPersonController cc;

        GameObject uISceneCanvas;
        PauseGame pauseGameScript;

        GameObject snoringSound;
        GameObject huhSound;

        public Waypoint CurrentWaypoint { get; private set; }
        #endregion

        public StandingStillState(GameObject owner, Path path,
            Direction direction, float arriveDistance)
            : base()
        {
            State = AIStateType.StandingStill;

            if (Owner == null)
            {
                Owner = owner;
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            cc = enemy.Target.GetComponent<Invector.CharacterController.vThirdPersonController>();


            uISceneCanvas = GameObject.Find("UISceneCanvas");
            pauseGameScript = uISceneCanvas.GetComponent<PauseGame>();

            snoringSound = GameObject.Find("SnoringSound");
            huhSound = GameObject.Find("HuhSound");

            AddTransition(AIStateType.FollowTarget);
            AddTransition(AIStateType.GoToLastKnownPosition);
            AddTransition(AIStateType.GoToNoiseArea);
            _path = path;
            _direction = direction;
            _arriveDistance = arriveDistance;
        }

        public override void StateActivated()
        {
            base.StateActivated();
            CurrentWaypoint = _path.GetClosestWaypoint(Owner.transform.position);
        }

        public override void Update()
        {
            if (!ChangeState())
            {
                if (pauseGameScript.isPaused)
                {
                    return;
                }

                if (enemy.isRoomTwo)
                {
                    enemy._docAnimator.SetBool("isSleeping", true);
                    enemy.agent.baseOffset = -5f;
                }

                CurrentWaypoint = GetWaypoint();

                if (enemy.goToAlertMode)
                {
                    enemy.hearDistance = 10;
                    enemy.viewDistance = 12;
                }
                else if (enemy.isRoomTwo)
                {
                    enemy.hearDistance = 1;
                }
                else
                {
                    enemy.hearDistance = 8;
                    enemy.viewDistance = 10;
                }
                Vector3 currentWaypointWithoutY = new Vector3(CurrentWaypoint.Position.x, enemy.transform.position.y, CurrentWaypoint.Position.z);
                enemy.speed = 0f;
                enemy.agent.speed = 0f;
                if (enemy.isRoomTwo)
                {
                    enemy.transform.LookAt(CurrentWaypoint.Position);
                }
                else
                {
                    enemy.transform.LookAt(currentWaypointWithoutY);
                }
            }
        }

        private Waypoint GetWaypoint()
        {
            Waypoint result = CurrentWaypoint;
            Vector3 toWaypointVector = CurrentWaypoint.Position - Owner.transform.position;
            float toWaypointSqr = toWaypointVector.sqrMagnitude;
            float sqrArriveDistance = _arriveDistance * _arriveDistance;
            if (toWaypointSqr <= sqrArriveDistance)
            {
                if (enemy.isStandingStill)
                {
                    if (enemy.time >= enemy.waitTime)
                    {
                        result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
                        enemy.time = 0;
                    }
                }
                else
                {
                    result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
                }
            }

            return result;
        }

        private bool ChangeState()
        {
            if (!enemy.isRoomTwo)
            {
                if (enemy.playerVisibleTimer >= 0.5f && enemy.playerVisibleTimer <= 0.99f ||
                    !cc.isCrouching && Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.hearDistance
                            || Vector3.Distance(Owner.transform.position, enemy.Target.position) < enemy.stopDistance / 1.5f)
                {
                    enemy.SetOwnLastKnownPosition();
                    enemy.hasBeenNoticed = true;
                    enemy.time = 0;
                    Debug.Log("Noticed player!");
                    return enemy.PerformTransition(AIStateType.FollowTarget);
                }
            } 

            if (NoiseArea.heardNoise)
            {
                if (enemy.isRoomTwo)
                {
                    enemy.time += Time.deltaTime;
                    enemy.agent.baseOffset = 0;
                    enemy._docAnimator.SetBool("isSleeping", false);
                    snoringSound.SetActive(false);
                    huhSound.SetActive(false);

                    if (enemy.isRoomTwo && enemy.time < roomTwoTime)
                    {
                        enemy.speed = 0.001f;
                        enemy.agent.speed = 0.01f;
                    }

                    if (enemy.isRoomTwo && enemy.time >= roomTwoTime)
                    {
                        Debug.Log("Heard something!");
                        return enemy.PerformTransition(AIStateType.GoToNoiseArea);
                    }
                }
                else
                {
                    enemy.time = 0;
                    Debug.Log("Heard something!");
                    return enemy.PerformTransition(AIStateType.GoToNoiseArea);
                }
            }

            if (enemy.inCameraView)
            {
                enemy.time = 0;
                Debug.Log("Seen by camera!");
                return enemy.PerformTransition(AIStateType.GoToLastKnownPosition);
            }

            return false;
        }
    }
}
