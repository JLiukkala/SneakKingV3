using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private string loadLevel;

    [SerializeField]
    private string currentRoom;

    public bool isInEndMenu;
    public bool notInMainMenu;

    public GameObject confirmButton;
    public GameObject pauseMenu;
    public GameObject yesButton;

    EventSystem es;
    #endregion

    void Start ()
    {
        if (isInEndMenu)
        {
#if !UNITY_EDITOR
                Cursor.visible = true;
#endif
        }

        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
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
        Time.timeScale = 1;
    }

    public void ToOptionsMenu ()
    {
        SceneManager.LoadScene(1);
    }

    public void ToControlsMenu ()
    {
        SceneManager.LoadScene(2);
    }

    public void RestartCurrentRoom ()
    {
        SceneManager.LoadScene(currentRoom);
        Time.timeScale = 1;
    }

    public void ExitGame ()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }

    /// <summary>
    /// Shows the confirmation window for exiting to the main menu.
    /// Highlights the 'Yes' button.
    /// </summary>
    public void ShowConfirmMessage ()
    {
        if (notInMainMenu)
        {
            pauseMenu.SetActive(false);
            confirmButton.SetActive(true);
            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(yesButton);
        }
    }

    /// <summary>
    /// Returns to the pause menu window after pressing 'No'
    /// on the confirm window. Highlights the first button. 
    /// </summary>
    public void ChooseNo ()
    {
        if (notInMainMenu)
        {
            pauseMenu.SetActive(true);
            confirmButton.SetActive(false);
            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(es.firstSelectedGameObject);
        }
    }
}
