using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    // This is the scene that gets loaded next.
    [SerializeField]
    private string loadLevel;

    void Update ()
    {
        // If the player presses Space, the next scene is loaded.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(loadLevel);
        }
    }
}
