using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invector.CharacterController
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField]
        private string loadLevel;

        // Reference to the UI object.
        GameObject gameLoseUI;

        // A boolean for whether the game is over or not.
        bool gameIsOver;

        GameObject movingEnemy;

        GameObject notMovingEnemy;

        int enemyCount;

        // Subscribing the ShowGameLoseUI method to the event 
        // OnEnemyHasSpottedPlayer located in the Enemy script.
        void Start()
        {
            gameLoseUI = GameObject.Find("GameLose");
            Debug.Log("GameUI Start");
            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            Debug.Log(enemyCount);
            if (GameObject.Find("EnemyMoving"))
            {
                movingEnemy = GameObject.Find("EnemyMoving");
            }
            if (GameObject.Find("EnemyNotMoving"))
            {
                notMovingEnemy = GameObject.Find("EnemyNotMoving");
            }

            //this.gameObject.SetActive(true);
            //Enemy.OnEnemyHasSpottedPlayer += ShowGameLoseUI;
        }

        void Update()
        {
            if (enemyCount == 1 && movingEnemy)
            {
                if (movingEnemy.GetComponent<Enemy>().spotted == true)
                {
                    ShowGameLoseUI();
                }
            }
            else if (enemyCount == 1 && notMovingEnemy)
            {
                if (notMovingEnemy.GetComponent<Enemy>().spotted == true)
                {
                    ShowGameLoseUI();
                }
            }

            //if (enemyCount == 2 && movingEnemy)
            //{
            //    if (movingEnemy.GetComponent<Enemy>().spotted == true)
            //    {
            //        ShowGameLoseUI();
            //    }
            //}
            //else if (enemyCount == 2 && notMovingEnemy)
            //{ 
            //    if (notMovingEnemy.GetComponent<Enemy>().spotted == true)
            //    {
            //        ShowGameLoseUI();
            //    }
            //}

            if (enemyCount == 2 && movingEnemy)
            {
                if (enemyCount == 2 && notMovingEnemy)
                {
                    if (notMovingEnemy.GetComponent<Enemy>().spotted == true)
                    {
                        ShowGameLoseUI();
                    }
                }

                if (movingEnemy.GetComponent<Enemy>().spotted == true)
                {
                    ShowGameLoseUI();
                }
            }

            // If the game is over, the player must press 
            // Spacebar to start again. The boolean variable
            // gameIsOver is set to false and timescale is brought
            // back up to 1. All the game objects are destroyed.
            if (gameIsOver)
            {
                Time.timeScale = 0.8f;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene(loadLevel);

                    gameIsOver = false;

                    Time.timeScale = 1;

                    //DestroyAllGameObjects();
                }
            }
        }

        // Shows the GameLoseUI, sets the game to be over
        // and unsubscribes the method from OnEnemyHasSpottedPlayer.
        void ShowGameLoseUI()
        {
            //gameLoseUI.SetActive(true);
            //this.gameObject.SetActive(true);
            Debug.Log("ShowGameLoseUI");

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            
            gameIsOver = true;
            //Enemy.OnEnemyHasSpottedPlayer -= ShowGameLoseUI;
        }

        // A method that destroys all game objects in the scene.
        //public void DestroyAllGameObjects()
        //{
        //    GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);

        //    for (int i = 0; i < GameObjects.Length; i++)
        //    {
        //        Destroy(GameObjects[i]);
        //    }
        //}
    }
}
