using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointOfInterestCamera : MonoBehaviour
{
    vThirdPersonCamera _camera;

    [HideInInspector]
    public Transform _pointOfInterestPosition;

    public float minimumX = 0;
    public float maximumX = 0;

    public float minimumY = 0;
    public float maximumY = 0;

    public float minimumZ = 0;
    public float maximumZ = 0;

    public float duration = 0;

    [HideInInspector]
    public float t = 0;

    private float startTime;

    public bool hasPointOfInterest;

    [HideInInspector]
    public bool hasReachedEnd;

    //[HideInInspector]
    //public bool hasReachedCameraSwitchPoint;

	void Start ()
    {
        startTime = Time.time;
        _camera = GetComponent<vThirdPersonCamera>();
        _pointOfInterestPosition = GameObject.Find("PointOfInterestPosition").transform;
	}
	
	void Update ()
    {
        if (hasPointOfInterest)
        {
            t = (Time.time - startTime) / duration;
            _camera.transform.position = new Vector3(Mathf.SmoothStep(minimumX, maximumX, t), Mathf.SmoothStep(minimumY, maximumY, t), Mathf.SmoothStep(minimumZ, maximumZ, t));

            //if (t > 0.5f)
            //{
            //    hasReachedCameraSwitchPoint = true;
            //}

            if (t > 0.75f)
            {
                hasReachedEnd = true;
                //t = (Time.time - startTime) / duration;
                //_camera.transform.position = new Vector3(Mathf.SmoothStep(maximumX, minimumX, t), Mathf.SmoothStep(maximumY, minimumY, t), Mathf.SmoothStep(maximumZ, minimumZ, t));
                Destroy(this);

            }
        }
    }
}
