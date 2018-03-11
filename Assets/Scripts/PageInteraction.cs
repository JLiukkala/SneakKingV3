using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageInteraction : MonoBehaviour
{
    // References needed to activate and deactivate objects.
    public GameObject pageInteraction;
    public GameObject pagePickUpText;

    // These methods are pretty clear.

    void OnTriggerEnter(Collider other)
    {
        pagePickUpText.SetActive(true);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        { 
            pagePickUpText.SetActive(false);
            pageInteraction.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pageInteraction.SetActive(false);
            pagePickUpText.SetActive(false);
        }
    }
}
