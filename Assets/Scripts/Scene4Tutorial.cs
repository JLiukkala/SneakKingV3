using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene4Tutorial : MonoBehaviour
{
    public GameObject sprintTutorial;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sprintTutorial.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sprintTutorial.SetActive(false);
        }
    }
}
