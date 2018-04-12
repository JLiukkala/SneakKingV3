using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    // References that are needed.
    public GameObject keyInteraction;
    public GameObject keyObject;
    public GameObject keyPickUpText;

    // The number of keys that the player has.
    // The player should only have one at a time.
    [HideInInspector]
    public int numberOfKeys = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyPickUpText.SetActive(true);
        }
    }

    // While staying inside the trigger collider,
    // the pick-up text for the key is active.
    void OnTriggerStay(Collider other)
    {
        //keyPickUpText.SetActive(true);

        // If the player presses E while inside,
        // the pick-up text disappears and the key
        // appears in the top-left corner of the screen.
        // The number of keys is then set to one plus the 
        // key object and trigger collider are destroyed.
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) || other.CompareTag("Player") && Input.GetButtonDown("Fire3"))
        {
            keyInteraction.SetActive(true);
            keyPickUpText.SetActive(false);

            numberOfKeys++;

            Destroy(keyObject);
            Destroy(gameObject.GetComponent<BoxCollider>());
        }
    }

    // If the player exits the trigger, the text is set inactive.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyPickUpText.SetActive(false);
        }
    }
}
