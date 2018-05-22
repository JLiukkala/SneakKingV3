using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Invector
{
    public class FinalScene : MonoBehaviour
    {
        #region Variables
        public GameObject pageInteraction;
        public RawImage background;
        public Animator anim;

        GameObject player;
        Invector.CharacterController.vThirdPersonController cc;

        GameObject cam;
        vThirdPersonCamera camScript;

        public GameObject uISceneCanvas;
        PauseGame pauseGameScript;

        public GameObject pauseMenu;
        public GameObject confirmMenu;

        public GameObject theEndButton;
        public GameObject yesButton;

        public GameObject doorOpenSound;
        public GameObject cueSound;
        public GameObject musicBoxSound;

        EventSystem es;

        bool isInsideCollider = false;

        private float time;
        private float waitTimeOne = 7;
        private float waitTimeTwo = 8.5f;
        private float waitTimeThree = 10.5f;
        private float waitTimeFour = 12;
        #endregion

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            cc = player.GetComponent<Invector.CharacterController.vThirdPersonController>();

            cam = GameObject.FindGameObjectWithTag("MainCamera");
            camScript = cam.GetComponent<vThirdPersonCamera>();

            pauseGameScript = uISceneCanvas.GetComponent<PauseGame>();

            es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        void Update()
        {
            if (isInsideCollider)
            {
                time += Time.deltaTime;
            }

            if (time >= waitTimeOne)
            {
                doorOpenSound.SetActive(true);
            }

            if (time >= waitTimeTwo)
            {
                cueSound.SetActive(true);
            }

            if (time >= waitTimeThree)
            {
                musicBoxSound.SetActive(true);
            }

            if (time >= waitTimeFour)
            {
                time = 0;
                // A button appears at the bottom of the page and it is set as the 
                // selected game object on the Event System (the button becomes highlighted).
                theEndButton.SetActive(true);
                es.SetSelectedGameObject(yesButton);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Upon entering the page collider, the page UI object is set
            // to active while the rest of the screen fades to black. 
            // Movement for the player and the camera locks. 
            // Objects related to pausing the game are removed from the scene.
            isInsideCollider = true;

            pageInteraction.SetActive(true);
            anim.SetBool("Fade", true);
            cc.lockMovement = true;
            camScript.lockCamera = true;

            Destroy(pauseMenu.gameObject);
            Destroy(confirmMenu.gameObject);
            Destroy(pauseGameScript);
        }
    }
}
