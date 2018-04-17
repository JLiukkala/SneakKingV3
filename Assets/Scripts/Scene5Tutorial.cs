using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene5Tutorial : MonoBehaviour
{
    public GameObject crouchAndHideTutorial;

    void OnTriggerEnter(Collider other)
    {
        crouchAndHideTutorial.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        crouchAndHideTutorial.SetActive(false);
    }
}
