using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invector
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField]
        private string loadLevel;

        public static bool _losingStatement = false;

        // Reference to the UI object.
        GameObject gameLoseUI;

        // A boolean for whether the game is over or not.
        bool gameIsOver;

        GameObject movingEnemy;

        GameObject notMovingEnemy;

        void Start()
        {
            _losingStatement = false;
            gameLoseUI = GameObject.Find("GameLose");

            if (GameObject.Find("EnemyMoving"))
            {
                movingEnemy = GameObject.Find("EnemyMoving");
            }
            if (GameObject.Find("EnemyNotMoving"))
            {
                notMovingEnemy = GameObject.Find("EnemyNotMoving");
            }
        }

        void Update()
        {
            if (_losingStatement)
            {
                ShowGameLoseUI();
            }

            // If the game is over, the player must press 
            // Spacebar to start again. The boolean variable
            // gameIsOver is set to false and timescale is brought
            // back up to 1. All the game objects are destroyed.
            if (gameIsOver)
            {
                Time.timeScale = 0f;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _losingStatement = false;
                    SceneManager.LoadScene(loadLevel);
                    
                    gameIsOver = false;

                    Time.timeScale = 1;
                }
            }
        }

        void ShowGameLoseUI()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            
            gameIsOver = true;
        }
    }
}
