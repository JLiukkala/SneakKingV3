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

    // These methods are easy to understand.

    void OnTriggerStay(Collider other)
    {
        keyPickUpText.SetActive(true);

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            keyInteraction.SetActive(true);
            keyPickUpText.SetActive(false);

            numberOfKeys++;

            Destroy(keyObject);
            Destroy(gameObject.GetComponent<BoxCollider>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyPickUpText.SetActive(false);
        }
    }
}
