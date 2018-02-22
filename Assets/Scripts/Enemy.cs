﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float viewAngle;
    private float playerVisibleTimer;

    private Vector3 lastPositionOfPlayer;

    public Transform pathHolder;
    Transform player;
    Color originalVisionConeColor;

    bool spotted;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = visionCone.spotAngle;
        originalVisionConeColor = visionCone.color;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
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
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
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

        //if (playerVisibleTimer >= 0.25f && playerVisibleTimer <= 0.69f)
        //{
        //    lastPositionOfPlayer = player.transform.position;
        //    Vector3.MoveTowards(transform.position, lastPositionOfPlayer, speed * Time.deltaTime);
        //}

        if (spotted)
        {
            StartCoroutine(TurnToFace(player.position));
            speed = speed * 1.05f;
            Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            speed = 0;
        }
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
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, 
                speed * Time.deltaTime);

            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
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
