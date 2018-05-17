using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Invector
{
    public class PauseGame : MonoBehaviour
    {
        #region Variables
        public Transform pauseBackground;

        EventSystem es;

        public GameObject confirmButton;

        [HideInInspector]
        public bool isPaused = false;
        #endregion

        void Start()
        {
            es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        void Update()
        {
            // If the Tab or JoystickButton 7 (Start button on the Xbox controller) 
            // key is pressed, the pause screen is set to active and the timescale 
            // is set to zero. When pressed again, the opposite happens.
            // The button for confirmation on exiting to the main menu is set 
            // as inactive in both cases.
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.JoystickButton7))
            {
                if (pauseBackground.gameObject.activeInHierarchy == false)
                {
                    pauseBackground.gameObject.SetActive(true);
                    confirmButton.SetActive(false);
                    Time.timeScale = 0;
                    isPaused = true;
                    es.SetSelectedGameObject(null);
                    es.SetSelectedGameObject(es.firstSelectedGameObject);
                }
                else
                {
                    confirmButton.SetActive(false);
                    pauseBackground.gameObject.SetActive(false);
                    Time.timeScale = 1;
                    isPaused = false;
                }
            }
        }
    }
}
