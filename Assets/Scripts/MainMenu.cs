using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string loadLevel;

    public bool isInEndMenu;

    void Start ()
    {
        if (isInEndMenu)
        {
#if !UNITY_EDITOR
                Cursor.visible = true;
#endif
        }
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene(3);
    }

    public void ToCredits ()
    {
        SceneManager.LoadScene(loadLevel);
    }

    public void ToMainMenu ()
    {
        SceneManager.LoadScene(0);
    }

    public void ToOptionsMenu ()
    {
        SceneManager.LoadScene(1);
    }

    public void ToControlsMenu ()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame ()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
