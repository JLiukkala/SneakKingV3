using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invector
{
    public class GameUI : MonoBehaviour
    {
        // This determines which scene is loaded after restarting.
        [SerializeField]
        private string loadLevel;

        // A static boolean that sets the game to be over.
        public static bool _losingStatement = false;

        // A boolean for whether the game is over or not.
        bool gameIsOver;

        void Start()
        {
            // Losing statement is set to false in the beginning.
            _losingStatement = false;
        }

        void Update()
        {
            // If the losing statement is set to true, shows the game lose UI.
            if (_losingStatement)
            {
                ShowGameLoseUI();
            }

            // If the game is over, time scale is set to 0 and the
            // player must press Spacebar to start the scene again.  
            // The boolean variables are set to false and time 
            // scale is brought back up to 1.
            if (gameIsOver)
            {
                Time.timeScale = 0f;

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
                {
                    _losingStatement = false;
                    SceneManager.LoadScene(loadLevel);
                    
                    gameIsOver = false;

                    Time.timeScale = 1;
                }
            }
        }

        // Sets the children objects of the GameLose game object 
        // to be active. Also sets gameIsOver to be true.
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
