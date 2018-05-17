using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PointOfInterestCamera : MonoBehaviour
{
    #region Variables
    vThirdPersonCamera _camera;

    // Reference to the transform of the point of interest
    // in the scene. The camera will look at this position
    // while the SmoothStep takes place. 
    [HideInInspector]
    public Transform _pointOfInterestPosition;

    [HideInInspector]
    public GameObject skipCamera;

    // The minimum values indicate the starting position
    // of the camera. The maximum values are where the 
    // camera will be smoothstepping towards.
    public float minimumX = 0;
    public float maximumX = 0;

    public float minimumY = 0;
    public float maximumY = 0;

    public float minimumZ = 0;
    public float maximumZ = 0;

    public float duration = 0;

    // When t reaches 1, the SmoothStep is at the end point (maximum values).
    [HideInInspector]
    public float t = 0;

    private float startTime;

    private float timeWhenDestroyed = 0.75f;

    // If this is unchecked (false), the SmoothStep will not occur.
    public bool hasPointOfInterest;

    [HideInInspector]
    public bool hasReachedEnd;
    #endregion

    void Start ()
    {
        startTime = Time.time;

        _camera = GetComponent<vThirdPersonCamera>();

        if (hasPointOfInterest)
        {
            _pointOfInterestPosition = GameObject.Find("PointOfInterestPosition").transform;

            skipCamera = GameObject.Find("SkipCamera");
        }
	}
	
	void Update ()
    {
        if (hasPointOfInterest)
        {
            MoveCamera();

            // If Space or Fire2 (B button on the Xbox controller) is 
            // pressed during MoveCamera(), the camera movement is skipped.
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire2"))
            {
                hasReachedEnd = true;
                Destroy(skipCamera.gameObject);
                Destroy(this);
            }
        }
    }

    /// <summary>
    /// Moves the camera from one place to another with the help of SmoothStep.
    /// </summary>
    public void MoveCamera ()
    {
        t = (Time.time - startTime) / duration;
        _camera.transform.position = new Vector3(Mathf.SmoothStep(minimumX, maximumX, t), Mathf.SmoothStep(minimumY, maximumY, t), Mathf.SmoothStep(minimumZ, maximumZ, t));

        // If t is greater than timeWhenDestroyed, the end position has been reached
        // and this script + the skip camera object are destroyed from the scene.
        if (t > timeWhenDestroyed)
        {
            hasReachedEnd = true;
            Destroy(skipCamera.gameObject);
            Destroy(this);
        }
    }
}
