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
        GameManager.Instance.mainMenuSFX.mute = false;
        GameManager.Instance.mainMenuSFX.Play();

        GameManager.Instance.saveMenu.SetActive(false);
        GameManager.Instance.settingsMenu.SetActive(false);
        GameManager.Instance.creditsMenu.SetActive(false);
        GameManager.Instance.exitMenu.SetActive(false);

        if (SoundManager.Instance._musicSource.mute == false)
        {
            SoundManager.Instance.ToggleMusic();
        }

        if (SoundManager.Instance._effectSource.mute == false)
        {
            SoundManager.Instance.ToggleEffects();
        }

        if (GameManager.Instance.settingsMenu.activeInHierarchy == false && GameManager.Instance.mainMenuTheme.isPlaying != true)
        {
            GameManager.Instance.mainMenuTheme.Play();
        }

        GameManager.Instance.mainMenuSFX.mute = true;
        GameManager.Instance.mainMenuSFX.Stop();
    }

    // Button to confirm exiting the application
    public void Yes()
    {
        StartCoroutine(GameManager.Instance.MenuSoundEffect());
        Application.Quit();
    }
}