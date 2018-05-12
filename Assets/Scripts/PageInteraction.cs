using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageInteraction : MonoBehaviour
{
    // Reference needed to activate and deactivate the object.
    public GameObject pageInteraction;

    // Upon entering the trigger collider, the page pops up on the screen.
    void OnTriggerEnter(Collider other)
    {
        pageInteraction.SetActive(true);
    }

    // When the player exits the trigger, the page is set inactive.
    void OnTriggerExit(Collider other)
    {
        pageInteraction.SetActive(false);
    }
}
