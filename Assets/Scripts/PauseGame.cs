using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    // The transform of the pause screen.
    public Transform pauseBackground;

	void Update()
    {
        // If the Tab key is pressed, the pause screen is 
        // set to active and the timescale is set to zero.
        // When Tab is pressed again, the opposite happens.
		if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (pauseBackground.gameObject.activeInHierarchy == false)
            {
                pauseBackground.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                pauseBackground.gameObject.SetActive(false);
                Time.timeScale = 1;

                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
        }
	}
}
