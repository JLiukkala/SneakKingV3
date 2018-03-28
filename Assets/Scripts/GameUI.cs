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

        public GameObject gameLoseUI;
        bool gameIsOver;

        void Start()
        {
            Enemy.OnEnemyHasSpottedPlayer += ShowGameLoseUI;
        }

        void Update()
        {
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

        void ShowGameLoseUI()
        {
            gameLoseUI.SetActive(true);
            gameIsOver = true;
            Enemy.OnEnemyHasSpottedPlayer -= ShowGameLoseUI;
        }

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
