﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageInteraction : MonoBehaviour
{
    // References needed to activate and deactivate objects.
    public GameObject pageInteraction;
    public GameObject pagePickUpText;

    // Upon entering the trigger collider, the text
    // for reading the page pops up.
    void OnTriggerEnter(Collider other)
    {
        //pagePickUpText.SetActive(true);
        pageInteraction.SetActive(true);
    }

    // If the player stays in the trigger and presses E,
    // the pick-up text goes away and the page shows itself. 
    //void OnTriggerStay(Collider other)
    //{
    //    if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire3"))
    //    { 
    //        pagePickUpText.SetActive(false);
    //        pageInteraction.SetActive(true);
    //    }
    //}

    // When the player exits the trigger, both texts are set inactive.
    void OnTriggerExit(Collider other)
    {
        pageInteraction.SetActive(false);
        //pagePickUpText.SetActive(false);
    }
}
