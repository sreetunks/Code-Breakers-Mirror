using UnityEngine;

public class MenuButtonFunctions : MonoBehaviour
{
    // Button to load the last saved game
    public void Continue()
    {
        GameManager.Instance.LoadGameScene();
    }

    // Button to start a new game
    public void NewGame()
    {
        GameManager.Instance.ToggleSaveMenu();
    }

    // Button to load a save file
    public void LoadGame()
    {
        GameManager.Instance.ToggleSaveMenu();
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
        GameManager.Instance.saveMenu.SetActive(false);
        GameManager.Instance.settingsMenu.SetActive(false);
        GameManager.Instance.creditsMenu.SetActive(false);
        GameManager.Instance.exitMenu.SetActive(false);

        if (GameManager.Instance.SoundManager.settingsTestMusic.mute == false)
        {
            GameManager.Instance.SoundManager.ToggleMusic();
        }

        if (GameManager.Instance.SoundManager.settingsTestSFX.mute == false)
        {
            GameManager.Instance.SoundManager.ToggleEffects();
        }

        if (GameManager.Instance.settingsMenu.activeInHierarchy == false && GameManager.Instance.SoundManager.mainMenuTheme.isPlaying != true)
        {
            GameManager.Instance.SoundManager.mainMenuTheme.Play();
        }
    }

    // Button to confirm exiting the application
    public void Yes()
    {
        StartCoroutine(GameManager.Instance.MenuSoundEffect());
        Application.Quit();
    }
}