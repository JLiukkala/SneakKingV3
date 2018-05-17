using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageInteraction : MonoBehaviour
{
    public GameObject pageInteraction;

    void OnTriggerEnter(Collider other)
    {
        pageInteraction.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        pageInteraction.SetActive(false);
    }
}
