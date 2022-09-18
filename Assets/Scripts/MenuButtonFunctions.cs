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

    // Button to open the credits screen
    public void Credits()
    {
        GameManager.Instance.ToggleCreditsMenu();
    }

    // Button to close the application
    public void Exit()
    {
        GameManager.Instance.ToggleExitMenu();
    }

    // Button to select a save file
    public void Select()
    {
        StartCoroutine(GameManager.Instance.MenuSoundEffect());
    }

    // Button to back out of a menu
    public void Back()
    {
        GameManager.Instance.mainMenuSfx.mute = false;
        GameManager.Instance.mainMenuSfx.Play();

        GameManager.Instance.saveMenu.SetActive(false);
        GameManager.Instance.creditsMenu.SetActive(false);
        GameManager.Instance.exitMenu.SetActive(false);

        if (GameManager.Instance.SoundManager.effectSource.mute == false)
        {
            GameManager.Instance.SoundManager.ToggleEffects();
        }

        GameManager.Instance.mainMenuSfx.mute = true;
        GameManager.Instance.mainMenuSfx.Stop();
    }

    // Button to confirm exiting the application
    public void Yes()
    {
        StartCoroutine(GameManager.Instance.MenuSoundEffect());
        Application.Quit();
    }
}