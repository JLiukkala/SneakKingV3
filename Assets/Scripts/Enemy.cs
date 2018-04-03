using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.CharacterController
{
    public class Enemy : MonoBehaviour
    {
        // The necessary variables and references.
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
            // Setting some variables and references.
            player = GameObject.FindGameObjectWithTag("Player").transform;
            viewAngle = visionCone.spotAngle;
            originalVisionConeColor = visionCone.color;

            // Filling the waypoints with the positions of the child 
            // objects in the pathHolder. After that the FollowPath 
            // method is started as a coroutine.
            waypoints = new Vector3[pathHolder.childCount];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = pathHolder.GetChild(i).position;
                waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);

                StartCoroutine(FollowPath(waypoints));
            }

            // The method IsSpotted is subscribed to 
            // the OnEnemyHasSpottedPlayer event.
            OnEnemyHasSpottedPlayer += IsSpotted;
        }

        void Update()
        {
            // If the enemy sees the player while the player
            // visible timer is less than the time to spot the player,
            // playerVisibleTimer is increased by Time.deltaTime.
            if (CanSeePlayer() && playerVisibleTimer < timeToSpotPlayer)
            {
                playerVisibleTimer += Time.deltaTime;
            }
            // Otherwise, playerVisibleTimer is decresed by Time.deltaTime.
            else if (!CanSeePlayer() && playerVisibleTimer > 0)
            {
                playerVisibleTimer -= Time.deltaTime;
            }

            // Clamps the value of the playerVisibleTimer variable 
            // to be between zero and the timeToSpotPlayer variable.
            playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

            // The enemy's vision cone lerps towards red 
            // if the player is visible to the enemy.
            visionCone.color = Color.Lerp(originalVisionConeColor, Color.red,
                playerVisibleTimer / timeToSpotPlayer);

            // If the playerVisibleTimer reaches the value of the timeToSpotPlayer
            // variable, the event OnEnemyHasSpottedPlayer is called.
            if (playerVisibleTimer >= timeToSpotPlayer)
            {
                if (OnEnemyHasSpottedPlayer != null)
                {
                    OnEnemyHasSpottedPlayer();
                }
            }

            // If the enemy sees the player for only a very short 
            // while when the player is not crouching and is close 
            // enough to the enemy, the player is almost spotted 
            // and their last known position is recorded.
            if (playerVisibleTimer >= 0.5f && playerVisibleTimer <= 0.99f ||
                !player.GetComponent<vThirdPersonController>().isCrouching &&
                    Vector3.Distance(transform.position, player.position) < hearDistance)
            {
                almostSpotted = true;
                SetLastKnownPosition();
            }

            // If the player is almost spotted, all coroutines are stopped
            // to end the patrolling and the enemy turns to the last known
            // position of the player. The enemy moves towards the position.
            if (almostSpotted && !spotted)
            {
                StopAllCoroutines();
                StartCoroutine(TurnToFace(lastPositionOfPlayer));
                speed = 1;
                transform.position = Vector3.MoveTowards(transform.position, lastPositionOfPlayer, speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, 0.9f, transform.position.z);
            }

            //// Doesn't go over here!
            //if (lastPositionOfPlayer != null && Vector3.Distance(transform.position, lastPositionOfPlayer) < 1)
            //{
            //    Debug.Log("Blaa!");
            //    almostSpotted = false;
            //    StopAllCoroutines();
            //    speed = 0.15f;
            //    transform.position = Vector3.MoveTowards(transform.position, waypoints[0], speed * Time.deltaTime);
            //    transform.position = new Vector3(transform.position.x, 0.9f, transform.position.z);
            //    StartCoroutine(FollowPath(waypoints));
            //}

            // If the player is spotted by the enemy, the enemy
            // stops moving and turns to face the player.
            if (spotted)
            {
                almostSpotted = false;
                StartCoroutine(TurnToFace(player.position));
                speed = 0;
            }
        }

        // Sets the last known position of the player for the enemy.
        void SetLastKnownPosition()
        {
            lastPositionOfPlayer = player.position;
        }

        // Determines whether the enemy can see the player.
        bool CanSeePlayer()
        {
            // If the distance between the enemy and the player is less than
            // the view distance that has been set, the angle between them is
            // less than the view angle divided by 2 and the player's collider
            // is recognized by the enemy, the enemy is able to see the player.
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

        // The method for moving between the waypoints.
        IEnumerator FollowPath(Vector3[] waypoints)
        {
            // The enemy's position is set to be 
            // the same as the first waypoint.
            transform.position = waypoints[0];

            int targetWaypointIndex = 1;
            
            // The waypoint that is the target for the enemy.
            Vector3 targetWaypoint = waypoints[targetWaypointIndex];

            //if (lastWaypoint != 0)
            //{
            //    targetWaypoint = waypoints[lastWaypoint];
            //}

            // The enemy looks at the target waypoint.
            transform.LookAt(targetWaypoint);

            while (true)
            {
                // If the enemy doesn't need to move, they will 
                // just change the direction they are facing
                // towards the target waypoint and wait a while.
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
                    // The enemy moves towards the target waypoint.
                    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint,
                                    speed * Time.deltaTime);

                    //if (almostSpotted)
                    //{
                    //    lastWaypoint = targetWaypointIndex;
                    //}

                    // If the enemy reaches the target waypoint, the target 
                    // changes to the next one. The wait time is then 
                    // applied to WaitForSeconds and TurnToFace starts. 
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

        // Turns the enemy towards the look target.
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
            // True when the player has been spotted.
            spotted = true;
        }

        // Draws waypoints as spheres, lines between the waypoints and a
        // ray for the vision cone that indicates how far the enemy sees.
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

        // When the objects are destroyed, IsSpotted 
        // is unsubscribed from OnEnemyHasSpottedPlayer.
        void OnDestroy()
        {
            OnEnemyHasSpottedPlayer -= IsSpotted;
        }
    }
}
