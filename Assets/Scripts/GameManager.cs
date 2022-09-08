using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AudioSource mainMenuTheme;
    public AudioSource mainMenuSFX;

    public GameObject saveMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject exitMenu;

    public bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleSaveMenu()
    {
        saveMenu.SetActive(!saveMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
        mainMenuTheme.Stop();
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleCreditsMenu()
    {
        creditsMenu.SetActive(!creditsMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleExitMenu()
    {
        exitMenu.SetActive(!exitMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public IEnumerator MenuSoundEffect()
    {
        mainMenuSFX.mute = false;
        mainMenuSFX.Play();
        yield return new WaitForSeconds(0.25f);
        mainMenuSFX.mute = true;
        mainMenuSFX.Stop();
    }
}