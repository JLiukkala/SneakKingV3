using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCollisionScript : MonoBehaviour {

    [SerializeField]
    private CapsuleCollider _lightCol;

	// Use this for initialization
	void Start () {
        _lightCol = GetComponent<CapsuleCollider>();
	}

    private void OnCollisionStay(Collision collision)
    {

    }

    private void OnTriggerStay(Collider other)
    {

        if (VignetteController._vignetteVolume > 0.1f)
        {
            VignetteController._vignetteVolume -= 0.4f * Time.deltaTime;
        }
    }
}
