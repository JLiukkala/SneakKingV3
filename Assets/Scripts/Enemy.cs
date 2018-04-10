using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.CharacterController
{
    public class Enemy : MonoBehaviour
    {
        public static event System.Action OnEnemyHasSpottedPlayer;

        public float speed = 0.15f;
        public float waitTime = 0.3f;
        public float turnSpeed = 90;
        public float timeToSpotPlayer = 1;

        public Light visionCone;
        public float viewDistance;
        public LayerMask viewMask;

        public float hearDistance;

        private float viewAngle;
        private float playerVisibleTimer;

        private Vector3 lastPositionOfPlayer;
        private int lastWaypoint;
        Vector3[] waypoints;

        public Transform pathHolder;
        Transform player;
        Color originalVisionConeColor;

        bool spotted;
        bool almostSpotted;
        public bool isEnemyStill;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            viewAngle = visionCone.spotAngle;
            originalVisionConeColor = visionCone.color;

            waypoints = new Vector3[pathHolder.childCount];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = pathHolder.GetChild(i).position;
                waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);

                StartCoroutine(FollowPath(waypoints));
            }

            OnEnemyHasSpottedPlayer += IsSpotted;
        }

        void Update()
        {
            if (CanSeePlayer() && playerVisibleTimer < timeToSpotPlayer)
            {
                playerVisibleTimer += Time.deltaTime;
            }
            else if (!CanSeePlayer() && playerVisibleTimer > 0)
            {
                playerVisibleTimer -= Time.deltaTime;
            }

            playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
            visionCone.color = Color.Lerp(originalVisionConeColor, Color.red,
                playerVisibleTimer / timeToSpotPlayer);

            if (playerVisibleTimer >= timeToSpotPlayer)
            {
                if (OnEnemyHasSpottedPlayer != null)
                {
                    OnEnemyHasSpottedPlayer();
                }
            }

            if (playerVisibleTimer >= 0.5f && playerVisibleTimer <= 0.99f ||
                !player.GetComponent<vThirdPersonController>().isCrouching &&
                    Vector3.Distance(transform.position, player.position) < hearDistance)
            {
                almostSpotted = true;
                SetLastKnownPosition();
            }

            if (almostSpotted && !spotted)
            {
                StopAllCoroutines();
                StartCoroutine(TurnToFace(lastPositionOfPlayer));
                speed = 1;
                transform.position = Vector3.MoveTowards(transform.position, lastPositionOfPlayer, speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, 0.9f, transform.position.z);
            }

            // Doesn't go over here!
            if (lastPositionOfPlayer != null && Vector3.Distance(transform.position, lastPositionOfPlayer) < 1)
            {
                Debug.Log("Blaa!");
                almostSpotted = false;
                StopAllCoroutines();
                speed = 0.15f;
                transform.position = Vector3.MoveTowards(transform.position, waypoints[0], speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, 0.9f, transform.position.z);
                StartCoroutine(FollowPath(waypoints));
            }

            if (spotted)
            {
                almostSpotted = false;
                StartCoroutine(TurnToFace(player.position));
                speed = 0;
            }
        }

        void SetLastKnownPosition()
        {
            lastPositionOfPlayer = player.position;
        }

        bool CanSeePlayer()
        {
            if (Vector3.Distance(transform.position, player.position) < viewDistance)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

                if (angleBetweenEnemyAndPlayer < viewAngle / 2)
                {
                    if (!Physics.Linecast(transform.position, player.position, viewMask))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        IEnumerator FollowPath(Vector3[] waypoints)
        {
            transform.position = waypoints[0];

            int targetWaypointIndex = 1;

            Vector3 targetWaypoint = waypoints[targetWaypointIndex];

            if (lastWaypoint != 0)
            {
                targetWaypoint = waypoints[lastWaypoint];
            }

            transform.LookAt(targetWaypoint);

            while (true)
            {
                if (isEnemyStill)
                {
                    waitTime = 6;

                    targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                    targetWaypoint = waypoints[targetWaypointIndex];
                    yield return new WaitForSeconds(waitTime);
                    yield return StartCoroutine(TurnToFace(targetWaypoint));
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint,
                                    speed * Time.deltaTime);

                    if (almostSpotted)
                    {
                        lastWaypoint = targetWaypointIndex;
                    }

                    if (transform.position == targetWaypoint)
                    {
                        targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                        targetWaypoint = waypoints[targetWaypointIndex];
                        yield return new WaitForSeconds(waitTime);
                        yield return StartCoroutine(TurnToFace(targetWaypoint));
                    }
                }
                yield return null;
            }
        }

        IEnumerator TurnToFace(Vector3 lookTarget)
        {
            Vector3 directionToLookTarget = (lookTarget - transform.position).normalized;
            float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z,
                directionToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
            {
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle,
                    turnSpeed * Time.deltaTime);

                transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }

        void IsSpotted()
        {
            spotted = true;
        }

        void OnDrawGizmos()
        {
            Vector3 startPosition = pathHolder.GetChild(0).position;
            Vector3 previousPosition = startPosition;

            foreach (Transform waypoint in pathHolder)
            {
                Gizmos.DrawSphere(waypoint.position, 0.3f);
                Gizmos.DrawLine(previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }
            Gizmos.DrawLine(previousPosition, startPosition);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        }

        void OnDestroy()
        {
            OnEnemyHasSpottedPlayer -= IsSpotted;
        }
    }
}
