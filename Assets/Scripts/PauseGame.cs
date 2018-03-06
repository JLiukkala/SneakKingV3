using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public Transform canvas;

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (canvas.gameObject.activeInHierarchy == false)
            {
                canvas.gameObject.SetActive(true);
                Time.timeScale = 0;

                //if (Input.GetKeyDown(KeyCode.Escape))
                //{
                //    Application.Quit();
                //}
            }
            else
            {
                canvas.gameObject.SetActive(false);
                Time.timeScale = 1;

                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
        }
	}
}
