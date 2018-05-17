using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;

public class VentCrouch : MonoBehaviour
{
    #region Variables
    public GameObject cameraObject;
    private vThirdPersonCamera cameraScript;

    [HideInInspector]
    public Transform cameraPosition;

    [HideInInspector]
    public bool enteringVent = false;
    #endregion

    void Start()
    {
        cameraScript = cameraObject.GetComponent<vThirdPersonCamera>();
        cameraPosition = GameObject.Find("CameraPosition").transform;

        cameraScript.isRoomOne = true;
    }

    void OnTriggerEnter(Collider other)
    {
        enteringVent = true;
    }

    void OnTriggerExit(Collider other)
    {
        enteringVent = false;
    }
}
