using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Invector
{
    public class PauseGame : MonoBehaviour
    {
        // The transform of the pause screen.
        public Transform pauseBackground;

        EventSystem es;

        public GameObject confirmButton;

        //GameObject player;
        //Invector.CharacterController.vThirdPersonController cc;

        [HideInInspector]
        public bool isPaused = false;

        void Start()
        {
            es = GameObject.Find("EventSystem").GetComponent<EventSystem>();

            //player = GameObject.FindGameObjectWithTag("Player");
            //cc = player.GetComponent<Invector.CharacterController.vThirdPersonController>();
        }

        void Update()
        {
            // If the Tab key is pressed, the pause screen is 
            // set to active and the timescale is set to zero.
            // When Tab is pressed again, the opposite happens.

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
                    //cc.lockMovement = true;
                }
                else
                {
                    confirmButton.SetActive(false);
                    pauseBackground.gameObject.SetActive(false);
                    Time.timeScale = 1;
                    isPaused = false;

                    //Cursor.visible = false;
                    //Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }
}
