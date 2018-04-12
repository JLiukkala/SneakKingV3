using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointOfInterestCamera : MonoBehaviour
{
    // Reference to the camera.
    vThirdPersonCamera _camera;

    // Reference to the transform of the point of interest
    // in the scene. The camera will look at this position
    // while the SmoothStep takes place. 
    [HideInInspector]
    public Transform _pointOfInterestPosition;

    // The minimum values indicate the starting position
    // of the camera. The maximum values are where the 
    // camera will be smoothstepping towards.
    public float minimumX = 0;
    public float maximumX = 0;

    public float minimumY = 0;
    public float maximumY = 0;

    public float minimumZ = 0;
    public float maximumZ = 0;

    // The duration of the camera lerp (SmoothStep).
    public float duration = 0;

    // When t reaches 1, the SmoothStep is at the end point (maximum values).
    [HideInInspector]
    public float t = 0;

    private float startTime;

    // If this is unchecked (false), the SmoothStep will not occur.
    public bool hasPointOfInterest;

    // A boolean variable for whether the camera 
    // has reached the desired end position.
    [HideInInspector]
    public bool hasReachedEnd;

    //[HideInInspector]
    //public bool hasReachedCameraSwitchPoint;

    // Setting variables and references in the Start function.
	void Start ()
    {
        startTime = Time.time;

        _camera = GetComponent<vThirdPersonCamera>();
        if (hasPointOfInterest)
        {
            _pointOfInterestPosition = GameObject.Find("PointOfInterestPosition").transform;
        }
	}
	
	void Update ()
    {
        // As long as the scene is set to have a point of interest,
        // the SmoothStep will happen.
        if (hasPointOfInterest)
        {
            t = (Time.time - startTime) / duration;
            _camera.transform.position = new Vector3(Mathf.SmoothStep(minimumX, maximumX, t), Mathf.SmoothStep(minimumY, maximumY, t), Mathf.SmoothStep(minimumZ, maximumZ, t));

            //if (t > 0.5f)
            //{
            //    hasReachedCameraSwitchPoint = true;
            //}

            // If t is greater than this value, the end 
            // position has been reached and this script 
            // is destroyed from the camera in the scene.
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
