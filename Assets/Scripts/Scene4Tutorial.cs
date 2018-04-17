using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene4Tutorial : MonoBehaviour
{
    public GameObject sprintTutorial;

    void OnTriggerEnter(Collider other)
    {
        sprintTutorial.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        sprintTutorial.SetActive(false);
    }
}
