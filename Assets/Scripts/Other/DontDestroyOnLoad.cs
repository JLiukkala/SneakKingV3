using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // This is used to have the GameLoseCanvas persist through scene changes.
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
