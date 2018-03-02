using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    public Transform secondRoomSpawnPoint;

    //public RawImage fadeScreen;

    //void Start()
    //{
    //    fadeScreen.canvasRenderer.SetAlpha(0.0f);
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //FadeIn();
            other.transform.position = secondRoomSpawnPoint.position;
        }
    }

    //void OnTriggerExit(Collider other)
    //{
    //    FadeOut();
    //}

    //public void FadeIn()
    //{
    //    fadeScreen.CrossFadeAlpha(1.0f, 3.0f, true);
    //}

    //public void FadeOut()
    //{
    //    fadeScreen.CrossFadeAlpha(0.0f, 3.0f, true);
    //}
}
