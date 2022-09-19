using UnityEngine;

public class MenuButtonFunctions : MonoBehaviour
{
    // Button to load the last saved game
    public void Continue()
    {
        GameManager.Instance.LoadSavedGameScene();
    }

    // Button to start a new game
    public void NewGame()
    {
        GameManager.Instance.LoadNewGame();
    }

    // Button to open the settings menu
    public void Settings()
    {
        GameManager.Instance.ToggleSettingsMenu();
    }

    // Button to close the application
    public void Exit()
    {
        GameManager.Instance.ToggleExitMenu();
    }

    // Button to back out of a menu
    public void Back()
    {
        GameManager.Instance.exitMenu.SetActive(false);
    }

    // Button to confirm exiting the application
    public void Yes()
    {
        Application.Quit();
    }
}