using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentAnimationTrigger : MonoBehaviour
{
    public Animator vent;

    void OnTriggerEnter(Collider other)
    {
        vent.SetBool("ifNearVent", true);
        Destroy(gameObject);
    }
}
