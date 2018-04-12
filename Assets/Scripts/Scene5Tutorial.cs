using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene5Tutorial : MonoBehaviour
{
    public GameObject crouchAndHideTutorial;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            crouchAndHideTutorial.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            crouchAndHideTutorial.SetActive(false);
        }
    }
}
