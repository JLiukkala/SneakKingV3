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

        EventSystem es;

        bool isInsideCollider = false;

        private float time;
        private float waitTimeOne = 7;
        private float waitTimeTwo = 10;

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
                time = 0;
                theEndButton.SetActive(true);
                es.SetSelectedGameObject(yesButton);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            pageInteraction.SetActive(true);
            anim.SetBool("Fade", true);
            cc.lockMovement = true;
            camScript.lockCamera = true;

            Destroy(pauseMenu.gameObject);
            Destroy(confirmMenu.gameObject);
            Destroy(pauseGameScript);

            isInsideCollider = true;
        }
    }
}
