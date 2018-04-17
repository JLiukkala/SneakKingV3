using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    // The scene that is transitioned into.
    [SerializeField]
    private string loadLevel;

    // These are for the fadein and fadeout at 
    // the beginning and end of each scene.
    public RawImage fadeScreen;
    public Animator anim;

    // The collider for a key (if the scene has one).
    [HideInInspector]
    public GameObject keyCollider;

    KeyInteraction keyInteraction;

    // The text that appears when the player 
    // steps in front of a locked door. 
    public GameObject doorLockedText;

    // If this boolean is set to true, 
    // a key is needed to open the door.
    public bool isLocked = false;

    bool isInTransition = false;

    void Start()
    {
        if (isLocked)
        {
            keyCollider = GameObject.Find("KeyCollider");
            keyInteraction = keyCollider.GetComponent<KeyInteraction>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // If the door is locked and needs a key to open it, 
        // a message is displayed to the player about that. 
        if (isLocked)
        {
            if (keyInteraction.numberOfKeys == 1)
            {
                doorLockedText.SetActive(false);
            }
            else
            {
                if (isInTransition)
                {
                    doorLockedText.SetActive(false);
                } 
                else
                {
                    doorLockedText.SetActive(true);
                }
            }
        }
        else
        {
            // This is for when isLocked is set to false (the door is not locked).
            StartCoroutine(Fading());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isLocked)
        {
            // If the player is inside the collider and has a key, the transition occurs.
            if (keyInteraction.numberOfKeys == 1)
            {
                isInTransition = true;
                StartCoroutine(Fading());
                keyInteraction.numberOfKeys = 0;
                doorLockedText.SetActive(false);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // When the player leaves the collider, the text 
        // related to the door being locked is set as inactive.
        doorLockedText.SetActive(false);
    }

    // This is the method that does the fadein and loads the next scene.
    IEnumerator Fading()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => fadeScreen.color.a == 1);
        SceneManager.LoadScene(loadLevel);
    }
}
