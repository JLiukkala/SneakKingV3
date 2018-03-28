using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamScript : MonoBehaviour {

    public Transform _cameraBody;
    public Transform _cameraStand;
    public Transform _cameraLens;

    [Tooltip("Camera rotates based on positive and negative versions of this value.")]
    public float _rotationAngle = 30;

    [Tooltip("Camera rotation speed.")]
    public float _rotationSpeed = 5;

    private bool _toRight = true;
    private bool _toLeft = false;

    private Quaternion _startRot;

    // Use this for initialization
    void Awake () {
        _startRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        
        Quaternion cRot = _cameraBody.transform.rotation;
        Quaternion _targetRotation =_startRot;



        if (_toRight)
        {

            _targetRotation = Quaternion.Euler(-55, _startRot.eulerAngles.y + _rotationAngle, _startRot.eulerAngles.z + _rotationAngle);
            if (cRot == _targetRotation)
            {
                _toLeft = true;
                _toRight = false;
            }
        }
        else if (_toLeft)
        {
            _targetRotation = Quaternion.Euler(-55, _startRot.eulerAngles.y - _rotationAngle, _startRot.eulerAngles.z -_rotationAngle);
            if (cRot == _targetRotation)
            {
                _toLeft = false;
                _toRight = true;
            }
        }

        cRot = Quaternion.RotateTowards(cRot, _targetRotation, _rotationSpeed * Time.deltaTime);

        _cameraBody.transform.rotation = cRot;
        _cameraStand.transform.rotation = cRot;
        _cameraLens.transform.rotation = cRot;
    }
}
