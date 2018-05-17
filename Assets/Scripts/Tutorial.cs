using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialMessage;

    void OnTriggerEnter(Collider other)
    {
        tutorialMessage.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        tutorialMessage.SetActive(false);
    }
}
