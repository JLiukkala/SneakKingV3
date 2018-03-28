using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointOfInterestCamera : MonoBehaviour
{
    public vThirdPersonCamera _camera;
    public Transform _pointOfInterestPosition;

    public float minimumX = 16.83924f;
    public float maximumX = 2.2f;

    public float minimumY = 4.59f;
    public float maximumY = 0.95f;

    public float minimumZ = 29.05f;
    public float maximumZ = 15.91f;

    public float duration = 6f;

    [HideInInspector]
    public float t = 0;

    private float startTime;

    public bool hasPointOfInterest;

    [HideInInspector]
    public bool hasReachedEnd;

    [HideInInspector]
    public bool hasReachedCameraSwitchPoint;

	void Start ()
    {
        startTime = Time.time;
	}
	
	void Update ()
    {
        if (hasPointOfInterest)
        {
            t = (Time.time - startTime) / duration;
            _camera.transform.position = new Vector3(Mathf.SmoothStep(minimumX, maximumX, t), Mathf.SmoothStep(minimumY, maximumY, t), Mathf.SmoothStep(minimumZ, maximumZ, t));

            if (t > 0.5f)
            {
                hasReachedCameraSwitchPoint = true;
            }

            if (t > 0.75f)
            {
                hasReachedEnd = true;
                t = (Time.time - startTime) / duration;
                _camera.transform.position = new Vector3(Mathf.SmoothStep(maximumX, minimumX, t), Mathf.SmoothStep(maximumY, minimumY, t), Mathf.SmoothStep(maximumZ, minimumZ, t));
                Destroy(this);

            }
        }
    }
}
