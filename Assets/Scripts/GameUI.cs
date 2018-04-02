using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invector.CharacterController
{
    public class GameUI : MonoBehaviour
    {
        //[SerializeField]
        //private string loadLevel;

        // Reference to the UI object.
        public GameObject gameLoseUI;

        // A boolean for whether the game is over or not.
        bool gameIsOver;

        // Subscribing the ShowGameLoseUI method to the event 
        // OnEnemyHasSpottedPlayer located in the Enemy script.
        void Start()
        {
            Enemy.OnEnemyHasSpottedPlayer += ShowGameLoseUI;
        }

        void Update()
        {
            // If the game is over, the player must press 
            // Spacebar to start again. The boolean variable
            // gameIsOver is set to false and timescale is brought
            // back up to 1. All the game objects are destroyed.
            if (gameIsOver)
            {
                Time.timeScale = 0.8f;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene(0);

                    gameIsOver = false;

                    Time.timeScale = 1;

                    DestroyAllGameObjects();
                }
            }
        }

        // Shows the GameLoseUI, sets the game to be over
        // and unsubscribes the method from OnEnemyHasSpottedPlayer.
        void ShowGameLoseUI()
        {
            gameLoseUI.SetActive(true);
            gameIsOver = true;
            Enemy.OnEnemyHasSpottedPlayer -= ShowGameLoseUI;
        }

        // A method that destroys all game objects in the scene.
        public void DestroyAllGameObjects()
        {
            GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);

            for (int i = 0; i < GameObjects.Length; i++)
            {
                Destroy(GameObjects[i]);
            }
        }
    }
}
